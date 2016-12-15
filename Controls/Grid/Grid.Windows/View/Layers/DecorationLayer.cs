using Telerik.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    internal abstract class DecorationLayer : SharedUILayer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecorationLayer" /> class.
        /// </summary>
        protected DecorationLayer()
        {
        }

        internal bool IsAlternateRow(ItemInfo info)
        {
            if (this.Owner == null)
            {
                return false;
            }

            if (this.Owner.AlternationStep == 0 || info.IsCollapsible)
            {
                return false;
            }

            int rowIndex = info.LayoutInfo.ChildLine - this.Owner.AlternationStartIndex;
            if (rowIndex < 0)
            {
                return false;
            }

            return rowIndex % this.Owner.AlternationStep == 0;
        }

        internal bool HasHorizontalLine(ItemInfo info)
        {
            if (this.Owner == null)
            {
                return false;
            }

            if (info.IsCollapsible)
            {
                return false;
            }

            if (info.LayoutInfo.ChildLine == 0 && this.Owner.Model.RowPool.Layout.TotalLineCount > 1)
            {
                return false;
            }

            return this.Owner.HasHorizontalGridLines;
        }

        internal bool IsFirstLine(ItemInfo info)
        {
            return info.LayoutInfo.ChildLine == 0 && this.Owner.Model.RowPool.Layout.TotalLineCount > 1;
        }

        internal bool IsLastLine(ItemInfo info)
        {
            return info.LayoutInfo.Line == this.Owner.Model.RowPool.Layout.VisibleLineCount - 1;
        }

        internal virtual void MakeVisible(LineDecorationModel element)
        {
            // TODO: pass only the UI representation rather than the model
            UIElement container = element.Container as UIElement;
            if (container != null)
            {
                container.ClearValue(UIElement.VisibilityProperty);
            }
        }

        internal virtual void Collapse(LineDecorationModel element)
        {
            // TODO: pass only the UI representation rather than the model
            UIElement container = element.Container as UIElement;
            if (container != null)
            {
                container.Visibility = Visibility.Collapsed;
            }
        }

        internal virtual bool HasDecorations(DecorationType decoratorElementType, ItemInfo itemInfo)
        {
            if (this.Owner == null)
            {
                return false;
            }

            switch (decoratorElementType)
            {
                case DecorationType.Row:
                    if (this.HasRowFill(itemInfo))
                    {
                        return true;
                    }

                    if (this.HasHorizontalLine(itemInfo))
                    {
                        return true;
                    }

                    return this.Owner.HasHorizontalGridLines && this.IsLastLine(itemInfo);
                case DecorationType.Column:
                    // no need of column decoration for the first line as the border is handled by the root Grid's Border
                    if (itemInfo.LayoutInfo.Line == 0)
                    {
                        return false;
                    }

                    return (this.Owner.GridLinesVisibility & GridLinesVisibility.Vertical) == GridLinesVisibility.Vertical;
            }

            return false;
        }

        internal virtual object GenerateContainerForLineDecorator()
        {
            return null;
        }

        internal abstract void ApplyLineDecoratorProperties(LineDecorationModel lineDecorator, DecorationType decoratorElementType);

        internal abstract RadRect ArrangeLineDecorator(LineDecorationModel decorator, RadRect radRect);

        internal virtual void PrepareCellDecoration(GridCellModel cell)
        {
        }

        internal virtual void ArrangeCellDecoration(GridCellModel cell)
        {
        }

        private bool HasRowFill(ItemInfo info)
        {
            if (this.Owner.RowBackground != null)
            {
                return true;
            }

            if (this.IsAlternateRow(info))
            {
                return true;
            }

            if (this.Owner.RowBackgroundSelector != null && !info.IsCollapsible)
            {
                return true;
            }

            return false;
        }
    }
}
