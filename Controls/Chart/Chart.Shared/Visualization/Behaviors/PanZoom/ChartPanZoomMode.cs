using System;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Defines how a <see cref="RadChartBase"/> instance will handle a zoom or a pan gesture.
    /// </summary>
    [Flags]
    public enum ChartPanZoomMode
    {
        /// <summary>
        /// A zoom/pan gesture is not handled.
        /// </summary>
        None = 0,

        /// <summary>
        /// The chart is zoomed/panned horizontally (along the X-axis).
        /// </summary>
        Horizontal = 1,

        /// <summary>
        /// The chart is zoomed/panned vertically (along the Y-axis).
        /// </summary>
        Vertical = Horizontal << 1,

        /// <summary>
        /// Both Horizontal and Vertical flags are valid.
        /// </summary>
        Both = Horizontal | Vertical
    }
}
