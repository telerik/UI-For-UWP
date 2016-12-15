namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Visualizes a collection of DataPoints, using a <see cref="Windows.UI.Xaml.Shapes.Line"/> shape. This class represents the Range Action Verification Index financial indicator.
    /// </summary>
    public class RaviIndicator : ShortLongPeriodIndicatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RaviIndicator"/> class.
        /// </summary>
        public RaviIndicator()
        {
            this.DefaultStyleKey = typeof(RaviIndicator);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        public override string ToString()
        {
            return "RAVI (" + this.LongPeriod + ", " + this.ShortPeriod + ")";
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new RaviIndicatorDataSource();
        }
    }
}
