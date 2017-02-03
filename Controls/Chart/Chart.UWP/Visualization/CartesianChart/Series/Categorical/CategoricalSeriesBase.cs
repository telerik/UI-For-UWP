using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Charting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Base class for all <see cref="ChartSeries"/> that may contain <see cref="CategoricalDataPoint"/>.
    /// </summary>
    [ContentProperty(Name = "DataPoints")]
    public abstract class CategoricalSeriesBase : CartesianSeries
    {
        /// <summary>
        /// Identifies the <see cref="ValueBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty ValueBindingProperty =
            DependencyProperty.Register(nameof(ValueBinding), typeof(DataPointBinding), typeof(CategoricalSeriesBase), new PropertyMetadata(null, OnValueBindingChanged));

        /// <summary>
        /// Identifies the <see cref="CategoryBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty CategoryBindingProperty =
            DependencyProperty.Register(nameof(CategoryBinding), typeof(DataPointBinding), typeof(CategoricalSeriesBase), new PropertyMetadata(null, OnCategoryBindingChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoricalSeriesBase"/> class.
        /// </summary>
        protected CategoricalSeriesBase()
        {
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
        /// Gets the collection of data points associated with the series.
        /// </summary>
        public DataPointCollection<CategoricalDataPoint> DataPoints
        {
            get
            {
                return (this.Model as CategoricalSeriesModel).DataPoints;
            }
        }

        /// <summary>
        /// Creates the concrete data source for this instance.
        /// </summary>
        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new CategoricalSeriesDataSource();
        }
        private static void OnValueBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CategoricalSeriesBase presenter = d as CategoricalSeriesBase;
            (presenter.dataSource as CategoricalSeriesDataSource).ValueBinding = e.NewValue as DataPointBinding;
        }

        private static void OnCategoryBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CategoricalSeriesBase presenter = d as CategoricalSeriesBase;
            (presenter.dataSource as CategoricalSeriesDataSourceBase).CategoryBinding = e.NewValue as DataPointBinding;
        }
    }
}
