namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines the possible thumb move modes for a <see cref="SliderBase"/> control instance.
    /// </summary>
    public enum RangeSliderTrackTapMode
    {
        /// <summary>
        /// No thumb move when track is tapped.
        /// </summary>
        None,

        /// <summary>
        /// Moves to the current tap position.
        /// </summary>
        MoveToTapPosition,

        /// <summary>
        /// Increment by <see cref="RangeInputBase.LargeChange"/> property.
        /// </summary>
        IncrementByLargeChange,

        /// <summary>
        /// Increment by <see cref="RangeInputBase.SmallChange"/> property.
        /// </summary>
        IncrementBySmallChange
    }   
}
