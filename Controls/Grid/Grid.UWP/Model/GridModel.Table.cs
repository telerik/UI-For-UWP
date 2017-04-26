using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Telerik.Core;
using Telerik.Core.Data;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Grid.Commands;

namespace Telerik.UI.Xaml.Controls.Grid.Model
{
    internal partial class GridModel
    {
        internal readonly CellEditorsController CellEditorsController;
        internal readonly EditRowPool EditRowPool;

        internal readonly CellsController<GridCellModel> CellsController;
        internal readonly DecorationsController DecorationsController;
        internal readonly DecorationsController FrozenDecorationsController;
        internal readonly RowModelPool RowPool;
        internal readonly ColumnHeaderPool ColumnPool;

        internal InvalidateMeasureFlags pendingMeasureFlags;

        private static readonly int RowHeightPropertyKey = PropertyKeys.Register(typeof(GridModel), "RowHeight");

        private static readonly int DataLoadingModePropertyKey = PropertyKeys.Register(typeof(GridModel), "DataLoadingMode");

        private readonly BaseLayout rowLayout;
        private readonly BaseLayout columnLayout;
        private bool rowHeightIsNaN;

        private RadSize availableSize;
        private RadSize desiredSize;
        private RadSize lastArrangeSize;

        public bool RowHeightIsNaN
        {
            get
            {
                return this.rowHeightIsNaN;
            }
        }

        public BatchLoadingMode DataLoadingMode
        {
            get
            {
                return this.GetTypedValue<BatchLoadingMode>(DataLoadingModePropertyKey, BatchLoadingMode.Auto);
            }
            set
            {
                this.SetValue(DataLoadingModePropertyKey, value);
                this.SetRowLayoutSource();
                this.CellsController.Update(UpdateFlags.All);
                this.InvalidateRowsMeasure();
            }
        }

        public double RowHeight
        {
            get
            {
                return this.GetTypedValue<double>(RowHeightPropertyKey, double.NaN);
            }
            set
            {
                this.SetValue(RowHeightPropertyKey, value);
            }
        }

        int ITable.FrozenColumnCount
        {
            get
            {
                return this.FrozenColumnCount;
            }
        }

        double ITable.GetWidthForLine(int line)
        {
            return this.CellsController.GetSlotWidth(line);
        }

        double ITable.GetHeightForLine(int line)
        {
            return this.CellsController.GetSlotHeight(line);
        }

        double ITable.GenerateCellsForColumn(int columnSlot, double largestColumnElementWidth, IItemInfoNode columnDecorator)
        {
            this.CellsController.UpdateSlotWidth(columnSlot, largestColumnElementWidth);

            Debug.Assert(columnDecorator != null, "Decorator shoundn't be null");
            bool shouldUpdateRows = this.CellsController.GenerateCellsForColumn(columnDecorator, columnSlot);

            if (shouldUpdateRows)
            {
                this.InvalidateRowsMeasure();
            }

            return this.CellsController.GetSlotWidth(columnSlot);
        }

        double ITable.GenerateCellsForRow(int rowSlot, double largestRowElementHeight, IItemInfoNode rowDecorator)
        {
            if (this.isInEditMode && rowDecorator == this.EditRow || rowDecorator == this.FrozenEditRow)
            {
                return this.GenerateCellsForEditRow(rowSlot, largestRowElementHeight, rowDecorator);
            }
            else
            {
                var height = this.GenerateCellsForReadOnlyRow(rowSlot, largestRowElementHeight, rowDecorator);

                this.UpdateReadOnlyRowOpacity(rowDecorator);

                return height;
            }
        }

        void ITable.RecycleColumn(ItemInfo itemInfo)
        {
            this.CellsController.RecycleColumn(itemInfo.Slot);
        }

        void ITable.RecycleRow(ItemInfo itemInfo)
        {
            this.CellsController.RecycleRow(itemInfo.Slot);
        }

        void ITable.RecycleEditRow(ItemInfo itemInfo)
        {
            if (this.isInEditMode && this.EditRow.ItemInfo.Equals(itemInfo))
            {
                this.CellEditorsController.RecycleRow(itemInfo.Slot);
            }
        }

        object ITable.GetCellValue(ItemInfo rowItemInfo, ItemInfo columnItemInfo)
        {
            var columnDefinition = columnItemInfo.Item as DataGridColumn;

