using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    public class CartesianPlotBandAnnotationAutomationPeer : ChartAnnotationAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the ChartAnnotationAutomationPeer class.
        /// </summary>
        public CartesianPlotBandAnnotationAutomationPeer(CartesianPlotBandAnnotation owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(CartesianPlotBandAnnotation);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(CartesianPlotBandAnnotation);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "cartesian plot band annotation";
        }
    }
}
