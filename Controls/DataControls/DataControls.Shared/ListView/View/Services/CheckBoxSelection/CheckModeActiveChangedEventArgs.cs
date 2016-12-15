using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Telerik.UI.Xaml.Controls.Data
{
    public class CheckModeActiveChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsCheckModeActiveChangedEventArgs"/> class.
        /// </summary>
        /// <param name="checkBoxesVisible">True if the check mode is about to be activated. False otherwise.</param>
        /// <param name="tappedItem">The data item from the source collection that has been tapped to activate the check mode.</param>
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

        public ObservableCollection<object> SelectedItems
        {
            get;
            private set;
        }
   
        public bool IsCheckModeActive
        {
            get;
            private set;
        }
    }
}
