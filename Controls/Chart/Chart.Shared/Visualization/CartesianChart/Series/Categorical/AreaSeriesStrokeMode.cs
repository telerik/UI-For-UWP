using System;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Defines how an <see cref="AreaSeries"/> shape is outlined.
    /// </summary>
    [Flags]
    public enum AreaSeriesStrokeMode
    {
        /// <summary>
        /// No outlining.
        /// </summary>
        None = 0,

        /// <summary>
        /// The left line (from plotline to the first point) is outlined.
        /// </summary>
        LeftLine = 1,

        /// <summary>
        /// The line that connects all points is outlined. This is the default mode.
        /// </summary>
        Points = LeftLine << 1,

        /// <summary>
        /// The right line (from plotline to the last point) is outlined.
        /// </summary>
        RightLine = Points << 1,

        /// <summary>
        /// The plotline is outlines.
        /// </summary>
        PlotLine = RightLine << 1,

        /// <summary>
        /// Left line and points are outlined.
        /// </summary>
        LeftAndPoints = LeftLine | Points,

        /// <summary>
        /// Right line and points are outlined.
        /// </summary>
        RightAndPoints = Points | RightLine,

        /// <summary>
        /// All members except the PlotLine are specified.
        /// </summary>
        AllButPlotLine = LeftLine | Points | RightLine,

        /// <summary>
        /// All enumeration members are defined and the area is fully outlined.
        /// </summary>
        All = LeftLine | Points | RightLine | PlotLine,
    }
}
