using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Visualizes a collection of DataPoints, using a <see cref="Windows.UI.Xaml.Shapes.Line"/> shape. This class represents the Momentum oscillator.
    /// </summary>
    public class UltimateOscillatorIndicator : HighLowClosePeriodIndicatorBase
    {
        /// <summary>
        /// Identifies the <see cref="Period2"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Period2Property = 
            DependencyProperty.Register(nameof(Period2), typeof(int), typeof(UltimateOscillatorIndicator), new PropertyMetadata(0, OnPeriod2Changed));

        /// <summary>
        /// Identifies the <see cref="Period3"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Period3Property = 
            DependencyProperty.Register(nameof(Period3), typeof(int), typeof(UltimateOscillatorIndicator), new PropertyMetadata(0, OnPeriod3Changed));

        /// <summary>
        /// Initializes a new instance of the <see cref="UltimateOscillatorIndicator"/> class.
        /// </summary>
        public UltimateOscillatorIndicator()
        {
            this.DefaultStyleKey = typeof(UltimateOscillatorIndicator);
        }
        
        /// <summary>
        /// Gets or sets the third period.
        /// </summary>
        /// <value>The third period.</value>
        public int Period3
        {
            get
            {
                return (int)this.GetValue(Period3Property);
            }
            set
            {
                this.SetValue(Period3Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the second period.
        /// </summary>
        /// <value>The second period.</value>
        public int Period2
        {
            get
            {
                return (int)this.GetValue(Period2Property);
            }
            set
            {
                this.SetValue(Period2Property, value);
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        public override string ToString()
        {
            return "Ultimate Oscillator (" + this.Period + ", " + this.Period2 + ", " + this.Period3 + ")";
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new UltimateOscillatorIndicatorDataSource();
        }

        private static void OnPeriod2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UltimateOscillatorIndicator presenter = d as UltimateOscillatorIndicator;
            (presenter.dataSource as UltimateOscillatorIndicatorDataSource).Period2 = (int)e.NewValue;
        }

        private static void OnPeriod3Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UltimateOscillatorIndicator presenter = d as UltimateOscillatorIndicator;
            (presenter.dataSource as UltimateOscillatorIndicatorDataSource).Period3 = (int)e.NewValue;
        }
    }
}
