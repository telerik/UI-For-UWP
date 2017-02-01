namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Visualizes a collection of DataPoints, using a <see cref="Windows.UI.Xaml.Shapes.Line"/> shape. This class represents the Modified Exponential Moving Average financial indicator. Its values are defined as the average of the exponentially weighted values of the last points.
    /// </summary>
    public class ModifiedExponentialMovingAverageIndicator : ExponentialMovingAverageIndicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiedExponentialMovingAverageIndicator"/> class.
        /// </summary>
        public ModifiedExponentialMovingAverageIndicator()
        {
            this.DefaultStyleKey = typeof(ModifiedExponentialMovingAverageIndicator);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        public override string ToString()
        {
            return "Modified Exponential Moving Average (" + this.Period + ")";
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new ExponentialMovingAverageIndicatorDataSource() { IsModified = true };
        }
    }
}
