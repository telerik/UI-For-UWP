using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Primitives.SideDrawer.Commands
{
    /// <summary>
    /// A command executed when the key is pressed down inside an element placed in the SideDrawer.
    /// </summary>
    public class SideDrawerKeyDownCommand : SideDrawerCommand
    {
        /// <inheritdoc/>
        public override bool CanExecute(object parameter)
        {
            return parameter is KeyRoutedEventArgs;
        }

        /// <inheritdoc/>
        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            var e = parameter as KeyRoutedEventArgs;

            if (e.OriginalSource is RadSideDrawer && (e.Key == Windows.System.VirtualKey.Space || e.Key == Windows.System.VirtualKey.Enter))
            {
                this.Owner.ToggleDrawer();
                e.Handled = true;
            }
        }
    }
}
