using System;
using System.Collections.Generic;
using System.Text;
using Telerik.UI.Xaml.Controls.Input;
using Windows.Foundation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Automation.Peers
{
    public class NumericTextBoxAutomationPeer : TextBoxAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the NumericTextBoxAutomationPeer class.
        /// </summary>
        /// <param name="tBox">The TextBox that is associated with this NumericTextBoxAutomationPeer.</param>
        /// <param name="numeric">The RadNumeriBox that is associated with this NumericTextBoxAutomationPeer.</param>        
        public NumericTextBoxAutomationPeer(TextBox tBox, RadNumericBox numeric) : base(tBox)
        {
            this.ParentNumericBox = numeric;
        }

        private RadNumericBox ParentNumericBox
        {
            get;
            set;
        }

        private RadNumericBoxAutomationPeer parentPeer;

        private RadNumericBoxAutomationPeer NumericBoxPeer
        {
            get
            {
                if (this.parentPeer == null)
                {
                    this.parentPeer = (RadNumericBoxAutomationPeer)CreatePeerForElement(this.ParentNumericBox);
                }
                return this.parentPeer;
            }
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return this.NumericBoxPeer.GetAutomationControlType();
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            return this.NumericBoxPeer.GetName();
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            return this.NumericBoxPeer.GetPattern(patternInterface);
        }
    }
}
