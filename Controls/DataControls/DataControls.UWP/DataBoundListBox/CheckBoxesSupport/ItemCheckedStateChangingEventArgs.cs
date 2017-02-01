using System.ComponentModel;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Holds information about the <see cref="RadDataBoundListBox.ItemCheckedStateChanging"/> event.
    /// </summary>
    public class ItemCheckedStateChangingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCheckedStateChangingEventArgs"/> class.
        /// </summary>
        public ItemCheckedStateChangingEventArgs(object item, bool isChecked)
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
        /// Gets a value indicating whether the item is being checked or unchecked.
        /// </summary>
        public bool IsChecked
        {
            get;
            private set;
        }
    }
}
