namespace Telerik.UI.Xaml.Controls.Data.ListView.Commands
{
    /// <summary>
    /// Command that is invoked when Pull to refresh is initiated.
    /// </summary>
    public class RefreshRequestedCommand : ListViewCommand
    {
        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        public override bool CanExecute(object parameter)
        {
            return this.Owner != null;
        }

        /// <summary>
        /// Performs the core action given the provided parameter.
        /// </summary>
        public override void Execute(object parameter)
        {
            base.Execute(parameter);
            this.Owner.OnRefreshRequested(parameter);
        }
    }
}
