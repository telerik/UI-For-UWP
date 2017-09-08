using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="ArrowGaugeIndicator"/>.
    /// </summary>
    public class ArrowGaugeIndicatorAutomationPeer : NeedleGaugeIndicatorAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the ArrowGaugeIndicatorAutomationPeer class.
        /// </summary>
        public ArrowGaugeIndicatorAutomationPeer(ArrowGaugeIndicator owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.ArrowGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.ArrowGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "arrow gauge indicator";
        }
    }
}
