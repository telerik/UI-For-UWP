using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    public class RadarPointSeriesAutomationPeer : ChartSeriesAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadarPointSeriesAutomationPeer class.
        /// </summary>
        public RadarPointSeriesAutomationPeer(RadarPointSeries owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(RadarPointSeries);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(RadarPointSeries);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "radar point series";
        }
    }
}
