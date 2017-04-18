using System;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal partial class SelectionService
    {
        private SelectOperation selectOperation;

        internal void StartSelection(Point lastPointSelected)
        {
            var cellModel = this.Owner.GetCellModelByPoint(lastPointSelected);

            if (cellModel != null)
            {
                this.InitSelection(cellModel);
            }
        }

        internal void InitSelection(CalendarCellModel cellModel)
        {
            this.selectOperation = new SelectOperation(cellModel);
            this.UpdateCellsIsSelectedFlags(this.selectOperation.CurrentSelectedRange, true);

            this.RefreshModifiedCells();
        }

        internal void ModifySelection(CalendarCellModel currentModelSelected)
        {
            if (this.selectOperation == null || !this.selectOperation.ShouldModifySelection(currentModelSelected))
            {
                return;
            }

            if (this.selectOperation.ShouldModifySelection(currentModelSelected))
            {
                this.Owner.VisualStateService.UpdateHoldDecoration(null);
            }

            this.selectOperation.SelectCell(currentModelSelected);

            this.UpdateCellsIsSelectedFlags(this.selectOperation.CellRangeToRefresh, false);
            this.UpdateCellsIsSelectedFlags(this.selectOperation.CurrentSelectedRange, true);

            this.UpdateSelectedCells();

            var cellsToUpdate = this.GetCellModelsByRange(this.selectOperation.CellRangeToRefresh);
            this.Owner.UpdatePresenters(cellsToUpdate);
        }

        internal void EndSelection(bool success)
        {
            if (success)
            {
                this.CommitSelectOperation(this.selectOperation.CurrentSelectedRange);
            }
            else
            {
                //// TODO: clear visuals refresh selected ones and update UI.
            }

            this.selectOperation = null;
        }

        internal void MakeRangeSelection(CalendarCellModel startModel, CalendarCellModel endModel)
        {
            this.InitSelection(startModel);
            this.ModifySelection(endModel);
            this.EndSelection(true);
        }

        private void CommitSelectOperation(Tuple<int, int> range)
        {
            var calendarCells = this.Owner.Model.CalendarCells;

            CalendarDateRange newDateRange = new CalendarDateRange(calendarCells[range.Item1].Date, calendarCells[range.Item2].Date);

            this.Select(newDateRange);
        }

        private void RefreshModifiedCells()
        {
            var cellsToUpdate = this.GetCellModelsByRange(this.selectOperation.CellRangeToRefresh);
            this.Owner.UpdatePresenters(cellsToUpdate);
        }       
    }
}
