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

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a <see cref="RadChartBase"/> instance that uses a Cartesian Coordinate System to plot the associated data points.
    /// </summary>
    [ContentProperty(Name = "Series")]
    public class RadCartesianChart : RadChartBase
    {
        /// <summary>
        /// Identifies the <see cref="HorizontalAxis"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalAxisProperty =
            DependencyProperty.Register(nameof(HorizontalAxis), typeof(CartesianAxis), typeof(RadCartesianChart), new PropertyMetadata(null, OnHorizontalAxisChanged));

        /// <summary>
        /// Identifies the <see cref="VerticalAxis"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalAxisProperty =
            DependencyProperty.Register(nameof(VerticalAxis), typeof(CartesianAxis), typeof(RadCartesianChart), new PropertyMetadata(null, OnVerticalAxisChanged));

        internal const int AxisZIndex = 300;

        private CartesianAxis horizontalAxisCache;
        private CartesianAxis verticalAxisCache;
        private CartesianChartGrid grid;
        private CartesianSeriesCollection series;
        private IndicatorCollection indicators;
        private CartesianAnnotationCollection annotations;

        private LegendItemCollection legendInfos;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadCartesianChart"/> class.
        /// </summary>
        public RadCartesianChart()
        {
            this.DefaultStyleKey = typeof(RadCartesianChart);

            this.series = new CartesianSeriesCollection(this);
            this.indicators = new IndicatorCollection(this);
            this.annotations = new CartesianAnnotationCollection(this);

            this.legendInfos = new LegendItemCollection();
        }

        /// <summary>
        /// Gets the collection containing all the series presented by this instance.
        /// </summary>
        public CartesianSeriesCollection Series
        {
            get
            {
                return this.series;
            }
        }

        /// <summary>
        /// Gets the collection containing all the indicators presented by this instance.
        /// </summary>
        public IndicatorCollection Indicators
        {
            get
            {
                return this.indicators;
            }
        }

        /// <summary>
        /// Gets the collection containing all the annotations presented by this instance.
        /// </summary>
        public CartesianAnnotationCollection Annotations
        {
            get
            {
                return this.annotations;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="CartesianChartGrid"/> used to decorate the chart plot area with major/minor grid and strip lines.
        /// </summary>
        public CartesianChartGrid Grid
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
        /// Gets or sets the visual <see cref="Axis"/> instance that will be used to plot points along the horizontal (X) axis.
        /// </summary>
        public CartesianAxis HorizontalAxis
        {
            get
            {
                return this.horizontalAxisCache;
            }
            set
            {
                this.SetValue(HorizontalAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the visual <see cref="Axis"/> instance that will be used to plot points along the vertical (Y) axis.
        /// </summary>
        public CartesianAxis VerticalAxis
        {
            get
            {
                return this.verticalAxisCache;
            }
            set
            {
                this.SetValue(VerticalAxisProperty, value);
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
            return (this.chartArea as CartesianChartAreaModel).ConvertPointToData(point);
        }

        /// <summary>
        /// Converts the specified physical coordinates in pixels to data using the specified chart axes.
        /// </summary>
        /// <param name="coordinates">The physical coordinates.</param>
        /// <param name="horizontalAxis">The horizontal axis.</param>
        /// <param name="verticalAxis">The vertical axis.</param>
        public Tuple<object, object> ConvertPointToData(Point coordinates, Axis horizontalAxis, Axis verticalAxis)
        {
            if (horizontalAxis == null || verticalAxis == null || this.chartArea == null)
            { 
                return new Tuple<object, object>(null, null);
            }

            RadPoint point = new RadPoint(coordinates.X, coordinates.Y);
            var chartArea = this.chartArea as CartesianChartAreaModel;
          
            return chartArea.ConvertPointToData(point, horizontalAxis.model, verticalAxis.model);
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

            RadPoint coordinates = (this.chartArea as CartesianChartAreaModel).ConvertDataToPoint(data);
            return coordinates.ToPoint();
        }

        /// <summary>
        /// Converts the specified data point coordinates to physical coordinates (in pixels) using the specified chart axes.
        /// </summary>
        /// <param name="data">The data point coordinates according to the specified chart axes.</param>
        /// <param name="horizontalAxis">The horizontal axis.</param>
        /// <param name="verticalAxis">The vertical axis.</param>
        public Point ConvertDataToPoint(Tuple<object, object> data, Axis horizontalAxis, Axis verticalAxis)
        {
            if (data == null || horizontalAxis == null || verticalAxis == null)
            {
                return new Point(double.NaN, double.NaN);
            }

            RadPoint coordinates = (this.chartArea as CartesianChartAreaModel).ConvertDataToPoint(data, horizontalAxis.model, verticalAxis.model);
            return coordinates.ToPoint();
        }

        /// <summary>
        /// Creates the model of the plot area.
        /// </summary>
        internal override ChartAreaModel CreateChartAreaModel()
        {
            return new CartesianChartAreaModel();
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadCartesianChartAutomationPeer(this);
        }

        /// <summary>
        /// Exposed for testing purposes.
        /// </summary>
        internal void SetTestChartArea(CartesianChartAreaModel testModel)
        {
            this.chartArea = testModel;
        }

        private static void OnHorizontalAxisChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            RadCartesianChart chart = target as RadCartesianChart;
            CartesianAxis oldAxis = args.OldValue as CartesianAxis;
            CartesianAxis newAxis = args.NewValue as CartesianAxis;

            chart.horizontalAxisCache = newAxis;
            if (chart.horizontalAxisCache != null)
            {
                chart.horizontalAxisCache.type = AxisType.First;
            }

            chart.OnAxisChanged(oldAxis, newAxis);
        }

        private static void OnVerticalAxisChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            RadCartesianChart chart = target as RadCartesianChart;
            CartesianAxis oldAxis = args.OldValue as CartesianAxis;
            CartesianAxis newAxis = args.NewValue as CartesianAxis;

            chart.verticalAxisCache = newAxis;
            if (chart.verticalAxisCache != null)
            {
                chart.verticalAxisCache.type = AxisType.Second;
            }

            chart.OnAxisChanged(oldAxis, newAxis);
        }

        private void OnAxisChanged(CartesianAxis oldAxis, CartesianAxis newAxis)
        {
            foreach (var seriesItem in this.series)
            {
                seriesItem.ChartAxisChanged(oldAxis, newAxis);
            }
            foreach (var indicator in this.indicators)
            {
                indicator.ChartAxisChanged(oldAxis, newAxis);
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