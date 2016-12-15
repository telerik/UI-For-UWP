using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines the triggers that opens the <see cref="RadRadialMenu"/>.
    /// </summary>
    [Flags]
    public enum RadialMenuAttachTriggers
    {
        /// <summary>
        /// <see cref="RadRadialMenu"/> cannot be opened with an automatic trigger.
        /// </summary>
        None = 1,

        /// <summary>
        /// <see cref="RadRadialMenu"/> is opened when pointer press event occurs on the target element.
        /// </summary>
        PointerPressed = None << 1,

        /// <summary>
        /// <see cref="RadRadialMenu"/> is opened when pointer over event occurs on the target element.
        /// </summary>
        PointerOver = PointerPressed << 1,

        /// <summary>
        /// <see cref="RadRadialMenu"/> is opened when the target element gets focus.
        /// </summary>
        Focused = PointerOver << 1,

        /// <summary>
        /// <see cref="RadRadialMenu"/> is opened when the target element gets focus or press action.
        /// </summary>
        PressedOrFocused = PointerPressed | Focused
    }
}
