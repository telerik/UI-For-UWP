using System;
using System.Globalization;

namespace Telerik.Charting
{
    /// <summary>
    /// Collection of helper methods for retrieving unique (year-wise) values for some date/time components besides the ones provided by DateTime class.  
    /// </summary>
    public static class DateTimeExtensions
    {
        //// Engine classes should be built against .NET20 for WinForms compatibility and .NET20 does not support extension methods.

        /// <summary>
        /// Gets the week component of the date represented by the DateTime instance.
        /// </summary>
        public static int GetWeekOfYear(DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstFourDayWeek, CultureInfo.CurrentUICulture.DateTimeFormat.FirstDayOfWeek);
        }

        /// <summary>
        /// Gets the quarter component of the date represented by the DateTime instance.
        /// </summary>
        public static int GetQuarterOfYear(DateTime dateTime)
        {
            return ((dateTime.Month - 1) / 3) + 1;
        }

        internal static int GetHourOfYear(DateTime dateTime)
        {
            return ((dateTime.DayOfYear - 1) * 24) + dateTime.Hour;
        }

        internal static int GetMinuteOfYear(DateTime dateTime)
        {
            return ((GetHourOfYear(dateTime) - 1) * 60) + dateTime.Minute;
        }

        internal static int GetSecondOfYear(DateTime dateTime)
        {
            return ((GetMinuteOfYear(dateTime) - 1) * 60) + dateTime.Second;
        }

        internal static int GetMillisecondOfYear(DateTime dateTime)
        {
            return ((GetSecondOfYear(dateTime) - 1) * 1000) + dateTime.Millisecond;
        }
    }
}
