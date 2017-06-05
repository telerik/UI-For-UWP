using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    /// <summary>
    /// Encapsulates the command-related routine within a <see cref="RadDataGrid"/> instance.
    /// </summary>
    public class CommandService : CommandServiceBase<RadDataGrid>
    {
        internal CommandService(RadDataGrid owner)
            : base(owner)
        {
            this.InitKnownCommands();
            this.userCommands.CollectionChanged += this.CommandsCollectionChanged;
        }

        /// <summary>
        /// Gets the collection with all the user commands registered with the service. User commands have higher priority than the built-in (default) ones.
        /// </summary>
        internal CommandCollection<RadDataGrid> UserCommands
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
        /// Determines whether the command, associated with the specified Id can be executed given the parameter provided.
        /// </summary>
        /// <param name="id">The <see cref="CommandId"/> value to look for.</param>
        /// <param name="parameter">The parameter that is passed to the CanExecute and Execute methods of the command.</param>
        /// <returns>True if the command can be executed, false otherwise.</returns>
        public bool CanExecuteCommand(CommandId id, object parameter)
        {
            var command = this.GetCommandById((int)id, true);
            if (command != null)
            {
                return command.CanExecute(parameter);
            }

            return false;
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

        private void LoadMoreDataCanExecuteChanged(object sender, EventArgs e)
        {
            this.Owner.updateService.RegisterUpdate((int)UpdateFlags.AffectsData);
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

        private void CommandsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (DataGridCommand newCommand in e.NewItems)
                {
                    if (newCommand != null && newCommand.Id == CommandId.LoadMoreData)
                    {
                        newCommand.CanExecuteChanged += this.LoadMoreDataCanExecuteChanged;
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (DataGridCommand oldCommand in e.OldItems)
                {
                    if (oldCommand != null && oldCommand.Id == CommandId.LoadMoreData)
                    {
                        oldCommand.CanExecuteChanged -= this.LoadMoreDataCanExecuteChanged;
                    }
                }
            }
        }

        private DataGridCommand CreateKnownCommand(CommandId id)
        {
            DataGridCommand command = null;

            switch (id)
            {
                case CommandId.ColumnHeaderTap:
                    command = new ColumnHeaderTapCommand();
                    break;
                case CommandId.CellTap:
                    command = new CellTapCommand();
                    break;
                case CommandId.CellPointerOver:
                    command = new CellPointerOverCommand();
                    break;
                case CommandId.GenerateColumn:
                    command = new GenerateColumnCommand();
                    break;
                case CommandId.GroupHeaderTap:
                    command = new GroupHeaderTapCommand();
                    break;
                case CommandId.FlyoutGroupHeaderTap:
                    command = new FlyoutGroupHeaderTapCommand();
                    break;
                case CommandId.FilterButtonTap:
                    command = new FilterButtonTapCommand();
                    break;
                case CommandId.FilterRequested:
                    command = new FilterRequestedCommand();
                    break;
                case CommandId.DataBindingComplete:
                    command = new DataBindingCompleteCommand();
                    break;
                case CommandId.CellDoubleTap:
                    command = new CellDoubleTapCommand();
                    break;
                case CommandId.BeginEdit:
                    command = new BeginEditCommand();
                    break;
                case CommandId.CommitEdit:
                    command = new CommitEditCommand();
                    break;
                case CommandId.CancelEdit:
                    command = new CancelEditCommand();
                    break;
                case CommandId.ValidateCell:
                    command = new ValidateCellCommand();
                    break;
                case CommandId.LoadMoreData:
                    command = new LoadMoreDataCommand();
                    break;
                case CommandId.KeyDown:
                    command = new KeyDownCommand();
                    break;
                case CommandId.ColumnHeaderAction:
                    command = new ColumnHeaderActionCommand();
                    break;
                case CommandId.CellHolding:
                    command = new CellHoldingCommand();
                    break;
                case CommandId.CellFlyoutAction:
                    command = new CellFlyoutActionCommand();
                    break;
                case CommandId.ToggleColumnVisibility:
                    command = new ToggleColumnVisibilityCommand();
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
