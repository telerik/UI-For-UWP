using System;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal class SelectOperation
    {
        private int lastPivotIndex;
        private int firstCellIndex;

        public SelectOperation(CalendarCellModel startCell)
        {
            this.firstCellIndex = startCell.CollectionIndex;
            this.lastPivotIndex = startCell.CollectionIndex;

            this.CellRangeToRefresh = new Tuple<int, int>(this.firstCellIndex, this.firstCellIndex);
            this.CurrentSelectedRange = new Tuple<int, int>(this.firstCellIndex, this.firstCellIndex);
        }

        internal Tuple<int, int> CurrentSelectedRange
        {
            get;
            private set;
        }

        internal Tuple<int, int> CellRangeToRefresh
        {
            get;
            private set;
        }

        internal bool ShouldModifySelection(CalendarCellModel currentModelSelected)
        {
            return this.lastPivotIndex != currentModelSelected.CollectionIndex;
        }

        internal void SelectCell(CalendarCellModel currentModelSelected)
        {
            this.lastPivotIndex = currentModelSelected.CollectionIndex;

            this.UpdateCellsToRefresh(this.CurrentSelectedRange, this.lastPivotIndex);

            this.UpdateCurrentRange(this.lastPivotIndex);
        }

        private void UpdateCurrentRange(int selectedIndex)
        {
            var startRangeIndex = Math.Min(this.firstCellIndex, selectedIndex);
            var endRangeIndex = Math.Max(this.firstCellIndex, selectedIndex);
            this.CurrentSelectedRange = new Tuple<int, int>(startRangeIndex, endRangeIndex);
        }

        private void UpdateCellsToRefresh(Tuple<int, int> currentSelectedRange, int pivot)
        {
            var startRangeIndex = Math.Min(currentSelectedRange.Item1, pivot);
            var endRangeIndex = Math.Max(currentSelectedRange.Item2, pivot);

            this.CellRangeToRefresh = new Tuple<int, int>(startRangeIndex, endRangeIndex);
        }
    }
}
