using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the RadExpanderControl class.
    /// </summary>
    public class RadExpanderControlAutomationPeer : RadContentControlAutomationPeer, IExpandCollapseProvider
    {
        /// <summary>
        /// Initializes a new instance of the RadExpanderControlAutomationPeer class.
        /// </summary>
        public RadExpanderControlAutomationPeer(RadExpanderControl owner) 
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the state, expanded or collapsed, of the control.
        /// </summary>
        /// <returns>The state, expanded or collapsed, of the control.</returns>
        public ExpandCollapseState ExpandCollapseState
        {
            get
            {
                return this.RadExpanderControl.IsExpanded
                    ? ExpandCollapseState.Expanded
                    : ExpandCollapseState.Collapsed;
            }
        }

        internal RadExpanderControl RadExpanderControl
        {
            get
            {
                return (RadExpanderControl)this.Owner;
            }
        }

        /// <summary>
        /// IExpandCollapseProvider implementation.
        /// </summary>
        public void Collapse()
        {
            if (this.RadExpanderControl.IsExpandable)
            {
                this.RadExpanderControl.IsExpanded = false;
            }
        }

        /// <summary>
        /// IExpandCollapseProvider implementation.
        /// </summary>
        public void Expand()
        {
            if (this.RadExpanderControl.IsExpandable)
            {
                this.RadExpanderControl.IsExpanded = true;
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

        /// <summary>
        /// Gets a control pattern that is associated with this AutomationPeer.
        /// </summary>
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ExpandCollapse)
            {
                return this;
            }
            return base.GetPatternCore(patternInterface);
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Primitives.RadExpanderControl);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Primitives.RadExpanderControl);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad expander control";
        }

        /// <inheritdoc />
        protected override bool IsEnabledCore()
        {
            return this.RadExpanderControl.IsExpandable;
        }

        /// <inheritdoc />
        protected override bool HasKeyboardFocusCore()
        {
            return true;
        }

        /// <inheritdoc />
        protected override string GetAutomationIdCore()
        {
            var automationId = base.GetAutomationIdCore();
            if (!string.IsNullOrEmpty(automationId))
            {
                return automationId;
            }

            return nameof(Telerik.UI.Xaml.Controls.Primitives.RadExpanderControl);
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Group;
        }
    }
}
