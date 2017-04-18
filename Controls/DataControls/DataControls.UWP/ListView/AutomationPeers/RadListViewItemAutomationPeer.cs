using System;
using Telerik.UI.Xaml.Controls.Data;
using Telerik.UI.Xaml.Controls.Data.ListView;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{

    /// <summary>
    /// AutomationPeer class for <see cref="RadListViewItem"/>.
    /// </summary>
    public class RadListViewItemAutomationPeer : RadContentControlAutomationPeer, ISelectionItemProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadListViewItemAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="RadListViewItem"/> that is associated with this <see cref="RadListViewItemAutomationPeer"/>.</param>
        public RadListViewItemAutomationPeer(RadListViewItem owner)
            : base(owner)
        {

        }

        private RadListViewItem ListViewItemOwner
        {
            get
            {
                return (RadListViewItem)this.Owner;
            }
        }

        /// <summary>
        /// Indicates whether an item is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                if (this.ListViewItemOwner.ListView != null)
                {
                    return this.ListViewItemOwner.ListView.SelectedItems.Contains(this.ListViewItemOwner.Content);
                }
                return this.IsSelected;
            }
        }

        /// <summary>
        /// Specifies the provider that implements ISelectionProvider and acts as the container for the calling object.
        /// </summary>
        public IRawElementProviderSimple SelectionContainer
        {
            get
            {
                RadListView parentListView = this.ListViewItemOwner.ListView;
                if (parentListView != null)
                {
                    return this.ProviderFromPeer(CreatePeerForElement(parentListView));
                }
                return null;
            }
        }

        public void AddToSelection()
        {
            if (!IsEnabled())
                throw new ElementNotEnabledException();

            var selector = this.ListViewItemOwner.ListView;
            if ((selector == null) || (!this.CanParentSelectMultiple(selector) && selector.SelectedItem != null && selector.SelectedItem != this.ListViewItemOwner.Content))
            {
                // Parent must exist and be multi-select
                // in single-select mode the selected item should be null or Owner
                throw new InvalidOperationException("Parent non existing or not in Multiselect mode. In single-select mode the selected item should be null or Owner");
            }

            this.Select();
        }

        public void RemoveFromSelection()
        {
            if (!IsEnabled())
                throw new ElementNotEnabledException();

            this.UnSelect();
        }

        /// <summary>
        /// Selects the current element.
        /// </summary>
        public void Select()
        {
            if (!this.IsSelected)
            {
                if (this.ListViewItemOwner.ListView != null)
                {
                    this.ListViewItemOwner.ListView.SelectItem(this.ListViewItemOwner.Content);
                }
                this.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, false, true);
            }
        }

        /// <inheritdoc />	
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ListItem;
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
        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad listview item";
        }

        /// <summary>
        /// Unselects the current element.
        /// </summary>
        private void UnSelect()
        {
            if (this.IsSelected)
            {
                if (this.ListViewItemOwner.ListView != null)
                {
                    this.ListViewItemOwner.ListView.DeselectItem(this.ListViewItemOwner.Content);
                }
                this.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, true, false);
            }
        }

        private bool CanParentSelectMultiple(RadListView parent)
        {
            return parent.SelectionMode == DataControlsSelectionMode.Multiple ||
                   parent.SelectionMode == DataControlsSelectionMode.MultipleWithCheckBoxes;
        }
    }
}
