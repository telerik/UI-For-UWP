using Telerik.UI.Xaml.Controls.DataVisualization;

namespace Telerik.UI.Automation.Peers
{
    public class BarIndicatorSegmentAutomationPeer : RadControlAutomationPeer
    {
        public BarIndicatorSegmentAutomationPeer(BarIndicatorSegment owner) 
            : base(owner)
        {
        }


        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(BarIndicatorSegment);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(BarIndicatorSegment);
        }

        /// <inheritdoc />
        protected override string GetAutomationIdCore()
        {
            var automationId = base.GetAutomationIdCore();
            if (!string.IsNullOrEmpty(automationId))
            {
                return automationId;
            }

            return nameof(BarIndicatorSegment);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "bar indicator segment";
        }
    }
}
