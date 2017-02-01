using System;
using Telerik.Data.Core;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    /// <summary>
    /// Represents a custom <see cref="IValueConverter"/> implementation that converts a <see cref="SortOrder"/> value to a character code from the Segoe UI Symbol font.
    /// </summary>
    public class SortOrderToGlyphConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value of type <see cref="SortOrder"/> to a string that corresponds to a symbol that represents the sort order.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is SortOrder))
            {
                return value;
            }

            SortOrder direction = (SortOrder)value;
#if WINDOWS_UWP
            switch (direction)
            {
                case SortOrder.Ascending:
                    return "\uEB11";
                case SortOrder.Descending:
                    return "\uEB0F";
                default:
                    throw new ArgumentException("Unknown SortOrder.");
            }
#else
            switch (direction)
            {
                case SortOrder.Ascending:
                    return "\u25B2";
                case SortOrder.Descending:
                    return "\u25BC";
                default:
                    throw new ArgumentException("Unknown SortOrder.");
            }
#endif
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
