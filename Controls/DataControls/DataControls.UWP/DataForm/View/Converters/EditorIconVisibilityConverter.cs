using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data.DataForm
{
    /// <summary>
    /// Value converter that converts an <see cref="EditorIconDisplayMode"/> value to a <see cref="Visibility"/>.
    /// </summary>
    public class EditorIconVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Convert an <see cref="EditorIconDisplayMode"/> value to a <see cref="Visibility"/>.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">The format string.</param>
        /// <param name="language">The language of the conversion. Not used.</param>
        /// <returns>The formatted string.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter != null && parameter.ToString() == "HasErrors")
            {
                if (value != null)
                {
                    var i = System.Convert.ToInt32(value);
                    return i > 0;
                }
            }

            if (value != null)
            {
                var displayMode = (EditorIconDisplayMode)value;
                var iconType = parameter.ToString();
                if (iconType == "Label")
                {
                    if (displayMode == EditorIconDisplayMode.All || displayMode == EditorIconDisplayMode.Label)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }
                else if (iconType == "Error")
                {
                    if (displayMode == EditorIconDisplayMode.All || displayMode == EditorIconDisplayMode.Error)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }
            }

            return Visibility.Collapsed;
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
