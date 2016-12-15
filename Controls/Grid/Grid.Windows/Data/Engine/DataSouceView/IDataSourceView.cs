using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Telerik.Core.Data;

namespace Telerik.Data.Core
{
    internal interface IDataSourceView
    {
        event NotifyCollectionChangedEventHandler CollectionChanged;
        event PropertyChangedEventHandler ItemPropertyChanged;

        IList<object> InternalList { get; }
        BatchLoadingProvider<object> BatchDataProvider { get; }

        void Dispose();
    }
}
