using System;
using Telerik.Charting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents <see cref="CartesianSeries"/> which may visualize <see cref="PolarDataPoint"/> objects in <see cref="RadPolarChart"/>.
    /// </summary>
    [ContentProperty(Name = "DataPoints")]
    public class PolarPointSeries : PolarSeries
    {
        /// <summary>
        /// Identifies the <see cref="AngleBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty AngleBindingProperty =
            DependencyProperty.Register(nameof(AngleBinding), typeof(DataPointBinding), typeof(PolarPointSeries), new PropertyMetadata(null, OnAngleBindingChanged));

        private PolarSeriesModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolarPointSeries"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public PolarPointSeries()
        {
            this.DefaultStyleKey = typeof(PolarPointSeries);

            this.model = new PolarSeriesModel();
        }

        /// <summary>
        /// Gets the collection of data points associated with the series.
        /// </summary>
        public DataPointCollection<PolarDataPoint> DataPoints
        {
            get
            {
                return this.model.DataPoints;
            }
        }

        /// <summary>
        /// Gets or sets the binding that will be used to fill the <see cref="PolarDataPoint.Angle"/> member of the contained data points.
        /// </summary>
        public DataPointBinding AngleBinding
        {
            get
            {
                return this.GetValue(AngleBindingProperty) as DataPointBinding;
            }
            set
            {
                this.SetValue(AngleBindingProperty, value);
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
            return new PolarSeriesDataSource();
        }

        /// <summary>
        /// Called when <seealso cref="PolarSeries.ValueBinding" /> has changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnValueBindingChanged(DataPointBinding oldValue, DataPointBinding newValue)
        {
            (this.dataSource as PolarSeriesDataSource).ValueBinding = newValue;
        }

        private static void OnAngleBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolarPointSeries presenter = d as PolarPointSeries;
            (presenter.dataSource as PolarSeriesDataSource).AngleBinding = e.NewValue as DataPointBinding;
        }
    }
}