            var nestedPropertyColumn = columnDefinition as INestedPropertyColumn;
            if (nestedPropertyColumn != null)
            {
                return nestedPropertyColumn.GetDisplayValueForInstance(rowItemInfo.Item);
            }

            return columnDefinition.GetValueForInstance(rowItemInfo.Item);
        }

        void ITable.InvalidateHeadersPanelMeasure()
        {
            this.GridView.InvalidateHeadersPanelMeasure();
        }

        RadSize ITable.Measure(Node node)
        {
            return this.GridView.MeasureContent(node, null);
        }

        void ITable.Arrange(Node node)
        {
            this.GridView.Arrange(node);
        }

        void ITable.InvalidateCellsPanelMeasure()
        {
            this.GridView.InvalidateCellsPanelMeasure();
        }

        void ITable.InvalidateCellsPanelArrange()
        {
            this.GridView.InvalidateCellsPanelArrange();
        }

        internal void UpdateRowPoolRenderInfo(int index, double value)
        {
            var previousValue = this.RowPool.RenderInfo.ValueForIndex(index);
            this.RowPool.RenderInfo.Update(index, Math.Max(previousValue, value));
        }

        internal RadSize MeasureHeaderRow(RadSize newAvailableSize)
        {
            var desiredSize = this.ColumnPool.OnMeasure(newAvailableSize, this.PhysicalHorizontalOffset, this.FrozenColumnCount, this.VerticalBufferScale);

            var desiredWidth = this.columnLayout.TotalLineCount > 0 ? this.columnLayout.RenderInfo.OffsetFromIndex(this.columnLayout.TotalLineCount - 1) : 0;
            return new RadSize(desiredWidth, desiredSize.Height);
        }

        internal RadSize ArrangeHeaderRow(RadSize finalSize)
        {
            if ((this.pendingMeasureFlags & InvalidateMeasureFlags.Header) == InvalidateMeasureFlags.Header)
            {
                return finalSize;
            }

            return this.ColumnPool.OnArrange(finalSize);
        }

        internal RadSize MeasureCells(RadSize newAvailableSize)
        {
            this.availableSize = newAvailableSize;

            var cellsDesiredSize = this.RowPool.OnMeasure(this.availableSize, this.PhysicalVerticalOffset, this.FrozenColumnCount, this.VerticalBufferScale);

            this.RecycleDecorations();
            this.ColumnPool.RecycleAfterMeasure();
            this.RowPool.RecycleAfterMeasure();

            if ((this.pendingMeasureFlags & InvalidateMeasureFlags.Cells) == InvalidateMeasureFlags.None)
            {
                this.CellsController.FullyRecycleDecorators();
                this.CellEditorsController.FullyRecycleDecorators();

                this.GenerateRowLineDecorators();
                this.GenerateColumnLineDecorators();
            }

            if (this.ShouldRequestItems(null))
            {
                this.GridView.CommandService.ExecuteCommand(CommandId.LoadMoreData, new LoadMoreDataContext());
            }

            // NOTE: If we decide that we won't zero the length of collapsed rows then this 'TotalLineCount - 1' will be incorrect.
            this.desiredSize = new RadSize(this.columnLayout.RenderInfo.OffsetFromIndex(this.columnLayout.TotalLineCount - 1), this.rowLayout.RenderInfo.OffsetFromIndex(this.rowLayout.TotalLineCount - 1));

            return this.desiredSize;
        }

        internal RadSize ArrangeCells(RadSize finalSize)
        {
            this.lastArrangeSize = finalSize;
            if ((this.pendingMeasureFlags & InvalidateMeasureFlags.Cells) == InvalidateMeasureFlags.Cells)
            {
                return finalSize;
            }

            if (this.VisibleColumns.Count() == this.ColumnPool.ViewportItemCount)
            {
                bool columnsStretched = this.CellsController.TryStretchColumns(this.availableSize.Width);
                if (columnsStretched)
                {
                    this.GridView.RebuildUI();
                    return finalSize;
                }
            }

            var result = this.RowPool.OnArrange(finalSize);
            result = this.CellsController.OnCellsArrange(finalSize);

            this.UpdateEditRow();

            this.ArrangeEditorsPool(finalSize);

            // Wait for cell to be arranged since arranging the frozen decorators need the cells lyaoutslot to arrange.
            this.RowPool.ArrangeFrozenDecorators();

            this.ArrangeLineDecorators(finalSize);

            this.ApplyLayersClipping(result);

            return result;
        }

