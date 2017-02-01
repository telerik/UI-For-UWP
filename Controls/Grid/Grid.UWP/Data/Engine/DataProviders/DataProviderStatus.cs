namespace Telerik.Data.Core
{
    /// <summary>
    /// An <see cref="IDataProvider"/> status.
    /// </summary>
    internal enum DataProviderStatus
    {
        /// <summary>
        /// The provider is in uninitialized state.
        /// </summary>
        Uninitialized,

        /// <summary>
        /// The provider is initializing.
        /// </summary>
        Initializing,

        /// <summary>
        /// The provider has extracted field descriptions.
        /// </summary>
        DescriptionsReady,

        /// <summary>
        /// The provider is ready for data processing.
        /// </summary>
        Ready,

        /// <summary>
        /// The provider is currently processing data.
        /// </summary>
        ProcessingData,

        /// <summary>
        /// The provider is currently requesting data.
        /// </summary>
        RequestingData,

        /// <summary>
        /// The provider has requested data.
        /// </summary>
        DataLoadingCompleted,

        /// <summary>
        /// The provider has failed.
        /// </summary>
        Faulted
    }
}