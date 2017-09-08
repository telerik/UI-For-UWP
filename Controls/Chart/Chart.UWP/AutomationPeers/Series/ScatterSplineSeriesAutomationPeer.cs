using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="ScatterSplineSeries"/>.
    /// </summary>
    public class ScatterSplineSeriesAutomationPeer : ScatterLineSeriesAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the ScatterSplineSeriesAutomationPeer class.
        /// </summary>
        public ScatterSplineSeriesAutomationPeer(ScatterSplineSeries owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.ScatterSplineSeries);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.ScatterSplineSeries);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "scatter spline series";
        }
    }
}
