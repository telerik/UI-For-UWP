using System;
using Telerik.UI.Xaml.Controls.Data;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadDataBoundListBoxItem"/>.
    /// </summary>
    public class RadDataBoundListBoxItemAutomationPeer : RadContentControlAutomationPeer, ISelectionItemProvider
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="RadDataBoundListBoxItemAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="RadDataBoundListBoxItem"/> that is associated with this <see cref="RadDataBoundListBoxItemAutomationPeer"/>.</param>
        public RadDataBoundListBoxItemAutomationPeer(RadDataBoundListBoxItem owner) : base(owner)
        {
        }

        /// <summary>
        /// Gets a value indicating whether whether an item is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                if (this.ListBoxItemOwner.typedOwner.IsCheckModeEnabled)
                {
                    return this.ListBoxItemOwner.IsChecked;
                }
                else
                {
                    return this.ListBoxItemOwner.IsSelected;
                }
            }
        }

        /// <summary>
        /// Gets the provider that implements ISelectionProvider and acts as the container for the calling object.
        /// </summary>
        public IRawElementProviderSimple SelectionContainer
        {
            get
            {
                RadDataBoundListBox parentListBox = this.ListBoxItemOwner.typedOwner;
                if (parentListBox != null)
                {
                    return this.ProviderFromPeer(FrameworkElementAutomationPeer.CreatePeerForElement(parentListBox));
                }
                return null;
            }
        }

        private RadDataBoundListBoxItem ListBoxItemOwner
        {
            get
            {
                return (RadDataBoundListBoxItem)this.Owner;
            }
        }

        /// <summary>
        /// ISelectionItemProvider implementation.
        /// </summary>
        public void AddToSelection()
        {
            if (!this.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            var selector = this.ListBoxItemOwner.typedOwner;
            if ((selector == null) || (!selector.IsCheckModeEnabled && selector.SelectedItem != null && selector.SelectedItem != this.ListBoxItemOwner.DataContext))
            {
                // Parent must exist and be multi-select
                // in single-select mode the selected item should be null or Owner
                throw new InvalidOperationException("Parent non existing or not in Multiselect mode. In single-select mode the selected item should be null or Owner");
            }

            this.Select();
        }

        /// <summary>
        /// ISelectionItemProvider implementation.
        /// </summary>
        public void RemoveFromSelection()
        {
            if (!this.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            RadDataBoundListBox listbox = this.ListBoxItemOwner.typedOwner;
            if (listbox != null)
            {
                this.SetSelection(listbox, true, false);
            }
        }

        /// <summary>
        /// Selects the current element.
        /// </summary>
        public void Select()
        {
            RadDataBoundListBox listbox = this.ListBoxItemOwner.typedOwner;
            if (this.IsSelected || listbox == null)
            {
                return;
            }

            this.SetSelection(listbox, false, true);
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
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ListItem;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad databound list box item";
        }

        private void SetSelection(RadDataBoundListBox listBox, bool oldValue, bool newValue)
        {
            if (listBox.IsCheckModeEnabled)
            {
                listBox.HandleItemCheckStateChange(this.ListBoxItemOwner);
                this.ListBoxItemOwner.UpdateCheckedState();
            }
            else
            {
                listBox.SelectedItem = newValue ? this.ListBoxItemOwner.DataContext : null;
                this.ListBoxItemOwner.IsSelected = newValue;
            }

            this.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, oldValue, newValue);
        }
    }
}
