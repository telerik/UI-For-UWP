namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Defines the different modes for item swipe direction
    /// <see cref="RadListView"/>.
    /// </summary>
    public enum ListViewItemSwipeDirection
    {
        /// <summary>
        /// Swipe is allowed from right to left in Vertical orientation, and from bottom to top in Horizontal orientation.
        /// </summary>
        Forward,

        /// <summary>
        /// Swipe is allowed from left to right in Vertical orientation, and from top to bottom in Horizontal orientation.
        /// </summary>
        Backwards,

        /// <summary>
        /// The swipe gesture is not restricted.
        /// </summary>
        All,
    }
}
