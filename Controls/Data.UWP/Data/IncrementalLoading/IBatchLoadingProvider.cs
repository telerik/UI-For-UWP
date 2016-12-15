using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Core.Data
{
    internal interface IBatchLoadingProvider
    {
        event EventHandler<BatchLoadingEventArgs> StatusChanged;
        uint? BatchSize { get; }
        bool ShouldRequestItems(int lastRequestedIndex, int bufferSize);
        void RequestItems(int lastRequestedIndex, int bufferSize);
        void OnStatusChanged(BatchLoadingStatus status);
        void Dispose();
    }
}
