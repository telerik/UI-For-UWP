using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a base class for financial indicators whose value depends on one input value (Open, High, Low, Close) and Period.
    /// </summary>
    public abstract class ValuePeriodIndicatorBase : ValueIndicatorBase
    {
        /// <summary>
        /// Identifies the <see cref="Period"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PeriodProperty = 
            DependencyProperty.Register(nameof(Period), typeof(int), typeof(ValuePeriodIndicatorBase), new PropertyMetadata(0, OnPeriodChanged));

        /// <summary>
        /// Gets or sets the indicator period.
        /// </summary>
        /// <value>The period.</value>
        public int Period
        {
            get
            {
                return (int)this.GetValue(PeriodProperty);
            }
            set
            {
                this.SetValue(PeriodProperty, value);
            }
        }

        private static void OnPeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValuePeriodIndicatorBase presenter = d as ValuePeriodIndicatorBase;
            (presenter.dataSource as ValuePeriodIndicatorDataSourceBase).Period = (int)e.NewValue;
        }
    }
}
