using System.Windows.Input;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Input.Calendar.Commands
{
    /// <summary>
    /// Represents a predefined command that aggregates a custom ICommand implementation, which may be used to perform additional action over the default implementation.
    /// </summary>
    public sealed class CalendarUserCommand : CalendarCommand
    {
        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(CalendarUserCommand), new PropertyMetadata(null, OnCommandChanged));

        /// <summary>
        /// Identifies the <see cref="EnableDefaultCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableDefaultCommandProperty =
            DependencyProperty.Register(nameof(EnableDefaultCommand), typeof(bool), typeof(CalendarUserCommand), new PropertyMetadata(true, OnEnableDefaultCommandChanged));

        private ICommand commandCache;
        private bool enableDefaultCommandCache = true;

        /// <summary>
        /// Gets or sets the <see cref="ICommand"/> implementation that is used to perform the custom logic associated with this command.
        /// </summary>
        public ICommand Command
        {
            get
            {
                return this.commandCache;
            }
            set
            {
                this.SetValue(CommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the default command implementation, related to the current Id, should be executed.
        /// </summary>
        /// <value>
        /// The default value is <c>True</c>.
        /// </value>
        public bool EnableDefaultCommand
        {
            get
            {
                return this.enableDefaultCommandCache;
            }
            set
            {
                this.SetValue(EnableDefaultCommandProperty, value);
            }
        }

        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns>Boolean value that specifies whether the command can be executed.</returns>
        public override bool CanExecute(object parameter)
        {
            if (this.Owner == null)
            {
                return false;
            }

            if (this.commandCache != null)
            {
                return this.commandCache.CanExecute(parameter);
            }

            return this.Owner.CommandService.CanExecuteDefaultCommand((CommandId)this.CommandId, parameter);
        }

        /// <summary>
        /// Performs the core action given the provided parameter.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        public override void Execute(object parameter)
        {
            if (this.enableDefaultCommandCache)
            {
                this.Owner.CommandService.ExecuteDefaultCommand((CommandId)this.CommandId, parameter);
            }

            if (this.commandCache != null)
            {
                this.commandCache.Execute(parameter);
            }
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var command = d as CalendarUserCommand;
            command.commandCache = e.NewValue as ICommand;
        }

        private static void OnEnableDefaultCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var command = d as CalendarUserCommand;
            command.enableDefaultCommandCache = (bool)e.NewValue;
        }
    }
}
