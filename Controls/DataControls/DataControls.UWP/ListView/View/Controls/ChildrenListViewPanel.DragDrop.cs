using System;
using System.Collections.Generic;
using System.Text;
using Telerik.UI.Xaml.Controls.Data.ListView.Commands;
using Telerik.UI.Xaml.Controls.Data.ListView.View.Controls;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop.Reorder;
using Windows.Foundation;

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

        bool IDragDropElement.CanStartDrag(DragDropTrigger trigger, object initializeContext = null)
        {
            return false;
        }

        DragStartingContext IDragDropElement.DragStarting(DragDropTrigger trigger, object initializeContext = null)
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