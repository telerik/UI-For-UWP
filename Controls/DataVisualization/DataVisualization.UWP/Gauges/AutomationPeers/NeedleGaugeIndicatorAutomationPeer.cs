using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="NeedleGaugeIndicator"/>.
    /// </summary>
    public class NeedleGaugeIndicatorAutomationPeer : RadialGaugeIndicatorAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the NeedleGaugeIndicatorAutomationPeer class.
        /// </summary>
        public NeedleGaugeIndicatorAutomationPeer(NeedleGaugeIndicator owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.NeedleGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.NeedleGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "needle gauge indicator";
        }
    }
}
