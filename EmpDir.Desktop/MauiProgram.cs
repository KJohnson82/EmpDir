using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.LifecycleEvents;
using EmpDir.Core.Services;
using EmpDir.Desktop.Data;
using EmpDir.Desktop.Services;
using Microsoft.Extensions.Logging;
using EmpDir.Desktop.Layout;
using Microsoft.Data.Sqlite;
using SQLitePCL;


#if WINDOWS
using Microsoft.UI.Windowing;
using WinRT.Interop;
using Microsoft.UI;
#endif

namespace EmpDir.Desktop;

public static class MauiProgram
{
    // Store the database path as a static field so it's consistent throughout the app
    private static string _databasePath = null!;
    private static string _connectionString = null!;

    public static MauiApp CreateMauiApp()
    {
        // ===== STEP 1: Initialize SQLite provider FIRST =====
        SQLitePCL.Batteries_V2.Init();
        //SQLitePCL.raw.SetProvider(new SQLite3Provider_e_sqlite3());

        // ===== STEP 2: Setup database path and CREATE the empty database file =====
        // This MUST happen before any EF Core registration
        SetupDatabase();

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
                            overlappedPresenter.IsResizable = true;
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
        var timeoutSeconds = int.Parse(builder.Configuration["ApiSettings:TimeoutSeconds"] ?? "5");

        //builder.Services.AddHttpClient("EmpDirApi", client =>
        //{
        //    client.BaseAddress = new Uri(apiBaseUrl);
        //    client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
        //    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        //});

        builder.Services.AddHttpClient("EmpDirApi", client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        })
.ConfigurePrimaryHttpMessageHandler(() =>
{
    // Configure the handler for better connection management
    return new SocketsHttpHandler
    {
        // Don't keep connections alive forever - helps prevent stale connection issues
        PooledConnectionLifetime = TimeSpan.FromMinutes(2),

        // How long to keep an idle connection in the pool
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),

        // Connection timeout for establishing new connections
        ConnectTimeout = TimeSpan.FromSeconds(10),

        // Enable TCP keep-alive to detect dead connections
        KeepAlivePingPolicy = HttpKeepAlivePingPolicy.WithActiveRequests,
        KeepAlivePingDelay = TimeSpan.FromSeconds(30),
        KeepAlivePingTimeout = TimeSpan.FromSeconds(10),

        // Max connections per server
        MaxConnectionsPerServer = 10
    };
});

        // ===== STEP 3: Register DbContext with the connection string =====
        // The database file already exists at this point
        builder.Services.AddDbContext<LocalCacheContext>(options =>
        {
            options.UseSqlite(_connectionString);

#if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
#endif
        }, ServiceLifetime.Scoped);

        // ===== SERVICE REGISTRATION =====
        builder.Services.AddScoped<IApiService, ApiService>();
        builder.Services.AddScoped<ICacheService, CacheService>();
        builder.Services.AddScoped<ISyncService, SyncService>();
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

        // ===== STEP 4: Initialize the database schema =====
        InitializeDatabaseSchema(app.Services);

        return app;
    }

    private static void SetupDatabase()
    {
        string basePath = GetDatabaseDirectory();

        _databasePath = Path.Combine(basePath, "empdir_cache.db");
        _connectionString = $"Data Source={_databasePath};Pooling=False";

        System.Diagnostics.Debug.WriteLine($"=== Database Setup ===");
        System.Diagnostics.Debug.WriteLine($"Base path: {basePath}");
        System.Diagnostics.Debug.WriteLine($"Database path: {_databasePath}");

        // CRITICAL: Ensure directory exists
        try
        {
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
                System.Diagnostics.Debug.WriteLine($"✓ Created directory: {basePath}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"✓ Directory already exists: {basePath}");
            }

            // Verify we can write to this directory
            var testFile = Path.Combine(basePath, "test.tmp");
            File.WriteAllText(testFile, "test");
            File.Delete(testFile);
            System.Diagnostics.Debug.WriteLine($"✓ Directory is writable");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"✗✗✗ FATAL: Cannot create/write to directory: {ex.Message}");
            throw new InvalidOperationException($"Cannot create database directory at {basePath}", ex);
        }

        // Check if database file exists
        if (File.Exists(_databasePath))
        {
            System.Diagnostics.Debug.WriteLine($"✓ Database file already exists: {_databasePath}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Creating new database file...");
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT sqlite_version();";
                var version = command.ExecuteScalar();

                System.Diagnostics.Debug.WriteLine($"✓ SQLite version: {version}");
                System.Diagnostics.Debug.WriteLine($"✓ Database file created: {_databasePath}");

                connection.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"✗✗✗ FATAL: Failed to create database: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw new InvalidOperationException($"Failed to create database at {_databasePath}", ex);
            }
        }

        // REMOVE THIS LINE - It's causing the crash
        // SqliteConnection.ClearAllPools();

        System.Diagnostics.Debug.WriteLine($"=== Database Setup Complete ===");
    }

    /// <summary>
    /// Gets the appropriate database directory based on the platform.
    /// </summary>
    private static string GetDatabaseDirectory()
    {

        // For development, use a simple, reliable path
        var basePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EmpDir"
        );

        System.Diagnostics.Debug.WriteLine($"Using database directory: {basePath}");
        return basePath;
        //try
        //{
        //    // For MSIX packaged apps, use the WinRT ApplicationData API
        //    // This provides the correct virtualized LocalState folder
        //    var localFolder = ApplicationData.Current.LocalFolder.Path;
        //    System.Diagnostics.Debug.WriteLine($"Using Windows ApplicationData.LocalFolder: {localFolder}");
        //    return localFolder;
        //}
        //catch (Exception ex)
        //{
        //    System.Diagnostics.Debug.WriteLine($"ApplicationData.Current.LocalFolder failed: {ex.Message}");

        //    // Fallback for unpackaged Windows apps
        //    var fallbackPath = Path.Combine(
        //        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        //        "EmpDir");
        //    System.Diagnostics.Debug.WriteLine($"Using fallback path: {fallbackPath}");
        //    return fallbackPath;
        //}
    }

    private static IConfiguration LoadConfiguration()
    {
        var configBuilder = new ConfigurationBuilder();
        var assembly = typeof(MauiProgram).Assembly;

        var stream = assembly.GetManifestResourceStream("EmpDir.Desktop.appsettings.json");
        if (stream != null)
        {
            configBuilder.AddJsonStream(stream);
        }

#if DEBUG
        var devStream = assembly.GetManifestResourceStream("EmpDir.Desktop.appsettings.Development.json");
        if (devStream != null)
        {
            configBuilder.AddJsonStream(devStream);
        }
#else
        var prodStream = assembly.GetManifestResourceStream("EmpDir.Desktop.appsettings.Production.json");
        if (prodStream != null)
        {
            configBuilder.AddJsonStream(prodStream);
        }
#endif

        return configBuilder.Build();
    }

    /// <summary>
    /// Initializes the database schema using EF Core.
    /// Called after the app is built and services are registered.
    /// </summary>
    private static void InitializeDatabaseSchema(IServiceProvider services)
    {
        System.Diagnostics.Debug.WriteLine("=== Initializing Database Schema ===");

        try
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<LocalCacheContext>();

            // EnsureCreated will create the tables if they don't exist
            // Since we already created the database file in SetupDatabase(),
            // this should just add the tables
            context.Database.EnsureCreated();

            System.Diagnostics.Debug.WriteLine("Database schema initialized successfully");
            Console.WriteLine($"✓ Cache database ready at: {_databasePath}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ERROR initializing database schema: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            Console.WriteLine($"✗ Database initialization failed: {ex.Message}");

#if DEBUG
            throw;
#endif
        }
    }
}