namespace Telerik.Core.Data
{
    /// <summary>
    /// Defines the supported loading status of the BatchDataProvider.
    /// </summary>
    public enum BatchLoadingStatus
    {
        /// <summary>
        /// Specifies an "ItemsRequested" batch loading status.
        /// </summary>
        ItemsRequested,

        /// <summary>
        /// Specifies an "ItemsLoaded" batch loading status.
        /// </summary>
        ItemsLoaded,

        /// <summary>
        /// Specifies an "ItemsLoadFailed" batch loading status.
        /// </summary>
        ItemsLoadFailed
    }
}
