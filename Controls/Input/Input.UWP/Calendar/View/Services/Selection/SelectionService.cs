using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal partial class SelectionService : ServiceBase<RadCalendar>
    {
        internal CalendarDateRangeCollection selectedDateRanges;
        internal CalendarDateRangeCollection detachedDateRanges;

        internal SelectionService(RadCalendar owner)
            : base(owner)
        {
            if (this.Owner == null)
            {
                throw new ArgumentNullException("Selection service cannot operate without owner");
            }

            this.selectedDateRanges = new CalendarDateRangeCollection(this.Owner);
            this.detachedDateRanges = new CalendarDateRangeCollection(this.Owner);
        }

        internal event EventHandler<CurrentSelectionChangedEventArgs> SelectionChanged;

        internal void UpdateSelectedCells()
        {
            if (this.Owner.DisplayMode != CalendarDisplayMode.MonthView || this.selectedDateRanges.Count == 0)
            {
                return;
            }

            foreach (CalendarDateRange range in this.selectedDateRanges)
            {
                Tuple<int, int> rangeIndices = this.GetVisibleSelectedIndicesFromRange(range);

                if (rangeIndices == null)
                {
                    continue;
                }

                this.UpdateCellsIsSelectedFlags(rangeIndices, true);
            }
        }

        internal void Select(CalendarDateRange newDateRange)
        {
            this.selectedDateRanges.Add(newDateRange);
        }

        internal void ClearSelection(bool raiseSelectionChanged = true)
        {
            this.detachedDateRanges = new CalendarDateRangeCollection(this.Owner);
            foreach (var item in this.selectedDateRanges)
            {
                this.detachedDateRanges.AddDateRange(item);
            }
            this.selectedDateRanges.Clear(raiseSelectionChanged);
        }

        internal void RaiseSelectionChanged()
        {
            this.UpdateSelectedCells();

            List<CalendarCellModel> cellsToUpdate = new List<CalendarCellModel>();
            var allDates = this.detachedDateRanges.Union(this.selectedDateRanges);
            foreach (var range in allDates)
            {
                var indices = this.GetVisibleSelectedIndicesFromRange(range);
                if (indices == null)
                {
                    continue;
                }
                
                var cells = this.GetCellModelsByRange(indices);
                cellsToUpdate.AddRange(cells);
            }
            this.Owner.UpdatePresenters(cellsToUpdate.Distinct());

            this.OnSelectionChanged();
        }

        internal void RaiseCellsIsSelectedFlags(CalendarDateRange range)
        {
            Tuple<int, int> rangeIndices = this.GetVisibleSelectedIndicesFromRange(range);
            if (rangeIndices != null)
            {
                this.UpdateCellsIsSelectedFlags(rangeIndices, true);
            }
        }

        internal void ClearCellsIsSelectedFlags()
        {
            if (this.selectedDateRanges == null || this.Owner.Model.CalendarCells == null)
            {
                return;
            }

            foreach (CalendarDateRange range in this.selectedDateRanges)
            {
                Tuple<int, int> rangeIndices = this.GetVisibleSelectedIndicesFromRange(range);

                if (rangeIndices == null)
                {
                    continue;
                }

                this.UpdateCellsIsSelectedFlags(rangeIndices, false);
            }
        }

        internal Tuple<int, int> GetVisibleSelectedIndicesFromRange(CalendarDateRange range)
        {
            CalendarCellModel firstVisibleCell = this.Owner.Model.CalendarCells[0];
            CalendarCellModel lastVisibleCell = this.Owner.Model.CalendarCells[this.Owner.Model.CalendarCells.Count - 1];

            // NOTE: Ignore time part for calendar calculations.
            if (range.EndDate.Date < firstVisibleCell.Date || range.StartDate.Date > lastVisibleCell.Date)
            {
                return null;
            }

            int firstCellIndex, lastCellIndex;

            if (range.StartDate.Date < firstVisibleCell.Date && range.EndDate.Date > lastVisibleCell.Date)
            {
                firstCellIndex = firstVisibleCell.CollectionIndex;
                lastCellIndex = lastVisibleCell.CollectionIndex;
            }
            else if (range.StartDate.Date < firstVisibleCell.Date)
            {
                firstCellIndex = firstVisibleCell.CollectionIndex;
                lastCellIndex = this.Owner.GetCellByDate(range.EndDate).CollectionIndex;
            }
            else if (range.EndDate.Date > lastVisibleCell.Date)
            {
                firstCellIndex = this.Owner.GetCellByDate(range.StartDate).CollectionIndex;
                lastCellIndex = lastVisibleCell.CollectionIndex;
            }
            else
            {
                firstCellIndex = this.Owner.GetCellByDate(range.StartDate).CollectionIndex;
                lastCellIndex = this.Owner.GetCellByDate(range.EndDate).CollectionIndex;
            }

            return new Tuple<int, int>(firstCellIndex, lastCellIndex);
        }

        internal bool ShouldDeselectCell(CalendarCellModel cell)
        {
            // NOTE: Ignore time part for calendar calculations.
            if (this.selectedDateRanges.Count == 1 && this.selectedDateRanges[0].StartDate.Date == this.selectedDateRanges[0].EndDate.Date)
            {
                return this.Owner.GetCellByDate(this.selectedDateRanges[0].StartDate) == cell;
            }

            return false;
        }

        private static void UpdateCellIsSelectedFlag(CalendarCellModel cellModel, bool isSelected)
        {
            cellModel.IsSelected = isSelected;
        }

        private void UpdateCellsIsSelectedFlags(Tuple<int, int> range, bool isSelected)
        {
            for (int i = range.Item1; i <= range.Item2; i++)
            {
                SelectionService.UpdateCellIsSelectedFlag(this.Owner.Model.CalendarCells[i], isSelected);
            }
        }

        private IEnumerable<CalendarCellModel> GetCellModelsByRange(Tuple<int, int> range)
        {
            var list = new List<CalendarCellModel>();

            for (int i = range.Item1; i <= range.Item2; i++)
            {
                list.Add(this.Owner.Model.CalendarCells[i]);
            }

            return list;
        }

        private void OnSelectionChanged()
        {
            this.Owner.OnPropertyChanged(new PropertyChangedEventArgs("SelectedDateRanges"));

            CurrentSelectionChangedEventArgs args = new CurrentSelectionChangedEventArgs();

            if (this.Owner.SelectedDateRange.HasValue)
            {
                args.NewSelection = this.Owner.SelectedDateRange.Value.StartDate;
            }
            else
            {
                args.NewSelection = null;
            }

            EventHandler<CurrentSelectionChangedEventArgs> handler = this.SelectionChanged;
            if (handler != null)
            {
                handler(this.Owner, args);
            }

            if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected) ||
                    AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection))
            {
                RadCalendarAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this.Owner) as RadCalendarAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseSelectionEvents(args);
                }
            }
        }
    }
}
