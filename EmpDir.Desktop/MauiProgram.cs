using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.LifecycleEvents;
using EmpDir.Core.Services;
using EmpDir.Desktop.Data;
using EmpDir.Desktop.Services;
using Microsoft.Extensions.Logging;
using System.IO;
using EmpDir.Desktop.Layout;




#if WINDOWS
using Microsoft.UI.Windowing;
using WinRT.Interop;
using Microsoft.UI;
#endif

namespace EmpDir.Desktop;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // Initialize SQLite for unpackaged apps
        //Batteries_V2.Init();
        SQLitePCL.Batteries_V2.Init();

        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // ===== CONFIGURATION =====
        var configuration = LoadConfiguration();
        builder.Configuration.AddConfiguration(configuration);

        // ===== PLATFORM LIFECYCLE EVENTS =====
        builder.ConfigureLifecycleEvents(events =>
        {
#if WINDOWS
            events.AddWindows(windowsLifecycleBuilder =>
            {
                windowsLifecycleBuilder.OnWindowCreated(window =>
                {
                    window.ExtendsContentIntoTitleBar = false;

                    Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
                    {
                        var handle = WindowNative.GetWindowHandle(window);
                        var id = Win32Interop.GetWindowIdFromWindow(handle);
                        var appWindow = AppWindow.GetFromWindowId(id);

                        if (appWindow?.Presenter is OverlappedPresenter overlappedPresenter)
                        {
                            overlappedPresenter.IsMaximizable = false;
                            overlappedPresenter.IsResizable = false;
                            overlappedPresenter.IsMinimizable = true;
                        }
                    });
                });
            });
#endif
        });

        // ===== HTTP CLIENT FOR API =====
        var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
        var apiKey = builder.Configuration["ApiSettings:ApiKey"] ?? "maui-app-key-2025";
        var timeoutSeconds = int.Parse(builder.Configuration["ApiSettings:TimeoutSeconds"] ?? "30");

        builder.Services.AddHttpClient("EmpDirApi", client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        });

        // ===== LOCAL CACHE DATABASE =====
        // 1. Define the path
        //var localDbPath = Path.Combine(FileSystem.AppDataDirectory, "empdir_cache.db");

        // FIX 1: Use standard AppData path instead of MAUI sandbox path
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(localAppData, "EmpDir");
        var localDbPath = Path.Combine(appFolder, "empdir_cache.db");

        // 2. CRITICAL: Ensure the directory exists
        var dbDirectory = Path.GetDirectoryName(localDbPath);
        if (!string.IsNullOrEmpty(dbDirectory) && !Directory.Exists(dbDirectory))
        {
            Directory.CreateDirectory(dbDirectory);
        }

        // 3. Register DbContext
        builder.Services.AddDbContext<LocalCacheContext>(options =>
        {
            //var connectionString = $"Data Source={localDbPath}";
            //options.UseSqlite(connectionString);
            // FIX 2: Add Pooling=False to prevent "Error 14" file locks during development
            var connectionString = $"Data Source={localDbPath};Pooling=False";
            options.UseSqlite(connectionString);

#if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
#endif
        }, ServiceLifetime.Scoped); // <--- Make sure this closes here

        // ===== SERVICE REGISTRATION =====
        // Desktop-specific services
        builder.Services.AddScoped<IApiService, ApiService>();
        builder.Services.AddScoped<ICacheService, CacheService>();
        builder.Services.AddScoped<ISyncService, SyncService>();

        // IDirectoryService implementation using cache
        builder.Services.AddScoped<IDirectoryService, CachedDirectoryService>();

        // UI services
        builder.Services.AddTelerikBlazor();
        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddSingleton<LayoutState>();
        builder.Services.AddScoped<ISearchService, CachedSearchService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        // ===== INITIALIZE CACHE DATABASE =====
        InitializeCacheDatabase(app.Services);

        return app;
    }
              

    private static IConfiguration LoadConfiguration()
    {
        var configBuilder = new ConfigurationBuilder();
        var assembly = typeof(MauiProgram).Assembly;

        // Load base appsettings.json
        var stream = assembly.GetManifestResourceStream("EmpDir.Desktop.appsettings.json");
        if (stream != null)
        {
            configBuilder.AddJsonStream(stream);
        }

#if DEBUG
        // In Debug mode, load development settings
        var devStream = assembly.GetManifestResourceStream("EmpDir.Desktop.appsettings.Development.json");
        if (devStream != null)
        {
            configBuilder.AddJsonStream(devStream);
        }
#else
        // In Release mode, load production settings
        var prodStream = assembly.GetManifestResourceStream("EmpDir.Desktop.appsettings.Production.json");
        if (prodStream != null)
        {
            configBuilder.AddJsonStream(prodStream);
        }
#endif

        return configBuilder.Build();
    }

    private static void InitializeCacheDatabase(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LocalCacheContext>();

        try
        {
            var dbPath = context.Database.GetConnectionString();
            Console.WriteLine($"DEBUG: Database path is: {dbPath}");
            var displayPath = dbPath?.Replace("Data Source=", "").Replace(";Pooling=False", "");
            Console.WriteLine($"DEBUG: Database path is: {displayPath}");

            // FIX 3: Clear pools explicitly before deleting to avoid "Error 14"
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();

            // Force recreation during development
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();


            Console.WriteLine($"✓ Cache database recreated at: {dbPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Cache database initialization failed: {ex.Message}");
        }
    }
}

