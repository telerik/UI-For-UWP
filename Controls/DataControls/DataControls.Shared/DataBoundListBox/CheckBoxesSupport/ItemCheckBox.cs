using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents the check box within a <see cref="RadDataBoundListBoxItem"/>.
    /// </summary>
    public class ItemCheckBox : CheckBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCheckBox" /> class.
        /// </summary>
        public ItemCheckBox()
        {
            this.DefaultStyleKey = typeof(ItemCheckBox);
        }
    }
}
