using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a custom panel that contains the elements of the <see cref="RadHexView"/> control.
    /// </summary>
    public class HexPanel : Panel
    {
        internal bool measureRequested = true;
        private Size sizeCache;

        internal RadHexView Owner { get; set; }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.Owner.OnChildrenPanelArrange(finalSize);
            return finalSize;
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.measureRequested)
            {
                this.sizeCache = this.Owner.OnChildrenPanelMeasure(availableSize);
                this.measureRequested = false;
            }

            return this.sizeCache;
        }
    }
}
