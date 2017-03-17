using System;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop.Reorder;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a panel that lays out header cells in a <see cref="DataGridColumnHeaderPanel"/>.
    /// </summary>
    public partial class DataGridColumnHeaderPanel : Canvas
    {
        private Size desiredSize;
        private Size lastArrangeSize;
        private bool skipSingleMeasurePass;
        private RectangleGeometry clipGeometry = new RectangleGeometry();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridColumnHeaderPanel" /> class.
        /// </summary>
        public DataGridColumnHeaderPanel()
        {
            this.Clip = this.clipGeometry;
            this.SizeChanged += this.OnSizeChanged;

            this.InitializeDragDrop();
        }

        internal RadDataGrid Owner { get; set; }

        internal void AddChild(FrameworkElement container)
        {
            this.Children.Add(container);

            this.SetupReorderItem(container as IReorderItem);
        }

        internal void Measure()
        {
            Size previousDesiredSize = this.desiredSize;
            this.desiredSize = this.Owner.OnHeaderRowMeasure(this.Owner.CellsHostAvaialbleSize).ToSize();

            try
            {
                this.skipSingleMeasurePass = true;

                // NOTE: Skip next layout pass because it was probably caused by the scroll operation and we have already measured new cells.
                // Individual cell measure can be skipped only when using TextBlocks with same text.
                this.Measure(this.Owner.CellsHostAvaialbleSize.ToSize());
            }
            finally
            {
                this.skipSingleMeasurePass = false;
            }

            // We neeed to skip next measure pass if if desiredSize is different.
            // TODO: Why is this, Sir?
            // this.skipSingleMeasurePass = previousDesiredSize != this.desiredSize;
        }

        internal void Arrange()
        {
            this.Owner.OnHeaderRowArrange(this.lastArrangeSize.ToRadSize());
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            // after the column headers are measured we should allow the measurement of the cells.
            this.Owner.CellsPanel.isDirectMeasure = false;

            if (this.skipSingleMeasurePass)
            {
                this.skipSingleMeasurePass = false;
                return this.desiredSize;
            }

            if (this.Owner == null)
            {
                return base.MeasureOverride(availableSize);
            }

            var size = new Telerik.Core.RadSize(Math.Max(availableSize.Width, this.Owner.CellsHostAvaialbleSize.Width), Math.Max(availableSize.Height, this.Owner.CellsHostAvaialbleSize.Height));

            this.Owner.Model.pendingMeasureFlags &= ~InvalidateMeasureFlags.Header;
            this.desiredSize = this.Owner.OnHeaderRowMeasure(this.Owner.CellsHostAvaialbleSize).ToSize();

            ////Force sync between PhysicalOffset and Scrollview Offset in case header binding is used and width is auto, causing changing scrollviewer extend. 
            this.Owner.UpdateHorizontalPosition();

            return this.desiredSize;
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            // NOTE: We cannot skip arrange pass else containers won't show at all.
            // TODO: Consider using Arrange method instead of Canvas.Set* methods - probably it will be faster because we have to Arrange then anyway.
            base.ArrangeOverride(finalSize);

            this.lastArrangeSize = finalSize;

            if (this.Owner.Model.IsDataProviderUpdating)
            {
                // The model is still waiting for an asynchronous data update
                return finalSize;
            }

            // TODO: HACK!!! The InvalidateMeasure call does not work while in MeasureOverride pass
            if ((this.Owner.Model.pendingMeasureFlags & InvalidateMeasureFlags.Header) == InvalidateMeasureFlags.Header)
            {
                this.InvalidateMeasure();
            }
 
            this.Margin = new Windows.UI.Xaml.Thickness(-this.Owner.Model.FrozenColumnsWidth, 0, 0, 0);
            this.Owner.FrozenColumnHeadersHost.Width = this.Owner.Model.FrozenColumnsWidth;
            this.Owner.FrozenColumnHeadersHost.Height = finalSize.Height;

            return this.Owner.OnHeaderRowArrange(this.lastArrangeSize.ToRadSize()).ToSize();
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DataGridColumnHeaderPanelAutomationPeer(this);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var size = e.NewSize;

            this.clipGeometry.Rect = new Rect(0, 0, size.Width, size.Height);
        }
    }
}