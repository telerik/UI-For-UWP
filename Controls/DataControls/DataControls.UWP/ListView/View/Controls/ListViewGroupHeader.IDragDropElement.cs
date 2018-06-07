using Telerik.UI.Xaml.Controls.Data.ListView.View.Controls;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.ListView.Primitives
{
    public partial class ListViewGroupHeader : IDragDropElement
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

        bool IDragDropElement.CanDrop(DragContext dragContext)
        {
            return false;
        }

        bool IDragDropElement.CanStartDrag(DragDropTrigger trigger, object initializeContext)
        {
            return false;
        }

        void IDragDropElement.DragEnter(DragContext context)
        {
            if (this.Owner.Orientation == Orientation.Vertical)
            {
                DragDrop.SetDragPositionMode(this, DragPositionMode.RailY);
            }
            else
            {
                DragDrop.SetDragPositionMode(this, DragPositionMode.RailX);
            }
        }

        void IDragDropElement.DragLeave(DragContext context)
        {
        }

        void IDragDropElement.DragOver(DragContext context)
        {
            if (context == null)
            {
                return;
            }

            var data = context.PayloadData as ReorderItemsDragOperation;

            if (data == null)
            {
                return;
            }

            var position = context.GetDragPosition(this);

            if (!this.ShouldReorder(position, data))
            {
                return;
            }

            if (this.Owner.swipedItem == data.Item)
            {
                this.Owner.ResetActionContent();
            }

            var newIndex = this.reorderCoordinator.ReorderItem(data.CurrentSourceReorderIndex, this);

            data.CurrentSourceReorderIndex = newIndex;
        }

        DragStartingContext IDragDropElement.DragStarting(DragDropTrigger trigger, object initializeContext)
        {
            return null;
        }

        void IDragDropElement.OnDragDropComplete(DragCompleteContext dragContext)
        {
        }

        void IDragDropElement.OnDragging(DragContext dragContext)
        {
        }

        void IDragDropElement.OnDragVisualCleared(DragCompleteContext dragContext)
        {
        }

        void IDragDropElement.OnDrop(DragContext dragContext)
        {
        }

        private bool ShouldReorder(Point position, ReorderItemsDragOperation data)
        {
            if (this.reorderCoordinator == null)
            {
                return false;
            }

            var sourceElement = this.reorderCoordinator.Host.ElementAt(data.CurrentSourceReorderIndex);

            if (sourceElement == null)
            {
                return false;
            }

            var startPosition = this.Owner.Orientation == Orientation.Horizontal ? position.X : position.Y;
            var itemLength = this.Owner.Orientation == Orientation.Horizontal ? this.ActualWidth : this.ActualHeight;

            return (startPosition >= itemLength / 2 + DragInitializer.DefaultStartTreshold &&
                    startPosition <= itemLength && this.logicalIndex > data.CurrentSourceReorderIndex) ||
                   (startPosition <= itemLength / 2 - DragInitializer.DefaultStartTreshold &&
                    startPosition >= 0 && this.logicalIndex < data.CurrentSourceReorderIndex);
        }
    }
}
