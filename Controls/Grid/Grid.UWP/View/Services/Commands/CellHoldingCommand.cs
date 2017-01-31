using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    internal class CellHoldingCommand : DataGridCommand
    {
        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        public override bool CanExecute(object parameter)
        {
            return parameter is CellHoldingContext;
        }

        /// <summary>
        /// Performs the core action given the provided parameter.
        /// </summary>
        public override void Execute(object parameter)
        {                  
            base.Execute(parameter);

            var context = parameter as CellHoldingContext;

            this.Owner.OnCellHolding(context.CellInfo, context.HoldingState);
        }
    }
}
