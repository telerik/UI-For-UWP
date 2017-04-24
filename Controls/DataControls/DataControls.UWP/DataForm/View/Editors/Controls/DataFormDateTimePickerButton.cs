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
            DependencyProperty.Register(nameof(ErrorIconStyle), typeof(Style), typeof(DataFormDateTimePickerButton), new PropertyMetadata(null));

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
            DependencyProperty.Register(nameof(HasErrors), typeof(bool), typeof(DataFormDateTimePickerButton), new PropertyMetadata(false));

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
            DependencyProperty.Register(nameof(IconDisplayMode), typeof(EditorIconDisplayMode), typeof(DataFormDateTimePickerButton), new PropertyMetadata(EditorIconDisplayMode.None));

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
            DependencyProperty.Register(nameof(LabelIconStyle), typeof(Style), typeof(DataFormDateTimePickerButton), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFormDateTimePickerButton"/> class.
        /// </summary>
        public DataFormDateTimePickerButton()
        {
            this.DefaultStyleKey = typeof(DataFormDateTimePickerButton);
        }
    }
}
