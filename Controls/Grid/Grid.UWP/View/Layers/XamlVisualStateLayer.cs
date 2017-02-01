using System;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    internal class XamlVisualStateLayer : VisualStateLayer
    {
        private RadRect hoverNodeSlot;
        private RadRect currentNodeSlot;

        private FrameworkElement hoverVisual;
        private FrameworkElement currencyVisual;

        internal FrameworkElement HoverVisual
        {
            get
            {
                return this.hoverVisual;
            }
        }

        internal FrameworkElement CurrencyVisual
        {
            get
            {
                return this.currencyVisual;
            }
        }

        /// <summary>
        /// Exposed for testing purposes.
        /// </summary>
        internal override void UpdateHoverDecoration(RadRect slot)
        {
            this.hoverNodeSlot = slot;
            this.EnsureHoverVisual();

            if (this.hoverNodeSlot.IsSizeValid())
            {
                this.ArrangeHoverVisual(this.hoverNodeSlot);
            }
            else
            {
                this.ClearHoverState();
            }
        }

        internal override void UpdateCurrencyDecoration(RadRect slot)
        {
            this.currentNodeSlot = slot;
            this.EnsureCurrencyVisual();

            if (slot.IsSizeValid())
            {
                this.ArrangeCurrencyVisual(slot);
            }
            else
            {
                this.ClearCurrencyState();
            }
        }

        internal override void UpdateVisuals()
        {
            this.UpdateCurrencyDecoration(this.currentNodeSlot);
            this.UpdateHoverDecoration(this.hoverNodeSlot);
        }

        private void ClearHoverState()
        {
            this.hoverVisual.Visibility = Visibility.Collapsed;
        }

        private void EnsureHoverVisual()
        {
            if (this.hoverVisual == null)
            {
                this.hoverVisual = new DataGridHoverControl();

                this.AddVisualChild(this.hoverVisual);
            }
        }

        private void ArrangeHoverVisual(RadRect slot)
        {
            this.hoverVisual.ClearValue(UIElement.VisibilityProperty);

            var rect = slot.ToRect();

            Canvas.SetLeft(this.hoverVisual, rect.Left);
            Canvas.SetTop(this.hoverVisual, rect.Top);
            Canvas.SetZIndex(this.hoverVisual, ZIndexConstants.VisualStateControlBaseZIndex);
            this.hoverVisual.Width = rect.Width;
            this.hoverVisual.Height = rect.Height;
        }

        private void ClearCurrencyState()
        {
            this.currencyVisual.Visibility = Visibility.Collapsed;
        }

        private void EnsureCurrencyVisual()
        {
            if (this.currencyVisual == null)
            {
                this.currencyVisual = new DataGridCurrencyControl();

                this.AddVisualChild(this.currencyVisual);
            }
        }

        private void ArrangeCurrencyVisual(RadRect slot)
        {
            this.currencyVisual.ClearValue(UIElement.VisibilityProperty);

            var rect = slot.ToRect();

            Canvas.SetLeft(this.currencyVisual, rect.Left);
            Canvas.SetTop(this.currencyVisual, rect.Top);
            Canvas.SetZIndex(this.currencyVisual, ZIndexConstants.CurrencyBaseZIndex);
            this.currencyVisual.Width = rect.Width;
            this.currencyVisual.Height = rect.Height;
        }
    }
}
