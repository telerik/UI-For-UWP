
using Telerik.UI.Xaml.Controls.Data.DataForm;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
namespace Telerik.UI.Xaml.Controls.Data
{
    public class BooleanEditor : CheckBox, ITypeEditor
    {
        public BooleanEditor()
        {
            this.DefaultStyleKey = typeof(CheckBox);
        }

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
