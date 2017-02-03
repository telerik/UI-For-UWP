using System;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    /// <summary>
    /// Represents a predefined command that aggregates a custom ICommand implementation, which may be used to perform additional action over the default implementation.
    /// </summary>
    public sealed class DataGridUserCommand : DataGridCommand
    {
        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(DataGridUserCommand), new PropertyMetadata(null, OnCommandChanged));

        /// <summary>
        /// Identifies the <see cref="EnableDefaultCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableDefaultCommandProperty =
            DependencyProperty.Register(nameof(EnableDefaultCommand), typeof(bool), typeof(DataGridUserCommand), new PropertyMetadata(true, OnEnableDefaultCommandChanged));

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
        /// Gets or sets a value indicating whether the default command implementation, related to the current Id, should be executed. True by default.
        /// </summary>
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

            return this.Owner.commandService.CanExecuteDefaultCommand(this.Id, parameter);
        }

        /// <summary>
        /// Performs the core action given the provided parameter.
        /// </summary>
        public override void Execute(object parameter)
        {
            if (this.enableDefaultCommandCache)
            {
                this.Owner.commandService.ExecuteDefaultCommand(this.Id, parameter);
            }

            if (this.commandCache != null)
            {
                this.commandCache.Execute(parameter);
            }
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var command = d as DataGridUserCommand;
            command.commandCache = e.NewValue as ICommand;
        }

        private static void OnEnableDefaultCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var command = d as DataGridUserCommand;
            command.enableDefaultCommandCache = (bool)e.NewValue;
        }
    }
}