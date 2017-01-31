namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines how specific visual items are positioned within a <see cref="ScalePrimitive"/> control.
    /// </summary>
    public enum ScaleElementPlacement
    {
        /// <summary>
        /// Invisible visual.
        /// </summary>
        None,

        /// <summary>
        /// Positioned on top if the control has horizontal orientation and at left if the orientation is vertical.
        /// </summary>
        TopLeft,

        /// <summary>
        /// Positioned at bottom if the control has horizontal orientation and at right if the orientation is vertical.
        /// </summary>
        BottomRight,

        /// <summary>
        /// Positioned in the center.
        /// </summary>
        Center,
    }
}
