namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents the CommodityChannelIndicator.
    /// </summary>
    public class CommodityChannelIndexIndicator : HighLowClosePeriodIndicatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommodityChannelIndexIndicator" /> class.
        /// </summary>
        public CommodityChannelIndexIndicator()
        {
            this.DefaultStyleKey = typeof(CommodityChannelIndexIndicator);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        public override string ToString()
        {
            return "Commodity Channel Index (" + this.Period + ")";
        }
        
        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new CommodityChannelIndicatorDataSource();
        }
    }
}
