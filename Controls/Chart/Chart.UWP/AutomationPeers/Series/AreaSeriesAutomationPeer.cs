using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    public class AreaSeriesAutomationPeer : ChartSeriesAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the AreaSeriesAutomationPeer class.
        /// </summary>
        public AreaSeriesAutomationPeer(AreaSeries owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(AreaSeries);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(AreaSeries);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "area series";
        }
    }
}
