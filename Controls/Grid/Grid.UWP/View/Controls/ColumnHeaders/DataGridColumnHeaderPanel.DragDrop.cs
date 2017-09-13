using System;
using System.Linq;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop.Reorder;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    public partial class DataGridColumnHeaderPanel : IReorderItemsHost, IDragDropElement
    {
        private ReorderItemsCoordinator reorderCoordinator;

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
            return this.Children.OfType<IReorderItem>().FirstOrDefault(c => c.LogicalIndex == index);
        }

        void IReorderItemsHost.OnItemsReordered(IReorderItem sourceItem, IReorderItem destinationItem)
        {
        }

        void IReorderItemsHost.CommitReorderOperation(int sourceIndex, int destinationIndex)
        {
            this.Owner.DragBehavior.ReorderVisibleColumn(sourceIndex, destinationIndex);

            foreach (var item in this.Children)
            {
                item.Opacity = 1;
            }
        }

        bool IDragDropElement.CanStartDrag(DragDropTrigger trigger, object initializeContext)
        {
            return false;
        }

        DragStartingContext IDragDropElement.DragStarting(DragDropTrigger trigger, object initializeContext)
        {
            throw new NotImplementedException();
        }

        void IDragDropElement.DragEnter(DragContext context)
        {
        }

        void IDragDropElement.DragOver(DragContext context)
        {
        }

        void IDragDropElement.DragLeave(DragContext context)
        {
        }

        void IDragDropElement.OnDragging(DragContext context)
        {
        }

        bool IDragDropElement.CanDrop(DragContext dragContext)
        {
            return true;
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

        internal void SetupReorderItem(IReorderItem reorderItem)
        {
            if (reorderItem != null)
            {
                reorderItem.Coordinator = this.reorderCoordinator;
            }
        }

        private void InitializeDragDrop()
        {
            this.reorderCoordinator = new ReorderItemsCoordinator(this);
        }
    }
}