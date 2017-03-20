using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    public class SegmentedRadialGaugeIndicatorAutomationPeer : SegmentedGaugeIndicatorAutomationPeer
    {
        public SegmentedRadialGaugeIndicatorAutomationPeer(SegmentedRadialGaugeIndicator owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(SegmentedRadialGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(SegmentedRadialGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "segmented radial gauge indicator";
        }
    }
}
