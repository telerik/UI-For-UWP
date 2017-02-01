using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Defines the possible ways of a <see cref="ChartPalette"/> instance to apply the palette of its owning chart.
    /// </summary>
    public enum SeriesPaletteMode
    {
        /// <summary>
        /// The palette is applied to data points depending on the index of the owning ChartSeries instance.
        /// </summary>
        Series,

        /// <summary>
        /// The palette is applied to the data points depending on the index of each data point within the owning ChartSeries instance.
        /// </summary>
        DataPoint
    }
}
