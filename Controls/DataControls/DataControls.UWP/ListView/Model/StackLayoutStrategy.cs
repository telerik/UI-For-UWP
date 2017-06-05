using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;

namespace Telerik.UI.Xaml.Controls.Data.ListView.Model
{
    internal class StackLayoutStrategy : BaseLayoutStrategy
    {
        private CompactLayout layout;

        public StackLayoutStrategy(ItemModelGenerator generator, IOrientedParentView owner) : base(generator, owner)
        {
            this.layout = new CompactLayout(new GroupHierarchyAdapter(), IndexStorage.UnknownItemLength);
        }

        internal override BaseLayout Layout
        {
            get
            {
                return this.layout;
            }
        }

        internal override IGenerateLayoutLength GeneratedLengthContext
        {
            get
            {
                return new StackedGeneratedLengthContext(0);
            }
        }

        public override GeneratedItemModel GetDisplayedElement(int slot, int id)
        {
            List<GeneratedItemModel> containers;
            if (this.generatedContainers.TryGetValue(slot, out containers))
            {
                return containers.LastOrDefault();
            }

            return null;
        }

        public override void ArrangeContent(RadSize adjustedfinalSize, double topOffset)
        {
            bool initialized = false;
            var topLeft = new RadPoint(0, 0);
            int elementSequenceNumber = 0;

            foreach (var pair in this.GetDisplayedElements())
            {
                var decorators = pair.Value;
                RadRect arrangeRect = new RadRect();
                double length = this.IsHorizontal ? adjustedfinalSize.Height : adjustedfinalSize.Width;
                double itemEnd = 0;
                foreach (var decorator in decorators)
                {
                    if (!initialized || elementSequenceNumber != decorator.ItemInfo.Slot)
                    {
                        initialized = true;
                        var offset = decorator.ItemInfo.Slot != 0 ? this.layout.PhysicalOffsetFromSlot(decorator.ItemInfo.Slot - 1) : 0;
                        elementSequenceNumber = decorator.ItemInfo.Slot;

                        if (this.IsHorizontal)
                        {
                            topLeft.X = offset;
                        }
                        else
                        {
                            topLeft.Y = offset + topOffset;
                        }
                    }

                    itemEnd = this.layout.PhysicalOffsetFromSlot(decorator.ItemInfo.Slot);

                    if (this.IsHorizontal)
                    {
                        arrangeRect = new RadRect(topLeft.X, topLeft.Y, itemEnd - topLeft.X, length);
                    }
                    else
                    {
                        arrangeRect = new RadRect(topLeft.X, topLeft.Y, length, itemEnd - topLeft.Y + topOffset);
                    }

                    decorator.LayoutSlot = arrangeRect;
                    this.Owner.Arrange(decorator);
                }

                if (this.IsHorizontal)
                {
                    topLeft.X += arrangeRect.Width;
                }
                else
                {
                    topLeft.Y += arrangeRect.Height;
                }
                elementSequenceNumber++;
            }

            this.ArrangeFrozenDecorators();
        }

        public override int GetElementFlatIndex(int index)
        {
            return index;
        }

        internal override RadSize ComputeDesiredSize(MeasureContext context)
        {
            RadSize desiredSize;

            if (this.IsHorizontal)
            {
                desiredSize = new RadSize(context.GeneratedLength, context.MaxLength);
                desiredSize.Width = Math.Min(desiredSize.Width, context.AvailableLength);
                desiredSize.Height = Math.Max(desiredSize.Height, ListViewModel.DoubleArithmetics.Ceiling(this.AvailableSize.Height));
            }
            else
            {
                desiredSize = new RadSize(Math.Max(AvailableSize.Width, context.MaxLength), Math.Min(context.AvailableLength, context.GeneratedLength));
            }

            return desiredSize;
        }

        internal override RadSize GenerateContainer(IList<ItemInfo> itemInfos, BaseLayoutStrategy.MeasureContext context)
        {
            int slot = itemInfos.Last().Slot;

            List<GeneratedItemModel> decorators;
            if (!this.generatedContainers.TryGetValue(slot, out decorators))
            {
                decorators = new List<GeneratedItemModel>();
                this.generatedContainers[slot] = decorators;
            }
            else
            {
                return RadSize.Empty;
            }

            for (int i = 0; i < itemInfos.Count; i++)
            {
                var itemInfo = itemInfos[i];

                if (itemInfo.IsDisplayed || itemInfo.IsSummaryVisible)
                {
                    GeneratedItemModel decorator = this.GetPreviouslyVisibleDecorator(itemInfo.Item);

                    // Recycle cells on this slot if container is Collasped/Expanded.
                    if (decorator != null && (decorator.ItemInfo.IsCollapsed != itemInfo.IsCollapsed))
                    {
                        this.Generator.RecycleDecorator(decorator);
                        decorator = null;
                    }

                    if (decorator == null)
                    {
                        decorator = this.GenerateAndPrepareContainer(ref itemInfo);
                    }

                    decorator.ItemInfo = itemInfo;
                    decorators.Add(decorator);
                }
            }

            GeneratedItemModel lastElement = null;
            double largestLength = 0;

            double comulativeScrollLength = 0;
            foreach (var decorator in decorators)
            {
                var desiredSize = this.Owner.Measure(decorator, this.GetContainerAvailableSize(decorator.ItemInfo));

                double length;
                double width;
                if (this.IsHorizontal)
                {
                    comulativeScrollLength += desiredSize.Width;
                    width = desiredSize.Width;
                    length = desiredSize.Height;
                }
                else
                {
                    comulativeScrollLength += desiredSize.Height;
                    width = desiredSize.Height;
                    length = desiredSize.Width;
                }

                lastElement = decorator;
                largestLength = Math.Max(largestLength, length);

                this.layout.UpdateSlotLength(decorator.ItemInfo.Slot, width);
            }

            for (int i = 0; i < itemInfos.Count; i++)
            {
                this.UpdateFrozenContainerInfos(itemInfos[i]);
            }

            this.UpdateAverageContainerLength(comulativeScrollLength);

            if (this.IsHorizontal)
            {
                return new RadSize(comulativeScrollLength, largestLength);
            }
            else
            {
                return new RadSize(largestLength, comulativeScrollLength);
            }
        }

        private RadSize GetContainerAvailableSize(ItemInfo info)
        {
            return this.IsHorizontal ? new RadSize(double.PositiveInfinity, this.AvailableSize.Height) : new RadSize(this.AvailableSize.Width, double.PositiveInfinity);
        }
    }
}
