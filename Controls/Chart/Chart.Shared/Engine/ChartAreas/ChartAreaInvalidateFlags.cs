using System;

namespace Telerik.Charting
{
    /// <summary>
    /// Defines the possible impact a chart node property may have over the entire chart area.
    /// </summary>
    [Flags]
    internal enum ChartAreaInvalidateFlags
    {
        /// <summary>
        /// Property does not affect the chart element tree.
        /// </summary>
        None = 0,

        /// <summary>
        /// Invalidates the visual presentation of the axes.
        /// </summary>
        ResetAxes = 1,

        /// <summary>
        /// Resets the plot information of the axes.
        /// </summary>
        InvalidateAxes = ResetAxes << 1,

        /// <summary>
        /// Invalidates the visual presentation of the series.
        /// </summary>
        InvalidateSeries = InvalidateAxes << 1,

        /// <summary>
        /// Invalidates the visual presentation of the chart grid (if any).
        /// </summary>
        InvalidateGrid = InvalidateSeries << 1,

        /// <summary>
        /// Resets the plot information of the annotations.
        /// </summary>
        ResetAnnotations = InvalidateGrid << 1,

        /// <summary>
        /// Invalidates the visual presentation of the chart annotations (if any).
        /// </summary>
        InvalidateAnnotations = ResetAnnotations << 1,

        /// <summary>
        /// Invalidates each axis plus the chart grid (if any).
        /// </summary>
        InvalidateAxesAndGrid = InvalidateAxes | InvalidateGrid,

        /// <summary>
        /// All invalidate flags are specified.
        /// </summary>
        InvalidateAll = InvalidateAxes | InvalidateSeries | InvalidateGrid | InvalidateAnnotations,

        /// <summary>
        /// All flags are specified.
        /// </summary>
        All = InvalidateAll | ResetAxes | ResetAnnotations
    }
}
