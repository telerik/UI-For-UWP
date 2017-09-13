using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Telerik.Core.Data
{
    /// <summary>
    /// Represents a dynamic data collection that implements <see cref="IIncrementalBatchLoading"/> and simplifies its usage.
    /// </summary>
    public class IncrementalLoadingCollection<T> : ObservableCollection<T>, IIncrementalBatchLoading
    {
        private Func<uint, Task<IEnumerable<T>>> load;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncrementalLoadingCollection{T}" /> class.
        /// </summary>
        /// <param name="load">Specifies the callback used when more items are requested.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Provides simplified API for the users.")]
        public IncrementalLoadingCollection(Func<uint, Task<IEnumerable<T>>> load)
        {
            this.HasMoreItems = true;
            this.load = load;
        }

        /// <summary>
        /// Gets a value indicating whether more items could be loaded.
        /// </summary>
        /// <returns>True if additional unloaded items remain in the view; otherwise, false.
        /// </returns>
        public virtual bool HasMoreItems { get; private set; }

        /// <summary>
        /// Gets or sets the data batch size that will be requested.
        /// </summary>
        /// <value>The size of the batch.</value>
        public uint? BatchSize { get; set; }

        /// <summary>Initializes incremental loading from the view.</summary>
        /// <returns>The wrapped results of the load operation.</returns>
        /// <param name="count">The number of items to load.</param>
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return AsyncInfo.Run(async c =>
            {
                IEnumerable<T> data = null;

                var task = load(count);

                if (task != null)
                {
                    try
                    {
                        data = await task;

                        HasMoreItems = data != null && data.Any();

                        if (data != null)
                        {
                            foreach (var item in data)
                            {
                                Add(item);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        this.HasMoreItems = false;
                        throw e;
                    }
                }

                return new LoadMoreItemsResult
                {
                    Count = data != null ? (uint)data.Count() : 0,
                };
            });
        }
    }
}
