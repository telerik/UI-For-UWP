using System;
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
    }
}
