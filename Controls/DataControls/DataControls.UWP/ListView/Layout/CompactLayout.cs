using System;
using System.Collections.Generic;
using System.Linq;

namespace Telerik.Data.Core.Layouts
{
    internal class CompactLayout : BaseLayout
    {
        internal Dictionary<object, GroupInfo> itemInfoTable = new Dictionary<object, GroupInfo>();
        internal IndexToValueTable<bool> collapsedSlotsTable;
        internal double averageItemLength;
        private IndexToValueTable<GroupInfo> groupHeadersTable;
        private IHierarchyAdapter hierarchyAdapter;
        private IRenderInfo renderInfo;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "These virtual calls do not rely on uninitialized base state.")]
        public CompactLayout(IHierarchyAdapter adapter, double defaultItemLength)
        {
            if (adapter == null)
            {
                throw new ArgumentNullException("adapter", "Adapter cannot be null.");
            }

            this.DefaultItemLength = defaultItemLength;

            this.hierarchyAdapter = adapter;
            this.averageItemLength = defaultItemLength;
            this.collapsedSlotsTable = new IndexToValueTable<bool>();
            this.groupHeadersTable = new IndexToValueTable<GroupInfo>();

            this.LayoutStrategies.Add(new ItemsLayoutStrategy());

            this.renderInfo = new IndexStorage(this.TotalSlotCount, this.DefaultItemLength);
        }

        public override int GroupCount
        {
            get
            {
                return this.groupHeadersTable.IndexCount;
            }
        }

        internal int RenderInfoCount
        {
            get
            {
                return this.renderInfo != null ? this.RenderInfo.Count : 0;
            }
        }

        protected override IRenderInfo RenderInfo
        {
            get
            {
                if (this.renderInfo == null)
                {
                    this.renderInfo = new IndexStorage(this.TotalSlotCount, this.DefaultItemLength);
                }

                return this.renderInfo;
            }
        }

        protected IHierarchyAdapter HierarchyAdapter
        {
            get
            {
                return this.hierarchyAdapter;
            }
        }

        protected IndexToValueTable<GroupInfo> GroupHeadersTable
        {
            get
            {
                return this.groupHeadersTable;
            }
        }

        protected Dictionary<object, GroupInfo> ItemInfoTable
        {
            get
            {
                return this.itemInfoTable;
            }
        }

        public override IEnumerable<Group> GetGroupsByKey(object key)
        {
            if (key == null)
            {
                yield break;
            }

            foreach (var pair in this.itemInfoTable)
            {
                var group = pair.Key as Group;
                if (group != null && key.Equals(group.Name))
                {
                    yield return group;
                }
            }
        }

        public override IEnumerable<IList<ItemInfo>> GetLines(int line, bool forward)
        {
            if (this.VisibleLineCount == 0 || line < 0 || line >= this.VisibleLineCount)
            {
                yield break;
            }

            int slot = this.GetVisibleSlot(line);

            while (true)
            {
                IList<ItemInfo> itemInfos = null;
                foreach (var strategy in this.LayoutStrategies)
                {
                    itemInfos = strategy.BuildItemInfos(this, line, slot);

                    if (itemInfos != null && itemInfos.Count > 0)
                    {
                        break;
                    }
                }

                if (itemInfos == null || itemInfos.Count == 0)
                {
                    yield break;
                }

                yield return itemInfos;

                slot = forward ? this.GetNextVisibleSlot(slot) : this.GetPreviousVisibleSlot(slot);
                line += forward ? 1 : -1;
            }
        }

        public override void Expand(object item)
        {
            var groupInfo = this.GetGroupInfo(item);
            bool isCollapsed = groupInfo != null ? !groupInfo.IsExpanded : false;

            if (isCollapsed)
            {
                groupInfo.IsExpanded = true;
                if (groupInfo.IsVisible())
                {
                    int groupSlot = 0;
                    int groupSlotSpan = 0;
                    this.GetCollapseRange(groupInfo, out groupSlot, out groupSlotSpan);

                    this.collapsedSlotsTable.RemoveValues(groupSlot, groupSlotSpan);
                    int expandedCount = groupSlotSpan;

                    foreach (var collapsedGroup in this.CollapsedChildItems(groupInfo.Item))
                    {
                        int slot;
                        int slotSpan;
                        this.GetCollapseRange(collapsedGroup, out slot, out slotSpan);
                        expandedCount -= slotSpan;
                        this.collapsedSlotsTable.AddValues(slot, slotSpan, true);
                    }

                    this.VisibleLineCount += expandedCount;
                    this.RaiseExpanded(new ExpandCollapseEventArgs(item, groupSlot, groupSlotSpan));
                }
            }
        }

        public override void Collapse(object item)
        {
            var groupInfo = this.GetGroupInfo(item);

            if (groupInfo != null && groupInfo.IsExpanded && this.IsCollapsible(groupInfo))
            {
                groupInfo.IsExpanded = false;
                if (groupInfo.IsVisible())
                {
                    this.CollapseCore(groupInfo, true);
                }
            }
        }

        public override bool IsCollapsed(object item)
        {
            var groupInfo = this.GetGroupInfo(item);
            return groupInfo != null ? !groupInfo.IsExpanded : false;
        }
        
        internal static void UpdateParentGroupInfosLastSlot(int count, GroupInfo groupInfo)
        {
            GroupInfo parentGroupInfo = groupInfo != null ? groupInfo.Parent : null;
            while (parentGroupInfo != null)
            {
                parentGroupInfo.LastSubItemSlot += count;
                parentGroupInfo = parentGroupInfo.Parent;
            }
        }

        internal override int IndexFromSlot(int slotToRequest)
        {
            return slotToRequest;
        }

        internal override void RefreshRenderInfo(bool force)
        {
            if (force || (this.renderInfo != null && this.VisibleLineCount != this.renderInfo.Count))
            {
                this.renderInfo = null;
            }
        }

        internal override AddRemoveLayoutResult AddItem(object changedItem, object addRemoveItem, int addRemoveItemIndex)
        {
            int count = 1;
            bool isVisible = true;
            var groupInfo = this.GetGroupInfo(changedItem);
            bool changedGroupIsRoot = groupInfo == null;
            int slot = addRemoveItemIndex;

            int level = changedGroupIsRoot ? 0 : groupInfo.Level + 1;

            // We added/removed group so we need to index all subgroups.
            if (addRemoveItem is IGroup && changedItem != addRemoveItem)
            {
                slot = this.GetInsertedGroupSlot(changedItem, addRemoveItemIndex);
                isVisible = groupInfo != null ? (groupInfo.IsExpanded && groupInfo.IsVisible()) : true;

                // We give a list in which to insert groups so that we can manually correct the indexes in groupHeadersTable.
                List<GroupInfo> insertedGroups = new List<GroupInfo>();
                count = this.CountAndPopulateTables(addRemoveItem, slot, level, this.GroupLevels, groupInfo, false, insertedGroups);

                bool first = true;
                int innerMostSlot = slot;
                foreach (var newGroupInfo in insertedGroups)
                {
                    int groupInfoToInsertSpan = newGroupInfo.GetLineSpan();
                    if (first)
                    {
                        first = false;
                        this.groupHeadersTable.InsertIndexes(newGroupInfo.Index, groupInfoToInsertSpan);
                        if (isVisible)
                        {
                            this.collapsedSlotsTable.InsertIndexes(slot, count);
                            this.VisibleLineCount += count;
                        }
                        else
                        {
                            this.collapsedSlotsTable.InsertIndexesAndValues(slot, count, false);
                        }
                    }
                    this.groupHeadersTable.AddValue(newGroupInfo.Index, newGroupInfo);

                    // We get the inner most group so that we update groups after inner most (the upper one are newly created and their index and last slot count are correct.
                    // We get it only if the change is in the root group. In existing groups we know the correct slot.
                    if (changedGroupIsRoot)
                    {
                        innerMostSlot = newGroupInfo.Index;
                    }
                }

                // Update groups after current one.
                this.UpdateGroupHeadersTable(innerMostSlot, count);

                if (groupInfo != null)
                {
                    // Update the group last slot.
                    groupInfo.LastSubItemSlot += count;
                }
            }
            else
            {
                // We added new item (not group) into root group.
                if (changedGroupIsRoot)
                {
                    if (isVisible)
                    {
                        this.VisibleLineCount += count;
                    }

                    // slot should be already correct.
                    // No need to correct collapsed slots. They should be empty (e.g. no groups to collapse).
                    System.Diagnostics.Debug.Assert(this.groupHeadersTable.IsEmpty, "GroupHeaders table should be empty.");
                    System.Diagnostics.Debug.Assert(this.collapsedSlotsTable.IsEmpty, "CollapsedSlots table should be empty since we don't have groups.");
                }
                else
                {
                    // We added new item (not group) into existing bottom level group (not root group).
                    slot = groupInfo.Index + 1 + addRemoveItemIndex;

                    // Update the group last slot.
                    groupInfo.LastSubItemSlot += count;
                    this.groupHeadersTable.InsertIndex(slot);

                    isVisible = groupInfo.IsExpanded && groupInfo.IsVisible();
                    if (isVisible)
                    {
                        this.collapsedSlotsTable.InsertIndexes(slot, count);
                        this.VisibleLineCount += count;
                    }
                    else
                    {
                        this.collapsedSlotsTable.InsertIndexesAndValues(slot, count, false);
                    }

                    this.UpdateGroupHeadersTable(groupInfo.Index, count);
                }
            }

            // Update parent groups last slot.
            UpdateParentGroupInfosLastSlot(count, groupInfo);

            // Update TotalCount.
            this.TotalSlotCount += count;

            // Update Visible line count if not collapsed.
            double length = isVisible ? this.averageItemLength : 0;

            // Insert new records into IndexTree.
            this.RenderInfo.InsertRange(slot, IndexStorage.UnknownItemLength, count);

            var layoutResult = new AddRemoveLayoutResult(slot, count);

            foreach (var strategy in this.LayoutStrategies)
            {
                strategy.OnItemAdded(layoutResult);
            }

            // return result indexes so that changed can be reflected to UI.
            return layoutResult;
        }

        internal override AddRemoveLayoutResult RemoveItem(object changedItem, object addRemoveItem, int addRemoveItemIndex)
        {
            int count = 1;
            bool isVisible = true;
            GroupInfo changedGroupInfo = this.GetGroupInfo(changedItem);
            bool changedGroupIsRoot = changedGroupInfo == null;
            int slot = addRemoveItemIndex;

            int level = changedGroupIsRoot ? 0 : changedGroupInfo.Level + 1;

            IGroup removedGroup = addRemoveItem as IGroup;

            // We added/removed group so we need to index all subgroups.
            if (removedGroup != null && changedItem != addRemoveItem)
            {
                GroupInfo groupInfo = this.GetGroupInfo(addRemoveItem);
                System.Diagnostics.Debug.Assert(groupInfo != null, "Cannot remove group that are not indexed.");
                slot = groupInfo.Index;
                isVisible = groupInfo.IsVisible();
                count = groupInfo.GetLineSpan();

                if (!removedGroup.HasItems)
                {
                    foreach (var nextGroupIndex in this.groupHeadersTable.GetIndexes(slot + 1))
                    {
                        GroupInfo nextGroupInfo = this.groupHeadersTable.GetValueAt(nextGroupIndex);
                        if (nextGroupInfo.Level <= groupInfo.Level)
                        {
                            break;
                        }

                        this.RemoveGroupInfo(nextGroupInfo);
                    }
                }

                this.UpdateGroupHeadersTable(groupInfo.Index, -count);

                this.itemInfoTable.Remove(removedGroup);
                this.groupHeadersTable.RemoveIndexesAndValues(slot, count);

                if (isVisible)
                {
                    if (groupInfo.IsExpanded)
                    {
                        int collapsedItems = count - this.GetCollapsedSlotsCount(slot, slot + count - 1);
                        this.VisibleLineCount -= collapsedItems;
                    }
                    else
                    {
                        this.VisibleLineCount -= 1;
                    }
                }

                this.collapsedSlotsTable.RemoveIndexesAndValues(slot, count);

                if (changedGroupInfo != null)
                {
                    // Update the group last slot.
                    changedGroupInfo.LastSubItemSlot -= count;
                }
            }
            else
            {
                // We added new item (not group) into root group.
                if (changedGroupIsRoot)
                {
                    if (isVisible)
                    {
                        this.VisibleLineCount -= count;
                    }

                    // slot should be already correct.
                    // No need to correct collapsed slots. They should be empty (e.g. no groups to collapse).
                    System.Diagnostics.Debug.Assert(this.groupHeadersTable.IsEmpty, "GroupHeaders table should be empty.");
                    System.Diagnostics.Debug.Assert(this.collapsedSlotsTable.IsEmpty, "CollapsedSlots table should be empty since we don't have groups.");
                }
                else
                {
                    // We added new item (not group) into existing bottom level group (not root group).
                    slot = changedGroupInfo.Index + 1 + addRemoveItemIndex;

                    // Update the group last slot.
                    changedGroupInfo.LastSubItemSlot -= count;
                    this.groupHeadersTable.RemoveIndex(slot);

                    isVisible = changedGroupInfo.IsExpanded && changedGroupInfo.IsVisible();
                    if (isVisible)
                    {
                        this.collapsedSlotsTable.RemoveIndexes(slot, count);
                        this.VisibleLineCount -= count;
                    }
                    else
                    {
                        this.collapsedSlotsTable.RemoveIndexesAndValues(slot, count);
                    }

                    this.UpdateGroupHeadersTable(changedGroupInfo.Index, -count);
                }
            }

            // Update parent groups last slot.
            UpdateParentGroupInfosLastSlot(-count, changedGroupInfo);

            // Update TotalCount.
            this.TotalSlotCount -= count;

            // Insert new records into IndexTree.
            this.RenderInfo.RemoveRange(slot, count);

            var layoutResult = new AddRemoveLayoutResult(slot, count);
            foreach (var strategy in this.LayoutStrategies)
            {
                strategy.OnItemRemoved(layoutResult);
            }

            // return result indexes so that changed can be reflected to UI.
            return layoutResult;
        }

        internal override int GetNextVisibleSlot(int slot)
        {
            return this.collapsedSlotsTable.GetNextGap(slot);
        }

        internal override bool IsItemCollapsed(int slot)
        {
            return this.collapsedSlotsTable.Contains(slot);
        }

        internal override IRenderInfoState GetRenderLoadState()
        {
            return new LayoutRenderInfoState(this.collapsedSlotsTable);
        }

        internal int GetPreviousVisibleSlot(int slot)
        {
            return this.collapsedSlotsTable.GetPreviousGap(slot);
        }

        internal virtual int GetLayoutLevel(ItemInfo itemInfo, GroupInfo parentGroupInfo)
        {
            return 0;
        }

        internal virtual int GetIndent(ItemInfo itemInfo, GroupInfo parentGroupInfo)
        {
            if (itemInfo.ItemType == GroupType.Subtotal)
            {
                if (parentGroupInfo != null && !parentGroupInfo.IsExpanded)
                {
                    return this.AggregatesLevel;
                }
                else
                {
                    return itemInfo.Level - 1;
                }
            }

            return itemInfo.Level;
        }

        internal override int GetVisibleSlot(int index)
        {
            return this.collapsedSlotsTable.CountNextNotIncludedIndexes(0, index);
        }

        internal override int GetCollapsedSlotsCount(int startSlot, int endSlot)
        {
            return this.collapsedSlotsTable.GetIndexCount(startSlot, endSlot);
        }

        internal override GroupInfo GetGroupInfo(object item)
        {
            GroupInfo groupInfo;
            if (this.itemInfoTable != null && this.itemInfoTable.TryGetValue(item, out groupInfo))
            {
                return groupInfo;
            }

            return null;
        }

        internal override double SlotFromPhysicalOffset(double physicalOffset, bool includeCollapsed = false)
        {
            var logicalOffset = this.RenderInfo.IndexFromOffset(physicalOffset);
            double offset;
            if (logicalOffset > 0)
            {
                double currentItemWidth = this.RenderInfo.ValueForIndex(logicalOffset);

                if (currentItemWidth == 0)
                {
                    currentItemWidth = this.DefaultItemLength;
                }

                var previousItemOffset = this.RenderInfo.OffsetFromIndex(logicalOffset - 1);

                if (!includeCollapsed)
                {
                    var collapsedSlotCount = this.GetCollapsedSlotsCount(0, logicalOffset);
                    logicalOffset -= collapsedSlotCount;
                }

                offset = logicalOffset + (physicalOffset - previousItemOffset) / currentItemWidth;
            }
            else
            {
                offset = this.RenderInfo.OffsetFromIndex(logicalOffset);
                offset = physicalOffset / Math.Max(offset, 1);
            }

            return offset;
        }

        internal override void UpdateAverageLength(int startIndex, int endIndex)
        {
            int generated = endIndex - startIndex + 1;
            if (generated == 1)
            {
                this.averageItemLength = this.RenderInfo.ValueForIndex(startIndex);
            }
            else
            {
                double endOffset = this.RenderInfo.OffsetFromIndex(endIndex);
                double startOffset = startIndex > 0 ? this.RenderInfo.OffsetFromIndex(startIndex - 1) : 0;
                this.averageItemLength = (endOffset - startOffset) / generated;
            }
        }

        internal override int CountAndPopulateTables(object item, int rootSlot, int level, int levels, GroupInfo parent, bool shouldIndexItem, List<GroupInfo> insert, ref int totalLines)
        {
            return this.CountAndPopulateTables(item, rootSlot, level, levels, parent, shouldIndexItem, insert);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", Justification = "Not a real issue.")]
        internal int CountAndPopulateTables(object item, int rootSlot, int level, int levels, GroupInfo parent, bool shouldIndexItem, List<GroupInfo> insert)
        {
            int treeCount = 1;
            var subItems = this.hierarchyAdapter.GetItems(item);

            IGroup group = item as IGroup;
            GroupInfo itemGroupInfo = null;

            if (level <= levels - 1)
            {
                bool shouldIndexChildren = false;
                foreach (var subItem in subItems)
                {
                    if (itemGroupInfo == null)
                    {
                        itemGroupInfo = new GroupInfo(item, parent, true, level, rootSlot, rootSlot + treeCount - 1);
                    }

                    int childrenCount = this.CountAndPopulateTables(subItem, rootSlot + treeCount, level + 1, levels, itemGroupInfo, shouldIndexChildren, insert);
                    shouldIndexChildren = shouldIndexChildren || childrenCount > 1;
                    treeCount += childrenCount;
                }
            }

            shouldIndexItem = shouldIndexItem || treeCount > 1;

            if (shouldIndexItem)
            {
                if (itemGroupInfo == null)
                {
                    itemGroupInfo = new GroupInfo(item, parent, true, level, rootSlot, rootSlot + treeCount - 1);
                }
                else
                {
                    itemGroupInfo.LastSubItemSlot = rootSlot + treeCount - 1;
                }

                if (insert == null)
                {
                    this.groupHeadersTable.AddValue(rootSlot, itemGroupInfo);
                }
                else
                {
                    insert.Insert(0, itemGroupInfo);
                }

                if (this.itemInfoTable == null)
                {
                    this.itemInfoTable = new Dictionary<object, GroupInfo>();
                }

                this.itemInfoTable.Add(item, itemGroupInfo);
            }

            return treeCount;
        }

        internal int GetInsertedGroupSlot(object changedItem, int itemIndex)
        {
            GroupInfo changedItemInfo = this.GetGroupInfo(changedItem);

            if (itemIndex == 0)
            {
                return changedItemInfo != null ? changedItemInfo.Index + 1 : itemIndex;
            }

            // We get the previous group GroupInfo and then calculate the Slot because the new one is not indexed yet.
            var group = this.hierarchyAdapter.GetItemAt(changedItem, itemIndex - 1);
            var groupInfo = this.GetGroupInfo(group);
            return groupInfo.LastSubItemSlot + 1;
        }

        internal void UpdateGroupHeadersTable(int groupIndex, int count)
        {
            // Update groups after current one.
            foreach (var nextGroupIndex in this.groupHeadersTable.GetIndexes(groupIndex + 1))
            {
                GroupInfo nextGroupInfo = this.groupHeadersTable.GetValueAt(nextGroupIndex);
                nextGroupInfo.Index += count;
                nextGroupInfo.LastSubItemSlot += count;
            }
        }
        
        internal override IList<ItemInfo> GetItemInfosAtSlot(int visibleLine, int slot)
        {
            List<ItemInfo> items = new List<ItemInfo>();

            ItemInfo itemInfo = new ItemInfo();
            itemInfo.IsDisplayed = true;

            GroupInfo groupInfo;
            int lowerBound;

            int itemsCount = 0;
            foreach (var item in this.groupHeadersTable)
            {
                itemsCount = Math.Max(item.Value.LastSubItemSlot, itemsCount);
            }

            if (this.groupHeadersTable.TryGetValue(slot, out groupInfo, out lowerBound) && slot <= itemsCount)
            {
                if (lowerBound != slot)
                {
                    int childIndex = slot - lowerBound - 1;
                    itemInfo.Id = groupInfo.Index + childIndex + 1;
                    itemInfo.Item = this.hierarchyAdapter.GetItemAt(groupInfo.Item, childIndex);
                    itemInfo.Level = groupInfo.Level + 1;
                    itemInfo.Slot = itemInfo.Id;

                    itemInfo.IsCollapsible = false;
                    itemInfo.IsCollapsed = false;
                    itemInfo.ItemType = BaseLayout.GetItemType(itemInfo.Item);
                    itemInfo.IsSummaryVisible = itemInfo.ItemType == GroupType.Subtotal && this.IsCollapsed(groupInfo.Item);

                    items.Add(itemInfo);

                    var parentItemInfo = new ItemInfo();
                    parentItemInfo.IsDisplayed = false;
                    parentItemInfo.Id = groupInfo.Index;
                    parentItemInfo.Item = groupInfo.Item;
                    parentItemInfo.Level = groupInfo.Level;
                    parentItemInfo.Slot = groupInfo.Index;
                    parentItemInfo.IsCollapsible = this.IsCollapsible(groupInfo);
                    parentItemInfo.IsCollapsed = !groupInfo.IsExpanded;
                    parentItemInfo.ItemType = BaseLayout.GetItemType(groupInfo.Item);
                    parentItemInfo.IsSummaryVisible = parentItemInfo.ItemType == GroupType.Subtotal && groupInfo.Parent != null && this.IsCollapsed(groupInfo.Parent.Item);

                    items.Insert(0, parentItemInfo);
                }
                else
                {
                    itemInfo.Id = groupInfo.Index;
                    itemInfo.Item = groupInfo.Item;
                    itemInfo.Level = groupInfo.Level;
                    itemInfo.Slot = itemInfo.Id;
                    itemInfo.IsCollapsible = this.IsCollapsible(groupInfo);
                    itemInfo.IsCollapsed = !groupInfo.IsExpanded;
                    itemInfo.ItemType = BaseLayout.GetItemType(itemInfo.Item);
                    itemInfo.IsSummaryVisible = itemInfo.ItemType == GroupType.Subtotal && groupInfo.Parent != null && this.IsCollapsed(groupInfo.Parent.Item);

                    items.Add(itemInfo);
                }

                var parentGroupInfo = groupInfo.Parent;
                while (parentGroupInfo != null)
                {
                    var parentItemInfo = new ItemInfo();
                    parentItemInfo.IsDisplayed = false;
                    parentItemInfo.Id = parentGroupInfo.Index;
                    parentItemInfo.Item = parentGroupInfo.Item;
                    parentItemInfo.Level = parentGroupInfo.Level;
                    parentItemInfo.Slot = parentGroupInfo.Index;
                    parentItemInfo.IsCollapsible = this.IsCollapsible(parentGroupInfo);
                    parentItemInfo.IsCollapsed = !parentGroupInfo.IsExpanded;
                    parentItemInfo.ItemType = BaseLayout.GetItemType(parentGroupInfo.Item);
                    parentItemInfo.IsSummaryVisible = parentItemInfo.ItemType == GroupType.Subtotal && parentGroupInfo.Parent != null && this.IsCollapsed(parentGroupInfo.Parent.Item);

                    items.Insert(0, parentItemInfo);
                    parentGroupInfo = parentGroupInfo.Parent;
                }
            }
            else if (this.ItemsSource != null && visibleLine < this.ItemsSource.Count)
            {
                itemInfo.Item = this.ItemsSource[visibleLine];
                itemInfo.Level = 0;
                itemInfo.Id = slot;
                itemInfo.Slot = itemInfo.Id;

                itemInfo.IsCollapsible = false;
                itemInfo.IsCollapsed = false;
                itemInfo.ItemType = BaseLayout.GetItemType(itemInfo.Item);
                itemInfo.IsSummaryVisible = false;

                items.Add(itemInfo);
            }

            return items;
        }

        protected override void SetItemsSourceOverride(IReadOnlyList<object> source, bool restoreCollapsed)
        {
            this.collapsedSlotsTable.Clear();
            this.groupHeadersTable.Clear();

            HashSet<object> collapsedGroups = null;
            var canRestoreCollapsedState = this.itemInfoTable != null && restoreCollapsed;

            if (this.itemInfoTable != null)
            {
                if (restoreCollapsed)
                {
                    collapsedGroups = this.CopyCollapsedStates();
                }
                this.itemInfoTable.Clear();
            }

            int slotsCount = 0;

            int temp = 0;

            foreach (var strategy in this.LayoutStrategies)
            {
                slotsCount += strategy.CalculateAppendedSlotsCount(this, slotsCount, ref temp);
            }

            this.TotalSlotCount = this.VisibleLineCount = slotsCount;

            this.RefreshRenderInfo(false);

            if (canRestoreCollapsedState)
            {
                foreach (var pair in this.itemInfoTable)
                {
                    if (collapsedGroups.Contains(pair.Key))
                    {
                        this.Collapse(pair.Key);
                    }
                }
            }
        }

        private HashSet<object> CopyCollapsedStates()
        {
            HashSet<object> hashSet = new HashSet<object>();

            foreach (var pair in this.itemInfoTable)
            {
                if (!pair.Value.IsExpanded)
                {
                    hashSet.Add(pair.Key);
                }
            }

            return hashSet;
        }

        private bool IsCollapsible(GroupInfo groupInfo)
        {
            int itemLevel = groupInfo.Level;
            bool hasItems = Enumerable.Any(this.hierarchyAdapter.GetItems(groupInfo.Item));
            {
                return hasItems;
            }
        }

        private void GetCollapseRange(GroupInfo groupInfo, out int slot, out int slotSpan)
        {
            int itemSlot = groupInfo.Index;
            int itemLevel = groupInfo.Level;

            slot = itemSlot + 1;
            slotSpan = groupInfo.GetLineSpan() - 1;

            int aggregatesLevel = this.ShowAggregateValuesInline ? this.AggregatesLevel - 1 : this.AggregatesLevel;
        }

        private void CollapseCore(GroupInfo info, bool raiseExpanded)
        {
            info.IsExpanded = false;

            int slot = 0;
            int slotSpan = 0;
            this.GetCollapseRange(info, out slot, out slotSpan);

            int collapsedItems = slotSpan - this.GetCollapsedSlotsCount(slot, slot + slotSpan - 1);
            this.collapsedSlotsTable.AddValues(slot, slotSpan, true);

            this.VisibleLineCount -= collapsedItems;

            if (raiseExpanded)
            {
                this.RaiseCollapsed(new ExpandCollapseEventArgs(info.Item, slot, slotSpan));
            }
        }

        private IEnumerable<GroupInfo> CollapsedChildItems(object item)
        {
            var groupInfo = this.GetGroupInfo(item);
            if (groupInfo == null)
            {
                yield break;
            }
            else if (!groupInfo.IsExpanded)
            {
                yield return groupInfo;
            }
            else if (Enumerable.Any(this.hierarchyAdapter.GetItems(item)) && groupInfo.Level < this.GroupLevels - 1)
            {
                var items = this.hierarchyAdapter.GetItems(item);
                foreach (var childGroup in items)
                {
                    foreach (var collapsedChildGroup in this.CollapsedChildItems(childGroup))
                    {
                        yield return collapsedChildGroup;
                    }
                }
            }
        }

        private void RemoveGroupInfo(GroupInfo groupInfo)
        {
            this.itemInfoTable.Remove(groupInfo.Item);
        }
    }
}
