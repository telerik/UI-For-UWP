using Telerik.UI.Xaml.Controls.Data.DataForm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a BooleanEditor control.
    /// </summary>
    public class BooleanEditor : CheckBox, ITypeEditor
    {
        /// <summary>
        /// Identifies the <see cref="CheckedStateBackgroundBrush"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CheckedStateBackgroundBrushProperty =
            DependencyProperty.Register(nameof(CheckedStateBackgroundBrush), typeof(Brush), typeof(BooleanEditor), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanEditor"/> class.
        /// </summary>
        public BooleanEditor()
        {
            this.DefaultStyleKey = typeof(BooleanEditor);
        }

        /// <summary>
        /// Gets or sets the Background of the rectangle area of the <see cref="CheckBox"/> when the control is checked.
        /// </summary>
        public Brush CheckedStateBackgroundBrush
        {
            get
            {
                return (Brush)GetValue(CheckedStateBackgroundBrushProperty);
            }
            set
            {
                this.SetValue(CheckedStateBackgroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Method used for generating bindings for the <see cref="ITypeEditor"/> properties.
        /// </summary>
        public void BindEditor()
        {
            Binding b = new Binding() { Mode = BindingMode.TwoWay };
            b.Path = new PropertyPath("PropertyValue");
            this.SetBinding(BooleanEditor.IsCheckedProperty, b);

            Binding b3 = new Binding();
            b3.Converter = new IsEnabledEditorConvetrer();
            b3.Path = new PropertyPath(string.Empty);
            this.SetBinding(BooleanEditor.IsEnabledProperty, b3);
        }
    }
}
