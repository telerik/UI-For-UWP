using System;
using System.Collections;
using System.Collections.Generic;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    ///  Provides data for the RadDataGrid SelectionChanged event.
    /// </summary>
    public class DataGridSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridSelectionChangedEventArgs" /> class.
        /// </summary>
        /// <param name="removedItems">The removed items.</param>
        /// <param name="addedItems">The added items.</param>
        public DataGridSelectionChangedEventArgs(IEnumerable<object> removedItems, IEnumerable<object> addedItems)
        {
            this.AddedItems = addedItems;
            this.RemovedItems = removedItems;
        }

        /// <summary>
        ///    Gets a IEnumerable that contains the items that were deselected.
        /// </summary>
        public IEnumerable<object> RemovedItems
        {
            get;
            private set;
        }

        /// <summary>
        ///    Gets a IEnumerable that contains the items that were selected.
        /// </summary>
        public IEnumerable<object> AddedItems
        {
            get;
            private set;
        }
    }
}
