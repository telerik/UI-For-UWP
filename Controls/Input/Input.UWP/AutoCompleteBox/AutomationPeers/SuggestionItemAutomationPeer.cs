using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Input.AutoCompleteBox;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    public class SuggestionItemAutomationPeer : ListBoxItemAutomationPeer, IInvokeProvider
    {
        private SuggestionItemsControl suggestionItemsControl;
        private RadAutoCompleteBox parentAutoCompleteBox
        {
            get
            {
                if (this.suggestionItemsControl != null)
                {
                    return this.suggestionItemsControl.owner;
                }

                return null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the SuggestionItemAutomationPeer class.
        /// </summary>
        /// <param name="owner">The SuggestionItem that is associated with this SuggestionItemAutomationPeer.</param>
        public SuggestionItemAutomationPeer(SuggestionItem owner) 
            : base(owner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SuggestionItemAutomationPeer class.
        /// </summary>
        /// <param name="owner">The SuggestionItem that is associated with this SuggestionItemAutomationPeer.</param>
        /// <param name="parent">The SuggestionItemsControl that is parent of SuggestionItem.</param>
        public SuggestionItemAutomationPeer(SuggestionItem owner, SuggestionItemsControl parent) 
            : this(owner)
        {
            this.suggestionItemsControl = parent;
        }

        private SuggestionItem SuggestionItem
        {
            get
            {
                return (SuggestionItem)this.Owner;
            }
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Invoke)
            {
                return this;
            }

            return base.GetPatternCore(patternInterface);
        }

        /// <inheritdoc />
        protected override bool HasKeyboardFocusCore()
        {
            return true;
        }

        /// <summary>
        /// IInvokeProvider implementation.
        /// </summary>
        public void Invoke()
        {
            if (this.parentAutoCompleteBox != null && this.parentAutoCompleteBox.IsDropDownOpen)
            {
                this.parentAutoCompleteBox.UpdateTextFromPopupInteraction(this.SuggestionItem.DataItem);
                this.parentAutoCompleteBox.IsDropDownOpen = false;

                var autoCompleteTextBoxPeer = FrameworkElementAutomationPeer.FromElement(this.parentAutoCompleteBox.textbox) as TextBoxAutomationPeer;
                if (autoCompleteTextBoxPeer != null)
                {
                    autoCompleteTextBoxPeer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
                }
            }
        }
    }
}
