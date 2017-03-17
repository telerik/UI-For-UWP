using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    public class ArrowGaugeIndicatorAutomationPeer : NeedleGaugeIndicatorAutomationPeer
    {
        public ArrowGaugeIndicatorAutomationPeer(ArrowGaugeIndicator owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(ArrowGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(ArrowGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "arrow gauge indicator";
        }
    }
}
