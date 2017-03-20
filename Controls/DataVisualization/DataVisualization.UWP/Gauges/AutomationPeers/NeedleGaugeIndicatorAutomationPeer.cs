using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    public class NeedleGaugeIndicatorAutomationPeer : RadialGaugeIndicatorAutomationPeer
    {
        public NeedleGaugeIndicatorAutomationPeer(NeedleGaugeIndicator owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(NeedleGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(NeedleGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "needle gauge indicator";
        }
    }
}
