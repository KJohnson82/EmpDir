using EmpDir.Desktop.Layout;
using EmpDir.UI.Layout;
using static EmpDir.UI.Components.FooterNav;


namespace EmpDir.UI.Services
{
    /// <summary>
    /// Service class that manages the global layout state for the application.
    /// This includes header and footer component data that can be shared across different pages/components.
    /// Uses the Observer pattern to notify subscribers when layout state changes.
    /// </summary>
    public class LayoutState
    {
        #region Properties

        /// <summary>
        /// Stores the current header information model.
        /// Private setter ensures state can only be modified through controlled methods.
        /// Nullable to handle cases where no header info is set.
        /// </summary>
        public HeaderCardModels.HeadInfoCardModel? HeaderInfo { get; private set; }

        /// <summary>
        /// Collection of footer navigation items.
        /// Initialized as empty list to prevent null reference exceptions.
        /// Private setter maintains encapsulation - use SetFooter() to modify.
        /// </summary>
        public List<FooterItem> FooterItems { get; private set; } = new();

        /// <summary>
        /// Event that fires when any layout state changes occur.
        /// Components can subscribe to this to react to layout updates.
        /// Uses Action delegate for simplicity (no parameters needed).
        /// </summary>
        public event Action? OnChange;

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the header information and notifies subscribers if the data has changed.
        /// Includes change detection to prevent unnecessary notifications and re-renders.
        /// </summary>
        /// <param name="info">The new header info model, or null to clear the header</param>
        public void SetHeader(HeaderCardModels.HeadInfoCardModel? info)
        {
            // Only notify listeners if the data has actually changed.
            // This prevents unnecessary re-renders and event firing
            if (HeaderInfo != info)
            {
                HeaderInfo = info;
                NotifyStateChanged();
            }
        }

        /// <summary>
        /// Updates the footer items collection and notifies all subscribers.
        /// Always triggers notification since List comparison would be more complex.
        /// </summary>
        /// <param name="items">New collection of footer navigation items</param>
        public void SetFooter(List<FooterItem> items)
        {
            FooterItems = items;
            NotifyStateChanged();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Triggers the OnChange event to notify all subscribers that the layout state has been modified.
        /// Uses null-conditional operator (?.) to safely invoke event even if no subscribers exist.
        /// </summary>
        private void NotifyStateChanged() => OnChange?.Invoke();

        #endregion
    }
}