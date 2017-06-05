using System.Diagnostics.CodeAnalysis;
using Telerik.Charting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// A base class for chart range series.
    /// </summary>
    [ContentProperty(Name = "DataPoints")]
    public abstract class RangeSeriesBase : CartesianSeries
    {
        /// <summary>
        /// Identifies the <see cref="CategoryBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty CategoryBindingProperty =
            DependencyProperty.Register(nameof(CategoryBinding), typeof(DataPointBinding), typeof(RangeSeriesBase), new PropertyMetadata(null, OnCategoryBindingChanged));

        /// <summary>
        /// Identifies the <see cref="LowBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty LowBindingProperty =
            DependencyProperty.Register(nameof(LowBinding), typeof(DataPointBinding), typeof(RangeSeriesBase), new PropertyMetadata(null, OnLowBindingChanged));

        /// <summary>
        /// Identifies the <see cref="HighBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty HighBindingProperty =
            DependencyProperty.Register(nameof(HighBinding), typeof(DataPointBinding), typeof(RangeSeriesBase), new PropertyMetadata(null, OnHighBindingChanged));

        internal RangeSeriesModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeSeriesBase"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "These virtual calls do not rely on uninitialized base state.")]
        internal RangeSeriesBase()
        {
            this.model = this.CreateModel();
        }

        /// <summary>
        /// Gets the collection of data points associated with the series.
        /// </summary>
        public DataPointCollection<RangeDataPoint> DataPoints
        {
            get
            {
                return (this.Model as RangeSeriesModel).DataPoints;
            }
        }

        /// <summary>
        /// Gets or sets the binding that will be used to fill the <see cref="RangeSeriesBase.CategoryBinding"/> member of the contained data points.
        /// </summary>
        public DataPointBinding CategoryBinding
        {
            get
            {
                return (DataPointBinding)this.GetValue(CategoryBindingProperty);
            }
            set
            {
                this.SetValue(CategoryBindingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the binding that will be used to fill the <see cref="RangeSeriesBase.LowBinding"/> member of the contained data points.
        /// </summary>
        public DataPointBinding LowBinding
        {
            get
            {
                return (DataPointBinding)this.GetValue(LowBindingProperty);
            }
            set
            {
                this.SetValue(LowBindingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the binding that will be used to fill the <see cref="RangeSeriesBase.HighBinding"/> member of the contained data points.
        /// </summary>
        public DataPointBinding HighBinding
        {
            get
            {
                return (DataPointBinding)this.GetValue(HighBindingProperty);
            }
            set
            {
                this.SetValue(HighBindingProperty, value);
            }
        }

        internal override ChartSeriesModel Model
        {
            get
            {
                return this.model;
            }
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new RangeSeriesDataSource();
        }

        internal abstract RangeSeriesModel CreateModel();

        private static void OnCategoryBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeSeriesBase presenter = d as RangeSeriesBase;
            (presenter.dataSource as RangeSeriesDataSource).CategoryBinding = e.NewValue as DataPointBinding;
        }

        private static void OnLowBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeSeriesBase presenter = d as RangeSeriesBase;
            (presenter.dataSource as RangeSeriesDataSource).LowBinding = e.NewValue as DataPointBinding;
        }

        private static void OnHighBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeSeriesBase presenter = d as RangeSeriesBase;
            (presenter.dataSource as RangeSeriesDataSource).HighBinding = e.NewValue as DataPointBinding;
        }
    }
}
