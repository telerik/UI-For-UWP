using Telerik.Charting;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a chart series that plot their points using financial "Bar" shapes.
    /// </summary>
    public class OhlcSeries : OhlcSeriesBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OhlcSeries" /> class.
        /// </summary>
        public OhlcSeries()
        {
            this.DefaultStyleKey = typeof(OhlcSeries);
        }

        internal override FrameworkElement CreateDefaultDataPointVisual(DataPoint point)
        {
            return new OhlcStick();
        }
    }
}
