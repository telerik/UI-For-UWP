using Telerik.Core;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadSideDrawer"/>.
    /// </summary>
    public class RadSideDrawerAutomationPeer : RadControlAutomationPeer, IExpandCollapseProvider
    {
        private RadSideDrawer SideDrawerOwner
        {
            get
            {
                return (RadSideDrawer)this.Owner;
            }
        }

        /// <summary>
        /// Initializes a new instance of the RadSideDrawerAutomationPeer class.
        /// </summary>
        /// <param name="owner">The RadSideDrawer that is associated with this RadSideDrawerAutomationPeer.</param>
        public RadSideDrawerAutomationPeer(RadSideDrawer owner) 
            : base(owner)
        {

        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            if (this.SideDrawerOwner.MainContent != null)
            {
                var textBlock = ElementTreeHelper.FindVisualDescendant<Windows.UI.Xaml.Controls.TextBlock>(this.SideDrawerOwner);
                if (textBlock != null)
                {
                    return textBlock.Text;
                }
            }

            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
                return nameCore;

            return string.Empty;
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

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad side drawer";
        }

        /// <summary>
        /// IExpandCollapseProvider implementation.
        /// </summary>
        public void Collapse()
        {
            this.SideDrawerOwner.ToggleDrawer();
        }

        /// <summary>
        /// IExpandCollapseProvider implementation.
        /// </summary>
        public void Expand()
        {
            this.SideDrawerOwner.ToggleDrawer();
        }

        /// <summary>
        /// IExpandCollapseProvider implementation.
        /// </summary>
        public ExpandCollapseState ExpandCollapseState
        {
            get
            {
                return ConvertDrawerToExpandCollapseState(this.SideDrawerOwner.DrawerState);
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseExpandCollapseAutomationEvent(DrawerState oldValue, DrawerState newValue)
        {
            var oldConvertedValue = this.ConvertDrawerToExpandCollapseState(oldValue);
            var newConvertedValue = this.ConvertDrawerToExpandCollapseState(newValue);

            this.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldConvertedValue.ToString(), newConvertedValue.ToString());
            this.RaisePropertyChangedEvent(AutomationElementIdentifiers.ItemStatusProperty, oldConvertedValue.ToString(), newConvertedValue.ToString());
        }

        private ExpandCollapseState ConvertDrawerToExpandCollapseState(DrawerState drawerState)
        {
            if (drawerState == DrawerState.Opened)
            {
                return ExpandCollapseState.Expanded;
            }
            else if (drawerState == DrawerState.Closed)
            {
                return ExpandCollapseState.Collapsed;
            }

            return ExpandCollapseState.PartiallyExpanded;
        }
    }
}
