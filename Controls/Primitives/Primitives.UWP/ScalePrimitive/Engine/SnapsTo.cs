namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines the possible snapping values for a <see cref="SliderBase"/> control instance.
    /// </summary>
    public enum SnapsTo
    {
        /// <summary>
        /// No snapping is applied.
        /// </summary>
        None,

        /// <summary>
        /// Snaps to <see cref="SliderBase.TickFrequency"/> property.
        /// </summary>
        Ticks,

        /// <summary>
        /// Snaps to <see cref="LargeChange"/> property.
        /// </summary>
        LargeChange,

        /// <summary>
        /// Snaps to <see cref="SmallChange"/> property.
        /// </summary>
        SmallChange
    }   
}
