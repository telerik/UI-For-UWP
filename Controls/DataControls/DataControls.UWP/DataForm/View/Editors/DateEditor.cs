using Telerik.UI.Xaml.Controls.Data.DataForm;
using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Input.DateTimePickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a DateEditor control.
    /// </summary>
    public class DateEditor : RadDatePicker, ITypeEditor
    {
        /// <summary>
        /// Identifies the <see cref="LabelIconStyle"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty LabelIconStyleProperty =
            DependencyProperty.Register(nameof(LabelIconStyle), typeof(Style), typeof(DateEditor), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ErrorIconStyle"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ErrorIconStyleProperty =
            DependencyProperty.Register(nameof(ErrorIconStyle), typeof(Style), typeof(DateEditor), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="HasErrors"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty HasErrorsProperty =
            DependencyProperty.Register(nameof(HasErrors), typeof(bool), typeof(DateEditor), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IconDisplayMode"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty IconDisplayModeProperty =
            DependencyProperty.Register(nameof(IconDisplayMode), typeof(EditorIconDisplayMode), typeof(DateEditor), new PropertyMetadata(EditorIconDisplayMode.None));

        /// <summary>
        /// Identifies the <see cref="HighlightFillBrush"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty HighlightFillBrushProperty =
            DependencyProperty.Register(nameof(HighlightFillBrush), typeof(Brush), typeof(DateEditor), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SelectedBackground"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty SelectedBackgroundProperty =
            DependencyProperty.Register(nameof(SelectedBackground), typeof(Brush), typeof(DateEditor), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SelectedForeground"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty SelectedForegroundProperty =
            DependencyProperty.Register(nameof(SelectedForeground), typeof(Brush), typeof(DateEditor), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="DateEditor"/> class.
        /// </summary>
        public DateEditor()
        {
            this.DefaultStyleKey = typeof(DateEditor);
        }

        /// <summary>
        /// Gets or sets the display mode of the icon for the editor.
        /// </summary>
        public EditorIconDisplayMode IconDisplayMode
        {
            get { return (EditorIconDisplayMode)GetValue(IconDisplayModeProperty); }
            set { this.SetValue(IconDisplayModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control has errors.
        /// </summary>
        public bool HasErrors
        {
            get { return (bool)GetValue(HasErrorsProperty); }
            set { this.SetValue(HasErrorsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for the label icon of the editor.
        /// </summary>
        public Style LabelIconStyle
        {
            get { return (Style)GetValue(LabelIconStyleProperty); }
            set { this.SetValue(LabelIconStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for the error icon of the editor.
        /// </summary>
        public Style ErrorIconStyle
        {
            get { return (Style)GetValue(ErrorIconStyleProperty); }
            set { this.SetValue(ErrorIconStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Background of the rectangle area of the <see cref="CheckBox"/> when the control is checked.
        /// </summary>
        public Brush HighlightFillBrush
        {
            get { return (Brush)GetValue(HighlightFillBrushProperty); }
            set { this.SetValue(HighlightFillBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Background of the rectangle area of the <see cref="DateTimeListItem"/> when the item is selected.
        /// </summary>
        public Brush SelectedBackground
        {
            get { return (Brush)GetValue(SelectedBackgroundProperty); }
            set { this.SetValue(SelectedBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Foreground of the rectangle area of the <see cref="DateTimeListItem"/> when the item is selected.
        /// </summary>
        public Brush SelectedForeground
        {
            get { return (Brush)GetValue(SelectedForegroundProperty); }
            set { this.SetValue(SelectedForegroundProperty, value); }
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
