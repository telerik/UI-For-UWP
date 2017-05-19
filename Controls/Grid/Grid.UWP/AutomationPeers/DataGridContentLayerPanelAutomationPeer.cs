using System.Collections.Generic;
using System.Linq;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Automation.Peers
{
    internal class DataGridContentLayerPanelAutomationPeer : FrameworkElementAutomationPeer
    {
        public DataGridContentLayerPanelAutomationPeer(DataGridContentLayerPanel owner)
            : base(owner)
        {
        }

        private DataGridContentLayerPanel DataGridContentLayerPanel
        {
            get
            {
                return this.Owner as DataGridContentLayerPanel;
            }
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(DataGridContentLayerPanel);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "datagrid content layer panel";
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Table;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
            {
                return nameCore;
            }

            return nameof(DataGridContentLayerPanel);
        }

        /// <inheritdoc />
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            if (this.DataGridContentLayerPanel != null)
            {
                var automationElements = new List<AutomationPeer>();

                automationElements = this.DataGridContentLayerPanel.Children.OfType<UIElement>()
                .Where(e => e.Visibility == Visibility.Visible)
                .Select(e => FrameworkElementAutomationPeer.CreatePeerForElement(e))
                .Where(ap => ap != null)
                .ToList();

                return automationElements;
            }

            return new List<AutomationPeer>();
        }
    }
}
