using System;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    internal class CellPointerOverCommand : DataGridCommand
    {
        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        public override bool CanExecute(object parameter)
        {
            // TODO: connect this to datagrid can select.
            return parameter is DataGridCellInfo;
        }

        /// <summary>
        /// Performs the core action given the provided parameter.
        /// </summary>
        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            this.Owner.visualStateService.UpdateHoverDecoration((parameter as DataGridCellInfo).Cell);
        }
    }
}