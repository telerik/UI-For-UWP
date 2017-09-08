using System;
using System.Collections.Generic;

namespace Telerik.Data.Core.Layouts
{
    internal class StackedCompactLayout : CompactLayout
    {
        private int stackCount;

        public StackedCompactLayout(IHierarchyAdapter adapter, double defaultItemLength, int stackCount)
            : base(adapter, defaultItemLength)
        {
            this.StackCount = stackCount;
        }

        public int StackCount
        {
            get
            {
                return this.stackCount;
            }
            set
            {
                this.stackCount = value;
                this.LayoutStrategies.Clear();
                this.LayoutStrategies.Add(new StackedItemsLayoutStrategy { StackCount = this.StackCount });
                this.SetSource(this.ItemsSource, this.GroupLevels, 0, 0, 0, true);
            }
        }

        internal override int CalculateFlatRowCount()
        {
            return (int)Math.Ceiling((double)this.ItemsSource.Count / this.stackCount);
        }

        internal override int CountAndPopulateTables(object item, int rootSlot, int level, int levels, GroupInfo parent, bool shouldIndexItem, List<GroupInfo> insert, ref int totalLines)
        {
            int subtotalItemsCount = 1;
            var subItems = this.HierarchyAdapter.GetItems(item);

            IGroup group = item as IGroup;
            GroupInfo itemGroupInfo = null;

            if (level <= levels - 1)
            {
                bool shouldIndexChildren = false;
                int totalChildrenCount = 0;

                foreach (var subItem in subItems)
                {
                    if (itemGroupInfo == null)
                    {
                        itemGroupInfo = new GroupInfo(item, parent, true, level, rootSlot, rootSlot + subtotalItemsCount - 1);
                    }

                    int childrenSlotsCount = this.CountAndPopulateTables(subItem, rootSlot + subtotalItemsCount, level + 1, levels, itemGroupInfo, shouldIndexChildren, insert, ref totalLines);
                    shouldIndexChildren = shouldIndexChildren || childrenSlotsCount > 1;

                    totalChildrenCount += childrenSlotsCount;

                    subtotalItemsCount += childrenSlotsCount;
                }

                // Only if last group level
                if (level == levels - 1)
                {
                    totalLines += (int)Math.Ceiling(totalChildrenCount / (double)this.StackCount) + 1; // total slots for a group including its children
                }
                else
                {
                    totalLines += 1; // only group header
                }
            }

            shouldIndexItem = shouldIndexItem || subtotalItemsCount > 1;

            if (shouldIndexItem)
            {
                if (itemGroupInfo == null)
                {
                    itemGroupInfo = new GroupInfo(item, parent, true, level, rootSlot, rootSlot + subtotalItemsCount - 1);
                }
                else
                {
                    itemGroupInfo.LastSubItemSlot = rootSlot + subtotalItemsCount - 1;
                }

                if (insert == null)
                {
                    this.GroupHeadersTable.AddValue(rootSlot, itemGroupInfo);
                }
                else
                {
                    insert.Insert(0, itemGroupInfo);
                }

                this.ItemInfoTable.Add(item, itemGroupInfo);
            }

            return subtotalItemsCount;
        }
        
