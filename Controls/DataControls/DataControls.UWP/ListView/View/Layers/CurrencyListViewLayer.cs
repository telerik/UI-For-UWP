using Telerik.Core;
using Telerik.UI.Xaml.Controls.Data.ListView.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class CurrencyListViewLayer : ListViewLayer
    {
        private const int CurrentItemZIndex = 4;

        private RadRect currentNodeSlot;
        private FrameworkElement currencyVisual;

        internal FrameworkElement CurrencyVisual
        {
            get
            {
                return this.currencyVisual;
            }
        }

        internal void UpdateCurrencyDecoration(RadRect slot)
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

        internal void ArrangeCurrencyVisual(RadRect slot)
        {
            this.currencyVisual.ClearValue(UIElement.VisibilityProperty);

            var rect = slot.ToRect();

            Canvas.SetLeft(this.currencyVisual, rect.Left);
            Canvas.SetTop(this.currencyVisual, rect.Top);

            // TODO CURRENCY REVIEW Z INDEX: 
            Canvas.SetZIndex(this.currencyVisual, CurrentItemZIndex);
            this.currencyVisual.Width = rect.Width;
            this.currencyVisual.Height = rect.Height;
        }

        private void ClearCurrencyState()
        {
            this.currencyVisual.Visibility = Visibility.Collapsed;
        }

        private void EnsureCurrencyVisual()
        {
            if (this.currencyVisual == null)
            {
                this.currencyVisual = new ListViewCurrencyControl();

                this.AddVisualChild(this.currencyVisual);
            }
        }
    }
}
