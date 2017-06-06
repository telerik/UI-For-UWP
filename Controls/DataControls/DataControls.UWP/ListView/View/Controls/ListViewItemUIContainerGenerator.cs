using System;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Data.ListView.Model;
using Telerik.UI.Xaml.Controls.Data.ListView.Primitives;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop.Reorder;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.ListView.View.Controls
{
    internal class ListViewItemUIContainerGenerator : IUIContainerGenerator<GeneratedItemModel, ItemGenerationContext>
    {
        private const int MaxGroupCount = 20;
        private static Type loadingDataControlType = typeof(ListViewLoadDataControl);
        private static Type placeholderModelType = typeof(PlaceholderInfo);
        private static Type listViewItemType = typeof(RadListViewItem);
        private static Type listViewGroupHeaderType = typeof(ListViewGroupHeader);
        private static object listViewFrozenGroupHeaderType = new object();

        private RadListView owner;

        public ListViewItemUIContainerGenerator(RadListView owner)
        {
            this.owner = owner;
        }

        public void PrepareContainerForItem(GeneratedItemModel element)
        {
            var loadDataControl = element.Container as ListViewLoadDataControl;
            if (loadDataControl != null)
            {
                loadDataControl.Owner = this.owner;
                if (this.owner != null)
                {
                    this.owner.PrepareLoadDataControl(loadDataControl);
                }
                this.owner.visualStateService.RegisterDataLoadingListener(loadDataControl);
            }

            var listItem = element.Container as RadListViewItem;
            if (listItem != null)
            {
                listItem.Orientation = this.owner.Orientation;

                this.owner.PrepareContainerForItem(listItem, element.ItemInfo.Item);
                listItem.PrepareSwipeDragHandles();

                var reorderItem = listItem as IReorderItem;
                reorderItem.LogicalIndex = element.ItemInfo.Id;
                this.owner.PrepareReorderItem(listItem);
            }

            var groupHeader = element.Container as ListViewGroupHeader;
            if (groupHeader != null && this.owner.GroupDescriptors.Count > element.ItemInfo.Level)
            {
                groupHeader.Owner = this.owner;

                var context = new GroupHeaderContext()
                {
                    Level = element.ItemInfo.Level,
                    Descriptor = this.owner.GroupDescriptors[element.ItemInfo.Level],
                    Group = element.ItemInfo.Item as IDataGroup,
                    IsExpanded = !element.ItemInfo.IsCollapsed,
                    Owner = this.owner
                };

                if (groupHeader.IsFrozen)
                {
                    // We assume that no more that 20 GroupDescriptions will be added.
                    // If needed this number can be increased.
                    Canvas.SetZIndex(groupHeader, MaxGroupCount - element.ItemInfo.Level);
                }

                groupHeader.IsExpanded = context.IsExpanded;
                this.owner.PrepareContainerForGroupHeader(groupHeader, context);
            }
        }

        public void ClearContainerForItem(GeneratedItemModel element)
        {
            if (element.ContainerType == listViewItemType)
            {
                var item = element.Container as RadListViewItem;

                this.owner.swipeActionContentControl.Tapped -= item.SwipeActionContentControl_Tapped;
                item.ClearValue(RadListViewItem.IsSelectedProperty);
                item.isDraggedForAction = false;
                this.owner.CleanupReorderItem(item);
                item.EndDragOperation();
                this.owner.CleanupSwipedItem();

                this.owner.ClearContainerForItem(item);
            }

            if (element.ContainerType == listViewGroupHeaderType)
            {
                var item = element.Container as ListViewGroupHeader;
                item.ClearValue(ListViewGroupHeader.WidthProperty);
                item.ClearValue(ListViewGroupHeader.HeightProperty);
                item.ArrangeSize.Height = 0;
                item.ArrangeSize.Width = 0;

                this.owner.ClearContainerForGroupHeader(item);
            }
        }

        public object GetContainerTypeForItem(ItemGenerationContext info)
        {
            var item = info.Info;
            bool isFrozen = info.IsFrozen;

            if (info.Info.Item is IGroup)
            {
                if (isFrozen)
                {
                    return listViewFrozenGroupHeaderType;
                }

                return listViewGroupHeaderType;
            }

            if (info.Info.Item.GetType() == placeholderModelType)
            {
                var type = (info.Info.Item as PlaceholderInfo).Type;
                switch (type)
                {
                    case PlaceholderInfoType.IncrementalLoading:
                        return loadingDataControlType;
                }
            }

            return typeof(RadListViewItem);
        }

        public object GenerateContainerForItem(ItemGenerationContext info, object containerType)
        {
            UIElement element = null;

            ListViewGroupHeader groupHeader = null;
            if (listViewGroupHeaderType.Equals(containerType))
            {
                groupHeader = this.owner.GetContainerForGroupHeader();
            }
            else if (object.Equals(listViewFrozenGroupHeaderType, containerType))
            {
                groupHeader = this.owner.GetContainerForGroupHeader(true);
            }

            if (groupHeader != null)
            {
                if (groupHeader.IsFrozen)
                {
                    // Frozen Headers are placed in different panel (not in childrens panel).
                    this.owner.frozenGroupHeadersHost.Children.Add(groupHeader);
                }
                else
                {
                    this.owner.childrenPanel.Children.Add(groupHeader);
                }

                return groupHeader;
            }
            else
            {
                if (containerType != null)
                {
                    if (containerType.Equals(loadingDataControlType))
                    {
                        element = new ListViewLoadDataControl();
                        this.owner.loadMoreDataControl = element as ListViewLoadDataControl;
                    }
                    else if (containerType.Equals(listViewItemType))
                    {
                        var listViewItem = this.owner.GetContainerForItem();
                        listViewItem.ListView = this.owner;
                        element = listViewItem;
                    }
                    else
                    {
                        throw new ArgumentException("Unknown container type.");
                    }
                }
            }

            this.owner.childrenPanel.Children.Add(element);

            return element;
        }

        public void MakeVisible(GeneratedItemModel element)
        {
            ((FrameworkElement)element.Container).SetValue(UIElement.VisibilityProperty, Visibility.Visible);
        }

        public void MakeHidden(GeneratedItemModel element)
        {
            ((FrameworkElement)element.Container).SetValue(UIElement.VisibilityProperty, Visibility.Collapsed);
        }

        public void SetOpacity(GeneratedItemModel element, byte opacity)
        {
        }
    }
}
