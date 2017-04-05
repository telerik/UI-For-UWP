using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    public class ScatterLineSeriesAutomationPeer : ScatterPointSeriesAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the ScatterLineSeriesAutomationPeer class.
        /// </summary>
        public ScatterLineSeriesAutomationPeer(ScatterLineSeries owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(ScatterLineSeries);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(ScatterLineSeries);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "scatter line series";
        }
    }
}
