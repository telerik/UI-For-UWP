using System.Collections.Generic;
using System.Linq;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="DataGridColumnHeaderPanel"/>.
    /// </summary>
    public class DataGridColumnHeaderPanelAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridColumnHeaderPanelAutomationPeer"/> class.
        /// </summary>
        public DataGridColumnHeaderPanelAutomationPeer(DataGridColumnHeaderPanel owner) 
            : base(owner)
        {
        }

        private DataGridColumnHeaderPanel DataGridColumnHeaderPanel
        {
            get
            {
                return this.Owner as DataGridColumnHeaderPanel;
            }
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(DataGridColumnHeaderPanel);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "datagrid columnheader panel";
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
            {
                return nameCore;
            }

            return nameof(DataGridColumnHeaderPanel);
        }

        /// <inheritdoc />
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            if (this.DataGridColumnHeaderPanel != null)
            {
                var automationElements = new List<AutomationPeer>();
                automationElements = this.DataGridColumnHeaderPanel.Children.OfType<UIElement>()
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
