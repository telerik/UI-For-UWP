using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    public class SegmentedGaugeIndicatorAutomationPeer : GaugeIndicatorAutomationPeer
    {
        public SegmentedGaugeIndicatorAutomationPeer(SegmentedGaugeIndicator owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(SegmentedGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(SegmentedGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "segmented gauge indicator";
        }
    }
}
