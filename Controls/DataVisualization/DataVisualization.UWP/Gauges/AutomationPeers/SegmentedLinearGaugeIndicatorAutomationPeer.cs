using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    public class SegmentedLinearGaugeIndicatorAutomationPeer : SegmentedGaugeIndicatorAutomationPeer
    {
        public SegmentedLinearGaugeIndicatorAutomationPeer(SegmentedLinearGaugeIndicator owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(SegmentedLinearGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(SegmentedLinearGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "segmented linear gauge indicator";
        }
    }
}
