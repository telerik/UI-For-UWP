using System;
using System.Diagnostics;
using System.Linq;
using Telerik.Core;
using Telerik.Data.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid.Model
{
    internal partial class GridModel
    {
        internal ItemInfo? FindItemInfo(object item)
        {
            if (!this.IsDataReady || item == null)
            {
                return null;
            }

            int index;
            bool isExpanded = true;

            if (item is IDataGroup)
            {
                index = this.rowLayout.GetGroupInfo(item).Index;
            }
            else
            {
                int childIndex = 0;
                int groupIndex = 0;

                var group = this.FindItemParentGroup(item);
                if (group != null)
                {
                    if (this.rowLayout.IsCollapsed(group))
                    {
                        isExpanded = false;
                    }
                    else
                    {
                        groupIndex = this.rowLayout.GetGroupInfo(group).Index + 1;
                        childIndex = group.IndexOf(item, this.CurrentDataProvider.ValueProvider.GetSortComparer());
                    }
                }
                else
                {
                    group = this.CurrentDataProvider.Results.Root.RowGroup as DataGroup;
                    childIndex = group.IndexOf(item, this.CurrentDataProvider.ValueProvider.GetSortComparer());
                }

                index = groupIndex + childIndex;
            }

            if (!isExpanded)
            {
                return null;
            }

            index -= this.rowLayout.GetCollapsedSlotsCount(0, index);
            return this.FindDataItemFromIndex(index, true);
        }

        internal ItemInfo? FindFirstDataItemInView()
        {
            return this.FindDataItemFromIndex(0, true);
        }

        internal ItemInfo? FindLastDataItemInView()
        {
            return this.FindDataItemFromIndex(this.rowLayout.VisibleLineCount - 1, true);
        }

        internal DataGroup FindItemParentGroup(object item)
        {
            if (!this.IsDataReady || item == null)
            {
                return null;
            }

            // no groups, item is expanded
            if (this.groupDescriptors.Count == 0)
            {
                return null;
            }

            // coordinate is struct so it cannot be empty.
            var coordinate = this.CurrentDataProvider.Results.Root;

            var rootGroup = coordinate.RowGroup as Group;
            if (rootGroup == null)
            {
                return null;
            }

            var currentGroup = rootGroup;

            for (int i = 0; i < this.groupDescriptors.Count; i++)
            {
                object groupKey = this.CurrentDataProvider.DataView.GetGroupKey(item, i);

                if (groupKey == null)
                {
                    PropertyGroupDescriptionBase propertyGroupDescriptor = this.groupDescriptors[i].EngineDescription as PropertyGroupDescriptionBase;
                    groupKey = propertyGroupDescriptor.GroupNameFromItem(item, i);
                }

                Group subGroup;
                if (currentGroup.TryGetGroup(groupKey, out subGroup))
                {
                    currentGroup = subGroup;
                }

                if (subGroup == null)
                {
                    Debug.Assert(RadControl.IsInTestMode, "No group for item?");
                    return null;
                }
            }

            return currentGroup as DataGroup;
        }

        internal ItemInfo? FindPreviousOrNextDataItem(object pivotItem, bool next)
        {
            if (!this.IsDataReady)
            {
                return null;
            }

            int index;
            var info = this.FindItemInfo(pivotItem);
            if (info == null)
            {
                index = this.GetItemGroupIndex(pivotItem);
            }
            else
            {
                index = next ? info.Value.LayoutInfo.Line + 1 : info.Value.LayoutInfo.Line - 1;
            }
            return this.FindDataItemFromIndex(index, next);
        }

        internal ItemInfo? FindPageUpOrDownDataItem(object pivotItem, bool pageDown)
        {
            if (!this.IsDataReady)
            {
                return null;
            }

            int index = 0;
            int viewportItemCount = this.RowPool.ViewportItemCount;
            var info = this.FindItemInfo(pivotItem);

            if (info != null)
            {
                index = info.Value.LayoutInfo.Line;
                viewportItemCount -= this.rowLayout.GetCollapsedSlotsCount(0, index);
            }
            else
            {
                index = this.GetItemGroupIndex(pivotItem);
            }

            int itemCount = 0;
            var enumerator = this.rowLayout.GetLines(index, pageDown).GetEnumerator();
            ItemInfo? result = null;

            while (enumerator.MoveNext())
            {
                var lastItem = enumerator.Current.LastOrDefault();
                if (lastItem.ItemType == GroupType.BottomLevel)
                {
                    result = lastItem;
                }

                itemCount++;
                if (itemCount == viewportItemCount)
                {
                    break;
                }
            }

            return result;
        }

        internal void OnRowGroupExpandStateChanged(object group, bool isExpanded)
        {
            this.RowPool.OnGroupExpandStateChanged(group, isExpanded);
        }

        internal void ScrollIndexIntoView(ScrollIntoViewOperation<int> operation)
        {
            var index = operation.RequestedItem;
            var frozenContainersLength = Math.Max(0, this.RowPool.FrozenContainersLength);

            var itemLength = this.rowLayout.RenderInfo.ValueForIndex(index);
            var offsetToScroll = this.rowLayout.RenderInfo.OffsetFromIndex(index) - itemLength;

            if (DoubleArithmetics.IsLessThan(operation.InitialScrollOffset + frozenContainersLength, offsetToScroll))
            {
                if (index > 0)
                {
                    offsetToScroll -= this.View.ViewportHeight;
                    offsetToScroll += itemLength;
                }
            }
            else if (DoubleArithmetics.IsLessThanOrEqual(offsetToScroll, operation.InitialScrollOffset + frozenContainersLength))
            {
                offsetToScroll -= frozenContainersLength;
            }

            var scrollPosition = new RadPoint(this.PhysicalHorizontalOffset, Math.Max(0, offsetToScroll));

            this.GridView.SetScrollPosition(scrollPosition, true, true);
        }

        internal bool IsIndexInView(ScrollIntoViewOperation<int> operation)
        {
            var layoutSlot = this.RowPool.GetPreviousDisplayedLayoutSlot(operation.RequestedItem);
            if (layoutSlot == RadRect.Invalid)
            {
                return false;
            }

            var frozenContainersLength = Math.Max(0, this.RowPool.FrozenContainersLength);
            return DoubleArithmetics.IsGreaterThanOrEqual(layoutSlot.Y, this.PhysicalVerticalOffset + frozenContainersLength) &&
                   DoubleArithmetics.IsLessThanOrEqual(layoutSlot.Y + layoutSlot.Height, this.PhysicalVerticalOffset + this.View.ViewportHeight);
        }

        internal GridCellEditorModel GetCellEditorModel(object container)
        {
            return this.CellEditorsController.GetCellsForRow(1).FirstOrDefault(c => c.EditorHost == container);
        }

        internal bool IsColumnIndexInView(ScrollIntoViewOperation<int> operation)
        {
            var column = this.ColumnPool.GetDisplayedElement(operation.RequestedItem);
            if (column == null)
            {
                return false;
            }

            // Since the headers are not part of the scrollviewer we need to compare their position to the viewport window and omit the scroll offset.
            return DoubleArithmetics.Ceiling(column.LayoutSlot.X) >= 0 &&
                   DoubleArithmetics.IsLessThanOrEqual(column.LayoutSlot.X + column.layoutSlot.Width, this.View.ViewportWidth);
        }

        internal void ScrollIndexIntoViewCore(ScrollIntoViewOperation<int> scrollOperation)
        {
            var update = new DelegateUpdate<UpdateFlags>(() =>
            {
                if (scrollOperation.ScrollAttempts < ScrollIntoViewOperation<int>.MaxScrollAttempts)
                {
                    if (this.IsIndexInView(scrollOperation))
                    {
                        if (scrollOperation.CompletedAction != null)
                        {
                            this.GridView.UpdateService.RegisterUpdate(new DelegateUpdate<UpdateFlags>(scrollOperation.CompletedAction));
                        }
                    }
                    else
                    {
                        this.ScrollIndexIntoView(scrollOperation);
                        scrollOperation.ScrollAttempts++;

                        this.ScrollIndexIntoViewCore(scrollOperation);
                    }
                }
            })
            {
                RequiresValidMeasure = true
            };

            this.GridView.UpdateService.RegisterUpdate(update);
        }

        internal void ScrollColumnIntoViewCore(ScrollIntoViewOperation<int> scrollOperation)
        {
            var update = new DelegateUpdate<UpdateFlags>(() =>
            {
                if (scrollOperation.ScrollAttempts < ScrollIntoViewOperation<int>.MaxScrollAttempts)
                {
                    this.ScrollColumnIndexIntoView(scrollOperation);
                    scrollOperation.ScrollAttempts++;

                    if (this.IsColumnIndexInView(scrollOperation))
                    {
                        if (scrollOperation.CompletedAction != null)
                        {
                            scrollOperation.CompletedAction();
                        }
                    }
                    else
                    {
                        this.ScrollColumnIntoViewCore(scrollOperation);
                    }
                }
            });

            this.GridView.UpdateService.RegisterUpdate(update);
        }

        internal void ScrollColumnIndexIntoView(ScrollIntoViewOperation<int> operation)
        {
            var columnIndex = operation.RequestedItem;

            if (columnIndex < 0)
            {
                return;
            }

            var itemLength = this.columnLayout.RenderInfo.ValueForIndex(columnIndex);
            var offsetToScroll = this.columnLayout.RenderInfo.OffsetFromIndex(columnIndex) - itemLength;

            if (this.PhysicalHorizontalOffset < offsetToScroll)
            {
                offsetToScroll -= this.View.ViewportWidth - itemLength;
            }

            var scrollPosition = new RadPoint(offsetToScroll, this.PhysicalVerticalOffset);

            this.GridView.SetScrollPosition(scrollPosition, true, true);
        }

        private ItemInfo? FindDataItemFromIndex(int index, bool next, object dataItem = null)
        {
            var enumerator = this.rowLayout.GetLines(index, next).GetEnumerator();
            ItemInfo? info = null;
            while (enumerator.MoveNext())
            {
                var lastItem = enumerator.Current.LastOrDefault();
                if (lastItem.ItemType == GroupType.BottomLevel)
                {
                    if (dataItem == null || object.ReferenceEquals(lastItem.Item, dataItem))
                    {
                        info = lastItem;
                        break;
                    }
                }
            }

            return info;
        }

        private void InvalidateRowsMeasure()
        {
            this.GridView.InvalidateCellsPanelMeasure();
        }

        private int GetItemGroupIndex(object item)
        {
            int index;

            var group = this.FindItemParentGroup(item);
            if (group != null)
            {
                index = this.rowLayout.GetGroupInfo(group).Index;
                index -= this.rowLayout.GetCollapsedSlotsCount(0, index);
            }
            else
            {
                index = 0;
            }

            return index;
        }
    }
}