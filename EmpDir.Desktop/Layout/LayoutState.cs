using EmpDir.UI.Layout;
using static EmpDir.Desktop.Components.FooterNav;

namespace EmpDir.Desktop.Layout
{
    // Manages global layout state for header and footer components
    public class LayoutState
    {
        // Current header information model
        public HeaderCardModels.HeadInfoCardModel? HeaderInfo { get; private set; }

        // Collection of footer navigation items
        public List<FooterItem> FooterItems { get; private set; } = new();

        // Event fired when layout state changes
        public event Action? OnChange;

        // Updates header information and notifies subscribers if changed
        public void SetHeader(HeaderCardModels.HeadInfoCardModel? info)
        {
            if (HeaderInfo != info)
            {
                HeaderInfo = info;
                NotifyStateChanged();
            }
        }

        // Updates footer items and notifies subscribers
        public void SetFooter(List<FooterItem> items)
        {
            FooterItems = items;
            NotifyStateChanged();
        }

        // Triggers OnChange event to notify subscribers
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}