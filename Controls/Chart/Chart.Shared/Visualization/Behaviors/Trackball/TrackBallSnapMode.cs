using System;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Defines how a <see cref="ChartTrackBallBehavior"/> or a <see cref="ChartTooltipBehavior"/> instance should snap to the closest to a physical location data points.
    /// </summary>
    public enum TrackBallSnapMode
    {
        /// <summary>
        /// The trackball will not be snapped to any of the closest data points.
        /// </summary>
        None,

        /// <summary>
        /// The behavior will snap to the closest data point, regardless of the chart series that own it.
        /// </summary>
        ClosestPoint,

        /// <summary>
        /// The behavior will snap to the closest data points from all chart series.
        /// </summary>
        AllClosePoints
    }
}
