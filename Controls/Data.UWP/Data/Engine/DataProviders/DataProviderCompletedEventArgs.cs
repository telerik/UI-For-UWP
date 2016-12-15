using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Event data for the Completed event of all <see cref="IDataProvider"/> types.
    /// </summary>
    internal class DataProviderCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataProviderCompletedEventArgs" /> class.
        /// </summary>
        /// <param name="error">The error.</param>
        public DataProviderCompletedEventArgs(Exception error)
        {
            this.Error = error;
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>The error.</value>
        public Exception Error { get; private set; }
    }
}