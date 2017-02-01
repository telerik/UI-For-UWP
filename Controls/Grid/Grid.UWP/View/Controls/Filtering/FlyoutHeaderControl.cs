using System.Windows.Input;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    public class FlyoutHeaderControl : RadControl
    {
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(FlyoutHeaderControl), new PropertyMetadata(null));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(FlyoutHeaderControl), new PropertyMetadata(null));

        public static readonly DependencyProperty IconStyleProperty =
            DependencyProperty.Register("IconStyle", typeof(Style), typeof(FlyoutHeaderControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ContentStyleProperty =
            DependencyProperty.Register("ContentStyle", typeof(Style), typeof(FlyoutHeaderControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ButtonStyleProperty =
            DependencyProperty.Register("ButtonStyle", typeof(Style), typeof(FlyoutHeaderControl), new PropertyMetadata(null));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(FlyoutHeaderControl), new PropertyMetadata(null));

        public FlyoutHeaderControl()
        {
            this.DefaultStyleKey = typeof(FlyoutHeaderControl);
        }

        public object Content
        {
            get
            {
                return GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        public object Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public Style IconStyle
        {
            get { return (Style)GetValue(IconStyleProperty); }
            set { SetValue(IconStyleProperty, value); }
        }

        public Style ContentStyle
        {
            get { return (Style)GetValue(ContentStyleProperty); }
            set { SetValue(ContentStyleProperty, value); }
        }

        public Style ButtonStyle
        {
            get { return (Style)GetValue(ButtonStyleProperty); }
            set { SetValue(ButtonStyleProperty, value); }
        }

        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public DataGridFilteringFlyout Owner { get; internal set; }
    }
}