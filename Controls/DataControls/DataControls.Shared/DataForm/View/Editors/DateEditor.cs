
using Telerik.UI.Xaml.Controls.Data.DataForm;
using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
namespace Telerik.UI.Xaml.Controls.Data
{
    public class DateEditor : RadDatePicker, ITypeEditor
    {
        public Style LabelIconStyle
        {
            get { return (Style)GetValue(LabelIconStyleProperty); }
            set { SetValue(LabelIconStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelIconStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelIconStyleProperty =
            DependencyProperty.Register("LabelIconStyle", typeof(Style), typeof(DateEditor), new PropertyMetadata(null));

        public Style ErrorIconStyle
        {
            get { return (Style)GetValue(ErrorIconStyleProperty); }
            set { SetValue(ErrorIconStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorIconStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorIconStyleProperty =
            DependencyProperty.Register("ErrorIconStyle", typeof(Style), typeof(DateEditor), new PropertyMetadata(null));

        public bool HasErrors
        {
            get { return (bool)GetValue(HasErrorsProperty); }
            set { SetValue(HasErrorsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasErrors.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasErrorsProperty =
            DependencyProperty.Register("HasErrors", typeof(bool), typeof(DateEditor), new PropertyMetadata(false));    

        public EditorIconDisplayMode IconDisplayMode
        {
            get { return (EditorIconDisplayMode)GetValue(IconDisplayModeProperty); }
            set { SetValue(IconDisplayModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconDisplayMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconDisplayModeProperty =
            DependencyProperty.Register("IconDisplayMode", typeof(EditorIconDisplayMode), typeof(DateEditor), new PropertyMetadata(EditorIconDisplayMode.None)); 

        
        public DateEditor()
        {
            this.DefaultStyleKey = (typeof(DateEditor));
        }

        public void BindEditor()
        {
            Binding b = new Binding() { Mode = BindingMode.TwoWay };
            b.Path = new Windows.UI.Xaml.PropertyPath("PropertyValue");
            this.SetBinding(DateEditor.ValueProperty, b);

            Binding b1 = new Binding();
            b1.Path = new PropertyPath("Errors.Count");
            b1.Converter = new EditorIconVisibilityConverter();
            b1.ConverterParameter = "HasErrors";
            this.SetBinding(DateEditor.HasErrorsProperty, b1);

            Binding b2 = new Binding();
            b2.Path = new PropertyPath("Watermark");
            this.SetBinding(DateEditor.EmptyContentProperty, b2);

            Binding b3 = new Binding();
            b3.Converter = new IsEnabledEditorConvetrer();
            b3.Path = new PropertyPath(string.Empty);
            this.SetBinding(DateEditor.IsEnabledProperty, b3);
        }
    }
}
