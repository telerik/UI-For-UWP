using System;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Data.HexView
{
    internal class FlatTightHexLayoutStrategy : HexLayoutStrategyBase
    {
        public FlatTightHexLayoutStrategy(HexItemModelGenerator generator, RadHexView owner, FlatTightHexLayoutDefinition definition)
            : base(generator, owner, definition)
        {
        }

        /// <summary>
        /// Example         /0\_/2\_
        ///                 \_/1\_/3\  oppositeLength
        ///                   \_/ \_/     
        ///                   length
        ///               -availableLength-.
        /// </summary>
        /// <returns>The top left position of the i-th element.</returns>
        protected override Point GetAbsolutePositionFromIndex(int index, double length, double oppositeLength, double offset, double oppositeOffset, double availableLength)
        {
            var effectiveItemLength = length * 0.75;
            var rowItemsCount = 1 + Math.Max(0, (int)((availableLength - length) / effectiveItemLength));

            var rowsCount = index / rowItemsCount;
            var remainingItems = index % rowItemsCount;

            var y = rowsCount * oppositeLength + (remainingItems % 2 == 0 ? 0 : 0.5 * oppositeLength);
            var x = remainingItems * effectiveItemLength;

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
            var effectiveItemLength = length * 0.75;
            var rowItemsCount = 1 + Math.Max(0, (int)((availableLength - length) / effectiveItemLength));

            var rowsCount = (int)(offset / oppositeLength);
            var remainingHeight = offset % oppositeLength;

            var count = rowsCount * rowItemsCount;

            if (remainingHeight < oppositeLength / 2)
            {
                return count;
            }
            else
            {
                return count + 1;
            }
        }

        protected override Size GetAbsoluteContentSize(int count, double length, double oppositeLength, double availableLength)
        {
            var effectiveItemLength = length * 0.75;
            var rowItemsCount = 1 + Math.Max(0, (int)((availableLength - length) / effectiveItemLength));

            var rowsCount = count / rowItemsCount;
            var remainingItems = count % rowItemsCount;

            var width = rowItemsCount * effectiveItemLength + 0.25 * length;
            var height = rowsCount * oppositeLength;

            if (remainingItems == 0)
            {
                height += 0.5 * oppositeLength;
            }
            else if (remainingItems == 1)
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
