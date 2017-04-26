using System;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    /// <summary>
    /// Value converter that converts a <see cref="bool"/> value to a content.
    /// </summary>
    public class DataOperationsButtonTextConverter : IValueConverter
    {
        /// <summary>
        /// Convert a <see cref="bool"/> value to a Content.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">The format string.</param>
        /// <param name="language">The language of the conversion. Not used.</param>
        /// <returns>The formatted string.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var canGroupBy = (bool)value;
            if (canGroupBy)
            {
                return GridLocalizationManager.Instance.GetString("DataOperationsButtonGroup");
            }
            else
            {
                return GridLocalizationManager.Instance.GetString("DataOperationsButtonUngroup");
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
