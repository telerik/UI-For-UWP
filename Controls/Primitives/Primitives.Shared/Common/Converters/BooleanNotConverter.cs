using System;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a custom <see cref="IValueConverter"/> that converts a Boolean value to its opposite (!)(NOT) value.
    /// </summary>
    public class BooleanNotConverter : IValueConverter
    {
        /// <summary>
        /// Converts a Boolean value to its opposite (!)(NOT) value.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is bool))
            {
                return value;
            }

            return !(bool)value;
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
