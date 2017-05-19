using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Telerik.Charting;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a chart series that visualize data points using arcs that form a pie.
    /// </summary>
    [ContentProperty(Name = "DataPoints")]
    public class PieSeries : ChartSeries
    {
        /// <summary>
        /// Identifies the <see cref="DefaultSegmentStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty DefaultSegmentStyleProperty =
            DependencyProperty.Register(nameof(DefaultSegmentStyle), typeof(Style), typeof(PieSeries), new PropertyMetadata(null, OnDefaultSegmentStyleChanged));

        /// <summary>
        /// Identifies the <see cref="ValueBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty ValueBindingProperty =
            DependencyProperty.Register(nameof(ValueBinding), typeof(DataPointBinding), typeof(PieSeries), new PropertyMetadata(null, OnValueBindingChanged));

        /// <summary>
        /// Identifies the <see cref="SegmentStyleSelector"/> property.
        /// </summary>
        public static readonly DependencyProperty SegmentStyleSelectorProperty =
            DependencyProperty.Register(nameof(SegmentStyleSelector), typeof(StyleSelector), typeof(PieSeries), new PropertyMetadata(null, OnSliceStyleSelectorChanged));

        /// <summary>
        /// Identifies the <see cref="RadiusFactor"/> property.
        /// </summary>
        public static readonly DependencyProperty RadiusFactorProperty =
            DependencyProperty.Register(nameof(RadiusFactor), typeof(double), typeof(PieSeries), new PropertyMetadata(DefaultRadiusFactor, OnRadiusFactorChanged));

        /// <summary>
        /// Identifies the <see cref="SelectedPointOffset"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedPointOffsetProperty =
            DependencyProperty.Register(nameof(SelectedPointOffset), typeof(double), typeof(PieSeries), new PropertyMetadata(DefaultSelectionOffset, OnSelectedPointOffsetChanged));

        /// <summary>
        /// Identifies the <see cref="HighlightBrush"/> property.
        /// </summary>
        public static readonly DependencyProperty HighlightBrushProperty =
            DependencyProperty.Register(nameof(HighlightBrush), typeof(Brush), typeof(PieSeries), new PropertyMetadata(null, OnHighlightBrushChanged));

        /// <summary>
        /// Identifies the <see cref="HighlightInnerRadiusFactor"/> property.
        /// </summary>
        public static readonly DependencyProperty HighlightInnerRadiusFactorProperty =
            DependencyProperty.Register(nameof(HighlightInnerRadiusFactor), typeof(double), typeof(PieSeries), new PropertyMetadata(0.9, OnHighlightInnerRadiusFactorChanged));

        /// <summary>
        /// Identifies the <see cref="LegendTitleBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty LegendTitleBindingProperty =
            DependencyProperty.Register(nameof(LegendTitleBinding), typeof(DataPointBinding), typeof(PieSeries), new PropertyMetadata(null, OnLegendTitleBindingChanged));

        /// <summary>
        /// Identifies the <see cref="IsVisibleInLegendBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty IsVisibleInLegendBindingProperty =
            DependencyProperty.Register(nameof(IsVisibleInLegendBinding), typeof(DataPointBinding), typeof(PieSeries), new PropertyMetadata(null, OnVisibleInLegendBindingChanged));

        /// <summary>
        /// Identifies the <see cref="AngleRange"/> property.
        /// </summary>
        public static readonly DependencyProperty AngleRangeProperty =
            DependencyProperty.Register(nameof(AngleRange), typeof(AngleRange), typeof(PieSeries), new PropertyMetadata(Telerik.Charting.AngleRange.Default, OnAngleRangePropertyChanged));

        private const double ArcPadding = 2;
        private const double DefaultRadiusFactor = 1;
        private const double DefaultSelectionOffset = 0.15;

        private List<PieSegment> segments;
        private ObservableCollection<Style> segmentStyles;
        private PieSeriesModel model;
        private StyleSelector segmentStyleSelectorCache;
        private Style defaultSegmentStyleCache;
        private PieUpdateContext updateContext;
        private double selectedPointOffsetCache = DefaultSelectionOffset;
        private double highlightRadiusFactorCache = 0.9;

        private LegendItemCollection legendInfos;

        /// <summary>
        /// Initializes a new instance of the <see cref="PieSeries"/> class.
        /// </summary>
        public PieSeries()
        {
            this.DefaultStyleKey = typeof(PieSeries);

            this.model = new PieSeriesModel();
            this.segments = new List<PieSegment>();
            this.segmentStyles = new ObservableCollection<Style>();
            this.segmentStyles.CollectionChanged += this.OnSegmentStylesChanged;

            this.legendInfos = new LegendItemCollection();

            this.ClipToPlotArea = false;
        }

        /// <summary>
        /// Gets or sets the scale ([0,1]) that defines the radius of the overlay UI that represents the highlight effect of the series.
        /// </summary>
        public double HighlightInnerRadiusFactor
        {
            get
            {
                return this.highlightRadiusFactorCache;
            }
            set
            {
                this.SetValue(HighlightInnerRadiusFactorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the brush used to render the semi-filled ellipse over the series.
        /// </summary>
        public Brush HighlightBrush
        {
            get
            {
                return (Brush)this.GetValue(HighlightBrushProperty);
            }
            set
            {
                this.SetValue(HighlightBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> instance that defines the default appearance of each <see cref="Path"/> presenting a pie slice.
        /// </summary>
        public Style DefaultSegmentStyle
        {
            get
            {
                return this.defaultSegmentStyleCache;
            }
            set
            {
                this.SetValue(DefaultSegmentStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the offset applied to a <see cref="PieDataPoint"/> which is currently selected. This value is applied only if the point's OffsetFromCenter property is 0.
        /// </summary>
        public double SelectedPointOffset
        {
            get
            {
                return this.selectedPointOffsetCache;
            }
            set
            {
                this.SetValue(SelectedPointOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the binding that will be used to fill the <see cref="SingleValueDataPoint.Value"/> member of the contained data points.
        /// </summary>
        public DataPointBinding ValueBinding
        {
            get
            {
                return this.GetValue(ValueBindingProperty) as DataPointBinding;
            }
            set
            {
                this.SetValue(ValueBindingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the binding that will be used by any attached legend to display legend item title.
        /// </summary>
        public DataPointBinding LegendTitleBinding
        {
            get
            {
                return this.GetValue(LegendTitleBindingProperty) as DataPointBinding;
            }
            set
            {
                this.SetValue(LegendTitleBindingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the binding that will be used by any attached legend whether to add the item in the legend.
        /// </summary>
        public DataPointBinding IsVisibleInLegendBinding
        {
            get
            {
                return this.GetValue(IsVisibleInLegendBindingProperty) as DataPointBinding;
            }
            set
            {
                this.SetValue(IsVisibleInLegendBindingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="StyleSelector"/> instance used to select the appropriate style for a <see cref="PieDataPoint"/>.
        /// </summary>
        public StyleSelector SegmentStyleSelector
        {
            get
            {
                return this.segmentStyleSelectorCache;
            }
            set
            {
                this.SetValue(SegmentStyleSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the radius factor used to calculate the radius of the visual series.
        /// </summary>
        /// <remarks>
        /// This value is usually within the (0,1) range but it is possible to oversize the series by setting a value greater than 1.
        /// </remarks>
        /// <value>The default value is 1.</value>
        public double RadiusFactor
        {
            get
            {
                return (double)this.GetValue(RadiusFactorProperty);
            }
            set
            {
                this.SetValue(RadiusFactorProperty, value);
            }
        }

        /// <summary>
        /// Gets the collection of data points associated with the series.
        /// </summary>
        public ElementCollection<PieDataPoint> DataPoints
        {
            get
            {
                return this.model.DataPoints;
            }
        }

        /// <summary>
        /// Gets the collection storing a Style instance for each segment present on the chart.
        /// </summary>
        public ObservableCollection<Style> SegmentStyles
        {
            get
            {
                return this.segmentStyles;
            }
        }

        /// <summary>
        /// Gets or sets the angle range that define the pie.
        /// </summary>
        public AngleRange AngleRange
        {
            get
            {
                return this.model.Range;
            }
            set
            {
                this.SetValue(AngleRangeProperty, value);
            }
        }

        internal RadRect PieRect
        {
            get
            {
                if (this.updateContext == null)
                {
                    return RadRect.Empty;
                }

                return new RadRect(
                    this.updateContext.Center.X - this.updateContext.Radius,
                    this.updateContext.Center.Y - this.updateContext.Radius,
                    this.updateContext.Diameter,
                    this.updateContext.Diameter);
            }
        }

        /// <summary>
        /// Gets the human-readable family of this instance. For example Area is the family for Area, SplineArea and PolarArea series.
        /// </summary>
        internal override string Family
        {
            get
            {
                return ChartPalette.PieFamily;
            }
        }

        internal override ChartSeriesModel Model
        {
            get
            {
                return this.model;
            }
        }

        /// <summary>
        /// Gets the pie segments. Exposed for testing purposes.
        /// </summary>
        internal List<PieSegment> Segments
        {
            get
            {
                return this.segments;
            }
        }

        internal LegendItemCollection LegendInfos
        {
            get
            {
                return this.legendInfos;
            }
        }

        internal static bool HitTestValid(PieDataPoint point, Rect touchRect)
        {
            if (point == null)
            {
                return false;
            }

            return point.ContainsRect(touchRect);
        }

        /// <summary>
        /// Creates the concrete data source for this instance.
        /// </summary>
        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new PieSeriesDataSource();
        }

        internal override void SetDynamicLegendTitle(string titlePath, string extractedValue)
        {
            this.LegendTitleBinding = new PropertyNameDataPointBinding(titlePath);
        }

        internal override int GetPaletteIndexForPoint(DataPoint point)
        {
            return point.CollectionIndex;
        }

        internal override void OnDataPointSelectionChanged(DataPoint point)
        {
            (point as PieDataPoint).OffsetFromCenter = point.isSelected ? this.selectedPointOffsetCache : 0;

            base.OnDataPointSelectionChanged(point);
        }

        /// <summary>
        /// Updates of all of the chart elements presented by this instance.
        /// </summary>
        internal override void UpdateUICore(ChartLayoutContext context)
        {
            if (this.renderSurface == null)
            {
                return;
            }
            RadSize availableSize = new RadSize(
                this.chart.chartArea.layoutSlot.Width * this.chart.zoomCache.Width,
                this.chart.chartArea.layoutSlot.Height * this.chart.zoomCache.Height);

            this.updateContext = this.GetUpdateContext(availableSize);
            this.UpdateSegments();

            base.UpdateUICore(context);
        }

        internal override void ApplyPaletteCore()
        {
            base.ApplyPaletteCore();

            for (int i = 0; i < this.segments.Count; i++)
            {
                PieSegment segment = this.segments[i];

                if (segment.Path.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                Brush fill = null;
                Brush stroke = null;

                // segment style has higher priority as it comes from our SliceStyles property
                // so it should be null in order to apply a palette entry values
                if (segment.isDefaultStyle)
                {
                    fill = this.chart.GetPaletteBrush(segment.point.CollectionIndex, PaletteVisualPart.Fill, this.Family, segment.point.isSelected);
                    stroke = this.chart.GetPaletteBrush(segment.point.CollectionIndex, PaletteVisualPart.Stroke, this.Family, segment.point.isSelected);
                }

                if (fill != null)
                {
                    segment.Path.Fill = fill;
                }
                else
                {
                    segment.Path.ClearValue(Shape.FillProperty);
                }

                if (stroke != null)
                {
                    segment.Path.Stroke = stroke;
                }
                else
                {
                    segment.Path.ClearValue(Shape.StrokeProperty);
                }

                segment.UpdateLegendItem();
            }
        }

        internal override void ArrangeLabel(FrameworkElement visual, ChartSeriesLabelUpdateContext context)
        {
            RadSize size = MeasureVisual(visual);
            PieDataPoint piePoint = context.Point as PieDataPoint;

            double radius = this.updateContext.Radius;
            double offsetFromCenter = radius * piePoint.OffsetFromCenter;
            double offset = context.Definition.Margin.Left;

            // calculate the position of the label, depending on its size
            this.updateContext.Radius = radius - offset + offsetFromCenter;
            this.updateContext.StartAngle = piePoint.startAngle;

            double angle = piePoint.startAngle;
            if (this.AngleRange.SweepDirection == ChartSweepDirection.Clockwise)
            {
                angle += piePoint.sweepAngle / 2;
            }
            else
            {
                angle -= piePoint.sweepAngle / 2;
            }

            Point middlePoint = this.updateContext.CalculateArcPoint(angle);
            middlePoint.X += size.Width * Math.Cos(angle * RadMath.DegToRadFactor) / 2;
            middlePoint.Y += size.Height * Math.Sin(angle * RadMath.DegToRadFactor) / 2;

            RadRect labelRect = RadRect.Round(new RadRect(middlePoint.X - (size.Width / 2), middlePoint.Y - (size.Height / 2), size.Width, size.Height));

            this.ArrangeUIElement(visual, labelRect);

            this.updateContext.Radius = radius;
        }

        internal override double GetDistanceToPoint(DataPoint dataPoint, Point tapLocation, ChartPointDistanceCalculationMode pointDistanceMode)
        {
            var pieDataPoint = dataPoint as PieDataPoint;

            if (pieDataPoint != null && pointDistanceMode == ChartPointDistanceCalculationMode.TwoDimensional)
            {
                // TODO: Consider whether we will need linear distance.
                return pieDataPoint.GetPolarDistance(tapLocation);
            }

            return base.GetDistanceToPoint(dataPoint, tapLocation, pointDistanceMode);
        }

        internal virtual PieUpdateContext SetupUpdateContext(RadSize availableSize, Size updatedAvailableSize, PieUpdateContext context)
        {
            context.Diameter = Math.Min(updatedAvailableSize.Width, updatedAvailableSize.Height) * this.RadiusFactor;
            context.Radius = context.Diameter / 2;

            // Adding 0.5 to exceed the integer part of the double. That way after cast to int will return the same result as round operation.
            // This solution is used since it is faster.
            context.Center = new Point(
                (int)(((availableSize.Width - updatedAvailableSize.Width) / 2) + (updatedAvailableSize.Width / 2) + .5),
                (int)(((availableSize.Height - updatedAvailableSize.Height) / 2) + (updatedAvailableSize.Height / 2) + .5));
            context.StartAngle = this.model.Range.StartAngle;
            context.ArcSize = new Size(context.Radius, context.Radius);
            context.SweepDirection = (SweepDirection)this.AngleRange.SweepDirection;

            return context;
        }

        internal virtual PieUpdateContext CreateUpdateContext()
        {
            return new PieUpdateContext();
        }

        internal virtual PieSegment CreateSegment()
        {
            return new PieSegment();
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new PieSeriesAutomationPeer(this);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DataPoint> HitTestDataPointsCore(Rect touchRect, bool includeAllDataPoints)
        {
            foreach (var point in this.DataPoints)
            {
                if (point.isVisible && PieSeries.HitTestValid(point, touchRect))
                {
                    yield return point;
                }
            }
        }

        private static void OnDefaultSegmentStyleChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            PieSeries presenter = target as PieSeries;
            presenter.defaultSegmentStyleCache = args.NewValue as Style;
            presenter.InvalidateCore();
        }

        private static void OnSelectedPointOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            PieSeries presenter = target as PieSeries;
            presenter.selectedPointOffsetCache = (double)args.NewValue;
            presenter.InvalidateCore();
        }

        private static void OnValueBindingChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            PieSeries presenter = target as PieSeries;
            (presenter.dataSource as PieSeriesDataSource).ValueBinding = args.NewValue as DataPointBinding;
        }

        private static void OnSliceStyleSelectorChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            PieSeries series = target as PieSeries;
            series.segmentStyleSelectorCache = args.NewValue as StyleSelector;
            series.InvalidateCore();
        }

        private static void OnRadiusFactorChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            PieSeries series = target as PieSeries;
            series.InvalidateCore();
        }

        private static void OnLegendTitleBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = (PieSeries)d;

            foreach (PieSegment segment in series.segments)
            {
                if (segment.point == null || segment.point.isEmpty)
                {
                    continue;
                }

                segment.UpdateLegendTitle(series.LegendTitleBinding);
            }
        }

        private static void OnVisibleInLegendBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = (PieSeries)d;

            foreach (PieSegment segment in series.segments)
            {
                if (segment.point == null || segment.point.isEmpty)
                {
                    continue;
                }

                segment.UpdateVisibleInLegend(series.IsVisibleInLegendBinding);
            }
        }

        private static void OnHighlightInnerRadiusFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as PieSeries;
            if (series.IsInternalPropertyChange)
            {
                return;
            }

            double newValue = (double)e.NewValue;
            if (newValue < 0)
            {
                newValue = 0;
            }
            else if (newValue > 1)
            {
                newValue = 1;
            }

            series.highlightRadiusFactorCache = newValue;
            if (newValue != (double)e.NewValue)
            {
                series.ChangePropertyInternally(HighlightInnerRadiusFactorProperty, newValue);
            }

            series.InvalidateCore();
        }

        private static void OnHighlightBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as PieSeries;
            series.InvalidateCore();
        }

        private static void OnAngleRangePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var series = (PieSeries)sender;
            var newRange = (AngleRange)args.NewValue;

            if (series.model.Range == newRange)
            {
                return;
            }

            series.model.Range = newRange;
            series.InvalidateCore();
        }

        private void UpdateSegments()
        {
            int index = 0;
            int realizedIndex = 0;
            foreach (PieDataPoint point in this.model.DataPoints)
            {
                if (point.isEmpty)
                {
                    index++;
                    continue;
                }

                if (point.isSelected && point.OffsetFromCenter == 0d)
                {
                    // the point is selected before the ElementTree is loaded, set the property silently (no invalidate)
                    point.SetPropertySilently(PieDataPoint.OffsetFromCenterPropertyKey, this.selectedPointOffsetCache);
                }

                PieSegment segment = this.GetSegment(realizedIndex);
                segment.Update(point, this.updateContext);

                this.UpdateSegmentStyle(point, segment, index);

                if (!this.LegendInfos.Contains(segment.LegendItem))
                {
                    this.LegendInfos.Add(segment.LegendItem);
                }

                if (!DesignMode.DesignModeEnabled)
                {
                    segment.UpdateLegendTitle(this.LegendTitleBinding);
                    segment.UpdateVisibleInLegend(this.IsVisibleInLegendBinding);
                }

                index++;
                realizedIndex++;

                if (point.SweepAngle > PieSegment.ArcSegmentMaxAngle)
                {
                    break;
                }
            }

            // hide extra segments
            while (realizedIndex < this.segments.Count)
            {
                this.segments[realizedIndex].point = null;
                this.segments[realizedIndex].Path.Tag = null;

                this.segments[realizedIndex].Path.Visibility = Visibility.Collapsed;
                this.segments[realizedIndex].HighlightPath.Visibility = Visibility.Collapsed;

                this.LegendInfos.Remove(this.segments[realizedIndex].LegendItem);

                realizedIndex++;
            }
        }

        private void UpdateSegmentStyle(PieDataPoint point, PieSegment segment, int index)
        {
            segment.isDefaultStyle = false;
            segment.HighlightPath.Fill = this.HighlightBrush;

            if (this.segmentStyleSelectorCache != null)
            {
                Style style = this.segmentStyleSelectorCache.SelectStyle(point, this);
                if (style != null)
                {
                    segment.Path.Style = style;
                    return;
                }
            }

            if (this.segmentStyles.Count > 0)
            {
                Style style = this.segmentStyles[index % this.segmentStyles.Count];
                if (style != null)
                {
                    segment.Path.Style = style;
                    return;
                }
            }

            segment.Path.Style = this.defaultSegmentStyleCache;
            segment.isDefaultStyle = true;
        }

        private PieUpdateContext GetUpdateContext(RadSize availableSize)
        {
            Size updatedAvailableSize = this.GetUpdatedSize(availableSize);

            var context = this.CreateUpdateContext();

            return this.SetupUpdateContext(availableSize, updatedAvailableSize, context);
        }

        private Size GetUpdatedSize(RadSize availableSize)
        {
            double radiusFactor = this.RadiusFactor;

            // do not apply automatic offset if a radius factor is applied
            double offsetFromCenter = radiusFactor == 1d ? this.model.MaxOffsetFromCenter : 0;

            Size available = new Size(Math.Max(0, availableSize.Width - (2 * ArcPadding)), Math.Max(0, availableSize.Height - (2 * ArcPadding)));

            // TODO: Calculation revisit for the offset
            available.Width -= (int)(available.Width * offsetFromCenter);
            available.Height -= (int)(available.Height * offsetFromCenter);
            return available;
        }

        private PieSegment GetSegment(int realizedIndex)
        {
            PieSegment segment;

            if (realizedIndex < this.segments.Count)
            {
                segment = this.segments[realizedIndex];
                segment.Path.Visibility = Visibility.Visible;
                segment.HighlightPath.Visibility = Visibility.Visible;
            }
            else
            {
                segment = this.CreateSegment();
                segment.ParentSeries = this;

                this.segments.Add(segment);

                this.renderSurface.Children.Add(segment.Path);
                this.renderSurface.Children.Add(segment.HighlightPath);
            }

            return segment;
        }

        private void OnSegmentStylesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.InvalidateCore();
        }
    }
}