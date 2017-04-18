using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    public class PolarAxisPlotBandAnnotationAutomationPeer : ChartAnnotationAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the ChartAnnotationAutomationPeer class.
        /// </summary>
        public PolarAxisPlotBandAnnotationAutomationPeer(PolarAxisPlotBandAnnotation owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(PolarAxisPlotBandAnnotation);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(PolarAxisPlotBandAnnotation);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "polar axis plot band annotation";
        }
    }
}
