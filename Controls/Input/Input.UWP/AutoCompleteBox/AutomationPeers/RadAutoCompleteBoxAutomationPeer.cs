using System;
using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation peer class for RadAutoCompleteBox.
    /// </summary>
    public class RadAutoCompleteBoxAutomationPeer : RadHeaderedControlAutomationPeer, IValueProvider, ISelectionProvider, IExpandCollapseProvider
    {
        /// <summary>
        /// Initializes a new instance of the RadAutoCompleteBoxAutomationPeer class.
        /// </summary>
        /// <param name="owner">The RadAutoCompleteBox that is associated with this RadAutoCompleteBoxAutomationPeer.</param>
        public RadAutoCompleteBoxAutomationPeer(RadAutoCompleteBox owner)
            : base(owner)
        {
        }

        private RadAutoCompleteBox AutoCompleteBox
        {
            get
            {
                return (RadAutoCompleteBox)this.Owner;
            }
        }

        /// <summary>
        /// Gets a value that specifies whether the value of a control is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the value is read-only; false if it can be modified.
        /// </returns>
        public bool IsReadOnly
        {
            get
            {
                return this.AutoCompleteBox.textbox.IsReadOnly;
            }
        }

        /// <summary>
        /// Gets the Text of the AutoCompleteTextBox.
        /// </summary>
        public string Value
        {
            get
            {
                if (this.AutoCompleteBox != null)
                {
                    return this.AutoCompleteBox.Text;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns whether multiple selection is possible.
        /// </summary>
        public bool CanSelectMultiple
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns whether selection is required.
        /// </summary>
        public bool IsSelectionRequired
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Closes the DropDown list.
        /// </summary>
        public void Collapse()
        {
            this.AutoCompleteBox.IsDropDownOpen = false;
        }

        /// <summary>
        /// Opens the DropDown list.
        /// </summary>
        public void Expand()
        {
            this.AutoCompleteBox.IsDropDownOpen = true;
        }

        /// <summary>
        /// Returns whether the control is in Expanded or Collapsed state.
        /// </summary>
        public ExpandCollapseState ExpandCollapseState
        {
            get
            {
                return this.AutoCompleteBox.IsDropDownOpen ?
                    ExpandCollapseState.Expanded :
                    ExpandCollapseState.Collapsed;
            }
        }

        /// <summary>
        /// Sets the Text of the AutoCompleteBox.
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(string value)
        {
            if (this.AutoCompleteBox != null)
            {
                this.AutoCompleteBox.Text = value;
            }
        }

        /// <summary>
        ///  Retrieves a UI Automation provider for each child element that is selected.
        /// </summary>
        /// <returns></returns>
        public IRawElementProviderSimple[] GetSelection()
        {
            if (this.AutoCompleteBox.suggestionsControl != null)
            {
                object selectedItem = this.AutoCompleteBox.suggestionsControl.SelectedItem;
                if (selectedItem != null)
                {
                    UIElement uIElement = this.AutoCompleteBox.suggestionsControl.ContainerFromItem(selectedItem) as UIElement;
                    if (uIElement != null)
                    {
                        AutomationPeer automationPeer = FrameworkElementAutomationPeer.CreatePeerForElement(uIElement);
                        if (automationPeer != null)
                        {
                            return new IRawElementProviderSimple[] { base.ProviderFromPeer(automationPeer) };
                        }
                    }
                }
            }
            return new IRawElementProviderSimple[] { };
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad autocomplete box";
        }

        /// <summary>
        /// Gets the control type for the RadAutoCompleteBox that is associated with this RadAutoCompleteBoxAutomationPeer.
        /// </summary>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ComboBox;
        }

        /// <inheritdoc />
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            var automationPeers = new List<AutomationPeer>();

            if (this.AutoCompleteBox != null && this.AutoCompleteBox.IsTemplateApplied)
            {
                AutomationPeer tBoxPeer = CreatePeerForElement(this.AutoCompleteBox.textbox);
                automationPeers.Insert(0, tBoxPeer);

                AutomationPeer suggestionControlPeer = CreatePeerForElement(this.AutoCompleteBox.suggestionsControl);
                if (suggestionControlPeer != null)
                {
                    IList<AutomationPeer> listChildren = suggestionControlPeer.GetChildren();
                    if (listChildren != null)
                    {
                        foreach (AutomationPeer child in listChildren)
                        {
                            automationPeers.Add(child);
                        }
                    }
                }
            }

            return automationPeers;
        }

        /// <inheritdoc />
        protected override Rect GetBoundingRectangleCore()
        {
            Point topLeft = this.Owner.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0));
            Rect result = new Rect(topLeft, this.Owner.RenderSize);
            return result;
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            object pattern = null;
            if (patternInterface == PatternInterface.Value)
            {
                pattern = this;
            }
            else if (patternInterface == PatternInterface.ExpandCollapse)
            {
                pattern = this;
            }
            else if (patternInterface == PatternInterface.Selection)
            {
                AutomationPeer selectionPeer = FrameworkElementAutomationPeer.CreatePeerForElement(this.AutoCompleteBox.suggestionsControl) as AutomationPeer;
                if (selectionPeer != null)
                {
                    pattern = selectionPeer.GetPattern(patternInterface);
                }
            }

            if (pattern == null)
            {
                pattern = base.GetPatternCore(patternInterface);
            }

            return pattern;
        }        
    }
}
