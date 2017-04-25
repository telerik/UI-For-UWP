using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Primitives.LoopingList;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the RadLoopingList class.
    /// </summary>
    public class RadLoopingListAutomationPeer : RadControlAutomationPeer, ISelectionProvider, IExpandCollapseProvider
    {
        /// <summary>
        /// Initializes a new instance of the RadLoopingListAutomationPeer class.
        /// </summary>
        /// <param name="owner">The RadLoopingList that is associated with this RadLoopingListAutomationPeer.</param>
        public RadLoopingListAutomationPeer(RadLoopingList owner) 
            : base(owner)
        {
        }

        /// <inheritdoc/>
        public ExpandCollapseState ExpandCollapseState
        {
            get
            {
                return this.LoopingList.IsExpanded ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
            }
        }
        
        /// <summary>
        ///  Gets a value indicating whether the UI Automation provider allows more than one child element to be selected concurrently.
        /// </summary>
        public bool CanSelectMultiple
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the UI Automation provider requires at least one child element to be selected.
        /// </summary>
        public bool IsSelectionRequired
        {
            get
            {
                return true;
            }
        }

        private RadLoopingList LoopingList
        {
            get
            {
                return (RadLoopingList)this.Owner;
            }
        }

        /// <summary>
        /// Retrieves a UI Automation provider for each child element that is selected.
        /// </summary>
        public IRawElementProviderSimple[] GetSelection()
        {
            var selectedIndex = this.LoopingList.SelectedIndex;
            var providerSamples = new List<IRawElementProviderSimple>();

            if (this.LoopingList.ItemsPanel != null)
            {
                var item = this.LoopingList.ItemsPanel.ItemFromVisualIndex(selectedIndex);
                if (item != null)
                {
                    var loopingItemPeer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as LoopingListItemAutomationPeer;
                    if (loopingItemPeer != null)
                    {
                        providerSamples.Add(this.ProviderFromPeer(loopingItemPeer));
                    }
                }
            }

            return providerSamples.ToArray();
        }

        /// <summary>
        /// IExpandCollapseProvider implementation.
        /// </summary>
        public void Collapse()
        {
            this.LoopingList.IsExpanded = false;
        }

        /// <summary>
        /// IExpandCollapseProvider implementation.
        /// </summary>
        public void Expand()
        {
            this.LoopingList.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            if (!this.LoopingList.IsExpanded)
            {
                this.LoopingList.IsExpanded = true;
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

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
            {
                return nameCore;
            }

            return "RadLoopingList";
        }

        /// <summary>
        /// Gets the control type for the RadLoopingList that is associated with this RadLoopingListAutomationPeer.
        /// </summary>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.List;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return "RadLoopingList";
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad looping list";
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Selection || patternInterface == PatternInterface.ExpandCollapse)
            {
                return this;
            }

            return null;
        }

        /// <inheritdoc />
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            var children = new List<AutomationPeer>();
            if (this.LoopingList != null)
            {
                if (this.LoopingList.ItemsPanel != null)
                {
                    var indexes = this.LoopingList.ItemsPanel.VisualIndexChain;
                    foreach (var index in indexes)
                    {
                        var item = this.LoopingList.ItemsPanel.ItemFromVisualIndex(index);
                        var loopingItemPeer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as LoopingListItemAutomationPeer;
                        if (loopingItemPeer != null)
                        {
                            children.Add(loopingItemPeer);
                        }
                    }
                }
            }

            return children;
        }
    }
}
