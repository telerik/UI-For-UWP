using System;
using System.Collections.Generic;
using Telerik.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.HexView
{
    internal abstract class HexLayoutStrategyBase : DependencyObject
    {
        private const double OppositeLengthFactor = 0.8660254037844386;
        private const double OppositeSpacingFactor = 1.154700538379252;

        private static readonly Thickness thickness = new Thickness(0, 0, 0, 0);

        private readonly RadHexView owner;
        private readonly HexItemModelGenerator generator;
        private readonly bool isVertical;
        private readonly double totalItemLength;
        private readonly double totalItemOppositeLength;
        private readonly double halfSpacing;
        private readonly double halfOppositeSpacing;
        private readonly double viewportExtension;
        private readonly double rawItemLength;
        private readonly double rawItemOppositeLength;

        private Dictionary<object, GeneratedItemModel> generatedContainers = new Dictionary<object, GeneratedItemModel>();

        public HexLayoutStrategyBase(HexItemModelGenerator generator, RadHexView owner, HexLayoutDefinitionBase layoutDefinition)
        {
            this.generator = generator;
            this.owner = owner;

            this.isVertical = layoutDefinition.Orientation == Orientation.Vertical;

            this.halfSpacing = Math.Round(layoutDefinition.ItemsSpacing * OppositeSpacingFactor / 2);
            this.halfOppositeSpacing = Math.Round(layoutDefinition.ItemsSpacing / 2);

            // used only when calculating the size of the item
            this.rawItemLength = Math.Round(layoutDefinition.ItemLength);
            this.rawItemOppositeLength = Math.Round(layoutDefinition.ItemLength * OppositeLengthFactor);

            this.totalItemLength = layoutDefinition.ItemLength + layoutDefinition.ItemsSpacing * OppositeSpacingFactor;
            this.totalItemOppositeLength = layoutDefinition.ItemLength * OppositeLengthFactor + layoutDefinition.ItemsSpacing;

            var extension = this.totalItemLength % 4;
            var oppositeExtension = this.totalItemOppositeLength % 2;

            this.totalItemLength += extension != 0 ? 4 - extension : 0;
            this.totalItemOppositeLength += oppositeExtension != 0 ? 2 - oppositeExtension : 0;

            this.viewportExtension = layoutDefinition.ViewPortExtension;
        }

        public bool IsVertical
        {
            get
            {
                return this.isVertical;
            }
        }

        internal Size MeasureContent(Size viewportSize, double scrollOffset)
        {
            if (this.owner.SourceView == null)
            {
                return new Size();
            }

            var items = this.owner.SourceView.InternalList;
            var newGeneratedContainers = new Dictionary<object, GeneratedItemModel>();

            double startOffset, endOffset;
            Size contentSize;

            if (this.isVertical)
            {
                startOffset = scrollOffset - viewportSize.Height * this.viewportExtension;
                endOffset = scrollOffset + viewportSize.Height * (1 + this.viewportExtension);
                contentSize = this.GetContentSize(viewportSize.Width, items.Count);
            }
            else
            {
                startOffset = scrollOffset - viewportSize.Width * this.viewportExtension;
                endOffset = scrollOffset + viewportSize.Width * (1 + this.viewportExtension);
                contentSize = this.GetContentSize(viewportSize.Height, items.Count);
            }

            int startIndex = Math.Max(0, this.GetIndexFromOffset(contentSize, startOffset));
            for (int i = startIndex; i < items.Count; i++)
            {
                var itemRect = this.GetLayoutSlotForIndex(i, viewportSize);
                var item = items[i];

                if (this.isVertical ? itemRect.Y > endOffset : itemRect.Right > endOffset)
                {
                    break;
                }
                else
                {
                    GeneratedItemModel container;
                    if (this.generatedContainers.ContainsKey(item))
                    {
                        container = this.generatedContainers[item];
                        this.generatedContainers.Remove(item);
                    }
                    else
                    {
                        container = this.generator.GenerateContainer(items[i], items[i]);
                    }

                    container.LayoutSlot = itemRect;
                    container.ItemInfo = new ItemInfo { Item = item };

                    newGeneratedContainers.Add(item, container);

                    this.generator.PrepareContainerForItem(container);
                    this.UpdateContainerLayoutParameters(container);
                    (container.Container as UIElement).Measure(new Size(itemRect.Width, itemRect.Height));
                }
            }

            foreach (var pair in this.generatedContainers)
            {
                this.generator.RecycleDecorator(pair.Value);
            }

            this.generatedContainers = newGeneratedContainers;

            return contentSize;
        }

        internal void ArrangeContent()
        {
            foreach (var pair in this.generatedContainers)
            {
                var element = pair.Value.Container as RadHexHubTile;
                element.UpdateInterval = this.owner.GetRandomUpdateInterval();
                var rect = pair.Value.LayoutSlot.ToRect();
                element.Arrange(rect);
            }
        }

        internal void RecycleAllItems()
        {
            foreach (var pair in this.generatedContainers)
            {
                this.generator.RecycleDecorator(pair.Value);
            }

            this.generatedContainers.Clear();
        }

        internal bool RequiresUpdate(Size availableSize, double previousOffset, double offset)
        {
            var index = this.GetIndexFromOffset(availableSize, offset);
            var prevIndex = this.GetIndexFromOffset(availableSize, previousOffset);

            return index != prevIndex;
        }

        protected abstract HexOrientation GetElementOrientation();

        protected abstract int GetIndexFromOffset(double length, double oppositeLength, double availableLength, double offset);

        protected abstract Size GetAbsoluteContentSize(int count, double length, double oppositeLength, double availableLength);

        protected abstract Size GetAbsoluteElementSize(double length, double oppositeLength);

        protected abstract Point GetAbsolutePositionFromIndex(int index, double length, double oppositeLength, double offset, double oppositeOffset, double availableLength);

        private void UpdateContainerLayoutParameters(GeneratedItemModel container)
        {
            var element = container.Container as RadHexHubTile;
            if (element.Length != this.rawItemLength)
            {
                element.Length = this.rawItemLength;
            }

            var orientation = this.GetElementOrientation();
            if (element.Orientation != orientation)
            {
                element.Orientation = orientation;
            }

            if (element.Margin != thickness)
            {
                element.Margin = thickness;
            }
        }

        private RadRect GetLayoutSlotForIndex(int index, Size availableSize)
        {
            var availableLength = Math.Round(this.isVertical ? availableSize.Width : availableSize.Height);
            var absolutePosition = this.GetAbsolutePositionFromIndex(index, this.totalItemLength, this.totalItemOppositeLength, this.halfSpacing, this.halfOppositeSpacing, availableLength);
            var size = this.GetAbsoluteElementSize(this.rawItemLength, this.rawItemOppositeLength);

            if (this.isVertical)
            {
                return new RadRect(absolutePosition.X, absolutePosition.Y, size.Width, size.Height);
            }
            else
            {
                return new RadRect(absolutePosition.Y, absolutePosition.X, size.Height, size.Width);
            }
        }

        private int GetIndexFromOffset(Size availableSize, double offset)
        {
            var availableLength = Math.Round(this.isVertical ? availableSize.Width : availableSize.Height);
            return this.GetIndexFromOffset(this.totalItemLength, this.totalItemOppositeLength, availableLength, offset);
        }

        private Size GetContentSize(double availableLength, int itemsCount)
        {
            var size = this.GetAbsoluteContentSize(itemsCount, this.totalItemLength, this.totalItemOppositeLength, availableLength);

            if (this.isVertical)
            {
                return size;
            }
            else
            {
                return new Size(size.Height, size.Width);
            }
        }
    }
}