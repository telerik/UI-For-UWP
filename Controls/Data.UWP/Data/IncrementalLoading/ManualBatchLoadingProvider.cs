using System;
using System.Collections.Generic;
namespace Telerik.Core.Data
{
    internal class ManualBatchLoadingProvider<T> : IDisposable, IBatchLoadingProvider
    {
        private ICollection<T> loadedItemsCollection;
        public ManualBatchLoadingProvider(ICollection<T> loadedItemsCollection)
        {
            this.loadedItemsCollection = loadedItemsCollection;
        }

        public event EventHandler<BatchLoadingEventArgs> StatusChanged;
        public uint? BatchSize { get; private set; }

        public bool ShouldRequestItems(int lastRequestedIndex, int bufferSize)
        {
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
            var handler = this.StatusChanged;
            if (handler != null)
            {
                handler(this, new BatchLoadingEventArgs(status));
            }
        }
    }
}
