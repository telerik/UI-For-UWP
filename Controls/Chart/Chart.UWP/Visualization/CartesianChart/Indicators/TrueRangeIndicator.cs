namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Visualizes a collection of DataPoints, using a <see cref="Windows.UI.Xaml.Shapes.Line"/> shape. This class represents the True Range oscillator.
    /// </summary>
    public class TrueRangeIndicator : HighLowCloseIndicatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrueRangeIndicator"/> class.
        /// </summary>
        public TrueRangeIndicator()
        {
            this.DefaultStyleKey = typeof(TrueRangeIndicator);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        public override string ToString()
        {
            return "True Range";
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new TrueRangeIndicatorDataSource();
        }
    }
}
