using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="CartesianChartGrid"/>.
    /// </summary>
    public class CartesianChartGridAutomationPeer : RadControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianChartGridAutomationPeer"/> class.
        /// </summary>
        public CartesianChartGridAutomationPeer(CartesianChartGrid owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.CartesianChartGrid);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.CartesianChartGrid);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "cartesian chart grid";
        }
    }
}
