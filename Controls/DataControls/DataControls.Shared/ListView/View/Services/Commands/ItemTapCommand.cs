namespace Telerik.UI.Xaml.Controls.Data.ListView.Commands
{
    /// <summary>
    /// Represents a command that is executed when the user taps on a <see cref="RadListViewItem"/>.
    /// </summary>
    public class ItemTapCommand : ListViewCommand
    {
        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        public override bool CanExecute(object parameter)
        {
            return parameter is ItemTapContext;
        }

        /// <summary>
        /// Performs the core action given the provided parameter.
        /// </summary>
        public override void Execute(object parameter)
        {
            if (this.Owner.isActionContentDisplayed)
            {
                this.Owner.RebuildUI();
                return;
            }

            base.Execute(parameter);

            this.Owner.OnItemTap(parameter as ItemTapContext);
        }
    }
}