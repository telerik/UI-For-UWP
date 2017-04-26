using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Core;
using Telerik.Data.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;

namespace Telerik.UI.Xaml.Controls.Data.ListView.Model
{
    // TODO: there exist another LayoutStrategyBase in Telerik.Data.Core.Layouts

    /// <summary>
    /// Enables the IOrientedParent to layout its elements in a specified order.
    /// </summary>
    /// <remarks>
    /// Use the following terms through the layout module:
    ///  - Id - uniquely identifies the index of an item in the source view
    ///  - Slot - identifies the row/column index depending on the orientation in the layout. Takes into account and skips collapsed items.
    ///  - Stack - identifies the columns/row (opposite) index depending on the orientation in the layout.
    /// </remarks>
    internal abstract class BaseLayoutStrategy
    {
        internal bool shouldGenerateFrozenContainer;
        internal IDictionary<int, List<GeneratedItemModel>> generatedContainers = new Dictionary<int, List<GeneratedItemModel>>();
       
        internal Dictionary<int, GeneratedItemModel> recycledFrozenDecorators = new Dictionary<int, GeneratedItemModel>();
        internal IDictionary<int, List<GeneratedItemModel>> generatedFrozenContainers = new SortedDictionary<int, List<GeneratedItemModel>>();

        internal Dictionary<int, GeneratedItemModel> generatedFrozenDecoratorsPerLevel = new Dictionary<int, GeneratedItemModel>();
        internal Dictionary<int, double> frozenDecoratorLengthPerLevel = new Dictionary<int, double>();
        internal IList<ItemInfo> frozenDecoratorsToGenerate = new List<ItemInfo>();

        internal int viewportItemsCount;
        internal double FrozenContainersLength;

        private static double defaultGroupHeaderLength = 40;
        private readonly ItemModelGenerator generator;

        private Dictionary<object, GeneratedItemModel> recycledDecorators = new Dictionary<object, GeneratedItemModel>();

        public BaseLayoutStrategy(ItemModelGenerator generator, IOrientedParentView owner)
        {
            this.generator = generator;
            this.Owner = owner;
            this.IsBufferNeeded = true;
        }
        public ItemModelGenerator Generator
        {
            get
            {
                return this.generator;
            }
        }

        public IOrientedParentView Owner { get; private set; }

        public double AvailableLength { get; protected set; }

        public RadSize AvailableSize { get; protected set; }

        internal virtual bool IsHorizontal { get; set; }

        internal virtual bool IsItemsSourceChanging { get; set; }

        /// <summary>
        /// Gets or sets the pixels that correspond to the offset of the first visible item in the viewport.
        /// </summary>
        internal double HiddenPixels { get; set; }

        internal double АverageContainerLength { get; set; }

        internal bool EnableFrozenDecorators { get; set; }

        internal abstract BaseLayout Layout { get; }
        
        internal abstract IGenerateLayoutLength GeneratedLengthContext { get; }

        /// <summary>
        /// Gets or sets a value indicating whether if buffer is needed - used for test purposes.
        /// </summary>
        private bool IsBufferNeeded { get; set; }

        public abstract void ArrangeContent(RadSize adjustedfinalSize, double initialOffset);

        public virtual RadSize MeasureContent(RadSize availableSize, double offset, double bufferScale)
        {
            this.RecycleLocally();

            this.AvailableSize = availableSize;

            this.InitializeForMeasure();

            int visibleItemCount = this.Layout.VisibleLineCount;
            if (visibleItemCount == 0)
            {
                this.Generator.FullyRecycleDecorators();
                return new RadSize();
            }

            if (this.IsHorizontal)
            {
                var resultSize = this.MeasureHorizontally(availableSize, offset, bufferScale);
                var width = this.Layout.GetTotalLength();
                var height = double.IsPositiveInfinity(availableSize.Height) ? resultSize.Height : availableSize.Height;

                return new RadSize(width, height);
            }
            else
            {
                var resultSize = this.MeasureVertically(availableSize, offset, bufferScale);
                var height = this.Layout.GetTotalLength();
                var width = double.IsPositiveInfinity(availableSize.Width) ? resultSize.Width : availableSize.Width;

                return new RadSize(width, height);
            }
        }

