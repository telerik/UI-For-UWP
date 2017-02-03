using System;
using System.Linq;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    public partial class DataGridServicePanel : IDragDropElement
    {
        private bool dropPossible;
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

        internal bool DropPossible
        {
            get
            {
                return this.dropPossible;
            }
            set
            {
                this.dropPossible = value;
                this.UpdateVisualState(this.IsTemplateApplied);
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
            this.DropPossible = true;
        }

        void IDragDropElement.DragOver(DragContext context)
        {
            if (context != null)
            {
                // Ensure that state is correct when dragging multiple items and one leaves the panel.
                var columnContext = context.PayloadData as DataGridColumnHeaderDragOperation;

                if (columnContext != null)
                {
                    this.DropPossible = this.Owner.DragBehavior.CanGroupBy(columnContext.Column);
                }
            }
        }

        void IDragDropElement.OnDragging(DragContext context)
        {
        }

        void IDragDropElement.DragLeave(DragContext context)
        {
            this.DropPossible = false;
        }

        bool IDragDropElement.CanDrop(DragContext dragContext)
        {
            if (dragContext != null)
            {
                var columnContext = dragContext.PayloadData as DataGridColumnHeaderDragOperation;

                if (columnContext != null)
                {
                    this.DropPossible = this.Owner.DragBehavior.CanGroupBy(columnContext.Column);

                    return this.DropPossible;
                }
            }

            return false;
        }

        void IDragDropElement.OnDrop(DragContext dragContext)
        {
            if (dragContext != null)
            {
                var columnContext = dragContext.PayloadData as DataGridColumnHeaderDragOperation;

                if (columnContext != null)
                {
                    this.Owner.DragBehavior.GroupBy(columnContext.Column);
                }
            }
        }

        void IDragDropElement.OnDragDropComplete(DragCompleteContext dragContext)
        {
        }

        void IDragDropElement.OnDragVisualCleared(DragCompleteContext dragContext)
        {
        }
    }
}