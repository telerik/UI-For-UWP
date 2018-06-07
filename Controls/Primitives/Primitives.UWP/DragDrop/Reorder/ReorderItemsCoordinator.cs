using System;
using System.Collections.Generic;
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
        }

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

        protected void AnimateItem(IReorderItem source, IReorderItem destination, Point position)
        {
            var dragPositionMode = DragDrop.GetDragPositionMode(destination.Visual);

            Storyboard b = new Storyboard();
            Action actionCompleted = () =>
            {
                destination.ArrangePosition = position;
                this.animatingElements.Remove(destination);
                this.Host.OnItemsReordered(source, destination);
            };

            this.runningAnimations.Add(b, actionCompleted);

            if (dragPositionMode == DragPositionMode.RailY || dragPositionMode == DragPositionMode.Free)
            {
                var topAnimation = new DoubleAnimation();
                topAnimation.Duration = DefaultAnimationDuration;
                topAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseIn };
                topAnimation.To = position.Y;
                Storyboard.SetTargetProperty(topAnimation, "(Canvas.Top)");
                Storyboard.SetTarget(topAnimation, destination.Visual);
                b.Children.Add(topAnimation);
            }

            if (dragPositionMode == DragPositionMode.RailX || dragPositionMode == DragPositionMode.Free)
            {
                var leftAnimation = new DoubleAnimation();
                leftAnimation.Duration = DefaultAnimationDuration;
                leftAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseIn };
                leftAnimation.To = position.X;
                Storyboard.SetTargetProperty(leftAnimation, "(Canvas.Left)");
                Storyboard.SetTarget(leftAnimation, destination.Visual);
                b.Children.Add(leftAnimation);
            }

            b.Completed += (s, e) =>
                {
                    runningAnimations.Remove(b);
                    actionCompleted();
                };

            b.Begin();
        }

        protected virtual void UpdatePositionAndIndices(IReorderItem source, IReorderItem destination)
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

                var destinationPosition = GetRearrangePosition(currentDestinationItem, source);
                source.ArrangePosition = GetRearrangePosition(source, currentDestinationItem);

                var index = source.LogicalIndex;
                source.LogicalIndex = currentDestinationItem.LogicalIndex;
                currentDestinationItem.LogicalIndex = index;

                this.AnimateItem(source, currentDestinationItem, destinationPosition);
            }
        }

        private static Point GetRearrangePosition(IReorderItem targetItem, IReorderItem adjacentItem)
        {
            var position = targetItem.ArrangePosition;
            var dragPositionMode = DragDrop.GetDragPositionMode(targetItem.Visual);

            if (dragPositionMode == DragPositionMode.RailY || dragPositionMode == DragPositionMode.Free)
            {
                position.Y = adjacentItem.ArrangePosition.Y;

                if (targetItem.LogicalIndex < adjacentItem.LogicalIndex)
                {
                    position.Y += adjacentItem.ActualSize.Height - targetItem.ActualSize.Height;
                }
            }

            if (dragPositionMode == DragPositionMode.RailX || dragPositionMode == DragPositionMode.Free)
            {
                position.X = adjacentItem.ArrangePosition.X;

                if (targetItem.LogicalIndex < adjacentItem.LogicalIndex)
                {
                    position.X += adjacentItem.ActualSize.Width - targetItem.ActualSize.Width;
                }
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
    }
}