        internal override IList<ItemInfo> GetItemInfosAtSlot(int visibleLine, int rowSlot)
        {
            List<ItemInfo> items = new List<ItemInfo>();

            GroupInfo groupInfo;
            int lowerBound;

            int itemsCount = 0;
            foreach (var item in this.GroupHeadersTable)
            {
                itemsCount = Math.Max(item.Value.LastSubItemSlot, itemsCount);
            }

            int stackSlot = this.GetFirstStackSlotOnRow(rowSlot, itemsCount);

            if (this.GroupHeadersTable.TryGetValue(stackSlot, out groupInfo, out lowerBound) && stackSlot <= itemsCount)
            {
                if (lowerBound != stackSlot)
                {
                    // Children item inside the group
                    int childSlotIndex = stackSlot - lowerBound - 1;

                    // TODO find more effition
                    for (int i = 0; i < this.StackCount; i++)
                    {
                        var item = this.HierarchyAdapter.GetItemAt(groupInfo.Item, childSlotIndex + i);

                        if (item == null)
                        {
                            // End of child collection - the stack row/column is not full.
                            break;
                        }

                        ItemInfo itemInfo = new ItemInfo();
                        itemInfo.IsDisplayed = true;
                        itemInfo.Id = groupInfo.Index + childSlotIndex + 1 + i;
                        itemInfo.Item = item;
                        itemInfo.Level = groupInfo.Level + 1;
                        itemInfo.Slot = rowSlot; // groupInfo.Index + (int)Math.Ceiling(childSlotIndex/(double)this.StackCount) + 1;

                        itemInfo.IsCollapsible = false;
                        itemInfo.IsCollapsed = false;
                        itemInfo.ItemType = BaseLayout.GetItemType(itemInfo.Item);
                        itemInfo.IsSummaryVisible = itemInfo.ItemType == GroupType.Subtotal && this.IsCollapsed(groupInfo.Item);

                        items.Add(itemInfo);
                    }

                    // Add parent (group) for lookup
                    var parentItemInfo = new ItemInfo();
                    parentItemInfo.IsDisplayed = false;
                    parentItemInfo.Id = groupInfo.Index;
                    parentItemInfo.Item = groupInfo.Item;
                    parentItemInfo.Level = groupInfo.Level;
                    parentItemInfo.Slot = rowSlot;
                    
                    parentItemInfo.IsCollapsed = !groupInfo.IsExpanded;
                    parentItemInfo.ItemType = BaseLayout.GetItemType(groupInfo.Item);
                    parentItemInfo.IsSummaryVisible = parentItemInfo.ItemType == GroupType.Subtotal && groupInfo.Parent != null && this.IsCollapsed(groupInfo.Parent.Item);

                    items.Insert(0, parentItemInfo);
                }
                else
                {
                    // Group header
                    ItemInfo itemInfo = new ItemInfo();
                    itemInfo.IsDisplayed = true;
                    itemInfo.Id = groupInfo.Index;
                    itemInfo.Item = groupInfo.Item;
                    itemInfo.Level = groupInfo.Level;
                    itemInfo.Slot = rowSlot;
                    itemInfo.IsCollapsed = !groupInfo.IsExpanded;
                    itemInfo.ItemType = BaseLayout.GetItemType(itemInfo.Item);
                    itemInfo.IsSummaryVisible = itemInfo.ItemType == GroupType.Subtotal && groupInfo.Parent != null && this.IsCollapsed(groupInfo.Parent.Item);

                    items.Add(itemInfo);
                }

                // Add parent path to root
                int line = visibleLine - 1;
                var parentGroupInfo = groupInfo.Parent;
                while (parentGroupInfo != null)
                {
                    var parentItemInfo = new ItemInfo();
                    parentItemInfo.IsDisplayed = false;
                    parentItemInfo.Id = parentGroupInfo.Index;
                    parentItemInfo.Item = parentGroupInfo.Item;
                    parentItemInfo.Level = parentGroupInfo.Level;
                    parentItemInfo.Slot = parentGroupInfo.Index;
                    parentItemInfo.IsCollapsed = !parentGroupInfo.IsExpanded;
                    parentItemInfo.ItemType = BaseLayout.GetItemType(parentGroupInfo.Item);
                    parentItemInfo.IsSummaryVisible = parentItemInfo.ItemType == GroupType.Subtotal && parentGroupInfo.Parent != null && this.IsCollapsed(parentGroupInfo.Parent.Item);

                    line--;

                    items.Insert(0, parentItemInfo);
                    parentGroupInfo = parentGroupInfo.Parent;
                }
            }
            else
            {
                for (int stackIndex = 0; stackIndex < this.StackCount; stackIndex++)
                {
                    var position = this.StackCount * visibleLine + stackIndex;

                    if (this.ItemsSource != null && position < this.ItemsSource.Count)
                    {
                        ItemInfo itemInfo = new ItemInfo();
                        itemInfo.IsDisplayed = true;

                        itemInfo.Item = this.ItemsSource[position];
                        itemInfo.Level = 0;
                        itemInfo.Id = position;
                        itemInfo.Slot = rowSlot;

                        itemInfo.IsCollapsible = false;
                        itemInfo.IsCollapsed = false;
                        itemInfo.ItemType = BaseLayout.GetItemType(itemInfo.Item);
                        itemInfo.IsSummaryVisible = false;

                        items.Add(itemInfo);
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// Gets the slot as if in flat scenario (without stacks).
        /// </summary>
        /// <returns> Returns-1 if the targetRowSlot negative or positive number that represents that last slot in a row.</returns>
        internal int GetFirstStackSlotOnRow(int targetRowSlot, int totalItemsCount)
        {
            var prevRow = targetRowSlot - 1;

            int currentSlotIndex = -1;
            int currentRowIndex = -1;
            GroupInfo groupInfo;
            int lowerBound;

            if (targetRowSlot == 0)
            {
                return 0;
            }

            if (this.GroupLevels == 0)
            {
                return targetRowSlot > 0 ? targetRowSlot * this.StackCount : 0;
            }

            for (int i = 0; i < totalItemsCount; i++)
            {
                if (this.GroupHeadersTable.TryGetValue(currentSlotIndex + 1, out groupInfo, out lowerBound))
                {
                    if (groupInfo.Level == this.GroupLevels - 1 && currentSlotIndex + 1 > lowerBound)
                    {
                        // lowest level group - we need to calculate the number of rows inside it.
                        var itemsCount = groupInfo.LastSubItemSlot - groupInfo.Index;

                        var groupRows = (int)Math.Ceiling(itemsCount / (double)this.StackCount);

                        if (currentRowIndex + groupRows < prevRow)
                        {
                            currentRowIndex += groupRows;
                            currentSlotIndex += itemsCount;
                        }
                        else
                        {
                            // the row is in this group as leaf
                            var innerRowIndex = prevRow - currentRowIndex;
                            currentSlotIndex += Math.Min(innerRowIndex * this.StackCount, itemsCount);
                            break;
                        }
                    }
                    else
                    {
                        // high level group - add only its slot
                        currentSlotIndex++;
                        currentRowIndex++;

                        // row is over a group header
                        if (currentRowIndex == prevRow) 
                        {
                            break;
                        }
                    }
                }
            }

            return currentSlotIndex + 1;
        }

        internal override int IndexFromSlot(int slotToRequest)
        {
            return slotToRequest * this.StackCount;
        }

        internal override AddRemoveLayoutResult AddItem(object changedItem, object addRemoveItem, int addRemoveItemIndex)
        {
            int addedItemsCount = 1;
            bool isVisible = true;
            var groupInfo = this.GetGroupInfo(changedItem);
            bool changedGroupIsRoot = groupInfo == null;
            int flatSlot = -1; // = addRemoveItemIndex;
            int rowSlot = -1;
            int addedLinesCount = 0;

            int level = changedGroupIsRoot ? 0 : groupInfo.Level + 1;

            // We added/removed group so we need to index all subgroups.
            if (addRemoveItem is IGroup && changedItem != addRemoveItem)
            {
                flatSlot = this.GetInsertedGroupSlot(changedItem, addRemoveItemIndex);
                isVisible = groupInfo != null ? (groupInfo.IsExpanded && groupInfo.IsVisible()) : true;

                // We give a list in which to insert groups so that we can manually correct the indexes in groupHeadersTable.
                List<GroupInfo> insertedGroups = new List<GroupInfo>();
                addedItemsCount = this.CountAndPopulateTables(addRemoveItem, flatSlot, level, this.GroupLevels, groupInfo, false, insertedGroups);

                addedLinesCount = addedItemsCount;

                bool first = true;
                int innerMostSlot = flatSlot;
                foreach (var newGroupInfo in insertedGroups)
                {
                    int groupInfoToInsertSpan = newGroupInfo.GetLineSpan();
                    if (first)
                    {
                        first = false;
                        this.GroupHeadersTable.InsertIndexes(newGroupInfo.Index, groupInfoToInsertSpan);
                        if (isVisible)
                        {
                            this.collapsedSlotsTable.InsertIndexes(flatSlot, addedItemsCount);
                            this.VisibleLineCount += addedItemsCount;
                        }
                        else
                        {
                            this.collapsedSlotsTable.InsertIndexesAndValues(flatSlot, addedItemsCount, false);
                        }
                    }
                    this.GroupHeadersTable.AddValue(newGroupInfo.Index, newGroupInfo);

                    // We get the inner most group so that we update groups after inner most (the upper one are newly created and their index and last slot count are correct.
                    // We get it only if the change is in the root group. In existing groups we know the correct slot.
                    if (changedGroupIsRoot)
                    {
                        innerMostSlot = newGroupInfo.Index;
                    }
                }

                // Update groups after current one.
                this.UpdateGroupHeadersTable(innerMostSlot, addedItemsCount);

                if (groupInfo != null)
                {
                    // Update the group last slot.
                    groupInfo.LastSubItemSlot += addedItemsCount;
                }
            }
            else
            {
                // We added new item (not group) into root group.
                if (changedGroupIsRoot)
                {
                    if (isVisible)
                    {
                        addedLinesCount = this.GetAddedSlots(this.ItemsSource.Count, addedItemsCount);

                        this.VisibleLineCount += addedLinesCount;

                        flatSlot = addRemoveItemIndex;
                    }

                    // slot should be already correct.
                    // No need to correct collapsed slots. They should be empty (e.g. no groups to collapse).
                    System.Diagnostics.Debug.Assert(this.GroupHeadersTable.IsEmpty, "GroupHeaders table should be empty.");
                    System.Diagnostics.Debug.Assert(this.collapsedSlotsTable.IsEmpty, "CollapsedSlots table should be empty since we don't have groups.");
                }
                else
                {
                    // We added new item (not group) into existing bottom level group (not root group).
                    var children = (groupInfo.Item as IGroup).Items.Count;

                    addedLinesCount = this.GetAddedSlots(children, addedItemsCount);

                    flatSlot = groupInfo.Index + 1 + addRemoveItemIndex;

                    // We give a list in which to insert groups so that we can manually correct the indexes in groupHeadersTable.

                    // Update the group last slot.
                    groupInfo.LastSubItemSlot += addedItemsCount;

                    if (addedItemsCount > 0)
                    {
                        this.GroupHeadersTable.InsertIndex(flatSlot);
                    }

                    isVisible = groupInfo.IsExpanded && groupInfo.IsVisible();
                    if (addedItemsCount > 0)
                    {
                        if (isVisible)
                        {
                            this.collapsedSlotsTable.InsertIndexes(flatSlot, addedItemsCount);
                            this.VisibleLineCount += addedLinesCount;
                        }
                        else
                        {
                            this.collapsedSlotsTable.InsertIndexesAndValues(flatSlot, addedItemsCount, false);
                        }
                    }

                    this.UpdateGroupHeadersTable(groupInfo.Index, addedItemsCount);
                }
            }

            rowSlot = this.GetRowSlotFromFlatSlot(flatSlot);

            // Update parent groups last slot.
            CompactLayout.UpdateParentGroupInfosLastSlot(addedItemsCount, groupInfo);

            // Update TotalCount.
            this.TotalSlotCount += addedLinesCount;

            // Update Visible line count if not collapsed.
            double length = isVisible ? this.averageItemLength : 0;

            if (addedLinesCount >= 0)
            {
                this.InsertToRenderInfo(rowSlot, null, addedLinesCount);
            }
            var layoutResult = new AddRemoveLayoutResult(rowSlot, addedLinesCount);

            foreach (var strategy in this.LayoutStrategies)
            {
                strategy.OnItemAdded(layoutResult);
            }

            // return result indexes so that changed can be reflected to UI.
            return layoutResult;
        }

        internal override AddRemoveLayoutResult RemoveItem(object changedItem, object addRemoveItem, int addRemoveItemIndex)
        {
            int removedItemsCount = 1;
            int removedLinesCount = 0;

            bool isVisible = true;
            GroupInfo changedGroupInfo = this.GetGroupInfo(changedItem);
            bool changedGroupIsRoot = changedGroupInfo == null;
            int flatSlot = -1;
            int rowSlot = -1;

            int level = changedGroupIsRoot ? 0 : changedGroupInfo.Level + 1;

            IGroup removedGroup = addRemoveItem as IGroup;

            // We added/removed group so we need to index all subgroups.
            if (removedGroup != null && changedItem != addRemoveItem)
            {
                GroupInfo groupInfo = this.GetGroupInfo(addRemoveItem);
                System.Diagnostics.Debug.Assert(groupInfo != null, "Cannot remove group that are not indexed.");
                flatSlot = groupInfo.Index;
                isVisible = groupInfo.IsVisible();
                removedItemsCount = groupInfo.GetLineSpan();

                removedLinesCount = removedItemsCount;

                if (!removedGroup.HasItems)
                {
                    foreach (var nextGroupIndex in this.GroupHeadersTable.GetIndexes(flatSlot + 1))
                    {
                        GroupInfo nextGroupInfo = this.GroupHeadersTable.GetValueAt(nextGroupIndex);
                        if (nextGroupInfo.Level <= groupInfo.Level)
                        {
                            break;
                        }

                        this.RemoveGroupInfo(nextGroupInfo);
                    }
                }

                this.UpdateGroupHeadersTable(groupInfo.Index, -removedItemsCount);

                this.itemInfoTable.Remove(removedGroup);
                this.GroupHeadersTable.RemoveIndexesAndValues(flatSlot, removedItemsCount);

                if (isVisible)
                {
                    if (groupInfo.IsExpanded)
                    {
                        int collapsedItems = removedItemsCount - this.GetCollapsedSlotsCount(flatSlot, flatSlot + removedItemsCount - 1);
                        this.VisibleLineCount -= collapsedItems;
                    }
                    else
                    {
                        this.VisibleLineCount -= 1;
                    }
                }

                this.collapsedSlotsTable.RemoveIndexesAndValues(flatSlot, removedItemsCount);

                if (changedGroupInfo != null)
                {
                    // Update the group last slot.
                    changedGroupInfo.LastSubItemSlot -= removedItemsCount;
                }
            }
            else
            {
                // We added new item (not group) into root group.
                if (changedGroupIsRoot)
                {
                    if (isVisible)
                    {
                        removedLinesCount = this.GetRemovedSlots(this.ItemsSource.Count, removedItemsCount);

                        this.VisibleLineCount -= removedLinesCount;

                        flatSlot = addRemoveItemIndex;
                    }

                    // slot should be already correct.
                    // No need to correct collapsed slots. They should be empty (e.g. no groups to collapse).
                    System.Diagnostics.Debug.Assert(this.GroupHeadersTable.IsEmpty, "GroupHeaders table should be empty.");
                    System.Diagnostics.Debug.Assert(this.collapsedSlotsTable.IsEmpty, "CollapsedSlots table should be empty since we don't have groups.");
                }
                else
                {
                    var children = (changedGroupInfo.Item as IGroup).Items.Count;

                    // Be carefull that this represents lines/slots rather than items. Also consider if collapsed table should use this
                    removedLinesCount = this.GetRemovedSlots(children, removedItemsCount);

                    flatSlot = changedGroupInfo.Index + 1 + addRemoveItemIndex;

                    // Update the group last slot.
                    changedGroupInfo.LastSubItemSlot -= removedItemsCount;
                    this.GroupHeadersTable.RemoveIndex(flatSlot);

                    isVisible = changedGroupInfo.IsExpanded && changedGroupInfo.IsVisible();
                    if (isVisible)
                    {
                        this.collapsedSlotsTable.RemoveIndexes(flatSlot, removedLinesCount);
                        this.VisibleLineCount -= removedLinesCount;
                    }
                    else
                    {
                        this.collapsedSlotsTable.RemoveIndexesAndValues(flatSlot, removedLinesCount);
                    }

                    this.UpdateGroupHeadersTable(changedGroupInfo.Index, -removedItemsCount);
                }
            }

            rowSlot = this.GetRowSlotFromFlatSlot(flatSlot);

            // Update parent groups last slot.
            CompactLayout.UpdateParentGroupInfosLastSlot(-removedItemsCount, changedGroupInfo);

            // Update TotalCount.
            this.TotalSlotCount -= removedLinesCount;

            this.RemoveFromRenderInfo(removedLinesCount, rowSlot);

            var layoutResult = new AddRemoveLayoutResult(rowSlot, removedLinesCount);
            foreach (var strategy in this.LayoutStrategies)
            {
                strategy.OnItemRemoved(layoutResult);
            }

            // return result indexes so that changed can be reflected to UI.
            return layoutResult;
        }

        protected override void SetItemsSourceOverride(IReadOnlyList<object> source, bool restoreCollapsed)
        {
            int slotsCount = 0;

            int linesCount = 0;

            this.collapsedSlotsTable.Clear();
            this.GroupHeadersTable.Clear();

            var canRestoreCollapsedState = this.itemInfoTable != null && restoreCollapsed;

            if (this.itemInfoTable != null)
            {
                ////if (restoreCollapsed)
                ////{
                ////    collapsedGroups = this.CopyCollapsedStates();
                ////}
                this.itemInfoTable.Clear();
            }

            foreach (var strategy in this.LayoutStrategies)
            {
                slotsCount += strategy.CalculateAppendedSlotsCount(this, slotsCount, ref linesCount);
            }

            this.TotalSlotCount = linesCount;

            if (this.GroupLevels == 0)
            {
                this.VisibleLineCount = this.CalculateFlatRowCount();
            }
            else
            {
                this.VisibleLineCount = linesCount;
            }

            this.RefreshRenderInfo(false);
        }

        private int GetRowSlotFromFlatSlot(int flatSlot)
        {
            int currentRowIndex = -1;
            int currentFlatSlotIndex = 0;
            GroupInfo groupInfo;
            int lowerBound;

            while (currentFlatSlotIndex < this.TotalSlotCount)
            {
                if (this.GroupHeadersTable.TryGetValue(currentFlatSlotIndex, out groupInfo, out lowerBound))
                {
                    if (groupInfo.Level == this.GroupLevels - 1 && currentFlatSlotIndex > lowerBound)
                    {
                        // lowest level group
                        if (flatSlot > groupInfo.LastSubItemSlot)
                        {
                            // slot is not in this group
                            var itemsCount = groupInfo.LastSubItemSlot - groupInfo.Index;
                            var groupRows = (int)Math.Ceiling(itemsCount / (double)this.StackCount);
                            currentRowIndex += groupRows;
                            currentFlatSlotIndex += itemsCount;
                        }
                        else
                        {
                            // slot is in this group
                            var itemsCount = flatSlot - groupInfo.Index;
                            var rows = (int)Math.Ceiling(itemsCount / (double)this.StackCount);
                            currentRowIndex += rows;
                            currentFlatSlotIndex += itemsCount;
                            break;
                        }
                    }
                    else
                    {
                        // high level group - add only its slot
                        currentFlatSlotIndex++;
                        currentRowIndex++;
                    }
                }
                else
                {
                    currentRowIndex = (int)Math.Ceiling((flatSlot + 1) / (double)this.StackCount) - 1;
                    currentFlatSlotIndex = flatSlot;
                    break;
                }
            }

            return Math.Max(currentRowIndex, 0);
        }

        private int GetAddedSlots(int totalCount, int addedCount)
        {
            return (int)(Math.Ceiling(totalCount / (double)this.StackCount) - Math.Ceiling((totalCount - addedCount) / (double)this.StackCount));
        }

        private int GetRemovedSlots(int totalCount, int removedCount)
        {
            return (int)(Math.Ceiling((totalCount + removedCount) / (double)this.StackCount) - Math.Ceiling(totalCount / (double)this.StackCount));
        }

        private void InsertToRenderInfo(int slot, double? value, int addedSlotsCount)
        {
            if (this.RenderInfo.Count != this.TotalSlotCount)
            {
                this.RenderInfo.InsertRange(slot, value, addedSlotsCount);
            }
        }
        
        private void RemoveFromRenderInfo(int removedItemsCount, int slot)
        {
            if (this.RenderInfo.Count != this.TotalSlotCount)
            {
                // Insert new records into IndexTree.
                this.RenderInfo.RemoveRange(slot, removedItemsCount);
            }
        }

        private void RemoveGroupInfo(GroupInfo groupInfo)
        {
            this.itemInfoTable.Remove(groupInfo.Item);
        }
    }
}
