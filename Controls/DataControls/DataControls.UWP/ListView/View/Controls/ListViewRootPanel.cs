using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// Represents a panel that supports the layout of Table components.
    /// </summary>
    public sealed class ListViewRootPanel : Panel
    {
        internal RadListView Owner { get; set; }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.Owner == null)
            {
                return base.MeasureOverride(availableSize);
            }

            this.Owner.ScrollViewer.Measure(availableSize);

            // Hack to force measuring scrollviewer when available size grows.
            this.Owner.OnContentPanelMeasure(this.Owner.panelAvailableSize);

            if (this.Owner.AdornerHost != null)
            {
                this.Owner.AdornerHost.Measure(availableSize);
            }

            return this.Owner.ScrollViewer.DesiredSize;
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.Owner == null)
            {
                return base.ArrangeOverride(finalSize);
            }

            double topOffset = 0;
            double bottomOffset = 0;

            if (this.Owner != null && this.Owner.Orientation == Orientation.Horizontal && this.Owner.headerFooterLayerCache != null)
            {
                topOffset = this.Owner.headerFooterLayerCache.topOffset;
                bottomOffset = this.Owner.headerFooterLayerCache.bottomOffset;
            }

            var rect = new Rect(0, topOffset, finalSize.Width, Math.Max(0, finalSize.Height - bottomOffset - topOffset));
            this.Owner.ScrollViewer.Arrange(rect);

            if (this.Owner.AdornerHost != null)
            {
                this.Owner.AdornerHost.Arrange(rect);
                this.Owner.AdornerHost.Clip = new RectangleGeometry { Rect = rect };
            }

            if (this.Owner.ScrollableAdornerHost != null)
            {
                this.Owner.ScrollableAdornerHost.Arrange(rect);
            }

            if (this.Owner.overlayAdornerLayerCache != null)
            {
                this.Owner.overlayAdornerLayerCache.ArrangeOverlay(rect);
            }

            return finalSize;
        }
    }
}
