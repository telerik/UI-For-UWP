namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Visualizes a collection of DataPoints, using a <see cref="Windows.UI.Xaml.Shapes.Line"/> shape. 
    /// This class represents the Weighted Moving Average financial indicator. 
    /// </summary>
    public class WeightedMovingAverageIndicator : ValuePeriodIndicatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeightedMovingAverageIndicator" /> class.
        /// </summary>
        public WeightedMovingAverageIndicator()
        {
            this.DefaultStyleKey = typeof(WeightedMovingAverageIndicator);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        public override string ToString()
        {
            return "Weighted Moving Average Indicator (" + this.Period + ")";
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new WeightedMovingAverageIndicatorDataSource();
        }
    }
}
