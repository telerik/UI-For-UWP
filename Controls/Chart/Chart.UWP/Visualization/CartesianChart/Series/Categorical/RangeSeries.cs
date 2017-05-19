using System.Diagnostics.CodeAnalysis;
using Telerik.Charting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a chart range area series.
    /// </summary>
    public class RangeSeries : RangeSeriesBase, IFilledSeries, IStrokedSeries
    {
        /// <summary>
        /// Identifies the <see cref="Fill"/> property.
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(RangeSeries), new PropertyMetadata(null, OnFillChanged));

        /// <summary>
        /// Identifies the <see cref="Stroke"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(nameof(Stroke), typeof(Brush), typeof(RangeSeries), new PropertyMetadata(null, OnStrokeChanged));

        /// <summary>
        /// Identifies the <see cref="StrokeThickness"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(RangeSeries), new PropertyMetadata(2d, OnStrokeThicknessChanged));
      
        /// <summary>
        /// Identifies the <see cref="StrokeDashArray"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register(nameof(StrokeDashArray), typeof(DoubleCollection), typeof(RangeSeries), new PropertyMetadata(null, OnStrokeDashArrayChanged));

        /// <summary>
        /// Identifies the <see cref="StrokeLineJoin"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeLineJoinProperty =
            DependencyProperty.Register(nameof(StrokeLineJoin), typeof(PenLineJoin), typeof(RangeSeries), new PropertyMetadata(PenLineJoin.Miter, OnStrokeLineJoinChanged));

        internal RangeRenderer rangeRenderer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeSeries" /> class.
        /// </summary>
        public RangeSeries()
        {
            this.DefaultStyleKey = typeof(RangeSeries);
            this.rangeRenderer = new RangeRenderer();
            this.rangeRenderer.model = this.model;     
        }

        /// <summary>
        /// Gets or sets the style used to draw the <see cref="Windows.UI.Xaml.Shapes.Polyline"/> shape.
        /// </summary>
        public Brush Fill
        {
            get
            {
                return this.rangeRenderer.areaShape.Fill;
            }
            set
            {
                this.SetValue(FillProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Fill"/> property has been set locally.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        bool IFilledSeries.IsFillSetLocally
        {
            get
            {
                return this.ReadLocalValue(FillProperty) is Brush;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> instance that defines the stroke of the area shape.
        /// </summary>
        public Brush Stroke
        {
            get
            {
                return this.rangeRenderer.strokeShape.Stroke;
            }
            set
            {
                this.SetValue(StrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Stroke"/> property has been set locally.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        bool IStrokedSeries.IsStrokeSetLocally
        {
            get
            {
                return this.ReadLocalValue(StrokeProperty) is Brush;
            }
        }

        /// <summary>
        /// Gets or sets the thickness of the line used to present the series.
        /// </summary>
        public double StrokeThickness
        {
            get
            {
                return this.rangeRenderer.strokeShape.StrokeThickness;
            }
            set
            {
                this.SetValue(StrokeThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a collection of <see cref="T:System.Double" /> values that indicates the pattern of dashes and gaps that is used to outline stroked series.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public DoubleCollection StrokeDashArray
        {
            get
            {
                return (DoubleCollection)this.GetValue(StrokeDashArrayProperty);
            }
            set
            {
                this.SetValue(StrokeDashArrayProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the mode that defines how the area is stroked.
        /// </summary>
        public RangeSeriesStrokeMode StrokeMode
        {
            get
            {
                return this.rangeRenderer.strokeMode;
            }
            set
            {
                if (this.rangeRenderer.strokeMode == value)
                {
                    return;
                }

                this.rangeRenderer.strokeMode = value;
                this.InvalidateCore();
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="PenLineJoin" /> enumeration value that specifies the type of join that is used at the vertices of a stroked series.
        /// </summary>
        public PenLineJoin StrokeLineJoin
        {
            get
            {
                return (PenLineJoin)this.GetValue(StrokeLineJoinProperty);
            }
            set
            {
                this.SetValue(StrokeLineJoinProperty, value);
            }
        }   

        internal override string Family
        {
            get { return ChartPalette.AreaFamily; }
        }

        internal override FrameworkElement SeriesVisual
        {
            get
            {
                return this.rangeRenderer.areaShape;
            }
        }

        internal override void UpdateUICore(ChartLayoutContext context)
        {
            base.UpdateUICore(context);

            this.rangeRenderer.Render();
        }

        internal override RangeSeriesModel CreateModel()
        {
            return new RangeSeriesModel();
        }

        internal override void ApplyPaletteCore()
        {
            base.ApplyPaletteCore();

            this.rangeRenderer.ApplyPalette();

            this.UpdateLegendItem(null, null);
        }

        internal override void UpdateLegendItem(FrameworkElement visual, DataPoint dataPoint)
        {
            this.UpdateLegendItemProperties(this.rangeRenderer.areaShape.Fill, this.rangeRenderer.strokeShape.Stroke);
        }

        /// <summary>
        /// Adds the polyline shapes to the visual tree.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            if (applied)
            {
                this.renderSurface.Children.Add(this.rangeRenderer.areaShape);
                this.renderSurface.Children.Add(this.rangeRenderer.strokeShape);
            }

            return applied;
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            if (this.renderSurface != null)
            {
                this.renderSurface.Children.Remove(this.rangeRenderer.areaShape);
                this.renderSurface.Children.Remove(this.rangeRenderer.strokeShape);
            }
        }

        private static void OnFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeSeries series = d as RangeSeries;
            series.rangeRenderer.areaShape.Fill = e.NewValue as Brush;

            if (series.isPaletteApplied)
            {
                series.UpdatePalette(true);
            }
        }

        private static void OnStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeSeries series = d as RangeSeries;
            series.rangeRenderer.strokeShape.Stroke = e.NewValue as Brush;

            if (series.isPaletteApplied)
            {
                series.UpdatePalette(true);
            }
        }

        private static void OnStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeSeries series = d as RangeSeries;
            series.rangeRenderer.strokeShape.StrokeThickness = (double)e.NewValue;
        }

        private static void OnStrokeDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeSeries series = d as RangeSeries;
            series.rangeRenderer.strokeShape.StrokeDashArray = (e.NewValue as DoubleCollection).Clone();
        }

        private static void OnStrokeLineJoinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeSeries series = d as RangeSeries;
            series.rangeRenderer.strokeShape.StrokeLineJoin = (PenLineJoin)e.NewValue;
        }
    }
}
