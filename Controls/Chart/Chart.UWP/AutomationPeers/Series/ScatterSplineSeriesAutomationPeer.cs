using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
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
            return nameof(ScatterSplineSeries);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(ScatterSplineSeries);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "scatter spline series";
        }
    }
}
