using Telerik.UI.Xaml.Controls.Data.DataForm;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a DateEditor control.
    /// </summary>
    public class DateEditor : RadDatePicker, ITypeEditor
    {
        /// <summary>
        /// Gets or sets the style for the label icon of the editor.
        /// </summary>
        public Style LabelIconStyle
        {
            get { return (Style)GetValue(LabelIconStyleProperty); }
            set { SetValue(LabelIconStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="LabelIconStyle"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty LabelIconStyleProperty =
            DependencyProperty.Register(nameof(LabelIconStyle), typeof(Style), typeof(DateEditor), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the style for the error icon of the editor.
        /// </summary>
        public Style ErrorIconStyle
        {
            get { return (Style)GetValue(ErrorIconStyleProperty); }
            set { SetValue(ErrorIconStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ErrorIconStyle"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ErrorIconStyleProperty =
            DependencyProperty.Register(nameof(ErrorIconStyle), typeof(Style), typeof(DateEditor), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets whether the control has errors.
        /// </summary>
        public bool HasErrors
        {
            get { return (bool)GetValue(HasErrorsProperty); }
            set { SetValue(HasErrorsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="HasErrors"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty HasErrorsProperty =
            DependencyProperty.Register(nameof(HasErrors), typeof(bool), typeof(DateEditor), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the display mode of the icon for the editor.
        /// </summary>
        public EditorIconDisplayMode IconDisplayMode
        {
            get { return (EditorIconDisplayMode)GetValue(IconDisplayModeProperty); }
            set { SetValue(IconDisplayModeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IconDisplayMode"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty IconDisplayModeProperty =
            DependencyProperty.Register(nameof(IconDisplayMode), typeof(EditorIconDisplayMode), typeof(DateEditor), new PropertyMetadata(EditorIconDisplayMode.None));

        /// <summary>
        /// Initializes a new instance of the <see cref="DateEditor"/> class.
        /// </summary>
        public DateEditor()
        {
            this.DefaultStyleKey = (typeof(DateEditor));
        }

        /// <summary>
        /// Method used for generating bindings for the <see cref="ITypeEditor"/> properties.
        /// </summary>
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
