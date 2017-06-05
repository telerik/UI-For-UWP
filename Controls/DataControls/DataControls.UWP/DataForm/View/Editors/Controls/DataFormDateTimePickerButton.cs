using Telerik.UI.Xaml.Controls.Input.DateTimePickers;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Data.DataForm
{
    /// <summary>
    /// Represents a DataFormDateTimePickerButton control.
    /// </summary>
    public class DataFormDateTimePickerButton : DateTimePickerButton
    {
        /// <summary>
        /// Identifies the <see cref="ErrorIconStyle"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ErrorIconStyleProperty =
            DependencyProperty.Register(nameof(ErrorIconStyle), typeof(Style), typeof(DataFormDateTimePickerButton), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="HasErrors"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty HasErrorsProperty =
            DependencyProperty.Register(nameof(HasErrors), typeof(bool), typeof(DataFormDateTimePickerButton), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IconDisplayMode"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty IconDisplayModeProperty =
            DependencyProperty.Register(nameof(IconDisplayMode), typeof(EditorIconDisplayMode), typeof(DataFormDateTimePickerButton), new PropertyMetadata(EditorIconDisplayMode.None));

        /// <summary>
        /// Identifies the <see cref="LabelIconStyle"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty LabelIconStyleProperty =
            DependencyProperty.Register(nameof(LabelIconStyle), typeof(Style), typeof(DataFormDateTimePickerButton), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFormDateTimePickerButton"/> class.
        /// </summary>
        public DataFormDateTimePickerButton()
        {
            this.DefaultStyleKey = typeof(DataFormDateTimePickerButton);
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
        /// Gets or sets the style for the label icon of the editor.
        /// </summary>
        public Style LabelIconStyle
        {
            get { return (Style)GetValue(LabelIconStyleProperty); }
            set { this.SetValue(LabelIconStyleProperty, value); }
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
        /// Gets or sets the display mode of the icon for the editor.
        /// </summary>
        public EditorIconDisplayMode IconDisplayMode
        {
            get { return (EditorIconDisplayMode)GetValue(IconDisplayModeProperty); }
            set { this.SetValue(IconDisplayModeProperty, value); }
        }
    }
}
