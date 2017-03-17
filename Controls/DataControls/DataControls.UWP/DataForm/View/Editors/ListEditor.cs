using System.Collections;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Data.DataForm;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    public class ListEditor : ComboBox, ITypeEditor
    {
        public ListEditor()
        {
            this.DefaultStyleKey = typeof(ListEditor);
        }

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
    }
}
