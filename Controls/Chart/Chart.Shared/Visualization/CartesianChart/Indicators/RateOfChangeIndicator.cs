namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Visualizes a collection of DataPoints, using a <see cref="Windows.UI.Xaml.Shapes.Line"/> shape. This class represents the Momentum oscillator.
    /// </summary>
    public class RateOfChangeIndicator : MomentumIndicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RateOfChangeIndicator"/> class.
        /// </summary>
        public RateOfChangeIndicator()
        {
            this.DefaultStyleKey = typeof(RateOfChangeIndicator);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        public override string ToString()
        {
            return "Rate Of Change (" + this.Period + ")";
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new RateOfChangeIndicatorDataSource();
        }
   }
}
