using EmpDir.Core.Services;

namespace EmpDir.Desktop
{
    public partial class App : Application
    {
        private readonly ISyncService _syncService;

        public App(ISyncService syncService)
        {
            InitializeComponent();
            _syncService = syncService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var appWindow = new Window(new MainPage())
            {
                Title = "McElroy Directory",
                FlowDirection = FlowDirection.MatchParent,
                TitleBar = new TitleBar
                {
                    Title = "McElroy Directory",
                    Background = Colors.Transparent,
                    ForegroundColor = Colors.Coral

                }
            };

            // Load and apply saved window state
            var windowState = SavedWindowState.Load();
            windowState.ApplyToWindow(appWindow);

            // Save window state on position/size changes
            appWindow.SizeChanged += (_, _) =>
            {
                Task.Delay(500).ContinueWith(_ =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        new SavedWindowState(appWindow).Save();
                    });
                });
            };

            return appWindow;
        }

        // Sync on app launch
        protected override void OnStart()
        {
            base.OnStart();


            // Perform sync in background on app launch
            _ = Task.Run(async () =>
            {
                try
                {
                    var result = await _syncService.SyncOnLaunchAsync();

                    // Log the result
                    if (result.Success)
                    {
                        System.Diagnostics.Debug.WriteLine($"✓ Sync successful: {result.Message}");

                        if (result.ApiWasAvailable)
                        {
                            System.Diagnostics.Debug.WriteLine($"  - Employees: {result.EmployeesUpdated}");
                            System.Diagnostics.Debug.WriteLine($"  - Departments: {result.DepartmentsUpdated}");
                            System.Diagnostics.Debug.WriteLine($"  - Locations: {result.LocationsUpdated}");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"✗ Sync failed: {result.Message}");
                    }

                    // Optionally show a toast notification to the user
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        // You can add UI notification here if desired
                        // For example, update a status bar or show a brief toast
                    });
                }
                catch (System.IO.IOException ioEx)
                {
                    System.Diagnostics.Debug.WriteLine($"✗ Network I/O error during sync: {ioEx.Message}");
                    System.Diagnostics.Debug.WriteLine("App will continue with cached data");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"✗ Sync error: {ex.Message}");
                }
            });
        }
    }

    // Window state persistence using JSON file
    public class SavedWindowState
    {
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }

        // Default dimensions for first launch
        private const double DefaultWidth = 375;
        private const double DefaultHeight = 874;
        private const double DefaultMaxWidth = 425;
        private const double DefaultMaxHeight = 900;
        private const double DefaultMinWidth = 372;
        private const double DefaultMinHeight = 826;

        // File path for storing window state
        private static string SettingsFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EmpDir",
            "window_state.json"
        );

        // Constructor for saving current window state
        public SavedWindowState(Window window)
        {
            X = window.X;
            Y = window.Y;
            Width = window.Width;
            Height = window.Height;
        }

        // Parameterless constructor for deserialization - DO NOT load here!
        public SavedWindowState()
        {
            // Empty - properties will be set by deserializer
        }


        // Static method to load saved state
        public static SavedWindowState Load()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    var json = File.ReadAllText(SettingsFilePath);
                    var loaded = System.Text.Json.JsonSerializer.Deserialize<SavedWindowState>(json);

                    if (loaded != null)
                    {
                        return loaded;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading window state: {ex.Message}");
            }

            return new SavedWindowState(); // Return empty state if load fails
        }

        public void ApplyToWindow(Window window)
        {
            // Set default constraints first
            window.MinimumWidth = DefaultMinWidth;
            window.MinimumHeight = DefaultMinHeight;
            window.MaximumWidth = DefaultMaxWidth;
            window.MaximumHeight = DefaultMaxHeight;

            // Apply saved dimensions or defaults
            window.Width = Width ?? DefaultWidth;
            window.Height = Height ?? DefaultHeight;

            // Apply position if saved (with basic validation)
            if (X.HasValue && Y.HasValue)
            {
                // Basic validation to prevent completely off-screen windows
                var screenWidth = DeviceDisplay.Current.MainDisplayInfo.Width / DeviceDisplay.Current.MainDisplayInfo.Density;
                var screenHeight = DeviceDisplay.Current.MainDisplayInfo.Height / DeviceDisplay.Current.MainDisplayInfo.Density;

                // Ensure at least part of the window is visible
                if (X.Value < screenWidth - 100 && Y.Value < screenHeight - 100 &&
                    X.Value > -window.Width + 100 && Y.Value > -50)
                {
                    window.X = X.Value;
                    window.Y = Y.Value;
                }
                else
                {
                    // Center on screen if saved position is problematic
                    CenterWindow(window);
                }
            }
            else
            {
                // Center on screen for first launch
                CenterWindow(window);
            }

            System.Diagnostics.Debug.WriteLine($"Applied window state: {window.Width}x{window.Height} at ({window.X}, {window.Y})");
        }

        private void CenterWindow(Window window)
        {
            try
            {
                var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
                var screenWidth = displayInfo.Width / displayInfo.Density;
                var screenHeight = displayInfo.Height / displayInfo.Density;

                window.X = (screenWidth - window.Width) / 2;
                window.Y = (screenHeight - window.Height) / 2;
            }
            catch
            {
                // Fallback positioning
                window.X = 100;
                window.Y = 100;
            }
        }

        public void Save()
        {
            try
            {
                var directory = Path.GetDirectoryName(SettingsFilePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = System.Text.Json.JsonSerializer.Serialize(this, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(SettingsFilePath, json);

                System.Diagnostics.Debug.WriteLine($"Saved window state: {Width}x{Height} at ({X}, {Y})");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving window state: {ex.Message}");
            }
        }
    }
}