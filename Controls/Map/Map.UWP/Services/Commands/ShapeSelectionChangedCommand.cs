using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Map
{
    internal class ShapeSelectionChangedCommand : MapCommand
    {
        public override bool CanExecute(object parameter)
        {
            return this.Owner != null && parameter is SelectionChangedEventArgs;
        }

        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            // TODO: Do we need some other handling?
        }
    }
}
