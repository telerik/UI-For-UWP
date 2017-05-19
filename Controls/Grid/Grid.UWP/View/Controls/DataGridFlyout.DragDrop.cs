using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop.Reorder;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    public partial class DataGridFlyout : IDragDropElement, IReorderItemsHost
    {
        internal ReorderItemsCoordinator reorderCoordinator;
        private IDragSurface dragSurface;
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

        IReorderItem IReorderItemsHost.ElementAt(int index)
        {
            return this.container.Children.OfType<IReorderItem>().Where(c => c.LogicalIndex == index).FirstOrDefault();
        }

        void IReorderItemsHost.OnItemsReordered(IReorderItem sourceItem, IReorderItem destinationItem)
        {
        }

        void IReorderItemsHost.CommitReorderOperation(int sourceIndex, int destinationIndex)
        {
            this.CommitReorderOperation(sourceIndex, destinationIndex);
        }
        
        bool IDragDropElement.CanStartDrag(DragDropTrigger trigger, object initializeContext)
        {
            return false;
        }

        DragStartingContext IDragDropElement.DragStarting(DragDropTrigger trigger, object initializeContext)
        {
            return null;
        }

        void IDragDropElement.DragEnter(DragContext context)
        {
        }

        void IDragDropElement.OnDragging(DragContext context)
        {
        }

        void IDragDropElement.DragOver(DragContext context)
        {
        }

        void IDragDropElement.DragLeave(DragContext context)
        {
        }

        bool IDragDropElement.CanDrop(DragContext dragContext)
        {
            if (dragContext == null)
            {
                return false;
            }

            return dragContext.PayloadData is ReorderItemsDragOperation;
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
        }

        void IDragDropElement.OnDragVisualCleared(DragCompleteContext dragContext)
        {
        }

        internal virtual void CommitReorderOperation(int sourceIndex, int destinationIndex)
        {
        }

        internal void InitializeDragDrop()
        {
            if (this.adorner != null)
            {
                this.dragSurface = new CanvasDragSurface(this, this.adorner);
            }

            this.reorderCoordinator = new ReorderItemsCoordinator(this);
        }

        internal virtual void SetupDragDropProperties(IReorderItem item, int logicalIndex)
        {
            DragDrop.SetDragPositionMode(item.Visual, DragPositionMode.RailY);

            item.Coordinator = this.reorderCoordinator;
            item.LogicalIndex = logicalIndex;
        }

        internal void HandleDragSurfaceRequested(object sender, DragSurfaceRequestedEventArgs e)
        {
            e.DragSurface = this.dragSurface;
        }

        internal virtual bool CanStartDrag(DragDropTrigger trigger, object initializeContext)
        {
            return false;
        }
    }
}
