using System;
using System.Collections;
using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Input.Calendar;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents a list of <see cref="CalendarDateRange"/> items used by the selection mechanism of <see cref="RadCalendar"/>.
    /// </summary>
    /// <seealso cref="CalendarDateRange"/>
    /// <seealso cref="RadCalendar.SelectedDateRange"/>
    public class CalendarDateRangeCollection : IEnumerable<CalendarDateRange>
    {
        private List<CalendarDateRange> dateRangesList;
        private int suspendLevel;
        private RadCalendar owner;

        internal CalendarDateRangeCollection(RadCalendar owner)
        {
            this.owner = owner;
            this.dateRangesList = new List<CalendarDateRange>();
        }

        /// <summary>
        /// Gets the number of date ranges actually contained in this instance.
        /// </summary>
        public int Count
        {
            get
            {
                return this.dateRangesList.Count;
            }
        }

        internal RadCalendar Owner
        {
            get
            {
                return this.owner;
            }
        }

        private bool IsSuspended
        {
            get
            {
                return this.suspendLevel > 0;
            }
        }

        /// <summary>
        /// Gets the <see cref="CalendarDateRange"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="CalendarDateRange"/>.
        /// </value>
        /// <param name="index">The index.</param>
        public CalendarDateRange this[int index]
        {
            get
            {
                return this.dateRangesList[index];
            }
        }

        /// <summary>
        /// Adds the specified <see cref="CalendarDateRange"/> instance to the list.
        /// </summary>
        /// <param name="newRange">The range to be added.</param>
        public void Add(CalendarDateRange newRange)
        {
            if (!this.Owner.IsTemplateApplied)
            {
                if (this.Owner.unattachedSelectedRanges == null)
                {
                    this.Owner.unattachedSelectedRanges = new List<CalendarDateRange>();
                }

                this.Owner.unattachedSelectedRanges.Add(newRange);

                return;
            }

            List<CalendarDateRange> rangesToSelect = this.GetActualRangesToSelect(newRange);
            if (rangesToSelect == null)
            {
                return;
            }

            bool isDirty = false;
            foreach (CalendarDateRange rangeToSelect in rangesToSelect)
            {
                isDirty = this.AddDateRange(rangeToSelect);
            }

            if (isDirty)
            {
                this.OnSelectionChanged();
            }
        }

        /// <summary>
        /// Clears all <see cref="CalendarDateRange"/> instances from the list.
        /// </summary>
        /// <param name="raiseSelectionChanged">Specifies whether the <see cref="OnSelectionChanged"/>
        /// event will be raised when the collection is cleared. The default value is <c>true</c></param>
        public void Clear(bool raiseSelectionChanged = true)
        {
            if (this.Count == 0)
            {
                return;
            }

            this.Owner.SelectionService.ClearCellsIsSelectedFlags();

            this.dateRangesList.Clear();

            if (raiseSelectionChanged)
            {
                this.OnSelectionChanged();
            }
        }

        /// <summary>
        /// Removes the specified <see cref="CalendarDateRange"/> instance from the list.
        /// </summary>
        /// <param name="range">The range to be removed.</param>
        public void Remove(CalendarDateRange range)
        {
            if (!this.dateRangesList.Remove(range))
            {
                return;
            }

            this.Owner.SelectionService.RaiseCellsIsSelectedFlags(range);

            this.OnSelectionChanged();
        }

        /// <summary>
        /// Removes the <see cref="CalendarDateRange"/> instance at the specified index from the list.
        /// </summary>
        /// <param name="index">The index of the range to be removed.</param>
        public void RemoveAt(int index)
        {
            this.Remove(this.dateRangesList[index]);
        }

        /// <summary>
        /// Iterates through all dates in all selected ranges.
        /// </summary>
        /// <returns>Returns <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/> of all dates
        /// in this collection.</returns>
        public IEnumerable<DateTime> AsDates()
        {
            foreach (CalendarDateRange range in this.dateRangesList)
            {
                foreach (DateTime date in range)
                {
                    yield return date;
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<CalendarDateRange> GetEnumerator()
        {
            return this.dateRangesList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.dateRangesList.GetEnumerator();
        }

        internal void SuspendUpdate()
        {
            this.suspendLevel++;
        }

        internal void ResumeUpdate()
        {
            if (this.suspendLevel > 0)
            {
                this.suspendLevel--;
            }
        }

        internal void SplitRangeByDate(CalendarCellModel blackoutCell)
        {
            if (this.dateRangesList.Count == 0)
            {
                return;
            }

            int index = this.dateRangesList.BinarySearch(new CalendarDateRange(blackoutCell.Date, blackoutCell.Date));

            if (index >= 0)
            {
                var rangeToResize = this.dateRangesList[index];
                rangeToResize.StartDate = blackoutCell.Date.AddDays(1);
            }
            else
            {
                int currentIndex = ~index;

                if (currentIndex == 0)
                {
                    return;
                }

                if (CalendarDateRangeCollection.DateIsInRange(blackoutCell.Date, this.dateRangesList[currentIndex - 1]))
                {
                    this.SplitRanges(currentIndex - 1, blackoutCell.Date);
                }
            }
        }

        /// <summary>
        /// Exposed for testing purposes.
        /// </summary>
        internal bool AddDateRange(CalendarDateRange newRange)
        {
            bool isDirty = false;

            int index = this.dateRangesList.BinarySearch(newRange);
            if (index < 0)
            {
                int currentIndex = ~index;
                int rangesCount = this.Count;
                this.dateRangesList.Insert(currentIndex, newRange);

                bool isMergeExecuted = this.MergeCollidingRanges(newRange, currentIndex);
                if (isMergeExecuted || rangesCount != this.Count)
                {
                    isDirty = true;
                }
            }
            else
            {
                var currentRange = this.dateRangesList[index];

                if (newRange.EndDate.Date > currentRange.EndDate.Date)
                {
                    this.dateRangesList[index] = newRange;

                    this.MergeCollidingRanges(newRange, index);
                    isDirty = true;
                }
            }

            return isDirty;
        }

        private static bool DateIsInRange(DateTime date, CalendarDateRange range)
        {
            // NOTE: Ignore time part for calendar calculations.
            if (range.StartDate.Date <= date && date <= range.EndDate.Date)
            {
                return true;
            }

            return false;
        }

        private List<CalendarDateRange> GetActualRangesToSelect(CalendarDateRange currentRange)
        {
            Tuple<int, int> rangeIndices = this.Owner.SelectionService.GetVisibleSelectedIndicesFromRange(currentRange);

            int rangeStartIndex = -1;
            int rangeEndIndex = -1;

            if (rangeIndices != null)
            {
                rangeStartIndex = rangeIndices.Item1;
                rangeEndIndex = rangeIndices.Item2;
            }

            List<CalendarDateRange> rangesToSelect = null;

            if (this.Owner.SelectionMode == CalendarSelectionMode.Single)
            {
                rangesToSelect = this.GetActualRangeForSingleSelection(currentRange, rangeStartIndex);
            }
            else if (this.Owner.SelectionMode == CalendarSelectionMode.Multiple)
            {
                rangesToSelect = this.GetActualRangesForMultipleSelection(currentRange, rangeStartIndex, rangeEndIndex);
            }

            return rangesToSelect;
        }

        private List<CalendarDateRange> GetActualRangeForSingleSelection(CalendarDateRange range, int rangeStartIndex)
        {
            List<CalendarDateRange> rangesToSelect = new List<CalendarDateRange>();

            // NOTE: Ignore time part for calendar calculations.
            if (range.StartDate.Date == range.EndDate.Date &&
                (this.Count == 0 || this.dateRangesList[0] != range))
            {
                CalendarCellModel firstVisibleCell = this.Owner.Model.CalendarCells[0];
                CalendarCellModel lastVisibleCell = this.Owner.Model.CalendarCells[this.Owner.Model.CalendarCells.Count - 1];

                CalendarCellModel cellToSelect = null;
                if (firstVisibleCell.Date <= range.StartDate.Date && range.StartDate.Date <= lastVisibleCell.Date)
                {
                    cellToSelect = this.Owner.Model.CalendarCells[rangeStartIndex];
                }

                if (cellToSelect == null || !cellToSelect.IsBlackout || (cellToSelect is CalendarMonthCellModel && !((CalendarMonthCellModel)cellToSelect).IsSpecialReadOnly))
                {
                    this.Clear(false);
                    rangesToSelect.Add(range);
                }
            }

            return rangesToSelect;
        }

        private List<CalendarDateRange> GetActualRangesForMultipleSelection(CalendarDateRange range, int rangeStartIndex, int rangeEndIndex)
        {
            List<CalendarDateRange> rangesToSelect = new List<CalendarDateRange>();

            CalendarCellModel firstVisibleCell = this.Owner.Model.CalendarCells[0];
            CalendarCellModel lastVisibleCell = this.Owner.Model.CalendarCells[this.Owner.Model.CalendarCells.Count - 1];

            // NOTE: Ignore time part for calendar calculations.
            if (range.EndDate.Date < firstVisibleCell.Date || range.StartDate.Date > lastVisibleCell.Date)
            {
                rangesToSelect.Add(range);
            }
            else
            {
                bool rangeInitialized = false;

                CalendarDateRange newRange = CalendarDateRange.Empty;
                if (range.StartDate.Date < firstVisibleCell.Date)
                {
                    newRange.StartDate = range.StartDate;
                    rangeInitialized = true;
                }

                for (int index = rangeStartIndex; index <= rangeEndIndex; index++)
                {
                    var cellModel = this.Owner.Model.CalendarCells[index];
                    if (!rangeInitialized && (!cellModel.IsBlackout || (cellModel is CalendarMonthCellModel && !((CalendarMonthCellModel)cellModel).IsSpecialReadOnly)))
                    {
                        newRange.StartDate = cellModel.Date;
                        rangeInitialized = true;

                        continue;
                    }

                    if ((cellModel.IsBlackout || (cellModel is CalendarMonthCellModel && ((CalendarMonthCellModel)cellModel).IsSpecialReadOnly)) && rangeInitialized)
                    {
                        newRange.EndDate = this.Owner.Model.CalendarCells[index - 1].Date;
                        rangesToSelect.Add(newRange);
                        rangeInitialized = false;
                    }
                }

                if (rangeInitialized)
                {
                    newRange.EndDate = this.Owner.Model.CalendarCells[rangeEndIndex].Date;

                    if (range.EndDate.Date > lastVisibleCell.Date)
                    {
                        newRange.EndDate = range.EndDate;
                    }

                    rangesToSelect.Add(newRange);
                }
            }

            return rangesToSelect;
        }

        private void SplitRanges(int indexToSplit, DateTime blackoutDate)
        {
            if (this.dateRangesList[indexToSplit].EndDate == blackoutDate)
            {
                var rangeToResize = this.dateRangesList[indexToSplit];
                rangeToResize.EndDate = blackoutDate.AddDays(-1);

                return;
            }

            CalendarDateRange leftRange = new CalendarDateRange(this.dateRangesList[indexToSplit].StartDate, blackoutDate.AddDays(-1));
            CalendarDateRange rightRange = new CalendarDateRange(blackoutDate.AddDays(1), this.dateRangesList[indexToSplit].EndDate);

            this.dateRangesList[indexToSplit] = leftRange;
            this.Add(rightRange);
        }

        private bool MergeCollidingRanges(CalendarDateRange newDataRange, int currentIndex)
        {
            bool isMergeExecuted = false;

            CalendarDateRange currentDataRange = newDataRange;

            // Try merge the range on the left
            if (currentIndex > 0 && this.dateRangesList.Count > 1)
            {
                if (this.dateRangesList[currentIndex - 1].IntersectsWithRange(currentDataRange))
                {
                    CalendarDateRange mergedRange = this.dateRangesList[currentIndex - 1].MergeDateRange(currentDataRange);
                    CalendarDateRange previousRange = this.dateRangesList[currentIndex - 1];

                    if (mergedRange.StartDate != previousRange.StartDate || mergedRange.EndDate != previousRange.EndDate)
                    {
                        isMergeExecuted = true;
                    }

                    this.dateRangesList[currentIndex - 1] = mergedRange;
                    this.dateRangesList.RemoveAt(currentIndex);

                    currentIndex--;
                    currentDataRange = this.dateRangesList[currentIndex];
                }
            }

            // Find the last colliding index
            int lastCollidingIndex = currentIndex;
            for (int i = currentIndex + 1; i < this.dateRangesList.Count; i++)
            {
                var rangeToMerge = this.dateRangesList[i];
                if (!rangeToMerge.IntersectsWithRange(currentDataRange))
                {
                    // break the loop, ranges are ordered by start date and no further ranges might intersect
                    break;
                }

                lastCollidingIndex = i;
            }

            if (lastCollidingIndex > currentIndex)
            {
                // Merge the current range with the last colliding one and remove all the ranges between the current index and the last colliding
                CalendarDateRange lastItemToMerge = this.dateRangesList[lastCollidingIndex];
                this.dateRangesList[currentIndex] = this.dateRangesList[currentIndex].MergeDateRange(lastItemToMerge);

                for (int i = lastCollidingIndex; i > currentIndex; i--)
                {
                    this.dateRangesList.RemoveAt(i);
                }

                isMergeExecuted = true;
            }

            return isMergeExecuted;
        }

        private void OnSelectionChanged()
        {
            if (this.IsSuspended)
            {
                return;
            }

            this.Owner.SelectionService.RaiseSelectionChanged();
        }
    }
}