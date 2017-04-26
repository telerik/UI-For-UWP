using System;
using System.Collections.Generic;
using Telerik.Core;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Data
{
    public partial class RadVirtualizingDataControl
    {
        /// <summary>
        /// Identifies the <see cref="RadVirtualizingDataControl.ItemAnimationMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemAnimationModeProperty =
            DependencyProperty.Register(nameof(ItemAnimationMode), typeof(ItemAnimationMode), typeof(RadVirtualizingDataControl), new PropertyMetadata(ItemAnimationMode.PlayAll, OnItemAnimationModeChanged));

        /// <summary>
        /// Identifies the ItemAddedAnimationInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemAddedAnimationIntervalProperty =
            DependencyProperty.Register(nameof(ItemAddedAnimationInterval), typeof(TimeSpan), typeof(RadVirtualizingDataControl), new PropertyMetadata(TimeSpan.FromMilliseconds(50), OnItemAddedAnimationIntervalChanged));

        /// <summary>
        /// Identifies the ItemRemovedAnimationInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemRemovedAnimationIntervalProperty =
            DependencyProperty.Register(nameof(ItemRemovedAnimationInterval), typeof(TimeSpan), typeof(RadVirtualizingDataControl), new PropertyMetadata(TimeSpan.FromMilliseconds(50), OnItemRemovedAnimationIntervalChanged));

        /// <summary>
        /// Identifies the ItemAddedAnimation dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemAddedAnimationProperty =
            DependencyProperty.Register(nameof(ItemAddedAnimation), typeof(RadAnimation), typeof(RadVirtualizingDataControl), new PropertyMetadata(null, OnItemAddedAnimationChanged));

        /// <summary>
        /// Identifies the ItemRemovedAnimation dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemRemovedAnimationProperty =
            DependencyProperty.Register(nameof(ItemRemovedAnimation), typeof(RadAnimation), typeof(RadVirtualizingDataControl), new PropertyMetadata(null, OnItemRemovedAnimationChanged));

        internal List<SingleItemAnimationContext> scheduledRemoveAnimations;
        internal List<SingleItemAnimationContext> scheduledAddAnimations;

        internal RadAnimation itemAddedAnimationCache;
        internal RadAnimation itemRemovedAnimationCache;
        internal bool itemAddedBatchAnimationScheduled = false;
        internal bool itemRemovedBatchAnimationScheduled = false;
        internal TimeSpan itemAddedAnimationIntervalCache = TimeSpan.FromMilliseconds(50);
        internal TimeSpan itemRemovedAnimationIntervalCache = TimeSpan.FromMilliseconds(50);

        internal ItemAnimationMode itemAnimationModeCache = ItemAnimationMode.PlayAll;

        /// <summary>
        /// Fires when an item animation has ended. This event will be fired both for
        /// add and remove item animations.
        /// </summary>
        public event EventHandler<ItemAnimationEndedEventArgs> ItemAnimationEnded;

        /// <summary>
        /// Gets or sets the item animation mode. The item animation mode
        /// defines how animations for items being added or removed from the source collection
        /// are played. This property also defines whether items will be animating when the <see cref="DataControlBase.ItemsSource"/>
        /// property is bound to a new collection or is reset.
        /// </summary>
        public ItemAnimationMode ItemAnimationMode
        {
            get
            {
                return this.itemAnimationModeCache;
            }
            set
            {
                this.SetValue(RadVirtualizingDataControl.ItemAnimationModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="RadAnimation"/> class
        /// that is used to animate visual containers once they are added
        /// into the <see cref="RadVirtualizingDataControl"/>'s viewport.
        /// </summary>
        public RadAnimation ItemAddedAnimation
        {
            get
            {
                return this.itemAddedAnimationCache;
            }
            set
            {
                this.SetValue(ItemAddedAnimationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="RadAnimation"/> class
        /// that is used to animate visual containers once they are removed
        /// from the <see cref="RadVirtualizingDataControl"/>'s viewport.
        /// </summary>
        public RadAnimation ItemRemovedAnimation
        {
            get
            {
                return this.itemRemovedAnimationCache;
            }
            set
            {
                this.SetValue(ItemRemovedAnimationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="TimeSpan"/> struct
        /// that defines the interval between the separate
        /// animations played for each container when it is
        /// added to the viewport upon initial load.
        /// </summary>
        public TimeSpan ItemAddedAnimationInterval
        {
            get
            {
                return (TimeSpan)this.GetValue(ItemAddedAnimationIntervalProperty);
            }
            set
            {
                this.SetValue(ItemAddedAnimationIntervalProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="TimeSpan"/> struct
        /// that defines the interval between the separate
        /// animations played for each container when it is
        /// removed from the viewport upon collection reset.
        /// </summary>
        public TimeSpan ItemRemovedAnimationInterval
        {
            get
            {
                return (TimeSpan)this.GetValue(ItemRemovedAnimationIntervalProperty);
            }
            set
            {
                this.SetValue(ItemRemovedAnimationIntervalProperty, value);
            }
        }

        internal virtual void OnItemAddedBatchAnimation()
        {
            this.itemAddedBatchAnimationScheduled = false;
            RadAnimation itemAddedAnimation = this.itemAddedAnimationCache;
            TimeSpan interval = this.ItemAddedAnimationInterval;
            TimeSpan defaultDelay = itemAddedAnimation.InitialDelay;
            foreach (RadVirtualizingDataControlItem container in this.realizedItems)
            {
                if (!container.scheduledForBatchAnimation)
                {
                    continue;
                }

                this.PlaySingleItemAddedAnimation(container);
                itemAddedAnimation.InitialDelay += interval;
            }

            itemAddedAnimation.InitialDelay = defaultDelay;
        }

        internal virtual void OnItemRemovedBatchAnimation()
        {
            FrameworkElement[] viewportItems = this.ViewportItems;
            RadAnimation itemRemovedAnimation = this.itemRemovedAnimationCache;
            TimeSpan interval = this.ItemRemovedAnimationInterval;
            TimeSpan defaultDelay = itemRemovedAnimation.InitialDelay;
            foreach (RadVirtualizingDataControlItem container in viewportItems)
            {
                if (!this.CanPlayAnimationForItem(container, false))
                {
                    continue;
                }

                this.PlaySingleItemRemovedAnimation(container);
                this.virtualizationStrategy.RecycleItem(container, false);
                itemRemovedAnimation.InitialDelay += interval;
            }
            itemRemovedAnimation.InitialDelay = defaultDelay;
        }

        internal void StopAllAddedAnimations()
        {
            while (this.scheduledAddAnimations.Count > 0)
            {
                // Here we do not need to enqueue the top item since the ForceStop method will invoke the RadAnimation.Ended event where
                // this is done.
                SingleItemAnimationContext context = this.scheduledAddAnimations[0];
                RadAnimationManager.ForceStop(context.AssociatedItem, this.itemAddedAnimationCache);
            }
        }

        internal void StopAllRemovedAnimations()
        {
            while (this.scheduledRemoveAnimations.Count > 0)
            {
                // Here we do not need to enqueue the top item since the ForceStop method will invoke the RadAnimation.Ended event where
                // this is done.
                SingleItemAnimationContext context = this.scheduledRemoveAnimations[0];
                RadAnimationManager.ForceStop(context.AssociatedItem, this.itemRemovedAnimationCache);
            }
        }

        internal virtual void OnItemAddedAnimationEnded(RadAnimation animation, SingleItemAnimationContext context)
        {
            if (context != null)
            {
                this.scheduledAddAnimations.Remove(context);
                context.AssociatedItem.scheduledForBatchAnimation = false;
            }

            if (this.scheduledAddAnimations.Count == 0 && !this.IsLoaded)
            {
                this.itemAddedAnimationCache.Ended -= this.OnItemAddedAnimation_Ended;
            }
            this.OnItemAnimationEnded(new ItemAnimationEndedEventArgs() { Animation = animation, RemainingAnimationsCount = this.scheduledAddAnimations.Count });
        }

        internal virtual void OnItemRemovedAnimationEnded(RadAnimation animation, SingleItemAnimationContext context)
        {
            // When a remove animation ends we take it from the queue and...
            if (context != null)
            {
                context.AssociatedItem.Visibility = Visibility.Collapsed;
                this.scheduledRemoveAnimations.Remove(context);
            }

            // In case the animated container is already recycled - we hide it. This happens
            // when a single item remove animation has been started, not a batch one.
            if (!this.itemRemovedBatchAnimationScheduled && context != null)
            {
                if (this.realizedItems.Count > 0)
                {
                    this.virtualizationStrategy.OnAfterItemRemovedAnimationEnded(context);
                }
                else
                {
                    this.CleanupAfterCollectionReset();
                }
            }
            else
            {
                if (this.scheduledRemoveAnimations.Count == 0)
                {
                    this.itemRemovedBatchAnimationScheduled = false;
                    this.CleanupAfterCollectionReset();
                    if (this.listSource.Count == 0)
                    {
                        if (this.ItemsSource == null)
                        {
                            this.ClearReycledItems();
                        }
                    }
                    else
                    {
                        this.BalanceVisualSpace();
                    }
                }
            }

            if (this.scheduledRemoveAnimations.Count == 0)
            {
                this.virtualizationStrategy.RecalculateViewportMeasurements();
            }

            if (this.scheduledRemoveAnimations.Count == 0 && !this.IsLoaded)
            {
                this.itemRemovedAnimationCache.Ended -= this.OnItemRemovedAnimation_Ended;
            }

            this.OnItemAnimationEnded(new ItemAnimationEndedEventArgs() { Animation = animation, RemainingAnimationsCount = this.scheduledRemoveAnimations.Count });
        }

        internal virtual bool CanPlayAnimationForItem(RadVirtualizingDataControlItem item, bool adding)
        {
            return this.IsLoaded || (adding && this.itemAddedBatchAnimationScheduled);
        }

        internal void PlaySingleItemAddedAnimation(RadVirtualizingDataControlItem item)
        {
            this.virtualizationStrategy.PlaySingleItemAddedAnimation(item);
        }

        internal void PlaySingleItemRemovedAnimation(RadVirtualizingDataControlItem item)
        {
            this.virtualizationStrategy.PlaySingleItemRemovedAnimation(item);
        }

        /// <summary>
        /// Called when an animation used to animate a visual container
        /// out of the viewport has ended. Fires the <see cref="ItemAnimationEnded"/> event.
        /// </summary>
        /// <param name="args">An instance of the <see cref="ItemAnimationEndedEventArgs"/> class
        /// that holds information about the event.</param>
        protected virtual void OnItemAnimationEnded(ItemAnimationEndedEventArgs args)
        {
            if (this.ItemAnimationEnded != null)
            {
                this.ItemAnimationEnded(this, args);
            }
        }

        private static void OnItemAddedAnimationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadVirtualizingDataControl typedSender = sender as RadVirtualizingDataControl;
            typedSender.OnItemAddedAnimationChanged(args);
        }

        private static void OnItemRemovedAnimationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadVirtualizingDataControl typedSender = sender as RadVirtualizingDataControl;
            typedSender.OnItemRemovedAnimationChanged(args);
        }

        private static void OnItemAddedAnimationIntervalChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadVirtualizingDataControl typedSender = sender as RadVirtualizingDataControl;
            typedSender.itemAddedAnimationIntervalCache = (TimeSpan)args.NewValue;
        }

        private static void OnItemAnimationModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadVirtualizingDataControl typedSender = sender as RadVirtualizingDataControl;
            typedSender.itemAnimationModeCache = (ItemAnimationMode)args.NewValue;
        }

        private static void OnItemRemovedAnimationIntervalChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadVirtualizingDataControl typedSender = sender as RadVirtualizingDataControl;
            typedSender.itemRemovedAnimationIntervalCache = (TimeSpan)args.NewValue;
        }

        partial void InitItemAnimations()
        {
            this.scheduledRemoveAnimations = new List<SingleItemAnimationContext>();
            this.scheduledAddAnimations = new List<SingleItemAnimationContext>();
        }

        private void OnItemAddedAnimationChanged(DependencyPropertyChangedEventArgs args)
        {
            this.StopAllAddedAnimations();
            if (this.itemAddedAnimationCache != null)
            {
                this.itemAddedAnimationCache.Ended -= this.OnItemAddedAnimation_Ended;
            }

            this.itemAddedAnimationCache = args.NewValue as RadAnimation;

            if (this.itemAddedAnimationCache != null)
            {
                this.itemAddedAnimationCache.Ended += this.OnItemAddedAnimation_Ended;
                this.itemAddedAnimationCache.RepeatBehavior = null;
            }
        }

        private void OnItemRemovedAnimationChanged(DependencyPropertyChangedEventArgs args)
        {
            this.StopAllRemovedAnimations();
            if (this.itemRemovedAnimationCache != null)
            {
                this.itemRemovedAnimationCache.Ended -= this.OnItemRemovedAnimation_Ended;
            }

            this.itemRemovedAnimationCache = args.NewValue as RadAnimation;

            if (this.itemRemovedAnimationCache != null)
            {
                this.itemRemovedAnimationCache.FillBehavior = AnimationFillBehavior.Stop;
                this.itemRemovedAnimationCache.RepeatBehavior = null;
                this.itemRemovedAnimationCache.Ended += this.OnItemRemovedAnimation_Ended;
            }
        }

        private void OnItemAddedAnimation_Ended(object sender, AnimationEndedEventArgs e)
        {
            SingleItemAnimationContext context = this.GetAnimationContextForTarget(e.AnimationInfo.Target as RadVirtualizingDataControlItem, true);
            RadAnimation endedAnimation = sender as RadAnimation;
            this.OnItemAddedAnimationEnded(endedAnimation, context);
        }

        private void OnItemRemovedAnimation_Ended(object sender, AnimationEndedEventArgs e)
        {
            SingleItemAnimationContext endedAnimation = this.GetAnimationContextForTarget(e.AnimationInfo.Target as RadVirtualizingDataControlItem, false);
            this.OnItemRemovedAnimationEnded(sender as RadAnimation, endedAnimation);
        }

        private SingleItemAnimationContext GetAnimationContextForTarget(RadVirtualizingDataControlItem target, bool lookInAdded)
        {
            if (lookInAdded)
            {
                foreach (SingleItemAnimationContext c in this.scheduledAddAnimations)
                {
                    if (c.AssociatedItem == target)
                    {
                        return c;
                    }
                }
            }
            else
            {
                foreach (SingleItemAnimationContext c in this.scheduledRemoveAnimations)
                {
                    if (c.AssociatedItem == target)
                    {
                        return c;
                    }
                }
            }

            return null;
        }
    }
}
