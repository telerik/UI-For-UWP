using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadialBarGaugeIndicator"/>.
    /// </summary>
    public class RadialBarGaugeIndicatorAutomationPeer : RadialGaugeIndicatorAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadialBarGaugeIndicatorAutomationPeer class.
        /// </summary>
        public RadialBarGaugeIndicatorAutomationPeer(RadialBarGaugeIndicator owner)
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.RadialBarGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.RadialBarGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "radial bar gauge indicator";
        }
    }
}
