namespace Telerik.Charting
{
    /// <summary>
    /// Determines the label fit mode of the chart axis labels.
    /// </summary>
    public enum AxisLabelFitMode
    {
        /// <summary>
        /// Does not attempt to fit the axis labels.
        /// </summary>
        None,

        /// <summary>
        /// Arranges axis labels on multiple lines with each label on a different line
        /// than its neighbor labels.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")]
        MultiLine,

        /// <summary>
        /// Arranges the axis labels so that they are rotated some degrees around their top left corner.
        /// </summary>
        Rotate,

        ///// <summary>
        ///// Arranges the axis labels by automatically choosing the best layout mode.
        ///// </summary>
        // Auto
    }
}
