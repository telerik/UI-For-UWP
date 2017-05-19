using System;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Data.HexView
{
    internal class FlatLooseHexLayoutStrategy : HexLayoutStrategyBase
    {
        public FlatLooseHexLayoutStrategy(HexItemModelGenerator generator, RadHexView owner, FlatLooseHexLayoutDefinition definition)
            : base(generator, owner, definition)
        {
        }

        /// <summary>
        /// Example   _   _   _ 
        ///          /0\_/1\ /2\
        ///          \_/3\_/ \_/  oppositeLength
        ///            \_/      
        ///             length
        /// <availableLength>
        /// </availableLength>
        /// </summary>
        /// <returns>The top left position of the i-th element.</returns>
        protected override Point GetAbsolutePositionFromIndex(int index, double length, double oppositeLength, double offset, double oppositeOffset, double availableLength)
        {
            var effectiveItemLength = length * 1.5;
            var evenRowItemsCount = 1 + Math.Max(0, (int)((availableLength - length) / effectiveItemLength)); // zero based row index
            var oddRowItemsCount = Math.Max(0, (int)((availableLength - 0.25 * length) / effectiveItemLength));

            var doubleRowsCount = index / (evenRowItemsCount + oddRowItemsCount);
            var doubleRowsHeight = doubleRowsCount * oppositeLength;

            var remainingItems = index % (evenRowItemsCount + oddRowItemsCount);

            var x = remainingItems >= evenRowItemsCount ?
                            0.75 * length + (remainingItems - evenRowItemsCount) * effectiveItemLength :
                            remainingItems * effectiveItemLength;
            var y = doubleRowsHeight + (remainingItems >= evenRowItemsCount ? oppositeLength * 0.5 : 0);

            return new Point(x + offset, y + oppositeOffset);
        }

        protected override Size GetAbsoluteElementSize(double length, double oppositeLength)
        {
            return new Size(length, oppositeLength);
        }

        protected override HexOrientation GetElementOrientation()
        {
            return this.IsVertical ? HexOrientation.Flat : HexOrientation.Angled;
        }

        protected override int GetIndexFromOffset(double length, double oppositeLength, double availableLength, double offset)
        {
            var effectiveItemLength = length * 1.5;
            var evenRowItemsCount = 1 + Math.Max(0, (int)((availableLength - length) / effectiveItemLength)); // zero based row index
            var oddRowItemsCount = Math.Max(0, (int)((availableLength - 0.25 * length) / effectiveItemLength));

            var rowsCount = (int)(offset / oppositeLength);
            var remainingHeight = offset % oppositeLength;

            var count = rowsCount * (evenRowItemsCount + oddRowItemsCount);

            if (remainingHeight < 0.5 * oppositeLength)
            {
                return Math.Max(0, count - oddRowItemsCount);
            }
            else
            {
                return count;
            }
        }

        protected override Size GetAbsoluteContentSize(int count, double length, double oppositeLength, double availableLength)
        {
            if (count == 0)
            {
                return new Size();
            }

            var effectiveItemLength = length * 1.5;
            var evenRowItemsCount = 1 + Math.Max(0, (int)((availableLength - length) / effectiveItemLength)); // zero based row index
            var oddRowItemsCount = Math.Max(0, (int)((availableLength - 0.25 * length) / effectiveItemLength));

            var doubleRowsCount = count / (evenRowItemsCount + oddRowItemsCount);
            var remainingItems = count % (evenRowItemsCount + oddRowItemsCount);

            var height = doubleRowsCount * oppositeLength;
            var width = (evenRowItemsCount + oddRowItemsCount) * 0.75 * length + (oddRowItemsCount < evenRowItemsCount ? 0 : 0.25 * length);

            if (remainingItems == 0)
            {
                height += 0.5 * oppositeLength;
            }
            else if (remainingItems <= evenRowItemsCount)
            {
                height += oppositeLength;
            }
            else
            {
                height += 1.5 * oppositeLength;
            }

            return new Size(width, height);
        }
    }
}
