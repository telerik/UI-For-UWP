using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Telerik.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;

namespace Telerik.UI.Xaml.Controls.Data.ListView.Model
{
    internal class WrapLayoutStrategy : BaseLayoutStrategy
    {
        private WrapLayout layout;
        private HashSet<object> generatedContainerItems = new HashSet<object>();

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "These virtual calls do not rely on uninitialized base state.")]
        public WrapLayoutStrategy(ItemModelGenerator generator, IOrientedParentView owner, double defaultItemLength, double defaultItemOppositeLength) : base(generator, owner)
        {
            this.layout = new WrapLayout(new GroupHierarchyAdapter(), defaultItemLength, defaultItemOppositeLength);
            this.ItemWidth = defaultItemOppositeLength;
        }

        internal double ItemWidth
        {
            get
            {
                if (this.Layout != null)
                {
                    return this.layout.DefaultItemOppositeLength;
                }

                return 100;
            }
            set
            {
                if (this.Layout != null)
                {
                    this.layout.DefaultItemOppositeLength = value;
                }
            }
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

        public override void ArrangeContent(RadSize adjustedfinalSize, double topOffset)
        {
            bool initialized = false;
            var topLeft = new RadPoint(0, 0);
            int elementSequenceNumber = 0;

            double oppositAvalilableLength = this.IsHorizontal ? adjustedfinalSize.Height : adjustedfinalSize.Width;

            foreach (var pair in this.GetDisplayedElements())
            {
                var decorators = pair.Value;
                RadRect arrangeRect = new RadRect();
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

                    var shouldStretch = decorator.ItemInfo.Item is Telerik.Data.Core.IGroup; /*|| decorator.ItemInfo.Item is Telerik.Data.Core.PlaceholderInfo - reconsider whether we need to include load more items button in the layout*/

                    if (this.IsHorizontal)
                    {
                        var arrangeHeight = shouldStretch ? oppositAvalilableLength : this.ItemWidth;
                        arrangeRect = new RadRect(topLeft.X, topLeft.Y, itemEnd - topLeft.X, arrangeHeight);
                    }
                    else
                    {
                        var arrangeWidth = shouldStretch ? oppositAvalilableLength : this.ItemWidth;
                        arrangeRect = new RadRect(topLeft.X, topLeft.Y, arrangeWidth, itemEnd - topLeft.Y + topOffset);
                    }

                    decorator.LayoutSlot = arrangeRect;
                    this.Owner.Arrange(decorator);

                    if (this.IsHorizontal)
                    {
                        topLeft.Y += arrangeRect.Height;
                    }
                    else
                    {
                        topLeft.X += arrangeRect.Width;
                    }
                }

                if (this.IsHorizontal)
                {
                    topLeft.X += arrangeRect.Width;
                    topLeft.Y = 0;
                }
                else
                {
                    topLeft.Y += arrangeRect.Height;
                    topLeft.X = 0;
                }

                elementSequenceNumber++;
            }

            this.ArrangeFrozenDecorators();
        }

        public override GeneratedItemModel GetDisplayedElement(int slot, int id)
        {
            List<GeneratedItemModel> containers;
            if (this.generatedContainers.TryGetValue(slot, out containers))
            {
                var visibleContainers = containers.Where((model) =>
                     {
                         return model.ItemInfo.Id == id;
                     });
                if (visibleContainers.Count() > 0)
                {
                    return visibleContainers.First();
                }
            }

            return null;
        }

        public override int GetElementFlatIndex(int index)
        {
            return (int)Math.Ceiling(this.layout.ColumnSlotsRenderInfo.OffsetFromIndex(index) / ((int)(this.layout.AvailableOppositeLength / this.layout.DefaultItemOppositeLength) * this.layout.DefaultItemOppositeLength)) - 1;
        }

        internal override void InitializeForMeasure()
        {
            var oppositeLength = this.IsHorizontal ? this.AvailableSize.Height : this.AvailableSize.Width;
            if (oppositeLength > 0)
            {
                this.layout.AvailableOppositeLength = oppositeLength;
                this.layout.Initialize();
            }
        }

        internal override void OnOrientationChanged()
        {
            base.OnOrientationChanged();
            if (this.layout == null)
            {
                return;
            }

            var newLayout = new WrapLayout(new GroupHierarchyAdapter(), this.layout.DefaultItemLength, this.ItemWidth);
            newLayout.SetSource(this.layout.ItemsSource, this.layout.GroupLevels, this.layout.TotalsPosition, this.layout.AggregatesLevel, 0, this.layout.ShowAggregateValuesInline);
            this.layout = newLayout;
        }

        internal override void RecycleUnused()
        {
            base.RecycleUnused();
            this.generatedContainerItems.Clear();
        }

        internal override RadSize ComputeDesiredSize(MeasureContext context)
        {
            RadSize desiredSize;

            if (this.IsHorizontal)
            {
                desiredSize = new RadSize(context.GeneratedLength, context.MaxLength);
                desiredSize.Width = Math.Min(desiredSize.Width, context.AvailableLength);
                desiredSize.Height = Math.Min(desiredSize.Height, ListViewModel.DoubleArithmetics.Ceiling(this.AvailableSize.Height));
            }
            else
            {
                desiredSize = new RadSize(context.MaxLength, context.GeneratedLength);
                desiredSize.Width = Math.Min(desiredSize.Width, ListViewModel.DoubleArithmetics.Ceiling(this.AvailableSize.Width));
                desiredSize.Height = Math.Min(desiredSize.Height, context.AvailableLength);
            }

            return desiredSize;
        }
        
        internal override void RecycleLocally()
        {
            base.RecycleLocally();
            this.generatedContainerItems.Clear();
        }

        internal override RadSize GenerateContainer(IList<ItemInfo> itemInfos, MeasureContext context)
        {
            int startColumnId = -1;
            int slot = itemInfos.Last().Slot;
            int lastProjectedId = this.layout.GetLastProjectedId(slot);

            List<GeneratedItemModel> decorators;
            if (!this.generatedContainers.TryGetValue(slot, out decorators))
            {
                decorators = new List<GeneratedItemModel>();
                this.generatedContainers[slot] = decorators;
            }

            double oppositeAvalilableLength = this.IsHorizontal ? this.AvailableSize.Height : this.AvailableSize.Width;
            double largestLength = 0;
            double cumulativeOppositeScrollLength = 0;
            int lastRealizedId = -1;

            for (int i = 0; i < itemInfos.Count; i++)
            {
                var itemInfo = itemInfos[i];

                // Check if item is realized from previous row.
                if (this.generatedContainerItems.Contains(itemInfo.Item))
                {
                    continue;
                }

                if (itemInfo.IsDisplayed || itemInfo.IsSummaryVisible)
                {
                    if (cumulativeOppositeScrollLength + this.ItemWidth > context.OppositeAvailableLength)
                    {
                        break;
                    }

                    if (startColumnId == -1)
                    {
                        startColumnId = itemInfo.Id;
                    }

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
                    this.generatedContainerItems.Add(itemInfo.Item);

                    var desiredSize = this.Owner.Measure(decorator, this.GetContainerAvailableSize(decorator.ItemInfo));
                    decorator.DesiredSize = desiredSize;

                    double length;
                    double oppositeLength = 0;
                    if (this.IsHorizontal)
                    {
                        var actualItemlength = this.ItemWidth > 0 ? this.ItemWidth : desiredSize.Height;

                        oppositeLength = decorator.ItemInfo.Item is Telerik.Data.Core.IGroup ? oppositeAvalilableLength : actualItemlength; // TODO replace this with desired size.
                        cumulativeOppositeScrollLength += oppositeLength;
                        length = desiredSize.Width;
                    }
                    else
                    {
                        var actualItemlength = this.ItemWidth > 0 ? this.ItemWidth : desiredSize.Width;

                        oppositeLength = decorator.ItemInfo.Item is Telerik.Data.Core.IGroup ? oppositeAvalilableLength : actualItemlength;
                        cumulativeOppositeScrollLength += oppositeLength;
                        length = desiredSize.Height;
                    }

                    largestLength = Math.Max(largestLength, length);

                    this.layout.UpdateSlotLength(decorator.ItemInfo.Slot, largestLength);
                    this.layout.ColumnSlotsRenderInfo.Update(decorator.ItemInfo.Id, oppositeLength);

                    if (cumulativeOppositeScrollLength > context.OppositeAvailableLength && Math.Abs(cumulativeOppositeScrollLength - context.OppositeAvailableLength) > 1)
                    {
                        this.RecycleLocallyContainer(decorator);
                        this.generatedContainerItems.Remove(decorator.ItemInfo.Item);

                        break;
                    }

                    lastRealizedId = decorator.ItemInfo.Id;
                }
            }

            if (lastRealizedId >= 0)
            {
                this.layout.EndSlotMeasure(slot, startColumnId, lastProjectedId, lastRealizedId);
            }

            for (int i = 0; i < itemInfos.Count; i++)
            {
                this.UpdateFrozenContainerInfos(itemInfos[i]);
            }

            if (this.IsHorizontal)
            {
                return new RadSize(largestLength, cumulativeOppositeScrollLength);
            }
            else
            {
                return new RadSize(cumulativeOppositeScrollLength, largestLength);
            }
        }

        private RadSize GetContainerAvailableSize(ItemInfo info)
        {
            return this.IsHorizontal ? new RadSize(double.PositiveInfinity, this.ItemWidth) : new RadSize(this.ItemWidth, double.PositiveInfinity);
        }
    }
}