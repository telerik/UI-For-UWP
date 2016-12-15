using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a custom templated <see cref="ComboBox"/> used in a <see cref="DataGridFilterControlBase"/> instance.
    /// </summary>
    public class DataGridFilterComboBox : ComboBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridFilterComboBox" /> class.
        /// </summary>
        public DataGridFilterComboBox()
        {
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DataGridFilterComboBoxItem();
        }
    }
}
