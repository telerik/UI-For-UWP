using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Telerik.Data.Core;
using Telerik.Data.Core.Engine;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    internal class PivotEngineMock : IDataEngine
    {
        public PivotEngineMock()
        {
            this.ActionOnRebuildCube = (ps) => { };
        }

        public void SetToReturnResponse()
        {
            this.ActionOnRebuildCube = (ps) =>
            {
                if (this.Completed != null)
                {
                    this.Completed(this, new DataEngineCompletedEventArgs(null, DataEngineStatus.Completed));
                }
            };
        }

        public void SetToReturnErrorResponse()
        {
            this.ActionOnRebuildCube = (ps) =>
            {
                if (this.Completed != null)
                {
                    var exceptions = new List<Exception>() { new Exception("dummy exception")};
                    var readOnlyExceptions = new ReadOnlyCollection<Exception>(exceptions);

                    this.Completed(this, new DataEngineCompletedEventArgs(readOnlyExceptions, DataEngineStatus.Completed));
                }
            };
        }

        public event EventHandler<DataEngineCompletedEventArgs> Completed;

        public Action<ParallelState> ActionOnRebuildCube
        {
            get;
            set;
        }

        public Coordinate Root
        {
            get;
            set;
        }

        public IReadOnlyList<GroupDescription> RowGroupDescriptions
        {
            get;
            set;
        }

        public IReadOnlyList<GroupDescription> ColumnGroupDescriptions
        {
            get;
            set;
        }

        public IReadOnlyList<IAggregateDescription> AggregateDescriptions
        {
            get;
            set;
        }

        public IReadOnlyList<FilterDescription> FilterDescriptions
        {
            get;
            set;
        }

        public virtual AggregateValue GetAggregateResult(int aggregateIndex, IGroup row, IGroup column)
        {
            return null;
        }

        public virtual AggregateValue GetAggregateResult(int aggregateIndex, Coordinate coordinate)
        {
            return null;
        }

        public virtual void RebuildCube(ParallelState state)
        {
            this.ActionOnRebuildCube(state);
        }

        public virtual void Clear(ParallelState state)
        {

        }

        public virtual void CancelCurrentProcessing()
        {

        }

        public virtual void WaitForParallel()
        {

        }

        public virtual void RebuildCubeParallel(ParallelState state)
        {
            this.ActionOnRebuildCube(state);
        }

        public void RaiseCompletedEvent(DataEngineCompletedEventArgs args)
        {
            if (this.Completed != null)
            {
                this.Completed(this, args);
            }
        }

        public IEnumerable<object> GetUniqueKeys(DataAxis axis, int groupDescriptionIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetUniqueFilterItems(int filterIndex)
        {
            throw new NotImplementedException();
        }


        public List<AddRemoveResult> InsertItems(int index, System.Collections.IEnumerable items)
        {
            throw new NotImplementedException();
        }

        public List<AddRemoveResult> RemoveItems(int index, System.Collections.IEnumerable items, bool removeFilteredItem, bool canUseComparer)
        {
            throw new NotImplementedException();
        }

        public int GetFilteredItemIndex(object item)
        {
            throw new NotImplementedException();
        }
    }
}
