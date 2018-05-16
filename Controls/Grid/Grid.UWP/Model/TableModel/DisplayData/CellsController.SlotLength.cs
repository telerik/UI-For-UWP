using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Grid.Model;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal partial class CellsController<T>
    {
        internal bool TryStretchColumns(double availableWidth)
        {
            double difference;
            int stretchableCount;
            if (!this.ShouldStretchColumns(availableWidth, out difference, out stretchableCount))
            {
                return false;
            }

            double percent = 1d / stretchableCount;
            double remainingDifference = difference;
            double correction = 0;
            double newValue = 0;
            double newTotal = 0;

            var renderInfo = this.ColumnPool.RenderInfo;

            List<KeyValuePair<int, double>> newValues = new List<KeyValuePair<int, double>>(renderInfo.Count);
            for (int i = 0; i < renderInfo.Count; i++)
            {
                var cell = this.ColumnPool.GetDisplayedElement(i);

                newValue = renderInfo.ValueForIndex(i, false);
                correction = 0;

                if (cell != null && cell.Column.SizeMode == DataGridColumnSizeMode.Stretch && newValue > 0)
                {
                    correction = Math.Round(percent * difference);
                    if (correction == 0)
                    {
                        correction = Math.Sign(difference);
                    }

                    var updatedValue = Math.Max(cell.Column.AutoWidth, newValue + correction);
                    correction = updatedValue - newValue;

                    newValue = updatedValue;

                    newValues.Add(new KeyValuePair<int, double>(i, newValue));
                }

                newTotal += newValue;
                remainingDifference -= correction;

                if (GridModel.DoubleArithmetics.IsZero(remainingDifference))
                {
                    newTotal = availableWidth;
                    break;
                }
            }

            if (!GridModel.DoubleArithmetics.AreClose(newTotal, availableWidth) && newValues.Count > 0)
            {
                double error = newTotal - availableWidth;
                var pair = newValues.Last();
                double newLastValue = Math.Max(0, pair.Value - error);
                pair = new KeyValuePair<int, double>(pair.Key, newLastValue);
                newValues[newValues.Count - 1] = pair;
                newTotal -= error;
            }

            bool updated = false;
            foreach (var pair in newValues)
            {
                updated = this.ForceUpdateSlotWidth(pair.Key, pair.Value) || updated;
            }

            return updated;
        }

        internal double GetSlotWidth(int cellSlot)
        {
            return this.ColumnPool.RenderInfo.ValueForIndex(cellSlot, false);
        }

        internal bool UpdateSlotWidth(int cellSlot, double cellWidth)
        {
            var renderInfo = this.ColumnPool.RenderInfo;
            var currentWidth = renderInfo.ValueForIndex(cellSlot, false);

            bool isLessThan = GridModel.DoubleArithmetics.IsLessThan(currentWidth, cellWidth);

            if (GridModel.DoubleArithmetics.IsZero(currentWidth) || isLessThan)
            {
                renderInfo.Update(cellSlot, cellWidth);
                return isLessThan;
            }

            return false;
        }

        internal virtual double GetSlotHeight(int cellSlot)
        {
            double height = 0;

            // TODO: First check for Resized RowHeight, then for RowHeight.
            if (this.table.RowHeightIsNaN || this.table.HasExpandedRowDetails(cellSlot))
            {
                height = this.RowPool.RenderInfo.ValueForIndex(cellSlot, false);
            }
            else
            {
                height = this.table.RowHeight;
            }
            return height;
        }

        /// <summary>
        /// Updates the Height for given Slot.
        /// </summary>
        /// <param name="cellSlot">The slot which Height will be updated.</param>
        /// <param name="cellHeight">The new Height.</param>
        /// <param name="forceUpdate">Should update if true.</param>
        /// <returns>Returns true only if Slot Height was Updated (e.g. Smaller then the new Height).</returns>
        internal bool UpdateSlotHeight(int cellSlot, double cellHeight, bool forceUpdate = true)
        {
            if (this.table.RowHeightIsNaN && !this.RowPool.IsItemCollapsed(cellSlot) || this.table.HasExpandedRowDetails(cellSlot))
            {
                var renderInfo = this.RowPool.RenderInfo;
                var currentHeight = renderInfo.ValueForIndex(cellSlot, false);
                
                if (GridModel.DoubleArithmetics.IsZero(currentHeight))
                {
                    renderInfo.Update(cellSlot, cellHeight);
                    return true;
                }

                var shouldUpdateHeight = forceUpdate ? !GridModel.DoubleArithmetics.AreClose(currentHeight, cellHeight) : GridModel.DoubleArithmetics.IsLessThan(currentHeight, cellHeight);
                if (shouldUpdateHeight)
                {
                    renderInfo.Update(cellSlot, cellHeight);
                    return true;
                }
            }

            return false;
        }

        private bool ShouldStretchColumns(double availableWidth, out double difference, out int stretchableCount)
        {
            difference = 0;
            stretchableCount = 0;

            if (!this.ColumnPool.RenderInfo.HasUpdatedValues)
            {
                return false;
            }

            double total = 0;
            double availableWidthForStretch = 0;
            double stretchableColumnWidths = 0;

            for (int i = 0; i < this.ColumnPool.RenderInfo.Count; i++)
            {
                var cell = this.ColumnPool.GetDisplayedElement(i);
                if (cell == null)
                {
                    continue;
                }

                var width = this.ColumnPool.RenderInfo.ValueForIndex(i, false);

                if (cell.Column.SizeMode == DataGridColumnSizeMode.Stretch)
                {
                    stretchableColumnWidths += width;
                    availableWidthForStretch += cell.Column.LayoutWidth - cell.Column.AutoWidth;
                    stretchableCount++;
                }

                total += width;
            }

            if (GridModel.DoubleArithmetics.IsZero(stretchableColumnWidths))
            {
                return false;
            }

            difference = availableWidth - total;
            if (GridModel.DoubleArithmetics.AreClose(0, difference) || (difference < 0 && availableWidthForStretch <= 0))
            {
                return false;
            }

            return true;
        }

        private bool ForceUpdateSlotWidth(int key, double value)
        {
            double currentValue = this.ColumnPool.RenderInfo.ValueForIndex(key, false);
            if (currentValue != 0.0 && currentValue == value)
            {
                return false;
            }

            this.ColumnPool.RenderInfo.Update(key, value);

            return true;
        }
    }
}
