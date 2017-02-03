using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Base class for financial indicators that are calculated upon the High, Low and Close values and define a period.
    /// </summary>
    public abstract class HighLowClosePeriodIndicatorBase : HighLowCloseIndicatorBase
    {
        /// <summary>
        /// Identifies the <see cref="Period"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PeriodProperty = 
            DependencyProperty.Register(nameof(Period), typeof(int), typeof(HighLowClosePeriodIndicatorBase), new PropertyMetadata(0, OnPeriodChanged));

        /// <summary>
        /// Gets or sets the moving average period.
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
            HighLowClosePeriodIndicatorBase presenter = d as HighLowClosePeriodIndicatorBase;
            (presenter.dataSource as HighLowClosePeriodIndicatorDataSourceBase).Period = (int)e.NewValue;
        }
    }
}
