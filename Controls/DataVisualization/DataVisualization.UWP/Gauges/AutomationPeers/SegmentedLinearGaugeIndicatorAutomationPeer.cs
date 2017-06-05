using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="SegmentedLinearGaugeIndicator"/>.
    /// </summary>
    public class SegmentedLinearGaugeIndicatorAutomationPeer : SegmentedGaugeIndicatorAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the SegmentedLinearGaugeIndicatorAutomationPeer class.
        /// </summary>
        public SegmentedLinearGaugeIndicatorAutomationPeer(SegmentedLinearGaugeIndicator owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.SegmentedLinearGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.SegmentedLinearGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "segmented linear gauge indicator";
        }
    }
}
