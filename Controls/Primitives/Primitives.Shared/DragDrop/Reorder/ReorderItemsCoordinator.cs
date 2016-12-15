using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives.DragDrop.Reorder
{
    internal class ReorderItemsCoordinator
    {
        private static readonly Duration DefaultAnimationDuration = new Duration(TimeSpan.FromSeconds(0.1));

        private HashSet<IReorderItem> animatingElements = new HashSet<IReorderItem>();

        private Dictionary<Storyboard, Action> runningAnimations = new Dictionary<Storyboard, Action>();

        public ReorderItemsCoordinator(IReorderItemsHost host)
        {
            this.Host = host;
            this.ReorderWithAnimation = true;
        }

        internal bool ReorderWithAnimation { get; set; }

        internal IReorderItemsHost Host
        {
            get;
            private set;
        }

        internal int ReorderItem(int sourceIndex, IReorderItem destinationItem)
        {
            var sourceItem = this.Host.ElementAt(sourceIndex);

            this.ReorderItems(sourceItem, destinationItem);

            return sourceItem.LogicalIndex;
        }

        internal void CommitReorderOperation(int sourceIndex, int destinationIndex)
        {
            if (sourceIndex != destinationIndex)
            {
                foreach (var animationPair in this.runningAnimations)
                {
                    animationPair.Key.Stop();
                    animationPair.Value();
                }

                this.runningAnimations.Clear();

                this.Host.CommitReorderOperation(sourceIndex, destinationIndex);
            }
        }

        internal void CancelReorderOperation(IReorderItem sourceItem, int initialSourceIndex)
        {
            var destinationItem = this.Host.ElementAt(initialSourceIndex);

            foreach (var animationPair in this.runningAnimations)
            {
                animationPair.Key.Stop();
                animationPair.Value();
            }

            this.runningAnimations.Clear();

            this.UpdatePositionAndIndices(sourceItem, destinationItem);
        }

        protected virtual void AnimateItem(IReorderItem item, Point position, Action actionCompleted)
        {
            var dragPositionMode = DragDrop.GetDragPositionMode(item.Visual);

            Storyboard b = new Storyboard();

            this.runningAnimations.Add(b, actionCompleted);

            if (dragPositionMode == DragPositionMode.RailY || dragPositionMode == DragPositionMode.Free)
            {
                var topAnimation = new DoubleAnimation();
                topAnimation.Duration = DefaultAnimationDuration;
                topAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseIn };
                topAnimation.To = position.Y;
                Storyboard.SetTargetProperty(topAnimation, "(Canvas.Top)");
                Storyboard.SetTarget(topAnimation, item.Visual);
                b.Children.Add(topAnimation);
            }

            if (dragPositionMode == DragPositionMode.RailX || dragPositionMode == DragPositionMode.Free)
            {
                var leftAnimation = new DoubleAnimation();
                leftAnimation.Duration = DefaultAnimationDuration;
                leftAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseIn };
                leftAnimation.To = position.X;
                Storyboard.SetTargetProperty(leftAnimation, "(Canvas.Left)");
                Storyboard.SetTarget(leftAnimation, item.Visual);
                b.Children.Add(leftAnimation);
            }

            b.Completed += (s, e) =>
                {
                    runningAnimations.Remove(b);
                    actionCompleted();
                };

            b.Begin();
        }

        private static Point GetRearangePosition(IReorderItem targetItem, IReorderItem adjasentItem)
        {
            Point position;

            if (targetItem.LogicalIndex > adjasentItem.LogicalIndex)
            {
                position = adjasentItem.ArrangePosition;
            }
            else
            {
                double x = -1;
                double y = -1;
                var dragPositionMode = DragDrop.GetDragPositionMode(targetItem.Visual);

                if (dragPositionMode == DragPositionMode.RailY || dragPositionMode == DragPositionMode.Free)
                {
                    x = adjasentItem.ArrangePosition.X;
                    y = adjasentItem.ArrangePosition.Y + adjasentItem.ActualSize.Height - targetItem.ActualSize.Height;
                }

                if (dragPositionMode == DragPositionMode.RailX || dragPositionMode == DragPositionMode.Free)
                {
                    x = adjasentItem.ArrangePosition.X + adjasentItem.ActualSize.Width - targetItem.ActualSize.Width;
                    y = adjasentItem.ArrangePosition.Y;
                }
                position = new Point(x, y);
            }

            return position;
        }

        private void ReorderItems(IReorderItem source, IReorderItem destination)
        {
            if (source != destination && !this.animatingElements.Contains(destination) && !this.animatingElements.Contains(source))
            {
                this.UpdatePositionAndIndices(source, destination);
            }
        }

        private void UpdatePositionAndIndices(IReorderItem source, IReorderItem destination)
        {
            var step = source.LogicalIndex < destination.LogicalIndex ? 1 : -1;

            var destinationIndex = destination.LogicalIndex;
            var sourceIndex = source.LogicalIndex;

            for (int i = sourceIndex + step; step > 0 ? i <= destinationIndex : i >= destinationIndex; i += step)
            {
                var currentDestinationItem = this.Host.ElementAt(i);

                if (this.animatingElements.Contains(currentDestinationItem))
                {
                    continue;
                }

                this.animatingElements.Add(currentDestinationItem);

                var destinationPosition = ReorderItemsCoordinator.GetRearangePosition(currentDestinationItem, source);
                source.ArrangePosition = ReorderItemsCoordinator.GetRearangePosition(source, currentDestinationItem);

                var index = source.LogicalIndex;
                source.LogicalIndex = currentDestinationItem.LogicalIndex;
                currentDestinationItem.LogicalIndex = index;

                Action moveCompletedAction = () =>
                {
                    currentDestinationItem.ArrangePosition = destinationPosition;
                    this.animatingElements.Remove(currentDestinationItem);
                    this.Host.OnItemsReordered(source, currentDestinationItem);
                };

                if (this.ReorderWithAnimation)
                {
                    this.AnimateItem(currentDestinationItem, destinationPosition, moveCompletedAction);
                }
                else
                {
                    moveCompletedAction();
                }
            }
        }
    }
}
