using System.Collections.Generic;
using System.Linq;
using Telerik.UI.Xaml.Controls.Chart;
using Telerik.Core;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Automation.Peers
{
    public class RadChartBaseAutomationPeer : RadControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadChartBaseAutomationPeer class.
        /// </summary>
        public RadChartBaseAutomationPeer(RadChartBase owner)
            : base(owner)
        {
        }

        internal RadChartBase OwningChart
        {
            get
            {
                return this.Owner as RadChartBase;
            }
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(RadChartBase);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(RadChartBase);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad chart base";
        }

        /// <inheritdoc />
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            List<AutomationPeer> list = new List<AutomationPeer>();

            IEnumerable<DependencyObject> childElements = ElementTreeHelper.EnumVisualDescendants(this.Owner, descendand => descendand is ChartElementPresenter);
            foreach (ChartElementPresenter child in childElements)
            {
                AutomationPeer item = FrameworkElementAutomationPeer.FromElement(child);
                if (item == null)
                {
                    item = FrameworkElementAutomationPeer.CreatePeerForElement(child);
                }

                if (item != null)
                {
                    list.Add(item);
                }
            }

            var emptyContentPresenter = ElementTreeHelper.EnumVisualDescendants(this.Owner, descendand => descendand is ContentPresenter)
                .Where(presenter => presenter.Equals(this.OwningChart.emptyContentPresenter)).SingleOrDefault() as ContentPresenter;

            if (emptyContentPresenter != null)
            {
                list.Add(FrameworkElementAutomationPeer.CreatePeerForElement(emptyContentPresenter));
            }

            return list;
        }
    }
}
