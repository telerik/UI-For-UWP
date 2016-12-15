using System;

namespace Telerik.Charting
{
    /// <summary>
    /// Defines how data points are plotted relatively to the ticks of the axis.
    /// </summary>
    public enum AxisPlotMode
    {
        /// <summary>
        /// Points are plotted in the middle of the range, defined by two ticks.
        /// </summary>
        BetweenTicks,

        /// <summary>
        /// Points are plotted over each tick.
        /// </summary>
        OnTicks,

        /// <summary>
        /// Points are plotted over each tick with a half tick step padding applied on both ends of the axis.
        /// </summary>
        OnTicksPadded
    }
}
