using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="SegmentedRadialGaugeIndicator"/>.
    /// </summary>
    public class SegmentedRadialGaugeIndicatorAutomationPeer : SegmentedGaugeIndicatorAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the SegmentedRadialGaugeIndicatorAutomationPeer class.
        /// </summary>
        public SegmentedRadialGaugeIndicatorAutomationPeer(SegmentedRadialGaugeIndicator owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.SegmentedRadialGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.SegmentedRadialGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "segmented radial gauge indicator";
        }
    }
}
