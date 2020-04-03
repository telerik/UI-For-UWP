using System;
using System.Collections.Generic;
namespace Telerik.Core.Data
{
    internal class ManualBatchLoadingProvider<T> : IDisposable, IBatchLoadingProvider
    {
        private ICollection<T> loadedItemsCollection;
        private BatchLoadingStatus? status;

        public ManualBatchLoadingProvider(ICollection<T> loadedItemsCollection)
        {
            this.loadedItemsCollection = loadedItemsCollection;
        }

        public event EventHandler<BatchLoadingEventArgs> StatusChanged;
        public uint? BatchSize { get; private set; }

        public bool ShouldRequestItems(int lastRequestedIndex, int bufferSize)
        {
            if (this.status == BatchLoadingStatus.ItemsRequested)
            {
                return false;
            }

            return lastRequestedIndex >= this.loadedItemsCollection.Count - bufferSize;
        }

        public void RequestItems(int lastRequestedIndex, int bufferSize)
        {
        }

        public void Dispose()
        {
        }

        public void OnStatusChanged(BatchLoadingStatus status)
        {
            this.status = status;

            var handler = this.StatusChanged;
            if (handler != null)
            {
                handler(this, new BatchLoadingEventArgs(status));
            }
        }
    }
}
