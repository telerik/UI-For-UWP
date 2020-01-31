using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Primitives.DragDrop
{
    internal class CanvasDragSurface : DragSurfaceBase
    {
        internal Thickness MaxPositionOffset = DragVisualContext.InfinityThickness;

        private Canvas adorner;
        private FrameworkElement owner;

        private bool shouldTruncateToBounds;

        public CanvasDragSurface(FrameworkElement owner, Canvas adorner, bool shouldTruncateToBounds = true)
        {
            this.owner = owner;
            this.adorner = adorner;
            this.shouldTruncateToBounds = shouldTruncateToBounds ? base.ShouldTruncateToBounds : shouldTruncateToBounds;
        }

        public override FrameworkElement RootElement
        {
            get { return this.owner; }
        }

        protected override bool ShouldTruncateToBounds
        {
            get
            {
                return this.shouldTruncateToBounds;
            }
        }

        public override DragVisualContext CreateDragContext()
        {
            var context = new ContentControlDragVisualContext();
            this.adorner.Children.Add(context.DragVisualHost);
            context.MaxPositionOffset = this.MaxPositionOffset;
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
