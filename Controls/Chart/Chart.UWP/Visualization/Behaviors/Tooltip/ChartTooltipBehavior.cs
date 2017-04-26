using System;
using System.Diagnostics;
using Telerik.Charting;
using Telerik.Core;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Defines the tooltip behavior for RadChart.
    /// </summary>
    public class ChartTooltipBehavior : ChartBehavior
    {
        /// <summary>
        /// Identifies the ContentTemplate attached property.
        /// Usually this property is used by chart series to define different tooltips on a per series basis.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.RegisterAttached("ContentTemplate", typeof(DataTemplate), typeof(ChartTooltipBehavior), new PropertyMetadata(null));

        private Popup toolTip;
        private ChartTooltip toolTipContent;
        private bool snapToClosestPoint;
        private Point touchOverhang = new Point(0, 24);
        private Point displayPosition;
        private Size contentSize = new Size(0, 0);
        private HorizontalAlignment horizontalAlign;
        private VerticalAlignment verticalAlign;
        private DataPointInfo closestDataPointInfo;
        private DispatcherTimer delayTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartTooltipBehavior"/> class.
        /// </summary>
        public ChartTooltipBehavior()
        {
            this.toolTip = new Popup();

            this.toolTipContent = new ChartTooltip();
            this.toolTipContent.SizeChanged += this.OnToolTipContent_SizeChanged;

            this.toolTip.Child = this.toolTipContent;
            this.snapToClosestPoint = true;

            this.horizontalAlign = HorizontalAlignment.Center;
            this.verticalAlign = VerticalAlignment.Top;
        }

        /// <summary>
        /// Fires before the tool tip of RadChart is shown so that the user can provide
        /// a custom view model which he/she can bind to in the ToolTipTemplate.
        /// </summary>
        public event EventHandler<TooltipContextNeededEventArgs> ContextNeeded;

        /// <summary>
        /// Gets or sets the offset to be applied when the tooltip position is calculated.
        /// </summary>
        public Point TouchOverhang
        {
            get
            {
                return this.touchOverhang;
            }
            set
            {
                this.touchOverhang = value;
            }
        }

        /// <summary>
        /// Gets or sets the delay to be applied before the tooltip is displayed.
        /// </summary>
        public TimeSpan ShowDelay
        {
            get
            {
                return this.delayTimer.Interval;
            }
            set
            {
                this.delayTimer.Interval = value;
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the tooltip according to the touch point along the horizontal axis.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                return this.horizontalAlign;
            }
            set
            {
                this.horizontalAlign = value;
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the tooltip according to the touch point along the vertical axis.
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get
            {
                return this.verticalAlign;
            }
            set
            {
                this.verticalAlign = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the tooltip is currently displayed.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return this.toolTip.IsOpen;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tooltip will snap to the closest to the physical location data point.
        /// </summary>
        public bool SnapToClosestPoint
        {
            get
            {
                return this.snapToClosestPoint;
            }
            set
            {
                this.snapToClosestPoint = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the developer will manually handle when the tooltip will be shown and hidden. 
        /// </summary>
        public virtual bool HandleTooltipManually
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the <see cref="DataTemplate"/> instance associated with the specified dependency object.
        /// </summary>
        public static DataTemplate GetContentTemplate(DependencyObject instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException();
            }

            return instance.GetValue(ContentTemplateProperty) as DataTemplate;
        }

        /// <summary>
        /// Sets the provided <see cref="DataTemplate"/> instance to the specified dependency object.
        /// </summary>
        public static void SetContentTemplate(DependencyObject instance, DataTemplate template)
        {
            if (instance == null)
            {
                throw new ArgumentNullException();
            }

            instance.SetValue(ContentTemplateProperty, template);
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.PointerEntered"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal override void OnPointerEntered(PointerRoutedEventArgs args)
        {
            base.OnPointerEntered(args);

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (!this.HandleTooltipManually)
            {
                this.SetInteractionPointCore(args.GetCurrentPoint(this.chart).Position);
            }
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.PointerExited"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal override void OnPointerExited(PointerRoutedEventArgs args)
        {
            base.OnPointerExited(args);

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (!this.HandleTooltipManually)
            {
                this.HideTooltip();
            }
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.PointerPressed"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal override void OnPointerPressed(PointerRoutedEventArgs args)
        {
            base.OnPointerPressed(args);

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (!this.HandleTooltipManually)
            {
                this.HideTooltip();
            }
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.PointerMoved"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal override void OnPointerMoved(PointerRoutedEventArgs args)
        {
            base.OnPointerMoved(args);

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                // Touch is handled through manipulation events
                return;
            }

            if (args.Pointer.IsInContact)
            {
                // only mouse hover will be handled
                return;
            }

            if (!this.HandleTooltipManually)
            {
                this.SetInteractionPointCore(args.GetCurrentPoint(this.chart).Position);
            }
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.HoldStarted"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal override void OnHoldStarted(HoldingRoutedEventArgs args)
        {
            base.OnHoldStarted(args);

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (!this.toolTip.IsOpen && !this.HandleTooltipManually)
            {
                this.SetInteractionPointCore(args.GetPosition(this.chart));
            }
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.HoldCompleted"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal override void OnHoldCompleted(HoldingRoutedEventArgs args)
        {
            base.OnHoldCompleted(args);

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (!this.HandleTooltipManually)
            {
                this.HideTooltip();
            }
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.PointerReleased"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal override void OnPointerReleased(PointerRoutedEventArgs args)
        {
            base.OnPointerReleased(args);

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (!this.HandleTooltipManually)
            {
                this.HideTooltip();
            }
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.ManipulationDelta"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs args)
        {
            base.OnManipulationDelta(args);

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (!this.chart.isInHold)
            {
                return;
            }
            if (!this.HandleTooltipManually)
            {
                this.SetInteractionPointCore(args.Position);
            }
        }

        /// <summary>
        /// Updates and opens the tooltip.
        /// </summary>
        protected void ShowToolTip()
        {
            if (this.HandleTooltipManually)
            {
                this.ShowTooltipCore();
            }
        }

        /// <summary>
        /// Hides the tooltip.
        /// </summary>
        protected void HideTooltip()
        {
            if (!this.HandleTooltipManually)
            {
                this.delayTimer.Stop();
            }

            this.toolTip.IsOpen = false;
            this.displayPosition = new Point(-1, -1);
        }

        /// <summary>
        /// This method is called when the chart owner is loaded.
        /// </summary>
        protected override void OnLoaded()
        {
            base.OnLoaded();

            if (!this.HandleTooltipManually)
            {
                this.InitializeTimer();
            }
            this.InitializeTooltip();
        }

        /// <summary>
        /// This method is called when the chart owner is removed from the visual tree.
        /// </summary>
        protected override void OnUnloaded()
        {
            base.OnUnloaded();

            if (!this.HandleTooltipManually)
            {
                this.StopTimer();
            }

            this.RemoveToolTip();
        }

        /// <summary>
        /// Gets <see cref="ChartDataContext" /> associated with a gives physical location.
        /// </summary>
        /// <param name="physicalOrigin">The relative physical position of the requested data context.</param>
        /// <param name="findNearestPoints">True to find the nearest points, if no points are found on the requested physical location.</param>
        /// <returns>
        /// Returns <see cref="ChartDataContext" /> object holding information for the requested physical location.
        /// </returns>
        protected override ChartDataContext GetDataContext(Point physicalOrigin, bool findNearestPoints)
        {
            if (this.chart != null)
            {
                return this.chart.GetDataContext(physicalOrigin, ChartPointDistanceCalculationMode.TwoDimensional);
            }

            return null;
        }

        /// <summary>
        /// Sets the interaction point which will be used to calculate the tooltip's position.
        /// </summary>
        protected void SetInteractionPoint(Point position)
        {
            if (this.HandleTooltipManually)
            {
                this.SetInteractionPointCore(position);
            }
        }

        /// <summary>
        /// Updates and open the tooltip.
        /// </summary>
        private void ShowTooltipCore()
        {
            object context = this.GetTooltipContext(this.displayPosition);
            if (context == null || this.closestDataPointInfo == null ||
               (!this.closestDataPointInfo.ContainsTouchLocation && !this.SnapToClosestPoint))
            {
                this.toolTip.IsOpen = false;
                return;
            }
            this.toolTipContent.Content = context;
            this.toolTip.IsOpen = true;
            this.UpdateTooltipPosition(this.displayPosition);
        }

        private object OnContextNeeded(ChartDataContext defaultContext)
        {
            EventHandler<TooltipContextNeededEventArgs> handler = this.ContextNeeded;
            if (handler == null)
            {
                return defaultContext.ClosestDataPoint;
            }

            TooltipContextNeededEventArgs args = new TooltipContextNeededEventArgs(defaultContext);
            handler(this, args);

            return args.Context != null ? args.Context : defaultContext.ClosestDataPoint;
        }

        private void InitializeTimer()
        {
            this.delayTimer = new DispatcherTimer();
            this.delayTimer.Interval = TimeSpan.FromMilliseconds(300);
            this.delayTimer.Tick += this.OnDelayTimer_Tick;
        }

        private void StopTimer()
        {
            if (this.delayTimer.IsEnabled)
            {
                this.delayTimer.Stop();
            }
        }

        private void InitializeTooltip()
        {
            if (this.chart.adornerLayer != null && !this.chart.adornerLayer.Children.Contains(this.toolTip))
            {
                this.chart.adornerLayer.Children.Add(this.toolTip);
            }
        }

        private void RemoveToolTip()
        {
            this.toolTip.IsOpen = false;

            if (this.chart.adornerLayer != null && this.chart.adornerLayer.Children.Contains(this.toolTip))
            {
                this.chart.adornerLayer.Children.Remove(this.toolTip);
            }
        }

        private object GetTooltipContext(Point location)
        {
            ChartDataContext defaultContext = this.GetDataContext(location, true);
            this.closestDataPointInfo = defaultContext.ClosestDataPoint;

            if (this.closestDataPointInfo != null && this.closestDataPointInfo.DataPoint.Presenter != null)
            {
                var template = GetContentTemplate(this.closestDataPointInfo.DataPoint.Presenter as ChartSeries);

                if (template != null)
                {
                    this.toolTipContent.ContentTemplate = template;
                }

                if (template == null)
                {
                    template = GetContentTemplate(this);
                    this.toolTipContent.ContentTemplate = template;
                }

                if (template == null)
                {
                    this.toolTipContent.ClearValue(ContentControl.ContentTemplateProperty);
                }
            }

            return this.OnContextNeeded(defaultContext);
        }

        private void SetInteractionPointCore(Point position)
        {
            if (this.displayPosition == position)
            {
                return;
            }

            if (!this.HandleTooltipManually)
            {
                if (!this.IsInPlotArea(position))
                {
                    this.HideTooltip();
                    return;
                }

                this.toolTip.IsOpen = false;

                // reset the timer
                this.delayTimer.Stop();
                this.delayTimer.Start();
            }

            this.displayPosition = position;
        }

        private void UpdateTooltipPosition(Point position)
        {
            var pointPosition = RadRect.Empty;

            if (this.closestDataPointInfo != null)
            {
                pointPosition = this.closestDataPointInfo.DataPoint.GetPosition();

                if (!pointPosition.IsSizeValid())
                {
                    pointPosition = RadRect.Empty;
                }
            }

            if (this.snapToClosestPoint && pointPosition.IsSizeValid())
            {
                position = pointPosition.Location.ToPoint();
                position.X += this.chart.PlotOrigin.X;
                position.Y += this.chart.PlotOrigin.Y;
            }

            switch (this.horizontalAlign)
            {
                case Windows.UI.Xaml.HorizontalAlignment.Left:
                    position.X -= this.contentSize.Width + this.touchOverhang.X;
                    break;
                case Windows.UI.Xaml.HorizontalAlignment.Center:
                case Windows.UI.Xaml.HorizontalAlignment.Stretch:
                    position.X += this.snapToClosestPoint ?
                        (pointPosition.Width - this.contentSize.Width) / 2 :
                        -this.contentSize.Width / 2;
                    break;
                case Windows.UI.Xaml.HorizontalAlignment.Right:
                    position.X = this.snapToClosestPoint ?
                        pointPosition.Right + this.touchOverhang.X :
                        this.touchOverhang.X;
                    break;
            }

            switch (this.verticalAlign)
            {
                case Windows.UI.Xaml.VerticalAlignment.Top:
                    position.Y -= this.contentSize.Height + this.touchOverhang.Y;
                    break;
                case Windows.UI.Xaml.VerticalAlignment.Center:
                case Windows.UI.Xaml.VerticalAlignment.Stretch:
                    position.Y += this.snapToClosestPoint ?
                        (pointPosition.Height - this.contentSize.Height) / 2 :
                        -this.contentSize.Height / 2;
                    break;
                case Windows.UI.Xaml.VerticalAlignment.Bottom:
                    position.Y += this.snapToClosestPoint ?
                        pointPosition.Height + this.touchOverhang.Y :
                        this.touchOverhang.Y;
                    break;
            }

            this.toolTip.HorizontalOffset = position.X;
            this.toolTip.VerticalOffset = position.Y;
        }

        private bool IsInPlotArea(Point position)
        {
            if (this.chart == null)
            {
                return false;
            }

            position.X -= this.chart.PlotOrigin.X;
            position.Y -= this.chart.PlotOrigin.Y;

            return this.chart.PlotAreaClip.Contains(position.X, position.Y);
        }

        private void OnDelayTimer_Tick(object sender, object e)
        {
            this.delayTimer.Stop();
            this.ShowTooltipCore();
        }

        private void OnToolTipContent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.contentSize = e.NewSize;
            if (this.toolTip.IsOpen)
            {
                this.UpdateTooltipPosition(this.displayPosition);
            }
        }
    }
}
