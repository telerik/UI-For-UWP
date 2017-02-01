using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Input.AutoCompleteBox
{
    /// <summary>
    /// Represents the TextBox part of the RadAutoCompleteBox. Exposed for easier styling via implicit style.
    /// </summary>
    public class AutoCompleteTextBox : TextBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCompleteTextBox" /> class.
        /// </summary>
        public AutoCompleteTextBox()
        {
            this.DefaultStyleKey = typeof(AutoCompleteTextBox);
        }
    }
}