        public virtual IEnumerable<KeyValuePair<int, List<GeneratedItemModel>>> GetDisplayedElements()
        {
            foreach (var item in this.generatedContainers)
            {
                yield return item;
            }
        }

        public abstract GeneratedItemModel GetDisplayedElement(int slot, int id);

        public abstract int GetElementFlatIndex(int index);

        internal virtual void OnOrientationChanged()
        {
        }

        internal abstract RadSize GenerateContainer(IList<ItemInfo> list, MeasureContext context);

        internal abstract RadSize ComputeDesiredSize(MeasureContext context);

        internal virtual void InitializeForMeasure()
        {
        }

        internal virtual AddRemoveLayoutResult AddItem(object changedItem, object addRemovedItem, int index)
        {
            return this.Layout.AddItem(changedItem, addRemovedItem, index);
        }

        internal virtual AddRemoveLayoutResult RemoveItem(object changedItem, object addRemovedItem, int index)
        {
            return this.Layout.RemoveItem(changedItem, addRemovedItem, index);
        }

        internal virtual void RecycleAfterMeasure()
        {
            this.RecycleUnused();
            this.Generator.FullyRecycleDecorators();
        }

        internal virtual GeneratedItemModel GetPreviouslyVisibleDecorator(object item)
        {
            GeneratedItemModel decorator;
            if (this.recycledDecorators.TryGetValue(item, out decorator))
            {
                this.recycledDecorators.Remove(item);
            }

            return decorator;
        }

        internal GeneratedItemModel GenerateAndPrepareContainer(ref ItemInfo itemInfo)
        {
            GeneratedItemModel decorator = this.Generator.GenerateContainer(new ItemGenerationContext(itemInfo, false), itemInfo.Item);
            decorator.ItemInfo = itemInfo;
            this.Generator.PrepareContainerForItem(decorator);
            return decorator;
        }

        internal GeneratedItemModel GenerateAndPrepareFrozenContainer(ref ItemInfo itemInfo)
        {
            GeneratedItemModel decorator = this.Generator.GenerateContainer(new ItemGenerationContext(itemInfo, true), itemInfo.Item);
            decorator.ItemInfo = itemInfo;
            this.Generator.PrepareContainerForItem(decorator);
            return decorator;
        }

        internal virtual void SetSource(IReadOnlyList<object> source, int groupDescriptionCount, bool keepCollapsedState)
        {
            this.Layout.SetSource(source, groupDescriptionCount, 0, 0, 0, keepCollapsedState);
        }

        internal virtual void RecycleLocally()
        {
            this.viewportItemsCount = 0;
            this.HiddenPixels = 0;

            foreach (var pair in this.generatedContainers)
            {
                foreach (var decorator in pair.Value)
                {
                    this.recycledDecorators.Add(decorator.ItemInfo.Item, decorator);
                }
            }

            this.generatedContainers.Clear();

            foreach (var frozenPair in this.generatedFrozenContainers)
            {
                foreach (var frozenDecorator in frozenPair.Value)
                {
                    this.recycledFrozenDecorators.Add(frozenDecorator.ItemInfo.Id, frozenDecorator);
                }
            }

            this.generatedFrozenContainers.Clear();
            this.АverageContainerLength = 0;
        }

        internal virtual void RecycleLocallyContainer(GeneratedItemModel container)
        {
            this.recycledDecorators.Add(container.ItemInfo.Item, container);

            List<GeneratedItemModel> decorators;

            if (this.generatedContainers.TryGetValue(container.ItemInfo.Slot, out decorators))
            {
                decorators.Remove(container);
            }
        }

