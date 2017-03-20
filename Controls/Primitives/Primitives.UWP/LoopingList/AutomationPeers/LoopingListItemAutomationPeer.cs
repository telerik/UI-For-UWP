using System;
using Telerik.UI.Xaml.Controls.Primitives.LoopingList;
using Windows.Foundation;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    public class LoopingListItemAutomationPeer : RadContentControlAutomationPeer, ISelectionItemProvider
    {
        /// <summary>
        /// Initializes a new instance of the LoopingListItemAutomationPeer class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public LoopingListItemAutomationPeer(LoopingListItem owner) 
            : base(owner)
        {
        }

        private LoopingListItem LoopingListItemOwner
        {
            get
            {
                return (LoopingListItem)this.Owner;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether associated LoopingListItem is selected or not.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.LoopingListItemOwner.IsSelected;
            }
        }

        /// <summary>
        /// Gets the control type for the  LoopingListItem that is associated with this LoopingListItemAutomationPeer.
        /// </summary>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.SelectionItem)
            {
                return this;
            }

            return base.GetPatternCore(patternInterface);
        }

        /// <inheritdoc />
        protected override string GetAutomationIdCore()
        {
            var automationIdCore = base.GetAutomationIdCore();
            if (!string.IsNullOrEmpty(automationIdCore))
                return automationIdCore;

            if (this.LoopingListItemOwner != null)
            {
                return this.GetNameCore();
            }

            return string.Empty;
        }
        
        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override Rect GetBoundingRectangleCore()
        {
            var loopingList = this.LoopingListItemOwner.Panel.Owner as RadLoopingList;
            if (loopingList != null && !loopingList.IsExpanded)
            {
                var loopingListAutomationPeer = FrameworkElementAutomationPeer.FromElement(loopingList) as RadLoopingListAutomationPeer;
                if (loopingListAutomationPeer != null)
                {
                    return loopingListAutomationPeer.GetBoundingRectangle();
                }
            }

            return base.GetBoundingRectangleCore();
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "loopinglist item";
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return "LoopingListItem";
        }

        /// <inheritdoc />
        protected override bool HasKeyboardFocusCore()
        {
            return true;
        }

        /// <summary>
        /// Gets the IRawElementProviderSimple for the LoopingListItemAutomationPeer.
        /// </summary>
        public IRawElementProviderSimple SelectionContainer
        {
            get
            {
                var list = this.LoopingListItemOwner.Panel.Owner as RadLoopingList;
                if (list != null)
                {
                    return this.ProviderFromPeer(CreatePeerForElement(list));
                }
                return null;
            }
        }

        /// <summary>
        /// Select the current element.
        /// </summary>
        public void AddToSelection()
        {
            var list = this.LoopingListItemOwner.Panel.Owner as RadLoopingList;
            if (list != null && list.SelectedIndex < 0)
            {
                throw new ArgumentNullException("Argument cannot be null or empty.");
            }

            this.SelectCurrentLoopingListItem();
           
        }

        /// <summary>
        /// Removes the current element from selection and selecs the next item in the list.
        /// </summary>
        public void RemoveFromSelection()
        {
            var loopintList = this.LoopingListItemOwner.Panel.Owner;
            if (loopintList != null && loopintList.SelectedIndex == this.LoopingListItemOwner.LogicalIndex
                && loopintList.CanChangeSelectedIndex(this.LoopingListItemOwner.LogicalIndex))
            {
                loopintList.SelectNext(loopintList.SelectedIndex + 1);
            }
        }

        /// <summary>
        /// Selects the LoopingListItem that is owner of the peer.
        /// </summary>
        public void Select()
        {
            this.SelectCurrentLoopingListItem();
            this.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
        }

        private void SelectCurrentLoopingListItem()
        {
            var loopintList = this.LoopingListItemOwner.Panel.Owner;
            if (loopintList != null && loopintList.SelectedIndex != this.LoopingListItemOwner.LogicalIndex
                && loopintList.CanChangeSelectedIndex(this.LoopingListItemOwner.LogicalIndex))
            {
                loopintList.SelectedIndex = this.LoopingListItemOwner.LogicalIndex;
                this.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, false, true);
            }
        }
    }
}
