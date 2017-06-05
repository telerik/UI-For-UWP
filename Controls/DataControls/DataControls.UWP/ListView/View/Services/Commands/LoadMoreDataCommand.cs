using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data.ListView.Commands
{
    /// <summary>
    /// Command that is executed when more data is requested by the <see cref="RadListView"/> control.
    /// </summary>
    public class LoadMoreDataCommand : ListViewCommand
    {
        /// <inheritdoc/>
        public override bool CanExecute(object parameter)
        {
            bool canExecute = this.Owner != null && parameter is LoadMoreDataContext;
            if (this.Owner != null)
            {
                canExecute = canExecute && this.Owner.ItemsSource is ISupportIncrementalLoading;
            }

            return canExecute;
        }

        /// <inheritdoc/>
        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            var context = parameter as LoadMoreDataContext;
            this.Owner.Model.UpdateRequestedItems(context.BatchSize, true);
        }
    }
}
