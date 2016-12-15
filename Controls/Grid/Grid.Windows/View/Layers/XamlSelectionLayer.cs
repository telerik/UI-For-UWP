using System;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    internal class XamlSelectionLayer : SelectionLayer
    {
        internal override object GenerateContainerForDecorator()
        {
            var tupple = new Tuple<FrameworkElement, FrameworkElement>(
                new SelectionRegionBackgroundControl(),
                new SelectionRegionBorderControl());

            this.AddVisualChild(tupple.Item1);
            this.AddVisualChild(tupple.Item2);

            return tupple;
        }

        internal override void MakeVisible(GridElement element)
        {
            var pair = element.Container as Tuple<FrameworkElement, FrameworkElement>;
            if (pair.Item1 != null)
            {
                pair.Item1.ClearValue(UIElement.VisibilityProperty);
            }

            if (pair.Item2 != null)
            {
                pair.Item2.ClearValue(UIElement.VisibilityProperty);
            }
        }

        internal override void Collapse(GridElement element)
        {
            var pair = element.Container as Tuple<FrameworkElement, FrameworkElement>;
            if (pair.Item1 != null)
            {
                pair.Item1.Visibility = Visibility.Collapsed;
            }

            if (pair.Item2 != null)
            {
                pair.Item2.Visibility = Visibility.Collapsed;
            }
        }

        internal override void ApplyDecoratorProperties(SelectionRegionModel decorator, DecorationType decorationType)
        {
            var pair = decorator.Container as Tuple<FrameworkElement, FrameworkElement>;

            if (pair.Item1 != null)
            {
                Canvas.SetZIndex(pair.Item1, ZIndexConstants.SelectionControlBackgroundBaseZIndex);
            }

            if (pair.Item2 != null)
            {
                Canvas.SetZIndex(pair.Item2, ZIndexConstants.SelectionControlBorderBaseZIndex);
            }
        }

        internal override RadRect ArrangeDecorator(SelectionRegionModel decorator, RadRect radRect)
        {
            var pair = decorator.Container as Tuple<FrameworkElement, FrameworkElement>;

            var backgroundControl = pair.Item1 as SelectionRegionBackgroundControl;
            var borderControl = pair.Item2 as SelectionRegionBorderControl;

            if (borderControl == null || backgroundControl == null)
            {
                return radRect;
            }

            double horizontalLineThickness = 0;

            if ((this.Owner.GridLinesVisibility & GridLinesVisibility.Horizontal) == GridLinesVisibility.Horizontal)
            {
                int nextItemId = decorator.SelectionInfo.EndItem.RowItemInfo.Id + 1;
                var nextItem = this.Owner.Model.RowPool.GetDisplayedElement(nextItemId);
                if (nextItem != null && !nextItem.ItemInfo.IsCollapsible)
                {
                    horizontalLineThickness = this.Owner.GridLinesThickness;
                }
            }

            var left = radRect.X;
            var top = radRect.Y;

            var arrangeRect = new Rect(left, top, radRect.Width, radRect.Height + horizontalLineThickness);

            Canvas.SetLeft(borderControl, arrangeRect.X);
            Canvas.SetTop(borderControl, arrangeRect.Y);

            borderControl.Width = arrangeRect.Width;
            borderControl.Height = arrangeRect.Height;

            Canvas.SetLeft(backgroundControl, arrangeRect.X);
            Canvas.SetTop(backgroundControl, arrangeRect.Y);

            backgroundControl.Width = arrangeRect.Width;
            backgroundControl.Height = arrangeRect.Height;

            return arrangeRect.ToRadRect();
        }
    }
}
