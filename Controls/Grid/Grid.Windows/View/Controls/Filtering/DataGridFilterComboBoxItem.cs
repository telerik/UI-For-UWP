using System;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a <see cref="ComboBoxItem"/> that appears within the operator combo box in a 
    /// <see cref="DataGridFilterControlBase"/> control.
    /// </summary>
    public class DataGridFilterComboBoxItem : ComboBoxItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridFilterComboBoxItem" /> class.
        /// </summary>
        public DataGridFilterComboBoxItem()
        {
            this.DefaultStyleKey = typeof(DataGridFilterComboBoxItem);
        }
    }
}
