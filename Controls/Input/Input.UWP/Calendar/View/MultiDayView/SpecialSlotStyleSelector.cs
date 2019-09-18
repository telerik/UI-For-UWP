using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// Provides a way to choose a style for the SpecialSlot based on the data object and the
    /// data-bound element.
    /// </summary>
    [Bindable]
    public class SpecialSlotStyleSelector : StyleSelector
    {
        /// <summary>
        /// Gets or sets the default style of the SpecialSlot.
        /// </summary>
        public Style DefaultStyle { get; set; }

        /// <summary>
        /// Gets or sets the read-only style of the SpecialSlot.
        /// </summary>
        public Style ReadOnlyStyle { get; set; }

        /// <summary>
        /// When overridden in a derived class, returns a Style based on custom logic.
        /// </summary>
        /// <param name="item">The content.</param>
        /// <param name="container">The element to which the style will be applied.</param>
        /// <returns>Returns an application-specific style to apply; otherwise, null.</returns>
        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            Slot slot = item as Slot;
            if (slot != null && slot.IsReadOnly)
            {
                return this.ReadOnlyStyle;
            }

            return this.DefaultStyle;
        }
    }
}
