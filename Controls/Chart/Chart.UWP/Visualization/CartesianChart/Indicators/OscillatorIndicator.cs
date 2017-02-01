namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Visualizes a collection of DataPoints, using a <see cref="Windows.UI.Xaml.Shapes.Line"/> shape. This class represents the Moving Average financial indicator. Its values are defined as the average value of the last points.
    /// </summary>
    public class OscillatorIndicator : ShortLongPeriodIndicatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OscillatorIndicator"/> class.
        /// </summary>
        public OscillatorIndicator()
        {
            this.DefaultStyleKey = typeof(OscillatorIndicator);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        public override string ToString()
        {
            return "Oscillator (" + this.LongPeriod + ", " + this.ShortPeriod + ")";
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new OscillatorIndicatorDataSource();
        }
    }
}
