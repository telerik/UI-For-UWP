using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Primitives.SideDrawer.Commands
{
    public class SideDrawerKeyDownCommand : SideDrawerCommand
    {
        public override bool CanExecute(object parameter)
        {
            return parameter is KeyRoutedEventArgs;
        }

        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            var e = parameter as KeyRoutedEventArgs;

            if (e.Key == Windows.System.VirtualKey.Space || e.Key == Windows.System.VirtualKey.Enter)
            {
                this.Owner.ToggleDrawer();
            }
        }
    }
}
