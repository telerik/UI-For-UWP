using Telerik.UI.Xaml.Controls.Data.ListView.View.Controls;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    public partial class ChildrenListViewPanel : IDragDropElement
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
            return false;
        }

        DragStartingContext IDragDropElement.DragStarting(DragDropTrigger trigger, object initializeContext)
        {
            return null;
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
        }

        void IDragDropElement.OnDragDropComplete(DragCompleteContext dragContext)
        {
        }

        void IDragDropElement.OnDragVisualCleared(DragCompleteContext dragContext)
        {
        }

        void IDragDropElement.OnDragging(DragContext dragContext)
        {
        }
    }
}