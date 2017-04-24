using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.DataForm
{
    /// <summary>
    /// Represents a DataFormTextBox control.
    /// </summary>
    public class DataFormTextBox : TextBox
    {
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
            DependencyProperty.Register(nameof(IconDisplayMode), typeof(EditorIconDisplayMode), typeof(DataFormTextBox), new PropertyMetadata(null));

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
            DependencyProperty.Register(nameof(LabelIconStyle), typeof(Style), typeof(DataFormTextBox), new PropertyMetadata(null));

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
            DependencyProperty.Register(nameof(ErrorIconStyle), typeof(Style), typeof(DataFormTextBox), new PropertyMetadata(null));

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
            DependencyProperty.Register(nameof(HasErrors), typeof(bool), typeof(DataFormTextBox), new PropertyMetadata(false));

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFormTextBox"/> class.
        /// </summary>
        public DataFormTextBox()
        {
            this.DefaultStyleKey = typeof(DataFormTextBox);
        }
    }
}
