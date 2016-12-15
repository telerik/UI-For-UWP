using System;
using System.Collections;
using System.Collections.Generic;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents a calendar date range structure that is described by <see cref="StartDate"/> and <see cref="EndDate"/>.
    /// </summary>
    /// <seealso cref="CalendarDateRangeCollection"/>
    /// <seealso cref="RadCalendar.SelectedDateRange"/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public struct CalendarDateRange : IComparable<CalendarDateRange>, IEnumerable<DateTime>
    {
        internal static CalendarDateRange Empty = new CalendarDateRange(DateTime.MinValue, DateTime.MinValue);

        private DateTime startDate;
        private DateTime endDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarDateRange"/> struct.
        /// </summary>
        /// <param name="startDate">The start date of the range.</param>
        /// <param name="endDate">The end date of the range.</param>
        public CalendarDateRange(DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }

        /// <summary>
        /// Gets or sets the start date of the range.
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                return this.startDate;
            }
            set
            {
                this.startDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the end date of the range.
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                return this.endDate;
            }
            set
            {
                this.endDate = value;
            }
        }

        /// <summary>
        /// Specifies equality operator for <see cref="CalendarDateRange"/>.
        /// </summary>
        /// <param name="range1">The first <see cref="CalendarDateRange"/> object.</param>
        /// <param name="range2">The second <see cref="CalendarDateRange"/> object.</param>
        /// <returns>Boolean value that indicates whether the ranges are equal or not.</returns>
        public static bool operator ==(CalendarDateRange range1, CalendarDateRange range2)
        {
            return range1.Equals(range2);
        }

        /// <summary>
        /// Specifies inequality operator for <see cref="CalendarDateRange"/>.
        /// </summary>
        /// <param name="range1">The first <see cref="CalendarDateRange"/> object.</param>
        /// <param name="range2">The second <see cref="CalendarDateRange"/> object.</param>
        /// <returns>Boolean value that indicates whether the ranges are equal or not.</returns>
        public static bool operator !=(CalendarDateRange range1, CalendarDateRange range2)
        {
            return !(range1 == range2);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">The object that this instance will be compared to.</param>
        /// <returns>A standard value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(CalendarDateRange other)
        {
            // NOTE: Ignore time part for calendar calculations.
            return this.startDate.Date.CompareTo(other.startDate.Date);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is CalendarDateRange))
            {
                return false;
            }

            CalendarDateRange other = (CalendarDateRange)obj;

            // NOTE: Ignore time part for calendar calculations.
            return this.StartDate.Date == other.startDate.Date && this.EndDate.Date == other.EndDate.Date;
        }

        /// <summary>
        /// Returns an enumerator that iterates every date between start and end date.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through every single date in the range from start to end date.
        /// </returns>
        public IEnumerator<DateTime> GetEnumerator()
        {
            DateTime date = this.startDate;
            while (date.Date <= this.endDate.Date)
            {
                yield return date;

                date = date.AddDays(1);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.StartDate.GetHashCode() ^ this.EndDate.GetHashCode();
        }

        internal bool IntersectsWithRange(CalendarDateRange otherDateRange)
        {
            // NOTE: Ignore time part for calendar calculations.
            // NOTE: "Intersects" for us means intersecting or adjacent.
            DateTime startDateToCompare = this.startDate.Date.AddDays(-1);
            DateTime endDateToCompare = this.endDate.Date.AddDays(1);

            if (startDateToCompare.CompareTo(otherDateRange.startDate.Date) == 0)
            {
                return true;
            }
            else if (startDateToCompare.CompareTo(otherDateRange.StartDate.Date) > 0)
            {
                return startDateToCompare.CompareTo(otherDateRange.EndDate.Date) <= 0;
            }
            else
            {
                return otherDateRange.StartDate.Date.CompareTo(endDateToCompare) <= 0;
            }
        }

        internal CalendarDateRange MergeDateRange(CalendarDateRange otherRange)
        {
            // NOTE: Ignore time part for calendar calculations.
            DateTime mergedStart = this.startDate;
            if (this.startDate.Date > otherRange.startDate.Date)
            {
                mergedStart = otherRange.startDate;
            }

            DateTime mergedEnd = this.endDate;
            if (this.endDate.Date <= otherRange.endDate.Date)
            {
                mergedEnd = otherRange.endDate;
            }

            return new CalendarDateRange(mergedStart, mergedEnd);
        }
    }
}
