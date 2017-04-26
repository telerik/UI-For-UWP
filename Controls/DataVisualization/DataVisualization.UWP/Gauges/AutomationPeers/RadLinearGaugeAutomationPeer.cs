using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadLinearGauge"/>.
    /// </summary>
    public class RadLinearGaugeAutomationPeer : RadGaugeAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadLinearGaugeAutomationPeer class.
        /// </summary>
        /// <param name="owner">The RadLinearGauge that is associated with this RadLinearGaugeAutomationPeer.</param>
        public RadLinearGaugeAutomationPeer(RadLinearGauge owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.RadLinearGauge);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.RadLinearGauge);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad linear gauge";
        }
    }
}
