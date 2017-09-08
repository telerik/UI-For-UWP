using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="PolarAxisPlotBandAnnotation"/>.
    /// </summary>
    public class PolarAxisPlotBandAnnotationAutomationPeer : ChartAnnotationAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PolarAxisPlotBandAnnotationAutomationPeer"/> class.
        /// </summary>
        public PolarAxisPlotBandAnnotationAutomationPeer(PolarAxisPlotBandAnnotation owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.PolarAxisPlotBandAnnotation);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.PolarAxisPlotBandAnnotation);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "polar axis plot band annotation";
        }
    }
}
