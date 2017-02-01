using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Core;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class SelectionRegionController
    {
        private IDecorationPresenter<SelectionRegionModel> decoratorPresenter;
        private DecorationPool<SelectionRegionModel> selectionRegionsPool;

        public SelectionRegionController(IDecorationPresenter<SelectionRegionModel> decoratorPresenter)
        {
            this.decoratorPresenter = decoratorPresenter;
            this.selectionRegionsPool = new DecorationPool<SelectionRegionModel>(decoratorPresenter);
        }

        /// <summary>
        /// Gets the selection regions pool. Exposed for testing purposes.
        /// </summary>
        internal DecorationPool<SelectionRegionModel> SelectionRegionsPool
        {
            get
            {
                return this.selectionRegionsPool;
            }
        }

        internal void Arrange(ArrangeDataForDecorations horizontalArrangement, ArrangeDataForDecorations verticalArrangement, IEnumerable<SelectionRegionInfo> selectionRegions)
        {
            this.selectionRegionsPool.RecycleAll();

            foreach (var region in selectionRegions)
            {
                var rect = GetArrangeRect(region, horizontalArrangement, verticalArrangement);

                if (rect.IsSizeValid())
                {
                    var visualContainer = this.CreateDecorator(region);

                    this.decoratorPresenter.ArrangeDecorator(visualContainer, rect);
                }
            }

            this.selectionRegionsPool.FullyRecycleElements();
        }

        private static RadRect GetArrangeRect(SelectionRegionInfo region, ArrangeDataForDecorations horizontalArrangement, ArrangeDataForDecorations verticalArrangement)
        {
            double left = -1;
            double right = -2;
            double top = -1;
            double bottom = -2;

            var startRowLayout = region.StartItem.RowItemInfo.LayoutInfo;
            var endRowLayout = region.EndItem.RowItemInfo.LayoutInfo;

            var invalidInfo = new LayoutInfo { Line = -1 };

            LayoutInfo startColumnLayout = region.StartItem.Column != null ? region.StartItem.Column.ItemInfo.LayoutInfo : invalidInfo;
            LayoutInfo endColumnLayout = region.EndItem.Column != null ? region.EndItem.Column.ItemInfo.LayoutInfo : invalidInfo;

            if (IsInsideArrangement(verticalArrangement, startRowLayout, endRowLayout))
            {
                if (!verticalArrangement.TryGetStartOfSlot(startRowLayout.Line, out top) && verticalArrangement.SlotsStartLine >= startRowLayout.Line)
                {
                    top = verticalArrangement.SlotsStart;
                }

                if (!verticalArrangement.TryGetEndOfSlot(endRowLayout.Line + endRowLayout.LineSpan - 1, out bottom) && verticalArrangement.SlotsEndLine <= endRowLayout.Line)
                {
                    bottom = verticalArrangement.SlotsEnd;
                }

                if (startColumnLayout.Equals(invalidInfo) || endColumnLayout.Equals(invalidInfo) || IsInsideArrangement(horizontalArrangement, startColumnLayout, endColumnLayout))
                {
                    if (!horizontalArrangement.TryGetStartOfSlot(startColumnLayout.Line, out left) && (horizontalArrangement.SlotsStartLine >= startColumnLayout.Line || startColumnLayout.Equals(invalidInfo)))
                    {
                        left = horizontalArrangement.SlotsStart;
                    }

                    if (!horizontalArrangement.TryGetEndOfSlot(endColumnLayout.Line + endColumnLayout.LineSpan - 1, out right) && (horizontalArrangement.SlotsEndLine <= endColumnLayout.Line || endColumnLayout.Equals(invalidInfo)))
                    {
                        right = horizontalArrangement.SlotsEnd;
                    }
                }
            }

            return new RadRect(left, top, right - left, bottom - top);
        }

        private static bool IsInsideArrangement(ArrangeDataForDecorations verticalArrangement, LayoutInfo startLayout, LayoutInfo endLayout)
        {
            return (verticalArrangement.SlotsStartLine <= endLayout.Line && endLayout.Line <= verticalArrangement.SlotsEndLine) || 
                (verticalArrangement.SlotsStartLine <= startLayout.Line && startLayout.Line <= verticalArrangement.SlotsEndLine) ||
                (verticalArrangement.SlotsStartLine >= startLayout.Line && verticalArrangement.SlotsEndLine <= endLayout.Line);
        }

        private SelectionRegionModel CreateDecorator(SelectionRegionInfo region)
        {
            var decorator = this.selectionRegionsPool.GetRecycledElement();
            if (decorator == null)
            {
                decorator = new SelectionRegionModel();
                decorator.Container = this.decoratorPresenter.GenerateContainerForDecorator();
            }

            decorator.SelectionInfo = region;

            this.decoratorPresenter.ApplyDecoratorProperties(decorator, DecorationType.Row);

            this.selectionRegionsPool.AddToDisplayedElements(decorator);

            return decorator;
        }
    }
}
