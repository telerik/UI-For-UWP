using Telerik.UI.Xaml.Controls.Data.DataForm;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    public class AutoCompleteEditor : RadAutoCompleteBox, ITypeEditor
    {
        public Style LabelIconStyle
        {
            get { return (Style)GetValue(LabelIconStyleProperty); }
            set { SetValue(LabelIconStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelIconStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelIconStyleProperty =
            DependencyProperty.Register("LabelIconStyle", typeof(Style), typeof(AutoCompleteEditor), new PropertyMetadata(null));

        public Style ErrorIconStyle
        {
            get { return (Style)GetValue(ErrorIconStyleProperty); }
            set { SetValue(ErrorIconStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorIconStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorIconStyleProperty =
            DependencyProperty.Register("ErrorIconStyle", typeof(Style), typeof(AutoCompleteEditor), new PropertyMetadata(null));

        public bool HasErrors
        {
            get { return (bool)GetValue(HasErrorsProperty); }
            set { SetValue(HasErrorsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasErrors.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasErrorsProperty =
            DependencyProperty.Register("HasErrors", typeof(bool), typeof(AutoCompleteEditor), new PropertyMetadata(false));

        public EditorIconDisplayMode IconDisplayMode
        {
            get { return (EditorIconDisplayMode)GetValue(IconDisplayModeProperty); }
            set { SetValue(IconDisplayModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconDisplayMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconDisplayModeProperty =
            DependencyProperty.Register("IconDisplayMode", typeof(EditorIconDisplayMode), typeof(AutoCompleteEditor), new PropertyMetadata(EditorIconDisplayMode.None));

        public AutoCompleteEditor()
        {
            this.DefaultStyleKey = typeof(AutoCompleteEditor);
        }

        public void BindEditor()
        {
            Binding b = new Binding() { Mode = BindingMode.TwoWay };
            b.Path = new  PropertyPath("PropertyValue");
            this.SetBinding(AutoCompleteEditor.TextProperty, b);

            Binding b1 = new Binding();
            b1.Path = new PropertyPath("Errors.Count");
            b1.Converter = new EditorIconVisibilityConverter();
            b1.ConverterParameter = "HasErrors";
            this.SetBinding(AutoCompleteEditor.HasErrorsProperty, b1);

            Binding b2 = new Binding();
            b2.Path = new PropertyPath("Watermark");
            this.SetBinding(AutoCompleteEditor.WatermarkProperty, b2);

            Binding b3 = new Binding();
            b3.Converter = new IsEnabledEditorConvetrer();
            b3.Path = new PropertyPath(string.Empty);
            this.SetBinding(AutoCompleteEditor.IsEnabledProperty, b3);

            Binding b4 = new Binding();
            b4.Path = new PropertyPath("ValueOptions");
            this.SetBinding(AutoCompleteEditor.ItemsSourceProperty, b4);
        }
    }
}
