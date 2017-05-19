using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadPieChart"/>.
    /// </summary>
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
            return nameof(Telerik.UI.Xaml.Controls.Chart.RadPieChart);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.RadPieChart);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad pie chart";
        }
    }
}
