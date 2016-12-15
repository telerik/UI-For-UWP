namespace Telerik.Core.Data
{
    /// <summary>
    /// Defines possible loading modes of data.
    /// </summary>
    public enum BatchLoadingMode
    {
        /// <summary>
        /// Data is Loaded automatically (when possible) when a specific case requested.
        /// </summary>
        Auto,

        /// <summary>
        /// Data is loaded when explicit request is made.
        /// </summary>
        Explicit
    }
}
