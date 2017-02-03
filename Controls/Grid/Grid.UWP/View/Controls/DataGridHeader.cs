using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Allows a user to view a header for some details.
    /// </summary>
    public abstract class DataGridHeader : RadControl
    {
        /// <summary>
        /// Identifies the DataTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register(nameof(ContentTemplate), typeof(DataTemplate), typeof(DataGridHeader), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridHeader"/> class.
        /// </summary>
        protected DataGridHeader()
        {
            this.DefaultStyleKey = typeof(DataGridHeader);
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display each item.
        /// </summary>
        /// <returns>The template that specifies the visualization of the data object. The default is null.</returns>
        public DataTemplate ContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ContentTemplateProperty);
            }
            set
            {
                this.SetValue(ContentTemplateProperty, value);
            }
        }
    }
}