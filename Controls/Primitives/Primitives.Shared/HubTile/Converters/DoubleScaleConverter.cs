using System;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Primitives.HubTile
{
    /// <summary>
    /// Represents a custom <see cref="IValueConverter"/> implementation that takes a double value and multiplies it by a given scale factor.
    /// </summary>
    public class DoubleScaleConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the scale used to multiply the provided System.Double value.
        /// </summary>
        public double Scale
        {
            get;
            set;
        }

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return this.Scale * (double)value;
        }

        /// <summary>
        /// ConvertBack is not a valid operation.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <exception cref="System.NotImplementedException">ConvertBack is not a valid operation for this converter.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
