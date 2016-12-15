using System;
using System.Linq;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    internal class DataGridDragSurface : DragSurfaceBase
    {
        private static readonly Duration DefaultAnimationDuration = new Duration(TimeSpan.FromSeconds(0.15));

        private DataGridLayer parentLayer;

        public DataGridDragSurface(DataGridLayer owner)
        {
            this.parentLayer = owner;
        }

        public override FrameworkElement RootElement
        {
            get { return this.parentLayer.Owner; }
        }

        public override DragVisualContext CreateDragContext()
        {
            var context = new ContentControlDragVisualContext();
            this.parentLayer.AddVisualChild(context.DragVisualHost);

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

            if (dragSuccessful)
            {
                context.ClearDragVisual();
                this.parentLayer.RemoveVisualChild(context.DragVisualHost);
            }
            else
            {
                Storyboard b = new Storyboard();

                var topAnimation = new DoubleAnimation();
                topAnimation.Duration = DefaultAnimationDuration;
                topAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseInOut };
                topAnimation.To = context.DragStartPosition.Y;
                Storyboard.SetTargetProperty(topAnimation, "(Canvas.Top)");
                Storyboard.SetTarget(topAnimation, context.DragVisualHost);

                var leftAnimation = new DoubleAnimation();
                leftAnimation.Duration = DefaultAnimationDuration;
                leftAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseInOut };
                leftAnimation.To = context.DragStartPosition.X;
                Storyboard.SetTargetProperty(leftAnimation, "(Canvas.Left)");
                Storyboard.SetTarget(leftAnimation, context.DragVisualHost);

                b.Children.Add(topAnimation);
                b.Children.Add(leftAnimation);

                b.Completed += (s, e) =>
                {
                    context.ClearDragVisual();
                    this.parentLayer.RemoveVisualChild(context.DragVisualHost);
                };

                b.Begin();
            }
        }
    }
}
