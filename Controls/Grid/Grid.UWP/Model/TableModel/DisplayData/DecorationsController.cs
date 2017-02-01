using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Telerik.Core;
using Telerik.Data.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Grid;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class DecorationsController
    {
        private Dictionary<int, LineDecorationModel> displayedRowDecorationsMap;
        private Dictionary<int, LineDecorationModel> displayedColumnDecorationsMap;
        private DecorationPool<LineDecorationModel> rowLinesElementsPool;
        private DecorationPool<LineDecorationModel> columnLinesElementsPool;
        private IDecorationPresenter<LineDecorationModel> decorationPresenter;

        public DecorationsController(IDecorationPresenter<LineDecorationModel> decoratorPresenter)
        {
            this.decorationPresenter = decoratorPresenter;
            this.rowLinesElementsPool = new DecorationPool<LineDecorationModel>(decoratorPresenter);
            this.columnLinesElementsPool = new DecorationPool<LineDecorationModel>(decoratorPresenter);

            this.displayedRowDecorationsMap = new Dictionary<int, LineDecorationModel>();
            this.displayedColumnDecorationsMap = new Dictionary<int, LineDecorationModel>();
        }

        /// <summary>
        /// Gets displayed row decorations map. Exposed for testing purposes.
        /// </summary>
        internal Dictionary<int, LineDecorationModel> DisplayedRowDecorationsMap
        {
            get
            {
                return this.displayedRowDecorationsMap;
            }
        }

        /// <summary>
        /// Gets displayed column decorations map. Exposed for testing purposes.
        /// </summary>
        internal Dictionary<int, LineDecorationModel> DisplayedColumnDecorationsMap
        {
            get
            {
                return this.displayedColumnDecorationsMap;
            }
        }

        internal void Arrange(ArrangeDataForDecorations horizontalArrangement, ArrangeDataForDecorations verticalArrangement)
        {
            this.ArrangeRowDecorations(horizontalArrangement, verticalArrangement);
            this.ArrangeColumnDecorations(horizontalArrangement, verticalArrangement);

            this.rowLinesElementsPool.FullyRecycleElements();
            this.columnLinesElementsPool.FullyRecycleElements();
        }

        internal void Update(UpdateFlags flags)
        {
            if ((flags & UpdateFlags.AffectsDecorations) == UpdateFlags.AffectsDecorations)
            {
                this.RecycleAllDecorators();
            }
        }

        internal void RecycleColumnDecorators(int slot)
        {
            RecycleDecorator(slot, this.columnLinesElementsPool, this.displayedColumnDecorationsMap);
        }

        internal void RecycleRowDecorators(int slot)
        {
            RecycleDecorator(slot, this.rowLinesElementsPool, this.displayedRowDecorationsMap);
        }

        internal void RecycleRowDecoratorBefore(int line)
        {
            foreach (var item in this.displayedRowDecorationsMap.ToList())
            {
                if (item.Key < line)
                {
                    this.RecycleRowDecorator(item.Key);
                }
            }
        }

        internal void RecycleRowDecoratorAfter(int line)
        {
            foreach (var item in this.displayedRowDecorationsMap.ToList())
            {
                if (item.Key > line)
                {
                    this.RecycleRowDecorator(item.Key);
                }
            }
        }

        internal void RecycleAllRowDecorators()
        {
            foreach (var item in this.displayedRowDecorationsMap.ToList())
            {
                this.RecycleRowDecorator(item.Key);
            }
        }

        internal void RecycleColumnDecoratorBefore(int line)
        {
            foreach (var item in this.displayedColumnDecorationsMap.ToList())
            {
                if (item.Key < line)
                {
                    this.RecycleColumnDecorator(item.Key);
                }
            }
        }

        internal void RecycleColumnDecoratorAfter(int line)
        {
            foreach (var item in this.displayedColumnDecorationsMap.ToList())
            {
                if (item.Key > line)
                {
                    this.RecycleColumnDecorator(item.Key);
                }
            }
        }

        internal void RecycleColumnDecoratorBetween(int startLine, int endLine)
        {
            foreach (var item in this.displayedColumnDecorationsMap.ToList())
            {
                if (startLine < item.Key && item.Key < endLine)
                {
                    this.RecycleColumnDecorator(item.Key);
                }
            }
        }

        internal void RecycleAllColumnDecorators()
        {
            foreach (var item in this.displayedColumnDecorationsMap.ToList())
            {
                this.RecycleColumnDecorator(item.Key);
            }
        }

        internal void GenerateRowDecorators(IEnumerable<ItemInfo> displayedRowInfos)
        {
            // Generate row decorators
            foreach (var itemInfo in displayedRowInfos)
            {
                this.GenerateRowDecorator(itemInfo);
            }
        }

        internal void GenerateColumnDecorators(IEnumerable<ItemInfo> displayedColumnInfos)
        {
            // Generate column decorators
            foreach (var itemInfos in displayedColumnInfos)
            {
                this.GenerateColumnDecorator(itemInfos);
            }
        }

        private static void RecycleDecorator(int id, DecorationPool<LineDecorationModel> displayData, Dictionary<int, LineDecorationModel> decorators)
        {
            LineDecorationModel decorator;
            if (decorators.TryGetValue(id, out decorator))
            {
                displayData.Recycle(decorator);
                decorators.Remove(id);
            }
        }

        private static void GenerateDecorator(int id, ItemInfo itemInfo, DecorationType decoratorElementType, DecorationPool<LineDecorationModel> decorationPool, Dictionary<int, LineDecorationModel> decorators, IDecorationPresenter<LineDecorationModel> owner)
        {
            LineDecorationModel decorator = null;
            if (owner != null && !decorators.TryGetValue(id, out decorator))
            {
                decorator = CreateDecorator(itemInfo, decoratorElementType, decorationPool, owner);
                decorators.Add(id, decorator);
            }
            else if (owner != null)
            {
                decorator.ItemInfo = itemInfo;
                owner.ApplyDecoratorProperties(decorator, decoratorElementType);
            }
        }

        private static LineDecorationModel CreateDecorator(ItemInfo itemInfo, DecorationType decoratorElementType, DecorationPool<LineDecorationModel> displayData, IDecorationPresenter<LineDecorationModel> owner)
        {
            if (owner == null)
            {
                return null;
            }

            var lineDecorator = displayData.GetRecycledElement();
            if (lineDecorator == null)
            {
                lineDecorator = new LineDecorationModel();
                lineDecorator.Container = owner.GenerateContainerForDecorator();
            }

            lineDecorator.ItemInfo = itemInfo;

            owner.ApplyDecoratorProperties(lineDecorator, decoratorElementType);

            displayData.AddToDisplayedElements(lineDecorator);

            return lineDecorator;
        }

        private static DecorationType GetDecoratorType(bool isRow)
        {
            return isRow ? DecorationType.Row : DecorationType.Column;
        }

        private void RecycleRowDecorator(int line)
        {
            RecycleDecorator(line, this.rowLinesElementsPool, this.displayedRowDecorationsMap);
        }

        private void RecycleColumnDecorator(int line)
        {
            RecycleDecorator(line, this.columnLinesElementsPool, this.displayedColumnDecorationsMap);
        }

        private void ArrangeColumnDecorations(ArrangeDataForDecorations horizontalArrangement, ArrangeDataForDecorations verticalArrangement)
        {
            double top;
            double bottom;
            double left;
            double right;

            foreach (var columnDecoratorPair in this.displayedColumnDecorationsMap)
            {
                LineDecorationModel decorator = columnDecoratorPair.Value;
                LayoutInfo layoutInfo = decorator.ItemInfo.LayoutInfo;

                top = verticalArrangement.GetStartOfLevel(layoutInfo.Level);
                bottom = layoutInfo.SpansThroughCells ? verticalArrangement.SlotsEnd : verticalArrangement.GetEndOfLevel(layoutInfo.Level + layoutInfo.LevelSpan - 1);
                left = horizontalArrangement.GetStartOfSlot(layoutInfo.Line);
                right = horizontalArrangement.GetEndOfSlot(layoutInfo.Line + layoutInfo.LineSpan - 1);

                var thickness = this.decorationPresenter.ArrangeDecorator(decorator, new RadRect(left, top, right, bottom));
            }
        }

        private void ArrangeRowDecorations(ArrangeDataForDecorations horizontalArrangement, ArrangeDataForDecorations verticalArrangement)
        {
            double top;
            double bottom;
            double left;
            double right;

            foreach (var rowDecoratorPair in this.displayedRowDecorationsMap)
            {
                LineDecorationModel decorator = rowDecoratorPair.Value;
                LayoutInfo layoutInfo = decorator.ItemInfo.LayoutInfo;

                top = verticalArrangement.GetStartOfSlot(layoutInfo.Line);
                bottom = verticalArrangement.GetEndOfSlot(layoutInfo.Line + layoutInfo.LineSpan - 1);
                left = horizontalArrangement.GetStartOfLevel(layoutInfo.Level);
                right = layoutInfo.SpansThroughCells ? horizontalArrangement.SlotsEnd : horizontalArrangement.GetEndOfLevel(layoutInfo.Level + layoutInfo.LevelSpan - 1);

                var thickness = this.decorationPresenter.ArrangeDecorator(decorator, new RadRect(left, top, right, bottom));
            }
        }

        private void RecycleRowDecoratorForGroup(int line)
        {
            RecycleDecorator(line, this.rowLinesElementsPool, this.displayedRowDecorationsMap);
        }

        private void RecycleColumnDecoratorForGroup(int line)
        {
            RecycleDecorator(line, this.columnLinesElementsPool, this.displayedColumnDecorationsMap);
        }

        private void GenerateRowDecorator(ItemInfo itemInfo)
        {
            var decoratorElementType = GetDecoratorType(true);
            bool hasDecoration = this.decorationPresenter.HasDecorations(decoratorElementType, itemInfo);
            if (hasDecoration)
            {
                this.GenerateRowDecoratorForGroup(itemInfo, decoratorElementType);
            }
            else
            {
                this.RecycleRowDecoratorForGroup(itemInfo.Id);
            }
        }

        private void GenerateColumnDecorator(ItemInfo itemInfo)
        {
            var decoratorElementType = GetDecoratorType(false);
            bool hasDecoration = this.decorationPresenter.HasDecorations(decoratorElementType, itemInfo);
            if (hasDecoration)
            {
                this.GenerateColumnDecoratorForGroup(itemInfo, decoratorElementType);
            }
            else
            {
                this.RecycleColumnDecoratorForGroup(itemInfo.Id);
            }
        }

        private void GenerateRowDecoratorForGroup(ItemInfo itemInfo, DecorationType decoratorElementType)
        {
            GenerateDecorator(itemInfo.Id, itemInfo, decoratorElementType, this.rowLinesElementsPool, this.displayedRowDecorationsMap, this.decorationPresenter);
        }

        private void GenerateColumnDecoratorForGroup(ItemInfo itemInfo, DecorationType decoratorElementType)
        {
            GenerateDecorator(itemInfo.Id, itemInfo, decoratorElementType, this.columnLinesElementsPool, this.displayedColumnDecorationsMap, this.decorationPresenter);
        }

        private void RecycleAllDecorators()
        {
            this.rowLinesElementsPool.RecycleAll();
            this.columnLinesElementsPool.RecycleAll();

            this.displayedRowDecorationsMap.Clear();
            this.displayedColumnDecorationsMap.Clear();
        }
    }
}