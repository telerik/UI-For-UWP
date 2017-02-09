using System;
using Windows.UI.Xaml.Data;

namespace Telerik.Core
{
    /// <summary>
    /// Uses the <see cref="M:String.ToUpper"/> method to convert a string to upper case.
    /// </summary>
    public class StringToUpperConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value to upper case using the <see cref="M:String.ToUpper"/> method.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var text = value as string;
            if (text != null)
            {
                return text.ToUpper();
            }

            return value;
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
