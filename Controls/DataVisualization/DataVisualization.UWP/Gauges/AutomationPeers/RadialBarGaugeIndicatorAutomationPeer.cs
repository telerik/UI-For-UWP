using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    public class RadialBarGaugeIndicatorAutomationPeer : RadialGaugeIndicatorAutomationPeer
    {
        public RadialBarGaugeIndicatorAutomationPeer(RadialBarGaugeIndicator owner)
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(RadialBarGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(RadialBarGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "radial bar gauge indicator";
        }
    }
}
