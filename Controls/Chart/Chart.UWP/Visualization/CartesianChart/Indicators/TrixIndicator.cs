namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// This class represents the TRIX financial indicator.
    /// </summary>
    public class TrixIndicator : ValuePeriodIndicatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrixIndicator"/> class.
        /// </summary>
        public TrixIndicator()
        {
            this.DefaultStyleKey = typeof(TrixIndicator);
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new TrixIndicatorDataSource();
        }
    }
}
