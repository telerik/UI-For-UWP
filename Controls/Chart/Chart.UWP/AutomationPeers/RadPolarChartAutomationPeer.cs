using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    public class RadPolarChartAutomationPeer : RadChartBaseAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadPolarChartAutomationPeer class.
        /// </summary>
        public RadPolarChartAutomationPeer(RadPolarChart owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(RadPolarChart);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(RadPolarChart);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad polar chart";
        }
    }
}
