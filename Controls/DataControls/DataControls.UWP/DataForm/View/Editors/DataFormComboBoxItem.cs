using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a custom ComboBoxItem element.
    /// </summary>
    public class DataFormComboBoxItem : ComboBoxItem
    {
        /// <summary>
        /// Identifies the <see cref="SelectedForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedForegroundProperty =
            DependencyProperty.Register(nameof(SelectedForeground), typeof(Brush), typeof(DataFormComboBoxItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SelectedBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedBackgroundProperty =
            DependencyProperty.Register(nameof(SelectedBackground), typeof(Brush), typeof(DataFormComboBoxItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DisabledForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisabledForegroundProperty =
            DependencyProperty.Register(nameof(DisabledForeground), typeof(Brush), typeof(DataFormComboBoxItem), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFormComboBoxItem"/> class.
        /// </summary>
        public DataFormComboBoxItem()
        {
            this.DefaultStyleKey = typeof(DataFormComboBoxItem);
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
    }
}