        internal void InvalidateCellsDesiredSize()
        {
            foreach (GridRowModel row in this.ForEachRow())
            {
                foreach (var cell in this.ForEachRowCell(row))
                {
                    cell.DesiredSize = RadSize.Invalid;
                }
            }
        }

        private void ApplyLayersClipping(RadSize finalSize)
        {
            RadRect rect = RadRect.Empty;
            RadRect frozenRect = RadRect.Empty;
            double frozenColumnsOffset = 0;

            var frozenЕlements = this.ColumnPool.GetFrozenDisplayedElements();

            var scrollableElements = this.ColumnPool.GetUnfrozenDisplayedElements();

            if (frozenЕlements.Count() > 0)
            {
                frozenColumnsOffset = frozenЕlements.Last().Value.Last().LayoutSlot.Right;
                frozenRect = new RadRect(0, this.PhysicalVerticalOffset, frozenColumnsOffset, finalSize.Height);
            }

            if (scrollableElements.Count() > 0)
            {
                var scrollableViewPortRight = scrollableElements.Last().Value.Last().layoutSlot.Right;
                rect = new RadRect(this.PhysicalHorizontalOffset + frozenColumnsOffset, this.PhysicalVerticalOffset, scrollableViewPortRight, finalSize.Height);
            }

            this.GridView.ApplyLayersClipping(rect, frozenRect);
        }

        private void ArrangeLineDecorators(RadSize finalSize)
        {
            Dictionary<int, double> horizontalCellsData = new Dictionary<int, double>();

            bool firstColumn = true;
            double leftOffset = 0;

            foreach (var item in this.ColumnPool.GetUnfrozenDisplayedElements())
            {
                horizontalCellsData.Add(item.Value.Last().ItemInfo.LayoutInfo.Line, this.CellsController.GetSlotWidth(item.Key));

                // NOTE: Remove when/if fallback to logical scrolling only.
                if (firstColumn)
                {
                    firstColumn = false;
                    int columnLine = item.Value.Last().ItemInfo.Slot;
                    leftOffset = columnLine - 1 >= 0 ? this.columnLayout.RenderInfo.OffsetFromIndex(columnLine - 1) : 0;
                }
            }

            var cellsWidth = finalSize.Width;

            ArrangeDataForDecorations scrollableHorizontalArrangement =
                new ArrangeDataForDecorations(horizontalCellsData, this.RowPool.GetGroupSizes(), leftOffset, cellsWidth);

            Dictionary<int, double> verticalCellsData = new Dictionary<int, double>();

            bool firstRow = true;
            double topOffset = 0;

            foreach (var item in this.RowPool.GetDisplayedElements())
            {
                var height = this.CellsController.GetSlotHeight(item.Key);
                verticalCellsData.Add(item.Value.Last().ItemInfo.LayoutInfo.Line, height);

                // NOTE: Remove when/if fallback to logical scrolling only.
                if (firstRow)
                {
                    firstRow = false;
                    int rowLine = item.Value.Last().ItemInfo.Slot;
                    topOffset = rowLine - 1 >= 0 ? this.rowLayout.RenderInfo.OffsetFromIndex(rowLine - 1) : 0;
                }
            }

            ArrangeDataForDecorations verticalArrangement =
                new ArrangeDataForDecorations(verticalCellsData, this.ColumnPool.GetGroupSizes(), topOffset, finalSize.Height);

            this.DecorationsController.Arrange(scrollableHorizontalArrangement, verticalArrangement);

            this.BuildSelectionRegions(this.RowPool.GetDisplayedItems(), this.ColumnPool.GetDisplayedItems());

            if (this.FrozenColumnCount > 0)
            {
                this.ArrangeFrozenLineDecorators(finalSize, verticalArrangement);
            }

            this.UpdateSelection(finalSize, scrollableHorizontalArrangement, verticalArrangement);
        }

