using System;
using System.Windows.Input;
using Telerik.UI.Xaml.Controls.Grid.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    public class ColumnHeaderFlyoutCommand : ICommand
    {
        public DataGridColumnHeader ColumnHeader { get; set; }
        public bool IsGrouped { get; set; }

        public ColumnHeaderFlyoutCommand(DataGridColumnHeader owner)
        {
            this.ColumnHeader = owner;
            this.IsGrouped = owner.Column.CanGroupBy;
        }

        public bool CanExecute(object parameter)
        {
            return parameter != null;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
           this.ColumnHeader.Owner.CommandService.ExecuteDefaultCommand(Commands.CommandId.ColumnHeaderAction, new ColumnHeaderActionContext(parameter.ToString(), this.ColumnHeader));
        }
    }
}
