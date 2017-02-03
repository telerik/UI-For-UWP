using Telerik.Charting;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a base class for financial indicators whose value depends on one input value (Open, High, Low, Close).
    /// </summary>
    public abstract class ValueIndicatorBase : LineIndicatorBase
    {
        /// <summary>
        /// Identifies the <see cref="ValueBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty ValueBindingProperty =
            DependencyProperty.Register(nameof(ValueBinding), typeof(DataPointBinding), typeof(ValueIndicatorBase), new PropertyMetadata(null, OnValueBindingChanged));

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

        private static void OnValueBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorBase presenter = d as IndicatorBase;
            (presenter.dataSource as CategoricalSeriesDataSource).ValueBinding = e.NewValue as DataPointBinding;
        }
    }
}
