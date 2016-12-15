namespace Telerik.Data.Core.Engine
{
    /// <summary>
    /// A data grouping status.
    /// </summary>
    internal enum DataEngineStatus
    {
        /// <summary>
        /// The data grouping has successfully completed grouping.
        /// </summary>
        Completed,

        /// <summary>
        /// The data grouping has failed.
        /// </summary>
        Faulted,

        /// <summary>
        /// The data result is working.
        /// </summary>
        InProgress
    }
}