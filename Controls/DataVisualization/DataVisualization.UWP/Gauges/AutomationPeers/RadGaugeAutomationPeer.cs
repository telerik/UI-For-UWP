using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    public class RadGaugeAutomationPeer : RadControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadGaugeAutomationPeer class.
        /// </summary>
        /// <param name="owner">The RadGauge that is associated with this RadGaugeAutomationPeer.</param>
        public RadGaugeAutomationPeer(RadGauge owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(RadGauge);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(RadGauge);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad gauge";
        }

        /// <inheritdoc />
        protected override bool HasKeyboardFocusCore()
        {
            return true;
        }

        /// <inheritdoc />
        protected override string GetAutomationIdCore()
        {
            var automationId = base.GetAutomationIdCore();
            if (!string.IsNullOrEmpty(automationId))
            {
                return automationId;
            }

            return nameof(RadGauge);
        }
    }
}
