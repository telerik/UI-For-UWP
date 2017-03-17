using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    public class LinearBarGaugeIndicatorAutomationPeer : BarGaugeIndicatorAutomationPeer
    {
        public LinearBarGaugeIndicatorAutomationPeer(LinearBarGaugeIndicator owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(LinearBarGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(LinearBarGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "linear bar gauge indicator";
        }
    }
}
