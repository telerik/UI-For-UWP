using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="LinearBarGaugeIndicator"/>.
    /// </summary>
    public class LinearBarGaugeIndicatorAutomationPeer : BarGaugeIndicatorAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the LinearBarGaugeIndicatorAutomationPeer class.
        /// </summary>
        public LinearBarGaugeIndicatorAutomationPeer(LinearBarGaugeIndicator owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.LinearBarGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.LinearBarGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "linear bar gauge indicator";
        }
    }
}
