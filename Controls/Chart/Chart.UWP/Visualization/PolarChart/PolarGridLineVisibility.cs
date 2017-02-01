namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Defines the availability of the grid lines within a PolarChartGrid.
    /// </summary>
    public enum PolarGridLineVisibility
    {
        /// <summary>
        /// No grid lines are displayed.
        /// </summary>
        None = 0,

        /// <summary>
        /// The Polar (radius) lines are visible.
        /// </summary>
        Polar = 1,

        /// <summary>
        /// The Radial (angle) lines (ellipses) are visible.
        /// </summary>
        Radial = Polar << 1,

        /// <summary>
        /// Both <see cref="Polar"/> and <see cref="Radial"/> lines are visible.
        /// </summary>
        Both = Polar | Radial
    }
}
