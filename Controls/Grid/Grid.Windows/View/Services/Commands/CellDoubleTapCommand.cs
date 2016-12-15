using System;
using System.Linq;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    internal class CellDoubleTapCommand : DataGridCommand
    {
        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        public override bool CanExecute(object parameter)
        {
            return parameter is DataGridCellInfo;
        }

        /// <summary>
        /// Performs the core action given the provided parameter.
        /// </summary>
        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            var cellInfo = parameter as DataGridCellInfo;

            this.Owner.OnCellDoubleTap(cellInfo);
        }
    }
}