using System;
using System.Linq;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    internal class BeginEditCommand : DataGridCommand
    {
        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        public override bool CanExecute(object parameter)
        {
            var context = parameter as EditContext;
            return context != null && context.CellInfo != null && this.Owner.UserEditMode != DataGridUserEditMode.None && this.Owner.Columns.Any(c => c.CanEdit);
        }

        /// <summary>
        /// Performs the core action given the provided parameter.
        /// </summary>
        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            var context = parameter as EditContext;
            context.IsSuccessful = this.Owner.editService.BeginEdit(context.CellInfo.RowItemInfo);
        }
    }
}