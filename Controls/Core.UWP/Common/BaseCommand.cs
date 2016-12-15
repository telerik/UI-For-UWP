using System;
using System.Windows.Input;

namespace Telerik.Core
{
    /// <summary>
    /// Base implementation of the <see cref="ICommand"/> interface.
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        /// <summary>
        /// Occurs when the CanExecute state of the command changes.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>A value that indicates whether the command can be executed.</returns>
        public virtual bool CanExecute(object parameter)
        {
            return false;
        }

        /// <summary>
        /// Performs the core action associated with the command, using the provided parameter.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        public void Execute(object parameter)
        {
            if (!this.CanExecute(parameter))
            {
                // TODO: Should we raise an exception or should we fails silently?
                throw new InvalidOperationException("The command may not be current executed.");
            }

            this.ExecuteCore(parameter);
        }

        /// <summary>
        /// Performs the core action associated with this command.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        protected virtual void ExecuteCore(object parameter)
        {
        }

        /// <summary>
        /// Raises the <see cref="E:CanExecuteChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
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