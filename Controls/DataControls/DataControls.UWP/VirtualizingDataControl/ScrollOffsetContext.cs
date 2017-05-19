using Telerik.Core.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// This class holds information about the current scroll offset in a <see cref="RadDataBoundListBox"/> control.
    /// </summary>
    internal struct ScrollOffsetContext
    {
        internal DataSourceItem topVisibleItem;
        internal double itemOffsetFromTopEdge;

        internal ScrollOffsetContext(DataSourceItem topItem, double itemOffset)
        {
            this.topVisibleItem = topItem;
            this.itemOffsetFromTopEdge = itemOffset;
        }
    }
}
