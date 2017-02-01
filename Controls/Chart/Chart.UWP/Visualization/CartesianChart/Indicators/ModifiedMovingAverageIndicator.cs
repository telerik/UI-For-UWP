namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Visualizes a collection of DataPoints, using a <see cref="Windows.UI.Xaml.Shapes.Line"/> shape. This class represents the Modified Moving Average financial indicator.
    /// </summary>
    public class ModifiedMovingAverageIndicator : ValuePeriodIndicatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiedMovingAverageIndicator"/> class.
        /// </summary>
        public ModifiedMovingAverageIndicator()
        {
            this.DefaultStyleKey = typeof(ModifiedMovingAverageIndicator);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        public override string ToString()
        {
            return "Modified Moving Average (" + this.Period + ")";
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new ModifiedMovingAverageIndicatorDataSource();
        }
    }
}
