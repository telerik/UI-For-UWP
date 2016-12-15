namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents the AverageTrueRange financial indicator.
    /// </summary>
    public class AverageTrueRangeIndicator : HighLowClosePeriodIndicatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AverageTrueRangeIndicator" /> class.
        /// </summary>
        public AverageTrueRangeIndicator()
        {
            this.DefaultStyleKey = typeof(AverageTrueRangeIndicator);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        public override string ToString()
        {
            return "Average True Range (" + this.Period + ")";
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new AverageTrueRangeIndicatorDataSource();
        }
    }
}
