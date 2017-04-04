using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    public class SplineSeriesAutomationPeer : LineSeriesAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the SplineSeriesAutomationPeer class.
        /// </summary>
        public SplineSeriesAutomationPeer(SplineSeries owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(SplineSeries);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(SplineSeries);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "spline series";
        }
    }
}
