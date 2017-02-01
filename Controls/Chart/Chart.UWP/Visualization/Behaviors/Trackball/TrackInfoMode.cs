using System;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Defines how the track information for each chart series is visualized by a <see cref="ChartTrackBallBehavior"/>.
    /// </summary>
    public enum TrackInfoMode
    {
        /// <summary>
        /// Each series information is displayed in a box on top of the plot area.
        /// </summary>
        Multiple,

        /// <summary>
        /// A tooltip-like box is displayed next to each intersection data point.
        /// </summary>
        Individual,
    }
}
