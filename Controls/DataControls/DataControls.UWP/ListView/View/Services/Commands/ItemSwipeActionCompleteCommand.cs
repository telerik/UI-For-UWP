namespace Telerik.UI.Xaml.Controls.Data.ListView.Commands
{
    /// <summary>
    /// Command that is executed when the swiping action of a <see cref="RadListViewItem"/> element has ended.
    /// </summary>
    public class ItemSwipeActionCompleteCommand : ListViewCommand
    {
        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        public override bool CanExecute(object parameter)
        {
            return parameter is ItemSwipeActionCompleteContext;
        }

        /// <summary>
        /// Performs the core action given the provided parameter.
        /// </summary>
        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            var context = parameter as ItemSwipeActionCompleteContext;
            context.Container.OnDragComplete(context);
        }
    }
}
