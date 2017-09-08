using Telerik.UI.Xaml.Controls.Input.AutoCompleteBox;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the SuggestionItem class.
    /// </summary>
    public class SuggestionItemAutomationPeer : ListBoxItemAutomationPeer, IInvokeProvider
    {
        /// <summary>
        /// Initializes a new instance of the SuggestionItemAutomationPeer class.
        /// </summary>
        /// <param name="owner">The SuggestionItem that is associated with this SuggestionItemAutomationPeer.</param>
        public SuggestionItemAutomationPeer(SuggestionItem owner) 
            : base(owner)
        {
        }
        
        private SuggestionItem SuggestionItem
        {
            get
            {
                return (SuggestionItem)this.Owner;
            }
        }

        /// <summary>
        /// IInvokeProvider implementation.
        /// </summary>
        public void Invoke()
        {
            this.SuggestionItem.OnItemTap();
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
    }
}
