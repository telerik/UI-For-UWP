using Telerik.UI.Xaml.Controls.Primitives.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    public class ColumnHeaderDataOperationsButton : InlineButton
    {
        public string IconText
        {
            get { return (string)GetValue(IconTextProperty); }
            set { SetValue(IconTextProperty, value); }
        }   

        // Using a DependencyProperty as the backing store for IconText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconTextProperty =
            DependencyProperty.Register(nameof(IconText), typeof(string), typeof(ColumnHeaderDataOperationsButton), new PropertyMetadata(null));
        
        public ColumnHeaderDataOperationsButton()
        {
            this.DefaultStyleKey = typeof(ColumnHeaderDataOperationsButton);
        }
    }
}
