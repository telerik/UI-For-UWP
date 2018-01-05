using Telerik.UI.Xaml.Controls.Data.DataForm;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a NumericEditor control.
    /// </summary>
    public class NumericEditor : RadNumericBox, ITypeEditor
    {
        /// <summary>
        /// Identifies the <see cref="ErrorIconStyle"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ErrorIconStyleProperty =
            DependencyProperty.Register(nameof(ErrorIconStyle), typeof(Style), typeof(NumericEditor), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="HasErrors"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty HasErrorsProperty =
            DependencyProperty.Register(nameof(HasErrors), typeof(bool), typeof(NumericEditor), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="LabelIconStyle"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty LabelIconStyleProperty =
            DependencyProperty.Register(nameof(LabelIconStyle), typeof(Style), typeof(NumericEditor), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IconDisplayMode"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty IconDisplayModeProperty =
            DependencyProperty.Register(nameof(IconDisplayMode), typeof(EditorIconDisplayMode), typeof(NumericEditor), new PropertyMetadata(EditorIconDisplayMode.None));

        /// <summary>
        /// Identifies the <see cref="ButtonsBackground"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ButtonsBackgroundProperty =
            DependencyProperty.Register("ButtonsBackground", typeof(Brush), typeof(NumericEditor), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ButtonsPointerOverBackgroundBrush"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ButtonsPointerOverBackgroundBrushProperty =
            DependencyProperty.Register("ButtonsPointerOverBackgroundBrush", typeof(Brush), typeof(NumericEditor), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericEditor"/> class.
        /// </summary>
        public NumericEditor()
        {
            this.DefaultStyleKey = typeof(NumericEditor);
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
        /// Gets or sets the background of the up and down buttons of the <see cref="NumericEditor"/>.
        /// </summary>
        public Brush ButtonsBackground
        {
            get { return (Brush)GetValue(ButtonsBackgroundProperty); }
            set { SetValue(ButtonsBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the background of the up and down buttons of the <see cref="NumericEditor"/> when the buttons are pressed.
        /// </summary>
        public Brush ButtonsPointerOverBackgroundBrush
        {
            get { return (Brush)GetValue(ButtonsPointerOverBackgroundBrushProperty); }
            set { SetValue(ButtonsPointerOverBackgroundBrushProperty, value); }
        }

        /// <summary>
        /// Method used for generating bindings for the <see cref="ITypeEditor"/> properties.
        /// </summary>
        public void BindEditor()
        {
            Binding b = new Binding() { Mode = BindingMode.TwoWay };
            b.Path = new PropertyPath("PropertyValue");
            this.SetBinding(NumericEditor.ValueProperty, b);

            Binding b1 = new Binding();
            b1.Path = new PropertyPath("Errors.Count");
            b1.Converter = new EditorIconVisibilityConverter();
            b1.ConverterParameter = "HasErrors";
            this.SetBinding(NumericEditor.HasErrorsProperty, b1);

            Binding b2 = new Binding();
            b2.Path = new PropertyPath("Watermark");
            this.SetBinding(NumericEditor.WatermarkProperty, b2);

            Binding b3 = new Binding();
            b3.Converter = new IsEnabledEditorConvetrer();
            b3.Path = new PropertyPath(string.Empty);
            this.SetBinding(NumericEditor.IsEnabledProperty, b3);

            Binding rangeB = new Binding();
            rangeB.Path = new PropertyPath("Range.Min");
            rangeB.FallbackValue = double.MinValue;
            this.SetBinding(NumericEditor.MinimumProperty, rangeB);

            rangeB = new Binding();
            rangeB.Path = new PropertyPath("Range.Max");
            rangeB.FallbackValue = double.MaxValue;
            this.SetBinding(NumericEditor.MaximumProperty, rangeB);
        }
    }
}
