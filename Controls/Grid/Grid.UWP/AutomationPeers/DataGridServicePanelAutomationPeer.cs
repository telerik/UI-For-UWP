using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    public class DataGridServicePanelAutomationPeer : RadControlAutomationPeer, IExpandCollapseProvider
    {
        public DataGridServicePanelAutomationPeer(DataGridServicePanel owner)
            : base(owner)
        {
        }

        private DataGridServicePanel DataGridServicePanel
        {
            get
            {
                return this.Owner as DataGridServicePanel;
            }
        }

        /// <summary>
        /// IExpandCollapseProvider implementation.
        /// </summary>
        public ExpandCollapseState ExpandCollapseState
        {
            get
            {
                return this.DataGridServicePanel.GroupFlyout.IsOpen ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
            }
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(DataGridServicePanel);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "datagrid service panel";
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Group;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
                return nameCore;

            return nameof(DataGridServicePanel);
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ExpandCollapse)
            {
                return this;
            }
            return base.GetPatternCore(patternInterface);
        }

        /// <summary>
        /// IExpandCollapseProvider implementation.
        /// </summary>
        public void Collapse()
        {
            if (this.DataGridServicePanel.GroupFlyout != null && this.DataGridServicePanel.GroupFlyout.IsOpen)
            {
                this.DataGridServicePanel.GroupFlyout.IsOpen = false;
            }
        }

        /// <summary>
        /// IExpandCollapseProvider implementation.
        /// </summary>
        public void Expand()
        {
            if (this.DataGridServicePanel.Owner != null && this.DataGridServicePanel.Owner.GroupDescriptors.Count != 0)
            {
                this.DataGridServicePanel.OpenGroupingFlyout();
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseExpandCollapseAutomationEvent(bool oldValue, bool newValue)
        {
            this.RaisePropertyChangedEvent(
                ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
                oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed,
                newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
        }
    }
}
