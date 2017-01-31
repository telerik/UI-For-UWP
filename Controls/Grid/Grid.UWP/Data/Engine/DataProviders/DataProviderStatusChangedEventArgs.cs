using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Event data for the <see cref="IDataProvider.StatusChanged"/> event of all <see cref="IDataProvider"/> types.
    /// </summary>
    internal class DataProviderStatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataProviderStatusChangedEventArgs" /> class.
        /// </summary>
        /// <param name="newStatus">The new status.</param>
        /// <param name="resultsChanges">DataProvider results have changed if set to <c>true</c>.</param>
        /// <param name="error">Exception if available .</param>
        public DataProviderStatusChangedEventArgs(DataProviderStatus newStatus, bool resultsChanges, Exception error)
        {
            this.NewStatus = newStatus;
            this.ResultsChanged = resultsChanges;
            this.Error = error;
        }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>The error.</value>
        public Exception Error
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the new status.
        /// </summary>
        /// <value>
        /// The new status.
        /// </value>
        public DataProviderStatus NewStatus
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the results of the data provider have changed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if results have changed; otherwise, <c>false</c>.
        /// </value>
        public bool ResultsChanged
        {
            get;
            private set;
        }
    }
}
