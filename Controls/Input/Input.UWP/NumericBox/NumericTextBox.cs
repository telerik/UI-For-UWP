using System;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Input.NumericBox
{
    /// <summary>
    /// Represents a special <see cref="TextBox"/> that resides in a <see cref="RadNumericBox"/> control.
    /// </summary>
    public class NumericTextBox : TextBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumericTextBox" /> class.
        /// </summary>
        public NumericTextBox()
        {
            this.DefaultStyleKey = typeof(NumericTextBox);
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            RadNumericBox parent = ElementTreeHelper.FindVisualAncestor<RadNumericBox>(this);
            if (parent != null)
            {
                return new NumericTextBoxAutomationPeer(this, parent);
            }
            return base.OnCreateAutomationPeer();
        }
    }
}
