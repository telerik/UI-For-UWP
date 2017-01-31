using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    internal class LoadMoreDataCommand : DataGridCommand
    {
        public override bool CanExecute(object parameter)
        {
            bool canExecute = this.Owner != null && this.Owner.ItemsSource is ISupportIncrementalLoading && parameter is LoadMoreDataContext;

            return canExecute;
        }

        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            var context = parameter as LoadMoreDataContext;
            this.Owner.Model.UpdateRequestedItems(context.BatchSize, true);
        }
    }
}
