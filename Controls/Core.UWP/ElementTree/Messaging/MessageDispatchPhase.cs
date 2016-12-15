namespace Telerik.Core
{
    /// <summary>
    /// Defines the possible phases a message dispatch process may enter.
    /// </summary>
    public enum MessageDispatchPhase
    {
        /// <summary>
        /// Message is being dispatched up in the parent chain.
        /// </summary>
        Bubble,

        /// <summary>
        /// Message is being dispatched down to all descendants.
        /// </summary>
        Tunnel,
    }
}
