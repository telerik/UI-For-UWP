using System;
using Windows.Globalization;

namespace Telerik.UI.Xaml.Controls.Input
{
    internal static class CalendarExtentions
    {
        /// <summary>
        /// Gets the UTC <see cref="DateTime"/> of the calendar.
        /// </summary>
        internal static DateTime GetUtcDateTime(this Windows.Globalization.Calendar calendar)
        {
            DateTime dateTime;
            if (calendar.Year == calendar.LastYearInThisEra)
            {
                calendar.AddYears(-1);
                dateTime = calendar.GetDateTime().UtcDateTime;
                if (dateTime > DateTime.MaxValue.AddYears(-1))
                {
                    dateTime = DateTime.MaxValue;
                }
                else
                {
                    dateTime = dateTime.AddYears(1);
                }

                calendar.AddYears(1);
            }
            else if (calendar.Year == calendar.FirstYearInThisEra)
            {
                calendar.AddYears(1);
                dateTime = calendar.GetDateTime().UtcDateTime;
                if (dateTime < DateTime.MinValue.AddYears(1))
                {
                    dateTime = DateTime.MinValue;
                }
                else
                {
                    dateTime = dateTime.AddYears(-1);
                }

                calendar.AddYears(-1);
            }
            else
            {
                dateTime = calendar.GetDateTime().UtcDateTime;
            }

            return dateTime;
        }

        /// <summary>
        /// Gets zero based calendar hour.
        /// </summary>
        /// <returns>
        /// Calendar hour. If the calendar clock is 12 hour and the
        /// hour value is 12, then this method will return 0.
        /// </returns>
        internal static int ZeroBasedHour(this Windows.Globalization.Calendar calendar)
        {
            if (calendar.GetClock() == ClockIdentifiers.TwentyFourHour)
            {
                return calendar.Hour;
            }
            else
            {
                return calendar.Hour == 12 ? 0 : calendar.Hour;
            }
        }

        /// <summary>
        /// Sets zero based calendar hour.
        /// </summary>
        internal static void SetZeroBasedHour(this Windows.Globalization.Calendar calendar, int hour)
        {
            if (calendar.GetClock() == ClockIdentifiers.TwentyFourHour)
            {
                if (hour < 0 || hour > 23)
                {
                    throw new ArgumentException("Hour should be between 0 and 23.");
                }

                calendar.AddHours(hour - calendar.Hour);
            }
            else
            {
                if (hour < 0 || hour > 11)
                {
                    throw new ArgumentException("Hour should be between 0 and 11.");
                }
                var calendarHour = calendar.Hour == 12 ? 0 : calendar.Hour;
                calendar.AddHours(hour - calendarHour);
            }
        }
    }
}