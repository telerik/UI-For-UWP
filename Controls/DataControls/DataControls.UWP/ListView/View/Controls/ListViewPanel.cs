using Telerik.Core;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// This is a custom panel that contains all elements in a <see cref="RadListView"/>.
    /// </summary>
    public class ListViewPanel : Panel
    {
        internal bool isInArrange;

        private Size desiredSize;
        private Size lastArrangeSize;
        private bool isDirectMeasure;
        private bool isInMeasure;
        private bool measured = false;

        internal bool IsInMeasure
        {
            get
            {
                return this.isInMeasure;
            }
        }

        internal bool IsInArrange
        {
            get
            {
                return this.isInArrange;
            }
        }

        internal Size LastArrangeSize
        {
            get
            {
                return this.lastArrangeSize;
            }
        }

        internal RadListView Owner { get; set; }

        internal void Measure()
        {
            try
            {
                this.isDirectMeasure = true;
                this.isInMeasure = true;
                this.desiredSize = this.Owner.OnContentPanelMeasure(this.Owner.panelAvailableSize).ToSize();

                // NOTE: Skip next layout pass because it was probably caused by the scroll operation and we have already measured new cells.
                // Individual cell measure can be skipped only when using TextBlocks with same text.
                this.Measure(this.desiredSize);
            }
            finally
            {
                this.isDirectMeasure = false;
                this.isInMeasure = false;
            }
        }

        internal void Arrange()
        {
            this.isInArrange = true;
            this.Owner.OnContentPanelArrange(this.lastArrangeSize.ToRadSize());
            this.isInArrange = false;
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            if (!this.measured)
            {
                this.measured = true;
                base.MeasureOverride(availableSize);
            }

            if (this.isDirectMeasure)
            {
                return this.desiredSize;
            }

            if (this.Owner != null)
            {
                this.isInMeasure = true;

                // TODO: Refactor:
                this.Owner.Model.pendingMeasure = false;
                this.desiredSize = this.Owner.OnContentPanelMeasure(this.Owner.panelAvailableSize).ToSize();
                foreach (var child in this.Children)
                {
                    child.Measure(availableSize);
                }

                this.isInMeasure = false;
            }
            else
            {
                this.desiredSize = base.MeasureOverride(availableSize);
            }

            return this.desiredSize;
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            // NOTE: We cannot skip arrange pass else containers won't show at all.
            foreach (var child in this.Children)
            {
                child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            }

            this.lastArrangeSize = finalSize;

            if (this.Owner.scrollableAdornerLayerCache != null)
            {
                this.Owner.scrollableAdornerLayerCache.OnOwnerArranging(new Rect(0, 0, finalSize.Width, finalSize.Height));
            }

            if (this.Owner.Model.IsDataProviderUpdating)
            {
                // The model is still waiting for an asynchronous data update
                return finalSize;
            }

            // The InvalidateMeasure call does not work while in MeasureOverride pass
            if (this.Owner.Model.pendingMeasure)
            {
                this.InvalidateMeasure();
            }

            this.isInArrange = true;
            var size = this.Owner.OnContentPanelArrange(finalSize.ToRadSize()).ToSize();
            this.isInArrange = false;

            return size;
        }
    }
}
