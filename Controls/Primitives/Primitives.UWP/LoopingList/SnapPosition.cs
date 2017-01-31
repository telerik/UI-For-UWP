using System;

namespace Telerik.UI.Xaml.Controls.Primitives.LoopingList
{
    /// <summary>
    /// Defines the different snapping positions for the centered item
    /// supported by <see cref="RadLoopingList"/>.
    /// </summary>
    public enum LoopingListItemSnapPosition
    {
        /// <summary>
        /// Positions the selected item in the center of the viewport.
        /// </summary>
        Middle,

        /// <summary>
        /// Positions the selected item at the near side of the viewport.
        /// </summary>
        Near,

        /// <summary>
        /// Positions the selected item at the far side of the viewport.
        /// </summary>
        Far
    }
}
