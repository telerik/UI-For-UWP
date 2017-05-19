using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Telerik.Core;
using Telerik.Data.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Grid.Model;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal abstract class NodePool<T, K> : INodePool<T>
        where T : Node, IItemInfoNode
        where K : IGenerationContext
    {
        internal double FrozenContainersLength;

        private readonly ITable owner;
        private readonly ItemModelGenerator<T, K> generator;
        private readonly BaseLayout layout;

        private bool preparingContainer;
        private bool shouldGenerateFrozenContainer;
        private bool isHorizontal;

        // This is changed to SortedDictionary since the GetGroupSizes method always orders it by the key
        private SortedDictionary<int, double> columnWidth = new SortedDictionary<int, double>();
        private Dictionary<int, T> recycledDecorators = new Dictionary<int, T>();

        private Dictionary<int, RadRect> lastValidState = new Dictionary<int, RadRect>();

        // TODO: Consider changing all the sorted dictionaries with a simple list, which is about three times faster
        private IDictionary<int, List<T>> generatedContainers = new SortedDictionary<int, List<T>>();

        private Dictionary<int, T> recycledFrozenDecorators = new Dictionary<int, T>();
        private IDictionary<int, List<T>> generatedFrozenContainers = new SortedDictionary<int, List<T>>();

        private Dictionary<int, T> generatedFrozenDecoratorsPerLevel = new Dictionary<int, T>();
        private Dictionary<int, double> frozenDecoratorLengthPerLevel = new Dictionary<int, double>();
        private IList<ItemInfo> frozenDecoratorsToGenerate = new List<ItemInfo>();

        public NodePool(ITable tableBase, ItemModelGenerator<T, K> generator, BaseLayout layout, bool isHorizontal)
        {
            this.isHorizontal = isHorizontal;
            this.owner = tableBase;
            this.generator = generator;

            this.layout = layout;
            this.layout.Collapsed += this.OnCollapsed;
            this.layout.Expanded += this.OnExpanded;
            this.layout.ItemsSourceChanged += this.OnItemsSourceChanged;

            this.IsBufferNeeded = true;

            this.Initialize();
        }

        public IRenderInfo RenderInfo
        {
            get { return this.Layout.RenderInfo; }
        }

        public ITable OwnerTable
        {
            get
            {
                return this.owner;
            }
        }

        public ItemModelGenerator<T, K> ModelGenerator
        {
            get
            {
                return this.generator;
            }
        }

        public bool EnableFrozenDecorators
        {
            get;
            set;
        }

        public double AvailableLength
        {
            get;
            private set;
        }

        public BaseLayout Layout
        {
            get
            {
                return this.layout;
            }
        }

        public double HiddenPixels
        {
            get;
            private set;
        }

        public int DisplayedElementCount
        {
            get
            {
                return this.generatedContainers.Count;
            }
        }

        public int ViewportItemCount
        {
            get;
            private set;
        }

        internal bool IsBufferNeeded
        {
            get;
            set;
        }

        internal IEnumerable<T> FrozenContainers
        {
            get
            {
                return this.generatedFrozenDecoratorsPerLevel.Values;
            }
        }

        protected virtual bool SupportMultipleContainersForSlot
        {
            get
            {
                return false;
            }
        }

        public void RefreshRenderInfo(double defaultValue)
        {
            var loadState = this.Layout.GetRenderLoadState();
            this.RenderInfo.ResetToDefaultValues(loadState, defaultValue);
        }

        public RadRect GetPreviousDisplayedLayoutSlot(int slot)
        {
            RadRect rect;

            if (this.lastValidState.TryGetValue(slot, out rect))
            {
                return rect;
            }

            return RadRect.Invalid;
        }

        public T GetDisplayedElement(int slot)
        {
            List<T> containers;
            if (this.generatedContainers.TryGetValue(slot, out containers))
            {
                return containers.LastOrDefault();
            }

            return null;
        }

        public IEnumerable<KeyValuePair<int, List<T>>> GetDisplayedElements()
        {
            foreach (var item in this.generatedContainers)
            {
                yield return item;
            }
        }

        public IEnumerable<KeyValuePair<int, List<T>>> GetUnfrozenDisplayedElements()
        {
            foreach (var item in this.generatedContainers)
            {
                foreach (var pair in item.Value)
                {
                    if (!pair.IsFrozen)
                    {
                        yield return item;
                    }

                    break;
                }
            }
        }

        public IEnumerable<KeyValuePair<int, List<T>>> GetFrozenDisplayedElements()
        {
            foreach (var item in this.generatedContainers)
            {
                foreach (var pair in item.Value)
                {
                    if (pair.IsFrozen)
                    {
                        yield return item;
                    }

                    break;
                }
            }
        }

        public bool IsItemCollapsed(int cellSlot)
        {
            return this.Layout.IsItemCollapsed(cellSlot);
        }

        internal abstract T GenerateAndPrepareContainer(ref ItemInfo itemInfo);

        internal abstract T GenerateAndPrepareFrozenContainer(ref ItemInfo itemInfo);

        internal IEnumerable<double> GetGroupSizes()
        {
            foreach (var pair in this.columnWidth)
            {
                yield return pair.Value;
            }
        }

        internal void Initialize()
        {
            this.columnWidth.Clear();
        }

        internal IEnumerable<ItemInfo> GetDisplayedItems()
        {
            foreach (var item in this.generatedContainers)
            {
                foreach (T element in item.Value)
                {
                    yield return element.ItemInfo;
                }
            }
        }

        internal IEnumerable<ItemInfo> GetFrozenDisplayedItems()
        {
            foreach (var item in this.generatedContainers)
            {
                foreach (T element in item.Value)
                {
                    if (element.IsFrozen)
                    {
                        yield return element.ItemInfo;
                    }
                }
            }
        }

        internal IEnumerable<ItemInfo> GetItemsToRecycle()
        {
            foreach (var item in this.recycledDecorators)
            {
                yield return item.Value.ItemInfo;
            }
        }

        internal void RecycleAfterMeasure()
        {
            this.RecycleUnused();
            this.generator.FullyRecycleDecorators();
        }

        internal RadSize OnMeasure(RadSize availableSize, double offset, int frozenElementsCount, double verticalBufferScale)
        {
            this.RecycleLocally();

            // Give valid AvailableSize - e.g. Not Infinity.
            int visibleItemCount = this.layout.VisibleLineCount;
            if (visibleItemCount == 0)
            {
                this.Initialize();
                this.generator.FullyRecycleDecorators();
                return new RadSize();
            }

            RadSize size;

            if (this.isHorizontal)
            {
                size = this.MeasureHorizontally(availableSize, offset, frozenElementsCount);
            }
            else
            {
                size = this.MeasureVertically(availableSize, offset, verticalBufferScale);
            }

            this.lastValidState.Clear();
            foreach (var item in this.generatedContainers.Values.SelectMany(c => c).Select(c => new { Slot = c.ItemInfo.Slot, LayoutRect = c.LayoutSlot }))
            {
                this.lastValidState[item.Slot] = item.LayoutRect;
            }

            return size;
        }

        internal RadSize OnArrange(RadSize finalSize)
        {
            if (!this.isHorizontal)
            {
                // The CellsController will handle rows and cells arrange
                //  this.ArrangeFrozenDecorators();
                return finalSize;
            }

            bool initialized = false;
            var topLeft = new RadPoint(0, 0);
            int elementSequenceNumber = 0;

            foreach (var pair in this.GetDisplayedElements())
            {
                var decorators = pair.Value;
                RadRect arrangeRect = new RadRect();
                double length = this.GetElementArrangeLength(pair.Key);

                foreach (var decorator in decorators)
                {
                    if (!initialized || elementSequenceNumber != decorator.ItemInfo.Slot)
                    {
                        initialized = true;
                        topLeft.X = decorator.ItemInfo.Slot != 0 ? this.RenderInfo.OffsetFromIndex(decorator.ItemInfo.Slot - 1) : 0;
                        elementSequenceNumber = decorator.ItemInfo.Slot;
                    }

                    int level = decorator.ItemInfo.LayoutInfo.Level;
                    int indent = 0;

                    double indentOffset = indent * 20;

                    double offset = 0;
                    int temp = level;
                    while (temp > 0)
                    {
                        offset += this.columnWidth[--temp];
                    }

                    // the code is executed for Horizontal pool
                    arrangeRect = new RadRect(topLeft.X, topLeft.Y + offset + indentOffset, length, this.columnWidth[level] - indentOffset);

                    decorator.layoutSlot = arrangeRect;
                    this.owner.Arrange(decorator);
                }

                topLeft.X += arrangeRect.Width;
                elementSequenceNumber++;
            }

            return finalSize;
        }

        internal void ArrangeFrozenDecorators()
        {
            if (!this.EnableFrozenDecorators || this.generatedFrozenContainers.Count == 0)
            {
                this.FrozenContainersLength = 0;
                return;
            }

            var topLeft = this.isHorizontal ? new RadPoint(0, -this.owner.PhysicalVerticalOffset) : new RadPoint(0, 0);

            foreach (var pair in this.generatedFrozenContainers)
            {
                var decorators = pair.Value;
                var frozenDecorator = decorators.Last();
                var frozenDecoratorItemInfo = frozenDecorator.ItemInfo;
                var frozenGroupLevel = frozenDecoratorItemInfo.Level;

                var frozenDecoratorDesiredSize = frozenDecorator.DesiredSize;

                var decoratorPairAfterCurrentFrozenDecorator = this.generatedContainers.FirstOrDefault(kvp => frozenDecoratorItemInfo.Id < kvp.Value.Last().ItemInfo.Id && kvp.Value.Last().ItemInfo.Level <= frozenDecoratorItemInfo.Level);
                List<T> notFrozenDecorators = decoratorPairAfterCurrentFrozenDecorator.Value;
                if (notFrozenDecorators != null && notFrozenDecorators.Count > 0)
                {
                    var lastDecorator = notFrozenDecorators.Last();
                    var layoutSlot = lastDecorator.LayoutSlot;
                    if (this.isHorizontal)
                    {
                        var physicalHorizontalOffset = this.owner.PhysicalHorizontalOffset;

                        if (layoutSlot.X - physicalHorizontalOffset < topLeft.X + frozenDecoratorDesiredSize.Width)
                        {
                            topLeft.X = layoutSlot.X - physicalHorizontalOffset - frozenDecoratorDesiredSize.Width;
                        }
                    }
                    else
                    {
                        var physicalVertivalOffset = this.owner.PhysicalVerticalOffset;

                        if (layoutSlot.Y - physicalVertivalOffset < topLeft.Y + frozenDecoratorDesiredSize.Height)
                        {
                            topLeft.Y = layoutSlot.Y - physicalVertivalOffset - frozenDecoratorDesiredSize.Height;
                        }
                    }
                }

                // TODO: Get the Pixel VerticalOffset and check the size of the first decorator.
                // If it doesn't fit - then we need to offset and clip the decorator.
                double length = this.GetElementArrangeLength(pair.Key);
                var arrangeRect = new RadRect();

                foreach (var decorator in decorators)
                {
                    int level = decorator.ItemInfo.LayoutInfo.Level;
                    int indent = 0;

                    double indentOffset = indent * 20;

                    double offset = 0;
                    int temp = level;
                    while (temp > 0)
                    {
                        offset += this.columnWidth[--temp];
                    }

                    if (this.isHorizontal)
                    {
                        arrangeRect = new RadRect(topLeft.X, topLeft.Y + offset + indentOffset, length, this.columnWidth[level] - indentOffset);
                        this.FrozenContainersLength = arrangeRect.Right;
                    }
                    else
                    {
                        arrangeRect = new RadRect(topLeft.X + offset + indentOffset, topLeft.Y, this.columnWidth[level] - indentOffset, length);
                        this.FrozenContainersLength = arrangeRect.Bottom;
                    }

                    decorator.layoutSlot = arrangeRect;
                    this.owner.Arrange(decorator);
                }

                if (this.isHorizontal)
                {
                    topLeft.X += arrangeRect.Width;
                }
                else
                {
                    topLeft.Y += arrangeRect.Height;
                }
            }
        }

        internal void OnGroupExpandStateChanged(object item, bool expanded)
        {
            if (this.preparingContainer)
            {
                return;
            }

            if (expanded)
            {
                this.layout.Expand(item);
            }
            else
            {
                this.layout.Collapse(item);
            }
        }

        internal void Update(UpdateFlags flags)
        {
            if ((flags & UpdateFlags.AffectsContent) == UpdateFlags.AffectsContent)
            {
                this.FullyRecycle();
            }

            if ((flags & UpdateFlags.AffectsColumnsWidth) == UpdateFlags.AffectsColumnsWidth)
            {
                this.columnWidth.Clear();
            }
        }

        private double GetElementArrangeLength(int slot)
        {
            if (this.isHorizontal)
            {
                return this.owner.GetWidthForLine(slot);
            }
            else
            {
                return this.owner.GetHeightForLine(slot);
            }
        }

        private double GenerateCellsForLine(int slot, double largestLength, T lastElement)
        {
            if (this.isHorizontal)
            {
                return this.owner.GenerateCellsForColumn(slot, largestLength, lastElement);
            }
            else
            {
                return this.owner.GenerateCellsForRow(slot, largestLength, lastElement);
            }
        }

        private void RecycleUnused()
        {
            foreach (var decorator in this.recycledDecorators)
            {
                this.Recycle(decorator.Value);
            }

            this.recycledDecorators.Clear();

            foreach (var decorator in this.recycledFrozenDecorators)
            {
                this.Recycle(decorator.Value);
            }

            this.recycledFrozenDecorators.Clear();
        }

        private void OnItemsSourceChanged(object sender, EventArgs e)
        {
            this.FullyRecycle();
        }

        private void OnExpanded(object sender, Telerik.Data.Core.Layouts.ExpandCollapseEventArgs e)
        {
            // NOTE: Consider removing this code for better performance once we use more complex IndexTree with nested IndexTrees for every Group.
            // This way we won't need to update the index tree on Expand/Collapse operation and we won't have to use GetNextVisible method.
            int end = e.StartSlot + e.SlotsCount;
            for (int i = e.StartSlot; i < end; i = this.layout.GetNextVisibleSlot(i))
            {
                this.Layout.RenderInfo.Update(i, this.isHorizontal ? GridModel.DefaultColumnWidth : GridModel.DefaultRowHeight);
            }

            // Gets the length up to (including) the index of expanded item.
            var physicalOffset = this.Layout.RenderInfo.ValueForIndex(e.StartSlot - 1);

            this.owner.InvalidateCellsPanelMeasure();
        }

        private void OnCollapsed(object sender, Telerik.Data.Core.Layouts.ExpandCollapseEventArgs e)
        {
            // NOTE: Consider removing this code for better performance once we use more complex IndexTree with nested IndexTrees for every Group.
            // This way we won't need to update the index tree on Expand/Collapse.
            int end = e.StartSlot + e.SlotsCount;

            for (int i = e.StartSlot; i < end; i++)
            {
                this.Layout.RenderInfo.Update(i, 0);
            }

            this.owner.InvalidateCellsPanelMeasure();
        }

        private RadSize GenerateContainer(IList<ItemInfo> itemInfos)
        {
            int slot = itemInfos.Last().Slot;

            List<T> decorators;
            if (!this.generatedContainers.TryGetValue(slot, out decorators))
            {
                decorators = new List<T>();
                this.generatedContainers[slot] = decorators;
            }
            else if (!this.SupportMultipleContainersForSlot)
            {
                return RadSize.Empty;
            }

            for (int i = 0; i < itemInfos.Count; i++)
            {
                var itemInfo = itemInfos[i];

                try
                {
                    if (itemInfo.IsDisplayed || itemInfo.IsSummaryVisible)
                    {
                        this.preparingContainer = true;

                        T decorator = this.GetPreviouslyVisibleDecorator(itemInfo.Id);

                        // Recycle cells on this slot if container is Collasped/Expanded.
                        if (decorator != null && (decorator.ItemInfo.IsCollapsed != itemInfo.IsCollapsed))
                        {
                            this.Recycle(decorator);
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
                finally
                {
                    this.preparingContainer = false;
                }
            }

            T lastElement = null;
            double largestLength = 0;
            foreach (var decorator in decorators)
            {
                int level = decorator.ItemInfo.LayoutInfo.Level;
                int indent = decorator.ItemInfo.LayoutInfo.Indent;
                double indentOffset = 0;

                var desiredSize = this.owner.Measure(decorator);

                double length;
                if (this.isHorizontal)
                {
                    double roundHeight = desiredSize.Height + indentOffset;
                    this.UpdateRowOrColumnLength(level, roundHeight);
                    length = desiredSize.Width;
                }
                else
                {
                    double roundWidth = desiredSize.Width + indentOffset;
                    this.UpdateRowOrColumnLength(level, roundWidth);
                    length = desiredSize.Height;
                }

                lastElement = decorator;
                largestLength = Math.Max(largestLength, length);
            }

            double height = this.GenerateCellsForLine(slot, largestLength, lastElement);

            double width = 0;
            for (int i = 0; i < this.columnWidth.Count; i++)
            {
                width += this.columnWidth[i];
            }

            try
            {
                this.preparingContainer = true;

                for (int i = 0; i < itemInfos.Count; i++)
                {
                    this.UpdateFrozenContainerInfos(itemInfos[i]);
                }
            }
            finally
            {
                this.preparingContainer = false;
            }

            if (this.isHorizontal)
            {
                return new RadSize(height, width);
            }
            else
            {
                return new RadSize(width, height);
            }
        }

        private T GetPreviouslyVisibleDecorator(int id)
        {
            T decorator;
            if (this.recycledDecorators.TryGetValue(id, out decorator))
            {
                this.recycledDecorators.Remove(id);
            }

            return decorator;
        }

        private void UpdateRowOrColumnLength(int level, double width)
        {
            double currentWidth;
            if (!this.columnWidth.TryGetValue(level, out currentWidth))
            {
                currentWidth = 0;
            }

            if (GridModel.DoubleArithmetics.IsLessThanOrEqual(currentWidth, width))
            {
                this.columnWidth[level] = width;
            }
        }

        private void RecycleLocally()
        {
            this.ViewportItemCount = 0;
            this.HiddenPixels = 0;

            foreach (var pair in this.generatedFrozenContainers)
            {
                foreach (var decorator in pair.Value)
                {
                    this.recycledFrozenDecorators.Add(decorator.ItemInfo.Id, decorator);
                }
            }

            this.generatedFrozenContainers.Clear();
            foreach (var pair in this.generatedContainers)
            {
                foreach (var decorator in pair.Value)
                {
                    this.recycledDecorators.Add(decorator.ItemInfo.Id, decorator);
                }
            }

            this.generatedContainers.Clear();
            this.generatedFrozenDecoratorsPerLevel.Clear();
            this.frozenDecoratorLengthPerLevel.Clear();
        }

        private void Recycle(T decorator)
        {
            this.generator.RecycleDecorator(decorator as T);

            if (!this.generatedContainers.ContainsKey(decorator.ItemInfo.Slot))
            {
                if (this.isHorizontal)
                {
                    this.owner.RecycleColumn(decorator.ItemInfo);
                }
                else
                {
                    this.owner.RecycleRow(decorator.ItemInfo);
                }
            }
        }

        private void FullyRecycle()
        {
            foreach (var decorators in this.generatedContainers)
            {
                foreach (var decorator in decorators.Value)
                {
                    this.Recycle(decorator);
                }
            }

            this.generatedContainers.Clear();

            foreach (var decorators in this.generatedFrozenContainers)
            {
                foreach (var decorator in decorators.Value)
                {
                    this.Recycle(decorator);
                }
            }

            this.generatedFrozenContainers.Clear();
        }

        private void GenerateFrozenContainers()
        {
            if (!this.EnableFrozenDecorators)
            {
                return;
            }

            for (int i = 0; i < this.frozenDecoratorsToGenerate.Count; i++)
            {
                bool realized = false;
                var itemInfo = this.frozenDecoratorsToGenerate[i];
                double desiredLength = 0;

                // Get list with frozen decorators per slot (created if empty);
                var frozenDecorators = this.GetCurrentlyVisibleFrozenDecorators(itemInfo.Slot);
                var frozenDecorator = this.GetPreviouslyVisibleFrozenDecorator(itemInfo.Id);

                realized = frozenDecorator != null;

                // Recycle cells on this slot if container is Collasped/Expanded.
                if (realized && (frozenDecorator.ItemInfo.IsCollapsed != itemInfo.IsCollapsed))
                {
                    this.Recycle(frozenDecorator);
                    realized = false;
                }

                if (!realized)
                {
                    frozenDecorator = this.GenerateAndPrepareFrozenContainer(ref itemInfo);
                }

                frozenDecorators.Add(frozenDecorator);

                if (!realized)
                {
                    this.owner.Measure(frozenDecorator);

                    desiredLength = this.isHorizontal ? frozenDecorator.DesiredSize.Width : frozenDecorator.DesiredSize.Height;
                    double length = this.GenerateCellsForLine(itemInfo.Slot, desiredLength, frozenDecorator);
                }
                else
                {
                    desiredLength = this.isHorizontal ? frozenDecorator.DesiredSize.Width : frozenDecorator.DesiredSize.Height;
                }

                this.frozenDecoratorLengthPerLevel[itemInfo.Level] = desiredLength;
                this.generatedFrozenDecoratorsPerLevel[itemInfo.Level] = frozenDecorator;
            }

            this.frozenDecoratorsToGenerate.Clear();
        }

        private void UpdateFrozenContainerInfos(ItemInfo info)
        {
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
                double currentFrozenHeaderOffset = itemInfo.Slot - 1 >= 0 ? this.Layout.RenderInfo.OffsetFromIndex(itemInfo.Slot - 1) : 0;

                // We should generate another frozen decorator if the current one will push the previous frozen frozen decorator or if we haven't done so.
                bool lastFrozenDecoratorIsHidden = currentFrozenHeaderOffset - this.owner.PhysicalVerticalOffset < frozenLengthUpToLevel;

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
                List<T> generatedContainersForSlot;

                if (this.generatedContainers.TryGetValue(itemInfo.Slot, out generatedContainersForSlot))
                {
                    var generatedContainer = generatedContainersForSlot.Last();
                    desiredLength = this.isHorizontal ? generatedContainer.DesiredSize.Width : generatedContainer.DesiredSize.Height;
                }
                else
                {
                    desiredLength = this.Layout.RenderInfo.ValueForIndex(itemInfo.Slot);
                }

                this.frozenDecoratorLengthPerLevel[itemInfo.Level] = desiredLength;
            }
        }

        private List<T> GetCurrentlyVisibleFrozenDecorators(int slot)
        {
            List<T> decorators;
            if (!this.generatedFrozenContainers.TryGetValue(slot, out decorators))
            {
                decorators = new List<T>();
                this.generatedFrozenContainers[slot] = decorators;
            }

            return decorators;
        }

        private T GetPreviouslyVisibleFrozenDecorator(int id)
        {
            T decorator;
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

        private int GenerateTopBufferItems(int startGenerateIndex, double viewportLength, double startOffset, MeasureContext context)
        {
            if (startGenerateIndex == 0)
            {
                return startGenerateIndex;
            }
            
            var buffer = this.isHorizontal ? 0.0 : viewportLength * context.BufferScale;
            var startBufferOffset = Math.Max(0, startOffset - buffer);
            var startIndex = Math.Min((int)this.Layout.IndexFromPhysicalOffset(startBufferOffset), startGenerateIndex);

            if (startIndex > 0)
            {
                startIndex -= 1;
            }

            var endIndex = startIndex - 1;

            using (var itemInfos = this.layout.GetLines(startIndex, true).GetEnumerator())
            {
                var generatedLength = 0.0;
                while (GridModel.DoubleArithmetics.IsLessThan(generatedLength, buffer) && itemInfos.MoveNext())
                {
                    var size = this.GenerateContainer(itemInfos.Current);
                    generatedLength += this.isHorizontal ? size.Width : size.Height;

                    // endIndex = itemInfos.Current.Last().Slot;
                    endIndex++;
                }
            }

            return endIndex == startIndex - 1 ? startGenerateIndex : endIndex + 1;
        }

        private int GenerateBottomBufferItems(int startGenerateIndex, double viewportLength, MeasureContext context)
        {
            var buffer = this.isHorizontal ? 0.0 : viewportLength * context.BufferScale;
            var startIndex = startGenerateIndex + 1;
            var lastGeneratedIndex = startIndex;

            using (var itemInfos = this.layout.GetLines(startIndex, true).GetEnumerator())
            {
                var generatedLength = 0.0;
                while (GridModel.DoubleArithmetics.IsLessThan(generatedLength, buffer) && itemInfos.MoveNext())
                {
                    var size = this.GenerateContainer(itemInfos.Current);
                    generatedLength += this.isHorizontal ? size.Width : size.Height;

                    lastGeneratedIndex++;
                }
            }

            return lastGeneratedIndex == startIndex ? startGenerateIndex : lastGeneratedIndex + 1;
        }

        private RadSize MeasureHorizontally(RadSize availableSize, double offset, int frozenElementsCount)
        {
            var context = new MeasureContext()
            {
                Offset = offset
            };

            if (frozenElementsCount > 0)
            {
                var frozenGeneratedContext = this.MeasureHorizontalFrozenElements(availableSize, frozenElementsCount);
                context.InitializeForHorizontalMeasure(this, availableSize, frozenGeneratedContext);
            }
            else
            {
                context.InitializeForHorizontalMeasure(this, availableSize);
            }

            this.AvailableLength = context.AvailableLength;
            this.MeasureForward(ref context);

            if (context.AvailableLength - context.ForwardGeneratedLength > GridModel.DoubleArithmetics.Ceiling(1d / IndexStorage.PrecisionMultiplier))
            {
                if (context.FirstVisibleIndex + this.ViewportItemCount < this.layout.VisibleLineCount)
                {
                    // perform the measure logic again, since this difference will most probably occur when the Index tree is cleared before the measure pass
                    this.RecycleLocally();
                    context.InitializeForHorizontalMeasure(this, availableSize);
                    this.MeasureForward(ref context);
                }
            }

            context.FirstVisibleIndex -= this.owner.FrozenColumnCount;
            this.MeasureBackwards(ref context);

            var desiredSize = new RadSize(context.GeneratedLength, context.MaxLength);
            desiredSize.Width = Math.Min(desiredSize.Width, context.AvailableLength);
            desiredSize.Height = Math.Min(desiredSize.Height, GridModel.DoubleArithmetics.Ceiling(availableSize.Height));

            this.Layout.UpdateAverageLength(context.FirstVisibleIndex, context.LastVisibleIndex);

            return desiredSize;
        }

        private MeasureContext MeasureHorizontalFrozenElements(RadSize availableSize, int frozenContainersCount)
        {
            var context = new MeasureContext();

            context.InitializeForHorizontalMeasure(this, availableSize);

            if (frozenContainersCount == 0)
            {
                return context;
            }

            var firstContainerDesiredSize = new RadSize();
            var lastContainerDesiredSize = new RadSize();

            using (var itemInfos = this.layout.GetLines(context.FirstVisibleIndex, true).GetEnumerator())
            {
                if (itemInfos.MoveNext())
                {
                    firstContainerDesiredSize = this.GenerateContainer(itemInfos.Current);
                    context.ForwardGeneratedLength = context.GetLength(firstContainerDesiredSize);
                    context.MaxLength = Math.Max(context.MaxLength, context.GetOppositeLength(firstContainerDesiredSize));

                    // Take the length portion of the indexed element that is not currently in viewport.
                    this.HiddenPixels = GridModel.DoubleArithmetics.Floor(context.ForwardGeneratedLength * (context.Index % 1));
                    context.ForwardGeneratedLength -= this.HiddenPixels;

                    context.ForwardGeneratedLength = GridModel.DoubleArithmetics.Ceiling(context.ForwardGeneratedLength);
                    lastContainerDesiredSize = firstContainerDesiredSize;

                    this.ViewportItemCount++;
                }

                while (frozenContainersCount - 1 > context.LastVisibleIndex && GridModel.DoubleArithmetics.IsLessThan(context.ForwardGeneratedLength, context.AvailableLength) && itemInfos.MoveNext())
                {
                    context.LastVisibleIndex++;
                    lastContainerDesiredSize = this.GenerateContainer(itemInfos.Current);
                    context.ForwardGeneratedLength += context.GetLength(lastContainerDesiredSize);
                    context.MaxLength = Math.Max(context.MaxLength, context.GetOppositeLength(lastContainerDesiredSize));

                    this.ViewportItemCount++;
                }
            }

            return context;
        }

        private RadSize MeasureVertically(RadSize availableSize, double offset, double bufferScale)
        {
            var context = new MeasureContext()
            {
                Offset = offset,
                BufferScale = bufferScale
            };
            context.InitializeForVerticalMeasure(this, availableSize);

            this.AvailableLength = context.AvailableLength;

            // Always start with frozen containers unitl we generate row with cells.
            this.shouldGenerateFrozenContainer = true;

            if (this.IsBufferNeeded)
            {
                context.FirstVisibleIndex = this.GenerateTopBufferItems(context.FirstVisibleIndex, context.AvailableLength, context.Offset, context);
                context.LastVisibleIndex = context.FirstVisibleIndex;
            }

            this.MeasureForward(ref context);

            ////TODO: We may need an additional synchronous measure pass in case less generated length (as in the Horizontal measure)

            this.MeasureBackwards(ref context);

            if (this.IsBufferNeeded)
            {
                this.GenerateBottomBufferItems(context.LastVisibleIndex, context.AvailableLength, context);
            }

            this.GenerateFrozenContainers();

            var desiredSize = new RadSize(context.MaxLength, context.GeneratedLength);
            desiredSize.Width = Math.Min(desiredSize.Width, GridModel.DoubleArithmetics.Ceiling(availableSize.Width));
            desiredSize.Height = Math.Min(desiredSize.Height, context.AvailableLength);

            this.Layout.UpdateAverageLength(context.FirstVisibleIndex, context.LastVisibleIndex);

            return desiredSize;
        }

        private void MeasureForward(ref MeasureContext context)
        {
            var firstContainerDesiredSize = new RadSize();
            var lastContainerDesiredSize = new RadSize();

            using (var itemInfos = this.layout.GetLines(context.FirstVisibleIndex, true).GetEnumerator())
            {
                if (itemInfos.MoveNext())
                {
                    firstContainerDesiredSize = this.GenerateContainer(itemInfos.Current);
                    context.ForwardGeneratedLength += context.GetLength(firstContainerDesiredSize);
                    context.MaxLength = Math.Max(context.MaxLength, context.GetOppositeLength(firstContainerDesiredSize));

                    // Take the length portion of the indexed element that is not currently in viewport.
                    this.HiddenPixels = GridModel.DoubleArithmetics.Floor(context.GetLength(firstContainerDesiredSize) * (context.Index % 1));
                    context.ForwardGeneratedLength -= this.HiddenPixels;

                    context.ForwardGeneratedLength = GridModel.DoubleArithmetics.Ceiling(context.ForwardGeneratedLength);
                    lastContainerDesiredSize = firstContainerDesiredSize;

                    this.ViewportItemCount++;
                }

                while (GridModel.DoubleArithmetics.IsLessThan(context.ForwardGeneratedLength, context.AvailableLength) && itemInfos.MoveNext())
                {
                    context.LastVisibleIndex++;
                    lastContainerDesiredSize = this.GenerateContainer(itemInfos.Current);
                    context.ForwardGeneratedLength += context.GetLength(lastContainerDesiredSize);
                    context.MaxLength = Math.Max(context.MaxLength, context.GetOppositeLength(lastContainerDesiredSize));

                    this.ViewportItemCount++;
                }
            }
        }

        private void MeasureBackwards(ref MeasureContext context)
        {
            if (context.AvailableLength - context.ForwardGeneratedLength > GridModel.DoubleArithmetics.Ceiling(1d / IndexStorage.PrecisionMultiplier))
            {
                if (context.FirstVisibleIndex + this.ViewportItemCount < this.layout.VisibleLineCount)
                {
                    var firstContainerDesiredSize = new RadSize();
                    using (var itemInfos = this.layout.GetLines(context.FirstVisibleIndex - 1, false).GetEnumerator())
                    {
                        while (GridModel.DoubleArithmetics.IsLessThan(context.GeneratedLength, context.AvailableLength) && itemInfos.MoveNext())
                        {
                            context.FirstVisibleIndex--;
                            firstContainerDesiredSize = this.GenerateContainer(itemInfos.Current);
                            context.BackwardGeneratedLength += context.GetLength(firstContainerDesiredSize);
                            context.MaxLength = Math.Max(context.MaxLength, context.GetOppositeLength(firstContainerDesiredSize));

                            this.ViewportItemCount++;
                        }
                    }
                }
            }
        }

        private struct MeasureContext
        {
            public double AvailableLength;
            public double Index;
            public int FirstVisibleIndex;
            public int LastVisibleIndex;
            public double ForwardGeneratedLength;
            public double BackwardGeneratedLength;
            public double MaxLength;
            public double Offset;

            private bool isHorizontal;

            public double GeneratedLength
            {
                get
                {
                    return this.ForwardGeneratedLength + this.BackwardGeneratedLength;
                }
            }

            public double BufferScale { get; internal set; }

            public double GetLength(RadSize size)
            {
                return this.isHorizontal ? size.Width : size.Height;
            }

            public double GetOppositeLength(RadSize size)
            {
                return this.isHorizontal ? size.Height : size.Width;
            }

            public void InitializeForHorizontalMeasure(NodePool<T, K> pool, RadSize size, MeasureContext previousContext)
            {
                this.AvailableLength = Math.Floor(GridModel.DoubleArithmetics.Ceiling(size.Width));
                this.isHorizontal = true;
                this.InitializeCore(pool, previousContext);
            }

            public void InitializeForHorizontalMeasure(NodePool<T, K> pool, RadSize size)
            {
                this.AvailableLength = Math.Floor(GridModel.DoubleArithmetics.Ceiling(size.Width));
                this.isHorizontal = true;
                this.InitializeCore(pool);
            }

            public void InitializeForVerticalMeasure(NodePool<T, K> pool, RadSize size)
            {
                this.AvailableLength = Math.Floor(GridModel.DoubleArithmetics.Ceiling(size.Height));
                this.InitializeCore(pool);
            }

            private void InitializeCore(NodePool<T, K> pool)
            {
                this.Index = pool.layout.IndexFromPhysicalOffset(this.Offset);
                this.FirstVisibleIndex = Math.Max(0, Math.Min(pool.layout.VisibleLineCount, (int)this.Index));
                this.LastVisibleIndex = this.FirstVisibleIndex;
            }

            private void InitializeCore(NodePool<T, K> pool, MeasureContext previousContext)
            {
                this.Index = pool.layout.IndexFromPhysicalOffset(this.Offset + previousContext.GeneratedLength);
                this.FirstVisibleIndex = Math.Max(previousContext.LastVisibleIndex - 1, Math.Min(pool.layout.VisibleLineCount, (int)this.Index));
                this.LastVisibleIndex = this.FirstVisibleIndex;
                this.MaxLength = previousContext.MaxLength;

                this.ForwardGeneratedLength = previousContext.ForwardGeneratedLength;
                this.BackwardGeneratedLength = previousContext.BackwardGeneratedLength;
            }
        }
    }
}
