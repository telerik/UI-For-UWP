namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Defines the available modes to process user input that affects the <see cref="RadMap.ZoomLevel"/> property.
    /// </summary>
    public enum MapZoomMode
    {
        /// <summary>
        /// The map is zoomed to the contact point with the primary pointer that triggered the input.
        /// </summary>
        ZoomToPoint,

        /// <summary>
        /// The map is zoomed its center regardless of the primary pointer that triggered the input.
        /// </summary>
        ZoomToCenter,

        /// <summary>
        /// The map may not be zoomed through user input.
        /// </summary>
        None
    }
}
