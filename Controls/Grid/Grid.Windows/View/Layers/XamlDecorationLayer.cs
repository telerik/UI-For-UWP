using System;
using Telerik.Core;
using Telerik.Data.Core.Layouts;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    internal class XamlDecorationLayer : DecorationLayer
    {
        internal override object GenerateContainerForLineDecorator()
        {
            var border = new Border();
            this.AddVisualChild(border);

            return border;
        }

        internal override void ApplyLineDecoratorProperties(LineDecorationModel lineDecorator, DecorationType decorationType)
        {
            var border = lineDecorator.Container as Border;
            if (border == null)
            {
                return;
            }

            switch (decorationType)
            {
                case DecorationType.Row:
                    var brush = this.GetRowFill(lineDecorator.ItemInfo);
                    if (brush != null)
                    {
                        border.Background = brush;
                    }
                    else
                    {
                        border.ClearValue(Border.BackgroundProperty);
                    }

                    bool horizontal = this.HasHorizontalLine(lineDecorator.ItemInfo);
                    bool last = this.IsLastLine(lineDecorator.ItemInfo);

                    if (horizontal || last)
                    {
                        border.BorderBrush = this.Owner.GridLinesBrush;
                        double topThickness = (horizontal && lineDecorator.ItemInfo.LayoutInfo.Line > 0) ? this.Owner.GridLinesThickness : 0;
                        double bottomThickness = last ? this.Owner.GridLinesThickness : 0;
                        border.BorderThickness = new Thickness(0, topThickness, 0, bottomThickness);
                    }
                    else
                    {
                        border.BorderThickness = new Thickness(0);
                    }
                    Canvas.SetZIndex(border, ZIndexConstants.HorizontalGridLineBaseZIndex);
                    break;

                case DecorationType.Column:
                    border.BorderBrush = this.Owner.GridLinesBrush;
                    border.BorderThickness = new Thickness(this.Owner.GridLinesThickness, 0, 0, 0);
                    Canvas.SetZIndex(border, ZIndexConstants.VerticalGridLineBaseZIndex + lineDecorator.ItemInfo.LayoutInfo.Line);
                    break;
            }
        }

        internal override RadRect ArrangeLineDecorator(LineDecorationModel decorator, RadRect radRect)
        {
            var border = decorator.Container as Border;
            if (border == null)
            {
                return radRect;
            }

            var arrangeRect = new RadRect(radRect.X, radRect.Y, radRect.Width - radRect.X, radRect.Height - radRect.Y);

            if (decorator.ItemInfo.Slot == this.Owner.Model.RowPool.Layout.VisibleLineCount - 1 || decorator.ItemInfo.Slot == this.Owner.FrozenColumnCount - 1)
            {
                // Render the last line outside the last row layout slot.
                arrangeRect.Height += this.Owner.GridLinesThickness;
            }

            Canvas.SetLeft(border, arrangeRect.X);
            Canvas.SetTop(border, arrangeRect.Y);
            border.Arrange(arrangeRect.ToRect());

            return arrangeRect;
        }

        internal override void PrepareCellDecoration(GridCellModel cell)
        {
            bool hasDecorationContainer = cell.DecorationContainer != null;

            cell.Column.PrepareCellDecoration(cell);

            // check whether we have a new container created by the Column and add it as VisualChild.
            if (cell.DecorationContainer != null && !hasDecorationContainer)
            {
                var uiElement = cell.DecorationContainer as UIElement;
                if (uiElement != null)
                {
                    this.AddVisualChild(uiElement);
                    Canvas.SetZIndex(uiElement, ZIndexConstants.CellDecorationsBaseZIndex);
                }
            }
        }

        internal override void ArrangeCellDecoration(GridCellModel cell)
        {
            var decorationContainer = cell.DecorationContainer as FrameworkElement;
            if (decorationContainer == null)
            {
                return;
            }

            RadRect layoutSlot = cell.layoutSlot;

            var rect = this.Owner.InflateCellHorizontally(cell, layoutSlot);
            rect = this.Owner.InflateCellVertically(cell, rect);

            Canvas.SetLeft(decorationContainer, rect.X);
            Canvas.SetTop(decorationContainer, rect.Y);
            decorationContainer.Width = rect.Width;
            decorationContainer.Height = rect.Height;
        }

        private Brush GetRowFill(ItemInfo info)
        {
            if (this.IsAlternateRow(info))
            {
                return this.Owner.AlternateRowBackground;
            }

            if (this.Owner.RowBackground != null)
            {
                return this.Owner.RowBackground;
            }

            if (this.Owner.RowBackgroundSelector != null && !info.IsCollapsible)
            {
                return this.Owner.RowBackgroundSelector.SelectObject(info.Item, this.Owner);
            }

            return null;
        }
    }
}
