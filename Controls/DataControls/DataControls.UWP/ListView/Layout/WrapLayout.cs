using System;
using System.Collections.Generic;
using System.Linq;

namespace Telerik.Data.Core.Layouts
{
    internal class WrapLayout : BaseLayout
    {
        internal double averageOppositeLength = 40;

        private int totalItemsCount;

        // private Dictionary<GroupInfo, double> groupPendingLengthChanges = new Dictionary<GroupInfo, double>();

        // groups by key
        private Dictionary<object, GroupInfo> groupInfoTable;

        // collabsed ranges
        // private IndexToValueTable<bool> collapsedSlotsTable;
        private IndexToValueTable<GroupInfo> groupHeadersTable;

        // generally group adapter
        private IHierarchyAdapter hierarchyAdapter;

        private IRenderInfo renderInfo;

        private double averageItemLength;
        private double availableOppositeLength = -1;
        private IRenderInfo columnSlotsRenderInfo;

        // Represents the blank space in each row.
        private IRenderInfo paddingRenderInfo;

        private bool isInitialized;
        private double removedItemsLength;

        public WrapLayout(IHierarchyAdapter adapter, double defaultItemLength, double defaultItemOppositeLength)
        {
            this.DefaultItemLength = defaultItemLength;
            this.DefaultItemOppositeLength = defaultItemOppositeLength;
            this.hierarchyAdapter = adapter;
            this.averageItemLength = this.DefaultItemLength;
            this.LayoutStrategies.Add(new ItemsLayoutStrategy());
            this.groupHeadersTable = new IndexToValueTable<GroupInfo>();
        }

        public double AvailableOppositeLength
        {
            get
            {
                return this.availableOppositeLength;
            }
            set
            {
                if (value > 0)
                {
                    var oldValue = this.availableOppositeLength;
                    this.availableOppositeLength = value;
                    if (oldValue != value && this.isInitialized)
                    {
                        this.OnAvailableLengthChanged(oldValue, value);
                    }
                }
            }
        }
        
        public override int GroupCount
        {
            get
            {
                return this.groupHeadersTable.IndexCount;
            }
        }

        public IRenderInfo ColumnSlotsRenderInfo
        {
            get
            {
                if (this.columnSlotsRenderInfo == null)
                {
                    this.columnSlotsRenderInfo = new IndexStorage(this.totalItemsCount, this.DefaultItemOppositeLength);
                }

                return this.columnSlotsRenderInfo;
            }
        }

        protected override IRenderInfo RenderInfo
        {
            get
            {
                if (this.renderInfo == null)
                {
                    this.Initialize();
                }

                return this.renderInfo;
            }
        }

        public void Initialize()
        {
            if (this.ItemsSource == null)
            {
                return;
            }

            if (!this.isInitialized)
            {
                this.isInitialized = true;
                this.UpdateTotalCount(this.ItemsSource);
            }
        }

        public override IEnumerable<Group> GetGroupsByKey(object key)
        {
            if (key == null)
            {
                yield break;
            }

            foreach (var pair in this.groupInfoTable)
            {
                var group = pair.Key as Group;
                if (group != null && key.Equals(group.Name))
                {
                    yield return group;
                }
            }
        }

        public override bool IsCollapsed(object item)
        {
            return false;
        }

        public override void Expand(object item)
        {
        }

        public override void Collapse(object item)
        {
        }

