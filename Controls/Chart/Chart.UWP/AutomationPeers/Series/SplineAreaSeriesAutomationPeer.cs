using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    public class SplineAreaSeriesAutomationPeer : AreaSeriesAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the SplineAreaSeriesAutomationPeer class.
        /// </summary>
        public SplineAreaSeriesAutomationPeer(SplineAreaSeries owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(SplineAreaSeries);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(SplineAreaSeries);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "spline area series";
        }
    }
}
