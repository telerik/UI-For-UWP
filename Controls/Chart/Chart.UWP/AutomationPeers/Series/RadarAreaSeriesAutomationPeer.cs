using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    public class RadarAreaSeriesAutomationPeer : RadarLineSeriesAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadarAreaSeriesAutomationPeer class.
        /// </summary>
        public RadarAreaSeriesAutomationPeer(RadarAreaSeries owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(RadarAreaSeries);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(RadarAreaSeries);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "radar series";
        }
    }
}
