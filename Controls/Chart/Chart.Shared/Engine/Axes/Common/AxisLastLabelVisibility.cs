using System;

namespace Telerik.Charting
{
    /// <summary>
    /// Defines the modes for displaying the last label of the axis.
    /// </summary>
    public enum AxisLastLabelVisibility
    {
        /// <summary>
        /// The desired space is reserved so that the label is fully visible.
        /// </summary>
        Visible,

        /// <summary>
        /// The last label is not displayed.
        /// </summary>
        Hidden,

        /// <summary>
        /// The last label is displayed but no space is reserved for the label and it could not be fully visible.
        /// </summary>
        Clip,
    }
}
