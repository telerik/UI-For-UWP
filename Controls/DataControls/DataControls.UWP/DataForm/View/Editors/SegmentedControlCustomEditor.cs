using Telerik.UI.Xaml.Controls.Data.DataForm;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a SegmentedCustomEditor control.
    /// </summary>
    public class SegmentedCustomEditor : RadSegmentedControl, ITypeEditor
    {
        /// <summary>
        /// Identifies the <see cref="SelectedForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedForegroundProperty =
            DependencyProperty.Register(nameof(SelectedForeground), typeof(Brush), typeof(SegmentedCustomEditor), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SelectedBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedBackgroundProperty =
            DependencyProperty.Register(nameof(SelectedBackground), typeof(Brush), typeof(SegmentedCustomEditor), new PropertyMetadata(null));
        
        /// <summary>
        /// Identifies the <see cref="DisabledForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisabledForegroundProperty =
            DependencyProperty.Register(nameof(DisabledForeground), typeof(Brush), typeof(SegmentedCustomEditor), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentedCustomEditor"/> class.
        /// </summary>
        public SegmentedCustomEditor()
        {
            this.DefaultStyleKey = typeof(SegmentedCustomEditor);
        }

        /// <summary>
        /// Gets or sets the background of the <see cref="SegmentedCustomEditor"/> when it gets selected.
        /// </summary>
        public Brush SelectedBackground
        {
            get { return (Brush)GetValue(SelectedBackgroundProperty); }
            set { SetValue(SelectedBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the foreground of the <see cref="SegmentedCustomEditor"/> when it gets selected.
        /// </summary>
        public Brush SelectedForeground
        {
            get { return (Brush)GetValue(SelectedForegroundProperty); }
            set { SetValue(SelectedForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the foreground of the <see cref="SegmentedCustomEditor"/> when it gets disabled.
        /// </summary>
        public Brush DisabledForeground
        {
            get { return (Brush)GetValue(DisabledForegroundProperty); }
            set { SetValue(DisabledForegroundProperty, value); }
        }

        public void BindEditor()
        {
            Binding b1 = new Binding();
            b1.Path = new PropertyPath("ValueOptions");
            this.SetBinding(SegmentedCustomEditor.ItemsSourceProperty, b1);

            Binding b2 = new Binding() { Mode = BindingMode.TwoWay };
            b2.Path = new PropertyPath("PropertyValue");
            this.SetBinding(SegmentedCustomEditor.SelectedItemProperty, b2);

            Binding b3 = new Binding();
            b3.Converter = new IsEnabledEditorConvetrer();
            b3.Path = new PropertyPath(string.Empty);
            this.SetBinding(SegmentedCustomEditor.IsEnabledProperty, b3);
        }
    }
}
