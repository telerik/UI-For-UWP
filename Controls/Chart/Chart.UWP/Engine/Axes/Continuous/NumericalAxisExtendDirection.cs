using System;

namespace Telerik.Charting
{
    /// <summary>
    /// Specifies how a numerical axis auto-range will be extended so that each data point is visualized in the best possible way.
    /// </summary>
    [Flags]
    public enum NumericalAxisRangeExtendDirection
    {
        /// <summary>
        /// The range minimum is the minimum data point value and the range maximum is the maximum data point value.
        /// </summary>
        None = 0,

        /// <summary>
        /// The range maximum will be extended (if necessary) with one major step.
        /// </summary>
        Positive = 1,

        /// <summary>
        /// The range minimum will be extended (if necessary) with one major step.
        /// </summary>
        Negative = Positive << 1,

        /// <summary>
        /// The range will be extended in both directions by one major tick step.
        /// </summary>
        Both = Positive | Negative
    }
}
