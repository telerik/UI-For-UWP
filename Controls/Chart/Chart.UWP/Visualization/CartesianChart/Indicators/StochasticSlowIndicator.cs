using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Base class for financial indicators that are calculated upon the High, Low and Close values and define a period.
    /// </summary>
    public class StochasticSlowIndicator : StochasticFastIndicator
    {
        /// <summary>
        /// Identifies the <see cref="SlowingPeriod"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SlowingPeriodProperty = 
            DependencyProperty.Register(nameof(SlowingPeriod), typeof(int), typeof(StochasticSlowIndicator), new PropertyMetadata(0, OnSlowingPeriodChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="StochasticSlowIndicator"/> class.
        /// </summary>
        public StochasticSlowIndicator()
        {
            this.DefaultStyleKey = typeof(StochasticSlowIndicator);
        }

        /// <summary>
        /// Gets or sets the indicator slowing period.
        /// </summary>
        /// <value>The period.</value>
        public int SlowingPeriod
        {
            get
            {
                return (int)this.GetValue(SlowingPeriodProperty);
            }
            set
            {
                this.SetValue(SlowingPeriodProperty, value);
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        public override string ToString()
        {
            return "Stochastic Slow (" + this.MainPeriod + ", " + this.SignalPeriod + ", " + this.SlowingPeriod + ")";
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new StochasticSlowIndicatorDataSource();
        }

        private static void OnSlowingPeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StochasticSlowIndicator presenter = d as StochasticSlowIndicator;
            (presenter.dataSource as StochasticSlowIndicatorDataSource).SlowingPeriod = (int)e.NewValue;
        }
    }
}
