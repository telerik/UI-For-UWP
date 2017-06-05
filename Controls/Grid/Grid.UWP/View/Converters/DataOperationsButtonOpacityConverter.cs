using System;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    /// <summary>
    /// Value converter that converts a <see cref="Boolean"/> value to an "Opacity".
    /// </summary>
    public class DataOperationsButtonOpacityConverter : IValueConverter
    {
        /// <summary>
        /// Convert a <see cref="Boolean"/> value to an "Opacity".
        /// </summary>
        /// <param name="value">The source string being passed to the target.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">Optional parameter. Not used.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>The string corresponding to the resource name.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool canOperate = (bool)value;

            return canOperate ? 1 : 0.5;
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
