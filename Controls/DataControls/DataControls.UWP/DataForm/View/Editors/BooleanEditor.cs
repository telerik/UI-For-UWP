using Telerik.UI.Xaml.Controls.Data.DataForm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a BooleanEditor control.
    /// </summary>
    public class BooleanEditor : CheckBox, ITypeEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanEditor"/> class.
        /// </summary>
        public BooleanEditor()
        {
            this.DefaultStyleKey = typeof(CheckBox);
        }

        /// <summary>
        /// Method used for generating bindings for the <see cref="ITypeEditor"/> properties.
        /// </summary>
        public void BindEditor()
        {
            Binding b = new Binding() { Mode = BindingMode.TwoWay };
            b.Path = new PropertyPath("PropertyValue");
            this.SetBinding(BooleanEditor.IsCheckedProperty, b);

            Binding b3 = new Binding();
            b3.Converter = new IsEnabledEditorConvetrer();
            b3.Path = new PropertyPath(string.Empty);
            this.SetBinding(BooleanEditor.IsEnabledProperty, b3);
        }
    }
}
