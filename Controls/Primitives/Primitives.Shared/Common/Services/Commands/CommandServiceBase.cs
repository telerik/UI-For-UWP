using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a strongly-typed class that provides basic infrastructure for <see cref="RadControl"/> commands.
    /// </summary>
    public abstract class CommandServiceBase<T> : ServiceBase<T> where T : RadControl
    {
        internal Dictionary<int, ICommand> defaultCommands;
        internal CommandCollection<T> userCommands;

        internal CommandServiceBase(T owner)
            : base(owner)
        {
            this.defaultCommands = new Dictionary<int, ICommand>();
            this.userCommands = new CommandCollection<T>(owner);
        }

        internal bool ExecuteCommandCore(int id, object parameter, bool searchUser)
        {
            if (this.Owner == null)
            {
                return false;
            }

            var command = this.GetCommandById(id, searchUser);
            if (command != null && command.CanExecute(parameter))
            {
                command.Execute(parameter);
                return true;
            }

            return false;
        }

        internal ICommand GetCommandById(int id, bool searchUser)
        {
            ICommand command;
            if (searchUser)
            {
                command = this.FindUserCommandById(id);
                if (command != null)
                {
                    return command;
                }
            }

            if (this.defaultCommands.TryGetValue(id, out command))
            {
                return command;
            }

            return null;
        }

        internal ICommand FindUserCommandById(int id)
        {
            foreach (var command in this.userCommands)
            {
                if (command.CommandId == id)
                {
                    return command;
                }
            }

            return null;
        }
    }
}
