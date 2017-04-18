using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    public class ScatterSplineAreaSeriesAutomationPeer : ScatterAreaSeriesAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the ScatterSplineAreaSeriesAutomationPeer class.
        /// </summary>
        public ScatterSplineAreaSeriesAutomationPeer(ScatterSplineAreaSeries owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(ScatterSplineAreaSeries);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(ScatterSplineAreaSeries);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "scatter spline area series";
        }
    }
}
