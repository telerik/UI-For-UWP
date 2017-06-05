using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="IndicatorBase"/>.
    /// </summary>
    public class IndicatorBaseAutomationPeer : ChartSeriesAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the IndicatorBaseAutomationPeer class.
        /// </summary>
        public IndicatorBaseAutomationPeer(IndicatorBase owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.IndicatorBase);
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = this.Owner.GetType().Name;
            if (!string.IsNullOrEmpty(nameCore))
            {
                return nameCore;
            }

            return base.GetNameCore();
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Chart.IndicatorBase);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "indicator base";
        }
    }
}
