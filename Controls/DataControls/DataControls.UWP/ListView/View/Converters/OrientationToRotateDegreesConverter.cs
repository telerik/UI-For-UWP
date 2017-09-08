using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// Value converter that converts an <see cref="Orientation"/> value to a rotation degrees.
    /// </summary>
    public class OrientationToRotateDegreesConverter : IValueConverter
    {
        /// <summary>
        /// Convert an <see cref="Orientation"/> value to a rotation degrees.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">The format string.</param>
        /// <param name="language">The language of the conversion. Not used.</param>
        /// <returns>The formatted string.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var orientation = (Orientation)value;
            if (orientation == Orientation.Vertical)
            {
                return 0;
            }
            else
            {
                return -90;
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">Optional parameter. Not used.</param>
        /// <param name="language">The language of the conversion. Not used.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
