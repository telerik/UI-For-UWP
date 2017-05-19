using System;
using System.Collections;
using Telerik.Charting;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a <see cref="RadChartBase"/> instance that uses Polar coordinate system to plot data points.
    /// </summary>
    [ContentProperty(Name = "Series")]
    public class RadPolarChart : RadChartBase
    {
        /// <summary>
        /// Identifies the <see cref="PolarAxis"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PolarAxisProperty =
            DependencyProperty.Register(nameof(PolarAxis), typeof(PolarAxis), typeof(RadPolarChart), new PropertyMetadata(null, OnPolarAxisChanged));

        /// <summary>
        /// Identifies the <see cref="RadialAxis"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadialAxisProperty =
            DependencyProperty.Register(nameof(RadialAxis), typeof(RadialAxis), typeof(RadPolarChart), new PropertyMetadata(null, OnRadialAxisChanged));

        /// <summary>
        /// Identifies the <see cref="StartAngle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register(nameof(StartAngle), typeof(double), typeof(RadPolarChart), new PropertyMetadata(0d, OnStartAngleChanged));

        private PolarAxis polarAxisCache;
        private RadialAxis radialAxisCache;
        private PolarChartGrid grid;
        private PolarSeriesCollection series;
        private PolarAnnotationCollection annotations;
        private TranslateTransform plotOriginTransform;

        private LegendItemCollection legendInfos = new LegendItemCollection();

        /// <summary>
        /// Initializes a new instance of the <see cref="RadPolarChart"/> class.
        /// </summary>
        public RadPolarChart()
        {
            this.DefaultStyleKey = typeof(RadPolarChart);

            this.series = new PolarSeriesCollection(this);
            this.annotations = new PolarAnnotationCollection(this);
            this.plotOriginTransform = new TranslateTransform();
        }

        /// <summary>
        /// Gets or sets the angle at which the polar axis is anchored. The angle is measured counter-clockwise, starting from the right side of the ellipse.
        /// </summary>
        public double StartAngle
        {
            get
            {
                return (this.chartArea as PolarChartAreaModel).StartAngle;
            }
            set
            {
                this.SetValue(StartAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="PolarChartGrid"/> used to decorate the chart plot area with polar and angle lines.
        /// </summary>
        public PolarChartGrid Grid
        {
            get
            {
                return this.grid;
            }
            set
            {
                if (this.grid != null)
                {
                    this.OnPresenterRemoved(this.grid);
                }

                this.grid = value;

                if (this.grid != null)
                {
                    this.OnPresenterAdded(this.grid);
                }
            }
        }

        /// <summary>
        /// Gets the collection containing all the series presented by this instance.
        /// </summary>
        public PolarSeriesCollection Series
        {
            get
            {
                return this.series;
            }
        }

        /// <summary>
        /// Gets or sets the visual <see cref="PolarAxis"/> instance that will be used to plot points along the polar (radius) axis.
        /// </summary>
        public PolarAxis PolarAxis
        {
            get
            {
                return this.polarAxisCache;
            }
            set
            {
                this.SetValue(PolarAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the visual <see cref="RadialAxis"/> instance that will be used to plot points along the vertical (Y) axis.
        /// </summary>
        public RadialAxis RadialAxis
        {
            get
            {
                return this.radialAxisCache;
            }
            set
            {
                this.SetValue(RadialAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets the collection containing all the annotations presented by this instance.
        /// </summary>
        /// <value>The annotations.</value>
        public PolarAnnotationCollection Annotations
        {
            get
            {
                return this.annotations;
            }
        }

        internal override IList SeriesInternal
        {
            get
            {
                return this.series;
            }
        }

        internal override LegendItemCollection LegendInfosInternal
        {
            get
            {
                return this.legendInfos;
            }
        }

        /// <summary>
        /// Converts the specified physical coordinates in pixels to data using the primary chart axes (if any).
        /// </summary>
        /// <param name="coordinates">The physical coordinates.</param>
        public Tuple<object, object> ConvertPointToData(Point coordinates)
        {
            RadPoint point = new RadPoint(coordinates.X, coordinates.Y);
            return (this.chartArea as PolarChartAreaModel).ConvertPointToData(point);
        }

        /// <summary>
        /// Converts the specified data point coordinates to physical coordinates (in pixels) using the primary chart axes (if any).
        /// </summary>
        /// <param name="data">The data point coordinates according to the primary chart axes (if any).</param>
        public Point ConvertDataToPoint(Tuple<object, object> data)
        {
            if (data == null)
            {
                return new Point(double.NaN, double.NaN);
            }

            RadPoint coordinates = (this.chartArea as PolarChartAreaModel).ConvertDataToPoint(data);
            return coordinates.ToPoint();
        }

        internal override void OnPlotOriginChanged()
        {
            base.OnPlotOriginChanged();

            Point plotOrigin = this.PlotOrigin;
            this.plotOriginTransform.X = plotOrigin.X;
            this.plotOriginTransform.Y = plotOrigin.Y;
        }

        internal override ChartAreaModel CreateChartAreaModel()
        {
            return new PolarChartAreaModel();
        }

        /// <summary>
        /// Applies the Pan transformation to the render surface.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            if (applied)
            {
                this.renderSurface.RenderTransform = this.plotOriginTransform;
            }

            return applied;
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadPolarChartAutomationPeer(this);
        }

        private static void OnPolarAxisChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            RadPolarChart chart = target as RadPolarChart;
            PolarAxis oldAxis = args.OldValue as PolarAxis;
            PolarAxis newAxis = args.NewValue as PolarAxis;

            chart.polarAxisCache = newAxis;
            if (chart.polarAxisCache != null)
            {
                chart.polarAxisCache.type = AxisType.First;
            }

            chart.OnAxisChanged(oldAxis, newAxis);
        }

        private static void OnRadialAxisChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            RadPolarChart chart = target as RadPolarChart;
            RadialAxis oldAxis = args.OldValue as RadialAxis;
            RadialAxis newAxis = args.NewValue as RadialAxis;

            chart.radialAxisCache = newAxis;
            if (chart.radialAxisCache != null)
            {
                chart.radialAxisCache.type = AxisType.Second;
            }

            chart.OnAxisChanged(oldAxis, newAxis);
        }

        private static void OnStartAngleChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            RadPolarChart chart = target as RadPolarChart;
            (chart.chartArea as PolarChartAreaModel).StartAngle = (double)args.NewValue;
        }

        private void OnAxisChanged(Axis oldAxis, Axis newAxis)
        {
            foreach (var seriesItem in this.series)
            {
                seriesItem.ChartAxisChanged(oldAxis, newAxis);
            }

            foreach (var annotation in this.annotations)
            {
                annotation.ChartAxisChanged(oldAxis, newAxis);
            }

            if (oldAxis != null)
            {
                this.OnPresenterRemoved(oldAxis);
                oldAxis.model.isPrimary = false;
            }

            if (newAxis != null)
            {
                newAxis.model.isPrimary = true;
                this.OnPresenterAdded(newAxis);
            }
        }
    }
}