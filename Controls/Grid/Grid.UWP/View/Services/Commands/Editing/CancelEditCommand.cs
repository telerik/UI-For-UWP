using System;
using System.Linq;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    internal class CancelEditCommand : DataGridCommand
    {
        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        public override bool CanExecute(object parameter)
        {
            var context = parameter as EditContext;
            return context != null;
        }

        /// <summary>
        /// Performs the core action given the provided parameter.
        /// </summary>
        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            var context = parameter as EditContext;
            context.IsSuccessful = this.Owner.editService.CancelEdit(context.TriggerAction);
        }
    }
}