namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Lists the possible item reordering directions when
    /// the collection changes and the viewport items need to be shifted.
    /// </summary>
    public enum CollectionChangeItemReorderMode
    {
        /// <summary>
        /// Moves the affected items down to make space for the new item.
        /// </summary>
        MoveItemsDown,

        /// <summary>
        /// Moves the affected items up to make space for the new item.
        /// </summary>
        MoveItemsUp
    }
}