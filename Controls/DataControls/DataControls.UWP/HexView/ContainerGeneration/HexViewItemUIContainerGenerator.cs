using System;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Data.HexView
{
    internal class HexViewItemUIContainerGenerator : IUIContainerGenerator<GeneratedItemModel, object>
    {
        private static Type hexViewItemType = typeof(RadHexHubTile);

        private RadHexView owner;

        public HexViewItemUIContainerGenerator(RadHexView owner)
        {
            this.owner = owner;
        }

        public void PrepareContainerForItem(GeneratedItemModel element)
        {
            this.MakeVisible(element);

            var hexItem = element.Container as RadHexHubTile;
            if (hexItem != null)
            {
                this.UpdateItem(hexItem, element.ItemInfo.Item);
            }
        }

        public void ClearContainerForItem(GeneratedItemModel element)
        {
            this.MakeHidden(element);
        }

        public object GetContainerTypeForItem(object item)
        {
            return hexViewItemType;
        }

        public object GenerateContainerForItem(object item, object containerType)
        {
            UIElement element = null;

            if (containerType != null && containerType.Equals(hexViewItemType))
            {
                var tile = new RadHexHubTile();
                tile.Owner = this.owner;
                element = tile;
            }
            else
            {
                throw new ArgumentException("Unknown container type.");
            }

            this.owner.ChildrenPanel.Children.Add(element);

            return element;
        }

        public void MakeVisible(GeneratedItemModel element)
        {
            var frameworkElement = element.Container as FrameworkElement;
            if (frameworkElement != null && frameworkElement.Visibility != Visibility.Visible)
            {
                frameworkElement.SetValue(UIElement.VisibilityProperty, Visibility.Visible);
            }
        }

        public void MakeHidden(GeneratedItemModel element)
        {
            var frameworkElement = element.Container as FrameworkElement;
            if (frameworkElement != null && frameworkElement.Visibility != Visibility.Collapsed)
            {
                frameworkElement.SetValue(UIElement.VisibilityProperty, Visibility.Collapsed);
            }
        }

        public void SetOpacity(GeneratedItemModel element, byte opacity)
        {
            ((FrameworkElement)element.Container).SetValue(UIElement.OpacityProperty, opacity);
        }

        private void UpdateItem(RadHexHubTile item, object context)
        {
            this.owner.UpdateBindings(item);

            if (item.DataContext != context)
            {
                item.DataContext = context;
            }

            var style = this.owner.ItemStyle;
            if (style == null)
            {
                if (this.owner.ItemStyleSelector != null)
                {
                    style = this.owner.ItemStyleSelector.SelectStyle(context, item);
                }
            }

            if (style != null)
            {
                if (item.Style != style)
                {
                    item.Style = style;
                }
            }
            else
            {
                item.ClearValue(RadHexHubTile.StyleProperty);
            }
        }
    }
}
