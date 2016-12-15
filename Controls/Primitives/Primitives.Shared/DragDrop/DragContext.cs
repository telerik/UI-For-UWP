using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Primitives.DragDrop
{
    internal class DragContext
    {
        private IDragDropOperation owner;

        public DragContext(object data, IDragDropOperation owner)
        {
            this.PayloadData = data;
            this.owner = owner;
        }

        public object PayloadData { get; private set; }

        public Rect GetDragVisualBounds(UIElement targetElement)
        {
            return this.owner.GetDragVisualBounds(targetElement);
        }

        public Point GetDragPosition(UIElement targetElement)
        {
            return this.owner.GetDragVisualPosition(targetElement);
        }

        public Point GetRelativeStartPosition()
        {
            return this.owner.RelativeStartPosition;
        }
    }
}