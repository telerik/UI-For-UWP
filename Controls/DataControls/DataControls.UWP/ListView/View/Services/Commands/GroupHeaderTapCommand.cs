using Telerik.UI.Xaml.Controls.Data.ListView.Primitives;

namespace Telerik.UI.Xaml.Controls.Data.ListView.Commands
{
    /// <summary>
    /// Represents a command that is executed when the user taps on a <see cref="Telerik.UI.Xaml.Controls.Data.ListView.Primitives.ListViewGroupHeader"/>.
    /// </summary>
    public class GroupHeaderTapCommand : ListViewCommand
    {
        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        public override bool CanExecute(object parameter)
        {
            return parameter is GroupHeaderContext;
        }

        /// <summary>
        /// Performs the core action given the provided parameter.
        /// </summary>
        public override void Execute(object parameter)
        {
            var context = (GroupHeaderContext)parameter;

            context.IsExpanded = !context.IsExpanded;
        }
    }
}
