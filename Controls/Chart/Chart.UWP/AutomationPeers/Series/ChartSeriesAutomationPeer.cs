using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    public class ChartSeriesAutomationPeer : RadControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the ChartSeriesAutomationPeer class.
        /// </summary>
        public ChartSeriesAutomationPeer(ChartSeries owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
                return nameCore;

            return nameof(ChartSeries);
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(ChartSeries);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(ChartSeries);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "chart series";
        }
    }
}
