using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Telerik.Data.Core.Engine
{
    /// <summary>
    /// Signals that data operation has completed, been canceled or an error occurred.
    /// </summary>
    internal class DataEngineCompletedEventArgs : EventArgs
    {
        internal DataEngineCompletedEventArgs(ReadOnlyCollection<Exception> innerExceptions, DataEngineStatus status)
        {
            this.InnerExceptions = innerExceptions;
            this.Status = status;

            if (this.InnerExceptions == null)
            {
                this.InnerExceptions = new ReadOnlyCollection<Exception>(new List<Exception>());
            }
        }

        /// <summary>
        /// Gets the completion status.
        /// </summary>
        public DataEngineStatus Status { get; private set; }

        /// <summary>
        /// Gets a read-only collection of any Exceptions thrown during a data grouping.
        /// </summary>
        public ReadOnlyCollection<Exception> InnerExceptions { get; private set; }
    }
}