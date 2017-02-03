using System;
using System.Collections;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Contains information about the <see cref="RadDataBoundListBox.SelectionChanging"/> event.
    /// </summary>
    public class SelectionChangingEventArgs : RoutedEventArgs
    {
        private object[] addedItems;
        private object[] removedItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionChangingEventArgs"/> class.
        /// </summary>
        /// <param name="removedItems">The removed items.</param>
        /// <param name="addedItems">The added items.</param>
        public SelectionChangingEventArgs(IList<object> removedItems, IList<object> addedItems)
        {
            if (removedItems == null)
            {
                throw new ArgumentNullException(nameof(removedItems));
            }
            if (addedItems == null)
            {
                throw new ArgumentNullException(nameof(addedItems));
            }
            this.removedItems = new object[removedItems.Count];
            removedItems.CopyTo(this.removedItems, 0);
            this.addedItems = new object[addedItems.Count];
            addedItems.CopyTo(this.addedItems, 0);
        }

        /// <summary>
        /// Gets the added items.
        /// </summary>
        /// <value>The added items.</value>
        public IList AddedItems
        {
            get
            {
                return this.addedItems;
            }
        }

        /// <summary>
        /// Gets the removed items.
        /// </summary>
        /// <value>The removed items.</value>
        public IList RemovedItems
        {
            get
            {
                return this.removedItems;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SelectionChangingEventArgs"/> is canceled.
        /// </summary>
        public bool Cancel
        {
            get;
            set;
        }
    }
}