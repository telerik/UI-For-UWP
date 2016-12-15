using System;

namespace Telerik.Core
{
    /// <summary>
    /// Defines how a message should be dispatched in the element tree.
    /// </summary>
    [Flags]
    public enum MessageDispatchMode
    {
        /// <summary>
        /// Message is dispatched to the direct target and its ancestors.
        /// </summary>
        Bubble = 1,

        /// <summary>
        /// Message is dispatched to the direct target all its descendants.
        /// </summary>
        Tunnel = Bubble << 1,

        /// <summary>
        /// Message is dispatched to the direct target, its ancestors and all its descendants.
        /// </summary>
        BubbleAndTunnel = Bubble | Tunnel,
    }
}
