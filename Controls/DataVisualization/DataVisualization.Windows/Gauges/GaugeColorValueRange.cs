using Windows.UI;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// This class represents a color that
    /// should be used only in visual elements
    /// that are between the min and max values.
    /// </summary>
    public class GaugeColorValueRange
    {
        /// <summary>
        /// Gets or sets the color of this particular rage.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets the bottom limit of the range.
        /// </summary>
        public double MinValue { get; set; }

        /// <summary>
        /// Gets or sets the top limit of the range.
        /// </summary>
        public double MaxValue { get; set; }
    }
}