        internal virtual void RecycleUnused()
        {
            foreach (var decorator in this.recycledDecorators)
            {
                this.Recycle(decorator.Value);
            }

            this.recycledDecorators.Clear();

            foreach (var frozenDecorator in this.recycledFrozenDecorators)
            {
                this.Recycle(frozenDecorator.Value);
            }

            this.recycledFrozenDecorators.Clear();
        }

        internal virtual void FullyRecycle()
        {
            foreach (var decorators in this.generatedContainers)
            {
                foreach (var decorator in decorators.Value)
                {
                    this.Recycle(decorator);
                }
            }

            this.generatedContainers.Clear();

            foreach (var frozenDecorators in this.generatedFrozenContainers)
            {
                foreach (var frozenDecorator in frozenDecorators.Value)
                {
                    this.Recycle(frozenDecorator);
                }
            }

            this.generatedFrozenContainers.Clear();

            this.АverageContainerLength = 0;
        }

        internal virtual void RecycleAnimatedDecorator(GeneratedItemModel decorator)
        {
            this.Generator.RecycleAnimatedDecorator(decorator);
        }

        internal void UpdateAverageContainerLength(double length)
        {
            this.АverageContainerLength = (this.АverageContainerLength * (this.generatedContainers.Count - 1) + length) / this.generatedContainers.Count;
        }

        internal void GenerateFrozenContainers()
        {
            if (!this.EnableFrozenDecorators)
            {
                return;
            }

            foreach (var frozenDecorators in this.generatedFrozenContainers)
            {
                foreach (var frozenDecorator in frozenDecorators.Value)
                {
                    this.Generator.RecycleDecorator(frozenDecorator);
                }
            }
            this.generatedFrozenContainers.Clear();

            for (int i = 0; i < this.frozenDecoratorsToGenerate.Count; i++)
            {
                bool realized = false;
                var itemInfo = this.frozenDecoratorsToGenerate[i];
                double desiredLength = 0;

                // Get list with frozen decorators per slot (created if empty);
                var frozenDecorator = this.GetPreviouslyVisibleFrozenDecorator(itemInfo.Id);

                realized = frozenDecorator != null;

                // Recycle cells on this slot if container is Collasped/Expanded.
                if (realized && (frozenDecorator.ItemInfo.IsCollapsed != itemInfo.IsCollapsed))
                {
                    this.Generator.RecycleDecorator(frozenDecorator);
                    realized = false;
                }

                if (!realized)
                {
                    frozenDecorator = this.GenerateAndPrepareFrozenContainer(ref itemInfo);
                }

                if (this.generatedFrozenContainers.ContainsKey(itemInfo.Id))
                {
                    this.generatedFrozenContainers[itemInfo.Id].Add(frozenDecorator);
                }
                else
                {
                    this.generatedFrozenContainers[itemInfo.Id] = new List<GeneratedItemModel>();
                    this.generatedFrozenContainers[itemInfo.Id].Add(frozenDecorator);
                }

                if (!realized)
                {
                    this.Owner.Measure(frozenDecorator, new RadSize(double.PositiveInfinity, double.PositiveInfinity));

                    desiredLength = this.IsHorizontal ? frozenDecorator.DesiredSize.Width : frozenDecorator.DesiredSize.Height;
                }
                else
                {
                    desiredLength = this.IsHorizontal ? frozenDecorator.DesiredSize.Width : frozenDecorator.DesiredSize.Height;
                }

                // Workaround for the optimizations of the measure in cases where desiredsize is 0,0.
                desiredLength = RadMath.AreClose(desiredLength, 0) ? defaultGroupHeaderLength : desiredLength;

                this.frozenDecoratorLengthPerLevel[itemInfo.Level] = desiredLength;
                this.generatedFrozenDecoratorsPerLevel[itemInfo.Level] = frozenDecorator;
            }

            this.frozenDecoratorsToGenerate.Clear();
        }

