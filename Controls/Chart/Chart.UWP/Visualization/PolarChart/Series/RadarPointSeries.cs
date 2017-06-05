using System;
using Telerik.Charting;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Markup;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents <see cref="CartesianSeries"/> which may visualize <see cref="CategoricalDataPoint"/> objects in <see cref="RadPolarChart"/>.
    /// </summary>
    [ContentProperty(Name = "DataPoints")]
    public class RadarPointSeries : PolarSeries
    {
        /// <summary>
        /// Identifies the <see cref="CategoryBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty CategoryBindingProperty =
            DependencyProperty.Register(nameof(CategoryBinding), typeof(DataPointBinding), typeof(RadarPointSeries), new PropertyMetadata(null, RadarPointSeries.OnCategoryBindingChanged));

        /// <summary>
        /// Identifies the <see cref="CombineMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CombineModeProperty =
            DependencyProperty.Register(nameof(CombineMode), typeof(ChartSeriesCombineMode), typeof(RadarPointSeries), new PropertyMetadata(ChartSeriesCombineMode.None, OnCombineModePropertyChanged));

        private RadarSeriesModel model;
        private ChartSeriesCombineMode combineModeCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadarPointSeries"/> class.
        /// </summary>
        public RadarPointSeries()
        {
            this.DefaultStyleKey = typeof(RadarPointSeries);

            this.model = new RadarSeriesModel();
        }

        /// <summary>
        /// Gets or sets the binding that will be used to fill the <see cref="CategoricalDataPointBase.Category"/> member of the contained data points.
        /// </summary>
        public DataPointBinding CategoryBinding
        {
            get
            {
                return this.GetValue(CategoryBindingProperty) as DataPointBinding;
            }
            set
            {
                this.SetValue(CategoryBindingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the combination mode to be used when data points are plotted.
        /// </summary>
        public ChartSeriesCombineMode CombineMode
        {
            get
            {
                return this.combineModeCache;
            }
            set
            {
                this.SetValue(CombineModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the key that defines in which stack group this series will be included if its <see cref="CombineMode"/> equals Stack or Stack100.
        /// </summary>
        public object StackGroupKey
        {
            get
            {
                return (this.Model as RadarSeriesModel).StackGroupKey;
            }
            set
            {
                (this.Model as RadarSeriesModel).StackGroupKey = value;
            }
        }

        /// <summary>
        /// Gets the collection of data points associated with the series.
        /// </summary>
        public DataPointCollection<CategoricalDataPoint> DataPoints
        {
            get
            {
                return this.model.DataPoints;
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
        /// Creates the concrete data source for this instance.
        /// </summary>
        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new CategoricalSeriesDataSource();
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadarPointSeriesAutomationPeer(this);
        }

        /// <summary>
        /// Called when <seealso cref="PolarSeries.ValueBinding" /> has changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnValueBindingChanged(DataPointBinding oldValue, DataPointBinding newValue)
        {
            (this.dataSource as CategoricalSeriesDataSource).ValueBinding = newValue;
        }

        private static void OnCategoryBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadarPointSeries presenter = d as RadarPointSeries;
            (presenter.dataSource as CategoricalSeriesDataSourceBase).CategoryBinding = e.NewValue as DataPointBinding;
        }

        private static void OnCombineModePropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            var series = target as RadarPointSeries;
            series.combineModeCache = (ChartSeriesCombineMode)args.NewValue;

            (series.Model as RadarSeriesModel).CombineMode = series.CombineMode;
        }
    }
}
