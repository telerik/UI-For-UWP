using System;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal static class CalendarMathHelper
    {
        internal static DateTime GetFirstDateOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        internal static DateTime GetFirstDateOfYear(DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }

        internal static DateTime GetFirstDateOfDecade(DateTime date)
        {
            return new DateTime((date.Year / 10) * 10, 1, 1);
        }

        internal static DateTime GetFirstDateOfCentury(DateTime date)
        {
            return new DateTime((date.Year / 100) * 100, 1, 1);
        }

        internal static DateTime GetFirstDateForCurrentDisplayUnit(DateTime date, CalendarDisplayMode displayMode)
        {
            // NOTE: Ignore time part for calendar calculations.
            date = date.Date;

            switch (displayMode)
            {
                case CalendarDisplayMode.YearView:
                    return CalendarMathHelper.GetFirstDateOfYear(date);
                case CalendarDisplayMode.DecadeView:
                    return CalendarMathHelper.GetFirstDateOfDecade(date);
                case CalendarDisplayMode.CenturyView:
                    return CalendarMathHelper.GetFirstDateOfCentury(date);
                default:
                    return CalendarMathHelper.GetFirstDateOfMonth(date);
            }
        }

        internal static DateTime GetLastDateForCurrentDisplayUnit(DateTime date, CalendarDisplayMode displayMode)
        {
            // NOTE: Ignore time part for calendar calculations.
            date = date.Date;

            switch (displayMode)
            {
                case CalendarDisplayMode.YearView:
                    return CalendarMathHelper.GetLastDateOfYearView(date);
                case CalendarDisplayMode.DecadeView:
                    return CalendarMathHelper.GetLastDateOfDecadeView(date);
                case CalendarDisplayMode.CenturyView:
                    return CalendarMathHelper.GetLastDateOfCenturyView(date);
                default:
                    return CalendarMathHelper.GetLastDateOfMonthView(date);
            }
        }

        internal static DateTime GetCellDateForViewLevel(DateTime date, CalendarDisplayMode displayMode)
        {
            switch (displayMode)
            {
                case CalendarDisplayMode.YearView:
                    return CalendarMathHelper.GetFirstDateOfMonth(date);
                case CalendarDisplayMode.DecadeView:
                    return CalendarMathHelper.GetFirstDateOfYear(date);
                case CalendarDisplayMode.CenturyView:
                    return CalendarMathHelper.GetFirstDateOfDecade(date);
                default:
                    return date;
            }
        }

        internal static DateTime IncrementByCell(DateTime date, int increment, CalendarDisplayMode displayMode)
        {
            switch (displayMode)
            {
                case CalendarDisplayMode.YearView:
                    return date.AddMonths(increment);
                case CalendarDisplayMode.DecadeView:
                    return date.AddYears(increment);
                case CalendarDisplayMode.CenturyView:
                    return date.AddYears(increment * 10);
                default:
                    return date.AddDays(increment);
            }
        }

        internal static bool IsCalendarViewChanged(DateTime displayDate, DateTime newDisplayDate, CalendarDisplayMode displayMode)
        {
            // NOTE: Ignore time part for calendar calculations.
            displayDate = displayDate.Date;
            newDisplayDate = newDisplayDate.Date;

            switch (displayMode)
            {
                case CalendarDisplayMode.YearView:
                    return displayDate.Year != newDisplayDate.Year;
                case CalendarDisplayMode.DecadeView:
                    return CalendarMathHelper.GetFirstDateOfDecade(displayDate) != CalendarMathHelper.GetFirstDateOfDecade(newDisplayDate);
                case CalendarDisplayMode.CenturyView:
                    return CalendarMathHelper.GetFirstDateOfCentury(displayDate) != CalendarMathHelper.GetFirstDateOfCentury(newDisplayDate);
                default:
                    return CalendarMathHelper.GetFirstDateOfMonth(displayDate) != CalendarMathHelper.GetFirstDateOfMonth(newDisplayDate);
            }
        }

        internal static DateTime IncrementByView(DateTime date, int increment, CalendarDisplayMode displayMode)
        {
            switch (displayMode)
            {
                case CalendarDisplayMode.YearView:
                    return date.AddYears(increment);
                case CalendarDisplayMode.DecadeView:
                    return date.AddYears(increment * 10);
                case CalendarDisplayMode.CenturyView:
                    return date.AddYears(increment * 100);
                default:
                    return date.AddMonths(increment);
            }
        }

        private static DateTime GetLastDateOfMonthView(DateTime date)
        {
            return CalendarMathHelper.GetFirstDateOfMonth(date).AddMonths(1).AddDays(-1);
        }

        private static DateTime GetLastDateOfYearView(DateTime date)
        {
            return new DateTime(date.Year, 12, 1);
        }

        private static DateTime GetLastDateOfDecadeView(DateTime date)
        {
            return CalendarMathHelper.GetFirstDateOfDecade(date).AddYears(9);
        }

        private static DateTime GetLastDateOfCenturyView(DateTime date)
        {
            return CalendarMathHelper.GetFirstDateOfCentury(date).AddYears(90);
        }
    }
}
