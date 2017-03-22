using System;
using System.Windows.Input;
using Telerik.UI.Xaml.Controls.Primitives.Menu;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.RadialMenu
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Customizations : ExamplePageBase
    {
        public Customizations()
        {
            this.InitializeComponent();

            this.DataContext = new MenuCommand();
        }
    }

    public class MenuCommand : DependencyObject, ICommand
    {
        public static readonly DependencyProperty BackGroundElementProperty =
                 DependencyProperty.Register("BackGroundElement", typeof(Grid), typeof(MenuCommand), new PropertyMetadata(null));

        public event EventHandler CanExecuteChanged;

        public Grid BackGroundElement
        {
            get { return (Grid)GetValue(BackGroundElementProperty); }
            set { SetValue(BackGroundElementProperty, value); }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.BackGroundElement.Background = (parameter as RadialMenuItemContext).CommandParameter as SolidColorBrush;
        }
    }
}
