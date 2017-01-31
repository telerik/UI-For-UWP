using System;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Defines how grid lines are displayed.
    /// </summary>
    [Flags]
    public enum GridLineRenderMode
    {
        /// <summary>
        /// First line is rendered.
        /// </summary>
        First = 1,

        /// <summary>
        /// Inner lines are rendered.
        /// </summary>
        Inner = First << 1,

        /// <summary>
        /// Last line is rendered.
        /// </summary>
        Last = Inner << 1,

        /// <summary>
        /// First and inner lines are rendered.
        /// </summary>
        FirstAndInner = First | Inner,

        /// <summary>
        /// Inner and last lines are rendered.
        /// </summary>
        InnerAndLast = Inner | Last,

        /// <summary>
        /// First and last lines are rendered.
        /// </summary>
        FirstAndLast = First | Last,

        /// <summary>
        /// All lines are rendered.
        /// </summary>
        All = First | Inner | Last
    }
}
