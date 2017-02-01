namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    internal class ValidateCellCommand : DataGridCommand
    {
        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        public override bool CanExecute(object parameter)
        {
            var context = parameter as ValidateCellContext;
            return context != null && context.CellInfo != null && context.Errors != null;
        }
    }
}