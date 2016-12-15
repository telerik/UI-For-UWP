using System;

namespace Telerik.Core.Data
{
    internal class BatchLoadingEventArgs : EventArgs
    {
        public BatchLoadingEventArgs(BatchLoadingStatus status)
            : base()
        {
            this.Status = status;
        }

        public BatchLoadingStatus Status { get; private set; }
    }
}
