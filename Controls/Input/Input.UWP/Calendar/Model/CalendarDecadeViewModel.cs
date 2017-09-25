using System;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal class CalendarDecadeViewModel : CalendarViewModel
    {
        internal const int DefaultRowCount = 5;
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
            return CalendarMathHelper.GetFirstDateOfDecade(date);
        }

        internal override DateTime GetNextDateToRender(DateTime date)
        {
            return CalendarMathHelper.CouldAddYearsToDate(date.Year + 1) ? date.AddYears(1) : date;
        }

        internal override void PrepareCalendarCell(CalendarCellModel cell, DateTime date)
        {
            cell.Date = date;
            cell.Label = string.Format(this.Calendar.Culture, this.Calendar.DecadeViewCellFormat, date);

            //// NOTE: With default (5x2) layout there are no cells from another view - consider adding this logic if option to change the layout is added.
        }
    }
}
