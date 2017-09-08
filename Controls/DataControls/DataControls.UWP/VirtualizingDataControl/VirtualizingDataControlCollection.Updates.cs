using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using Telerik.Core.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    public partial class RadVirtualizingDataControl
    {
        /// <summary>
        /// Gets the count of the data items in the current
        /// view.
        /// </summary>
        public virtual int ItemCount
        {
            get
            {
                return this.listSource.Count;
            }
        }

        /// <summary>
        /// Brings the control in a state in which collection updates are not handled.
        /// In this way batch update operations can be performed.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void BeginUpdate()
        {
            this.listSource.Suspend();
        }

        /// <summary>
        /// Resumes the normal state of the control in which collection updates are handled.
        /// </summary>
        /// <param name="update">True to trigger an update pass, otherwise false.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void EndUpdate(bool update)
        {
            this.listSource.Resume(update);
        }

        /// <summary>
        /// Called after the core ItemsSource changed logic has been executed.
        /// </summary>
        internal override void OnAfterItemsSourceChanged(IEnumerable oldSource)
        {
        }

        /// <summary>
        /// Called before the core ItemsSource changed logic is executed.
        /// </summary>
        internal override void OnBeforeItemsSourceChanged(IEnumerable oldSource)
        {
            base.OnBeforeItemsSourceChanged(oldSource);

            IEnumerable newValue = this.ItemsSource;

            if (newValue is IEnumerable)
            {
                this.ScheduleBatchItemAddedAnimation();
            }
        }

        /// <summary>
        /// Occurs when the <see cref="DataControlBase.ItemsSource" /> property has changed.
        /// </summary>
        protected override void OnItemsSourceChanged(IEnumerable oldSource)
        {
            base.OnItemsSourceChanged(oldSource);

            IEnumerable newValue = this.ItemsSource;

            this.listSource.SourceCollection = newValue;

            if (newValue == null)
            {
                this.ClearReycledItems();
            }
        }

        /// <summary>Called when the value of the <see cref="P:System.Windows.Controls.ItemsControl.Items"/>
        /// property changes.</summary>
        /// <param name="e">A <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs"/>
        /// that contains the event data.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.virtualizationStrategy != null)
            {
                this.UpdateViewportOnItemsChange(e);
            }

            if (this.ItemsChanged != null)
            {
                this.ItemsChanged(this, e);
            }

            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs("ItemCount"));
            }
        }

        private void UpdateViewportOnItemsChange(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.virtualizationStrategy.RefreshViewportOnItemAdded(e.NewItems[0] as IDataSourceItem);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    this.virtualizationStrategy.RefreshViewportOnItemRemoved(e.OldStartingIndex, e.OldItems[0] as IDataSourceItem);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    this.virtualizationStrategy.RefreshViewportOnItemReplaced(e.NewItems[0] as IDataSourceItem);
                    break;

                case NotifyCollectionChangedAction.Move:
                    this.virtualizationStrategy.RefreshViewportOnItemRemoved(e.OldStartingIndex, e.OldItems[0] as IDataSourceItem);
                    this.virtualizationStrategy.RefreshViewportOnItemAdded(e.NewItems[0] as IDataSourceItem);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.virtualizationStrategy.OnSourceCollectionReset();
                    this.OnCollectionReset();
                    break;
            }
        }

        private bool ScheduleBatchItemRemovedAnimation()
        {
            bool areAnimatableItemsInViewport = false;

            foreach (RadVirtualizingDataControlItem item in this.realizedItems)
            {
                areAnimatableItemsInViewport = this.CanPlayAnimationForItem(item, false);

                if (areAnimatableItemsInViewport)
                {
                    break;
                }
            }

            return this.itemRemovedBatchAnimationScheduled = areAnimatableItemsInViewport &&
                                                                (this.itemAnimationModeCache & ItemAnimationMode.PlayOnSourceReset) != 0 &&
                                                                this.itemRemovedAnimationCache != null &&
                                                                this.realizedItems.Count > 0;
        }

        private void ScheduleBatchItemAddedAnimation()
        {
            this.itemAddedBatchAnimationScheduled = (this.itemAnimationModeCache & ItemAnimationMode.PlayOnNewSource) != 0 && this.itemAddedAnimationCache != null;
        }

        private void OnCollectionReset()
        {
            // When the collection is reset we check whether a batch
            // item remove animation has been scheduled already
            if (this.itemRemovedBatchAnimationScheduled)
            {
                // ... if yes we check whether a new data source with items has been set
                if (this.GetItemCount() > 0)
                {
                    // ... if yes, we stop all previously started remove animations.
                    // When the last started animation is stopped, a balance will be scheduled
                    // and item add animation will be started if defined.
                    this.StopAllRemovedAnimations();
                }
                return;
            }

            // In case there is not batch remove animation started we stop all currently running add animations...
            this.StopAllRemovedAnimations();
            this.StopAllAddedAnimations();

            // And try to schedule bath remove animation.
            // Remove animation can be scheduled when there are realized items and defined remove animation.
            if (this.ScheduleBatchItemRemovedAnimation())
            {
                this.OnItemRemovedBatchAnimation();
                Array currentRealizedItems = this.realizedItems.ToArray();
                foreach (RadVirtualizingDataControlItem item in currentRealizedItems)
                {
                    if (this.CanPlayAnimationForItem(item, false))
                    {
                        this.ClearContainerForItemOverride(item, item.associatedDataItem);
                    }
                }
            }
            else
            {
                // If we can't schedule remove animation, we simply stop all remove animations and perform the standard OnCollectionReset logic.
                this.CleanupAfterCollectionReset();
            }
        }

        private void CleanupAfterCollectionReset()
        {
            this.itemRemovedBatchAnimationScheduled = false;
            this.EndAsyncBalance();

            this.RecycleAllItems();

            this.ResetScrollViewer();

            if (this.IsOperational())
            {
                this.BeginAsyncBalance();
                this.BalanceVisualSpace();
            }
        }

        private void ResetScrollViewer()
        {
            if (this.IsProperlyTemplated)
            {
                this.manipulationContainer.ChangeView(0, 0, null);
                Canvas.SetTop(this.itemsPanel, 0);
                Canvas.SetLeft(this.itemsPanel, 0);
                this.manipulationContainer.ClearValue(FrameworkElement.HeightProperty);
                this.scrollableContent.Height = VirtualizationStrategy.MaxScrollableLength;
                this.scrollableContent.Width = VirtualizationStrategy.MaxScrollableLength;
                this.manipulationContainer.UpdateLayout();
            }
        }

        private void OnListSource_CurrentItemChanged(object sender, EventArgs e)
        {
            this.OnListSourceCurrentItemChanged();
        }

        private void OnListSource_CurrentItemChanging(object sender, CurrentItemChangingEventArgs e)
        {
            this.OnListSourceCurrentItemChanging();
        }
    }
}
