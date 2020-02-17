using System;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Primitives.DragDrop
{
    internal abstract class DragSurfaceBase : IDragSurface
    {
        public abstract FrameworkElement RootElement
        {
            get;
        }

        protected virtual bool ShouldTruncateToBounds
        {
            get
            {
                return true;
            }
        }

        public abstract DragVisualContext CreateDragContext();

        public virtual Rect PositionDragHost(DragVisualContext context, Point dragPoint, Point relativeStartPosition)
        {
            if (context == null)
            {
                return Rect.Empty;
            }

            Point restrictedPoint = DragSurfaceBase.GetRestrictedDragPoint(context, dragPoint, relativeStartPosition, context.MaxPositionOffset);

            if (this.ShouldTruncateToBounds)
            {
                restrictedPoint = this.TruncateToBounds(context.DragVisualHost, restrictedPoint);
            }

            var rect = new Rect(restrictedPoint, context.DragVisualHost.RenderSize);
            context.DragVisualHost.Arrange(rect);

            return rect;
        }

        public virtual void CompleteDrag(DragVisualContext context, bool dragSuccessful)
        {
            if (context == null)
            {
                return;
            }

            context.ClearDragVisual();
        }

        private static Point GetRestrictedDragPoint(DragVisualContext context, Point dragPoint, Point relativeStartPosition, Thickness maxPositionOffset)
        {
            double x = RadMath.CoerceValue(dragPoint.X - relativeStartPosition.X, -maxPositionOffset.Right, maxPositionOffset.Left);
            double y = RadMath.CoerceValue(dragPoint.Y - relativeStartPosition.Y, -maxPositionOffset.Bottom, maxPositionOffset.Top);

            switch (context.PositionRestriction)
            {
                case DragPositionMode.RailX:
                    y = context.DragStartPosition.Y;
                    break;
                case DragPositionMode.RailY:
                    x = context.DragStartPosition.X;
                    break;
                case DragPositionMode.RailXForward:
                    y = context.DragStartPosition.Y;
                    x = Math.Max(0, x);
                    break;
                case DragPositionMode.RailXBackwards:
                    y = context.DragStartPosition.Y;
                    x = Math.Min(0, x);
                    break;
                case DragPositionMode.RailYForward:
                    x = context.DragStartPosition.X;
                    y = Math.Max(0, y);
                    break;
                case DragPositionMode.RailYBackwards:
                    x = context.DragStartPosition.X;
                    y = Math.Min(0, y);
                    break;
                case DragPositionMode.Free:
                default:
                    break;
            }

            return new Point(x, y);
        }

        private Point TruncateToBounds(FrameworkElement dragHost, Point point)
        {
            if (this.RootElement != null && dragHost != null)
            {
                var x = Math.Max(0, point.X);
                var y = Math.Max(0, point.Y);

                x = Math.Min(x, this.RootElement.ActualWidth - dragHost.ActualWidth);
                y = Math.Min(y, this.RootElement.ActualHeight - dragHost.ActualHeight);

                return new Point(x, y);
            }

            return point;
        }
    }
}
