using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.DragDrop
{
    internal interface IDragDropElement
    {
        bool SkipHitTest { get; set; }

        bool CanStartDrag(DragDropTrigger trigger, object initializeContext = null);

        DragStartingContext DragStarting(DragDropTrigger trigger, object initializeContext = null);

        void DragEnter(DragContext context);

        void DragOver(DragContext context);

        void DragLeave(DragContext context);

        bool CanDrop(DragContext dragContext);

        void OnDrop(DragContext dragContext);

        void OnDragging(DragContext dragContext);

        void OnDragDropComplete(DragCompleteContext dragContext);

        void OnDragVisualCleared(DragCompleteContext dragContext);   
    }
}
