using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Charting
{
    /// <summary>
    /// Defines the different modes used to plot a <see cref="DataPoint"/> by a <see cref="Telerik.UI.Xaml.Controls.Chart.DateTimeContinuousAxis"/> model.
    /// </summary>
    public enum DateTimePlotStretchMode
    {
        /// <summary>
        /// Each point is stretched within the owning tick (plot) slot.
        /// </summary>
        TickSlot,

        /// <summary>
        /// Each point's length is proportional to the total number of points plotted by the axis.
        /// </summary>
        Uniform,
    }
}
