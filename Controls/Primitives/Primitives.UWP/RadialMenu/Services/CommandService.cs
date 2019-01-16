using System;
using System.Linq;
using Telerik.UI.Xaml.Controls.Primitives.Menu.Commands;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    /// <summary>
    /// Encapsulates the command-related routine within a <see cref="RadRadialMenu"/> instance.
    /// </summary>
    public class CommandService : CommandServiceBase<RadRadialMenu>
    {
        internal CommandService(RadRadialMenu owner)
            : base(owner)
        {
            this.InitKnownCommands();
        }

        internal CommandCollection<RadRadialMenu> UserCommands
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
            this.defaultCommands[(int)CommandId.Open] = this.CreateKnownCommand(CommandId.Open);
            this.defaultCommands[(int)CommandId.Close] = this.CreateKnownCommand(CommandId.Close);
            this.defaultCommands[(int)CommandId.NavigateToView] = this.CreateKnownCommand(CommandId.NavigateToView);
            this.defaultCommands[(int)CommandId.NavigateBack] = this.CreateKnownCommand(CommandId.NavigateBack);
        }

        private RadialMenuCommand CreateKnownCommand(CommandId id)
        {
            RadialMenuCommand command = null;

            switch (id)
            {
                case CommandId.Open:
                    command = new OpenMenuCommand();
                    break;
                case CommandId.Close:
                    command = new CloseMenuCommand();
                    break;
                case CommandId.NavigateToView:
                    command = new NavigateCommand();
                    break;
                case CommandId.NavigateBack:
                    command = new NavigateBackCommand();
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
