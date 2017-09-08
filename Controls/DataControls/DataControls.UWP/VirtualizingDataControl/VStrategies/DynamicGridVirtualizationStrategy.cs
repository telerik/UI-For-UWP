using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Telerik.Core;
using Telerik.Core.Data;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class DynamicGridVirtualizationStrategy : VirtualizationStrategy
    {
        internal double itemExtent;

        // private readonly StaggeredRenderInfo itemSlotsRepository;
        private readonly Dictionary<int, int> slotsRepository;

        private readonly List<RadVirtualizingDataControlItem> topRealized = new List<RadVirtualizingDataControlItem>();
        private int stackCount;

        internal DynamicGridVirtualizationStrategy()
        {
            this.slotsRepository = new Dictionary<int, int>();
        }

        internal int StackCount
        {
            get
            {
                return this.stackCount;
            }
            set
            {
                if (this.stackCount != value)
                {
                    if (value < 2)
                    {
                        throw new ArgumentException("Stack count must be at least 2.");
                    }

                    this.stackCount = value;
                }
            }
        }

        internal override Orientation LayoutOrientation
        {
            get
            {
                return this.orientationCache == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
            }
        }

        internal override Size Measure(Size availableSize)
        {
            Size measuredSize = base.Measure(availableSize);

            this.itemExtent = this.ViewportExtent / this.stackCount;

            this.ValidateLayoutIntegrity();

            return measuredSize;
        }

        internal override void InitForMeasure()
        {
            this.itemExtent = this.ViewportExtent / this.stackCount;
        }

        internal override void OnAfterItemRemovedAnimationEnded(SingleItemAnimationContext context)
        {
            this.ReorderViewportItemsOnItemRemoved(context.RealizedIndex, context.AssociatedItem);

            if (this.owner.IsLoaded)
            {
                this.ManageLowerViewport(false);
                this.CheckBottomScrollableBounds();
            }
        }

        internal override void ReorderViewportItemsOnItemRemoved(int removedAt, RadVirtualizingDataControlItem removedContainer)
        {
            if (removedAt < this.owner.realizedItems.Count)
            {
                IDataSourceItem firstRealized = this.owner.realizedItems[removedAt].associatedDataItem;

                while (firstRealized != null)
                {
                    this.slotsRepository.Remove(firstRealized.GetHashCode());
                    firstRealized = firstRealized.Next;
                }

                this.ReorderViewportOnItemsChanged();
            }
        }

        internal override void ReorderViewportItemsOnItemRemovedFromTop(IDataSourceItem removedItem)
        {
            IDataSourceItem firstRealized = this.owner.listSource.GetItemAt(removedItem.Index);

            while (firstRealized != null)
            {
                this.slotsRepository.Remove(firstRealized.GetHashCode());
                firstRealized = firstRealized.Previous;
            }
        }

        internal override void ReorderViewportItemsOnItemAddedOnTop(IDataSourceItem addedItem)
        {
            IDataSourceItem firstDataItem = addedItem.Previous;

            while (firstDataItem != null)
            {
                this.slotsRepository.Remove(firstDataItem.GetHashCode());
                firstDataItem = firstDataItem.Previous;
            }
        }

        internal override void ReorderViewportItemsOnItemAdded(int physicalChangeLocation, RadVirtualizingDataControlItem addedItem)
        {
            this.owner.OnContainerStateChanged(addedItem, addedItem.associatedDataItem, ItemState.Realized);

            int slot = -1;

            // TODO rearange items
            if (this.slotsRepository.TryGetValue(addedItem.next.associatedDataItem.GetHashCode(), out slot))
            {
                this.slotsRepository.Remove(addedItem.next.associatedDataItem.GetHashCode());
                this.slotsRepository.Add(addedItem.associatedDataItem.GetHashCode(), slot);
                RadVirtualizingDataControlItem nextContainer = addedItem.next;
                addedItem.SetHorizontalOffset(nextContainer.horizontalOffsetCache);
                addedItem.SetVerticalOffset(nextContainer.verticalOffsetCache);
            }

            RadVirtualizingDataControlItem pivotContainer = addedItem.next;

            int containersToSkip = 0;

            if (pivotContainer != null)
            {
                containersToSkip = Math.Max(this.stackCount - this.owner.realizedItems.IndexOf(pivotContainer), 0);
            }

            while (pivotContainer != null)
            {
                if (containersToSkip > 0)
                {
                    containersToSkip--;
                    continue;
                }

                this.slotsRepository.Remove(pivotContainer.associatedDataItem.Index);
                pivotContainer = pivotContainer.next;
            }

            this.ReorderViewportOnItemsChanged();
        }

        internal override void ReorderViewportItemsOnItemReplaced(RadVirtualizingDataControlItem replacedItem)
        {
            RadVirtualizingDataControlItem pivotContainer = replacedItem.next;

            int containersToSkip = 0;

            // TODO: rearange items.
            if (pivotContainer != null)
            {
                containersToSkip = Math.Max(this.stackCount - this.owner.realizedItems.IndexOf(pivotContainer), 0);
            }

            while (pivotContainer != null)
            {
                if (containersToSkip > 0)
                {
                    containersToSkip--;
                    continue;
                }
                this.slotsRepository.Remove(pivotContainer.associatedDataItem.Index);
                pivotContainer = pivotContainer.next;
            }

            this.ReorderViewportOnItemsChanged();
        }

        internal override void OnSourceCollectionReset()
        {
            base.OnSourceCollectionReset();

            // this.itemSlotsRepository.Clear();
            this.slotsRepository.Clear();
        }

        internal override bool IsViewportFilled(double visibleItemsBottom)
        {
            int realizedItemsCount = this.owner.realizedItems.Count;

            if (realizedItemsCount == 0)
            {
                return false;
            }

            return visibleItemsBottom > this.ViewportLength + this.ScrollOffset;
        }

        internal override void MeasureContainer(RadVirtualizingDataControlItem container)
        {
            switch (this.orientationCache)
            {
                case Orientation.Horizontal:
                    container.Width = this.itemExtent;
                    container.Measure(new Size(this.itemExtent, double.PositiveInfinity));
                    break;

                case Orientation.Vertical:
                    container.Height = this.itemExtent;
                    container.Measure(new Size(double.PositiveInfinity, this.itemExtent));
                    break;
            }
            container.InvalidateCachedSize();

            base.MeasureContainer(container);
        }

        internal override void OnViewportSizeChanged(Size newSize, Size oldSize)
        {
            if (this.lastViewportSize == newSize)
            {
                base.OnViewportSizeChanged(newSize, oldSize);
                return;
            }

            base.OnViewportSizeChanged(newSize, oldSize);

            // do not balance if layout is still not updated
            if (!this.owner.layoutUpdated)
            {
                return;
            }

            if (this.owner.realizedItems.Count == 0)
            {
                return;
            }

            bool dirty = false;
            switch (this.orientationCache)
            {
                case Orientation.Horizontal:
                    dirty = newSize.Width != oldSize.Width;
                    break;

                case Orientation.Vertical:
                    dirty = newSize.Height != oldSize.Height;
                    break;
            }

            if (!dirty)
            {
                return;
            }

            IDataSourceItem initialItem = this.owner.realizedItems[0].associatedDataItem;
            while (this.owner.realizedItems.Count > 0)
            {
                this.owner.RecycleFirstItem();
            }
            this.slotsRepository.Clear();
            this.owner.initialVirtualizationItem = initialItem;
            this.owner.BalanceVisualSpace();
        }

        internal override void OnContainerSizeChanged(RadVirtualizingDataControlItem container, Size newSize, Size oldSize)
        {
            base.OnContainerSizeChanged(container, newSize, oldSize);

            if (!this.topRealized.Contains(container))
            {
                RadVirtualizingDataControlItem pivotContainer = container.next;

                int containersToSkip = 0;

                if (pivotContainer != null)
                {
                    containersToSkip = Math.Max(this.stackCount - this.owner.realizedItems.IndexOf(pivotContainer), 0);
                }

                while (pivotContainer != null)
                {
                    if (containersToSkip > 0)
                    {
                        containersToSkip--;
                        continue;
                    }

                    this.slotsRepository.Remove(pivotContainer.associatedDataItem.GetHashCode());
                    pivotContainer = pivotContainer.next;
                }

                this.ReorderViewportOnItemsChanged();
            }
            else
            {
                double newOffset = 0;
                double offsetDelta = 0;
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        offsetDelta = newSize.Height - oldSize.Height;
                        newOffset = container.verticalOffsetCache - offsetDelta;
                        container.SetVerticalOffset(newOffset);
                        double horizontalOffset = container.horizontalOffsetCache;
                        while (container.previous != null)
                        {
                            container = container.previous;
                            if (container.horizontalOffsetCache == horizontalOffset)
                            {
                                container.SetVerticalOffset(container.verticalOffsetCache - offsetDelta);
                            }
                        }
                        break;

                    case Orientation.Vertical:
                        offsetDelta = newSize.Width - oldSize.Width;
                        newOffset = container.horizontalOffsetCache - offsetDelta;
                        container.SetHorizontalOffset(newOffset);
                        double verticalOffset = container.verticalOffsetCache;

                        while (container.previous != null)
                        {
                            container = container.previous;
                            if (container.verticalOffsetCache == verticalOffset)
                            {
                                container.SetHorizontalOffset(container.horizontalOffsetCache - offsetDelta);
                            }
                        }
                        break;
                }
            }

            // Extend the length with the new diff, although the extension could be less
            var prevLength = this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal ? oldSize.Height : oldSize.Width;

            var sizeChange = this.GetItemLength(container) - prevLength;

            this.CorrectScrollableContentSize(sizeChange);

            this.EnsureCorrectLayout();

            this.CheckBottomScrollableBounds();
        }

        internal override void OnOrientationChanged(Orientation newValue)
        {
            base.OnOrientationChanged(newValue);

            if (this.owner == null || !this.owner.IsTemplateApplied)
            {
                return;
            }

            if (this.owner.GetItemCount() > 0)
            {
                this.owner.StopAllAddedAnimations();
                this.owner.StopAllRemovedAnimations();

                while (this.owner.realizedItems.Count > 0)
                {
                    RadVirtualizingDataControlItem lastItem = this.owner.realizedItems[this.owner.realizedItems.Count - 1];
                    lastItem.SetVerticalOffset(0);
                    lastItem.SetHorizontalOffset(0);
                    this.owner.RecycleLastItem();
                }

                this.owner.BeginAsyncBalance();
                this.owner.BalanceVisualSpace();
            }
        }

        internal override double GetItemRelativeOffset(RadVirtualizingDataControlItem item)
        {
            switch (this.orientationCache)
            {
                case Orientation.Horizontal:
                    return item.verticalOffsetCache;

                case Orientation.Vertical:
                    return item.horizontalOffsetCache;
            }

            return 0;
        }

        internal override RadVirtualizingDataControlItem GetTopVisibleContainer()
        {
            //// The approach here is to calculate the possible
            //// index of the topmost item by considering the upper buffer size
            //// and the average item height. In the ideal case of having equal height
            //// containers, the index will be calculated exactly. In case of wrong index calculation
            //// we estimate the direction we have to take in order to find the topmost item and
            //// interate to it.
            if (this.owner.realizedItems.Count == 0)
            {
                return null;
            }

            if (this.averageItemLength == 0)
            {
                return this.owner.firstItemCache;
            }

            double topThresholdAbs = Math.Abs(Math.Max(this.GetItemRelativeOffset(this.owner.firstItemCache), this.topVirtualizationThreshold));
            int countOfTopItems = Math.Min((int)(((topThresholdAbs / this.averageItemLength) + 1) * this.stackCount), this.owner.realizedItems.Count - 1);
            RadVirtualizingDataControlItem pivotItem = this.owner.realizedItems[countOfTopItems];
            int deltaFactor = -1;
            int realizedItemsCount = this.owner.realizedItems.Count;

            var relativeOffset = double.MaxValue;

            for (int i = countOfTopItems; i > -1 && i < realizedItemsCount; i += deltaFactor)
            {
                pivotItem = this.owner.realizedItems[i];

                if (relativeOffset >= this.GetItemRelativeOffset(this.owner.realizedItems[i]))
                {
                    pivotItem = this.owner.realizedItems[i];
                    relativeOffset = this.GetItemRelativeOffset(pivotItem);
                }
            }

            return pivotItem;
        }

        internal override double GetRealizedItemsBottom()
        {
            if (this.owner.realizedItems.Count == 0)
            {
                return 0;
            }

            double bottomMost = double.MinValue;

            foreach (RadVirtualizingDataControlItem item in this.owner.realizedItems)
            {
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        if (item.height + item.verticalOffsetCache > bottomMost)
                        {
                            bottomMost = item.height + item.verticalOffsetCache;
                        }
                        break;

                    case Orientation.Vertical:
                        if (item.width + item.horizontalOffsetCache > bottomMost)
                        {
                            bottomMost = item.width + item.horizontalOffsetCache;
                        }
                        break;
                }
            }

            return bottomMost;
        }

        internal override double GetRealizedItemsTop()
        {
            if (this.owner.realizedItems.Count == 0)
            {
                return 0;
            }

            double topMost = double.MaxValue;

            foreach (RadVirtualizingDataControlItem item in this.owner.realizedItems)
            {
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        if (item.verticalOffsetCache < topMost)
                        {
                            topMost = item.verticalOffsetCache;
                        }
                        break;

                    case Orientation.Vertical:
                        if (item.horizontalOffsetCache < topMost)
                        {
                            topMost = item.horizontalOffsetCache;
                        }
                        break;
                }
            }

            return topMost;
        }

        internal override bool CanRecycleTop(double visibleItemsTop)
        {
            return this.owner.realizedItems.Count > 1 &&
                   this.GetElementCanvasOffset(this.owner.firstItemCache) + this.GetItemLength(this.owner.firstItemCache) - this.ScrollOffset < this.topVirtualizationThreshold;
        }

        internal override bool CanRecycleBottom(double visibleItemsBottom)
        {
            return this.owner.realizedItems.Count > 1 &&
                  visibleItemsBottom - this.GetItemLength(this.owner.lastItemCache) > this.ViewportLength + this.bottomVirtualizationThreshold + this.ScrollOffset;
        }

        internal override void RecycleBottom(ref double visibleItemsBottom)
        {
            this.slotsRepository.Remove(this.owner.lastItemCache.associatedDataItem.GetHashCode());

            this.owner.RecycleLastItem();

            visibleItemsBottom = this.GetRealizedItemsBottom();
        }

        internal override void RecycleTop(ref double visibleItemsTop)
        {
            this.owner.RecycleFirstItem();

            visibleItemsTop = this.GetRealizedItemsTop();
        }

        internal override void RecycleItem(RadVirtualizingDataControlItem item, bool setVisibility)
        {
            this.topRealized.Remove(item);
            base.RecycleItem(item, setVisibility);
        }

        internal override bool CanRealizeBottom(double visibleItemsBottom)
        {
            if (this.owner.realizedItems.Count == 0)
            {
                return true;
            }

            return this.GetElementCanvasOffset(this.owner.lastItemCache) +
                   this.GetItemLength(this.owner.lastItemCache) -
                   this.ScrollOffset < this.ViewportLength + this.bottomVirtualizationThreshold;
        }

        internal override bool PositionTopRealizedItem(ref double visibleItemsTop)
        {
            this.topRealized.Add(this.owner.firstItemCache);
            this.PositionRealizedItemTop(this.owner.firstItemCache, ref visibleItemsTop);
            return true;
        }

        internal override bool CanRealizeTop(double visibleItemsTop)
        {
            if (this.owner.realizedItems.Count == 0)
            {
                return true;
            }

            return this.GetElementCanvasOffset(this.owner.firstItemCache) - this.ScrollOffset > this.topVirtualizationThreshold;
        }

        internal override void RecalculateViewportMeasurements()
        {
            base.RecalculateViewportMeasurements();

            if (this.owner.realizedItems.Count == 0)
            {
                return;
            }

            this.EnsureCorrectLayout();

            var realizedLength = this.owner.lastItemCache.CurrentOffset + this.GetItemLength(this.owner.lastItemCache) - this.owner.firstItemCache.CurrentOffset;

            if (realizedLength >= 0)
            {
                this.realizedItemsLength = realizedLength;
                this.scrollableItemsLength = this.averageItemLength * (this.owner.GetItemCount() / this.stackCount);
            }
        }

        internal override double CalculateItemOffset(IDataSourceItem item, double lastAverageLength)
        {
            if (item == null)
            {
                return 0;
            }

            var index = item.Index;

            return lastAverageLength * index / this.StackCount;
        }

        internal override bool IsItemSizeChangeValid(Size previousSize, Size newSize)
        {
            return previousSize.Width != newSize.Width || previousSize.Height != newSize.Height;
        }

        internal override void ResetRealizationStartWhenLowerUIBufferRecycled(double position)
        {
            double bottomDifference = position - this.GetItemLength(this.owner.lastItemCache);
            if (bottomDifference > this.bottomVirtualizationThreshold + this.ScrollOffset)
            {
                int rowCount = (int)Math.Round(bottomDifference / this.averageItemLength, 0);
                int itemCount = rowCount * this.stackCount;
                RadVirtualizingDataControlItem firstRealizedDataItem = this.owner.FirstRealizedDataItem;
                int currentFirstItemIndex = 0;
                if (firstRealizedDataItem != null)
                {
                    currentFirstItemIndex = Math.Max(firstRealizedDataItem.associatedDataItem.Index - itemCount, 0);
                }

                IDataSourceItem newDataItem = firstRealizedDataItem.associatedDataItem;

                while (newDataItem.Index > currentFirstItemIndex)
                {
                    newDataItem = newDataItem.Previous;

                    Debug.Assert(newDataItem != null, "The currentLastItemIndex should be within the bounds of the flattened view of the collection.");
                }

                while (this.owner.realizedItems.Count > 0)
                {
                    this.RecycleItem(this.owner.realizedItems[0]);
                }

                var newRealized = this.GetContainerForItem(newDataItem, false);
                double currentTop = this.ScrollOffset;
                this.PositionBottomRealizedItem(this.owner.lastItemCache, ref currentTop);
            }
        }

        internal override void ResetRealizationStartWhenUpperUIBufferRecycled(double position)
        {
            double topDifference = position + this.GetItemLength(this.owner.lastItemCache);
            if (topDifference < this.topVirtualizationThreshold)
            {
                int rowCount = (int)Math.Round(Math.Abs(topDifference) / this.averageItemLength, 0);
                int itemCount = rowCount * this.stackCount;
                RadVirtualizingDataControlItem lastRealizedDataItem = this.owner.LastRealizedDataItem;
                int lastEvenIndex = lastRealizedDataItem.associatedDataItem.Index - ((lastRealizedDataItem.associatedDataItem.Index + 1) % this.stackCount) + 1;
                int currentLastItemIndex = this.owner.GetDataItemCount();

                if (lastRealizedDataItem != null)
                {
                    currentLastItemIndex = Math.Min(lastEvenIndex + itemCount, currentLastItemIndex - this.stackCount);
                }

                IDataSourceItem newDataItem = lastRealizedDataItem.associatedDataItem;

                while (newDataItem.Index < currentLastItemIndex)
                {
                    newDataItem = newDataItem.Next;

                    Debug.Assert(newDataItem != null, "The currentLastItemIndex should be within the bounds of the flattened view of the collection.");
                }

                while (this.owner.realizedItems.Count > 0)
                {
                    this.RecycleItem(this.owner.realizedItems[0]);
                }

                var newRealized = this.GetContainerForItem(newDataItem, false);
                double currentTop = this.ScrollOffset;
                this.PositionBottomRealizedItem(this.owner.lastItemCache, ref currentTop);
            }
        }

        internal override bool PositionBottomRealizedItem(RadVirtualizingDataControlItem lastRealizedItem, ref double visibleItemsBottom)
        {
            Rect slot = this.FindFreeSpotForItemBottom(lastRealizedItem);

            switch (this.orientationCache)
            {
                case Orientation.Horizontal:
                    visibleItemsBottom = slot.Bottom;
                    lastRealizedItem.SetHorizontalOffset(slot.Left);
                    lastRealizedItem.SetVerticalOffset(slot.Top);
                    this.slotsRepository[lastRealizedItem.associatedDataItem.GetHashCode()] = (int)(slot.Left / this.itemExtent);
                    break;

                case Orientation.Vertical:
                    visibleItemsBottom = slot.Right;
                    lastRealizedItem.SetVerticalOffset(slot.Top);
                    lastRealizedItem.SetHorizontalOffset(slot.Left);
                    this.slotsRepository[lastRealizedItem.associatedDataItem.GetHashCode()] = (int)(slot.Top / this.itemExtent);
                    break;
            }

            return true;
        }

        internal override void EnsureCorrectLayout()
        {
            if (this.owner.RealizedItems.Length == 0)
            {
                base.EnsureCorrectLayout();
                return;
            }

            bool isLayoutCorrect = true;

            var item = this.owner.firstItemCache;

            for (int i = 0; i < this.StackCount; i++)
            {
                if (item == null)
                {
                    return;
                }

                var rowIndex = item.AssociatedDataItem.Index / this.StackCount;

                if (rowIndex == 0)
                {
                    isLayoutCorrect = RadMath.AreClose(this.GetItemRelativeOffset(item), 0, 0.01);

                    if (!isLayoutCorrect)
                    {
                        break;
                    }
                }

                item = item.next;
            }

            var firstRealizedItem = this.GetTopVisibleContainer();

            if (firstRealizedItem == null)
            {
                return;
            }

            var startPosition = firstRealizedItem.CurrentOffset;

            var manipulationOffset = 0.0;

            if (this.owner != null && this.owner.manipulationContainer != null)
            {
                manipulationOffset = this.ScrollOffset;
            }

            if (!isLayoutCorrect ||
     (startPosition + manipulationOffset < 0 && manipulationOffset < this.averageItemLength))
            {
                // reposition all
                this.owner.firstItemCache = this.owner.realizedItems[0];
                var container = this.owner.firstItemCache;

                var size = 0.0;

                while (container != null)
                {
                    container.horizontalOffsetCache = 0;
                    container.verticalOffsetCache = 0;
                    container = container.next;
                }

                container = this.owner.firstItemCache;

                while (container != null)
                {
                    this.PositionBottomRealizedItem(container, ref size);
                    container = container.next;
                }
            }
        }

        private void ValidateLayoutIntegrity()
        {
        }

        private void ReorderViewportOnItemsChanged()
        {
            foreach (RadVirtualizingDataControlItem item in this.owner.realizedItems)
            {
                Rect slot = this.FindFreeSpotForItemBottom(item);
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        item.SetHorizontalOffset(slot.Left);
                        item.SetVerticalOffset(slot.Top);
                        this.slotsRepository[item.associatedDataItem.GetHashCode()] = (int)(slot.Left / this.itemExtent);
                        break;

                    case Orientation.Vertical:
                        item.SetVerticalOffset(slot.Top);
                        item.SetHorizontalOffset(slot.Left);
                        this.slotsRepository[item.associatedDataItem.GetHashCode()] = (int)(slot.Top / this.itemExtent);
                        break;
                }
            }
        }

        private Rect FindFreeSpotForItemTop(RadVirtualizingDataControlItem item)
        {
            RadVirtualizingDataControlItem startingItem = item.next;

            Dictionary<int, Rect> slots = new Dictionary<int, Rect>();
            int stackIndex = -1;

            while (startingItem != null)
            {
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        stackIndex = (int)Math.Round(startingItem.horizontalOffsetCache / this.itemExtent);
                        break;

                    case Orientation.Vertical:
                        stackIndex = (int)Math.Round(startingItem.verticalOffsetCache / this.itemExtent);
                        break;
                }

                if (!slots.ContainsKey(stackIndex))
                {
                    slots.Add(
                        stackIndex,
                        new Rect(startingItem.horizontalOffsetCache, startingItem.verticalOffsetCache, startingItem.width, startingItem.height));
                }

                if (slots.Count == this.stackCount)
                {
                    break;
                }

                startingItem = startingItem.next;
            }

            Rect? slot = null;

            Rect[] sortedValues = new Rect[this.stackCount];

            Rect[] sorted = this.orientationCache == Orientation.Horizontal ? slots.Values.OrderByDescending<Rect, double>(r => r.Left).ToArray<Rect>() :
                            slots.Values.OrderByDescending<Rect, double>(r => r.Top).ToArray<Rect>();
            sorted.CopyTo(sortedValues, 0);
            if (sorted.Length < this.stackCount)
            {
                int difference = sorted.Length;
                for (int i = difference; i < this.stackCount; i++)
                {
                    int emptySlot = 0;

                    switch (this.orientationCache)
                    {
                        case Orientation.Horizontal:

                            while (slots.ContainsKey(emptySlot))
                            {
                                emptySlot++;
                            }

                            sortedValues[i] = new Rect(emptySlot * this.itemExtent, Math.Max(0, this.ScrollOffset + this.topVirtualizationThreshold), 0, 0);
                            slots.Add(emptySlot, sortedValues[i]);

                            break;

                        case Orientation.Vertical:
                            while (slots.ContainsKey(emptySlot))
                            {
                                emptySlot++;
                            }

                            sortedValues[i] = new Rect(emptySlot * this.itemExtent, Math.Max(0, this.ScrollOffset + this.topVirtualizationThreshold), 0, 0);
                            slots.Add(emptySlot, sortedValues[i]);

                            break;
                    }
                }
            }

            foreach (Rect freeSlot in sortedValues)
            {
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        if (slot == null)
                        {
                            slot = freeSlot;
                            continue;
                        }
                        else
                        {
                            if (freeSlot.Top > slot.Value.Top)
                            {
                                slot = freeSlot;
                            }
                        }
                        break;

                    case Orientation.Vertical:
                        if (slot == null)
                        {
                            slot = freeSlot;
                            continue;
                        }
                        else
                        {
                            if (freeSlot.Left > slot.Value.Left)
                            {
                                slot = freeSlot;
                            }
                        }
                        break;
                }
            }

            if (slot == null)
            {
                slot = new Rect();
            }
            else
            {
                Rect valueSlot = slot.Value;
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        slot = new Rect(valueSlot.Left, valueSlot.Top - item.height, item.width, item.height);
                        break;

                    case Orientation.Vertical:
                        slot = new Rect(valueSlot.Left - item.width, valueSlot.Top, item.width, item.height);
                        break;
                }
            }

            return slot.Value;
        }

        private Rect FindFreeSpotForItemBottom(RadVirtualizingDataControlItem item)
        {
            RadVirtualizingDataControlItem startingItem = item.previous;

            Dictionary<int, Rect> slots = new Dictionary<int, Rect>();
            int stackIndex = -1;

            while (startingItem != null)
            {
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        stackIndex = (int)Math.Round(startingItem.horizontalOffsetCache / this.itemExtent);
                        break;

                    case Orientation.Vertical:
                        stackIndex = (int)Math.Round(startingItem.verticalOffsetCache / this.itemExtent);
                        break;
                }

                if (!slots.ContainsKey(stackIndex))
                {
                    slots.Add(
                        stackIndex,
                        new Rect(startingItem.horizontalOffsetCache, startingItem.verticalOffsetCache, startingItem.width, startingItem.height));
                }

                if (slots.Count == this.stackCount)
                {
                    break;
                }
                startingItem = startingItem.previous;
            }

            Rect[] sortedValues = new Rect[this.stackCount];

            Rect[] sorted = this.orientationCache == Orientation.Horizontal ? slots.Values.OrderBy<Rect, double>(r => r.Left).ToArray<Rect>() :
                            slots.Values.OrderBy<Rect, double>(r => r.Top).ToArray<Rect>();
            sorted.CopyTo(sortedValues, 0);

            if (sorted.Length < this.stackCount)
            {
                var approximatePosition = this.averageItemLength > 0 ? item.AssociatedDataItem.Index / this.StackCount * this.averageItemLength : this.ScrollOffset;

                int difference = sorted.Length;
                for (int i = difference; i < this.stackCount; i++)
                {
                    var freeStackIndex = 0;

                    for (int j = 0; j < this.stackCount; j++)
                    {
                        if (!slots.ContainsKey(j))
                        {
                            freeStackIndex = j;
                            slots.Add(freeStackIndex, new Rect());
                            break;
                        }
                    }

                    switch (this.orientationCache)
                    {
                        case Orientation.Horizontal:

                            sortedValues[i] = new Rect(freeStackIndex * this.itemExtent, approximatePosition, 0, 0);
                            break;

                        case Orientation.Vertical:
                            sortedValues[i] = new Rect(approximatePosition, freeStackIndex * this.itemExtent, 0, 0);
                            break;
                    }
                }
            }

            Rect? slot = null;
            foreach (Rect freeSlot in sortedValues)
            {
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        if (slot == null)
                        {
                            slot = freeSlot;
                            continue;
                        }

                        if (freeSlot.Bottom < slot.Value.Bottom)
                        {
                            slot = freeSlot;
                        }
                        break;

                    case Orientation.Vertical:
                        if (slot == null)
                        {
                            slot = freeSlot;
                            continue;
                        }

                        if (freeSlot.Right < slot.Value.Right)
                        {
                            slot = freeSlot;
                        }
                        break;
                }
            }

            if (slot == null)
            {
                slot = new Rect();
            }
            else
            {
                Rect valueSlot = slot.Value;
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        slot = new Rect(valueSlot.Left, valueSlot.Bottom, item.width, item.height);
                        break;

                    case Orientation.Vertical:
                        slot = new Rect(valueSlot.Right, valueSlot.Top, item.width, item.height);
                        break;
                }
            }

            return slot.Value;
        }

        private void PositionRealizedItemTop(RadVirtualizingDataControlItem firstRealizedItem, ref double visibleItemsTop)
        {
            Rect slot = this.FindFreeSpotForItemTop(firstRealizedItem);

            switch (this.orientationCache)
            {
                case Orientation.Horizontal:
                    firstRealizedItem.SetHorizontalOffset(slot.Left);
                    firstRealizedItem.SetVerticalOffset(slot.Top);
                    visibleItemsTop = slot.Top - this.ScrollOffset;
                    break;

                case Orientation.Vertical:
                    firstRealizedItem.SetVerticalOffset(slot.Top);
                    firstRealizedItem.SetHorizontalOffset(slot.Left);
                    visibleItemsTop = slot.Left - this.ScrollOffset;
                    break;
            }

            this.EnsureCorrectLayout();
        }
    }
}