using System;
using System.Collections.Generic;

namespace Telerik.Data.Core.Engine
{
    internal interface IDataEngine : IDataResults
    {
        event EventHandler<DataEngineCompletedEventArgs> Completed;

        void RebuildCube(ParallelState state);

        void RebuildCubeParallel(ParallelState state);

        void Clear(ParallelState state);

        void WaitForParallel();

        List<AddRemoveResult> InsertItems(int index, System.Collections.IEnumerable items);

        List<AddRemoveResult> RemoveItems(int index, System.Collections.IEnumerable items, bool removeFilteredItem, bool canUseComparer);

        int GetFilteredItemIndex(object item);
    }
}