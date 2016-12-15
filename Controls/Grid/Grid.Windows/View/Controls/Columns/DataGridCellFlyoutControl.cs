using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    public class DataGridCellFlyoutControl : RadControl
    {
        public object Child
        {
            get { return (object)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register("Child", typeof(object), typeof(DataGridCellFlyoutControl), new PropertyMetadata(null));

        public SolidColorBrush OuterBorderBrush
        {
            get { return (SolidColorBrush)GetValue(OuterBorderBrushProperty); }
            set { SetValue(OuterBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OuterBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OuterBorderBrushProperty =
            DependencyProperty.Register("OuterBorderBrush", typeof(SolidColorBrush), typeof(DataGridCellFlyoutControl), new PropertyMetadata(null));

        
        public DataGridCellFlyoutControl()
        {
            this.DefaultStyleKey = typeof(DataGridCellFlyoutControl);
        }
    }
}
