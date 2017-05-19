using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="BarIndicatorSegment"/>.
    /// </summary>
    public class BarIndicatorSegmentAutomationPeer : RadControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the BarIndicatorSegmentAutomationPeer class.
        /// </summary>
        public BarIndicatorSegmentAutomationPeer(BarIndicatorSegment owner) 
            : base(owner)
        {
        }
        
        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.BarIndicatorSegment);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.BarIndicatorSegment);
        }

        /// <inheritdoc />
        protected override string GetAutomationIdCore()
        {
            var automationId = base.GetAutomationIdCore();
            if (!string.IsNullOrEmpty(automationId))
            {
                return automationId;
            }

            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.BarIndicatorSegment);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "bar indicator segment";
        }
    }
}
