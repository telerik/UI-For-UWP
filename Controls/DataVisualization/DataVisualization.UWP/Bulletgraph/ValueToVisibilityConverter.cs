using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.DataVisualization.BulletGraph
{
    /// <summary>
    /// This class converts 0 to its Visibility property value and non-zero to the opposite of its Visibility property value.
    /// </summary>
    public class ValueToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets a value that is returned if the value to be converted is 0.
        /// If the value to be converted is non-zero the opposite of this property is returned.
        /// </summary>
        public Visibility Visibility { get; set; }

        /// <summary>
        /// Converts a non positive value to the value of the <see cref="Visibility"/> property and
        /// a positive value to the opposite of the <see cref="Visibility"/> property value.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double actualValue = System.Convert.ToDouble(value, CultureInfo.CurrentUICulture.NumberFormat);
            
            if (actualValue <= 0)
            {
                return this.Visibility;
            }

            return this.Visibility.Opposite();
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
