using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Telerik.Charting;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Chart;
using Telerik.UI.Xaml.Controls.Chart.Primitives;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a behavior that adds two lines in <see cref="RadChartBase"/>'s render surface. The two lines intersect at the center of the closest data point found.
    /// </summary>
    public class ChartTrackBallBehavior : ChartBehavior
    {
        /// <summary>
        /// Identifies the <see cref="LineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty LineStyleProperty =
            DependencyProperty.Register(nameof(LineStyle), typeof(Style), typeof(ChartTrackBallBehavior), new PropertyMetadata(null, OnLineStylePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="InfoStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty InfoStyleProperty =
            DependencyProperty.Register(nameof(InfoStyle), typeof(Style), typeof(ChartTrackBallBehavior), null);

        /// <summary>
        /// Identifies the <see cref="IntersectionTemplateProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IntersectionTemplateProperty =
            DependencyProperty.RegisterAttached("IntersectionTemplate", typeof(DataTemplate), typeof(ChartTrackBallBehavior), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TrackInfoTemplateProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TrackInfoTemplateProperty =
            DependencyProperty.RegisterAttached("TrackInfoTemplate", typeof(DataTemplate), typeof(ChartTrackBallBehavior), new PropertyMetadata(null));

        internal List<ContentPresenter> individualTrackInfos;

        private static readonly Style DefaultLineStyle;

        private TrackBallLineControl lineControl;
        private TrackBallSnapMode snapMode;
        private bool showInfo = true;
        private bool showIntersectionPoints;
        private bool visualsDisplayed;
        private Point position;
        private TrackInfoMode infoMode = TrackInfoMode.Individual;

        private TrackBallInfoControl trackInfoControl;
        private List<ContentPresenter> intersectionPoints;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "The static DefaultLineStyle field requires additional initialization.")]
        static ChartTrackBallBehavior()
        {
            DefaultLineStyle = new Style(typeof(Polyline));
            DefaultLineStyle.Setters.Add(new Setter(Polyline.StrokeThicknessProperty, 2));
            DefaultLineStyle.Setters.Add(new Setter(Polyline.StrokeProperty, new SolidColorBrush(Colors.Gray)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartTrackBallBehavior"/> class.
        /// </summary>
        public ChartTrackBallBehavior()
        {
            this.lineControl = new TrackBallLineControl();
            this.snapMode = TrackBallSnapMode.ClosestPoint;

            this.trackInfoControl = new TrackBallInfoControl(this);
            this.intersectionPoints = new List<ContentPresenter>(4);
            this.individualTrackInfos = new List<ContentPresenter>(4);
        }

        /// <summary>
        /// Occurs when a track info is updated, just before the UI that represents it is updated.
        /// Allows custom information to be displayed.
        /// </summary>
        public event EventHandler<TrackBallInfoEventArgs> TrackInfoUpdated;

        /// <summary>
        /// Gets or sets the <see cref="Style"/> that defines the appearance of the line displayed by a <see cref="ChartTrackBallBehavior"/> instance.
        /// The style should target the <see cref="Windows.UI.Xaml.Shapes.Polyline"/> type.
        /// </summary>
        public Style LineStyle
        {
            get
            {
                return this.GetValue(LineStyleProperty) as Style;
            }
            set
            {
                this.SetValue(LineStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> that defines the appearance of the TrackInfo control displayed by a <see cref="ChartTrackBallBehavior"/> instance.
        /// The style should target the <see cref="Telerik.UI.Xaml.Controls.Chart.Primitives.TrackBallInfoControl"/> type.
        /// </summary>
        public Style InfoStyle
        {
            get
            {
                return this.GetValue(InfoStyleProperty) as Style;
            }
            set
            {
                this.SetValue(InfoStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a visual information for all the closest data points will be displayed.
        /// </summary>
        public bool ShowInfo
        {
            get
            {
                return this.showInfo;
            }
            set
            {
                this.showInfo = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a visual representation for all the intersection points will be displayed.
        /// </summary>
        public bool ShowIntersectionPoints
        {
            get
            {
                return this.showIntersectionPoints;
            }
            set
            {
                this.showIntersectionPoints = value;
            }
        }

        /// <summary>
        /// Gets or sets the how this behavior should snap to the closest to a physical location data points.
        /// </summary>
        public TrackBallSnapMode SnapMode
        {
            get
            {
                return this.snapMode;
            }
            set
            {
                this.snapMode = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the track information displayed by the behavior.
        /// </summary>
        public TrackInfoMode InfoMode
        {
            get
            {
                return this.infoMode;
            }
            set
            {
                this.infoMode = value;
            }
        }

        /// <summary>
        /// Gets the  control used to display Polyline shape that renders the trackball line. Exposed for testing purposes.
        /// </summary>
        internal TrackBallLineControl LineControl
        {
            get
            {
                return this.lineControl;
            }
        }

        /// <summary>
        /// Gets the control used to display the track information. Exposed for testing purposes.
        /// </summary>
        internal TrackBallInfoControl InfoControl
        {
            get
            {
                return this.trackInfoControl;
            }
        }

        /// <summary>
        /// Gets the list with all the content presenters used to visualize intersection points. Exposed for testing purposes.
        /// </summary>
        internal List<ContentPresenter> IntersectionPoints
        {
            get
            {
                return this.intersectionPoints;
            }
        }

        /// <summary>
        /// Gets the position of the track ball. Exposed for testing purposes.
        /// </summary>
        internal Point Position
        {
            get
            {
                return this.position;
            }
        }

        internal override ManipulationModes DesiredManipulationMode
        {
            get
            {
                return ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            }
        }

        /// <summary>
        /// Gets the <see cref="DataTemplate"/> instance for the specified object instance.
        /// </summary>
        /// <remarks>
        /// This template is used to highlight the intersection point of each <see cref="ChartSeries"/> instance with the trackball line.
        /// </remarks>
        public static DataTemplate GetIntersectionTemplate(DependencyObject instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException();
            }

            return instance.GetValue(IntersectionTemplateProperty) as DataTemplate;
        }

        /// <summary>
        /// Sets the <see cref="DataTemplate"/> instance for the specified object instance.
        /// </summary>
        /// <remarks>
        /// This template is used to highlight the intersection point of each <see cref="ChartSeries"/> instance with the trackball line.
        /// </remarks>
        /// <param name="instance">The object instance to apply the template to.</param>
        /// <param name="value">The specified <see cref="DataTemplate"/> instance.</param>
        public static void SetIntersectionTemplate(DependencyObject instance, DataTemplate value)
        {
            if (instance == null)
            {
                throw new ArgumentNullException();
            }

            instance.SetValue(IntersectionTemplateProperty, value);
        }

        /// <summary>
        /// Gets the <see cref="DataTemplate"/> instance for the provided object instance.
        /// </summary>
        /// <remarks>
        /// This template defines the appearance of the information for the currently hit data point of each <see cref="ChartSeries"/>.
        /// </remarks>
        public static DataTemplate GetTrackInfoTemplate(DependencyObject instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return instance.GetValue(TrackInfoTemplateProperty) as DataTemplate;
        }

        /// <summary>
        /// Gets the specified <see cref="DataTemplate"/> instance to the provided object instance.
        /// </summary>
        /// <remarks>
        /// This template defines the appearance of the information for the currently hit data point of each <see cref="ChartSeries"/>.
        /// </remarks>
        public static void SetTrackInfoTemplate(DependencyObject instance, DataTemplate value)
        {
            if (instance == null)
            {
                throw new ArgumentNullException();
            }

            instance.SetValue(TrackInfoTemplateProperty, value);
        }

        /// <summary>
        /// Exposed for testing purposes.
        /// </summary>
        internal void HandleDrag(Point currentPosition)
        {
            this.position = currentPosition;
            if (!this.visualsDisplayed)
            {
                this.PrepareVisuals();
            }
            this.UpdateVisuals();
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

            this.HandleDrag(args.GetCurrentPoint(this.chart).Position);
            args.Handled = true;
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

            if (args.Pointer.IsInContact)
            {
                return;
            }

            this.HandleDrag(args.GetCurrentPoint(this.chart).Position);
            args.Handled = true;
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

            if (args.Pointer.PointerDeviceType != PointerDeviceType.Touch)
            {
                this.EndTrack();
            }
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.PointerExited"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal override void OnPointerExited(PointerRoutedEventArgs args)
        {
            base.OnPointerExited(args);

            this.EndTrack();
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.HoldCompleted"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal override void OnHoldCompleted(HoldingRoutedEventArgs args)
        {
            base.OnHoldCompleted(args);

            this.EndTrack();
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

            this.EndTrack();
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

            this.HandleDrag(args.GetPosition(this.chart));
            args.Handled = true;
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

            if (!this.chart.isInHold || args.IsInertial)
            {
                return;
            }

            this.HandleDrag(args.Position);
            args.Handled = true;
        }

        /// <summary>
        /// A callback from the owning <see cref="RadChartBase"/> instance that notifies for a completed UpdateUI pass.
        /// </summary>
        protected internal override void OnChartUIUpdated()
        {
            base.OnChartUIUpdated();

            if (this.visualsDisplayed)
            {
                this.UpdateVisuals();
            }
        }

        /// <summary>
        /// This method is called when this behavior is removed from the chart.
        /// </summary>
        protected override void OnDetached()
        {
            base.OnDetached();

            this.chart.adornerLayer.LayoutUpdated -= this.OnAdornerLayerLayoutUpdated;

            if (this.chart.adornerLayer.Children.Contains(this.lineControl))
            {
                this.chart.adornerLayer.Children.Remove(this.lineControl);
            }

            if (this.chart.adornerLayer.Children.Contains(this.trackInfoControl))
            {
                this.chart.adornerLayer.Children.Remove(this.trackInfoControl);
            }

            foreach (ContentPresenter presenter in this.intersectionPoints)
            {
                this.chart.adornerLayer.Children.Remove(presenter);
            }
        }

        private static void OnLineStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartTrackBallBehavior behavior = d as ChartTrackBallBehavior;
            behavior.lineControl.LineStyle = e.NewValue as Style;
        }

        private void HideVisuals()
        {
            this.visualsDisplayed = false;
            this.lineControl.Visibility = Visibility.Collapsed;
            this.lineControl.Line.Points.Clear();

            this.trackInfoControl.Visibility = Visibility.Collapsed;

            foreach (ContentPresenter presenter in this.intersectionPoints)
            {
                presenter.Visibility = Visibility.Collapsed;
            }

            foreach (ContentPresenter presenter in this.individualTrackInfos)
            {
                presenter.Visibility = Visibility.Collapsed;
            }
        }

        private void PrepareVisuals()
        {
            this.visualsDisplayed = true;

            this.lineControl.Visibility = Visibility.Visible;

            if (!this.chart.adornerLayer.Children.Contains(this.lineControl))
            {
                this.chart.adornerLayer.Children.Add(this.lineControl);
            }

            if (!this.showInfo)
            {
                return;
            }

            this.AdornerLayer.LayoutUpdated += this.OnAdornerLayerLayoutUpdated;

            if (this.infoMode == TrackInfoMode.Individual)
            {
                return;
            }

            Style style = this.InfoStyle;
            if (style != null)
            {
                this.trackInfoControl.Style = style;
            }

            if (!this.chart.adornerLayer.Children.Contains(this.trackInfoControl))
            {
                this.chart.adornerLayer.Children.Add(this.trackInfoControl);
                this.trackInfoControl.SetValue(Canvas.ZIndexProperty, this.trackInfoControl.DefaultZIndex);
            }

            this.trackInfoControl.Visibility = Visibility.Visible;
        }

        private void UpdateVisuals()
        {
            ChartDataContext context = this.GetDataContext(this.position, true);

            if (context.ClosestDataPoint != null)
            {
                var point = context.ClosestDataPoint.DataPoint as CategoricalDataPoint;
                if (point != null)
                {
                    List<DataPointInfo> points = null;
                    if (point.Category == null)
                    {
                        points = context.DataPoints.Where(c => c.DataPoint is CategoricalDataPoint &&
                            ((CategoricalDataPoint)c.DataPoint).Category == null).ToList();
                    }
                    else
                    {
                        // select points only with the same category. TODO: Refactor this.
                        points = context.DataPoints.Where(c => 
                                c.DataPoint is CategoricalDataPoint &&
                                point.Category.Equals(((CategoricalDataPoint)c.DataPoint).Category)).ToList();
                    }

                    context = new ChartDataContext(points, context.ClosestDataPoint);
                }
            }

            this.UpdateLine(context);
            this.UpdateIntersectionPoints(context);
            this.UpdateTrackInfo(context);
        }

        private void UpdateTrackInfo(ChartDataContext context)
        {
            if (!this.showInfo)
            {
                return;
            }

            // arrange the track content
            if (this.lineControl.Line.Points.Count == 0)
            {
                Debug.Assert(false, "Must have line points initialized.");
                return;
            }

            TrackBallInfoEventArgs args = new TrackBallInfoEventArgs(context);
            if (this.TrackInfoUpdated != null)
            {
                this.TrackInfoUpdated(this, args);
            }

            if (this.infoMode == TrackInfoMode.Multiple)
            {
                this.trackInfoControl.Update(args);
            }
            else
            {
                this.BuildIndividualTrackInfos(context);
            }
        }

        private void UpdateLine(ChartDataContext context)
        {
            this.lineControl.Line.Points.Clear();

            Point plotOrigin = this.chart.PlotOrigin;

            if (this.snapMode == TrackBallSnapMode.AllClosePoints)
            {
                Point[] points = new Point[context.DataPoints.Count];
                int index = 0;

                foreach (DataPointInfo info in context.DataPoints)
                {
                    Point center = info.DataPoint.Center();
                    center.X += plotOrigin.X;
                    center.Y += plotOrigin.Y;
                    points[index++] = center;
                }

                Array.Sort(points, new PointYComparer());
                foreach (Point point in points)
                {
                    this.lineControl.Line.Points.Add(point);
                }
            }
            else if (this.snapMode == TrackBallSnapMode.ClosestPoint && context.ClosestDataPoint != null)
            {
                Point center = context.ClosestDataPoint.DataPoint.Center();
                center.X += plotOrigin.X;
                center.Y += plotOrigin.Y;

                // Temporary fix for NAN values. Remove when the chart starts to support null values.
                if (double.IsNaN(center.X))
                {
                    center.X = 0;
                }

                if (double.IsNaN(center.Y))
                {
                    center.Y = this.chart.chartArea.plotArea.layoutSlot.Bottom;
                }

                this.lineControl.Line.Points.Add(center);
            }

            RadRect plotArea = this.chart.chartArea.plotArea.layoutSlot;
            Point topPoint;
            Point bottomPoint;

            if (this.lineControl.Line.Points.Count > 0)
            {
                topPoint = new Point(this.lineControl.Line.Points[0].X, plotArea.Y);
                bottomPoint = new Point(this.lineControl.Line.Points[this.lineControl.Line.Points.Count - 1].X, plotArea.Bottom);
            }
            else
            {
                topPoint = new Point(this.position.X, plotArea.Y);
                bottomPoint = new Point(this.position.X, plotArea.Bottom);
            }

            this.lineControl.Line.Points.Insert(0, topPoint);
            this.lineControl.Line.Points.Insert(this.lineControl.Line.Points.Count, bottomPoint);
        }

        private void UpdateIntersectionPoints(ChartDataContext context)
        {
            if (!this.showIntersectionPoints)
            {
                return;
            }

            int index = 0;
            Point plotOrigin = this.chart.PlotOrigin;

            foreach (ContentPresenter presenter in this.intersectionPoints)
            {
                presenter.Visibility = Visibility.Collapsed;
            }

            foreach (DataPointInfo info in context.DataPoints)
            {
                DataTemplate template = GetIntersectionTemplate(info.Series);
                if (template == null)
                {
                    continue;
                }

                ContentPresenter presenter = this.GetIntersectionPointPresenter(index);
                presenter.Visibility = Visibility.Visible;
                presenter.Content = info;
                presenter.ContentTemplate = template;
                presenter.Measure(RadChartBase.InfinitySize);

                Point center = info.DataPoint.Center();
                Size desiredSize = presenter.DesiredSize;
                Canvas.SetLeft(presenter, center.X - Math.Abs(plotOrigin.X) - (desiredSize.Width / 2));
                Canvas.SetTop(presenter, center.Y - Math.Abs(plotOrigin.Y) - (desiredSize.Height / 2));

                index++;
            }
        }

        private ContentPresenter GetIntersectionPointPresenter(int index)
        {
            if (index < this.intersectionPoints.Count)
            {
                return this.intersectionPoints[index];
            }

            ContentPresenter visual = new ContentPresenter();
            this.chart.adornerLayer.Children.Add(visual);
            this.intersectionPoints.Add(visual);

            return visual;
        }

        private void OnAdornerLayerLayoutUpdated(object sender, object args)
        {
            if (this.lineControl.Line.Points.Count == 0)
            {
                return;
            }

            if (this.infoMode == TrackInfoMode.Multiple)
            {
                // fit the info control into the plot area
                RadRect plotArea = this.chart.chartArea.plotArea.layoutSlot;
                Point topPoint = new Point(this.lineControl.Line.Points[0].X, plotArea.Y);

                double left = topPoint.X - (int)(this.trackInfoControl.ActualWidth / 2);
                left = Math.Min(left, this.chart.ActualWidth - this.trackInfoControl.ActualWidth);
                left = Math.Max(0, left);

                Canvas.SetLeft(this.trackInfoControl, left);
                Canvas.SetTop(this.trackInfoControl, topPoint.Y);

                // update the top line point to reach the bottom edge of the track info
                topPoint.Y += this.trackInfoControl.ActualHeight;
                this.lineControl.Line.Points[0] = topPoint;
            }
            else
            {
                this.ArrangeIndividualTrackInfos();
            }
        }

        private void BuildIndividualTrackInfos(ChartDataContext context)
        {
            int index = 0;
            List<DataPointInfo> infos = new List<DataPointInfo>(context.DataPoints);
            infos.Sort(new DataPointInfoYComparer());

            foreach (DataPointInfo info in infos)
            {
                ContentPresenter presenter;
                if (this.individualTrackInfos.Count > index)
                {
                    presenter = this.individualTrackInfos[index];
                    presenter.Visibility = Visibility.Visible;
                }
                else
                {
                    presenter = new ContentPresenter();
                    this.individualTrackInfos.Add(presenter);
                    this.chart.adornerLayer.Children.Add(presenter);
                }

                presenter.Content = info;
                presenter.ContentTemplate = GetTrackInfoTemplate(info.Series);

                index++;
            }

            while (index < this.individualTrackInfos.Count)
            {
                this.individualTrackInfos[index].Visibility = Visibility.Collapsed;
                index++;
            }
        }

        private void ArrangeIndividualTrackInfos()
        {
            int index = 0;
            Point plotOrigin = this.chart.PlotOrigin;
            foreach (ContentPresenter presenter in this.individualTrackInfos)
            {
                if (presenter.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                DataPointInfo info = presenter.Content as DataPointInfo;
                RadRect layoutSlot = info.DataPoint.layoutSlot;

                Size size = presenter.DesiredSize;
                HorizontalAlignment horizontalAlign = index % 2 == 0 ? HorizontalAlignment.Left : HorizontalAlignment.Right;
                double x = 0;

                switch (horizontalAlign)
                {
                    case HorizontalAlignment.Left:
                        x = layoutSlot.X - size.Width;
                        break;
                    case HorizontalAlignment.Center:
                        x = layoutSlot.Center.X - size.Width / 2;
                        break;
                    case HorizontalAlignment.Right:
                        x = layoutSlot.Right;
                        break;
                }

                double y = layoutSlot.Center.Y - size.Height / 2;

                Canvas.SetLeft(presenter, x + plotOrigin.X);
                Canvas.SetTop(presenter, y + plotOrigin.Y);

                index++;
            }
        }

        private void EndTrack()
        {
            this.chart.adornerLayer.LayoutUpdated -= this.OnAdornerLayerLayoutUpdated;
            this.HideVisuals();
        }

        private class PointYComparer : IComparer<Point>
        {
            public int Compare(Point x, Point y)
            {
                return x.Y.CompareTo(y.Y);
            }
        }

        private class DataPointInfoYComparer : IComparer<DataPointInfo>
        {
            public int Compare(DataPointInfo x, DataPointInfo y)
            {
                if (x == null || y == null)
                {
                    return 0;
                }

                return x.DataPoint.layoutSlot.Center.Y.CompareTo(y.DataPoint.layoutSlot.Center.Y);
            }
        }
    }
}
