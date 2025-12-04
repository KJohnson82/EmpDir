using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.LifecycleEvents;
using EmpDir.Core.Services;
using EmpDir.Desktop.Data;
using EmpDir.Desktop.Services;
using EmpDir.UI.Services;

using Microsoft.Extensions.Logging;


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
        var localDbPath = Path.Combine(FileSystem.AppDataDirectory, "empdir_cache.db");

        builder.Services.AddDbContext<LocalCacheContext>(options =>
        {
            options.UseSqlite($"Data Source={localDbPath}");

#if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
#endif
        });

        // ===== SERVICE REGISTRATION =====
        // Desktop-specific services
        builder.Services.AddScoped<IApiService, ApiService>();
        builder.Services.AddScoped<ICacheService, CacheService>();
        builder.Services.AddScoped<ISyncService, SyncService>();

        // IDirectoryService implementation using cache
        builder.Services.AddScoped<IDirectoryService, CachedDirectoryService>();

        // UI services (if using EmpDir.UI components)
        builder.Services.AddTelerikBlazor();
        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddSingleton<LayoutState>();

        // Add SearchService if needed
        // builder.Services.AddScoped<SearchService>();

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

        // Load base settings
        using (var stream = assembly.GetManifestResourceStream("EmpDir.Desktop.appsettings.json"))
        {
            if (stream != null)
            {
                configBuilder.AddJsonStream(stream);
            }
        }

#if !DEBUG
        // In Release mode, load production settings
        using (var prodStream = assembly.GetManifestResourceStream("EmpDir.Desktop.appsettings.Production.json"))
        {
            if (prodStream != null)
            {
                configBuilder.AddJsonStream(prodStream);
            }
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
            context.Database.EnsureCreated();

#if DEBUG
            var dbPath = context.Database.GetConnectionString();
            Console.WriteLine($"✓ Cache database initialized at: {dbPath}");
#endif
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"✗ Cache database initialization failed: {ex.Message}");
#endif
        }
    }
}