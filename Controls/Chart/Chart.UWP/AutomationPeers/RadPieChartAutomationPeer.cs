using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    public class RadPieChartAutomationPeer : RadChartBaseAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadPieChartAutomationPeer class.
        /// </summary>
        public RadPieChartAutomationPeer(RadPieChart owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(RadPieChart);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(RadPieChart);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad pie chart";
        }
    }
}
