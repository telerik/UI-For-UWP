using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Telerik.Core.Data
{
    internal class BatchLoadingProvider<T> : IDisposable, IBatchLoadingProvider
    {
        private ISupportIncrementalLoading incrementalLoadingSource;
        private ICollection<T> loadedItemsCollection;
        private IAsyncOperation<LoadMoreItemsResult> loadItemsOperation;

        public BatchLoadingProvider(ISupportIncrementalLoading incrementalLoadingSource, ICollection<T> loadedItemsCollection)
        {
            if (incrementalLoadingSource == null)
            {
                throw new ArgumentException("The batch loading provider requires ISupportIncrementalLoading to operate!");
            }

            this.incrementalLoadingSource = incrementalLoadingSource;
            this.loadedItemsCollection = loadedItemsCollection;

            var incrementalBatchLoadingSource = incrementalLoadingSource as IIncrementalBatchLoading;
            if (incrementalBatchLoadingSource != null)
            {
                this.BatchSize = incrementalBatchLoadingSource.BatchSize;
            }
        }

        public event EventHandler<BatchLoadingEventArgs> StatusChanged;

        public uint? BatchSize { get; private set; }

        public bool ShouldRequestItems(int lastRequestedIndex, int bufferSize)
        {
            return lastRequestedIndex >= this.loadedItemsCollection.Count - bufferSize &&
                   (this.loadItemsOperation == null || this.loadItemsOperation.Status != AsyncStatus.Started) &&
                   this.incrementalLoadingSource.HasMoreItems;
        }

        public void RequestItems(int lastRequestedIndex, int bufferSize)
        {
            if (this.ShouldRequestItems(lastRequestedIndex, bufferSize))
            {
                this.OnStatusChanged(BatchLoadingStatus.ItemsRequested);

                uint countToRequest = this.BatchSize ?? (uint)(lastRequestedIndex - this.loadedItemsCollection.Count + 1);

                if (countToRequest == 0)
                {
                    return;
                }

                this.loadItemsOperation = this.incrementalLoadingSource.LoadMoreItemsAsync(countToRequest);

                if (this.loadItemsOperation != null)
                {
                    this.loadItemsOperation.Completed += (s, e) =>
                    {
                        if (this.loadItemsOperation.Status == AsyncStatus.Completed)
                        {
                            this.OnStatusChanged(BatchLoadingStatus.ItemsLoaded);
                        }
                        else if (this.loadItemsOperation.Status == AsyncStatus.Error || this.loadItemsOperation.Status == AsyncStatus.Canceled)
                        {
                            this.OnStatusChanged(BatchLoadingStatus.ItemsLoadFailed);
                        }
                    };
                }
            }
        }

        public void Dispose()
        {
            if (this.loadItemsOperation != null)
            {
                this.loadItemsOperation.Cancel();
            }
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