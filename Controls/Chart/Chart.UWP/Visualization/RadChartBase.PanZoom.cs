using System;
using System.Diagnostics.CodeAnalysis;
using Telerik.Charting;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    public partial class RadChartBase
    {
        /// <summary>
        /// Identifies the <see cref="Zoom"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register(nameof(Zoom), typeof(Size), typeof(RadChartBase), new PropertyMetadata(new Size(1, 1), OnZoomPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="MaxZoom"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register(nameof(MaxZoom), typeof(Size), typeof(RadChartBase), new PropertyMetadata(new Size(50, 50), OnMaxZoomPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="MaxZoom"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register(nameof(MinZoom), typeof(Size), typeof(RadChartBase), new PropertyMetadata(new Size(1, 1), OnMinZoomPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ScrollOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScrollOffsetProperty =
            DependencyProperty.Register(nameof(ScrollOffset), typeof(Point), typeof(RadChartBase), new PropertyMetadata(new Point(0, 0), OnScrollOffsetPropertyChanged));

        internal Size maxZoomCache = new Size(50, 50);
        internal Size minZoomCache = new Size(1, 1);
        internal Size zoomCache = new Size(1, 1);
        internal Point scrollOffsetCache;

        /// <summary>
        /// Gets or sets the current zoom (scale) of the chart.
        /// </summary>
        public Size Zoom
        {
            get
            {
                return this.zoomCache;
            }
            set
            {
                this.SetValue(ZoomProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum allowed zoom for this instance.
        /// </summary>
        public Size MaxZoom
        {
            get
            {
                return this.maxZoomCache;
            }
            set
            {
                this.SetValue(MaxZoomProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum allowed zoom for this instance.
        /// </summary>
        public Size MinZoom
        {
            get
            {
                return this.minZoomCache;
            }
            set
            {
                this.SetValue(MinZoomProperty, value);
            }
        }

        /// <summary>
        /// Gets the origin of the plot area relative to the viewport.
        /// </summary>
        public Point PlotOrigin
        {
            get
            {
                RadRect plotArea = this.chartArea.plotArea.layoutSlot;
                return new Point(this.scrollOffsetCache.X * plotArea.Width, this.scrollOffsetCache.Y * plotArea.Height);
            }
        }

        /// <summary>
        /// Gets the clip that encloses the plot area in view coordinates - that is without the zoom factor applied and with the pan offset calculated.
        /// </summary>
        public RadRect PlotAreaClip
        {
            get
            {
                RadRect plotArea = this.chartArea.plotArea.layoutSlot;
                return new RadRect(
                    -this.scrollOffsetCache.X * plotArea.Width + plotArea.X,
                    -this.scrollOffsetCache.Y * plotArea.Height + plotArea.Y,
                    plotArea.Width,
                    plotArea.Height);
            }
        }

        /// <summary>
        /// Gets or sets the origin used to calculate the arrange box of the chart area.
        /// The value is in units relative to the viewport size. For example value of (-1, 0) will scroll the chart scene to the left with the width of the viewport. 
        /// </summary>
        public Point ScrollOffset
        {
            get
            {
                return this.scrollOffsetCache;
            }
            set
            {
                this.SetValue(ScrollOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets the current scale applied along the X direction.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        double IChartView.ZoomWidth
        {
            get
            {
                return this.zoomCache.Width;
            }
        }

        /// <summary>
        /// Gets the current scale applied along the Y direction.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        double IChartView.ZoomHeight
        {
            get
            {
                return this.zoomCache.Height;
            }
        }

        /// <summary>
        /// Gets the X-coordinate of the top-left corner where the layout should start from.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        double IChartView.PlotOriginX
        {
            get
            {
                return this.scrollOffsetCache.X;
            }
        }

        /// <summary>
        /// Gets the Y-coordinate of the top-left corner where the layout should start from.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        double IChartView.PlotOriginY
        {
            get
            {
                return this.scrollOffsetCache.Y;
            }
        }

        private static void OnZoomPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadChartBase chart = sender as RadChartBase;
            if (chart.IsInternalPropertyChange)
            {
                return;
            }

            chart.UpdateZoom((Size)e.NewValue);
        }

        private static void OnMaxZoomPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadChartBase chart = sender as RadChartBase;
            chart.maxZoomCache = (Size)e.NewValue;

            chart.UpdateZoom(chart.zoomCache);
        }

        private static void OnMinZoomPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadChartBase chart = sender as RadChartBase;
            chart.minZoomCache = (Size)e.NewValue;

            chart.UpdateZoom(chart.zoomCache);
        }

        private static void OnScrollOffsetPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadChartBase chart = sender as RadChartBase;
            if (chart.IsInternalPropertyChange)
            {
                return;
            }

            chart.UpdateScrollOffset((Point)e.NewValue);
        }

        private static Point ClampScrollOffset(Point newOffset, Size newZoom)
        {
            if (newOffset.X >= 0)
            {
                newOffset.X = 0;
            }
            else if (-newOffset.X + 1 > newZoom.Width)
            {
                newOffset.X = -newZoom.Width + 1;
            }

            if (newOffset.Y >= 0)
            {
                newOffset.Y = 0;
            }
            else if (-newOffset.Y + 1 > newZoom.Height)
            {
                newOffset.Y = -newZoom.Height + 1;
            }

            return newOffset;
        }

        private Size ClampZoom(Size newZoom)
        {
            newZoom.Width = Math.Max(this.minZoomCache.Width, newZoom.Width);
            newZoom.Height = Math.Max(this.minZoomCache.Height, newZoom.Height);

            newZoom.Width = Math.Min(this.maxZoomCache.Width, newZoom.Width);
            newZoom.Height = Math.Min(this.maxZoomCache.Height, newZoom.Height);

            return newZoom;
        }

        private void UpdateZoom(Size newZoom)
        {
            Size clampedZoom = this.ClampZoom(newZoom);
            if (newZoom != clampedZoom)
            {
                this.ChangePropertyInternally(ZoomProperty, clampedZoom);
            }

            this.SetZoom(clampedZoom);
        }

        private void TranslateAfterZoom(Size newZoom, Size oldZoom)
        {
            double widthChange = (newZoom.Width - oldZoom.Width) / 2;
            double heightChange = (newZoom.Height - oldZoom.Height) / 2;

            this.ScrollOffset = new Point(this.scrollOffsetCache.X - widthChange, this.scrollOffsetCache.Y - heightChange);
        }

        private void SetZoom(Size newZoom)
        {
            if (newZoom == this.zoomCache)
            {
                return;
            }

            Size oldZoom = this.zoomCache;
            this.zoomCache = newZoom;

            if (!this.arrangePassed)
            {
                this.UpdateScrollOffset(this.scrollOffsetCache);
                return;
            }

            this.OnZoomChanged();

            if (this.manipulating)
            {
                this.TranslateAfterZoom(this.zoomCache, oldZoom);
            }
            else
            {
                this.UpdateScrollOffset(this.scrollOffsetCache);
            }
        }

        private void UpdateScrollOffset(Point newOffset)
        {
            Point clampedOffset = ClampScrollOffset(newOffset, this.zoomCache);
            if (clampedOffset != newOffset)
            {
                this.ChangePropertyInternally(ScrollOffsetProperty, clampedOffset);
            }

            this.SetScrollOffset(clampedOffset);
        }

        private void SetScrollOffset(Point newOffset)
        {
            if (this.scrollOffsetCache == newOffset)
            {
                return;
            }

            this.scrollOffsetCache = newOffset;

            if (this.arrangePassed)
            {
                this.OnPlotOriginChanged();
            }
        }

        partial void UpdatePanZoomOnArrange()
        {
            this.SetZoom((Size)this.GetValue(ZoomProperty));
            this.SetScrollOffset((Point)this.GetValue(ScrollOffsetProperty));
        }
    }
}
