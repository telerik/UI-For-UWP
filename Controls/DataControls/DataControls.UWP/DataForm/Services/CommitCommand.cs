using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Data.DataForm.Commands
{
    internal class CommitCommand : DataFormCommand
    {
        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        public override bool CanExecute(object parameter)
        {
            return this.Owner != null && parameter is EntityProperty;
        }

        /// <summary>
        /// Performs the core action given the provided parameter.
        /// </summary>
        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            this.Owner.Model.TransactionService.CommitPropertyCore(parameter as EntityProperty);
        }
    }
}
