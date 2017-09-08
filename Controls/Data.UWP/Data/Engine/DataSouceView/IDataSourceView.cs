using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Telerik.Core.Data;

namespace Telerik.Data.Core
{
    internal interface IDataSourceView
    {
        event NotifyCollectionChangedEventHandler CollectionChanged;
        event NotifyCollectionChangedEventHandler CollectionChanging;
        event PropertyChangedEventHandler ItemPropertyChanged;

        IList<object> InternalList { get; }

        IBatchLoadingProvider BatchDataProvider { get; }

        List<Tuple<object, int, int>> SourceGroups { get; }

        void ProcessPendingCollectionChange();

        void Dispose();
        object GetGroupKey(object item, int level);
    }
}