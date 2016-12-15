using System;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines the visibility of the horizontal and vertical lines that enclose a cell within a tabular component (e.g. RadDataGrid, RadCalendar, etc.).
    /// </summary>
    [Flags]
    public enum GridLinesVisibility
    {
        /// <summary>
        /// No grid lines are visible.
        /// </summary>
        None = 0,

        /// <summary>
        /// The horizontal grid lines are visible.
        /// </summary>
        Horizontal = 1,

        /// <summary>
        /// The vertical grid lines are visible.
        /// </summary>
        Vertical = Horizontal << 1,

        /// <summary>
        /// Both horizontal and vertical grid lines are visible.
        /// </summary>
        Both = Horizontal | Vertical
    }
}
