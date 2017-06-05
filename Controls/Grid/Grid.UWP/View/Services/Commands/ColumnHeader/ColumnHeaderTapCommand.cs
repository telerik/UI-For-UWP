using System.Diagnostics;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    internal class ColumnHeaderTapCommand : DataGridCommand
    {
        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        public override bool CanExecute(object parameter)
        {
            if (this.Owner == null)
            {
                return false;
            }

            var context = parameter as ColumnHeaderTapContext;
            if (context == null)
            {
                return false;
            }

            if (Owner.ColumnDataOperationsMode == ColumnDataOperationsMode.Inline)
            {
                return context.CanSort;
            }

            return true;
        }

        /// <summary>
        /// Performs the core action given the provided parameter.
        /// </summary>
        public override void Execute(object parameter)
        {
            ColumnHeaderTapContext context = parameter as ColumnHeaderTapContext;
            if (context == null)
            {
                Debug.Assert(false, "Parameter must be a valid ColumnDefinition");
                return;
            }

            // check whether the data layer is prepared
            if (!this.Owner.Model.IsDataProviderUpdating)
            {
                // delegate the actual implementation to the specific columns
                if (this.Owner.ColumnDataOperationsMode == ColumnDataOperationsMode.Inline)
                {
                    context.Column.ToggleSort(context.IsMultipleSortAllowed);

                    this.Owner.ContentFlyout.Hide(DataGridFlyoutId.All);
                }
                else if (this.Owner.ColumnDataOperationsMode == ColumnDataOperationsMode.Flyout)
                {
                    var popup = this.Owner.ContentFlyout;

                    if (popup != null)
                    {
                        var header = context.Column.HeaderControl;

                        // If the user taps on the same column header consequently, the flyout is closed rahter than opened again.
                        if (context.IsFlyoutOpen && popup.Id == DataGridFlyoutId.ColumnHeader && popup.Child != null)
                        {
                            var currentContext = popup.Child.DataContext as ActionContext;
                            if (currentContext != null && currentContext.Column.HeaderControl.Equals(header))
                            {
                                return;
                            }
                        }

                        if (header != null)
                        {
                            this.Owner.UpdateSelectedHeader(header, true);
                        }

                        FrameworkElement popupContent = null;

                        if (context.Column.DataOperationsFlyoutTemplate != null)
                        {
                            popupContent = context.Column.DataOperationsFlyoutTemplate.LoadContent() as FrameworkElement;
                        }
                        else
                        {
                            popupContent = new DataGridDataOperationsControl();
                        }

                        var actionContext = new ActionContext(context.Column, new ColumnHeaderFlyoutCommand(context.Column.HeaderControl));
                        popupContent.DataContext = actionContext;

                        this.Owner.ContentFlyout.Show(DataGridFlyoutId.ColumnHeader, popupContent);
                    }
                }
            }
        }
    }
}