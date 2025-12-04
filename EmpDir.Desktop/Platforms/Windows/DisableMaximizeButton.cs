namespace MMPD.Platforms.Windows
{
#if WINDOWS
    using Microsoft.UI;
    using Microsoft.UI.Windowing;
    using WinRT.Interop;

    public static class WindowExtensions
    {
        public static void DisableMaximizeButton(this Microsoft.Maui.Controls.Window window)
        {
            var handle = WindowNative.GetWindowHandle(window);
            var windowId = Win32Interop.GetWindowIdFromWindow(handle);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            if (appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.IsMaximizable = false;
            }
        }
    }
#endif
}