namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Specifies the point distance calculation method.
    /// </summary>
    public enum ChartPointDistanceCalculationMode
    {
        /// <summary>
        /// Point distance is calculated based on a single dimension (either x or y based on plot direction).
        /// </summary>
        Linear,

        /// <summary>
        /// Point distance is calculated based on both dimensions (based on the coordinate system).
        /// </summary>
        TwoDimensional
    }
}
