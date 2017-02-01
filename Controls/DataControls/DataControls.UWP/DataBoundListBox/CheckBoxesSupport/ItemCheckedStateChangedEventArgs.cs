using System;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Holds information about the <see cref="RadDataBoundListBox.ItemCheckedStateChanged"/> event.
    /// </summary>
    public class ItemCheckedStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCheckedStateChangedEventArgs"/> class.
        /// </summary>
        public ItemCheckedStateChangedEventArgs(object item, bool isChecked)
        {
            this.Item = item;
            this.IsChecked = isChecked;
        }

        /// <summary>
        /// Gets the data item associated with the event.
        /// </summary>
        public object Item
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the item has been checked or unchecked.
        /// </summary>
        public bool IsChecked
        {
            get;
            private set;
        }
    }
}
