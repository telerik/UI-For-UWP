using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Map
{
    internal class ShapeLayerSourceChangedCommand : MapCommand
    {
        public override bool CanExecute(object parameter)
        {
            return parameter is MapShapeLayer;
        }

        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            // TODO: Do we need to perform another logic here?
        }
    }
}
