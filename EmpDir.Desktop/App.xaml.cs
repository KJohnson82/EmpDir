namespace EmpDir.Desktop
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
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
            var windowState = new SavedWindowState();
            windowState.ApplyToWindow(appWindow);


            // Save window state whenever it changes
            appWindow.SizeChanged += (_, _) =>
            {
                // Small delay to avoid saving during rapid resize operations
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
    }

    public class SavedWindowState
    {
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }

        // Default dimensions for first launch
        private const double DefaultWidth = 373;
        private const double DefaultHeight = 830;
        private const double DefaultMaxWidth = 425;
        private const double DefaultMaxHeight = 900;
        private const double DefaultMinWidth = 372;
        private const double DefaultMinHeight = 823;

        // Constructor for saving current window state
        public SavedWindowState(Window window)
        {
            X = window.X;
            Y = window.Y;
            Width = window.Width;
            Height = window.Height;
        }

        // Constructor for loading saved state
        public SavedWindowState()
        {
            X = GetSavedValue("WindowLocationX");
            Y = GetSavedValue("WindowLocationY");
            Width = GetSavedValue("WindowSizeWidth");
            Height = GetSavedValue("WindowSizeHeight");
        }

        public void ApplyToWindow(Window window)
        {
            // Set default constraints first
            window.MinimumWidth = DefaultMinWidth;
            window.MinimumHeight = DefaultMinHeight;
            //window.MaximumWidth = DefaultMaxWidth;
            //window.MaximumHeight = DefaultMaxHeight;

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
                SaveValue("WindowLocationX", X);
                SaveValue("WindowLocationY", Y);
                SaveValue("WindowSizeWidth", Width);
                SaveValue("WindowSizeHeight", Height);

                System.Diagnostics.Debug.WriteLine($"Saved window state: {Width}x{Height} at ({X}, {Y})");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving window state: {ex.Message}");
            }
        }

        private double? GetSavedValue(string valueKey)
        {
            try
            {
                var value = Preferences.Get(valueKey, double.NaN);
                if (!double.IsNaN(value))
                {
                    return value;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading preference {valueKey}: {ex.Message}");
            }

            return null;
        }

        private void SaveValue(string valueKey, double? value)
        {
            try
            {
                if (value != null)
                    Preferences.Set(valueKey, value.Value);
                else
                    Preferences.Remove(valueKey);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving preference {valueKey}: {ex.Message}");
            }
        }
    }
}
