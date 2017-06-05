namespace Telerik.UI.Xaml.Controls.Data.ListView.Commands
{
    /// <summary>
    /// Command that is executed when a <see cref="RadListViewItem"/> element is being swiped.
    /// </summary>
    public class ItemSwipingCommand : ListViewCommand
    {
        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        public override bool CanExecute(object parameter)
        {
            return parameter is ItemSwipingContext;
        }
    }
}
