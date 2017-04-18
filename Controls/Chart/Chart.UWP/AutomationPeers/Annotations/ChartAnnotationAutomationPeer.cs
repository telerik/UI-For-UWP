using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    public class ChartAnnotationAutomationPeer : RadControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the ChartAnnotationAutomationPeer class.
        /// </summary>
        public ChartAnnotationAutomationPeer(ChartAnnotation owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(ChartAnnotation);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(ChartAnnotation);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "chart annotation";
        }
    }
}
