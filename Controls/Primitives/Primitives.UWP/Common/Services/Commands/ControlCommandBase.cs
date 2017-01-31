using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a command abstraction that is associated with a particular <see cref="ICommand"/> instance.
    /// </summary>
    public abstract class ControlCommandBase<T> : AttachableObject<T>, ICommand where T : RadControl
    {
        /// <summary>
        /// Occurs when the CanExecute state of the command changes.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        internal abstract int CommandId
        {
            get;
        }

        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        /// <value>
        /// The Default value is <c>False</c>.
        /// </value>
        /// <param name="parameter">The command parameter.</param>
        /// <returns>Value that specifies whether the command can be executed.</returns>
        public virtual bool CanExecute(object parameter)
        {
            return false;
        }

        /// <summary>
        /// Performs the core action given the provided parameter.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        public virtual void Execute(object parameter)
        {
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            var eh = this.CanExecuteChanged;
            if (eh != null)
            {
                eh(this, e);
            }
        }
    }
}
