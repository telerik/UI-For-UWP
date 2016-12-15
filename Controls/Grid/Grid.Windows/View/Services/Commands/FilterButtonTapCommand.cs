using Telerik.UI.Xaml.Controls.Grid.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    internal class FilterButtonTapCommand : DataGridCommand
    {
        public override bool CanExecute(object parameter)
        {
            return this.Owner != null && parameter is FilterButtonTapContext;
        }

        public override void Execute(object parameter)
        {
            var context = parameter as FilterButtonTapContext;

            var displayMode = FilteringFlyoutDisplayMode.Inline;
            if (this.Owner.ColumnDataOperationsMode == ColumnDataOperationsMode.Flyout)
            {
                if (this.Owner.ActualWidth <= 640)
                {
                    displayMode = FilteringFlyoutDisplayMode.Fill;
                }
                else
                {
                    displayMode = FilteringFlyoutDisplayMode.Flyout;
                }
            }

            var filteringFlyout = new DataGridFilteringFlyout { DisplayMode = displayMode };
            filteringFlyout.Initialize(this.Owner, context);

            var id = displayMode == FilteringFlyoutDisplayMode.Inline ? DataGridFlyoutId.FilterButton : DataGridFlyoutId.FlyoutFilterButton;
            this.Owner.ContentFlyout.Show(id, filteringFlyout);

            this.Owner.CancelEdit();
        }
    }
}