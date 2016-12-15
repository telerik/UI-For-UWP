using System;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// This converter can be used to fill visual elements
    /// in the ticks and labels with a color that depends on
    /// the value of the tick or label.
    /// </summary>
    [ContentProperty(Name = "Ranges")]
    public class GaugeValueToBrushConverter : IValueConverter
    {
        private Collection<GaugeColorValueRange> ranges = new Collection<GaugeColorValueRange>();

        /// <summary>
        /// Initializes a new instance of the GaugeValueToBrushConverter class.
        /// </summary>
        public GaugeValueToBrushConverter()
        {
            this.DefaultColor = Colors.White;
        }

        /// <summary>
        /// Gets a list of GaugeColorToValueRanges.
        /// </summary>
        public Collection<GaugeColorValueRange> Ranges
        {
            get
            {
                return this.ranges;
            }
        }

        /// <summary>
        /// Gets or sets the default color of the converter.
        /// </summary>
        /// <remarks>
        /// If a value is passed that is not in the ranges list
        /// the default color will be returned.
        /// </remarks>
        public Color DefaultColor
        {
            get;
            set;
        }

        /// <summary>
        /// Converts the value to its corresponding color.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double doubleValue;
            GaugeColorValueRange range;

            if (value != null && 
                double.TryParse(value.ToString(), out doubleValue) &&
                this.FindValueRange(doubleValue, out range))
            {
                return new SolidColorBrush() { Color = range.Color };
            }

            return new SolidColorBrush() { Color = this.DefaultColor };
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private static bool IsValueInRange(double value, double min, double max)
        {
            if (value >= min && value <= max)
            {
                return true;
            }

            return false;
        }

        private bool FindValueRange(double value, out GaugeColorValueRange result)
        {
            foreach (GaugeColorValueRange range in this.ranges)
            {
                if (IsValueInRange(value, range.MinValue, range.MaxValue))
                {
                    result = range;
                    return true;
                }
            }

            result = null;
            return false;
        }
    }
}
