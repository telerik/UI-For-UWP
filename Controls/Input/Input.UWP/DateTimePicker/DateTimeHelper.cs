using System;
using Windows.Globalization;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Provides helper methods for working with <see cref="DateTime"/> instances in the context of a <see cref="DateTimePicker"/> control.
    /// </summary>
    internal static class DateTimeHelper
    {
        internal const string LongHourChar = "H";

        /// <summary>
        /// Returns a boolean value that determines whether a given time format string
        /// implies 24 hour time format.
        /// </summary>
        /// <param name="timeFormat">The string to check.</param>
        /// <returns>True if the string implies 24 hour time format, otherwise false.</returns>
        internal static bool Is24HourFormat(string timeFormat)
        {
            return timeFormat.Contains(LongHourChar);
        }

        /// <summary>
        /// Coerces the <paramref name="utcDateTime"/> according to the min and max
        /// allowed values of the <paramref name="calendar"/> parameter.
        /// </summary>
        /// <returns>The coerced value.</returns>
        internal static DateTime CoerceDateTime(DateTime utcDateTime, Windows.Globalization.Calendar calendar)
        {
            var calendarValue = calendar.GetDateTime().UtcDateTime;
            var dateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);

            calendar.SetToMin();
            calendar.AddDays(1);
            var minValue = calendar.GetDateTime().UtcDateTime.AddDays(-1);

            calendar.SetToMax();
            calendar.AddDays(-1);
            var maxValue = calendar.GetDateTime().UtcDateTime.AddDays(1);

            calendar.SetDateTime(calendarValue);

            if (dateTime < minValue)
            {
                return DateTime.SpecifyKind(minValue, utcDateTime.Kind);
            }

            if (dateTime > maxValue)
            {
                return DateTime.SpecifyKind(maxValue, utcDateTime.Kind);
            }

            return utcDateTime;
        }

        /// <summary>
        /// Transforms the <paramref name="hour"/> parameter to a zero based hour.
        /// </summary>
        /// <returns>
        /// Zero based hour. If the clock is 12 hour and the
        /// hour value is 12, then this method will return 0.
        /// </returns>
        internal static int GetZeroBasedHour(string clockIdentifier, int hour)
        {
            if (clockIdentifier == ClockIdentifiers.TwentyFourHour)
            {
                return hour;
            }
            if (clockIdentifier == ClockIdentifiers.TwelveHour)
            {
                return hour == 12 ? 0 : hour;
            }

            throw new ArgumentException("Invalid clock identifier.");
        }
    }
}