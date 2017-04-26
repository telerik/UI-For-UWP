using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    /// <summary>
    /// Value converter that converts the Border's Thickness to Margin value.
    /// </summary>
    public class BorderThicknessToMarginConverter : IValueConverter
    {
        /// <summary>
        /// Convert a border's Thickness value to a Margin.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">The format string.</param>
        /// <param name="language">The language of the conversion. Not used.</param>
        /// <returns>The formatted string.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var margin = (Thickness)value;
            if (margin != null)
            {
                return new Thickness(0, 0, -margin.Right, 0);
            }

            return value;
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
