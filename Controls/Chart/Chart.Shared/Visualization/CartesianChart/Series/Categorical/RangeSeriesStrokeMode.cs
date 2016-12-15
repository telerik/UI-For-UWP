using System;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Defines how an <see cref="RangeSeries"/> shape is outlined.
    /// </summary>
    [Flags]
    public enum RangeSeriesStrokeMode
    {
        /// <summary>
        /// No outlining.
        /// </summary>
        None = 0,

        /// <summary>
        /// The path segment will have a stroke along the low values.
        /// </summary>
        LowPoints = 1,

        /// <summary>
        /// The path segment will have a stroke along the High values.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "HighPoints")]
        HighPoints = LowPoints << 1,

        /// <summary>
        /// The path segment will have a stroke along the low and high values.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "HighPoints")]
        LowAndHighPoints = LowPoints | HighPoints,
    }
}
