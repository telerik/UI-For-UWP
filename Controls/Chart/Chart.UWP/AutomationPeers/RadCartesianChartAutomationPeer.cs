using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadCartesianChart"/>.
    /// </summary>
    public class RadCartesianChartAutomationPeer : RadChartBaseAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadPieChartAutomationPeer class.
        /// </summary>
        public RadCartesianChartAutomationPeer(RadCartesianChart owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(RadCartesianChart);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(RadCartesianChart);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad cartesian chart";
        }
    }
}
