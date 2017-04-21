using System;
using System.Windows.Input;
using Telerik.UI.Xaml.Controls.Grid.Commands;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a command that can perform a given action.
    /// </summary>
    public class ExternalEditorActionCommand : ICommand
    {
        private ExternalEditorCommandId Id { get; set; }

        /// <summary>
        /// The owner of the Command.
        /// </summary>
        public IGridExternalEditor Owner { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalEditorActionCommand"/> class.
        /// </summary>
        public ExternalEditorActionCommand(IGridExternalEditor owner, ExternalEditorCommandId id)
        {
            this.Owner = owner;
            this.Id = id;
        }

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
            return this.Owner != null;
        }

#pragma warning disable 0067
        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        /// The parameter used by the command.
        /// </param>
        public void Execute(object parameter)
        {
            if (this.Owner == null)
            {
                return;
            }

            if (this.Id == ExternalEditorCommandId.Save)
            {
                this.Owner.CommitEdit();
            }
            else if (this.Id == ExternalEditorCommandId.Cancel)
            {
                this.Owner.CancelEdit();
            }
        }
    }
}
