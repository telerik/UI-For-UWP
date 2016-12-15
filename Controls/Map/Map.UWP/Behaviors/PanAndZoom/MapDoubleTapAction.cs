namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Defines the available actions that <see cref="MapPanAndZoomBehavior"/> can perform on double tap gesture.
    /// </summary>
    public enum MapDoubleTapAction
    {
        /// <summary>
        /// The map is reset to initial state (<see cref="RadMap.Center"/> and <see cref="RadMap.ZoomLevel"/> properties are reset to initial values).
        /// </summary>
        Reset,

        /// <summary>
        /// The map is zoomed to the contact point with the primary pointer that triggered the input.
        /// </summary>
        ZoomToPoint,

        /// <summary>
        /// The map does not handle the double tap gesture.
        /// </summary>
        None
    }
}
