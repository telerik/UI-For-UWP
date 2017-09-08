using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="ScatterAreaSeries"/>.
    /// </summary>
    public class ScatterAreaSeriesAutomationPeer : ScatterLineSeriesAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the ScatterAreaSeriesAutomationPeer class.
        /// </summary>
        public ScatterAreaSeriesAutomationPeer(ScatterAreaSeries owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.ScatterAreaSeries);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.ScatterAreaSeries);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "scatter area series";
        }
    }
}
