using System;
using System.Collections.Generic;
using System.Diagnostics;
using Telerik.Core.Data;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class WrapVirtualizationStrategy : VirtualizationStrategy
    {
        internal WrapLineAlignment wrapLineAlignment = WrapLineAlignment.Near;
        internal List<WrapRow> wrapRows;
        internal double allItemsExtent;
        internal double averageItemCountPerRow;
        internal int guessedRowCount;
        internal WrapRow lastWrapRow;
        internal WrapRow firstWrapRow;

        internal WrapVirtualizationStrategy()
        {
            this.wrapRows = new List<WrapRow>();
        }

        internal WrapLineAlignment WrapLineAlignment
        {
            get
            {
                return this.wrapLineAlignment;
            }
            set
            {
                if (this.wrapLineAlignment != value)
                {
                    WrapLineAlignment oldValue = this.wrapLineAlignment;
                    this.wrapLineAlignment = value;
                    this.OnWrapLineAlignmentChanged(oldValue, value);
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

        internal virtual double GetItemRowOffset(RadVirtualizingDataControlItem item)
        {
            if (this.orientationCache == Orientation.Vertical)
            {
                return item.verticalOffsetCache;
            }

            return item.horizontalOffsetCache;
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

        internal override void ReorderViewportItemsOnItemRemoved(int removedAt, RadVirtualizingDataControlItem removedCOntainer)
        {
            if (this.owner.realizedItems.Count == 0)
            {
                this.wrapRows.Clear();
                this.firstWrapRow = null;
                this.lastWrapRow = null;
                return;
            }

            WrapRow parentRow = this.GetWrapRowToStartReorderFrom(removedAt, false);

            parentRow.rowLength = this.GetItemLength(parentRow.firstItem);

            this.ReorderViewportItemsStartingAtRow(parentRow);
        }

        internal override void ReorderViewportItemsOnItemRemovedFromTop(IDataSourceItem removedItem)
        {
            //// We do this because we want to shift the realized items list
            //// backwards with one item
            this.owner.RecycleFirstItem();

            this.ReorderViewportItemsStartingAtRow(this.GetWrapRowToStartReorderFrom(0, false));
        }

        internal override void ReorderViewportItemsOnItemAddedOnTop(IDataSourceItem addedItem)
        {
            RadVirtualizingDataControlItem firstRealized = this.owner.firstItemCache;
            IDataSourceItem prev = this.owner.GetItemBefore(firstRealized.associatedDataItem);
            RadVirtualizingDataControlItem newFirst = this.GetContainerForItem(prev, false);
            newFirst.wrapRow = firstRealized.wrapRow;
            newFirst.wrapRow.firstItem = newFirst;
            this.ReorderViewportItemsStartingAtRow(newFirst.wrapRow);
        }

        internal override void ReorderViewportItemsOnItemAdded(int physicalChangeLocation, RadVirtualizingDataControlItem addedItem)
        {
            WrapRow parentRow = this.GetWrapRowToStartReorderFrom(physicalChangeLocation, true);
            parentRow.rowLength = 0;
            this.owner.OnContainerStateChanged(addedItem, addedItem.associatedDataItem, ItemState.Realized);
            this.ReorderViewportItemsStartingAtRow(parentRow);
        }

        internal override void ReorderViewportItemsOnItemReplaced(RadVirtualizingDataControlItem replacedItem)
        {
            WrapRow parentRow = replacedItem.wrapRow.firstItem == replacedItem ? replacedItem.wrapRow.previous != null ? replacedItem.wrapRow.previous : replacedItem.wrapRow : replacedItem.wrapRow;

            parentRow.rowLength = 0;

            this.ReorderViewportItemsStartingAtRow(parentRow);
        }

        internal override bool IsViewportFilled(double visibleItemsBottom)
        {
            int wrapRowCount = this.wrapRows.Count;

            if (wrapRowCount == 0)
            {
                return false;
            }

            return visibleItemsBottom - this.wrapRows[wrapRowCount - 1].rowLength > this.ViewportLength;
        }

        internal override RadVirtualizingDataControlItem GetContainerForItem(IDataSourceItem item, int insertAt)
        {
            RadVirtualizingDataControlItem container = base.GetContainerForItem(item, insertAt);
            this.allItemsExtent += this.GetItemExtent(container);
            return container;
        }

        internal override void MeasureContainer(RadVirtualizingDataControlItem container)
        {
            double availableWidth = this.owner.availableWidth;
            double availableHeight = this.owner.availableHeight;
            switch (this.orientationCache)
            {
                case Orientation.Horizontal:
                    container.Measure(new Size(availableWidth, double.PositiveInfinity));
                    break;

                case Orientation.Vertical:
                    container.Measure(new Size(double.PositiveInfinity, availableHeight));
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

            if (this.wrapRows.Count == 0)
            {
                return;
            }

            bool isSizeChangeRelevant = this.orientationCache == Orientation.Horizontal ? newSize.Width != oldSize.Width : newSize.Height != oldSize.Height;
            if (isSizeChangeRelevant)
            {
                WrapRow topVisibleRow = this.GetTopVisibleContainer().wrapRow;
                IDataSourceItem itemToBringIntoView = topVisibleRow.firstItem.associatedDataItem;

                while (this.owner.firstItemCache != topVisibleRow.firstItem)
                {
                    this.owner.RecycleFirstItem();
                }

                this.ReorderViewportItemsStartingAtRow(this.firstWrapRow);
                this.owner.BringIntoView(itemToBringIntoView.Value);
            }
        }

        internal override void OnContainerSizeChanged(RadVirtualizingDataControlItem container, Size newSize, Size oldSize)
        {
            base.OnContainerSizeChanged(container, newSize, oldSize);

            WrapRow parentRow = container.wrapRow.firstItem == container ? container.wrapRow.previous != null ? container.wrapRow.previous : container.wrapRow : container.wrapRow;
            this.allItemsExtent += this.orientationCache == Orientation.Horizontal ? newSize.Width - oldSize.Width : newSize.Height - oldSize.Height;
            parentRow.rowLength = 0;
            this.ReorderViewportItemsStartingAtRow(parentRow);

            this.RecalculateViewportMeasurements();

            if (this.owner.scheduledRemoveAnimations.Count == 0)
            {
                this.CheckBottomScrollableBounds();
            }
        }

        internal virtual void OnWrapLineAlignmentChanged(WrapLineAlignment oldValue, WrapLineAlignment newValue)
        {
            if (this.wrapRows.Count > 0)
            {
                this.ReorderViewportItemsStartingAtRow(this.firstWrapRow);
            }
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
                this.wrapRows.Clear();

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

        internal override void EnsureCorrectLayout()
        {
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

            if ((startPosition != 0 && firstRealizedItem.AssociatedDataItem.Index == 0) ||
     (startPosition + manipulationOffset < 0 && manipulationOffset < this.averageItemLength))
            {
                this.owner.RecycleAllItems();
            }
        }

        internal override RadVirtualizingDataControlItem GetTopVisibleContainer()
        {
            //// The approach here is to calculate the possible
            //// index of the topmost item by considering the upper buffer size
            //// and the average item height. In the ideal case of having equal height
            //// containers, the index will be calculated exactly. In case of wrong index calculation
            //// we estimate the direction we have to take in order to find the topmost item and
            //// interate to it.
            if (this.wrapRows.Count == 0)
            {
                return null;
            }

            if (this.averageItemLength == 0)
            {
                return this.owner.firstItemCache;
            }

            double topThresholdAbs = Math.Abs(Math.Max(this.GetItemRelativeOffset(this.owner.firstItemCache), this.topVirtualizationThreshold));
            int countOfTopItems = Math.Min((int)(topThresholdAbs / this.averageItemLength), this.wrapRows.Count - 1);
            WrapRow topRow = this.wrapRows[countOfTopItems];
            int deltaFactor = -1;
            int realizedItemsCount = this.wrapRows.Count;

            var relativeOffset = double.MaxValue;

            for (int i = countOfTopItems; i > -1 && i < realizedItemsCount; i += deltaFactor)
            {
                if (relativeOffset >= this.GetItemRelativeOffset(this.wrapRows[i].firstItem))
                {
                    topRow = this.wrapRows[i];
                    relativeOffset = this.GetItemRelativeOffset(topRow.firstItem);
                }
            }

            return topRow.firstItem;
        }

        internal override double GetRealizedItemsBottom()
        {
            if (this.owner.realizedItems.Count == 0)
            {
                return 0;
            }

            WrapRow lastRow = this.wrapRows[this.wrapRows.Count - 1];
            switch (this.orientationCache)
            {
                case Orientation.Horizontal:
                    return Canvas.GetTop(this.owner.lastItemCache) + lastRow.rowLength;

                case Orientation.Vertical:
                    return Canvas.GetLeft(lastRow.firstItem) + lastRow.rowLength;
            }
            return 0;
        }

        internal override double GetRealizedItemsTop()
        {
            if (this.owner.realizedItems.Count == 0)
            {
                return 0;
            }
            WrapRow firstRow = this.wrapRows[0];
            switch (this.orientationCache)
            {
                case Orientation.Horizontal:
                    return Canvas.GetTop(firstRow.firstItem);

                case Orientation.Vertical:
                    return Canvas.GetLeft(firstRow.firstItem);
            }
            return 0;
        }

        internal override bool CanRecycleTop(double visibleItemsTop)
        {
            return this.wrapRows.Count > 1 &&
                   visibleItemsTop + this.firstWrapRow.rowLength - this.ScrollOffset < this.topVirtualizationThreshold;
        }

        internal override void RecycleTop(ref double visibleItemsTop)
        {
            WrapRow firstRow = this.wrapRows[0];
            this.RemoveWrapRowAt(0);
            RadVirtualizingDataControlItem firstRowItem = firstRow.firstItem;
            int recycledItemsCount = 0;
            while (firstRowItem != null && firstRowItem.wrapRow == firstRow)
            {
                RadVirtualizingDataControlItem nextItem = firstRowItem.next;
                this.owner.ClearContainerForItemInternal(firstRowItem, firstRowItem.associatedDataItem);
                firstRowItem = nextItem;
                recycledItemsCount++;
            }

            visibleItemsTop += firstRow.rowLength;
        }

        internal override void RecycleItem(RadVirtualizingDataControlItem item, bool setVisibility)
        {
            this.allItemsExtent -= this.GetItemExtent(item);
            RadVirtualizingDataControlItem previousItem = item.previous;
            RadVirtualizingDataControlItem nextItem = item.next;
            base.RecycleItem(item, setVisibility);
            WrapRow parentRow = item.wrapRow;
            item.wrapRow = null;

            if (parentRow != null &&
                parentRow.firstItem == item && parentRow.lastItem == item)
            {
                this.RemoveWrapRowAt(this.wrapRows.IndexOf(parentRow));
            }
            else if (parentRow != null)
            {
                //// If the recycled item was the first on the row
                if (parentRow.firstItem == item)
                {
                    if (nextItem != null)
                    {
                        parentRow.firstItem = nextItem;
                    }
                }
                else if (parentRow.lastItem == item)
                {
                    if (nextItem != null)
                    {
                        parentRow.lastItem = nextItem;
                    }
                    else
                    {
                        parentRow.lastItem = previousItem;
                    }
                }
            }
        }

        internal override bool CanRealizeBottom(double visibleItemsBottom)
        {
            bool canFitVertically = visibleItemsBottom -
                   this.ScrollOffset < this.ViewportLength + this.bottomVirtualizationThreshold;

            return canFitVertically;
        }

        internal override bool PositionBottomRealizedItem(RadVirtualizingDataControlItem item, ref double visibleItemsBottom)
        {
            RadVirtualizingDataControlItem realizedItem = item;
            double itemLength = this.GetItemLength(realizedItem);
            WrapRow lastWrapRow = null;
            int wrapRowsCount = this.wrapRows.Count;
            if (wrapRowsCount > 0)
            {
                lastWrapRow = this.wrapRows[wrapRowsCount - 1];
            }

            bool isEnoughSpaceForItemInRow = true;

            if (lastWrapRow != null)
            {
                isEnoughSpaceForItemInRow = this.CheckEnoughSpaceForItemInRow(lastWrapRow, realizedItem, true);
            }

            if (lastWrapRow == null || !isEnoughSpaceForItemInRow)
            {
                if (!isEnoughSpaceForItemInRow)
                {
                    visibleItemsBottom += lastWrapRow.rowLength;
                    this.OnRowFilled(lastWrapRow, true);
                    if (!this.CanRealizeBottom(visibleItemsBottom))
                    {
                        return false;
                    }
                }

                lastWrapRow = this.InsertWrapRowAt(wrapRowsCount);
                lastWrapRow.firstItem = realizedItem;

                if (lastWrapRow.previous == null)
                {
                    double defaultPosition = 0;

                    if (this.owner.scrollScheduled)
                    {
                        defaultPosition = this.ScrollOffset;
                    }

                    lastWrapRow.rowOffset = this.averageItemLength > 0 ? item.AssociatedDataItem.Index / this.averageItemCountPerRow * this.averageItemLength : defaultPosition;
                }
                else
                {
                    lastWrapRow.rowOffset = lastWrapRow.previous.rowOffset + lastWrapRow.previous.rowLength;
                }

                lastWrapRow.rowLength = itemLength;
            }

            double currentRowLength = lastWrapRow.rowLength;

            switch (this.orientationCache)
            {
                case Orientation.Horizontal:
                    if (currentRowLength < realizedItem.height)
                    {
                        lastWrapRow.rowLength = realizedItem.height;
                        visibleItemsBottom += realizedItem.height - currentRowLength;
                    }
                    realizedItem.SetVerticalOffset(lastWrapRow.rowOffset);
                    if (lastWrapRow.lastItem == null)
                    {
                        realizedItem.SetHorizontalOffset(0);
                    }
                    else
                    {
                        realizedItem.SetHorizontalOffset(lastWrapRow.lastItem.horizontalOffsetCache + lastWrapRow.lastItem.width);
                    }
                    break;

                case Orientation.Vertical:
                    if (currentRowLength < realizedItem.width)
                    {
                        lastWrapRow.rowLength = realizedItem.width;
                        visibleItemsBottom += realizedItem.width - currentRowLength;
                    }
                    realizedItem.SetHorizontalOffset(lastWrapRow.rowOffset);
                    if (lastWrapRow.lastItem == null)
                    {
                        realizedItem.SetVerticalOffset(0);
                    }
                    else
                    {
                        realizedItem.SetVerticalOffset(lastWrapRow.lastItem.verticalOffsetCache + lastWrapRow.lastItem.height);
                    }
                    break;
            }

            lastWrapRow.lastItem = realizedItem;
            realizedItem.wrapRow = lastWrapRow;

            if (this.owner.IsLastItemLastInListSource())
            {
                this.OnRowFilled(lastWrapRow, true);
            }

            return true;
        }

        internal override bool CanRecycleBottom(double visibleItemsBottom)
        {
            return this.wrapRows.Count > 1 &&
                   visibleItemsBottom - this.lastWrapRow.rowLength - this.ScrollOffset > this.ViewportLength + this.bottomVirtualizationThreshold;
        }

        internal override void RecycleBottom(ref double visibleItemsBottom)
        {
            int wrapRowsLastRowIndex = this.wrapRows.Count - 1;

            WrapRow lastRow = this.wrapRows[wrapRowsLastRowIndex];
            this.RemoveWrapRowAt(wrapRowsLastRowIndex);

            RadVirtualizingDataControlItem lastRowItem = lastRow.lastItem;
            int recycledItemsCount = 0;
            while (lastRowItem != null && lastRowItem.wrapRow == lastRow)
            {
                recycledItemsCount++;
                RadVirtualizingDataControlItem prevItem = lastRowItem.previous;

                this.owner.ClearContainerForItemInternal(lastRowItem, lastRowItem.associatedDataItem);

                lastRowItem = prevItem;
            }

            visibleItemsBottom -= lastRow.rowLength;
        }

        internal override bool CanRealizeTop(double visibleItemsTop)
        {
            return visibleItemsTop - this.ScrollOffset > this.topVirtualizationThreshold;
        }

        internal override bool PositionTopRealizedItem(ref double visibleItemsTop)
        {
            RadVirtualizingDataControlItem firstRealizedItem = this.owner.firstItemCache;
            WrapRow firstWrapRow = null;
            int wrapRowsCount = this.wrapRows.Count;
            if (wrapRowsCount > 0)
            {
                firstWrapRow = this.wrapRows[0];
            }
            bool isEnoughSpaceForItemInRow = this.CheckEnoughSpaceForItemInRow(firstWrapRow, firstRealizedItem, false);
            if (firstWrapRow == null || !isEnoughSpaceForItemInRow)
            {
                if (!isEnoughSpaceForItemInRow)
                {
                    visibleItemsTop -= firstWrapRow.rowLength;
                    this.OnRowFilled(firstWrapRow, false);
                    if (!this.CanRealizeTop(visibleItemsTop))
                    {
                        return false;
                    }
                }

                firstWrapRow = this.InsertWrapRowAt(0);
                firstWrapRow.lastItem = firstRealizedItem;
                firstWrapRow.rowLength = this.GetItemLength(firstRealizedItem);
                firstWrapRow.rowOffset = firstWrapRow.next != null ? firstWrapRow.next.rowOffset - firstWrapRow.rowLength : 0;
            }

            double currentRowLength = firstWrapRow.rowLength;
            double difference = 0;
            switch (this.orientationCache)
            {
                case Orientation.Horizontal:
                    difference = firstRealizedItem.height - currentRowLength;
                    if (currentRowLength < firstRealizedItem.height)
                    {
                        firstWrapRow.rowLength = firstRealizedItem.height;
                        visibleItemsTop -= difference;
                        firstWrapRow.rowOffset -= difference;
                    }

                    firstRealizedItem.SetVerticalOffset(firstWrapRow.rowOffset);

                    if (firstWrapRow.firstItem == null)
                    {
                        firstRealizedItem.SetHorizontalOffset(this.owner.availableWidth - firstRealizedItem.width);
                    }
                    else
                    {
                        firstRealizedItem.SetHorizontalOffset(firstWrapRow.firstItem.horizontalOffsetCache - firstRealizedItem.width);

                        if (difference != 0)
                        {
                            this.SynchItemOffsetInRow(firstWrapRow);
                        }
                    }
                    break;

                case Orientation.Vertical:
                    difference = firstRealizedItem.width - currentRowLength;
                    if (currentRowLength < firstRealizedItem.width)
                    {
                        firstWrapRow.rowLength = firstRealizedItem.width;
                        visibleItemsTop -= difference;
                        firstWrapRow.rowOffset -= difference;
                    }
                    firstRealizedItem.SetHorizontalOffset(firstWrapRow.rowOffset);
                    if (firstWrapRow.firstItem == null)
                    {
                        firstRealizedItem.SetVerticalOffset(this.owner.availableHeight - firstRealizedItem.height);
                    }
                    else
                    {
                        firstRealizedItem.SetVerticalOffset(firstWrapRow.firstItem.verticalOffsetCache - firstRealizedItem.height);
                        if (difference != 0)
                        {
                            this.SynchItemOffsetInRow(firstWrapRow);
                        }
                    }
                    break;
            }

            firstWrapRow.firstItem = firstRealizedItem;
            firstRealizedItem.wrapRow = firstWrapRow;

            if (this.owner.IsFirstItemFirstInListSource())
            {
                this.OnRowFilled(firstWrapRow, false);
            }

            return true;
        }

        internal override void RecalculateViewportMeasurements()
        {
            base.RecalculateViewportMeasurements();

            if (this.owner.realizedItems.Count == 0)
            {
                return;
            }

            this.averageItemCountPerRow = (this.owner.RealizedItems.Length / (double)this.wrapRows.Count + this.averageItemCountPerRow) / 2.0;

            var lastRealizedindex = this.owner.LastRealizedDataItem.AssociatedDataItem.Index;
            int itemCount = this.owner.GetItemCount() - lastRealizedindex + 1;

            this.guessedRowCount = (int)(itemCount / this.averageItemCountPerRow);
            this.scrollableItemsLength = this.GetRealizedItemsBottom() + this.averageItemLength * this.guessedRowCount;

            this.realizedItemsLength = this.GetRealizedItemsBottom() - this.owner.FirstRealizedDataItem.CurrentOffset;
        }

        internal override bool IsItemSizeChangeValid(Size previousSize, Size newSize)
        {
            return previousSize.Width != newSize.Width || previousSize.Height != newSize.Height;
        }

        internal virtual void OnRowFilled(WrapRow row, bool isLast)
        {
            if (this.wrapLineAlignment != WrapLineAlignment.Near || !isLast)
            {
                this.CorrectRowItemsOffsetWhenRowFilled(row);
            }

            var item = row.firstItem;
            if (item != null)
            {
                this.generatedItemsLength[item.AssociatedDataItem.Index] = row.rowLength;
            }

            while (item != row.lastItem)
            {
                item = item.next;
                if (item != null)
                {
                    this.generatedItemsLength[item.AssociatedDataItem.Index] = row.rowLength;
                }
            }
        }

        internal virtual double GetRowExtent(WrapRow row)
        {
            if (this.orientationCache == Orientation.Horizontal)
            {
                return row.lastItem.horizontalOffsetCache + row.lastItem.width - row.firstItem.horizontalOffsetCache;
            }

            return row.lastItem.verticalOffsetCache + row.lastItem.height - row.firstItem.verticalOffsetCache;
        }

        internal override void ResetRealizationStartWhenLowerUIBufferRecycled(double position)
        {
            double bottomDifference = position - this.lastWrapRow.rowLength;
            if (bottomDifference > this.bottomVirtualizationThreshold)
            {
                int rowCount = (int)Math.Round(bottomDifference / this.averageItemLength, 0);
                int itemCount = (int)(rowCount * this.averageItemCountPerRow);
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

                this.GetContainerForItem(newDataItem, false);
                double currentTop = this.ScrollOffset;
                this.PositionBottomRealizedItem(this.owner.lastItemCache, ref currentTop);
            }
        }

        internal override void ResetRealizationStartWhenUpperUIBufferRecycled(double position)
        {
            double topDifference = position + this.lastWrapRow.rowLength;
            if (topDifference < this.topVirtualizationThreshold)
            {
                int rowCount = (int)Math.Round(Math.Abs(topDifference) / this.averageItemLength, 0);
                int itemCount = (int)(rowCount * this.averageItemCountPerRow);
                RadVirtualizingDataControlItem lastRealizedDataItem = this.owner.LastRealizedDataItem;
                int lastEvenIndex = lastRealizedDataItem.associatedDataItem.Index - (int)((lastRealizedDataItem.associatedDataItem.Index + 1) % this.averageItemCountPerRow) + 1;
                int currentLastItemIndex = this.owner.GetDataItemCount();

                if (lastRealizedDataItem != null)
                {
                    currentLastItemIndex = (int)Math.Min(lastEvenIndex + itemCount, currentLastItemIndex - this.averageItemCountPerRow);
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

                this.GetContainerForItem(newDataItem, false);
                double currentTop = this.ScrollOffset;
                this.PositionBottomRealizedItem(this.owner.lastItemCache, ref currentTop);
            }
        }

        internal override double CalculateItemOffset(IDataSourceItem item, double lastAverageLength)
        {
            if (item == null)
            {
                return 0;
            }

            var index = item.Index;

            return lastAverageLength * index / this.averageItemCountPerRow;
        }

        private bool CheckEnoughSpaceForItemInRow(WrapRow row, RadVirtualizingDataControlItem item, bool addingAtStart)
        {
            switch (this.orientationCache)
            {
                case Orientation.Horizontal:
                    if (addingAtStart)
                    {
                        return this.owner.availableWidth - (row.lastItem.horizontalOffsetCache + row.lastItem.width - row.firstItem.horizontalOffsetCache) >= item.width;
                    }
                    else
                    {
                        return row.firstItem.horizontalOffsetCache >= item.width;
                    }

                case Orientation.Vertical:
                    if (addingAtStart)
                    {
                        return this.owner.availableHeight - (row.lastItem.verticalOffsetCache + row.lastItem.height - row.firstItem.verticalOffsetCache) >= item.height;
                    }
                    else
                    {
                        return row.firstItem.verticalOffsetCache >= item.height;
                    }
            }

            return false;
        }

        private double GetRowOffset(WrapRow row)
        {
            if (row.previous != null)
            {
                return row.previous.rowOffset + row.previous.rowLength;
            }

            return row.rowOffset;
        }

        private WrapRow InsertWrapRowAt(int index)
        {
            if (index < 0)
            {
                return null;
            }

            WrapRow row = new WrapRow();

            int rowsCount = this.wrapRows.Count;

            this.wrapRows.Insert(index, row);

            if (index > 0)
            {
                WrapRow previousRow = this.wrapRows[index - 1];
                row.previous = previousRow;
                previousRow.next = row;
            }

            if (index < rowsCount)
            {
                WrapRow nextRow = this.wrapRows[index + 1];
                row.next = nextRow;
                nextRow.previous = row;
            }

            rowsCount++;

            this.firstWrapRow = this.wrapRows[0];
            this.lastWrapRow = this.wrapRows[rowsCount - 1];

            return row;
        }

        private void RemoveWrapRowAt(int index)
        {
            if (index < 0)
            {
                return;
            }
            WrapRow rowToRemove = this.wrapRows[index];
            int rowsCount = this.wrapRows.Count;

            if (index > 0)
            {
                rowToRemove.previous.next = rowToRemove.next;
            }

            if (index < rowsCount - 1)
            {
                rowToRemove.next.previous = rowToRemove.previous;
            }

            rowToRemove.next = null;
            rowToRemove.previous = null;

            this.wrapRows.RemoveAt(index);

            rowsCount--;

            if (rowsCount > 0)
            {
                this.firstWrapRow = this.wrapRows[0];
                this.lastWrapRow = this.wrapRows[rowsCount - 1];
            }
            else
            {
                this.firstWrapRow = null;
                this.lastWrapRow = null;
            }
        }

        private void SynchItemOffsetInRow(WrapRow row)
        {
            RadVirtualizingDataControlItem firstItem = row.firstItem;
            switch (this.orientationCache)
            {
                case Orientation.Horizontal:

                    while (true)
                    {
                        firstItem.SetVerticalOffset(row.rowOffset);
                        firstItem = firstItem.next;

                        if (firstItem.wrapRow != row)
                        {
                            break;
                        }
                    }
                    break;

                case Orientation.Vertical:
                    while (true)
                    {
                        firstItem.SetHorizontalOffset(row.rowOffset);
                        firstItem = firstItem.next;

                        if (firstItem.wrapRow != row)
                        {
                            break;
                        }
                    }
                    break;
            }
        }

        private void ReorderViewportItemsStartingAtRow(WrapRow row)
        {
            WrapRow parentRow = row;
            RadDataBoundListBoxItem processedItem = parentRow.firstItem as RadDataBoundListBoxItem;
            parentRow.lastItem = processedItem;
            processedItem.wrapRow = parentRow;
            parentRow.rowOffset = this.GetRowOffset(parentRow);
            double currentRowPosition = 0;
            while (true)
            {
                this.SetItemOffset(processedItem, parentRow.rowOffset);
                processedItem.wrapRow = parentRow;
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        processedItem.SetHorizontalOffset(currentRowPosition);
                        break;

                    case Orientation.Vertical:
                        processedItem.SetVerticalOffset(currentRowPosition);
                        break;
                }

                currentRowPosition += this.GetItemExtent(processedItem);
                double itemLength = this.GetItemLength(processedItem);
                if (itemLength > parentRow.rowLength)
                {
                    parentRow.rowLength = itemLength;
                }
                processedItem = processedItem.next as RadDataBoundListBoxItem;

                if (processedItem == null)
                {
                    int wrapRowsCount = this.wrapRows.Count;
                    this.OnRowFilled(parentRow, true);
                    while (this.lastWrapRow != parentRow)
                    {
                        int indexOfRowToRemove = --wrapRowsCount;
                        this.RemoveWrapRowAt(indexOfRowToRemove);
                    }
                    break;
                }
                else if (!this.CheckEnoughSpaceForItemInRow(parentRow, processedItem, true))
                {
                    parentRow.lastItem = processedItem.previous;
                    currentRowPosition = 0;
                    this.OnRowFilled(parentRow, false);

                    if (parentRow.next == null)
                    {
                        parentRow = this.InsertWrapRowAt(this.wrapRows.Count);
                    }
                    else
                    {
                        parentRow = parentRow.next;
                    }

                    parentRow.rowOffset = this.GetRowOffset(parentRow);
                    parentRow.firstItem = processedItem;
                    parentRow.lastItem = processedItem;
                    parentRow.rowLength = this.GetItemLength(processedItem);
                }
                else
                {
                    parentRow.lastItem = processedItem;
                }
            }
        }

        private void CorrectRowItemsOffsetWhenRowFilled(WrapRow row)
        {
            switch (this.orientationCache)
            {
                case Orientation.Horizontal:
                    this.SynchRowItemsHorizontalStartPosition(row);
                    break;

                case Orientation.Vertical:
                    this.SynchRowItemsVerticalStartPosition(row);
                    break;
            }
        }

        private WrapRow GetWrapRowToStartReorderFrom(int changeIndex, bool adding)
        {
            if (changeIndex > this.owner.realizedItems.Count - 1)
            {
                changeIndex = 0;
            }

            RadVirtualizingDataControlItem container = this.owner.realizedItems[changeIndex];
            WrapRow result = null;

            if (container.previous != null)
            {
                result = container.previous.wrapRow;
            }
            else
            {
                if (adding)
                {
                    result = container.next.wrapRow;
                    result.firstItem = container;
                }
                else
                {
                    result = container.wrapRow;
                    result.firstItem = container;
                }
            }

            if (result.firstItem.ItemState == ItemState.Recycled)
            {
                if (result.firstItem.previous != null)
                {
                    return result.firstItem.previous.wrapRow;
                }
                else if (result.firstItem.next != null)
                {
                    return result.firstItem.next.wrapRow;
                }
            }

            return result;
        }

        private void SynchRowItemsVerticalStartPosition(WrapRow row)
        {
            RadVirtualizingDataControlItem firstItem = row.firstItem;

            double currentPosition = this.wrapLineAlignment == WrapLineAlignment.Center ?
                                                                                         (this.owner.availableHeight - this.GetRowExtent(row)) / 2 : this.wrapLineAlignment == WrapLineAlignment.Near ? 0 : this.owner.availableHeight - this.GetRowExtent(row);

            while (true)
            {
                firstItem.SetVerticalOffset(currentPosition);
                currentPosition += firstItem.height;
                firstItem = firstItem.next;

                if (firstItem == null || firstItem.wrapRow != row)
                {
                    break;
                }
            }
        }

        private void SynchRowItemsHorizontalStartPosition(WrapRow row)
        {
            RadVirtualizingDataControlItem firstItem = row.firstItem;

            double currentPosition = this.wrapLineAlignment == WrapLineAlignment.Center ?
                                                                                         (this.owner.availableWidth - this.GetRowExtent(row)) / 2 : this.wrapLineAlignment == WrapLineAlignment.Near ? 0 : this.owner.availableWidth - this.GetRowExtent(row);

            while (true)
            {
                firstItem.SetHorizontalOffset(currentPosition);
                currentPosition += firstItem.width;
                firstItem = firstItem.next;
                if (firstItem == null || firstItem.wrapRow != row)
                {
                    break;
                }
            }
        }

        internal class WrapRow
        {
            internal RadVirtualizingDataControlItem firstItem;
            internal RadVirtualizingDataControlItem lastItem;
            internal WrapRow previous;
            internal WrapRow next;
            internal double rowLength;
            internal double rowOffset;
        }
    }
}