using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadialGaugeIndicator"/>.
    /// </summary>
    public class RadialGaugeIndicatorAutomationPeer : BarGaugeIndicatorAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadialGaugeIndicatorAutomationPeer class.
        /// </summary>
        public RadialGaugeIndicatorAutomationPeer(RadialGaugeIndicator owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.RadialGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.RadialGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "radial gauge indicator";
        }
    }
}
