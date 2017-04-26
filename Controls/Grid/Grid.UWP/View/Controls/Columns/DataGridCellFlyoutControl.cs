using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a DataGridCellFlyoutControl control.
    /// </summary>
    public class DataGridCellFlyoutControl : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="Child"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register(nameof(Child), typeof(object), typeof(DataGridCellFlyoutControl), new PropertyMetadata(null));
        
        /// <summary>
        /// Identifies the <see cref="OuterBorderBrush"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty OuterBorderBrushProperty =
            DependencyProperty.Register(nameof(OuterBorderBrush), typeof(SolidColorBrush), typeof(DataGridCellFlyoutControl), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridCellFlyoutControl"/> class.
        /// </summary>
        public DataGridCellFlyoutControl()
        {
            this.DefaultStyleKey = typeof(DataGridCellFlyoutControl);
        }

        /// <summary>
        /// Gets or sets the child element of the <see cref="DataGridCellFlyoutControl"/>.
        /// </summary>
        public object Child
        {
            get { return (object)GetValue(ChildProperty); }
            set { this.SetValue(ChildProperty, value); }
        }

        /// <summary>
        /// Gets or sets the brush for the outer Border of the <see cref="DataGridCellFlyoutControl"/>.
        /// </summary>
        public SolidColorBrush OuterBorderBrush
        {
            get { return (SolidColorBrush)GetValue(OuterBorderBrushProperty); }
            set { this.SetValue(OuterBorderBrushProperty, value); }
        }
    }
}
