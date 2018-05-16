using Telerik.UI.Xaml.Controls.Data.DataForm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a ListEditor control.
    /// </summary>
    public class ListEditor : ComboBox, ITypeEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListEditor"/> class.
        /// </summary>
        public ListEditor()
        {
            this.DefaultStyleKey = typeof(ListEditor);
        }

        /// <summary>
        /// Method used for generating bindings for the <see cref="ITypeEditor"/> properties.
        /// </summary>
        public void BindEditor()
        {
            Binding b1 = new Binding();
            b1.Path = new PropertyPath("ValueOptions");
            this.SetBinding(ListEditor.ItemsSourceProperty, b1);

            Binding b = new Binding() { Mode = BindingMode.TwoWay };
            b.Path = new PropertyPath("PropertyValue");
            this.SetBinding(ListEditor.SelectedItemProperty, b);

            Binding b2 = new Windows.UI.Xaml.Data.Binding();
            b2.Path = new PropertyPath("Watermark");
            this.SetBinding(ListEditor.PlaceholderTextProperty, b2);

            Binding b3 = new Binding();
            b3.Converter = new IsEnabledEditorConvetrer();
            b3.Path = new PropertyPath(string.Empty);
            this.SetBinding(ListEditor.IsEnabledProperty, b3);
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
