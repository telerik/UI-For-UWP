using System;
using System.Collections.Generic;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// Encapsulates the data, associated with a change in the <see cref="RadListView.SelectedItems"/>.
    /// </summary>
    public class ListViewSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewSelectionChangedEventArgs" /> class.
        /// </summary>
        /// <param name="removedItems">The removed items.</param>
        /// <param name="addedItems">The added items.</param>
        public ListViewSelectionChangedEventArgs(IEnumerable<object> removedItems, IEnumerable<object> addedItems)
        {
            this.AddedItems = addedItems;
            this.RemovedItems = removedItems;
        }

        /// <summary>
        /// Gets a IEnumerable that contains the items that were deselected.
        /// </summary>
        public IEnumerable<object> RemovedItems { get; private set; }

        /// <summary>
        /// Gets a IEnumerable that contains the items that were selected.
        /// </summary>
        public IEnumerable<object> AddedItems { get; private set; }
    }
}
