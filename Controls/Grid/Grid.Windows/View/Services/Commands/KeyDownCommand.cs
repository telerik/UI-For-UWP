using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    internal class KeyDownCommand : DataGridCommand
    {
        public override bool CanExecute(object parameter)
        {
            return this.Owner != null && parameter is KeyRoutedEventArgs;
        }

        public override void Execute(object parameter)
        {
            var args = parameter as KeyRoutedEventArgs;
            this.Owner.HandleKeyDown(args);
        }
    }
}
