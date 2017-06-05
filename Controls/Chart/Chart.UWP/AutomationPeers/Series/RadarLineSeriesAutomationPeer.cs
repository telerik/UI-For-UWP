using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadarLineSeries"/>.
    /// </summary>
    public class RadarLineSeriesAutomationPeer : RadarPointSeriesAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadarLineSeriesAutomationPeer class.
        /// </summary>
        public RadarLineSeriesAutomationPeer(RadarLineSeries owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.RadarLineSeries);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.RadarLineSeries);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "radar line series";
        }
    }
}
