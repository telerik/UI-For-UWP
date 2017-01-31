using System;

namespace Telerik.UI.Xaml.Controls.Primitives.LoopingList
{
    /// <summary>
    /// Defines the possible actions that may select an item within a LoopingList.
    /// </summary>
    internal enum LoopingListSelectionChangeReason
    {
        /// <summary>
        /// The selected index is changed by the list itself to reflect an internal change.
        /// </summary>
        Private,

        /// <summary>
        /// The selected index is changed by the list itself. The SelectedIndexChanged event is not raised.
        /// </summary>
        PrivateNoNotify,

        /// <summary>
        /// The selected index is changed by the user through code.
        /// </summary>
        User,

        /// <summary>
        /// A visual item was clicked.
        /// </summary>
        VisualItemClick,

        /// <summary>
        /// The item is in the middle of a LoopingPanel and automatically selected.
        /// </summary>
        VisualItemSnappedToMiddle,
    }
}