        private void UpdateSelection(RadSize finalSize, ArrangeDataForDecorations scrollableHorizontalArrangement, ArrangeDataForDecorations verticalArrangement)
        {
            Dictionary<int, double> horizontalCellsData = new Dictionary<int, double>();

            bool firstColumn = true;
            double leftOffset = 0;

            foreach (var item in this.ColumnPool.GetDisplayedElements())
            {
                horizontalCellsData.Add(item.Value.Last().ItemInfo.LayoutInfo.Line, this.CellsController.GetSlotWidth(item.Key));

                // NOTE: Remove when/if fallback to logical scrolling only.
                if (firstColumn)
                {
                    firstColumn = false;
                    int columnLine = item.Value.Last().ItemInfo.Slot;
                    leftOffset = columnLine - 1 >= 0 ? this.columnLayout.RenderInfo.OffsetFromIndex(columnLine - 1) : 0;
                }
            }

            var cellsWidth = finalSize.Width;

            ArrangeDataForDecorations horizontalArrangement =
                new ArrangeDataForDecorations(horizontalCellsData, this.RowPool.GetGroupSizes(), leftOffset, cellsWidth);

            this.FrozenSelectionPresenter.Arrange(horizontalArrangement, verticalArrangement, this.mergedSelectionRegions);
            this.selectionPresenter.Arrange(scrollableHorizontalArrangement, verticalArrangement, this.mergedSelectionRegions);
        }

        private void ArrangeFrozenLineDecorators(RadSize finalSize, ArrangeDataForDecorations verticalArrangement)
        {
            Dictionary<int, double> horizontalCellsData = new Dictionary<int, double>();

            bool firstColumn = true;
            double leftOffset = 0;

            var lastColumnLine = 0;

            foreach (var item in this.ColumnPool.GetFrozenDisplayedElements())
            {
                horizontalCellsData.Add(item.Value.Last().ItemInfo.LayoutInfo.Line, this.CellsController.GetSlotWidth(item.Key));

                // NOTE: Remove when/if fallback to logical scrolling only.
                if (firstColumn)
                {
                    firstColumn = false;
                    int columnLine = item.Value.Last().ItemInfo.Slot;
                    leftOffset = columnLine - 1 >= 0 ? this.columnLayout.RenderInfo.OffsetFromIndex(columnLine - 1) : 0;
                }

                lastColumnLine = item.Value.Last().ItemInfo.LayoutInfo.Line;
            }

            if (horizontalCellsData.Count > 0)
            {
                // TODO: refactor to get gridlines thickness into account.
                horizontalCellsData.Add(++lastColumnLine, 100);
            }

            var cellsWidth = finalSize.Width;

            ArrangeDataForDecorations horizontalArrangement =
                new ArrangeDataForDecorations(horizontalCellsData, this.RowPool.GetGroupSizes(), leftOffset, cellsWidth);

            this.FrozenDecorationsController.Arrange(horizontalArrangement, verticalArrangement);
        }

        private void RecycleDecorations()
        {
            var unusedCols = this.ColumnPool.GetItemsToRecycle();
            foreach (var itemInfo in unusedCols)
            {
                this.DecorationsController.RecycleColumnDecorators(itemInfo.Id);
                this.FrozenDecorationsController.RecycleColumnDecorators(itemInfo.Id);
            }

            var unusedRows = this.RowPool.GetItemsToRecycle();
            foreach (var itemInfo in unusedRows)
            {
                this.DecorationsController.RecycleRowDecorators(itemInfo.Id);
                this.FrozenDecorationsController.RecycleRowDecorators(itemInfo.Id);
            }
        }

