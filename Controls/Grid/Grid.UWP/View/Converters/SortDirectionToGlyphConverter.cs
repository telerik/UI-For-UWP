using System;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    /// <summary>
    /// Represents a custom <see cref="IValueConverter"/> used to convert a <see cref="SortDirection"/> value to a symbol glyph.
    /// </summary>
    public class SortDirectionToGlyphConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value of type <see cref="SortDirection"/> to a string that corresponds to a symbol that represents the sort direction.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is SortDirection))
            {
                return value;
            }

            SortDirection direction = (SortDirection)value;

            switch (direction)
            {
                case SortDirection.Ascending:
                    return "\uEB11";
                case SortDirection.Descending:
                    return "\uEB0F";
                default:
                    return string.Empty;
            }
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
