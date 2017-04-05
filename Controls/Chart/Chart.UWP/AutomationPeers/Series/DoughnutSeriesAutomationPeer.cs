using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    public class DoughnutSeriesAutomationPeer : PieSeriesAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the DoughnutSeriesAutomationPeer class.
        /// </summary>
        public DoughnutSeriesAutomationPeer(DoughnutSeries owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(DoughnutSeries);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(DoughnutSeries);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "doughnut series";
        }
    }
}
