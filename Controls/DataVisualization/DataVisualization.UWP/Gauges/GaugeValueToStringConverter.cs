using System.Globalization;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// Converts a value to a string and vice versa providing
    /// a Precision property to control the floating point precision
    /// of the converted value.
    /// </summary>
    public class GaugeValueToStringConverter : IValueConverter
    {
        private int precision = 1;

        /// <summary>
        /// Gets or sets the number of digits to be displayed after the decimal sign.
        /// This property is used only when converting a value to string, not vice versa.
        /// </summary>
        public int Precision
        {
            get
            {
                return this.precision;
            }

            set
            {
                this.precision = value;
            }
        }

        /// <summary>
        /// Converts a value to string depending on the <see cref="Precision"/> property.
        /// </summary>
        public object Convert(object value, System.Type targetType, object parameter, string language)
        {
            string fmt = "{0:F" + this.precision + "}";
            return string.Format(CultureInfo.CurrentUICulture, fmt, value);
        }

        /// <summary>
        /// Parses a string to double number.
        /// </summary>
        public object ConvertBack(object value, System.Type targetType, object parameter, string language)
        {
            return double.Parse((string)value, CultureInfo.CurrentUICulture.NumberFormat);
        }
    }
}
