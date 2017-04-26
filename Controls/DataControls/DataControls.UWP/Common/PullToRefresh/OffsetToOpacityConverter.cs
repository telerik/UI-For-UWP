using System;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// Represents a custom value converter that transforms an offset to opacity value.
    /// </summary>
    public class OffsetToOpacityConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double maxValue = System.Convert.ToDouble(parameter);

            var offset = (double)value;

            return Math.Min(1, offset / maxValue);
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
