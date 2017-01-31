using System;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Defines the visibility of the major and minor lines within a <see cref="CartesianChartGrid"/>.
    /// </summary>
    [Flags]
    public enum GridLineVisibility
    {
        /// <summary>
        /// Lines are hidden.
        /// </summary>
        None = 0,

        /// <summary>
        /// The lines along the X-axis are visible.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X")]
        X = 1,

        /// <summary>
        /// The lines along the Y-axis are visible.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Y")]
        Y = X << 1,

        /// <summary>
        /// The lines are visible along both axes.
        /// </summary>
        XY = X | Y,
    }
}
