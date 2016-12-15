using System;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Encapsulates the predefined commands in a <see cref="RadMap"/> instance. Exposes public APIs allowing for command execution.
    /// </summary>
    public class MapCommandService : CommandServiceBase<RadMap>
    {
        internal MapCommandService(RadMap owner)
            : base(owner)
        {
            this.InitKnownCommands();
        }

        /// <summary>
        /// Gets the collection with all the user commands registered with the service. User commands have higher priority than the built-in (default) ones.
        /// </summary>
        internal CommandCollection<RadMap> UserCommands
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
            var commands = Enum.GetValues(typeof(CommandId));

            foreach (var id in commands)
            {
                var commandId = (CommandId)id;

                if (commandId != CommandId.Unknown)
                {
                    this.defaultCommands[(int)commandId] = this.CreateKnownCommand(commandId);
                }
            }
        }

        private MapCommand CreateKnownCommand(CommandId id)
        {
            MapCommand command = null;

            switch (id)
            {
                case CommandId.ViewChanged:
                    command = new ViewChangedCommand();
                    break;
                case CommandId.ShapeSelectionChanged:
                    command = new ShapeSelectionChangedCommand();
                    break;
                case CommandId.ShapeLayerSourceChanged:
                    command = new ShapeLayerSourceChangedCommand();
                    break;
                default:
                    throw new ArgumentException("Unkown command id!", "id");
            }

            command.Id = id;
            command.Owner = this.Owner;

            return command;
        }
    }
}
