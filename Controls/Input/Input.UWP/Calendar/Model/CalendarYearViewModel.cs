using System;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal class CalendarYearViewModel : CalendarViewModel
    {
        internal const int DefaultRowCount = 6;
        internal const int DefaultColumnCount = 2;

        public override int RowCount
        {
            get
            {
                return DefaultRowCount;
            }
        }

        public override int ColumnCount
        {
            get
            {
                return DefaultColumnCount;
            }
        }

        internal override DateTime GetFirstDateToRender(DateTime date)
        {
            return CalendarMathHelper.GetFirstDateOfYear(date);
        }

        internal override DateTime GetNextDateToRender(DateTime date)
        {
            return date.Month == DateTime.MaxValue.Month && date.Year == DateTime.MaxValue.Year ? date : date.AddMonths(1);
        }

        internal override void PrepareCalendarCell(CalendarCellModel cell, DateTime date)
        {
            cell.Date = date;
            cell.Label = string.Format(this.Calendar.Culture, this.Calendar.YearViewCellFormat, date);
        }
    }
}
