using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.SideDrawer.Commands
{
     /// <summary>
    /// Encapsulates the command-related routine within a <see cref="RadSideDrawer"/> instance.
    /// </summary>
    public class CommandService : CommandServiceBase<RadSideDrawer>
    {
        internal CommandService(RadSideDrawer owner)
            : base(owner)
        {
            this.InitKnownCommands();
        }

        internal CommandCollection<RadSideDrawer> UserCommands
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
            this.defaultCommands[(int)CommandId.DrawerStateChanged] = this.CreateKnownCommand(CommandId.DrawerStateChanged);
            this.defaultCommands[(int)CommandId.GenerateAnimations] = this.CreateKnownCommand(CommandId.GenerateAnimations);
            this.defaultCommands[(int)CommandId.KeyDown] = this.CreateKnownCommand(CommandId.KeyDown);
        }

        private SideDrawerCommand CreateKnownCommand(CommandId id)
        {
            SideDrawerCommand command = null;

            switch (id)
            {
                case CommandId.DrawerStateChanged:
                    command = new DrawerStateCommand();
                    break;
                case CommandId.GenerateAnimations:
                    command = new GenerateAnimationsCommand();
                    break;
                case CommandId.KeyDown:
                    command = new SideDrawerKeyDownCommand();
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
