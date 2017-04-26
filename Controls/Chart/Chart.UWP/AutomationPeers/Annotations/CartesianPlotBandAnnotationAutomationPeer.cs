using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="CartesianPlotBandAnnotation"/>.
    /// </summary>
    public class CartesianPlotBandAnnotationAutomationPeer : ChartAnnotationAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianPlotBandAnnotationAutomationPeer"/> class.
        /// </summary>
        public CartesianPlotBandAnnotationAutomationPeer(CartesianPlotBandAnnotation owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.CartesianPlotBandAnnotation);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.CartesianPlotBandAnnotation);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "cartesian plot band annotation";
        }
    }
}
