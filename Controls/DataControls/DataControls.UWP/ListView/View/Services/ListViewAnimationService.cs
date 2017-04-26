using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Telerik.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Data.ListView;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal interface IAnimatingService
    {
        ListViewAnimationService AnimatingService { get; }
    }

    internal class ListViewAnimationService : ServiceBase<RadListView>
    {
        private List<object> scheduledItemsForAnimation = new List<object>();
        private Dictionary<RadAnimation, Tuple<object, Action<object>>> runningAnimations = new Dictionary<RadAnimation, Tuple<object, Action<object>>>();
        private List<IAnimated> scheduledModelsForRecycle = new List<IAnimated>();

        public ListViewAnimationService(RadListView owner)
            : base(owner)
        {
        }

        internal void PlayCheckBoxLayerAnimation(UIElement element, bool forward, bool beforeItem, double itemLength)
        {
            var checkBoxanimation = new RadMoveAnimation() { Duration = TimeSpan.FromSeconds(0.3), FillBehavior = AnimationFillBehavior.HoldEnd };
            checkBoxanimation.EndPoint = new Point(0, 0);
            itemLength = itemLength == 0 ? 29 : itemLength;
            var offset = beforeItem ? itemLength : -itemLength;
            checkBoxanimation.StartPoint = this.Owner.Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical ? new Point(-offset, 0) : new Point(0, -offset);

            if (!forward)
            {
                checkBoxanimation = checkBoxanimation.CreateOpposite() as RadMoveAnimation;
            }

            RadAnimationManager.Play(element, checkBoxanimation);
        }

        internal void PlayCheckModeAnimation(UIElement element, bool forward, bool beforeItem, double itemLength)
        {
            var animation = new RadMoveAnimation() { Duration = TimeSpan.FromSeconds(0.3), FillBehavior = AnimationFillBehavior.HoldEnd };
            animation.StartPoint = new Point(0, 0);
            itemLength = itemLength == 0 ? 29 : itemLength;
            var offset = beforeItem ? itemLength : -itemLength;

            animation.EndPoint = this.Owner.Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical ? new Point(offset, 0) : new Point(0, offset);

            if (forward)
            {
                animation.Ended += this.ForwardCheckModeAnimationEnded;
            }
            else
            {
                animation = animation.CreateOpposite() as RadMoveAnimation;
                animation.Ended += this.BackwardsCheckModeAnimationEnded;
            }

            RadAnimationManager.Play(element, animation);
        }
        
        internal bool IsAnimating(ItemInfo? info)
        {
            var models = this.runningAnimations.Values.Where((tuple) =>
                {
                    var model = tuple.Item1 as GeneratedItemModel;
                    if (model != null && model.ItemInfo.Equals(info))
                    {
                        return true;
                    }
                    return false;
                });
            if (models.Count() > 0)
            {
                return true;
            }
            return false;
        }

        internal bool HasItemsForAnimation()
        {
            return this.scheduledItemsForAnimation.Count > 0;
        }

        internal void ScheduleItemForAnimation(object item)
        {
            this.scheduledItemsForAnimation.Add(item);
        }

        internal void PlayNullSourceAnimation(Action<object> callback)
        {
            var animation = this.CreateAnimation(AnimationTrigger.NullSource);
            this.runningAnimations.Add(animation, new Tuple<object, Action<object>>(this.Owner.animatingChildrenPanel, callback));

            RadAnimationManager.Play(this.Owner.animatingChildrenPanel, animation);
        }

        internal void PlaySourceResetAnimation(Action<object> callback)
        {
            var animation = this.CreateAnimation(AnimationTrigger.ResetSource);
            this.runningAnimations.Add(animation, new Tuple<object, Action<object>>(this.Owner.animatingChildrenPanel, callback));

            RadAnimationManager.Play(this.Owner.animatingChildrenPanel, animation);
        }

        internal void PlayNewSourceAnimation(Action<object> callback)
        {
            var animation = this.CreateAnimation(AnimationTrigger.NewSource);
            this.runningAnimations.Add(animation, new Tuple<object, Action<object>>(this.Owner.animatingChildrenPanel, callback));

            RadAnimationManager.Play(this.Owner.animatingChildrenPanel, animation);
        }

        internal void PlayItemAddedAnimations(Action<object> callback)
        {
            foreach (var scheduledItem in this.scheduledItemsForAnimation)
            {
                foreach (var displayedItem in this.Owner.Model.ForEachDisplayedElement())
                {
                    if (displayedItem.ItemInfo.Item.Equals(scheduledItem))
                    {
                        var animation = this.CreateAnimation(AnimationTrigger.AddedItem);
                        displayedItem.IsAnimating = true;
                        RadAnimationManager.Play(displayedItem.Container as FrameworkElement, animation);

                        this.runningAnimations.Add(animation, new Tuple<object, Action<object>>(displayedItem, callback));
                    }
                }
            }

            this.scheduledItemsForAnimation.Clear();
        }

        internal void PlayItemRemovedAnimation(IList changedItems, Action<object> callback)
        {
            foreach (var changedItem in changedItems)
            {
                foreach (var displayedItem in this.Owner.Model.ForEachDisplayedElement())
                {
                    if (changedItem.Equals(displayedItem.ItemInfo.Item))
                    {
                        if (changedItem.Equals(this.Owner.CurrentItem))
                        {
                            var currencyVisual = this.Owner.currencyLayerCache.CurrencyVisual as IAnimated;
                            if (currencyVisual != null)
                            {
                                currencyVisual.IsAnimating = true;
                                var animation = this.Owner.ItemRemovedAnimation.Clone();
                                animation.FillBehavior = AnimationFillBehavior.Stop;
                                animation.Ended += this.CurrentItemAnimationEnded;
                                RadAnimationManager.Play(currencyVisual.Container as FrameworkElement, animation);
                                this.runningAnimations.Add(animation, new Tuple<object, Action<object>>(currencyVisual, callback));
                            }
                        }

                        this.scheduledModelsForRecycle.Add(displayedItem);
                        displayedItem.IsAnimating = true;

                        if (displayedItem != null)
                        {
                            var animation = this.CreateAnimation(AnimationTrigger.RemovedItem);

                            this.runningAnimations.Add(animation, new Tuple<object, Action<object>>(displayedItem, callback));

                            RadAnimationManager.Play(displayedItem.Container as FrameworkElement, animation);
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
            }
        }

        internal void StopAnimations()
        {
            while (this.runningAnimations.Count > 0)
            {
                RadAnimationManager.StopIfRunning(this.GetContainerForItem(this.runningAnimations.Values.First().Item1) as FrameworkElement, this.runningAnimations.Keys.First());
            }
        }

        internal void StopAnimation(object item)
        {
            var animation = this.runningAnimations.Single((keyValuePair) =>
                {
                    if (keyValuePair.Value.Item1.Equals(item))
                    {
                        return true;
                    }
                    return false;
                }).Key;

            RadAnimationManager.StopIfRunning(this.GetContainerForItem(item) as FrameworkElement, animation);
        }

        private object GetContainerForItem(object item)
        {
            if (item is IAnimated)
            {
                return (item as IAnimated).Container;
            }

            return item;
        }

        private RadAnimation CreateAnimation(AnimationTrigger change)
        {
            RadAnimation animation;
            switch (change)
            {
                case AnimationTrigger.ResetSource:
                    animation = this.Owner.ItemRemovedAnimation.Clone();
                    animation.FillBehavior = AnimationFillBehavior.Stop;
                    animation.Ended += this.ResetSourceAnimationEnded;
                    return animation;

                case AnimationTrigger.NullSource:
                    animation = this.Owner.ItemRemovedAnimation.Clone();
                    animation.FillBehavior = AnimationFillBehavior.Stop;
                    animation.Ended += this.NullSourceAnimationEnded;
                    return animation;

                case AnimationTrigger.NewSource:
                    animation = this.Owner.ItemAddedAnimation.Clone();
                    animation.FillBehavior = AnimationFillBehavior.Stop;
                    animation.Ended += this.AnimationEndedOnNewSource;
                    return animation;

                case AnimationTrigger.AddedItem:
                    animation = this.Owner.ItemAddedAnimation.Clone();
                    animation.Ended += this.AnimationEndedOnAddedItem;
                    animation.FillBehavior = AnimationFillBehavior.Stop;
                    return animation;

                case AnimationTrigger.RemovedItem:
                    animation = this.Owner.ItemRemovedAnimation.Clone();
                    animation.FillBehavior = AnimationFillBehavior.Stop;
                    animation.Ended += this.ItemRemovedAnimationEnded;
                    return animation;
            }

            return null;
        }

        private void CurrentItemAnimationEnded(object sender, AnimationEndedEventArgs e)
        {
            var animation = sender as RadAnimation;
            var item = this.runningAnimations[animation].Item1;
            if (item is IAnimated)
            {
                (item as IAnimated).IsAnimating = false;
            }
            this.runningAnimations.Remove(animation);
        }

        private void ItemRemovedAnimationEnded(object sender, AnimationEndedEventArgs e)
        {
            var callback = this.runningAnimations[sender as RadAnimation].Item2;

            if (this.scheduledModelsForRecycle.Count > 0)
            {
                var itemForRecycle = this.scheduledModelsForRecycle.First();
                itemForRecycle.IsAnimating = false;
                this.scheduledModelsForRecycle.Remove(itemForRecycle);

                if (this.runningAnimations.Keys.Count > 0)
                {
                    this.runningAnimations.Remove(sender as RadAnimation);
                }

                callback(itemForRecycle);
            }
        }

        private void AnimationEndedOnNewSource(object sender, AnimationEndedEventArgs e)
        {
            var callback = this.runningAnimations[sender as RadAnimation].Item2;
            this.runningAnimations.Remove(sender as RadAnimation);
            callback(null);
        }

        private void AnimationEndedOnAddedItem(object sender, AnimationEndedEventArgs e)
        {
            var animation = sender as RadAnimation;
            var callback = this.runningAnimations[animation].Item2;
            var item = this.runningAnimations[animation].Item1;
            if (item is IAnimated)
            {
                (item as IAnimated).IsAnimating = false;
            }

            this.runningAnimations.Remove(animation);
            callback(item);
        }

        private void NullSourceAnimationEnded(object sender, AnimationEndedEventArgs e)
        {
            var callback = this.runningAnimations[sender as RadAnimation].Item2;
            this.runningAnimations.Remove(sender as RadAnimation);
            callback(null);
        }

        private void BackwardsCheckModeAnimationEnded(object sender, AnimationEndedEventArgs e)
        {
            (sender as RadAnimation).Ended -= this.BackwardsCheckModeAnimationEnded;
            this.Owner.itemCheckBoxService.itemsAnimated = true;
            this.Owner.itemCheckBoxService.OnIsCheckModeActiveChanged();
        }

        private void ForwardCheckModeAnimationEnded(object sender, AnimationEndedEventArgs e)
        {
            (sender as RadAnimation).Ended -= this.ForwardCheckModeAnimationEnded;
            this.Owner.itemCheckBoxService.itemsAnimated = true;
        }

        private void ResetSourceAnimationEnded(object sender, AnimationEndedEventArgs e)
        {
            var callback = this.runningAnimations[sender as RadAnimation].Item2;
            this.runningAnimations.Remove(sender as RadAnimation);
            callback(null);
        }
    }
}
