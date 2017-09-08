using System;
using Telerik.UI.Xaml.Controls.Data.ListView.Commands;
using Telerik.UI.Xaml.Controls.Data.ListView.View.Controls;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    public partial class RadListViewItem : IDragDropElement
    {
        private bool skipHitTest;

        bool IDragDropElement.SkipHitTest
        {
            get
            {
                return this.skipHitTest;
            }
            set
            {
                this.skipHitTest = value;
            }
        }

        bool IDragDropElement.CanStartDrag(DragDropTrigger trigger, object initializeContext)
        {
            if (trigger == DragDropTrigger.Hold)
            {
                return this.ListView.IsItemReorderEnabled && this.ListView.GroupDescriptors.Count == 0 && !this.IsHandleEnabled;
            }
            else
            {
                if (this.isHandleEnabled && initializeContext == this.reorderHandle)
                {
                    return this.ListView.IsItemReorderEnabled && this.ListView.GroupDescriptors.Count == 0 && this.IsHandleEnabled;
                }

                return this.ListView.IsActionOnSwipeEnabled && !(this.ListView.ReorderMode == ListViewReorderMode.Handle && this.ListView.IsItemReorderEnabled == true);
            }
        }

        DragStartingContext IDragDropElement.DragStarting(DragDropTrigger trigger, object initializeContext)
        {
            this.ListView.CleanupSwipedItem();

            DragStartingContext context = null;
            var dataItem = this.DataContext;

            if (dataItem != null)
            {
                bool isExecuted = false;
                DragAction? dragAction = null;

                if (trigger == DragDropTrigger.Drag && (!this.isHandleEnabled || initializeContext != this.reorderHandle))
                {
                    if (this.ListView.LayoutDefinition is StackLayoutDefinition)
                    {
                        if (this.Owner.Orientation == Orientation.Horizontal)
                        {
                            if (this.SwipeDirection == ListViewItemSwipeDirection.Forward)
                            {
                                DragDrop.SetDragPositionMode(this, DragPositionMode.RailYForward);
                            }
                            else if (this.SwipeDirection == ListViewItemSwipeDirection.Backwards)
                            {
                                DragDrop.SetDragPositionMode(this, DragPositionMode.RailYBackwards);
                            }
                            else if (this.SwipeDirection == ListViewItemSwipeDirection.All)
                            {
                                DragDrop.SetDragPositionMode(this, DragPositionMode.RailY);
                            }
                        }
                        else
                        {
                            if (this.SwipeDirection == ListViewItemSwipeDirection.Forward)
                            {
                                DragDrop.SetDragPositionMode(this, DragPositionMode.RailXForward);
                            }
                            else if (this.SwipeDirection == ListViewItemSwipeDirection.Backwards)
                            {
                                DragDrop.SetDragPositionMode(this, DragPositionMode.RailXBackwards);
                            }
                            else if (this.SwipeDirection == ListViewItemSwipeDirection.All)
                            {
                                DragDrop.SetDragPositionMode(this, DragPositionMode.RailX);
                            }
                        }

                        isExecuted = this.ListView.commandService.ExecuteCommand(CommandId.ItemDragStarting, new ItemDragStartingContext(dataItem, DragAction.ItemAction, this));
                        dragAction = DragAction.ItemAction;

                        this.PrepareDragVisual(dragAction.Value);
                    }
                }
                else
                {
                    isExecuted = this.ListView.commandService.ExecuteCommand(CommandId.ItemDragStarting, new ItemDragStartingContext(dataItem, DragAction.Reorder, this));
                    dragAction = DragAction.Reorder;

                    this.PrepareDragVisual(dragAction.Value);
                }

                if (isExecuted && dragAction.HasValue)
                {
                    this.CancelDirectManipulations();

                    if (dragAction.Value == DragAction.Reorder)
                    {
                        context = this.InitializeReorder();
                    }
                    else if (dragAction.Value == DragAction.ItemAction)
                    {
                        // We might need to cancel manipulations on the dragvisual
                        //// this.dragVisual.CancelDirectManipulations();

                        this.UpdateActionContentClipping(0);

                        context = new DragStartingContext
                        {
                            DragSurface = new CanvasDragSurface(this.ListView.childrenPanel, this.ListView.childrenPanel as Canvas, false),
                            DragVisual = this.dragVisual,
                            HitTestStrategy = new ListViewItemHittestStrategy(this, this),
                            Payload = DragAction.ItemAction
                        };
                    }
                }
            }

            return context;
        }

        void IDragDropElement.DragEnter(DragContext context)
        {
        }

        void IDragDropElement.DragOver(DragContext context)
        {
            if (context == null)
            {
                return;
            }

            var data = context.PayloadData as ReorderItemsDragOperation;
            if (data != null)
            {
                var position = context.GetDragPosition(this);

                if (this.ShouldReorder(position, data))
                {
                    if (this.ListView.swipedItem == data.Item)
                    {
                        this.ClearActionContent();
                    }
                    var newIndex = this.reorderCoordinator.ReorderItem(data.CurrentSourceReorderIndex, this);
                    data.CurrentSourceReorderIndex = newIndex;
                }
            }
            else if (this.ListView != null && this.ListView.isActionContentDisplayed)
            {
                var startPoint = context.GetRelativeStartPosition();
                var currentPoint = context.GetDragPosition(this);

                this.dragX = currentPoint.X - startPoint.X;
                this.dragY = currentPoint.Y - startPoint.Y;

                this.UpdateActionContentClipping(this.ListView.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal ? this.dragY : this.dragX);
            }
        }

        void IDragDropElement.DragLeave(DragContext context)
        {
        }

        bool IDragDropElement.CanDrop(DragContext context)
        {
            if (context != null && this.reorderCoordinator != null)
            {
                var data = context.PayloadData as ReorderItemsDragOperation;

                if (data != null)
                {
                    return this.ListView.DragBehavior.CanReorder(data.Data, this.DataContext);
                }
                else
                {
                    if (this.Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
                    {
                        switch (this.SwipeDirection)
                        {
                            case ListViewItemSwipeDirection.Forward:
                                return this.dragX > this.ListView.ItemSwipeThreshold;

                            case ListViewItemSwipeDirection.Backwards:
                                return this.dragX < 0 && Math.Abs(this.dragX) > this.ListView.ItemSwipeThreshold;

                            case ListViewItemSwipeDirection.All:
                            default:
                                return Math.Abs(this.dragX) > this.ListView.ItemSwipeThreshold;
                        }
                    }
                    else
                    {
                        switch (this.SwipeDirection)
                        {
                            case ListViewItemSwipeDirection.Forward:
                                return this.dragY > this.ListView.ItemSwipeThreshold;

                            case ListViewItemSwipeDirection.Backwards:
                                return this.dragY < 0 && Math.Abs(this.dragY) > this.ListView.ItemSwipeThreshold;

                            case ListViewItemSwipeDirection.All:
                            default:
                                return Math.Abs(this.dragY) > this.ListView.ItemSwipeThreshold;
                        }
                    }
                }
            }

            return true;
        }

        void IDragDropElement.OnDrop(DragContext context)
        {
            if (context == null)
            {
                return;
            }

            var data = context.PayloadData as ReorderItemsDragOperation;
            if (data != null)
            {
                this.reorderCoordinator.CommitReorderOperation(data.InitialSourceIndex, data.CurrentSourceReorderIndex);
            }

            var startPoint = context.GetRelativeStartPosition();
            var currentPoint = context.GetDragPosition(this);

            this.dragX = currentPoint.X - startPoint.X;
            this.dragY = currentPoint.Y - startPoint.Y;
        }

        void IDragDropElement.OnDragging(DragContext dragContext)
        {
            if (dragContext == null)
            {
                return;
            }

            if (dragContext.PayloadData is DragAction && (DragAction)dragContext.PayloadData == DragAction.ItemAction)
            {
                var offset = this.ListView.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal ? this.dragY : this.dragX;
                var swipeContext = new ItemSwipingContext(this.DataContext, this, offset);
                this.ListView.swipedItem = this;
                this.ListView.commandService.ExecuteCommand(CommandId.ItemSwiping, swipeContext);
            }
        }

        void IDragDropElement.OnDragDropComplete(DragCompleteContext context)
        {
            var data = context.PayloadData as ReorderItemsDragOperation;

            if (data != null)
            {
                this.FinalizeReorder(context);

                object destinationDataItem = this.GetDestinationDataItem(data);
                bool isExecuted = this.ListView.commandService.ExecuteCommand(CommandId.ItemReorderComplete, new ItemReorderCompleteContext(data.Data, destinationDataItem, this));
            }
            else
            {
                if (context.DragSuccessful)
                {
                    double offset = 0;
                    var dragMode = DragDrop.GetDragPositionMode(this);

                    if (this.ListView.Orientation == Orientation.Horizontal)
                    {
                        switch (dragMode)
                        {
                            case DragPositionMode.RailXForward:
                                offset = Math.Max(0, this.dragY);
                                break;

                            case DragPositionMode.RailXBackwards:
                                offset = Math.Min(0, this.dragY);
                                break;

                            case DragPositionMode.RailX:
                                offset = this.dragY;
                                break;

                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (dragMode)
                        {
                            case DragPositionMode.RailXForward:
                                offset = Math.Max(0, this.dragX);
                                break;

                            case DragPositionMode.RailXBackwards:
                                offset = Math.Min(0, this.dragX);
                                break;

                            case DragPositionMode.RailX:
                                offset = this.dragX;
                                break;

                            default:
                                break;
                        }
                    }
                    var swipeContext = new ItemSwipeActionCompleteContext(this.DataContext, this, offset);

                    bool isExecuted = this.ListView.commandService.ExecuteCommand(CommandId.ItemSwipeActionComplete, swipeContext);

                    if (isExecuted)
                    {
                        this.UpdateActionContentClipping(swipeContext.FinalDragOffset);
                        this.isDraggedForAction = true;
                        this.ListView.swipedItem = this;
                    }
                    else
                    {
                        this.ClearActionContent();
                    }
                }
                else
                {
                    this.ClearActionContent();
                    this.ListView.CleanupSwipedItem();
                }
            }
            this.Opacity = 1;

            DragDrop.SetDragPositionMode(this, DragPositionMode.Free);
            this.ListView.InvalidatePanelArrange();
        }

        void IDragDropElement.OnDragVisualCleared(DragCompleteContext context)
        {
        }
    }
}