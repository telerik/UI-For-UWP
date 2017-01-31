namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Visualizes a collection of DataPoints, using a <see cref="Windows.UI.Xaml.Shapes.Line"/> shape. 
    /// This class represents the RelativeStrengthIndex financial indicator. 
    /// </summary>
    public class RelativeStrengthIndexIndicator : ValuePeriodIndicatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelativeStrengthIndexIndicator"/> class.
        /// </summary>
        public RelativeStrengthIndexIndicator()
        {
            this.DefaultStyleKey = typeof(RelativeStrengthIndexIndicator);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        public override string ToString()
        {
            return "Relative Strength Index (" + this.Period + ")";
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new RelativeStrengthIndexIndicatorDataSource();
        }
    }
}
