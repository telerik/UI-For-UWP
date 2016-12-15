using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid.Model;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class DataGridDataView : IDataView
    {
        private GridModel model;
        private RadDataGrid owner;

        internal DataGridDataView(RadDataGrid grid)
        {
            this.owner = grid;
            this.model = this.owner.Model;
        }

        public bool IsCurrentItemInView
        {
            get
            {
                return this.owner.CurrencyService.isCurrentInView;
            }
        }

        public bool IsDataReady
        {
            get
            {
                return this.model.IsDataReady;
            }
        }

        public IReadOnlyList<object> Items
        {
            get
            {
                if (!this.IsDataReady)
                {
                    return new List<object>();
                }

                var rootGroup = this.model.CurrentDataProvider.Results.Root.RowGroup as IDataGroup;
                if (rootGroup == null)
                {
                    return null;
                }

                return rootGroup.ChildItems;
            }
        }

        public object CurrentItem
        {
            get
            {
                return this.owner.CurrentItem;
            }
        }

        public object GetAggregateValue(int aggregateIndex, IDataGroup group)
        {
            if (!this.IsDataReady)
            {
                return false;
            }

            var coordinate = this.model.CurrentDataProvider.Results.Root;
            if (group != null)
            {
                coordinate.RowGroup = group as IGroup;
            }

            var value = this.model.CurrentDataProvider.Results.GetAggregateResult(aggregateIndex, coordinate);
            if (value != null)
            {
                return value.GetValue();
            }

            return null;
        }

        public IEnumerable GetAggregateValues(string propertyName, IDataGroup group)
        {
            if (!this.IsDataReady)
            {
                yield break;
            }

            for (int i = 0; i < this.model.AggregateDescriptors.Count; i++)
            {
                var propertyAggregate = this.model.AggregateDescriptors[i] as PropertyAggregateDescriptor;
                if (propertyAggregate == null)
                {
                    continue;
                }

                yield return this.GetAggregateValue(i, group);
            }
        }

        public bool MoveCurrentTo(object item)
        {
            return this.owner.CurrencyService.MoveCurrentTo(item);
        }

        public bool MoveCurrentToFirst()
        {
            return this.owner.CurrencyService.MoveCurrentToFirst();
        }

        public bool MoveCurrentToLast()
        {
            return this.owner.CurrencyService.MoveCurrentToLast();
        }

        public bool MoveCurrentToNext()
        {
            return this.owner.CurrencyService.MoveCurrentToNext();
        }

        public bool MoveCurrentToPrevious()
        {
            return this.owner.CurrencyService.MoveCurrentPrevious();
        }

        public bool GetIsExpanded(IDataGroup group)
        {
            if (!this.IsDataReady)
            {
                return false;
            }

            return !this.model.RowPool.Layout.IsCollapsed(group);
        }

        public void ExpandGroup(IDataGroup group)
        {
            if (!this.IsDataReady)
            {
                return;
            }

            this.ExpandRecursively(group);
        }

        public void CollapseGroup(IDataGroup group)
        {
            if (!this.IsDataReady)
            {
                return;
            }

            this.model.RowPool.Layout.Collapse(group);
        }

        public void ExpandAll()
        {
            if (this.IsDataReady)
            {
                foreach (var group in this.GetGroups(null))
                {
                    this.model.RowPool.Layout.Expand(group);
                }
            }
        }

        public void CollapseAll()
        {
            if (this.IsDataReady)
            {
                foreach (var group in this.GetGroups(null))
                {
                    this.model.RowPool.Layout.Collapse(group);
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            return new DataViewEnumerator(this, false);
        }

        public IEnumerable<IDataGroup> GetGroups(Predicate<IDataGroup> condition = null)
        {
            var en = new DataViewEnumerator(this, true);
            while (en.MoveNext())
            {
                var group = en.Current as IDataGroup;
                if (condition == null || condition(group))
                {
                    yield return group;
                }
            }
        }

        public void ExpandItem(object item)
        {
            var group = this.model.FindItemParentGroup(item);
            if (group != null)
            {
                this.ExpandRecursively(group);
            }
        }

        public void CollapseItem(object item)
        {
            var group = this.model.FindItemParentGroup(item);
            if (group != null)
            {
                this.model.RowPool.Layout.Collapse(group);
            }
        }

        public IDataGroup GetParentGroup(object item)
        {
            return this.model.FindItemParentGroup(item);
        }

        private void ExpandRecursively(IDataGroup group)
        {
            Stack<object> groups = new Stack<object>();
            var currentGroup = group;

            // we need to expand the parent chain starting from top-level groups, that's why we put them in a Stack first
            while (currentGroup != null)
            {
                groups.Push(currentGroup);
                currentGroup = currentGroup.ParentGroup;
            }

            while (groups.Count > 0)
            {
                this.model.RowPool.Layout.Expand(groups.Pop());
            }
        }

        private IEnumerable<IDataGroup> FindGroupsByKey(object key)
        {
            if (!this.IsDataReady)
            {
                yield break;
            }

            foreach (var group in this.model.RowPool.Layout.GetGroupsByKey(key))
            {
                yield return group as IDataGroup;
            }
        }

        private class DataViewEnumerator : IEnumerator
        {
            private DataGridDataView owner;
            private Stack<IEnumerator> enumerators;
            private bool isInitialized;
            private object current;
            private bool groupsOnly;

            public DataViewEnumerator(DataGridDataView owner, bool groupsOnly)
            {
                this.owner = owner;
                this.enumerators = new Stack<IEnumerator>();
                this.groupsOnly = groupsOnly;
            }

            public object Current
            {
                get
                {
                    return this.current;
                }
            }

            public void Reset()
            {
                this.isInitialized = false;
                this.current = null;
                this.enumerators.Clear();
            }

            public bool MoveNext()
            {
                if (this.owner.model.ItemsSource == null)
                {
                    return false;
                }

                if (!this.isInitialized)
                {
                    this.Initialize();
                }

                return this.MoveNextCore();
            }

            private void Initialize()
            {
                var root = this.owner.model.CurrentDataProvider.Results.Root;
                if (root != null && root.RowGroup != null)
                {
                    this.AddGroupEnumerator(root.RowGroup);
                }
                else
                {
                    this.enumerators.Push(this.owner.model.CurrentDataProvider.DataView.InternalList.GetEnumerator());
                }

                this.isInitialized = true;
            }

            private bool MoveNextCore()
            {
                this.current = null;

                if (this.enumerators.Count == 0)
                {
                    return false;
                }

                bool hasNext = false;

                while (this.enumerators.Count > 0)
                {
                    var enumerator = this.enumerators.Peek();
                    hasNext = enumerator.MoveNext();

                    if (hasNext)
                    {
                        this.current = enumerator.Current;

                        var group = this.current as IGroup;
                        if (group != null)
                        {
                            this.AddGroupEnumerator(group);
                        }

                        break;
                    }

                    this.enumerators.Pop();
                }

                return hasNext;
            }

            private void AddGroupEnumerator(IGroup group)
            {
                if (group.HasItems && (this.groupsOnly ? !group.IsBottomLevel : true))
                {
                    this.enumerators.Push(group.Items.GetEnumerator());
                }
            }
        }
    }
}
