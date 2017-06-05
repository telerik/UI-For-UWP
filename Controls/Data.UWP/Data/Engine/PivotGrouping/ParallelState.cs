using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Telerik.Data.Core.Engine
{
    internal class ParallelState
    {
        public ParallelState()
        {
            this.RowGroupDescriptions = new List<GroupDescription>();
            this.ColumnGroupDescriptions = new List<GroupDescription>();
            this.AggregateDescriptions = new List<IAggregateDescription>();
            this.FilterDescriptions = new List<FilterDescription>();

            this.SourceGroups = new List<Tuple<object, int>>();
        }

        public IDataSourceView DataView
        {
            get;
            set;
        }

        public TaskScheduler TaskScheduler
        {
            get;
            set;
        }

        public CancellationTokenSource CancellationTokenSource
        {
            get;
            set;
        }

        public CancellationToken CancellationToken
        {
            get
            {
                return this.CancellationTokenSource.Token;
            }
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

        public CultureInfo Culture
        {
            get;
            set;
        }

        public int ItemCount
        {
            get
            {
                if (this.Items != null)
                {
                    return this.Items.Count;
                }

                return this.DataView.InternalList.Count;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return this.ItemCount == 0 || (!this.HasDescriptions && this.ValueProvider.GetSortComparer() == null);
            }
        }

        public bool HasDescriptions
        {
            get
            {
                return this.RowGroupDescriptions.Count > 0 || this.ColumnGroupDescriptions.Count > 0 || this.AggregateDescriptions.Count > 0 || this.FilterDescriptions.Count > 0;
            }
        }

        public IValueProvider ValueProvider
        {
            get;
            set;
        }

        public int MaxDegreeOfParallelism
        {
            get;
            set;
        }

        internal IReadOnlyList<object> Items
        {
            get;
            set;
        }
        
        internal List<Tuple<object, int>> SourceGroups
        {
            get;
            set;
        }

        internal object GetItem(int index)
        {
            if (this.Items != null)
            {
                return this.Items[index];
            }

            return this.DataView.InternalList[index];
        }

        internal object GetGroupFromIndex(int index)
        {
            for (int i = 0; i < this.SourceGroups.Count; i++)
            {
                if (index < this.SourceGroups[i].Item2)
                {
                    return this.SourceGroups[i].Item1;
                }
            }

            return null;
        }
    }
}