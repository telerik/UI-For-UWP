using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Telerik.Charting;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents <see cref="RadarPointSeries"/> which connect each <see cref="CategoricalDataPoint"/> with a straight line segment.
    /// </summary>
    public class RadarLineSeries : RadarPointSeries, IStrokedSeries
    {
        /// <summary>
        /// Identifies the <see cref="Stroke"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(nameof(Stroke), typeof(Brush), typeof(RadarLineSeries), new PropertyMetadata(null, OnStrokeChanged));

        /// <summary>
        /// Identifies the <see cref="StrokeThickness"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(RadarLineSeries), new PropertyMetadata(2d, OnStrokeThicknessChanged));

        /// <summary>
        /// Identifies the <see cref="StrokeDashArray"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register(nameof(StrokeDashArray), typeof(DoubleCollection), typeof(RadarLineSeries), new PropertyMetadata(null, OnStrokeDashArrayChanged));

        /// <summary>
        /// Identifies the <see cref="StrokeLineJoin"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeLineJoinProperty =
            DependencyProperty.Register(nameof(StrokeLineJoin), typeof(PenLineJoin), typeof(RadarLineSeries), new PropertyMetadata(PenLineJoin.Miter, OnStrokeLineJoinChanged));

        internal RadarLineRenderer renderer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadarLineSeries"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "These virtual calls do not rely on uninitialized base state.")]
        public RadarLineSeries()
        {
            this.DefaultStyleKey = typeof(RadarLineSeries);

            this.renderer = this.CreateRenderer();
            this.renderer.model = this.Model;
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
        /// Gets or sets a value indicating whether the line curve will be closed.
        /// That is the last point to be connected to the first one. True by default.
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return this.renderer.isClosed;
            }
            set
            {
                this.renderer.isClosed = value;
                this.InvalidateCore();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> instance that defines the stroke of 
        /// the <see cref="Windows.UI.Xaml.Shapes.Line"/> shape.
        /// </summary>
        public Brush Stroke
        {
            get
            {
                return this.GetValue(StrokeProperty) as Brush;
            }
            set
            {
                this.SetValue(StrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the thickness of the line used to present the series.
        /// </summary>
        public double StrokeThickness
        {
            get
            {
                return this.renderer.strokeShape.StrokeThickness;
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

        /// <summary>
        /// Gets the human-readable family of this instance. For example Area is the family for Area, SplineArea and RadarArea series.
        /// </summary>
        internal override string Family
        {
            get
            {
                return ChartPalette.LineFamily;
            }
        }

        internal override bool SupportsDefaultVisuals
        {
            get
            {
                return false;
            }
        }

        internal override IEnumerable<FrameworkElement> RealizedDefaultVisualElements
        {
            get
            {
                yield return this.renderer.strokeShape;
            }
        }

        internal virtual RadarLineRenderer CreateRenderer()
        {
            return new RadarLineRenderer();
        }

        /// <summary>
        /// Updates of all of the chart elements presented by this instance.
        /// </summary>
        internal override void UpdateUICore(ChartLayoutContext context)
        {
            base.UpdateUICore(context);

            this.renderer.Render();
        }

        internal override void ApplyPaletteCore()
        {
            base.ApplyPaletteCore();

            this.renderer.ApplyPalette();
        }

        internal override void UpdateLegendItem(FrameworkElement visual, DataPoint dataPoint)
        {
            this.UpdateLegendItemProperties((Brush)visual.GetValue(Path.FillProperty) ?? (Brush)visual.GetValue(Path.StrokeProperty), (Brush)visual.GetValue(Path.StrokeProperty));
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadarLineSeriesAutomationPeer(this);
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            if (this.renderSurface != null)
            {
                this.renderSurface.Children.Remove(this.renderer.strokeShape);
            }
        }

        /// <summary>
        /// Adds the polyline shape to the visual tree.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            if (applied)
            {
                this.renderSurface.Children.Add(this.renderer.strokeShape);
            }

            return applied;
        }

        private static void OnStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadarLineSeries series = d as RadarLineSeries;
            series.renderer.strokeShape.Stroke = e.NewValue as Brush;

            if (series.isPaletteApplied)
            {
                series.UpdatePalette(true);
            }
        }

        private static void OnStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadarLineSeries series = d as RadarLineSeries;
            series.renderer.strokeShape.StrokeThickness = (double)e.NewValue;
        }

        private static void OnStrokeDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadarLineSeries series = d as RadarLineSeries;
            series.renderer.strokeShape.StrokeDashArray = (e.NewValue as DoubleCollection).Clone();
        }

        private static void OnStrokeLineJoinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadarLineSeries series = d as RadarLineSeries;
            series.renderer.strokeShape.StrokeLineJoin = (PenLineJoin)e.NewValue;
        }
    }
}
