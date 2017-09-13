using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Data.DataForm
{
    /// <summary>
    /// Represents a DataFormNumericTextBox control.
    /// </summary>
    public class DataFormNumericTextBox : DataFormTextBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataFormNumericTextBox"/> class.
        /// </summary>
        public DataFormNumericTextBox()
        {
            this.DefaultStyleKey = typeof(DataFormNumericTextBox);
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
