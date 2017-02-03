using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Visualizes a collection of DataPoints, using a <see cref="Windows.UI.Xaml.Shapes.Line"/> shape. This class represents the Exponential Moving Average financial indicator. Its values are defined as the average of the exponentially weighted values of the last points.
    /// </summary>
    public class AdaptiveMovingAverageKaufmanIndicator : ValuePeriodIndicatorBase
    {
        /// <summary>
        /// Identifies the <see cref="SlowPeriod"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SlowPeriodProperty = 
            DependencyProperty.Register(nameof(SlowPeriod), typeof(int), typeof(AdaptiveMovingAverageKaufmanIndicator), new PropertyMetadata(0, OnSlowPeriodChanged));

        /// <summary>
        /// Identifies the <see cref="FastPeriod"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FastPeriodProperty = 
            DependencyProperty.Register(nameof(FastPeriod), typeof(int), typeof(AdaptiveMovingAverageKaufmanIndicator), new PropertyMetadata(0, OnFastPeriodChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="AdaptiveMovingAverageKaufmanIndicator" /> class.
        /// </summary>
        public AdaptiveMovingAverageKaufmanIndicator()
        {
            this.DefaultStyleKey = typeof(AdaptiveMovingAverageKaufmanIndicator);
        }

        /// <summary>
        /// Gets or sets the "SlowPeriod" parameter of the <see cref="AdaptiveMovingAverageKaufmanIndicator"/>.
        /// </summary>
        /// <value>The "SlowPeriod" value.</value>
        public int SlowPeriod
        {
            get
            {
                return (int)this.GetValue(SlowPeriodProperty);
            }
            set
            {
                this.SetValue(SlowPeriodProperty, value);
            }
        } 

        /// <summary>
        /// Gets or sets the "FastPeriod" parameter of the <see cref="AdaptiveMovingAverageKaufmanIndicator"/>.
        /// </summary>
        /// <value>The "FastPeriod" value.</value>
        public int FastPeriod
        {
            get
            {
                return (int)this.GetValue(FastPeriodProperty);
            }
            set
            {
                this.SetValue(FastPeriodProperty, value);
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        public override string ToString()
        {
            return "Adaptive Moving Average Kauman (" + this.Period + ", " + this.FastPeriod + ", " + this.SlowPeriod + ")";
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new AdaptiveMovingAverageKaufmanIndicatorDataSource();
        }

        private static void OnSlowPeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AdaptiveMovingAverageKaufmanIndicator presenter = d as AdaptiveMovingAverageKaufmanIndicator;
            (presenter.dataSource as AdaptiveMovingAverageKaufmanIndicatorDataSource).SlowPeriod = (int)e.NewValue;
        }

        private static void OnFastPeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AdaptiveMovingAverageKaufmanIndicator presenter = d as AdaptiveMovingAverageKaufmanIndicator;
            (presenter.dataSource as AdaptiveMovingAverageKaufmanIndicatorDataSource).FastPeriod = (int)e.NewValue;
        }
    }
}
