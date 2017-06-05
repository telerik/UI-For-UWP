using System;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Data.HexView
{
    internal class AngledHexLayoutStrategy : HexLayoutStrategyBase
    {
        public AngledHexLayoutStrategy(HexItemModelGenerator generator, RadHexView owner, AngledHexLayoutDefinition definition)
            : base(generator, owner, definition)
        {
        }

        /// <summary>
        /// Length    /\  /\
        ///          |  ||  | 
        ///           \/  \/
        ///   oppositeLength
        ///  -availableLength-.
        /// </summary>
        /// <returns>The top left position of the i-th element.</returns>
        protected override Point GetAbsolutePositionFromIndex(int index, double length, double oppositeLength, double offset, double oppositeOffset, double availableLength)
        {
            var evenRowItemsCount = (int)(availableLength / oppositeLength);  // row index is zero based
            var oddRowItemsCount = (int)((availableLength - 0.5 * oppositeLength) / oppositeLength);

            var doubleRowsCount = index / (evenRowItemsCount + oddRowItemsCount);
            var doubleRowsHeight = doubleRowsCount * 1.5 * length;

            var remainingItems = index % (evenRowItemsCount + oddRowItemsCount);

            var y = doubleRowsHeight + (remainingItems >= evenRowItemsCount ? 0.75 * length : 0);
            var x = remainingItems >= evenRowItemsCount ?
                  0.5 * oppositeLength + (remainingItems - evenRowItemsCount) * oppositeLength :
                  remainingItems * oppositeLength;

            return new Point(x + oppositeOffset, y + offset);
        }

        protected override Size GetAbsoluteElementSize(double length, double oppositeLength)
        {
            return new Size(oppositeLength, length);
        }

        protected override HexOrientation GetElementOrientation()
        {
            return this.IsVertical ? HexOrientation.Angled : HexOrientation.Flat;
        }

        protected override int GetIndexFromOffset(double length, double oppositeLength, double availableLength, double offset)
        {
            if (offset <= 0)
            {
                return 0;
            }
            var evenRowItemsCount = (int)(availableLength / oppositeLength);
            var oddRowItemsCount = Math.Max(0, (int)((availableLength - 0.5 * oppositeLength) / oppositeLength));

            var rowHeight = oddRowItemsCount > 0 ? 1.5 * length : 0.75 * length;

            var rowsCount = (int)(offset / rowHeight);
            var remainingHeight = offset % rowHeight;

            int index;
            if (remainingHeight < 0.25 * length)
            {
                index = Math.Max(0, (rowsCount - 1) * (evenRowItemsCount + oddRowItemsCount) + evenRowItemsCount);
            }
            else if (remainingHeight < 0.75 * length)
            {
                index = rowsCount * (evenRowItemsCount + oddRowItemsCount);
            }
            else
            {
                index = rowsCount * (evenRowItemsCount + oddRowItemsCount) + evenRowItemsCount;
            }

            return index;
        }

        protected override Size GetAbsoluteContentSize(int count, double length, double oppositeLength, double availableLength)
        {
            // index is zero based
            var evenRowItemsCount = (int)(availableLength / oppositeLength);
            var oddRowItemsCount = (int)((availableLength - 0.5 * oppositeLength) / oppositeLength);

            var doubleRowsCount = count / (evenRowItemsCount + oddRowItemsCount);
            var doubleRowsHeight = doubleRowsCount * 1.5 * length;

            var remainingItems = count % (evenRowItemsCount + oddRowItemsCount);

            var height = doubleRowsHeight;
            var width = evenRowItemsCount * oppositeLength + (evenRowItemsCount == oddRowItemsCount ? 0.5 * oppositeLength : 0);

            if (remainingItems == 0)
            {
                height += 0.25 * length;
            }
            else if (remainingItems <= evenRowItemsCount)
            {
                height += length;
            }
            else
            {
                height += 1.75 * length;
            }

            return new Size(width, height);
        }
    }
}
