using System;

namespace Telerik.Charting
{
    internal static class DateTimeHelper
    {
        public static bool TryGetDateTime(object value, out DateTime date)
        {
            if (value is DateTime)
            {
                date = (DateTime)value;
                return true;
            }

            if (value is DateTimeOffset)
            {
                date = ((DateTimeOffset)value).DateTime;
                return true;
            }

            date = DateTime.Now;
            return false;
        }
    }
}
