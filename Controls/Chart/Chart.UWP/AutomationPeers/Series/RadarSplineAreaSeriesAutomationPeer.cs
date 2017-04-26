using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadarSplineAreaSeries"/>.
    /// </summary>
    public class RadarSplineAreaSeriesAutomationPeer : RadarAreaSeriesAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadarSplineAreaSeriesAutomationPeer class.
        /// </summary>
        public RadarSplineAreaSeriesAutomationPeer(RadarSplineAreaSeries owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.RadarSplineAreaSeries);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.RadarSplineAreaSeries);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "radar spline area series";
        }
    }
}
