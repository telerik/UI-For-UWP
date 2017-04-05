using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    public class RadRadialGaugeAutomationPeer : RadGaugeAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadRadialGaugeAutomationPeer class.
        /// </summary>
        /// <param name="owner">The RadRadialGauge that is associated with this RadRadialGaugeAutomationPeer.</param>
        public RadRadialGaugeAutomationPeer(RadRadialGauge owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(RadRadialGauge);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(RadRadialGauge);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad radial gauge";
        }
    }
}
