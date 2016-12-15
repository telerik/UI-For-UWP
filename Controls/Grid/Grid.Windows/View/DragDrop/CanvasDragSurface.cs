using System;
using System.Linq;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Drag
{
    internal class CanvasDragSurface : DragSurfaceBase
    {
        private Canvas adorner;
        private FrameworkElement owner;

        public CanvasDragSurface(FrameworkElement owner, Canvas adorner)
        {
            this.owner = owner;
            this.adorner = adorner;
        }

        public override Windows.UI.Xaml.FrameworkElement RootElement
        {
            get { return this.owner; }
        }

        public override DragVisualContext CreateDragContext()
        {
            var context = new ContentControlDragVisualContext();
            this.adorner.Children.Add(context.DragVisualHost);
            return context;
        }

        public override Windows.Foundation.Rect PositionDragHost(DragVisualContext context, Point dragPoint, Point relativeStartPosition)
        {
            if (context == null)
            {
                return Rect.Empty;
            }

            Rect position = base.PositionDragHost(context, dragPoint, relativeStartPosition);

            Canvas.SetLeft(context.DragVisualHost, position.X);
            Canvas.SetTop(context.DragVisualHost, position.Y);

            return position;
        }

        public override void CompleteDrag(DragVisualContext context, bool dragSuccessful)
        {
            if (context == null)
            {
                return;
            }

            base.CompleteDrag(context, dragSuccessful);

            // Check if Adorner is in visual tree (calling this when element is unloaded will throw COM exception).
            if (this.adorner.Parent != null)
            {
                this.adorner.Children.Remove(context.DragVisualHost);
            }
        }
    }
}
