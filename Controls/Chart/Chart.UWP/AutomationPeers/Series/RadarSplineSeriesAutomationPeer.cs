using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadarSplineSeries"/>.
    /// </summary>
    public class RadarSplineSeriesAutomationPeer : RadarLineSeriesAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadarSplineSeriesAutomationPeer class.
        /// </summary>
        public RadarSplineSeriesAutomationPeer(RadarSplineSeries owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.RadarSplineSeries);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.RadarSplineSeries);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "radar spline series";
        }
    }
}
