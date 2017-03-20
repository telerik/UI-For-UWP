using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Input;
using Windows.Foundation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for AutoCompleteTextBox.
    /// </summary>
    public class AutoCompleteTextBoxAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the AutoCompleteTextBoxAutomationPeer class.
        /// </summary>
        /// <param name="tBox">The TextBox that is associated with this AutoCompleteTextBoxAutomationPeer.</param>
        /// <param name="autoComplete">The RadAutoCompleteBox that is associated with this AutoCompleteTextBoxAutomationPeer.</param>        
        public AutoCompleteTextBoxAutomationPeer(TextBox tBox, RadAutoCompleteBox autoComplete) : base(tBox)
        {
            this.ParentAutoComplete = autoComplete;
        }

        private RadAutoCompleteBox ParentAutoComplete
        {
            get;
            set;
        }

        private RadAutoCompleteBoxAutomationPeer parentPeer;
        private RadAutoCompleteBoxAutomationPeer AutoCompletePeer
        {
            get
            {
                if (this.parentPeer == null)
                {
                    this.parentPeer = FrameworkElementAutomationPeer.FromElement(this.ParentAutoComplete) as RadAutoCompleteBoxAutomationPeer;
                }
                return this.parentPeer;
            }
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Edit;
        }

        /// <inheritdoc />
        protected override Rect GetBoundingRectangleCore()
        {
            return this.AutoCompletePeer.GetBoundingRectangle();
        }
    }
}
