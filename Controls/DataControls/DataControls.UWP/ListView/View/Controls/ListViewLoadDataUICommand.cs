using System;
using System.Windows.Input;
using Telerik.UI.Xaml.Controls.Data.ListView.Commands;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// Represents a command that can perform a given action.
    /// </summary>
    public class ListViewLoadDataUICommand : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewLoadDataUICommand"/> class.
        /// </summary>
        public ListViewLoadDataUICommand(ListViewLoadDataControl owner)
        {
            this.LoadDataControl = owner;
        }

#pragma warning disable 0067

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

#pragma warning restore 0067

        /// <summary>
        /// Gets or sets the <see cref="ListViewLoadDataControl"/>.
        /// </summary>
        public ListViewLoadDataControl LoadDataControl { get; set; }

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
            return true;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        /// The parameter used by the command.
        /// </param>
        public void Execute(object parameter)
        {
            this.LoadDataControl.Owner.CommandService.ExecuteCommand(Commands.CommandId.LoadMoreData, new LoadMoreDataContext());
        }
    }
}
