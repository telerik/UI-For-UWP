using Telerik.UI.Xaml.Controls.Data.DataForm;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a SegmentedCustomEditor control.
    /// </summary>
    public class SegmentedCustomEditor : RadSegmentedControl, ITypeEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentedCustomEditor"/> class.
        /// </summary>
        public SegmentedCustomEditor()
        {
            this.DefaultStyleKey = typeof(SegmentedCustomEditor);
        }
        
        /// <summary>
        /// Method used for generating bindings for the <see cref="ITypeEditor"/> properties.
        /// </summary>
        public void BindEditor()
        {
            Binding b1 = new Binding();
            b1.Path = new PropertyPath("ValueOptions");
            this.SetBinding(SegmentedCustomEditor.ItemsSourceProperty, b1);

            Binding b2 = new Binding() { Mode = BindingMode.TwoWay };
            b2.Path = new PropertyPath("PropertyValue");
            this.SetBinding(SegmentedCustomEditor.SelectedItemProperty, b2);

            Binding b3 = new Binding();
            b3.Converter = new IsEnabledEditorConvetrer();
            b3.Path = new PropertyPath(string.Empty);
            this.SetBinding(SegmentedCustomEditor.IsEnabledProperty, b3);
        }
    }
}
