namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents the possible item virtualization states.
    /// </summary>
    public enum ItemState
    {
        /// <summary>
        /// The visual item is about to be recycled for further use.
        /// </summary>
        Recycling,

        /// <summary>
        /// The visual item has been recycled.
        /// </summary>
        Recycled,

        /// <summary>
        /// The visual item is about to be bound to a data item.
        /// </summary>
        Realizing,

        /// <summary>
        /// The visual item has been bound to a data item.
        /// </summary>
        Realized
    }
}
