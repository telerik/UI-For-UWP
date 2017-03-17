using System;
using System.Linq;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop.Reorder;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    public partial class DataGridColumnHeader : IDragDropElement, IReorderItem
    {
        private int logicalIndex;
        private ReorderItemsCoordinator reorderCoordinator;
        private bool skipHitTest;

        DependencyObject IReorderItem.Visual
        {
            get
            {
                return this;
            }
        }

        int IReorderItem.LogicalIndex
        {
            get
            {
                return this.logicalIndex;
            }
            set
            {
                this.logicalIndex = value;
            }
        }

        Point IReorderItem.ArrangePosition
        {
            get
            {
                return new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
            }
            set
            {
                Canvas.SetLeft(this, value.X);
                Canvas.SetTop(this, value.Y);
            }
        }

        ReorderItemsCoordinator IReorderItem.Coordinator
        {
            get
            {
                return this.reorderCoordinator;
            }
            set
            {
                this.reorderCoordinator = value;
            }
        }

        Size IReorderItem.ActualSize
        {
            get
            {
                return this.ArrangeRestriction;
            }
        }

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

        internal Size ArrangeRestriction { get; set; }

        bool IDragDropElement.CanStartDrag(DragDropTrigger trigger, object initializeContext)
        {
            return this.Owner.DragBehavior != null && this.Owner.DragBehavior.CanStartDrag(this.Column);
        }

        DragStartingContext IDragDropElement.DragStarting(DragDropTrigger trigger, object initializeContext)
        {
            this.Opacity = 0;
            DragStartingContext context = this.CreateDragContext();
            this.Owner.DragBehavior.OnDragStarted(this);
            this.Owner.CancelEdit();
            this.Owner.ContentFlyout.Hide(DataGridFlyoutId.All);
            this.IsSelected = false;
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
                    var newIndex = this.reorderCoordinator.ReorderItem(data.CurrentSourceReorderIndex, this);
                    data.CurrentSourceReorderIndex = newIndex;
                }
            }
        }

        void IDragDropElement.DragLeave(DragContext context)
        {
        }

        bool IDragDropElement.CanDrop(DragContext dragContext)
        {
            if (dragContext != null && this.reorderCoordinator != null)
            {
                var data = dragContext.PayloadData as ReorderItemsDragOperation;

                if (data != null)
                {
                    var sourceElement = this.reorderCoordinator.Host.ElementAt(data.CurrentSourceReorderIndex) as DataGridColumnHeader;

                    return sourceElement != null && this.Owner.DragBehavior.CanReorder(sourceElement.Column, this.Column);
                }

                return false;
            }
            return false;
        }

        void IDragDropElement.OnDrop(DragContext dragContext)
        {
            if (dragContext == null)
            {
                return;
            }

            var data = dragContext.PayloadData as ReorderItemsDragOperation;
            if (data != null)
            {
                this.reorderCoordinator.CommitReorderOperation(data.InitialSourceIndex, data.CurrentSourceReorderIndex);
            }
        }

        void IDragDropElement.OnDragDropComplete(DragCompleteContext dragContext)
        {
            if (dragContext != null)
            {
                var data = dragContext.PayloadData as DataGridColumnHeaderDragOperation;

                if (data != null && data.HeaderOwner != null)
                {
                    data.HeaderOwner.DragBehavior.OnDragDropCompleted(this, dragContext.DragSuccessful);

                    var columnReordered = dragContext.DragSuccessful && (dragContext.Destination is DataGridColumnHeader || dragContext.Destination is DataGridColumnHeaderPanel);

                    if (!columnReordered && this.reorderCoordinator != null)
                    {
                        this.reorderCoordinator.CancelReorderOperation(this, data.InitialSourceIndex);
                    }
                }

                var gridOwner = this.Owner as RadDataGrid;
                if (gridOwner != null)
                {
                    var gridPeer = FrameworkElementAutomationPeer.FromElement(gridOwner) as RadDataGridAutomationPeer;
                    if (gridPeer != null && gridPeer.childrenCache != null)
                    {
                        gridPeer.childrenCache = null;
                    }
                }
            }
        }

        void IDragDropElement.OnDragVisualCleared(DragCompleteContext dragContext)
        {
            this.Opacity = 1;

            if (this.Column != null)
            {
                this.Content = this.Column.Header;
            }

            if (this.Owner != null)
            {
                this.Owner.InvalidateHeadersMeasure();
                this.Owner.updateService.RegisterUpdate(UpdateFlags.AffectsColumnsWidth);
            }
        }

        void IDragDropElement.OnDragging(DragContext dragContext)
        {
        }

        private bool ShouldReorder(Point position, ReorderItemsDragOperation data)
        {
            if (this.reorderCoordinator == null)
            {
                return false;
            }

            var sourceElement = this.reorderCoordinator.Host.ElementAt(data.CurrentSourceReorderIndex) as DataGridColumnHeader;

            bool draggingFromRight = position.X >= this.ActualWidth / 2 + DragInitializer.DefaultStartTreshold && position.X <= this.ActualWidth && this.logicalIndex > data.CurrentSourceReorderIndex;
            bool draggingFromLeft = position.X <= this.ActualWidth / 2 - DragInitializer.DefaultStartTreshold && position.X >= 0 && this.logicalIndex < data.CurrentSourceReorderIndex;

            bool canReorderColumn = sourceElement != null && this.Owner.DragBehavior.CanReorder(sourceElement.Column, this.Column);

            return canReorderColumn && (draggingFromLeft || draggingFromRight);
        }

        private DragStartingContext CreateDragContext()
        {
            var dragContent = this.Owner.DragBehavior.GetDragVisual(this);

            var headerContent = dragContent ?? this.Content;
            this.Content = null;

            var dragVisual = new DataGridColumnDragControl
            {
                DataContext = this.DataContext,
                Width = this.ArrangeSize.Width,
                Height = this.ArrangeSize.Height,
                Content = headerContent,
                FilterGlyphVisibility = this.FilterGlyphVisibility
            };

            var payload = new DataGridColumnHeaderDragOperation(this);

            return new DragStartingContext
            {
                Payload = payload,
                DragSurface = this.Owner.DragAdornerLayer.DragSurface,
                DragVisual = dragVisual,
                HitTestStrategy = new ColumnHeaderHittestStrategy(this, this.Owner.DragAdornerLayer.DragSurface.RootElement)
            };
        }
    }
}