using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    public class PieSeriesAutomationPeer : ChartSeriesAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the PieSeriesAutomationPeer class.
        /// </summary>
        public PieSeriesAutomationPeer(PieSeries owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(PieSeries);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(PieSeries);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "pie series";
        }
    }
}