        public override IEnumerable<IList<ItemInfo>> GetLines(int line, bool forward)
        {
            if (this.VisibleLineCount == 0 || line < 0 || line >= this.VisibleLineCount)
            {
                yield break;
            }

            int slot = line;

            while (true)
            {
                IList<ItemInfo> itemInfos = new List<ItemInfo>();

                foreach (var strategy in this.LayoutStrategies)
                {
                    var strategyInfos = strategy.BuildItemInfos(this, line, slot);

                    if (strategyInfos != null && strategyInfos.Count > 0)
                    {
                        foreach (var item in strategyInfos)
                        {
                            itemInfos.Add(item);
                        }

                        if (itemInfos.Last().Id < this.totalItemsCount)
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
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

        internal int GetPreviousVisibleSlot(int slot)
        {
            return slot - 1;
        }

        internal override int IndexFromSlot(int slotToRequest)
        {
            if (this.DefaultItemLength == 0)
            {
                return slotToRequest;
            }

            return (int)(this.AvailableOppositeLength / this.DefaultItemOppositeLength) * slotToRequest;
        }

        internal override GroupInfo GetGroupInfo(object item)
        {
            GroupInfo groupInfo;
            if (this.groupInfoTable != null && this.groupInfoTable.TryGetValue(item, out groupInfo))
            {
                return groupInfo;
            }

            return null;
        }

        internal override int GetVisibleSlot(int index)
        {
            return index;
        }

        internal override int GetCollapsedSlotsCount(int startSlot, int endSlot)
        {
            return 0;
        }

        internal override int GetNextVisibleSlot(int slot)
        {
            return slot + 1;
        }

        internal override void RefreshRenderInfo(bool force)
        {
            if (force || (this.renderInfo != null && this.VisibleLineCount != this.renderInfo.Count))
            {
                this.renderInfo = new IndexStorage(this.TotalSlotCount, IndexStorage.UnknownItemLength);
            }

            this.columnSlotsRenderInfo = null;
            this.paddingRenderInfo = null;

            if (this.AvailableOppositeLength > 0)
            {
                this.OnAvailableLengthChanged(this.AvailableOppositeLength, this.AvailableOppositeLength);
            }
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
            int generated = endIndex - startIndex;
            if (generated < 1)
            {
                this.averageItemLength = this.RenderInfo.ValueForIndex(startIndex);
            }
            else
            {
                this.averageItemLength = (this.RenderInfo.OffsetFromIndex(endIndex) - this.RenderInfo.OffsetFromIndex(startIndex)) / generated;
            }

            if (this.AvailableOppositeLength > 0 && this.ItemsSource != null && this.ItemsSource.Count > 0)
            {
                // Round to int but exclude calculation error due to rendering not on device pixels.
                var estimatedSlots = (int)((this.ColumnSlotsRenderInfo.OffsetFromIndex(this.totalItemsCount - 1) + this.paddingRenderInfo.OffsetFromIndex(this.TotalSlotCount - 1)) / this.AvailableOppositeLength + 0.9);

                if (estimatedSlots > this.TotalSlotCount)
                {
                    this.OnAvailableLengthChanged(this.AvailableOppositeLength, this.AvailableOppositeLength);
                }
            }
        }

        internal override AddRemoveLayoutResult AddItem(object changedItem, object addRemoveItem, int addRemoveItemIndex)
        {
            int count = 1;
            int totalLines = 0;

            bool isVisible = true;
            var groupInfo = this.GetGroupInfo(changedItem);
            bool changedGroupIsRoot = groupInfo == null;
            int index = addRemoveItemIndex;

            int level = changedGroupIsRoot ? 0 : groupInfo.Level + 1;

            // We added/removed group so we need to index all subgroups.
            if (addRemoveItem is IGroup && changedItem != addRemoveItem)
            {
                index = this.GetInsertedGroupSlot(changedItem, addRemoveItemIndex);
                isVisible = groupInfo != null ? (groupInfo.IsExpanded && groupInfo.IsVisible()) : true;

                // We give a list in which to insert groups so that we can manually correct the indexes in groupHeadersTable.
                List<GroupInfo> insertedGroups = new List<GroupInfo>();
                count = this.CountAndPopulateTables(addRemoveItem, index, level, this.GroupLevels, groupInfo, false, insertedGroups, ref totalLines);

                bool first = true;
                int innerMostSlot = index;
                foreach (var newGroupInfo in insertedGroups)
                {
                    int groupInfoToInsertSpan = newGroupInfo.GetLineSpan();
                    if (first)
                    {
                        first = false;
                        this.groupHeadersTable.InsertIndexes(newGroupInfo.Index, groupInfoToInsertSpan);
                        if (isVisible)
                        {
                            this.ColumnSlotsRenderInfo.InsertRange(index, IndexStorage.UnknownItemLength, count);
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
                // We added new item  in flat scenario.
                if (changedGroupIsRoot)
                {
                    if (isVisible)
                    {
                        var columnsCount = Math.Floor(this.availableOppositeLength / this.DefaultItemOppositeLength);
                        totalLines = (int)Math.Ceiling((this.totalItemsCount + count) / columnsCount) - (int)Math.Ceiling(this.totalItemsCount / columnsCount);
                        this.ColumnSlotsRenderInfo.InsertRange(index, this.DefaultItemOppositeLength, count);
                    }

                    // slot should be already correct.
                    // No need to correct collapsed slots. They should be empty (e.g. no groups to collapse).
                    System.Diagnostics.Debug.Assert(this.groupHeadersTable.IsEmpty, "GroupHeaders table should be empty.");
                    //// System.Diagnostics.Debug.Assert(this.collapsedSlotsTable.IsEmpty, "CollapsedSlots table should be empty since we don't have groups.");
                }
                else
                {
                    // We added new item (not group) into existing bottom level group (not root group).
                    index = groupInfo.Index + 1 + addRemoveItemIndex;

                    // Update the group last slot.
                    groupInfo.LastSubItemSlot += count;
                    this.groupHeadersTable.InsertIndex(index);

                    isVisible = groupInfo.IsExpanded && groupInfo.IsVisible();
                    if (isVisible)
                    {
                        if (groupInfo.Level == this.GroupLevels - 1)
                        {
                            var columnsCount = Math.Floor(this.availableOppositeLength / this.DefaultItemOppositeLength);

                            // get children count. 
                            var childrenCount = (groupInfo.Item as IGroup).Items.Count;
                            totalLines = (int)Math.Ceiling(childrenCount / columnsCount) - (int)Math.Ceiling((childrenCount - count) / columnsCount);
                        }
                    }

                    this.UpdateGroupHeadersTable(groupInfo.Index, count);

                    this.ColumnSlotsRenderInfo.InsertRange(index, this.DefaultItemOppositeLength, count);
                }
            }

            // Update parent groups last slot.
            UpdateParentGroupInfosLastSlot(count, groupInfo);

            // Update TotalCount.
            this.VisibleLineCount += totalLines;
            this.TotalSlotCount += totalLines;
            this.totalItemsCount += count;

            // Update Visible line count if not collapsed.
            double length = isVisible ? this.averageItemLength : 0;

            // Insert new records into IndexTree.
            var insertRowIndex = this.CalculateNextRowPosition(index);
            this.RenderInfo.InsertRange(insertRowIndex, IndexStorage.UnknownItemLength, totalLines);
            this.OnAvailableLengthChanged(this.AvailableOppositeLength, this.AvailableOppositeLength);

            var layoutResult = new AddRemoveLayoutResult(index, count);

            foreach (var strategy in this.LayoutStrategies)
            {
                strategy.OnItemAdded(layoutResult);
            }

            // return result indexes so that changed can be reflected to UI.
            return layoutResult;
        }
        
        internal override AddRemoveLayoutResult RemoveItem(object changedItem, object addRemoveItem, int addRemoveItemIndex)
        {
            int totalRemovedLines = 0;

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

                // TODO: VVG --  fix this, we should take into account the fact that there is an int number of items in a row
                totalRemovedLines = (int)Math.Ceiling((this.VisibleLineCount * this.DefaultItemOppositeLength) / this.availableOppositeLength) - (int)Math.Ceiling(((this.VisibleLineCount - count) * this.DefaultItemOppositeLength) / this.availableOppositeLength);

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

                this.groupInfoTable.Remove(removedGroup);
                this.groupHeadersTable.RemoveIndexesAndValues(slot, count);

                if (isVisible)
                {
                    if (groupInfo.IsExpanded)
                    {
                        int collapsedItems = count - this.GetCollapsedSlotsCount(slot, slot + count - 1);

                        this.removedItemsLength += this.ColumnSlotsRenderInfo.ValueForIndex(slot);
                        if (this.availableOppositeLength - this.removedItemsLength < this.DefaultItemOppositeLength)
                        {
                            this.removedItemsLength = 0;
                        }
                    }
                    else
                    {
                        // TODO: Deal with this when implementing collapsible groups.
                        // count = 1;
                    }

                    totalRemovedLines = count;
                }

                if (changedGroupInfo != null)
                {
                    // Update the group last slot.
                    changedGroupInfo.LastSubItemSlot -= count;
                }
            }
            else
            {
                // We removed an item (not group) from root group.
                if (changedGroupIsRoot)
                {
                    if (isVisible)
                    {
                        var columnsCount = Math.Floor(this.availableOppositeLength / this.DefaultItemOppositeLength);
                        totalRemovedLines = (int)Math.Ceiling(this.totalItemsCount / (double)columnsCount) - (int)Math.Ceiling((this.totalItemsCount - count) / columnsCount);
                    }

                    // slot should be already correct.
                    // No need to correct collapsed slots. They should be empty (e.g. no groups to collapse).
                    System.Diagnostics.Debug.Assert(this.groupHeadersTable.IsEmpty, "GroupHeaders table should be empty.");
                    //// System.Diagnostics.Debug.Assert(this.collapsedSlotsTable.IsEmpty, "CollapsedSlots table should be empty since we don't have groups.");
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
                        var changedGroupItemsCount = changedGroupInfo.LastSubItemSlot + count - changedGroupInfo.Index;
                        int itemsPerRow = (int)(this.availableOppositeLength / this.DefaultItemOppositeLength);
                        if (changedGroupItemsCount % itemsPerRow == count)
                        {
                            totalRemovedLines++;
                        }
                    }

                    this.UpdateGroupHeadersTable(changedGroupInfo.Index, -count);
                }
            }

            // Update parent groups last slot.
            UpdateParentGroupInfosLastSlot(-count, changedGroupInfo);

            // Update TotalCount.
            this.VisibleLineCount -= totalRemovedLines;
            this.totalItemsCount -= count;
            this.TotalSlotCount -= totalRemovedLines;

            if (this.RenderInfo.Count > 0 && totalRemovedLines > 0)
            {
                var removeRowIndex = this.CalculateNextRowPosition(slot);
                this.RenderInfo.RemoveRange(removeRowIndex, totalRemovedLines);
            }

            this.ColumnSlotsRenderInfo.RemoveRange(slot, count);
            this.OnAvailableLengthChanged(this.AvailableOppositeLength, this.AvailableOppositeLength);

            var layoutResult = new AddRemoveLayoutResult(slot, count);
            foreach (var strategy in this.LayoutStrategies)
            {
                strategy.OnItemRemoved(layoutResult);
            }

            // return result indexes so that changed can be reflected to UI.
            return layoutResult;
        }

        internal override bool IsItemCollapsed(int slot)
        {
            return false;
        }

        internal override IRenderInfoState GetRenderLoadState()
        {
            return null;
        }

        internal int GetLastProjectedId(int slot)
        {
            int visibleLine = slot;

            if (this.ItemsSource == null || visibleLine > this.VisibleLineCount - 1)
            {
                return -1;
            }

            // TODO refactor
            double itemsWidth = 0;

            var padding = this.paddingRenderInfo.OffsetFromIndex(slot);
            var startOffset = Math.Max(0, this.availableOppositeLength * visibleLine - padding);
            var startColumnIndex = startOffset > 0 ? this.ColumnSlotsRenderInfo.IndexFromOffset(startOffset) : -1;

            while (itemsWidth < this.availableOppositeLength && startColumnIndex < this.ColumnSlotsRenderInfo.Count - 1)
            {
                if (visibleLine < this.VisibleLineCount)
                {
                    startColumnIndex++;
                    itemsWidth += this.ColumnSlotsRenderInfo.ValueForIndex(startColumnIndex);
                }
            }

            return startColumnIndex;
        }

        internal override IList<ItemInfo> GetItemInfosAtSlot(int visibleLine, int rowSlot)
        {
            if (this.ItemsSource == null || visibleLine > this.VisibleLineCount - 1)
            {
                return new List<ItemInfo>();
            }

            var paddingOffset = this.paddingRenderInfo != null && visibleLine > 0 ? Math.Max(0, this.paddingRenderInfo.OffsetFromIndex(visibleLine - 1)) : 0;
            var startOffset = Math.Max(0, this.availableOppositeLength * visibleLine) - paddingOffset;
            var columnSlot = startOffset > 0 ? this.ColumnSlotsRenderInfo.IndexFromOffset(startOffset) + 1 : 0;

            List<ItemInfo> items = new List<ItemInfo>();
            if (this.TryGetGroupedItemsAtColumnSlot(visibleLine, rowSlot, columnSlot, ref items))
            {
                return items;
            }
            else
            {
                double itemsWidth = 0;
                ItemInfo itemInfo = new ItemInfo();
                itemInfo.IsDisplayed = true;

                // Add buffered items in case the projected items cannot fill the viewport. 
                var avalaibleLength = 2 * this.availableOppositeLength;

                while (itemsWidth < avalaibleLength && columnSlot < this.ColumnSlotsRenderInfo.Count && visibleLine < this.VisibleLineCount)
                {
                    itemInfo.Item = this.ItemsSource[columnSlot];
                    itemInfo.Id = columnSlot;
                    itemInfo.Slot = rowSlot;

                    itemInfo.IsCollapsible = false;
                    itemInfo.IsCollapsed = false;
                    itemInfo.ItemType = BaseLayout.GetItemType(itemInfo.Item);

                    itemsWidth += this.ColumnSlotsRenderInfo.ValueForIndex(columnSlot);
                    columnSlot++;

                    items.Add(itemInfo);
                }
                return items;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", Justification = "Not a real issue.")]
        internal override int CountAndPopulateTables(object item, int rootSlot, int level, int levels, GroupInfo parent, bool shouldIndexItem, List<GroupInfo> insert, ref int totalLines)
        {
            int treeCount = 1;
            var subItems = this.hierarchyAdapter.GetItems(item);

            IGroup group = item as IGroup;
            GroupInfo itemGroupInfo = null;

            // Grouped scenario
            if (level <= levels - 1)
            {
                bool shouldIndexChildren = false;
                int totalChildrenCount = 0;

                foreach (var subItem in subItems)
                {
                    if (itemGroupInfo == null)
                    {
                        itemGroupInfo = new GroupInfo(item, parent, true, level, rootSlot, rootSlot + treeCount - 1);
                    }

                    var childrenCount = this.CountAndPopulateTables(subItem, rootSlot + treeCount, level + 1, levels, itemGroupInfo, shouldIndexChildren, insert, ref totalLines);
                    shouldIndexChildren = shouldIndexChildren || childrenCount > 1;
                    totalChildrenCount += childrenCount;
                    treeCount += childrenCount;
                }

                // Leaves
                if (level == levels - 1)
                {
                    totalLines += (int)Math.Ceiling((double)totalChildrenCount / ((int)(this.availableOppositeLength / this.DefaultItemOppositeLength)));
                }
                totalLines++;
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

                if (this.groupInfoTable == null)
                {
                    this.groupInfoTable = new Dictionary<object, GroupInfo>();
                }

                this.groupInfoTable.Add(item, itemGroupInfo);
            }

            return treeCount;
        }

        internal override int CalculateFlatRowCount()
        {
            return (int)Math.Ceiling((double)this.ItemsSource.Count / ((int)(this.availableOppositeLength / this.DefaultItemOppositeLength)));
        }

        internal void EndSlotMeasure(int generatedSlot, int startLineColumn, int projectedGeneratedColumn, int actualGeneratedColumn)
        {
            var startItemOffset = startLineColumn > 0 ? this.ColumnSlotsRenderInfo.OffsetFromIndex(startLineColumn - 1) : 0;
            var endItemOffset = this.ColumnSlotsRenderInfo.OffsetFromIndex(actualGeneratedColumn);
            var itemLength = this.ColumnSlotsRenderInfo.ValueForIndex(actualGeneratedColumn);
            var lastGeneratedRowLength = endItemOffset - startItemOffset;

            var rowSpacing = this.availableOppositeLength - lastGeneratedRowLength;

            if (this.paddingRenderInfo != null)
            {
                this.paddingRenderInfo.Update(generatedSlot, Math.Min(rowSpacing, this.availableOppositeLength));
            }
        }

        protected override void SetItemsSourceOverride(IReadOnlyList<object> source, bool restoreCollapsed)
        {
            if (this.availableOppositeLength <= 0)
            {
                return;
            }

            this.UpdateTotalCount(source);
        }

        private static void UpdateParentGroupInfosLastSlot(int count, GroupInfo groupInfo)
        {
            GroupInfo parentGroupInfo = groupInfo != null ? groupInfo.Parent : null;
            while (parentGroupInfo != null)
            {
                parentGroupInfo.LastSubItemSlot += count;
                parentGroupInfo = parentGroupInfo.Parent;
            }
        }

        private void OnAvailableLengthChanged(double oldValue, double newValue)
        {
            if (this.ItemsSource == null)
            {
                return;
            }

            int slotCount = 0;
            double currentColumnLength = 0;

            this.paddingRenderInfo = new IndexStorage(0);

            for (int i = 0; i < this.ColumnSlotsRenderInfo.Count; i++)
            {
                List<ItemInfo> items = new List<ItemInfo>();
                var length = this.ColumnSlotsRenderInfo.ValueForIndex(i);

                if (this.TryGetGroupedItemsAtColumnSlot(slotCount, slotCount, i, ref items) && items.Count > 0 && items.Last().Item is IGroup)
                {
                    if (currentColumnLength > 0)
                    {
                        var paddingValue = Math.Max(0, newValue - currentColumnLength);
                        this.paddingRenderInfo.Add(paddingValue);
                        currentColumnLength = 0;
                        slotCount++;
                    }

                    this.ColumnSlotsRenderInfo.Update(i, newValue);
                    this.paddingRenderInfo.Add(0);
                    currentColumnLength = 0;
                    slotCount++;
                    continue;
                }
                else
                {
                    if (currentColumnLength + length <= newValue)
                    {
                        currentColumnLength += length;
                    }
                    else
                    {
                        var paddingValue = Math.Max(0, newValue - currentColumnLength);
                        this.paddingRenderInfo.Add(paddingValue);
                        slotCount++;
                        currentColumnLength = length;
                    }
                }
            }

            if (this.LayoutStrategies.Where(c => c is PlaceholderStrategy).Any())
            {
                if (currentColumnLength + this.DefaultItemOppositeLength > newValue)
                {
                    slotCount++;
                    currentColumnLength %= newValue;
                }
            }

            if (currentColumnLength > 0)
            {
                var paddingValue = Math.Max(0, newValue - currentColumnLength);
                this.paddingRenderInfo.Add(paddingValue);
                currentColumnLength = 0;
                slotCount++;
            }

            this.groupHeadersTable.Clear();

            if (this.groupInfoTable != null)
            {
                this.groupInfoTable.Clear();
            }

            int slotsCount = 0;
            int levels = this.GroupLevels;
            this.totalItemsCount = slotsCount;
            int totalLines = 0;

            foreach (var strategy in this.LayoutStrategies)
            {
                this.totalItemsCount += strategy.CalculateAppendedSlotsCount(this, 0, ref totalLines);
            }

            if (this.TotalSlotCount != slotCount)
            {
                // todo update
                this.TotalSlotCount = totalLines;
                this.VisibleLineCount = totalLines;

                // TODO find a better way to update this.
                this.renderInfo = new IndexStorage(this.TotalSlotCount, IndexStorage.UnknownItemLength);
            }
        }

        private bool TryGetGroupedItemsAtColumnSlot(int visibleLine, int slot, int columnSlot, ref List<ItemInfo> items)
        {
            ItemInfo itemInfo = new ItemInfo();
            itemInfo.IsDisplayed = true;

            GroupInfo groupInfo;
            int lowerBound;

            if (this.groupHeadersTable.TryGetValue(columnSlot, out groupInfo, out lowerBound))
            {
                if (lowerBound != columnSlot)
                {
                    var avalaibleLength = 2 * this.availableOppositeLength;
                    double itemsWidth = 0;

                    while (itemsWidth < avalaibleLength && columnSlot < this.ColumnSlotsRenderInfo.Count)
                    {
                        if (visibleLine < this.VisibleLineCount)
                        {
                            int childIndex = columnSlot - lowerBound - 1;

                            if (groupInfo.LastSubItemSlot - groupInfo.Index <= childIndex)
                            {
                                break;
                            }

                            var childItem = this.hierarchyAdapter.GetItemAt(groupInfo.Item, childIndex);
                            itemInfo.Item = childItem;

                            itemInfo.Level = groupInfo.Level + 1;
                            itemInfo.Id = groupInfo.Index + childIndex + 1;
                            itemInfo.Slot = slot;

                            itemInfo.IsCollapsible = false;
                            itemInfo.IsCollapsed = false;
                            itemInfo.ItemType = BaseLayout.GetItemType(itemInfo.Item);

                            itemsWidth += this.ColumnSlotsRenderInfo.ValueForIndex(columnSlot);
                            columnSlot++;

                            items.Add(itemInfo);
                        }
                        else
                        {
                            break;
                        }
                    }

                    var parentItemInfo = new ItemInfo();
                    parentItemInfo.IsDisplayed = false;
                    parentItemInfo.Id = groupInfo.Index;
                    parentItemInfo.Item = groupInfo.Item;
                    parentItemInfo.Level = groupInfo.Level;
                    parentItemInfo.Slot = this.CalculateNextRowPosition(groupInfo.Index);
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
                    itemInfo.Slot = slot;

                    itemInfo.IsCollapsed = !groupInfo.IsExpanded;
                    itemInfo.ItemType = BaseLayout.GetItemType(itemInfo.Item);

                    //// Denote the current slot as a group header as it will be stretched to  fill the entire opposite length
                    this.ColumnSlotsRenderInfo.Update(itemInfo.Id, this.AvailableOppositeLength);

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
                    parentItemInfo.Slot = this.CalculateNextRowPosition(groupInfo.Index);
                    parentItemInfo.IsCollapsed = !parentGroupInfo.IsExpanded;
                    parentItemInfo.ItemType = BaseLayout.GetItemType(parentGroupInfo.Item);
                    parentItemInfo.IsSummaryVisible = parentItemInfo.ItemType == GroupType.Subtotal && parentGroupInfo.Parent != null && this.IsCollapsed(parentGroupInfo.Parent.Item);

                    items.Insert(0, parentItemInfo);
                    parentGroupInfo = parentGroupInfo.Parent;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private int CalculateNextRowPosition(int index)
        {
            var offset = 0.0;
            int currentRow = 0;

            for (int i = 0; i <= index; i++)
            {
                List<ItemInfo> items = new List<ItemInfo>();
                var length = this.ColumnSlotsRenderInfo.ValueForIndex(i);
                if (length == 0)
                {
                    length = this.DefaultItemOppositeLength;
                }

                if (offset + length + this.paddingRenderInfo.ValueForIndex(currentRow) > this.AvailableOppositeLength)
                {
                    currentRow++;
                    offset = length;
                }
                else
                {
                    offset += length;
                }
            }
            return currentRow;
        }

        private void UpdateTotalCount(IReadOnlyList<object> source)
        {
            if (source != null)
            {
                this.groupHeadersTable.Clear();

                if (this.groupInfoTable != null)
                {
                    this.groupInfoTable.Clear();
                }

                int slotsCount = 0;
                int levels = this.GroupLevels;
                int slotsCountToAdd = slotsCount;
                this.totalItemsCount = slotsCount;
                int totalLines = 0;

                foreach (var strategy in this.LayoutStrategies)
                {
                    this.totalItemsCount += strategy.CalculateAppendedSlotsCount(this, slotsCount, ref totalLines);
                }

                if (this.GroupLevels == 0)
                {
                    totalLines = (int)Math.Ceiling(this.totalItemsCount / Math.Max(1d, (int)(this.availableOppositeLength / this.DefaultItemOppositeLength)));
                    this.totalItemsCount = this.ItemsSource.Count;
                }

                slotsCount += totalLines;

                // todo update
                this.TotalSlotCount = slotsCount;
                this.VisibleLineCount = slotsCount;

                this.RefreshRenderInfo(false);
                this.Initialize();
            }
        }

        private void UpdateGroupHeadersTable(int groupIndex, int count)
        {
            // Update groups after current one.
            foreach (var nextGroupIndex in this.groupHeadersTable.GetIndexes(groupIndex + 1))
            {
                GroupInfo nextGroupInfo = this.groupHeadersTable.GetValueAt(nextGroupIndex);
                nextGroupInfo.Index += count;
                nextGroupInfo.LastSubItemSlot += count;
            }
        }

        private void RemoveGroupInfo(GroupInfo groupInfo)
        {
            this.groupInfoTable.Remove(groupInfo.Item);
        }

        private int GetInsertedGroupSlot(object changedItem, int itemIndex)
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
    }
}