using System;
using System.Collections.ObjectModel;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Event arguments for RadListView's IsCheckModeActiveChanged event.
    /// </summary>
    public class CheckModeActiveChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckModeActiveChangedEventArgs"/> class.
        /// </summary>
        /// <param name="isCheckModeActive">True if the check mode is about to be activated. False otherwise.</param>
        /// <param name="tappedItem">The data item from the source collection that has been tapped to activate the check mode.</param>
        /// <param name="selectedItems">The data items from the source collection that have been selected.</param>
        public CheckModeActiveChangedEventArgs(bool isCheckModeActive, object tappedItem, ObservableCollection<object> selectedItems)
        {
            this.IsCheckModeActive = isCheckModeActive;
            this.TappedItem = tappedItem;
            this.SelectedItems = selectedItems;
        }

        /// <summary>
        /// Gets a reference to the data item from the source collection
        /// which has been initially tapped to activate the check mode.
        /// This property will return null when the change is not a result
        /// from end-user input.
        /// </summary>
        public object TappedItem
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the data items from the source collection that have been selected.
        /// </summary>
        public ObservableCollection<object> SelectedItems
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether check mode is on. True if the check mode is activated. False otherwise.
        /// </summary>
        public bool IsCheckModeActive
        {
            get;
            private set;
        }
    }
}
