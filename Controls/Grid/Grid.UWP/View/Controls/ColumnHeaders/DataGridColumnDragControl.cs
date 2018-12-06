using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents the drag visual used when dragging RadDataGrid columns.
    /// </summary>
    public class DataGridColumnDragControl : RadContentControl
    {
        /// <summary>
        /// Identifies the <see cref="FilterGlyphVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FilterGlyphVisibilityProperty =
            DependencyProperty.Register(nameof(FilterGlyphVisibility), typeof(Visibility), typeof(DataGridColumnDragControl), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridColumnDragControl" /> class.
        /// </summary>
        public DataGridColumnDragControl()
        {
            this.DefaultStyleKey = typeof(DataGridColumnDragControl);
        }

        /// <summary>
        /// Gets or sets the visibility of the filter glyph displayed on the right of the header.
        /// </summary>
        public Visibility FilterGlyphVisibility
        {
            get
            {
                return (Visibility)this.GetValue(FilterGlyphVisibilityProperty);
            }
            set
            {
                this.SetValue(FilterGlyphVisibilityProperty, value);
            }
        }

        /// <inheritdoc />
        protected override void OnUnloaded(object sender, RoutedEventArgs e)
        {
            base.OnUnloaded(sender, e);
            this.Content = null;
        }
    }
}
