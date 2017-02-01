using System;
using System.Collections.Generic;
using System.Windows.Input;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Telerik.UI.Xaml.Controls.Input.Calendar.Commands;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Input.Calendar.Commands
{
    /// <summary>
    /// Encapsulates the command-related routine within a <see cref="RadCalendar"/> instance.
    /// </summary>
    //// TODO: Consider extracting the command service implementations from Grid & Calendar to Core.
    public class CommandService : CommandServiceBase<RadCalendar>
    {
        internal CommandService(RadCalendar owner)
            : base(owner)
        {
            this.InitKnownCommands();
        }

        internal CommandCollection<RadCalendar> UserCommands
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
            this.defaultCommands[(int)CommandId.MoveToDate] = this.CreateKnownCommand(CommandId.MoveToDate);
            this.defaultCommands[(int)CommandId.MoveToPreviousView] = this.CreateKnownCommand(CommandId.MoveToPreviousView);
            this.defaultCommands[(int)CommandId.MoveToNextView] = this.CreateKnownCommand(CommandId.MoveToNextView);
            this.defaultCommands[(int)CommandId.MoveToUpperView] = this.CreateKnownCommand(CommandId.MoveToUpperView);
            this.defaultCommands[(int)CommandId.MoveToLowerView] = this.CreateKnownCommand(CommandId.MoveToLowerView);
            this.defaultCommands[(int)CommandId.CellPointerOver] = this.CreateKnownCommand(CommandId.CellPointerOver);
            this.defaultCommands[(int)CommandId.CellTap] = this.CreateKnownCommand(CommandId.CellTap);
        }

        private CalendarCommand CreateKnownCommand(CommandId id)
        {
            CalendarCommand command = null;

            switch (id)
            {
                case CommandId.MoveToDate:
                    command = new MoveToDateCommand();
                    break;
                case CommandId.MoveToPreviousView:
                    command = new MoveToPreviousViewCommand();
                    break;
                case CommandId.MoveToNextView:
                    command = new MoveToNextViewCommand();
                    break;
                case CommandId.MoveToUpperView:
                    command = new MoveToUpperViewCommand();
                    break;
                case CommandId.MoveToLowerView:
                    command = new MoveToLowerViewCommand();
                    break;
                case CommandId.CellPointerOver:
                    command = new CellPointerOverCommand();
                    break;
                case CommandId.CellTap:
                    command = new CellTapCommand();
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
