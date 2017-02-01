namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Visualizes a collection of DataPoints, using a <see cref="Windows.UI.Xaml.Shapes.Line"/> shape. This class represents the Momentum oscillator.
    /// </summary>
    public class MomentumIndicator : ValuePeriodIndicatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MomentumIndicator"/> class.
        /// </summary>
        public MomentumIndicator()
        {
            this.DefaultStyleKey = typeof(MomentumIndicator);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        public override string ToString()
        {
            return "Momentum (" + this.Period + ")";
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new MomentumIndicatorDataSource();
        }
    }
}
