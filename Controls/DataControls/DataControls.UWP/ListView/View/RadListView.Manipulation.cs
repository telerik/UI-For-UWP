﻿using System;
using Telerik.UI.Xaml.Controls.Data.ListView;
using Telerik.UI.Xaml.Controls.Data.ListView.Commands;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Data
{
    public partial class RadListView
    {

        public ListViewItemSwipeDirection ItemSwipeDirection
        {
            get { return (ListViewItemSwipeDirection)GetValue(ItemSwipeDirectionProperty); }
            set { SetValue(ItemSwipeDirectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemSwipeDirection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemSwipeDirectionProperty =
            DependencyProperty.Register(nameof(ItemSwipeDirection), typeof(ListViewItemSwipeDirection), typeof(RadListView), new PropertyMetadata(ListViewItemSwipeDirection.All, OnItemSwipeDirectionChanged));

        private const double CheckBoxSelectionTouchTargetThreshold = 20;

        /// <summary>
        /// Ends all currently running drag operations.
        /// </summary>
        public void EndItemSwipe()
        {
            if (this.swipedItem != null)
            {
                this.swipedItem.EndDragOperation();
                this.CleanupSwipedItem();
            }
        }

        internal void OnItemTap(RadListViewItem radListViewItem, Point relativePosition)
        {
            var item = radListViewItem.DataContext;
            this.CleanupSwipedItem();
            if (item != null)
            {
                var touchPoint = this.Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical ? relativePosition.X : relativePosition.Y;
                var itemSize = this.Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical ? radListViewItem.arrangeRect.Width : radListViewItem.arrangeRect.Height;
                bool isInArea = this.ItemCheckBoxPosition == CheckBoxPosition.BeforeItem ? touchPoint <= CheckBoxSelectionTouchTargetThreshold : itemSize - touchPoint <= CheckBoxSelectionTouchTargetThreshold;
                if (isInArea && this.SelectionMode == DataControlsSelectionMode.MultipleWithCheckBoxes)
                {
                    this.itemCheckBoxService.OnItemTap(radListViewItem);
                }
                this.commandService.ExecuteCommand(CommandId.ItemTap, new ItemTapContext(item, radListViewItem));
            }
        }

        internal void CleanupSwipedItem()
        {
            if (this.swipedItem != null)
            {
                this.swipedItem.isDraggedForAction = false;
                this.swipedItem.ResetDragPosition();
                this.swipeActionContentControl.Tapped -= this.swipedItem.SwipeActionContentControl_Tapped;
                this.swipedItem = null;
            }

            this.ResetActionContent();
        }

        internal void OnItemTap(ItemTapContext tapContext)
        {
            this.selectionService.Select(tapContext);
            this.currencyService.ChangeCurrentItem(tapContext.Item, true, true);
        }

        internal void OnItemHold(RadListViewItem radListViewItem, HoldingRoutedEventArgs e)
        {
            // TODO add holdCommand.
            DragDrop.StartDrag(radListViewItem, e, DragDropTrigger.Hold);

            //// var item = radListViewItem.DataContext;
            //// if (item != null)
            //// {
            ////     this.commandService.ExecuteCommand(CommandId.ItemTap, new ItemTapContext(item, radListViewItem));
            //// }
        }

        internal void OnItemReorderHandlePressed(RadListViewItem radListViewItem, PointerRoutedEventArgs e, object sender)
        {
            DragDrop.StartDrag(radListViewItem, e, DragDropTrigger.Drag, sender);
        }

        internal void OnItemActionControlTap(RadListViewItem radListViewItem, double offset)
        {
            var item = radListViewItem.DataContext;
            if (item != null)
            {
                this.commandService.ExecuteCommand(CommandId.ItemActionTap, new ItemActionTapContext(item, radListViewItem, offset));
            }
        }

        private static void OnItemSwipeDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;
            if (listView != null && listView.IsTemplateApplied)
            {
                listView.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
            }
        }
    }
}