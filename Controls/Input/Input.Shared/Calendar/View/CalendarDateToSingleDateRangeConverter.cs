using System;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// This converter is used to convert <see cref="DateTime"/> value to a single-date <see cref="CalendarDateRange"/> and vice versa.
    /// </summary>
    public class CalendarDateToSingleDateRangeConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified <see cref="DateTime"/> value to a single-date <see cref="CalendarDateRange"/> instance.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns><see cref="CalendarDateRange"/> instance with <see cref="CalendarDateRange.StartDate"/> and
        /// <see cref="CalendarDateRange.EndDate"/> properties equal to the <paramref name="value"/> parameter.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                DateTime date = (DateTime)value;
                return new CalendarDateRange(date, date);
            }

            return null;
        }

        /// <summary>
        /// Converts a <see cref="CalendarDateRange"/> instance to a <see cref="DateTime"/> value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns><see cref="DateTime"/> structure equal to the <see cref="CalendarDateRange.StartDate"/> property of the <see cref="CalendarDateRange"/>.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                return ((CalendarDateRange)value).StartDate;
            }

            return null;
        }
    }
}
