using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a panel that lays out cells in a <see cref="RadDataGrid"/> component.
    /// </summary>
    public class DataGridCellsPanel : Panel
    {
        internal bool isDirectMeasure;
        private Size desiredSize;
        private Size lastArrangeSize;
        private bool isInMeasure;
        private bool isInArrange;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridCellsPanel" /> class.
        /// </summary>
        public DataGridCellsPanel()
        {
            this.Tapped += this.OnTapped;
            this.DoubleTapped += this.OnDoubleTapped;
            this.PointerMoved += this.OnPointerMoved;
            this.PointerExited += this.OnPointerExited;
            this.PointerPressed += this.OnPointerPressed;
            this.KeyDown += this.OnKeyDown;
            this.Holding += this.OnHolding;
        }

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

        internal RadDataGrid Owner { get; set; }

        internal void Measure()
        {
            try
            {
                this.isDirectMeasure = true;
                this.isInMeasure = true;
                this.desiredSize = this.Owner.OnCellsPanelMeasure(this.Owner.CellsHostAvaialbleSize).ToSize();

                // NOTE: Skip next layout pass because it was probably caused by the scroll operation and we have already measured new cells.
                // Individual cell measure can be skipped only when using TextBlocks with same text.
                this.Measure(this.Owner.CellsHostAvaialbleSize.ToSize());
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
            this.Owner.OnCellsPanelArrange(this.lastArrangeSize.ToRadSize());
            this.isInArrange = false;
        }

        internal void OnContentLayerPanelMeasure()
        {
            if (this.isInMeasure)
            {
                return;
            }

            this.Owner.Model.InvalidateCellsDesiredSize();
            this.InvalidateMeasure();
        }

        internal void OnContentLayerPanelArrange()
        {
            if (this.isInArrange)
            {
                return;
            }

            this.Arrange();
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.isDirectMeasure)
            {
                return this.desiredSize;
            }

            if (this.Owner != null)
            {
                this.isInMeasure = true;

                this.Owner.Model.pendingMeasureFlags &= ~InvalidateMeasureFlags.Cells;
                this.desiredSize = this.Owner.OnCellsPanelMeasure(this.Owner.CellsHostAvaialbleSize).ToSize();

                foreach (var child in this.Children)
                {
                    child.Measure(availableSize);
                }

                this.Owner.visualStateLayerCache.UpdateVisuals();

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
            // We cannot skip arrange pass else containers won't show at all.
            // TODO: Consider using Arrange method instead of Canvas.Set* methods - probably it will be faster because we have to Arrange then anyway.
            base.ArrangeOverride(finalSize);

            this.lastArrangeSize = finalSize;

            if (this.Owner.scrolalbleAdornerLayerCache != null)
            {
                this.Owner.scrolalbleAdornerLayerCache.OnOwnerArranging(new Rect(0, 0, finalSize.Width, finalSize.Height));
            }

            if (this.Owner.Model.IsDataProviderUpdating)
            {
                // The model is still waiting for an asynchronous data update
                return finalSize;
            }

            // The InvalidateMeasure call does not work while in MeasureOverride pass
            if ((this.Owner.Model.pendingMeasureFlags & InvalidateMeasureFlags.Cells) == InvalidateMeasureFlags.Cells)
            {
                this.InvalidateMeasure();
            }

            this.isInArrange = true;

            var size = this.Owner.OnCellsPanelArrange(finalSize.ToRadSize()).ToSize();

            this.Margin = new Windows.UI.Xaml.Thickness(-this.Owner.Model.FrozenColumnsWidth, 0, 0, 0);
            this.Owner.DecorationsHost.Margin = new Windows.UI.Xaml.Thickness(-this.Owner.Model.FrozenColumnsWidth, 0, 0, 0);
            this.Owner.ScrollableAdornerHost.Margin = new Windows.UI.Xaml.Thickness(-this.Owner.Model.FrozenColumnsWidth, 0, 0, 0);

            this.Owner.FrozenContentHost.Width = this.Owner.Model.FrozenColumnsWidth;
            this.Owner.FrozenContentHost.Height = finalSize.Height;

            foreach (var child in this.Children)
            {
                child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            }

            this.isInArrange = false;

            return size;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DataGridCellsPanelAutomationPeer(this, this.Owner);
        }

        private void OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            if (this.Owner != null)
            {
               this.Owner.OnCellsPanelHolding(e);      
            }
        }

        private void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.OnCellsPanelTapped(e);
            }
        }

        private void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.OnCellsPanelDoubleTapped(e);
            }
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.OnCellsPanelPointerExited();
            }
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.OnCellsPanelPointerOver(e);
            }
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.OnCellsPanelPointerPressed();
            }
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.OnCellsPanelKeyDown(e);
            }
        }
    }
}