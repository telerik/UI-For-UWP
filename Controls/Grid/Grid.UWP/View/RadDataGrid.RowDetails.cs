using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid
{
    public partial class RadDataGrid
    {
        // Using a DependencyProperty as the backing store for RowDetailsDisplayMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowDetailsDisplayModeProperty =
            DependencyProperty.Register("RowDetailsDisplayMode", typeof(DataGridRowDetailsMode), typeof(RadDataGrid), new PropertyMetadata(DataGridRowDetailsMode.None, OnRowDetailsModeChanged));

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowDetailsTemplateProperty =
            DependencyProperty.Register("RowDetailsTemplate", typeof(DataTemplate), typeof(RadDataGrid), new PropertyMetadata(null, OnRowDetailsTemplateChanged));

        internal RowDetailsService rowDetailsService;

        RowDetailsService IGridView.RowDetailsService { get { return this.rowDetailsService; } }

        public DataGridRowDetailsMode RowDetailsDisplayMode
        {
            get { return (DataGridRowDetailsMode)GetValue(RowDetailsDisplayModeProperty); }
            set { SetValue(RowDetailsDisplayModeProperty, value); }
        }

        public DataTemplate RowDetailsTemplate
        {
            get { return (DataTemplate)GetValue(RowDetailsTemplateProperty); }
            set { SetValue(RowDetailsTemplateProperty, value); }
        }
        public void HideRowDetailsForItem(object item)
        {
            this.rowDetailsService.CollapseDetailsForItem(item);
        }

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
