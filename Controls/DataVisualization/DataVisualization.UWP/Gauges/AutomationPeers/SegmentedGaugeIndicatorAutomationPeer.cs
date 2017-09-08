using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="SegmentedGaugeIndicator"/>.
    /// </summary>
    public class SegmentedGaugeIndicatorAutomationPeer : GaugeIndicatorAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the SegmentedGaugeIndicatorAutomationPeer class.
        /// </summary>
        public SegmentedGaugeIndicatorAutomationPeer(SegmentedGaugeIndicator owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.SegmentedGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.SegmentedGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "segmented gauge indicator";
        }
    }
}
