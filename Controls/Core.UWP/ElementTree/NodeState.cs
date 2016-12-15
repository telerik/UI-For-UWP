namespace Telerik.Core
{
    /// <summary>
    /// Defines the possible states a <see cref="Node"/> may enter.
    /// </summary>
    public enum NodeState
    {
        /// <summary>
        /// The node is created.
        /// </summary>
        Initial,

        /// <summary>
        /// The node is loading on the visual scene.
        /// </summary>
        Loading,

        /// <summary>
        /// The node is loaded and ready to be visualized.
        /// </summary>
        Loaded,

        /// <summary>
        /// The node is in a process of being unloaded from the visual scene.
        /// </summary>
        Unloading,

        /// <summary>
        /// The node is detached from the visual scene.
        /// </summary>
        Unloaded
    }
}
