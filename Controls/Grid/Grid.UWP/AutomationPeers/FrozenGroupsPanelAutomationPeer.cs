using System.Collections.Generic;
using System.Linq;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Automation.Peers
{
    public class FrozenGroupsPanelAutomationPeer : FrameworkElementAutomationPeer
    {
        public FrozenGroupsPanelAutomationPeer(FrozenGroupsPanel owner)
            : base(owner)
        {
        }

        private FrozenGroupsPanel FrozenGroupsPanel
        {
            get
            {
                return this.Owner as FrozenGroupsPanel;
            }
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(FrozenGroupsPanel);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "frozen groups panel";
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
                return nameCore;

            return nameof(FrozenGroupsPanel);
        }


        /// <inheritdoc />
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            if (this.FrozenGroupsPanel != null)
            {
                var automationElements = new List<AutomationPeer>();
                automationElements = this.FrozenGroupsPanel.Children.OfType<UIElement>()
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
