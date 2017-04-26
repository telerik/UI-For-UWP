using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadPolarChart"/>.
    /// </summary>
    public class RadPolarChartAutomationPeer : RadChartBaseAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadPolarChartAutomationPeer class.
        /// </summary>
        public RadPolarChartAutomationPeer(RadPolarChart owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.RadPolarChart);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.RadPolarChart);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad polar chart";
        }
    }
}
