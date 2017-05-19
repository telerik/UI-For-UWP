using System;
using Telerik.Data.Core;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data.DataForm
{
    /// <summary>
    /// Value converter that returns information if <see cref="ITypeEditor"/> is enabled.
    /// </summary>
    public class IsEnabledEditorConvetrer : IValueConverter
    {
        /// <summary>
        /// Returns information if <see cref="ITypeEditor"/> is enabled.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">The format string.</param>
        /// <param name="language">The language of the conversion. Not used.</param>
        /// <returns>The formatted string.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var property = value as EntityProperty;

            if (property != null)
            {
                if (property.Entity != null && property.Entity.IsReadOnly)
                {
                    return false;
                }

                return !property.IsReadOnly;
            }

            return true;
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
