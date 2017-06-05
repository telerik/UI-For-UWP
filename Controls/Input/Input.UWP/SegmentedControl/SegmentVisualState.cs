using System;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Defines the supported visual states for the <see cref="Segment"/>.
    /// </summary>
    [Flags]
    public enum SegmentVisualState
    {
        /// <summary>
        /// Specifies a "Normal" visual state.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// Specifies a "PointerOver" visual state.
        /// </summary>
        PointerOver = 2,

        /// <summary>
        /// Specifies a "Pressed" visual state.
        /// </summary>
        Pressed = 4,

        /// <summary>
        /// Specifies a "Selected" visual state.
        /// </summary>
        Selected = 8,

        /// <summary>
        /// Specifies a "Disabled" visual state.
        /// </summary>
        Disabled = 16,

        /// <summary>
        /// Specifies a "SelectedPointerOver" visual state.
        /// </summary>
        SelectedPointerOver = Selected | PointerOver,

        /// <summary>
        /// Specifies a "SelectedPressed" visual state.
        /// </summary>
        SelectedPressed = Selected | Pressed,

        /// <summary>
        /// Specifies a "SelectedDisabled" visual state.
        /// </summary>
        SelectedDisabled = Selected | Disabled
    }
}