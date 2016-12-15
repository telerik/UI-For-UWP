using System;
using System.Collections;
using System.Collections.Generic;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Grid;

namespace Telerik.Data.Core.Layouts
{
    internal abstract class BaseLayout
    {
        private double defaultItemLength;

        public BaseLayout()
        {
            this.LayoutStrategies = new SortedSet<LayoutStrategyBase>(new LayoutStrategyComparer());
        }

        public event EventHandler<ExpandCollapseEventArgs> Collapsed;

        public event EventHandler<ExpandCollapseEventArgs> Expanded;

        public event EventHandler ItemsSourceChanged;

        public SortedSet<LayoutStrategyBase> LayoutStrategies { get; private set; }

        public abstract IRenderInfo RenderInfo
        {
            get;
        }

        public abstract int GroupCount { get; }

        public abstract int VisibleLineCount
        {
            get;
        }

        public abstract int TotalLineCount
        {
            get;
        }

        public double DefaultItemLength
        {
            get
            {
                return this.defaultItemLength;
            }
            set
            {
                this.defaultItemLength = value;
                this.RefreshRenderInfo(true);
            }
        }

        // source
        internal int TotalsCount
        {
            get;
            private set;
        }

        internal int AggregatesLevel
        {
            get;
            private set;
        }

        internal TotalsPosition TotalsPosition
        {
            get;
            private set;
        }

        internal bool ShowAggregateValuesInline
        {
            get;
            private set;
        }

        internal IReadOnlyList<object> ItemsSource
        {
            get;
            private set;
        }

        internal int GroupLevels
        {
            get;
            private set;
        }

        public abstract IEnumerable<Group> GetGroupsByKey(object key);

        public void SetSource(IEnumerable source)
        {
            this.SetSource(source, int.MaxValue, TotalsPosition.None, -1, 0, false, false);
        }

        public void SetSource(IEnumerable source, int groupLevels, TotalsPosition totalsPosition, int aggregatesLevel, int totalsCount, bool showAggregateValuesInline)
        {
            this.SetSource(source, groupLevels, totalsPosition, aggregatesLevel, totalsCount, showAggregateValuesInline, false);
        }

        // TODO: Too many arguments, pass a context instead
        public void SetSource(IEnumerable source, int groupLevels, TotalsPosition totalsPosition, int aggregatesLevel, int totalsCount, bool showAggregateValuesInline, bool restoreCollapsed)
        {
            if ((aggregatesLevel <= -1 || aggregatesLevel >= groupLevels) && totalsCount > 1)
            {
                aggregatesLevel = Math.Max(0, groupLevels - 1);
            }
            else if (totalsCount <= 1)
            {
                aggregatesLevel = 0;
            }

            this.GroupLevels = groupLevels;

            this.TotalsPosition = totalsPosition;
            this.AggregatesLevel = aggregatesLevel;
            this.TotalsCount = totalsCount;
            this.ShowAggregateValuesInline = showAggregateValuesInline;

            this.SetItemsSource(source);
            this.SetItemsSourceOverride(this.ItemsSource, restoreCollapsed);

            this.RaiseItemsSourceChanged(EventArgs.Empty);
        }

        // Expand/Collapse API
        public abstract bool IsCollapsed(object item);

        public abstract void Expand(object item);

        public abstract void Collapse(object item);

        public abstract IEnumerable<IList<ItemInfo>> GetLines(int start, bool forward);

        // TODO: Extract different layouts for the different controls so that this could move in something like pivot TabularLayout...
        internal static GroupType GetItemType(object item)
        {
            IGroup group = item as IGroup;
            if (group != null)
            {
                return group.Type;
            }

            return GroupType.BottomLevel;
        }

        internal abstract GroupInfo GetGroupInfo(object item);

        internal abstract int GetVisibleSlot(int index);
        internal abstract int GetCollapsedSlotsCount(int startSlot, int endSlot);
        internal abstract int GetNextVisibleSlot(int slot);
        internal abstract void RefreshRenderInfo(bool force);
        internal abstract double IndexFromPhysicalOffset(double physicalOffset, bool includeCollapsed = false);
        internal abstract void UpdateAverageLength(int startIndex, int endIndex);

        internal abstract AddRemoveLayoutResult AddItem(object changedItem, object addRemoveItem, int addRemoveItemIndex);
        internal abstract AddRemoveLayoutResult RemoveItem(object changedItem, object addRemoveItem, int addRemoveItemIndex);

        internal abstract bool IsItemCollapsed(int slot);

        internal abstract IRenderInfoState GetRenderLoadState();

        protected void RaiseCollapsed(ExpandCollapseEventArgs e)
        {
            if (this.Collapsed != null)
            {
                this.Collapsed(this, e);
            }
        }

        protected void RaiseExpanded(ExpandCollapseEventArgs e)
        {
            if (this.Expanded != null)
            {
                this.Expanded(this, e);
            }
        }

        protected virtual void RaiseItemsSourceChanged(EventArgs e)
        {
            if (this.ItemsSourceChanged != null)
            {
                this.ItemsSourceChanged(this, e);
            }
        }

        protected abstract void SetItemsSourceOverride(IReadOnlyList<object> source, bool restoreCollapsed);

        private void SetItemsSource(IEnumerable source)
        {
            IReadOnlyList<object> readOnlyList = source as IReadOnlyList<object>;
            if (readOnlyList != null)
            {
                this.ItemsSource = readOnlyList; // TODO: Should we copy?
            }
            else
                if (source == null)
                {
                    this.ItemsSource = new ReadOnlyList<object, object>(new List<object>());
                }
                else
                {
                    IList<object> sourceToList = new List<object>();
                    foreach (var item in source)
                    {
                        sourceToList.Add(item);
                    }

                    this.ItemsSource = new ReadOnlyList<object, object>(sourceToList);
                }
        }
    }
}
