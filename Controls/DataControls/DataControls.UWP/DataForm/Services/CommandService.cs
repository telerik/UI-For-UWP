using System;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Data.DataForm.Commands
{
    /// <summary>
    /// Encapsulates the command-related routine within a <see cref="RadDataForm"/> instance.
    /// </summary>
    public class CommandService : CommandServiceBase<RadDataForm>
    {
        internal CommandService(RadDataForm owner)
            : base(owner)
        {
            this.InitKnownCommands();
        }

        internal CommandCollection<RadDataForm> UserCommands
        {
            get
            {
                return this.userCommands;
            }
        }

        /// <summary>
        /// Attempts to find the command, associated with the specified Id and to perform its Execute routine, using the provided parameter.
        /// </summary>
        /// <param name="id">The <see cref="CommandId"/> value to look for.</param>
        /// <param name="parameter">The parameter that is passed to the CanExecute and Execute methods of the command.</param>
        /// <returns>True if the command is successfully executed, false otherwise.</returns>
        public bool ExecuteCommand(CommandId id, object parameter)
        {
            return this.ExecuteCommandCore((int)id, parameter, true);
        }

        /// <summary>
        /// Executes the default (built-in) command (without looking for user-defined commands), associated with the specified Id.
        /// </summary>
        /// <param name="id">The <see cref="CommandId"/> value to look for.</param>
        /// <param name="parameter">The parameter that is passed to the CanExecute and Execute methods of the command.</param>
        /// <returns>True if the command is successfully executed, false otherwise.</returns>
        public bool ExecuteDefaultCommand(CommandId id, object parameter)
        {
            return this.ExecuteCommandCore((int)id, parameter, false);
        }

        /// <summary>
        /// Determines whether the default command, associated with the specified Id can be executed given the parameter provided.
        /// </summary>
        /// <param name="id">The <see cref="CommandId"/> value to look for.</param>
        /// <param name="parameter">The parameter that is passed to the CanExecute and Execute methods of the command.</param>
        /// <returns>True if the command can be executed, false otherwise.</returns>
        public bool CanExecuteDefaultCommand(CommandId id, object parameter)
        {
            var command = this.GetCommandById((int)id, false);
            if (command != null)
            {
                return command.CanExecute(parameter);
            }

            return false;
        }

        private void InitKnownCommands()
        {
            this.defaultCommands[(int)CommandId.Commit] = this.CreateKnownCommand(CommandId.Commit);
            this.defaultCommands[(int)CommandId.Validate] = this.CreateKnownCommand(CommandId.Validate);
        }

        private DataFormCommand CreateKnownCommand(CommandId id)
        {
            DataFormCommand command = null;

            switch (id)
            {
                case CommandId.Commit:
                    command = new CommitCommand();
                    break;

                case CommandId.Validate:
                    command = new ValidateCommand();
                    break;

                default:
                    throw new ArgumentException("Unknown command id!", "id");
            }

            command.Id = id;
            command.Owner = this.Owner;

            return command;
        }
    }
}
