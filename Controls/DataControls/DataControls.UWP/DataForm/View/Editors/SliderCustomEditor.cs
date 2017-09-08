using Telerik.UI.Xaml.Controls.Data.DataForm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a SliderCustomEditor control.
    /// </summary>
    public class SliderCustomEditor : Slider, ITypeEditor
    {
        /// <summary>
        /// Identifies the <see cref="ThumbBackground"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ThumbBackgroundProperty =
            DependencyProperty.Register(nameof(ThumbBackground), typeof(Brush), typeof(SliderCustomEditor), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="SliderCustomEditor"/> class.
        /// </summary>
        public SliderCustomEditor()
        {
            this.DefaultStyleKey = typeof(SliderCustomEditor);
        }

        /// <summary>
        /// Gets or sets the Background of the Horizontal and Vertical <see cref="Thumb"/>s that are used to change the value of the <see cref="Slider"/> control.
        /// </summary>
        public Brush ThumbBackground
        {
            get
            {
                return (Brush)GetValue(ThumbBackgroundProperty);
            }
            set
            {
                SetValue(ThumbBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Method used for generating bindings for the <see cref="ITypeEditor"/> properties.
        /// </summary>
        public void BindEditor()
        {
            Binding b1 = new Binding();
            b1.Path = new PropertyPath("Range.Min");
            this.SetBinding(SliderCustomEditor.MinimumProperty, b1);

            Binding b2 = new Binding();
            b2.Path = new PropertyPath("Range.Max");
            this.SetBinding(SliderCustomEditor.MaximumProperty, b2);
            
            Binding b3 = new Binding();
            b3.Converter = new IsEnabledEditorConvetrer();
            b3.Path = new PropertyPath(string.Empty);
            this.SetBinding(SliderCustomEditor.IsEnabledProperty, b3);

            Binding b4 = new Binding();
            b4.Path = new PropertyPath("Range.Step");
            this.SetBinding(SliderCustomEditor.StepFrequencyProperty, b4);

            Binding b5 = new Binding() { Mode = BindingMode.TwoWay };
            b5.Path = new PropertyPath("PropertyValue");
            this.SetBinding(SliderCustomEditor.ValueProperty, b5);
        }
    }
}
