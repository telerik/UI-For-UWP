using System;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Data.HexView
{
    /// <summary>
    /// Contains information about the <see cref="RadHexView.ItemTap"/> event.
    /// </summary>
    public class HexViewItemTapEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HexViewItemTapEventArgs"/> class.
        /// </summary>
        public HexViewItemTapEventArgs(RadHexHubTile item)
        {
            this.Item = item.DataContext;
        }

        /// <summary>
        /// Gets the tapped item.
        /// </summary>
        public object Item { get; private set; }
    }
}
