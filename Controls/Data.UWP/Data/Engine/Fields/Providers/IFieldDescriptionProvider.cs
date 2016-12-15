using System;

namespace Telerik.Data.Core.Fields
{
    /// <summary>
    /// Handles creation and lookup of <see cref="IDataFieldInfo"/> instances.
    /// </summary>
    internal interface IFieldDescriptionProvider
    {
        /// <summary>
        /// Occurs when an asynchronous GetDescriptionsData operation completes.
        /// </summary>
        event EventHandler<GetDescriptionsDataCompletedEventArgs> GetDescriptionsDataAsyncCompleted;

        /// <summary>
        /// Gets a value indicating whether a GetDescriptionsData request is in progress.
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        /// Gets a value indicating whether the provider is properly initialized and ready to be used.
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Retrieves information about all available field descriptions.
        /// This method does not block the calling thread.
        /// </summary>
        void GetDescriptionsDataAsync(object state);
    }
}