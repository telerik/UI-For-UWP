using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid
{
    public partial class RadDataGrid
    {
        /// <summary>
        /// Identifies the <see cref="RowDetailsDisplayMode"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty RowDetailsDisplayModeProperty =
            DependencyProperty.Register(nameof(RowDetailsDisplayMode), typeof(DataGridRowDetailsMode), typeof(RadDataGrid), new PropertyMetadata(DataGridRowDetailsMode.None, OnRowDetailsModeChanged));

        /// <summary>
        /// Identifies the <see cref="RowDetailsTemplate"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty RowDetailsTemplateProperty =
            DependencyProperty.Register(nameof(RowDetailsTemplate), typeof(DataTemplate), typeof(RadDataGrid), new PropertyMetadata(null, OnRowDetailsTemplateChanged));

        internal RowDetailsService rowDetailsService;

        RowDetailsService IGridView.RowDetailsService
        {
            get
            {
                return this.rowDetailsService;
            }
        }

        /// <summary>
        /// Gets or sets the display mode for the row details - whether they should be visualized or not.
        /// </summary>
        public DataGridRowDetailsMode RowDetailsDisplayMode
        {
            get { return (DataGridRowDetailsMode)GetValue(RowDetailsDisplayModeProperty); }
            set { SetValue(RowDetailsDisplayModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Template of the row details.
        /// </summary>
        public DataTemplate RowDetailsTemplate
        {
            get { return (DataTemplate)GetValue(RowDetailsTemplateProperty); }
            set { SetValue(RowDetailsTemplateProperty, value); }
        }

        /// <summary>
        /// Hide the row details.
        /// </summary>
        /// <param name="item">The item for which the row details should be hide.</param>
        public void HideRowDetailsForItem(object item)
        {
            this.rowDetailsService.CollapseDetailsForItem(item);
        }

        /// <summary>
        /// Show the row details.
        /// </summary>
        /// <param name="item">The item that should have its row details visualized.</param>
        public void ShowRowDetailsForItem(object item)
        {
            this.rowDetailsService.ExpandDetailsForItem(item);
        }

        private static void OnRowDetailsModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RadDataGrid).rowDetailsService.UpdateItems();
        }

        private static void OnRowDetailsTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RadDataGrid).rowDetailsService.UpdateItems();
        }
    }
}
