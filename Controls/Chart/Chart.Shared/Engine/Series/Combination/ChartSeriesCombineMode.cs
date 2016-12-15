using System;

namespace Telerik.Charting
{
    /// <summary>
    /// Defines how multiple series of same type are combined on the plot area.
    /// </summary>
    public enum ChartSeriesCombineMode
    {
        /// <summary>
        /// No combining. Each series is plotted independently.
        /// </summary>
        None,

        /// <summary>
        /// Series are combined next to each other.
        /// </summary>
        Cluster,

        /// <summary>
        /// Series form stacks.
        /// </summary>
        Stack,

        /// <summary>
        /// Series for stacks that occupy 100% of the plot area.
        /// </summary>
        Stack100
    }
}
