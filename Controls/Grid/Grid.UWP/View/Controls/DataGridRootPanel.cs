using System;
using System.Diagnostics;
using Telerik.UI.Xaml.Controls.Grid.Model;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a panel that supports the layout of Table components.
    /// </summary>
    public sealed class DataGridRootPanel : Panel
    {
        internal RadDataGrid Owner
        {
            get;
            set;
        }

        internal Size CellsHostAvailableSize
        {
            get;
            set;
        }

        private UIElement ColumnHeadersHost
        {
            get
            {
                return this.Owner.ColumnHeadersHost;
            }
        }

        private UIElement FrozenColumnsHost
        {
            get
            {
                return this.Owner.FrozenColumnHeadersHost;
            }
        }

        private UIElement FrozenContentHost
        {
            get
            {
                return this.Owner.FrozenContentHost;
            }
        }

        private UIElement GroupHeadersHost
        {
            get
            {
                return this.Owner.GroupHeadersHost;
            }
        }

        private UIElement DecorationsHost
        {
            get
            {
                return this.Owner.DecorationsHost;
            }
        }

        private UIElement AdornerHost
        {
            get
            {
                return this.Owner.AdornerHost;
            }
        }

        private UIElement FrozenGroupsHost
        {
            get
            {
                return this.Owner.FrozenGroupHeadersHost;
            }
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This method is long, not complex.")]
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.Owner == null)
            {
                return base.MeasureOverride(availableSize);
            }

            try
            {
                this.Owner.RootPanelMeasure = true;

                var normalizedSize = NormalizeAvailableSize(availableSize);

                var model = this.Owner.Model;
                if (!model.IsTreeLoaded)
                {
                    model.LoadElementTree();
                }

                double availableWidth = normalizedSize.Width;
                double availableHeight = normalizedSize.Height;

                double servicePanelWidth = 0d;
                double servicePanelHeight = 0d;

                if (this.Owner.IsServicePanelVisible)
                {
                    var actualAvailableWidth = this.Owner.GroupPanelPosition == GroupPanelPosition.Left ? availableWidth : double.PositiveInfinity;
                    var actualAvailableHeight = this.Owner.GroupPanelPosition == GroupPanelPosition.Left ? double.PositiveInfinity : availableHeight;

                    this.Owner.ServicePanel.Measure(new Size(actualAvailableWidth, actualAvailableHeight));

                    if (this.Owner.GroupPanelPosition == GroupPanelPosition.Left)
                    {
                        servicePanelWidth = this.Owner.ServicePanel.DesiredSize.Width;
                        availableWidth -= servicePanelWidth;
                    }
                    else
                    {
                        servicePanelHeight = this.Owner.ServicePanel.DesiredSize.Height;
                        availableHeight -= servicePanelHeight;
                    }
                }

                this.Owner.ColumnReorderServicePanel.Measure(new Size(availableWidth, availableHeight));

                this.ColumnHeadersHost.Measure(new Size(availableWidth, double.PositiveInfinity));
                Size columnsHostDesiredSize = new Size(this.ColumnHeadersHost.DesiredSize.Width + this.FrozenColumnsHost.DesiredSize.Width, Math.Max(this.ColumnHeadersHost.DesiredSize.Height, this.FrozenColumnsHost.DesiredSize.Height));

                this.FrozenContentHost.Measure(new Size(availableWidth, double.PositiveInfinity));

                double availableRowHeight = availableHeight;

                var cellsAvailableSize = new Size(availableWidth, availableRowHeight);
                if (!GridModel.DoubleArithmetics.AreClose(this.CellsHostAvailableSize, cellsAvailableSize))
                {
                    // The cells panel is not properly measured when the available size is changed dynamically by the user.
                    this.Owner.InvalidateCellsMeasure();
                }

                this.CellsHostAvailableSize = cellsAvailableSize;

                this.Owner.ScrollViewer.Measure(this.CellsHostAvailableSize);
                Size cellsHostDesiredSize = this.Owner.ScrollViewer.DesiredSize;

                if (this.FrozenGroupsHost != null)
                {
                    this.FrozenGroupsHost.Measure(this.CellsHostAvailableSize);
                }

                if (this.Owner.GroupHeadersHost != null)
                {
                    this.Owner.GroupHeadersHost.Measure(this.CellsHostAvailableSize);
                }

                if (this.DecorationsHost != null)
                {
                    this.DecorationsHost.Measure(new Size(availableWidth, availableRowHeight));
                }

                if (this.AdornerHost != null)
                {
                    this.AdornerHost.Measure(new Size(availableWidth, availableRowHeight));
                }

                Size desiredSize = new Size(
                     Math.Max(cellsHostDesiredSize.Width + servicePanelWidth, 0),
                      cellsHostDesiredSize.Height);

                return desiredSize;
            }
            finally
            {
                this.Owner.RootPanelMeasure = false;
            }
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.Owner == null)
            {
                return base.ArrangeOverride(finalSize);
            }

            double left = 0;
            double top = 0;
            double width = finalSize.Width;
            double height = finalSize.Height;

            double servicePanelOffset = 0;

            if (this.Owner.IsServicePanelVisible)
            {
                var arrangeRect = this.Owner.GroupPanelPosition == GroupPanelPosition.Left ? new Rect(left, top, this.Owner.ServicePanel.DesiredSize.Width, finalSize.Height) : new Rect(left, this.Owner.ActualHeight - this.Owner.ServicePanel.DesiredSize.Height, finalSize.Width, this.Owner.ServicePanel.DesiredSize.Height);
                this.Owner.ServicePanel.Arrange(arrangeRect);

                if (this.Owner.GroupPanelPosition == GroupPanelPosition.Left)
                {
                    left += this.Owner.ServicePanel.DesiredSize.Width;
                    width -= this.Owner.ServicePanel.DesiredSize.Width;
                    servicePanelOffset = this.Owner.ServicePanel.DesiredSize.Width;
                }
                else
                {
                    height -= this.Owner.ServicePanel.DesiredSize.Height;
                }
            }

            var columnReorderPanelRect = new Rect(width + servicePanelOffset - this.Owner.ColumnReorderServicePanel.DesiredSize.Width, top, this.Owner.ColumnReorderServicePanel.DesiredSize.Width, this.Owner.ColumnReorderServicePanel.DesiredSize.Height);
            this.Owner.ColumnReorderServicePanel.Arrange(columnReorderPanelRect);
            this.ColumnHeadersHost.Arrange(new Rect(0, top, Math.Max(0, width), Math.Max(0, this.ColumnHeadersHost.DesiredSize.Height)));

            this.Owner.ScrollViewer.Arrange(new Rect(left, top, Math.Max(0, width), Math.Max(0, height)));
            this.FrozenGroupsHost.Arrange(new Rect(left, top + this.ColumnHeadersHost.DesiredSize.Height, Math.Max(0, width), Math.Max(0, height - this.ColumnHeadersHost.DesiredSize.Height)));

            if (this.AdornerHost != null)
            {
                var rect = new Rect(0, 0, finalSize.Width, finalSize.Height);
                this.AdornerHost.Arrange(rect);
                this.AdornerHost.Clip = new RectangleGeometry { Rect = rect };
            }

            if (this.Owner.ScrollableAdornerHost != null)
            {
                this.Owner.ScrollableAdornerHost.Arrange(new Rect(0, top, Math.Max(0, width), Math.Max(0, height)));
            }

            if (this.Owner.overlayAdornerLayerCache != null)
            {
                this.Owner.overlayAdornerLayerCache.ArrangeOverlay(new Rect(left, top, Math.Max(0, width), Math.Max(0, height)));
            }

            return finalSize;
        }

        private static Size NormalizeAvailableSize(Size size)
        {
            if (double.IsInfinity(size.Width))
            {
                size.Width = Window.Current.Bounds.Width;
            }

            if (double.IsInfinity(size.Height))
            {
                size.Height = Window.Current.Bounds.Height;
            }

            return size;
        }
    }
}
