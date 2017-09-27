using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Telerik.Core;
using Telerik.Data.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Grid.Model;
using Windows.Foundation.Collections;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal partial class CellsController<T> where T : GridCellModel
    {
        internal static readonly RadSize InfinitySize = new RadSize(double.PositiveInfinity, double.PositiveInfinity);

        private Dictionary<int, Dictionary<int, T>> generatedRowCells = new Dictionary<int, Dictionary<int, T>>();

        private ITable table;
        private ItemModelGenerator<T, CellGenerationContext> containerGenerator;

        public CellsController(ITable tableControl, ItemModelGenerator<T, CellGenerationContext> cellGenerator)
        {
            this.table = tableControl;
            this.containerGenerator = cellGenerator;
        }

        internal INodePool<GridRowModel> RowPool
        {
            get;
            set;
        }

        internal INodePool<GridHeaderCellModel> ColumnPool
        {
            get;
            set;
        }

        internal IEnumerable<T> GetCellsForRow(int slot)
        {
            Dictionary<int, T> rowCells;
            if (this.generatedRowCells.TryGetValue(slot, out rowCells))
            {
                foreach (var pair in rowCells)
                {
                    yield return pair.Value;
                }
            }
            else
            {
                yield break;
            }
        }

        internal void Update(UpdateFlags flags)
        {
            if ((flags & UpdateFlags.AffectsContent) == UpdateFlags.AffectsContent)
            {
                this.RecycleCells();
                this.RecycleRows();
            }

            if ((flags & UpdateFlags.AffectsColumnsWidth) == UpdateFlags.AffectsColumnsWidth)
            {
                this.RecycleColumns();
            }
        }

        internal RadSize OnCellsArrange(RadSize finalSize)
        {
            bool firstRow = true;
            double leftOffset = 0;
            double topOffset = 0;

            foreach (var rowPairs in this.RowPool.GetDisplayedElements())
            {
                int rowSlot = rowPairs.Key;
                var rowModel = rowPairs.Value.Last();
                if (firstRow)
                {
                    firstRow = false;
                    int line = rowModel.ItemInfo.Slot;
                    topOffset = line - 1 >= 0 ? (this.RowPool.RenderInfo.OffsetFromIndex(line - 1) - rowModel.RowDetailsSize.Height) : 0;
                }

                Dictionary<int, T> columnCellsPair;
                this.generatedRowCells.TryGetValue(rowSlot, out columnCellsPair);

                leftOffset = 0;

                double cellHeight = this.GetSlotHeight(rowSlot);
                bool firstColumn = true;
                int cellSequenceNumber = 0;

                foreach (var columnPairs in this.ColumnPool.GetDisplayedElements())
                {
                    if (firstColumn || cellSequenceNumber != columnPairs.Value.Last().ItemInfo.Slot)
                    {
                        firstColumn = false;
                        int columnLine = columnPairs.Value.Last().ItemInfo.Slot;
                        leftOffset = columnLine - 1 >= 0 ? this.ColumnPool.RenderInfo.OffsetFromIndex(columnLine - 1) : 0;
                    }

                    int cellSlot = columnPairs.Key;

                    T cellDecorator = null;
                    if (columnCellsPair != null)
                    {
                        columnCellsPair.TryGetValue(cellSlot, out cellDecorator);
                    }

                    double cellWidth = this.GetSlotWidth(cellSlot);
                    if (cellDecorator != null)
                    {
                        cellDecorator.layoutSlot = new RadRect(leftOffset, topOffset, cellWidth, cellHeight);
                        this.table.Arrange(cellDecorator);
                    }

                    leftOffset += cellWidth;
                    cellSequenceNumber++;
                }

                foreach (var row in rowPairs.Value)
                {
                    row.layoutSlot = new RadRect(0, topOffset, leftOffset, cellHeight);
                    this.table.Arrange(row);
                }

                topOffset += cellHeight;
            }

            return finalSize;
        }

        internal bool GenerateCellsForColumn(IItemInfoNode columnDecorator, int columnSlot)
        {
            double desiredHeight = 0;

            double availableHeight = this.RowPool.AvailableLength;
            double availableWidth = this.ColumnPool.AvailableLength;

            bool shouldUpdateRows = false;

            int generatedCellCount = 0;

            var rows = this.RowPool.GetDisplayedElements().ToArray();
            foreach (var rowPairs in rows)
            {
                int rowSlot = rowPairs.Key;
                var rowDecorator = rowPairs.Value.LastOrDefault();
                Debug.Assert(rowDecorator != null, "Decorator shoundn't be null");

                generatedCellCount++;
                bool isGroup = rowDecorator.ItemInfo.Item is IGroup;
                bool isPlaceHolder = rowDecorator.ItemInfo.Item is PlaceholderInfo;

                if (isGroup || isPlaceHolder)
                {
                    desiredHeight += rowDecorator.DesiredSize.Height;
                    continue;
                }
                T cellModel = null;

                if (this.generatedRowCells.ContainsKey(rowSlot))
                {
                    var cellsPerLine = this.generatedRowCells[rowSlot];
                    if (isGroup)
                    {
                        Debug.Assert(cellsPerLine.Count < 2, "Groups should have 1 cell only");
                        if (cellsPerLine.Count == 1)
                        {
                            cellModel = cellsPerLine.First().Value;
                        }
                    }
                    else
                    {
                        cellsPerLine.TryGetValue(columnSlot, out cellModel);
                    }
                }

                if (cellModel == null)
                {
                    cellModel = this.GetCellDecorator(rowDecorator, columnDecorator.ItemInfo, rowSlot, columnSlot);
                }

                if (cellModel != null)
                {
                    cellModel.parent = rowDecorator;
                }

                var desiredSize = this.MeasureCellDecorator(cellModel);
                double cellWidth = desiredSize.Width;
                double cellHeigth = desiredSize.Height;

                this.UpdateSlotWidth(columnSlot, cellWidth);
                bool slotHeightUpdated = this.UpdateSlotHeight(rowSlot, cellHeigth, false);
                shouldUpdateRows = shouldUpdateRows || slotHeightUpdated;

                desiredHeight += this.GetSlotHeight(rowSlot);
                if (generatedCellCount == 1)
                {
                    desiredHeight -= this.RowPool.HiddenPixels;
                }

                if (GridModel.DoubleArithmetics.IsGreaterThan(desiredHeight, availableHeight))
                {
                    break;
                }
            }

            shouldUpdateRows = generatedCellCount != this.RowPool.ViewportItemCount || shouldUpdateRows;

            return shouldUpdateRows;
        }

        internal bool GenerateCellsForRow(IItemInfoNode rowDecorator, int rowSlot)
        {
            var rowItem = rowDecorator.ItemInfo.Item;

            var isPlaceHolder = rowItem is PlaceholderInfo;

            if (rowItem is IGroup || isPlaceHolder)
            {
                return false;
            }

            double desiredWidth = 0;

            bool shouldUpdateColumns = false;

            double availableWidth = this.ColumnPool.AvailableLength;

            int generatedCellCount = 0;

            var columns = this.ColumnPool.GetDisplayedElements().ToArray();
            bool generated = this.generatedRowCells.ContainsKey(rowSlot);

            foreach (var columnPairs in columns)
            {
                int columnLine = columnPairs.Key;
                var columnDecorator = columnPairs.Value.LastOrDefault();
                Debug.Assert(columnDecorator != null, "Decorator shoundn't be null");

                int columnSlot = columnLine;
                generatedCellCount++;

                T cellModel = null;
                if (generated)
                {
                    var cellsPerLine = this.generatedRowCells[rowSlot];
                    cellsPerLine.TryGetValue(columnLine, out cellModel);
                }

                if (cellModel == null)
                {
                    cellModel = this.GetCellDecorator(rowDecorator, columnDecorator.ItemInfo, rowSlot, columnLine);
                }

                if (cellModel != null)
                {
                    cellModel.parent = rowDecorator as Element;
                }

                var desiredSize = this.MeasureCellDecorator(cellModel);
                double cellWidth = desiredSize.Width;
                double cellHeight = desiredSize.Height;

                this.UpdateSlotHeight(rowSlot, cellHeight, false);
                bool slotWidthUpdated = this.UpdateSlotWidth(columnSlot, cellWidth);
                shouldUpdateColumns = shouldUpdateColumns || slotWidthUpdated;

                desiredWidth += this.GetSlotWidth(columnSlot);
                if (generatedCellCount == 1)
                {
                    desiredWidth -= this.ColumnPool.HiddenPixels;
                }

                if (GridModel.DoubleArithmetics.IsGreaterThan(desiredWidth, availableWidth))
                {
                    break;
                }
            }

            // TODO:Handle this better in frozen columns scenario
            shouldUpdateColumns = generatedCellCount != this.ColumnPool.ViewportItemCount && this.table.FrozenColumnCount == 0 || shouldUpdateColumns;

            return shouldUpdateColumns;
        }

        internal void RecycleColumn(int columnSlot)
        {
            foreach (var rowPairs in this.RowPool.GetDisplayedElements())
            {
                int rowSlot = rowPairs.Key;
                var row = rowPairs.Value.LastOrDefault();
                Debug.Assert(row != null, "Decorator shoundn't be null");

                T cell;
                Dictionary<int, T> columnCellsPair;
                if (this.generatedRowCells.TryGetValue(rowSlot, out columnCellsPair) && columnCellsPair.TryGetValue(columnSlot, out cell))
                {
                    this.containerGenerator.RecycleDecorator(cell);
                    columnCellsPair.Remove(columnSlot);
                }
            }
        }

        internal void RecycleRow(int rowSlot)
        {
            Dictionary<int, T> columnCellsPair;
            if (this.generatedRowCells.TryGetValue(rowSlot, out columnCellsPair))
            {
                foreach (var columnCell in columnCellsPair)
                {
                    var cell = columnCell.Value;
                    this.containerGenerator.RecycleDecorator(cell);
                }

                this.generatedRowCells.Remove(rowSlot);
            }
        }

        internal void FullyRecycleDecorators()
        {
            this.containerGenerator.FullyRecycleDecorators();
        }

        private RadSize MeasureCellDecorator(T cellDecorator)
        {
            if (cellDecorator == null)
            {
                return new RadSize();
            }

            if (cellDecorator.DesiredSize == RadSize.Invalid)
            {
                return this.table.Measure(cellDecorator);
            }

            return cellDecorator.DesiredSize;
        }

        private T GetCellDecorator(IItemInfoNode parentRow, ItemInfo columnItemInfo, int rowLine, int columnLine)
        {
            Dictionary<int, T> columnCellsPair;
            if (!this.generatedRowCells.TryGetValue(rowLine, out columnCellsPair))
            {
                columnCellsPair = new Dictionary<int, T>();
                this.generatedRowCells[rowLine] = columnCellsPair;
            }

            var cellValue = this.table.GetCellValue(parentRow.ItemInfo, columnItemInfo);

            bool isFrozen = false;

            if (columnItemInfo.Item is DataGridColumn)
            {
                isFrozen = (columnItemInfo.Item as DataGridColumn).IsFrozen;
            }

            var context = new CellGenerationContext(parentRow.ItemInfo.Item, columnItemInfo.Item, cellValue, isFrozen);

            object containerType = this.containerGenerator.ContainerTypeForItem(context);

            if (containerType == null)
            {
                return null;
            }

            T cellDecorator;
            if (!columnCellsPair.TryGetValue(columnLine, out cellDecorator))
            {
                cellDecorator = this.containerGenerator.GenerateContainer(context);
                columnCellsPair.Add(columnLine, cellDecorator);

                cellDecorator.layoutSlot = RadRect.Invalid;
                cellDecorator.DesiredSize = RadSize.Invalid;
                cellDecorator.parent = parentRow as Element;
            }

            if (!object.Equals(cellDecorator.Value, cellValue) || cellDecorator.Column != columnItemInfo.Item || cellDecorator.Column.ShouldRefreshCell(cellDecorator))
            {
                cellDecorator.Value = cellValue;
                cellDecorator.Column = columnItemInfo.Item as DataGridColumn;
                cellDecorator.Column.ItemInfo = columnItemInfo;

                this.containerGenerator.PrepareContainerForItem(cellDecorator);
            }

            return cellDecorator;
        }

        private void RecycleColumns()
        {
            this.ColumnPool.RefreshRenderInfo(this.table.RowHeight);
        }

        private void RecycleRows()
        {
            this.generatedRowCells.Clear();
            this.RowPool.RefreshRenderInfo(this.table.RowHeight);
        }

        private void RecycleCells()
        {
            foreach (var rowSlot in this.generatedRowCells)
            {
                Dictionary<int, T> columnCellsPair = rowSlot.Value;

                foreach (var columnCell in columnCellsPair)
                {
                    var cell = columnCell.Value;
                    this.containerGenerator.RecycleDecorator(cell);
                }

                columnCellsPair.Clear();
            }
        }
    }
}
