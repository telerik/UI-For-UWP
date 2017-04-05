using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    public class PolarChartGridAutomationPeer : RadControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the PolarChartGridAutomationPeer class.
        /// </summary>
        public PolarChartGridAutomationPeer(PolarChartGrid owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(PolarChartGrid);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(PolarChartGrid);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "polar chart grid";
        }
    }
}
