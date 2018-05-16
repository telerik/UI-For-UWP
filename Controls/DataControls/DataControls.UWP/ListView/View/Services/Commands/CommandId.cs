namespace Telerik.UI.Xaml.Controls.Data.ListView.Commands
{
    /// <summary>
    /// Defines the known commands that are available within a <see cref="RadListView"/> control.
    /// </summary>
    public enum CommandId
    {
        /// <summary>
        /// The command is not familiar to the <see cref="RadListView"/> control.
        /// </summary>
        Unknown,

        /// <summary>
        /// A command associated with requesting and loading more data when available that occurs within a <see cref="RadListView"/> instance.
        /// </summary>
        LoadMoreData,

        /// <summary>
        /// A command associated with the Tap event that occurs over an item.
        /// </summary>
        ItemTap,

        /// <summary>
        /// A command associated with the starting a drag action on an item.
        /// </summary>
        ItemDragStarting,

        /// <summary>
        /// A command associated with the completion of a item reorder action on an item.
        /// </summary>
        ItemReorderComplete,

        /// <summary>
        /// A command associated with the swiping action on an item.
        /// </summary>
        ItemSwiping,

        /// <summary>
        /// A command associated with the completion of a item swipe action on an item.
        /// </summary>
        ItemSwipeActionComplete,

        /// <summary>
        /// A command associated with the Tap event that occurs on an action item.
        /// </summary>
        ItemActionTap,

        /// <summary>
        /// A command associated with the Pull to refresh that occurs when swipe gesture is executed.
        /// </summary>
        RefreshRequested,

        /// <summary>
        /// A command associated with the Hold event that occurs over an item.
        /// </summary>
        ItemHold,

        /// <summary>
        /// A command associated with the Tap event that occurs over a group header.
        /// </summary>
        GroupHeaderTap
    }
}
