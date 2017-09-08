using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="DataGridGroupHeader"/>.
    /// </summary>
    public class DataGridGroupHeaderAutomationPeer : RadControlAutomationPeer, IExpandCollapseProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridGroupHeaderAutomationPeer"/> class.
        /// </summary>
        public DataGridGroupHeaderAutomationPeer(DataGridGroupHeader owner) 
            : base(owner)
        {
        }
        
        /// <inheritdoc />
        public ExpandCollapseState ExpandCollapseState
        {
            get
            {
                return this.OwnerDataGridGroupHeader.IsExpanded ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
            }
        }

        /// <inheritdoc />
        private DataGridGroupHeader OwnerDataGridGroupHeader
        {
            get
            {
                return (DataGridGroupHeader)this.Owner;
            }
        }

        /// <summary>
        /// IExpandCollapseProvider implementation.
        /// </summary>
        public void Collapse()
        {
            this.OwnerDataGridGroupHeader.Owner.OnGroupHeaderTap(this.OwnerDataGridGroupHeader);
        }

        /// <summary>
        /// IExpandCollapseProvider implementation.
        /// </summary>
        public void Expand()
        {
            this.OwnerDataGridGroupHeader.Owner.OnGroupHeaderTap(this.OwnerDataGridGroupHeader);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseExpandCollapseAutomationEvent(bool oldValue, bool newValue)
        {
            this.RaisePropertyChangedEvent(
                ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
                oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed,
                newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
            {
                return nameCore;
            }

            var groupHeaderContext = this.OwnerDataGridGroupHeader.DataContext as GroupHeaderContext;
            if (groupHeaderContext != null && groupHeaderContext.Group != null)
            {
                var group = groupHeaderContext.Group as Group;
                if (group != null && group.Name != null)
                {
                    return group.Name.ToString();
                }
            }

            return nameof(Telerik.UI.Xaml.Controls.Grid.Primitives.DataGridGroupHeader);
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Header;
        }
        
        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Grid.Primitives.DataGridGroupHeader);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Grid.Primitives.DataGridGroupHeader);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "datagrid group header";
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
    }
}
