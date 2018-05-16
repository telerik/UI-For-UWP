using Telerik.UI.Xaml.Controls.Data.DataForm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents an EnumEditor control.
    /// </summary>
    public class EnumEditor : ComboBox, ITypeEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumEditor"/> class.
        /// </summary>
        public EnumEditor()
        {
            this.DefaultStyleKey = typeof(EnumEditor);
        }
        
        /// <summary>
        /// Method used for generating bindings for the <see cref="ITypeEditor"/> properties.
        /// </summary>
        public void BindEditor()
        {
            Binding b = new Binding() { Mode = BindingMode.TwoWay };
            b.Path = new PropertyPath("PropertyValue");
            this.SetBinding(EnumEditor.SelectedItemProperty, b);
            Binding b1 = new Binding();
            b1.Converter = new EnumToItemsSourceConverter();
            b1.Path = new PropertyPath("PropertyType");
            this.SetBinding(EnumEditor.ItemsSourceProperty, b1);

            Binding b2 = new Binding();
            b2.Path = new PropertyPath("Watermark");
            this.SetBinding(EnumEditor.PlaceholderTextProperty, b2);

            Binding b3 = new Binding();
            b3.Converter = new IsEnabledEditorConvetrer();
            b3.Path = new PropertyPath(string.Empty);
            this.SetBinding(EnumEditor.IsEnabledProperty, b3);

            Binding b4 = new Binding();
            b4.Path = new PropertyPath("PropertyValue");
            b4.Converter = new EnumToIndexConverter();
            this.SetBinding(EnumEditor.SelectedIndexProperty, b4);
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DataFormComboBoxItem();
        }
    }
}
