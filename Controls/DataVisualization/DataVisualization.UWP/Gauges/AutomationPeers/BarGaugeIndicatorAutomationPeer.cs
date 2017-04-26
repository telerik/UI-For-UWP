using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="BarGaugeIndicator"/>.
    /// </summary>
    public class BarGaugeIndicatorAutomationPeer : GaugeIndicatorAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the BarGaugeIndicatorAutomationPeer class.
        /// </summary>
        public BarGaugeIndicatorAutomationPeer(BarGaugeIndicator owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.BarGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.BarGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "bar gauge indicator";
        }
    }
}
