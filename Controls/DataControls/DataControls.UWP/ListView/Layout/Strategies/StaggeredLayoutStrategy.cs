using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Data;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Data.ListView;
using Telerik.UI.Xaml.Controls.Data.ListView.Model;

namespace Telerik.Data.Core.Layouts
{
    internal class StaggeredLayoutStrategy : BaseLayoutStrategy
    {
        private StaggeredLayout layout;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "These virtual calls do not rely on uninitialized base state.")]
        public StaggeredLayoutStrategy(ItemModelGenerator generator, IOrientedParentView view, double defaultItemHeight, int stackCount)
            : base(generator, view)
        {
            this.layout = new StaggeredLayout(new GroupHierarchyAdapter(), defaultItemHeight, stackCount);
            this.StackCount = stackCount;
        }

        internal int StackCount
        {
            get
            {
                if (this.Layout != null)
                {
                    return this.layout.StackCount;
                }

                return 1;
            }
            set
            {
                if (this.Layout != null)
                {
                    this.layout.StackCount = value;
                }
            }
        }

        internal override Telerik.Data.Core.Layouts.BaseLayout Layout
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
                return new StaggeredGeneratedLength(this.StackCount);
            }
        }

        public override void ArrangeContent(RadSize adjustedfinalSize, double topOffset)
        {
            bool initialized = false;
            var topLeft = new RadPoint(0, 0);

            foreach (var pair in this.GetDisplayedElements())
            {
                double largestLength = -1;

                var decorators = pair.Value;
                RadRect arrangeRect = new RadRect();
                double stackLength = (this.IsHorizontal ? adjustedfinalSize.Height : adjustedfinalSize.Width) / this.StackCount;
                double itemlength = 0;

                int elementSequenceNumber = 0;

                foreach (var decorator in decorators)
                {
                    elementSequenceNumber = this.layout.ColumnRenderInfo.GetColumnForId(decorator.ItemInfo.Id);

                    var offset = decorator.ItemInfo.Slot != 0 ? this.layout.ColumnRenderInfo.PhysicalOffsetFromSlot(decorator.ItemInfo.Id) : 0;

                    if (!initialized)
                    {
                        initialized = true;

                        if (this.IsHorizontal)
                        {
                            // topLeft.X = offset;
                        }
                        else
                        {
                            topLeft.Y = topOffset;
                        }
                    }

                    if (this.IsHorizontal)
                    {
                        topLeft.X = offset;
                    }
                    else
                    {
                        topLeft.Y = offset;
                    }

                    itemlength = this.layout.PhysicalLengthForSlot(decorator.ItemInfo.Slot);

                    if (this.IsHorizontal)
                    {
                        topLeft.Y = elementSequenceNumber * stackLength;
                        var oppositeLength = decorator.ItemInfo.Item is Telerik.Data.Core.IGroup ? adjustedfinalSize.Height : stackLength;
                        arrangeRect = new RadRect(topLeft.X, topLeft.Y, itemlength, oppositeLength);
                        largestLength = Math.Max(largestLength, arrangeRect.Width);
                    }
                    else
                    {
                        topLeft.X = elementSequenceNumber * stackLength;
                        var oppositeLength = decorator.ItemInfo.Item is Telerik.Data.Core.IGroup ? adjustedfinalSize.Width : stackLength;
                        arrangeRect = new RadRect(topLeft.X, topLeft.Y, oppositeLength, itemlength + topOffset);
                        largestLength = Math.Max(largestLength, arrangeRect.Height);
                    }

                    decorator.LayoutSlot = arrangeRect;
                    this.Owner.Arrange(decorator);

                    // elementSequenceNumber++;
                }

                if (this.IsHorizontal)
                {
                    topLeft.X += largestLength;
                }
                else
                {
                    topLeft.Y += largestLength;
                }

                this.ArrangeFrozenDecorators();
            }
        }

        public override GeneratedItemModel GetDisplayedElement(int slot, int id)
        {
            List<GeneratedItemModel> containers;
            if (this.generatedContainers.TryGetValue(slot, out containers))
            {
                return containers.FirstOrDefault(model => model.ItemInfo.Id == id);
            }

            return null;
        }

        public override int GetElementFlatIndex(int index)
        {
            return index;
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

            double largestLength = 0;

            double cumulativeStackLength = 0;
            foreach (var decorator in decorators)
            {
                var desiredSize = this.Owner.Measure(decorator, this.GetContainerAvailableSize(decorator.ItemInfo));

                double scrollLength;
                if (this.IsHorizontal)
                {
                    cumulativeStackLength += desiredSize.Height;
                    scrollLength = desiredSize.Width;
                }
                else
                {
                    cumulativeStackLength += desiredSize.Width;
                    scrollLength = desiredSize.Height;
                }

                largestLength = Math.Max(largestLength, scrollLength);

                this.Layout.UpdateSlotLength(decorator.ItemInfo.Slot, largestLength);
                this.layout.ColumnRenderInfo.Update(decorator.ItemInfo.Id, largestLength);
            }

            this.UpdateAverageContainerLength(largestLength);
            if (this.IsHorizontal)
            {
                return new RadSize(largestLength, cumulativeStackLength);
            }
            else
            {
                return new RadSize(cumulativeStackLength, largestLength);
            }
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
        
        private RadSize GetContainerAvailableSize(ItemInfo info)
        {
            if (info.Item is Telerik.Data.Core.IGroup)
            {
                return this.IsHorizontal ? new RadSize(double.PositiveInfinity, this.AvailableSize.Height) : new RadSize(this.AvailableSize.Width, double.PositiveInfinity);
            }

            return this.IsHorizontal ? new RadSize(double.PositiveInfinity, this.AvailableSize.Height / this.StackCount) : new RadSize(this.AvailableSize.Width / this.StackCount, double.PositiveInfinity);
        }
    }
}
