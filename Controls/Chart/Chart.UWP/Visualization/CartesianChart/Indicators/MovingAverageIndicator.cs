namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Visualizes a collection of DataPoints, using a <see cref="Windows.UI.Xaml.Shapes.Line"/> shape. This class represents the Moving Average financial indicator. Its values are defined as the average value of the last points.
    /// </summary>
    public class MovingAverageIndicator : ValuePeriodIndicatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovingAverageIndicator"/> class.
        /// </summary>
        public MovingAverageIndicator()
        {
            this.DefaultStyleKey = typeof(MovingAverageIndicator);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        public override string ToString()
        {
            return "Moving Average (" + this.Period + ")";
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new MovingAverageIndicatorDataSource();
        }
    }
}
