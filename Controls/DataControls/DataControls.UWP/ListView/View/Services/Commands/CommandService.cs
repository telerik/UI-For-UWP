using System;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Data.ListView.Commands
{
    internal interface ICommandService
    {
        CommandService CommandService
        {
            get;
        }
    }

    /// <summary>
    /// Encapsulates the command-related routine within a <see cref="RadListView"/> instance.
    /// </summary>
    public class CommandService : CommandServiceBase<RadListView>
    {
        internal CommandService(RadListView owner) : base(owner)
        {
            this.InitKnownCommands();
            this.userCommands.CollectionChanged += this.CommandsCollectionChanged;
        }

        /// <summary>
        /// Gets the collection with all the user commands registered with the service. User commands have higher priority than the built-in (default) ones.
        /// </summary>
        internal CommandCollection<RadListView> UserCommands
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
                foreach (ListViewCommand newCommand in e.NewItems)
                {
                    if (newCommand != null && newCommand.Id == CommandId.LoadMoreData)
                    {
                        newCommand.CanExecuteChanged += this.LoadMoreDataCanExecuteChanged;
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (ListViewCommand oldCommand in e.OldItems)
                {
                    if (oldCommand != null && oldCommand.Id == CommandId.LoadMoreData)
                    {
                        oldCommand.CanExecuteChanged -= this.LoadMoreDataCanExecuteChanged;
                    }
                }
            }
        }

        private void LoadMoreDataCanExecuteChanged(object sender, EventArgs e)
        {
            this.Owner.updateService.RegisterUpdate((int)UpdateFlags.AffectsData);
        }

        private ListViewCommand CreateKnownCommand(CommandId id)
        {
            ListViewCommand command = null;

            switch (id)
            {
                case CommandId.LoadMoreData:
                    command = new LoadMoreDataCommand();
                    break;

                case CommandId.ItemTap:
                    command = new ItemTapCommand();
                    break;

                case CommandId.RefreshRequested:
                    command = new RefreshRequestedCommand();
                    break;

                case CommandId.ItemDragStarting:
                    command = new ItemDragStartingCommand();
                    break;

                case CommandId.ItemReorderComplete:
                    command = new ItemReorderCompleteCommand();
                    break;

                case CommandId.ItemSwiping:
                    command = new ItemSwipingCommand();
                    break;

                case CommandId.ItemSwipeActionComplete:
                    command = new ItemSwipeActionCompleteCommand();
                    break;

                case CommandId.ItemActionTap:
                    command = new ItemActionTapCommand();
                    break;

                case CommandId.ItemHold:
                    command = new ItemHoldCommand();
                    break;

                case CommandId.GroupHeaderTap:
                    command = new GroupHeaderTapCommand();
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
