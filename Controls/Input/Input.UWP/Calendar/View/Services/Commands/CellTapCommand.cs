using Telerik.UI.Xaml.Controls.Primitives;
using Windows.System;

namespace Telerik.UI.Xaml.Controls.Input.Calendar.Commands
{
    internal class CellTapCommand : CalendarCommand
    {
        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        public override bool CanExecute(object parameter)
        {
            return parameter is CalendarCellTapContext;
        }

        /// <summary>
        /// Performs the core action given the provided parameter.
        /// </summary>
        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            CalendarCellTapContext context = parameter as CalendarCellTapContext;
            CalendarCellModel cellModel = context.CellModel;

            if (!cellModel.IsBlackout)
            {
                if (this.Owner.DisplayMode == CalendarDisplayMode.MonthView)
                {
                    bool appendToSelection = false;
                    if (this.Owner.SelectionMode == CalendarSelectionMode.Multiple)
                    {
                        appendToSelection = KeyboardHelper.IsModifierKeyDown(VirtualKey.Control);
                    }

                    // If there is only one cell selected ,check if tap is on the same cell (prevent SelectionChanged for the same single cell).
                    if (!appendToSelection && !this.Owner.SelectionService.ShouldDeselectCell(cellModel))
                    {
                        this.Owner.SelectionService.ClearSelection(false);
                    }

                    // If cell is selected do not trigger selection again (only Clear and Navigation)
                    if (!cellModel.IsSelected)
                    {
                        // SelectedDateRanges should not be cleared if Control key is down.
                        if (appendToSelection)
                        {
                            this.Owner.SelectionService.Select(new CalendarDateRange(cellModel.Date, cellModel.Date));
                        }
                        else
                        {
                            this.Owner.SelectionService.SelectCell(cellModel);
                        }
                    }

                    this.Owner.RaiseMoveToDateCommand(cellModel.Date);
                }
                else
                {
                    this.Owner.RaiseMoveToLowerCommand(cellModel.Date);
                }
            }

            if (this.Owner.CurrentDate != cellModel.Date)
            {
                this.Owner.CurrencyService.CurrentDate = cellModel.Date;
                this.Owner.FireAutomationFocusEvent();
            }
            else
            {
                // CurrentDate is not changed but we need to update the visuals on view change.
                this.Owner.CurrencyService.AsyncUpdateCurrencyPresenters(this.Owner.CurrentDate, this.Owner.CurrentDate);
            }
        }
    }
}
