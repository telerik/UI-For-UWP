using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    public class CandlestickSeriesAutomationPeer : ChartSeriesAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the CandlestickSeriesAutomationPeer class.
        /// </summary>
        public CandlestickSeriesAutomationPeer(CandlestickSeries owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(CandlestickSeries);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(CandlestickSeries);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "candle stick series";
        }
    }
}