        private void GenerateColumnLineDecorators()
        {
            var displayedColumns = this.ColumnPool.GetDisplayedElements().ToList();
            if (displayedColumns.Count > 0)
            {
                var firstPair = displayedColumns[0];

                if (this.FrozenColumnCount > 0)
                {
                    this.DecorationsController.RecycleColumnDecoratorBetween(this.FrozenColumnCount - 1, firstPair.Value.Last().ItemInfo.LayoutInfo.Line);
                }
                else
                {
                    this.DecorationsController.RecycleColumnDecoratorBefore(firstPair.Value.Last().ItemInfo.LayoutInfo.Line);
                }

                var lastPair = displayedColumns[displayedColumns.Count - 1];
                this.DecorationsController.RecycleColumnDecoratorAfter(lastPair.Value.Last().ItemInfo.LayoutInfo.Line);
            }
            else
            {
                this.DecorationsController.RecycleAllColumnDecorators();
                this.FrozenDecorationsController.RecycleAllColumnDecorators();
            }

            var displayedInfos = this.ColumnPool.GetDisplayedItems();
            this.DecorationsController.GenerateColumnDecorators(displayedInfos);

            var frozenDisplayedInfos = new List<ItemInfo>(this.ColumnPool.GetFrozenDisplayedItems());

            if (frozenDisplayedInfos.Count > 0)
            {
                var lastItemInfo = frozenDisplayedInfos.Last();

                // Add dummy layout info to render additional column line.
                var layoutInfo = new LayoutInfo
                {
                    Indent = 0,
                    Line = lastItemInfo.Id + 1,
                    Level = 0,
                    SpansThroughCells = true,
                    LevelSpan = 1,
                    LineSpan = 1
                };

                frozenDisplayedInfos.Add(new ItemInfo { Id = lastItemInfo.Id + 1, LayoutInfo = layoutInfo });
            }

            this.FrozenDecorationsController.GenerateColumnDecorators(frozenDisplayedInfos);
        }

        private void GenerateRowLineDecorators()
        {
            var displayedRows = this.RowPool.GetDisplayedElements().ToList();
            if (displayedRows.Count > 0)
            {
                var pair = displayedRows[0];
                this.DecorationsController.RecycleRowDecoratorBefore(pair.Value.Last().ItemInfo.LayoutInfo.Line);
                this.FrozenDecorationsController.RecycleRowDecoratorBefore(pair.Value.Last().ItemInfo.LayoutInfo.Line);

                pair = displayedRows[displayedRows.Count - 1];
                this.DecorationsController.RecycleRowDecoratorAfter(pair.Value.Last().ItemInfo.LayoutInfo.Line);
                this.FrozenDecorationsController.RecycleRowDecoratorAfter(pair.Value.Last().ItemInfo.LayoutInfo.Line);
            }
            else
            {
                this.FrozenDecorationsController.RecycleAllRowDecorators();
                this.DecorationsController.RecycleAllRowDecorators();
            }

            var displayedInfos = this.RowPool.GetDisplayedItems();
            this.DecorationsController.GenerateRowDecorators(displayedInfos);
            this.FrozenDecorationsController.GenerateRowDecorators(displayedInfos);
        }

        private double GenerateCellsForReadOnlyRow(int rowSlot, double largestRowElementWidth, IItemInfoNode rowDecorator)
        {
            this.CellsController.UpdateSlotHeight(rowSlot, largestRowElementWidth);

            Debug.Assert(rowDecorator != null, "Decorator shoundn't be null");
            bool shouldUpdateColumns = this.CellsController.GenerateCellsForRow(rowDecorator, rowSlot);

            if (shouldUpdateColumns)
            {
                this.GridView.InvalidateHeadersPanelMeasure();
                this.GridView.InvalidateCellsPanelMeasure();
            }

            var desiredHeight = this.CellsController.GetSlotHeight(rowSlot);
            this.rowLayout.RenderInfo.Update(rowSlot, desiredHeight);

            return desiredHeight;
        }

        private void UpdateReadOnlyRowOpacity(IItemInfoNode rowDecorator)
        {
            var row = rowDecorator as GridRowModel;

            if (!this.isInEditMode || row == null)
            {
                return;
            }

            if (this.EditRowPool.ReadOnlyItemInfo.Equals(row.ItemInfo))
            {
                this.HideRow(row);
            }
            else
            {
                this.ShowRow(row);
            }
        }

        private double GenerateCellsForEditRow(int rowSlot, double largestRowElementWidth, IItemInfoNode rowDecorator)
        {
            this.CellEditorsController.UpdateSlotHeight(rowSlot, largestRowElementWidth);

            Debug.Assert(rowDecorator != null, "Decorator shoundn't be null");
            bool shouldUpdateColumns = this.CellEditorsController.GenerateCellsForRow(rowDecorator, rowSlot);

            if (shouldUpdateColumns)
            {
                this.GridView.InvalidateHeadersPanelMeasure();
                this.GridView.InvalidateCellsPanelMeasure();
            }

            var desiredHeight = this.CellEditorsController.GetSlotHeight(rowSlot);

            this.EditRowPool.RenderInfo.Update(rowSlot, desiredHeight);

            return desiredHeight;
        }
    }
}