        internal void ArrangeFrozenDecorators()
        {
            if (!this.EnableFrozenDecorators || this.generatedFrozenContainers.Count == 0)
            {
                this.FrozenContainersLength = 0;
                return;
            }

            var topLeft = new RadPoint(0, 0);

            foreach (var pair in this.generatedFrozenContainers)
            {
                var decorators = pair.Value;
                var frozenDecorator = decorators.Last();
                var frozenDecoratorItemInfo = frozenDecorator.ItemInfo;
                var frozenGroupLevel = frozenDecoratorItemInfo.Level;

                var frozenDecoratorDesiredSize = frozenDecorator.DesiredSize;

                var decoratorPairAfterCurrentFrozenDecorator = this.generatedContainers.FirstOrDefault(kvp => frozenDecoratorItemInfo.Id < kvp.Value.Last().ItemInfo.Id && kvp.Value.Last().ItemInfo.Level <= frozenDecoratorItemInfo.Level);
                List<GeneratedItemModel> notFrozenDecorators = decoratorPairAfterCurrentFrozenDecorator.Value;
                if (notFrozenDecorators != null && notFrozenDecorators.Count > 0)
                {
                    var lastDecorator = notFrozenDecorators.Last();
                    var layoutSlot = lastDecorator.LayoutSlot;
                    if (this.IsHorizontal)
                    {
                        var physicalHorizontalOffset = this.Owner.ScrollOffset;

                        if (layoutSlot.X - physicalHorizontalOffset < topLeft.X + frozenDecoratorDesiredSize.Width)
                        {
                            topLeft.X = layoutSlot.X - physicalHorizontalOffset - frozenDecoratorDesiredSize.Width;
                        }
                    }
                    else
                    {
                        var physicalVertivalOffset = this.Owner.ScrollOffset;
                        if (layoutSlot.Y - physicalVertivalOffset < topLeft.Y + frozenDecoratorDesiredSize.Height)
                        {
                            topLeft.Y = layoutSlot.Y - physicalVertivalOffset - frozenDecoratorDesiredSize.Height;
                        }
                    }
                }

                // TODO: Get the Pixel VerticalOffset and check the size of the first decorator.
                // If it doesn't fit - then we need to offset and clip the decorator.
                double stretchLength = this.IsHorizontal ? this.AvailableSize.Height : this.AvailableSize.Width;
                double length = this.frozenDecoratorLengthPerLevel[frozenGroupLevel];
                var arrangeRect = new RadRect();

                foreach (var decorator in decorators)
                {
                    if (this.IsHorizontal)
                    {
                        arrangeRect = new RadRect(topLeft.X, topLeft.Y, length, stretchLength);
                        this.FrozenContainersLength = arrangeRect.Right;
                    }
                    else
                    {
                        arrangeRect = new RadRect(topLeft.X, topLeft.Y, stretchLength, length);
                        this.FrozenContainersLength = arrangeRect.Bottom;
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
            }
        }

        internal void UpdateFrozenContainerInfos(ItemInfo info)
        {
            this.shouldGenerateFrozenContainer = true;
            if (!this.EnableFrozenDecorators)
            {
                return;
            }

            var itemInfo = info;

            // We take the last frozen line for this level
            var frozenInfosToGenerateByLevel = this.frozenDecoratorsToGenerate.Where(c => c.Level == itemInfo.Level);
            var frozenInfosToGenerateBySlot = frozenInfosToGenerateByLevel.Where(c => c.Slot != itemInfo.Slot);

            if (frozenInfosToGenerateBySlot.Any())
            {
                ItemInfo frozenInfoToGenerate = frozenInfosToGenerateBySlot.First();

                // We take the length of the frozen decorators up to our level.
                double frozenLengthUpToLevel = this.GetFrozenLengthUpToLevel(itemInfo.Level - 1);

                // Take the position of the current decorator (as if not frozen).
                double currentFrozenHeaderOffset = itemInfo.Slot - 1 >= 0 ? this.Layout.PhysicalOffsetFromSlot(itemInfo.Slot - 1) : 0;

                // We should generate another frozen decorator if the current one will push the previous frozen frozen decorator or if we haven't done so.
                bool lastFrozenDecoratorIsHidden = currentFrozenHeaderOffset - this.Owner.ScrollOffset < frozenLengthUpToLevel;

                if (lastFrozenDecoratorIsHidden)
                {
                    this.frozenDecoratorsToGenerate.Remove(itemInfo);
                }

                this.shouldGenerateFrozenContainer = !frozenInfosToGenerateByLevel.Any() || lastFrozenDecoratorIsHidden;
            }

            if (this.shouldGenerateFrozenContainer && itemInfo.ItemType == GroupType.Subheading)
            {
                if (!frozenInfosToGenerateByLevel.Any())
                {
                    this.frozenDecoratorsToGenerate.Add(itemInfo);
                }
                else
                {
                    var index = this.frozenDecoratorsToGenerate.IndexOf(frozenInfosToGenerateByLevel.First());

                    // TODO: This is valid for top frozen containers. Revisit if at some point we need frozen containers at bottom.
                    if (itemInfo.Slot > this.frozenDecoratorsToGenerate[index].Slot)
                    {
                        this.frozenDecoratorsToGenerate[index] = itemInfo;
                    }
                }

                double desiredLength;
                List<GeneratedItemModel> generatedContainersForSlot;

                if (this.generatedContainers.TryGetValue(itemInfo.Slot, out generatedContainersForSlot))
                {
                    var generatedContainer = generatedContainersForSlot.Last();
                    desiredLength = this.IsHorizontal ? generatedContainer.DesiredSize.Width : generatedContainer.DesiredSize.Height;
                }
                else
                {
                    desiredLength = this.Layout.PhysicalLengthForSlot(itemInfo.Slot);
                }

                this.frozenDecoratorLengthPerLevel[itemInfo.Level] = desiredLength;
            }
        }

        private RadSize MeasureVertically(RadSize availableSize, double offset, double bufferScale)
        {
            var context = new MeasureContext()
            {
                Offset = offset,
                BufferScale = bufferScale
            };

            context.InitializeForVerticalMeasure(this.Layout, availableSize);
            this.AvailableLength = context.AvailableLength;

            var initialFirstVisibleIndex = context.FirstVisibleIndex;
            if (this.IsBufferNeeded)
            {
                context.FirstVisibleIndex = this.GenerateTopBufferItems(context.FirstVisibleIndex, context.AvailableLength, context.Offset, context);
                context.LastVisibleIndex = context.FirstVisibleIndex;
            }

            this.MeasureForward(ref context);

            // TODO: Investigate the case with maximizing and incorrect scroll offset.
            if (context.AvailableLength - context.ForwardGeneratedLength > ListViewModel.DoubleArithmetics.Ceiling(1d / IndexStorage.PrecisionMultiplier))
            {
                if (this.viewportItemsCount < this.Layout.VisibleLineCount)
                {
                    // perform the measure logic again, since this difference will most probably occur when the Index tree is cleared before the measure pass
                    this.RecycleLocally();
                    context.InitializeForVerticalMeasure(this.Layout, availableSize);

                    if (this.IsBufferNeeded)
                    {
                        context.FirstVisibleIndex = this.GenerateTopBufferItems(context.FirstVisibleIndex, context.AvailableLength, context.Offset, context);
                        context.LastVisibleIndex = context.FirstVisibleIndex;
                    }

                    this.MeasureForward(ref context);
                }
            }

            this.MeasureBackwards(ref context);

            if (this.IsBufferNeeded)
            {
                this.GenerateBottomBufferItems(context.LastVisibleIndex, context.AvailableLength, context);
            }

            var desiredSize = new RadSize(context.MaxLength, context.GeneratedLength);
            desiredSize.Width = Math.Min(desiredSize.Width, ListViewModel.DoubleArithmetics.Ceiling(availableSize.Width));
            desiredSize.Height = Math.Min(desiredSize.Height, context.AvailableLength);

            this.Layout.UpdateAverageLength(context.FirstVisibleIndex, context.LastVisibleIndex);
            if (Math.Floor(context.Index) > initialFirstVisibleIndex)
            {
                (this.Owner as RadListView).updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
            }
            return desiredSize;
        }

        private RadSize MeasureHorizontally(RadSize availableSize, double offset, double bufferScale)
        {
            // TODO: add buffer here
            var context = new MeasureContext()
            {
                Offset = offset,
                BufferScale = bufferScale
            };

            context.InitializeForHorizontalMeasure(this.Layout, availableSize);

            this.AvailableLength = context.AvailableLength;

            if (this.IsBufferNeeded)
            {
                context.FirstVisibleIndex = this.GenerateTopBufferItems(context.FirstVisibleIndex, context.AvailableLength, context.Offset, context);
                context.LastVisibleIndex = context.FirstVisibleIndex;
            }

            this.MeasureForward(ref context);

            // TODO: Investigate the case with maximizing and incorrect scroll offset.
            if (context.AvailableLength - context.ForwardGeneratedLength > ListViewModel.DoubleArithmetics.Ceiling(1d / IndexStorage.PrecisionMultiplier))
            {
                if (this.viewportItemsCount < this.Layout.VisibleLineCount)
                {
                    // perform the measure logic again, since this difference will most probably occur when the Index tree is cleared before the measure pass
                    this.RecycleLocally();
                    context.InitializeForHorizontalMeasure(this.Layout, availableSize);

                    if (this.IsBufferNeeded)
                    {
                        context.FirstVisibleIndex = this.GenerateTopBufferItems(context.FirstVisibleIndex, context.AvailableLength, context.Offset, context);
                        context.LastVisibleIndex = context.FirstVisibleIndex;
                    }

                    this.MeasureForward(ref context);
                }
            }

            this.MeasureBackwards(ref context);

            if (this.IsBufferNeeded)
            {
                this.GenerateBottomBufferItems(context.LastVisibleIndex, context.AvailableLength, context);
            }

            var desiredSize = this.ComputeDesiredSize(context);
            this.Layout.UpdateAverageLength(context.FirstVisibleIndex, context.LastVisibleIndex);

            return desiredSize;
        }

        private GeneratedItemModel GetPreviouslyVisibleFrozenDecorator(int id)
        {
            GeneratedItemModel decorator;
            if (this.recycledFrozenDecorators.TryGetValue(id, out decorator))
            {
                this.recycledFrozenDecorators.Remove(id);
            }

            return decorator;
        }

        private double GetFrozenLengthUpToLevel(int level)
        {
            double frozenLengthUpToLevel = 0;
            while (level >= 0)
            {
                double frozenLengthForLevel;
                this.frozenDecoratorLengthPerLevel.TryGetValue(level--, out frozenLengthForLevel);
                frozenLengthUpToLevel += frozenLengthForLevel;
            }

            return frozenLengthUpToLevel;
        }

        private void UpdateLayoutSlotsLength(double length)
        {
            for (int i = 0; i < this.Layout.TotalSlotCount; i++)
            {
                this.Layout.UpdateSlotLength(i, length);
            }
        }

        private void MeasureForward(ref MeasureContext context)
        {
            var firstContainerDesiredSize = new RadSize();
            var lastContainerDesiredSize = new RadSize();

            var lengthContext = this.GeneratedLengthContext;

            using (var itemInfos = this.Layout.GetLines(context.FirstVisibleIndex, true).GetEnumerator())
            {
                if (itemInfos.MoveNext())
                {
                    firstContainerDesiredSize = this.GenerateContainer(itemInfos.Current, context);
                    context.ForwardGeneratedLength += lengthContext.GenerateLength(context.GetLength(firstContainerDesiredSize));

                    context.MaxLength = Math.Max(context.MaxLength, context.GetOppositeLength(firstContainerDesiredSize));

                    // Take the length portion of the indexed element that is not currently in viewport.
                    this.HiddenPixels = ListViewModel.DoubleArithmetics.Floor(context.GetLength(firstContainerDesiredSize) * (context.Index % 1));
                    context.ForwardGeneratedLength -= this.HiddenPixels;

                    context.ForwardGeneratedLength = ListViewModel.DoubleArithmetics.Ceiling(context.ForwardGeneratedLength);
                    lastContainerDesiredSize = firstContainerDesiredSize;

                    this.viewportItemsCount++;
                }

                while (ListViewModel.DoubleArithmetics.IsLessThan(context.ForwardGeneratedLength, context.AvailableLength) && itemInfos.MoveNext())
                {
                    context.LastVisibleIndex++;
                    lastContainerDesiredSize = this.GenerateContainer(itemInfos.Current, context);
                    context.ForwardGeneratedLength += lengthContext.GenerateLength(context.GetLength(lastContainerDesiredSize));
                    context.MaxLength = Math.Max(context.MaxLength, context.GetOppositeLength(lastContainerDesiredSize));

                    this.viewportItemsCount++;
                }
            }
        }

        private void MeasureBackwards(ref MeasureContext context)
        {
            if (context.AvailableLength - context.ForwardGeneratedLength > ListViewModel.DoubleArithmetics.Ceiling(1d / IndexStorage.PrecisionMultiplier))
            {
                var firstContainerDesiredSize = new RadSize();

                var lengthGenerationContext = this.GeneratedLengthContext;

                using (var itemInfos = this.Layout.GetLines(context.FirstVisibleIndex - 1, false).GetEnumerator())
                {
                    while (ListViewModel.DoubleArithmetics.IsLessThan(context.GeneratedLength, context.AvailableLength) && itemInfos.MoveNext())
                    {
                        context.FirstVisibleIndex--;
                        firstContainerDesiredSize = this.GenerateContainer(itemInfos.Current, context);
                        context.BackwardGeneratedLength += lengthGenerationContext.GenerateLength(context.GetLength(firstContainerDesiredSize));

                        context.MaxLength = Math.Max(context.MaxLength, context.GetOppositeLength(firstContainerDesiredSize));

                        this.viewportItemsCount++;
                    }
                }
            }
        }

        private int GenerateTopBufferItems(int startGenerateIndex, double viewportLength, double startOffset, MeasureContext context)
        {
            if (startGenerateIndex == 0)
            {
                return startGenerateIndex;
            }

            var buffer = viewportLength * context.BufferScale;
            var startBufferOffset = Math.Max(0, startOffset - buffer);
            var startIndex = Math.Min((int)this.Layout.SlotFromPhysicalOffset(startBufferOffset), startGenerateIndex);

            if (startIndex > 0)
            {
                startIndex -= 1;
            }

            var endIndex = startIndex - 1;

            var generatedLengthContext = this.GeneratedLengthContext;

            using (var itemInfos = this.Layout.GetLines(startIndex, true).GetEnumerator())
            {
                var generatedLength = 0.0;
                while (ListViewModel.DoubleArithmetics.IsLessThan(generatedLength, buffer) && itemInfos.MoveNext())
                {
                    var size = this.GenerateContainer(itemInfos.Current, context);
                    generatedLength += generatedLengthContext.GenerateLength(context.GetLength(size));
                    endIndex++;
                }
            }

            return endIndex == startIndex - 1 ? startGenerateIndex : endIndex + 1;
        }

        private int GenerateBottomBufferItems(int startGenerateIndex, double viewportLength, MeasureContext context)
        {
            var buffer = viewportLength * context.BufferScale;
            var startIndex = startGenerateIndex + 1;
            var lastGeneratedIndex = startIndex;

            var generatedLengthContext = this.GeneratedLengthContext;

            using (var itemInfos = this.Layout.GetLines(startIndex, true).GetEnumerator())
            {
                var generatedLength = 0.0;
                var generatedLengthForward = context.ForwardGeneratedLength;
                while (ListViewModel.DoubleArithmetics.IsLessThan(generatedLength - generatedLengthForward, buffer) && itemInfos.MoveNext())
                {
                    var size = this.GenerateContainer(itemInfos.Current, context);
                    
                    generatedLength += generatedLengthContext.GenerateLength(context.GetLength(size));

                    lastGeneratedIndex++;
                }
            }

            return lastGeneratedIndex == startIndex ? startGenerateIndex : lastGeneratedIndex + 1;
        }

        private void Recycle(GeneratedItemModel decorator)
        {
            this.Generator.RecycleDecorator(decorator);
        }

        internal struct MeasureContext
        {
            public double OppositeAvailableLength;

            public double AvailableLength;
            public double Index;
            public int FirstVisibleIndex;
            public int LastVisibleIndex;
            public double ForwardGeneratedLength;
            public double BackwardGeneratedLength;
            public double MaxLength;
            public double Offset;
            public double BufferScale;

            private bool isHorizontal;

            public double GeneratedLength
            {
                get
                {
                    return this.ForwardGeneratedLength + this.BackwardGeneratedLength;
                }
            }

            public double GetLength(RadSize size)
            {
                return this.isHorizontal ? size.Width : size.Height;
            }

            public double GetOppositeLength(RadSize size)
            {
                return this.isHorizontal ? size.Height : size.Width;
            }

            public void InitializeForHorizontalMeasure(BaseLayout layout, RadSize size, MeasureContext previousContext)
            {
                this.AvailableLength = Math.Floor(ListViewModel.DoubleArithmetics.Ceiling(size.Width));
                this.OppositeAvailableLength = Math.Floor(ListViewModel.DoubleArithmetics.Ceiling(size.Height));
                this.isHorizontal = true;
                this.InitializeCore(layout, previousContext);
            }

            public void InitializeForHorizontalMeasure(BaseLayout layout, RadSize size)
            {
                this.AvailableLength = Math.Floor(ListViewModel.DoubleArithmetics.Ceiling(size.Width));
                this.OppositeAvailableLength = Math.Floor(ListViewModel.DoubleArithmetics.Ceiling(size.Height));
                this.isHorizontal = true;
                this.InitializeCore(layout);
            }

            public void InitializeForVerticalMeasure(BaseLayout layout, RadSize size)
            {
                this.AvailableLength = Math.Floor(ListViewModel.DoubleArithmetics.Ceiling(size.Height));
                this.OppositeAvailableLength = Math.Floor(ListViewModel.DoubleArithmetics.Ceiling(size.Width));
                this.InitializeCore(layout);
            }

            private void InitializeCore(BaseLayout layout)
            {
                this.Index = layout.SlotFromPhysicalOffset(this.Offset);
                this.FirstVisibleIndex = Math.Max(0, Math.Min(layout.VisibleLineCount, (int)this.Index));
                this.LastVisibleIndex = this.FirstVisibleIndex;
            }

            private void InitializeCore(BaseLayout layout, MeasureContext previousContext)
            {
                this.Index = layout.SlotFromPhysicalOffset(this.Offset + previousContext.GeneratedLength);
                this.FirstVisibleIndex = Math.Max(previousContext.LastVisibleIndex - 1, Math.Min(layout.VisibleLineCount, (int)this.Index));
                this.LastVisibleIndex = this.FirstVisibleIndex;
                this.MaxLength = previousContext.MaxLength;

                this.ForwardGeneratedLength = previousContext.ForwardGeneratedLength;
                this.BackwardGeneratedLength = previousContext.BackwardGeneratedLength;
            }
        }
    }
}