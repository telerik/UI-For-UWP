using System;
using System.Windows.Input;
using Telerik.UI.Xaml.Controls.Grid.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    /// <summary>
    /// Represents a command that executes when the Header is Tapped.
    /// </summary>
    public class ColumnHeaderFlyoutCommand : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnHeaderFlyoutCommand"/> class.
        /// </summary>
        public ColumnHeaderFlyoutCommand(DataGridColumnHeader owner)
        {
            this.ColumnHeader = owner;
            this.IsGrouped = owner.Column.CanGroupBy;
        }
        
#pragma warning disable 0067
        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

        /// <summary>
        /// Gets or sets the Tapped Header of the DataGrid.
        /// </summary>
        public DataGridColumnHeader ColumnHeader { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the grouping of the Column.
        /// </summary>
        public bool IsGrouped { get; set; }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// The parameter used by the command.
        /// </param>
        /// <returns>
        /// Returns a value indicating whether this command can be executed.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return parameter != null;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        /// The parameter used by the command.
        /// </param>
        public void Execute(object parameter)
        {
           this.ColumnHeader.Owner.CommandService.ExecuteDefaultCommand(Commands.CommandId.ColumnHeaderAction, new ColumnHeaderActionContext(parameter.ToString(), this.ColumnHeader));
        }
    }
}
