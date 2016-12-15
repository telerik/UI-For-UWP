using System;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Holds information about an item that is part of the <see cref="RadVirtualizingDataControl"/>'s virtualization mechanism.
    /// </summary>
    public class ItemStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the data item which state has changed.
        /// </summary>
        /// <value>The data item.</value>
        public object DataItem { get; internal set; }

        /// <summary>
        /// Gets the new item state.
        /// </summary>
        /// <value>The state.</value>
        public ItemState State { get; internal set; }
    }
}
