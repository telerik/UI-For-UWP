using System;
using System.Collections.Generic;
using System.ComponentModel;
using Telerik.Core.Data;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// This partial class holds the logic of the UI virtualization mechanism.
    /// </summary>
    public partial class RadVirtualizingDataControl
    {
        internal RadVirtualizingDataControlItem firstItemCache;
        internal RadVirtualizingDataControlItem lastItemCache;
        internal Panel itemsPanel;
        internal Canvas scrollableContent;
        internal ScrollViewer manipulationContainer;
        internal ScrollContentPresenter scrollContentPresenter;
        internal IDataSourceItem initialVirtualizationItem = null;
        internal List<RadVirtualizingDataControlItem> realizedItems;
        internal Queue<RadVirtualizingDataControlItem> recycledItems;
        internal bool useAsyncBalance;
        internal AsyncBalanceMode asyncBalanceMode;
        internal VirtualizationStrategy virtualizationStrategy;
        internal VirtualizationStrategyDefinition virtualizationStrategyDefinition;
        internal double realizedItemsBufferScale = 2.0;

        private bool handlesRendering;
        private bool enableAsyncBalance;
        private bool waitingForBalance;

        /// <summary>
        /// Gets or sets the relative size of the UI virtualization buffers to the height of the
        /// viewport. The size of the UI virtualization buffers is defined by multiplying
        /// the height of the viewport by the value defined on this property. If the minimum value
        /// of the UI virtualization buffers according to this scale falls below 800 pixels,
        /// the value of 800 pixels is used as a value.
        /// </summary>
        public double RealizedItemsBufferScale
        {
            get
            {
                return this.realizedItemsBufferScale;
            }
            set
            {
                if (value < 1.0 || value > 4.0)
                {
                    throw new ArgumentException("The realized items buffer scale must be in the range between 1.0 and 4.0");
                }

                if (this.realizedItemsBufferScale != value)
                {
                    this.realizedItemsBufferScale = value;
                    if (this.IsOperational())
                    {
                        this.virtualizationStrategy.ResetRealizedItemsBuffers();
                        this.BalanceVisualSpace();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that determines how the asynchronous balance is performed. The
        /// <see cref="Telerik.UI.Xaml.Controls.Data.AsyncBalanceMode.Standard"/> mode implies that each visual container is realized
        /// asynchronously, whereas the <see cref="Telerik.UI.Xaml.Controls.Data.AsyncBalanceMode.FillViewportFirst"/> implies
        /// that the viewport is filled synchronously after which an asynchronous balance is performed
        /// for the rest of the items.
        /// </summary>
        [DefaultValue(typeof(AsyncBalanceMode), "Standard")]
        public AsyncBalanceMode AsyncBalanceMode
        {
            get
            {
                return this.asyncBalanceMode;
            }
            set
            {
                this.asyncBalanceMode = value;
            }
        }

        internal virtual RadVirtualizingDataControlItem LastRealizedDataItem
        {
            get
            {
                return this.lastItemCache;
            }
        }

        internal virtual RadVirtualizingDataControlItem FirstRealizedDataItem
        {
            get
            {
                return this.firstItemCache;
            }
        }

        /// <summary>
        /// Gets the realized items length. Internal property used for testing purposes only.
        /// </summary>
        internal double RealizedItemsHeight
        {
            get
            {
                return this.virtualizationStrategy.realizedItemsLength;
            }
        }

        /// <summary>
        /// Gets the average item height. Internal property used for testing purposes only.
        /// </summary>
        internal double AverageItemHeight
        {
            get
            {
                return this.virtualizationStrategy.averageItemLength;
            }
        }

        /// <summary>
        /// Marks the next Balance to come as asynchronous.
        /// </summary>
        public void BeginAsyncBalance()
        {
            if (!this.enableAsyncBalance)
            {
                return;
            }

            this.useAsyncBalance = true;
        }

        /// <summary>
        /// Gets an instance of the <see cref="RadVirtualizingDataControlItem"/> class that represents
        /// the visual container for the given data item.
        /// </summary>
        /// <param name="item">The data item for which to get the container.</param>
        /// <returns>The container if found, otherwise null.</returns>
        public RadVirtualizingDataControlItem GetContainerForItem(object item)
        {
            foreach (RadVirtualizingDataControlItem container in this.realizedItems)
            {
                if (object.Equals(container.associatedDataItem.Value, item))
                {
                    return container;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets an instance of the <see cref="RadVirtualizingDataControlItem"/> class that represents
        /// the visual container for the item at the given index.
        /// </summary>
        /// <param name="index">The index of the data item to get the container for.</param>
        /// <returns>The container if found, otherwise null.</returns>
        public RadVirtualizingDataControlItem GetContainerFromIndex(int index)
        {
            IDataSourceItem itemAtIndex = this.listSource.GetItemAt(index);

            return this.GetContainerFromDataSourceItem(itemAtIndex);
        }

        /// <summary>
        /// Determines whether the specified data item is currently visible.
        /// </summary>
        /// <param name="dataItem">The data item.</param>
        /// <param name="includePartiallyVisibleItems">
        /// If this parameter is set to <c>true</c>, then the partially visible items will also be included.
        /// </param>
        public bool IsItemInViewport(object dataItem, bool includePartiallyVisibleItems = true)
        {
            if (!this.IsOperational())
            {
                return false;
            }

            RadVirtualizingDataControlItem container = this.virtualizationStrategy.GetTopVisibleContainer();
            int iterationStart = container.associatedDataItem.Index - this.firstItemCache.associatedDataItem.Index;
            double bottomViewportEdge = this.virtualizationStrategy.ViewportLength + this.virtualizationStrategy.ScrollOffset;

            for (int i = iterationStart; i < this.realizedItems.Count; i++)
            {
                container = this.realizedItems[i];
                var itemTop = this.virtualizationStrategy.GetItemRelativeOffset(container);
                if (itemTop < bottomViewportEdge)
                {
                    if (!includePartiallyVisibleItems && (itemTop < this.virtualizationStrategy.ScrollOffset || itemTop + this.virtualizationStrategy.GetItemLength(container) > bottomViewportEdge))
                    {
                        continue;
                    }

                    if (dataItem.Equals(container.associatedDataItem.Value) && container.Visibility == Visibility.Visible)
                    {
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }

            return false;
        }

        internal void SubscribeRendering()
        {
            if (this.handlesRendering)
            {
                return;
            }

            CompositionTarget.Rendering += this.OnCompositionTarget_Rendering;
            this.handlesRendering = true;
        }

        internal void UnsubscribeRendering()
        {
            if (!this.handlesRendering)
            {
                return;
            }

            CompositionTarget.Rendering -= this.OnCompositionTarget_Rendering;
            this.handlesRendering = false;
        }

        internal bool IsContainerInViewport(RadVirtualizingDataControlItem container)
        {
            double relativeOffset = this.virtualizationStrategy.GetItemRelativeOffset(container);
            return relativeOffset < this.virtualizationStrategy.ViewportLength && relativeOffset + this.virtualizationStrategy.GetItemLength(container) > 0;
        }

        internal RadVirtualizingDataControlItem GetContainerFromDataSourceItem(IDataSourceItem item)
        {
            if (this.realizedItems.Count == 0)
            {
                return null;
            }

            RadVirtualizingDataControlItem container = null;

            int firstRealizedIndex = this.GetFirstItemCacheIndex();
            int lastRealizedIndex = this.GetLastItemCacheIndex();
            int itemIndex = item.Index;

            if (itemIndex >= firstRealizedIndex && itemIndex <= lastRealizedIndex)
            {
                int realizedIndex = this.GetItemRealizedIndexFromListSourceIndex(itemIndex, firstRealizedIndex);

                container = this.realizedItems[realizedIndex];
            }

            return container;
        }

        /// <summary>
        /// Returns the first data item from the list source. When overridden in inheriting classes
        /// allows for prepending data items to the list which are not part of the actual data list.
        /// </summary>
        internal virtual IDataSourceItem GetFirstItem()
        {
            return this.listSource.GetFirstItem();
        }

        /// <summary>
        /// Gets the data item that comes after the given one in the data list.
        /// When overridden in inheriting classes can be used to append data items
        /// to the list which are not part of the actual data list.
        /// </summary>
        internal virtual IDataSourceItem GetItemAfter(IDataSourceItem item)
        {
            return this.listSource.GetItemAfter(this.lastItemCache.associatedDataItem);
        }

        /// <summary>
        /// Gets the data item that comes before the given one in the data list.
        /// When overridden in inheriting classes can be used to prepend data items
        /// to the list which are not part of the actual data list.
        /// </summary>
        internal virtual IDataSourceItem GetItemBefore(IDataSourceItem item)
        {
            return this.listSource.GetItemBefore(item);
        }

        /// <summary>
        /// Fired when the height of the scrollable content has been
        /// adjusted so that it matches the bottom edge of the last realized item.
        /// </summary>
        internal virtual void OnBottomEdgeCorrected()
        {
        }

        internal double GetCurrentScrollOffset(bool usePrediction)
        {
            return this.manipulationContainer.VerticalOffset;
        }

        internal void PrepareDataItem(RadVirtualizingDataControlItem element, IDataSourceItem dataItem)
        {
            if (this.itemTemplateSelectorCache != null)
            {
                DataTemplate template = this.itemTemplateSelectorCache.SelectTemplate(dataItem.Value, element);
                if (template != null)
                {
                    element.Content = dataItem.Value;
                    element.ContentTemplate = template;
                    return;
                }
            }

            if (this.itemTemplateCache != null)
            {
                element.Content = dataItem.Value;
                element.ContentTemplate = this.itemTemplateCache;
                return;
            }

            if (element.ContentTemplate != null)
            {
                element.ContentTemplate = null;
            }

            if (!string.IsNullOrEmpty(this.displayMemberPathCache))
            {
                if (dataItem.DisplayValue == null)
                {
                    object value = this.ReflectPropertyValueAndStoreInfoIfNeeded(ref this.displayMemberPropInfo, dataItem.Value, this.displayMemberPathCache);
                    dataItem.DisplayValue = value;
                }

                element.Content = dataItem.DisplayValue;
            }
            else
            {
                if (dataItem.Value != null)
                {
                    element.Content = dataItem.Value;
                }
            }
        }

        internal void PrepareStyle(RadVirtualizingDataControlItem element, IDataSourceItem dataItem)
        {
            if (this.itemContainerStyleCache != null)
            {
                element.Style = this.itemContainerStyleCache;
            }
            else if (this.itemContainerStyleSelectorCache != null)
            {
                element.Style = this.itemContainerStyleSelectorCache.SelectStyle(dataItem.Value, element);
            }
            else if (element.Style != null)
            {
                element.Style = null;
            }
        }

        internal void ClearReycledItems()
        {
            if (this.itemRemovedBatchAnimationScheduled)
            {
                return;
            }

            if (this.IsProperlyTemplated)
            {
                foreach (RadVirtualizingDataControlItem item in this.recycledItems)
                {
                    this.itemsPanel.Children.Remove(item);
                }
            }

            this.recycledItems.Clear();
        }

        internal void ManageViewport()
        {
            this.virtualizationStrategy.InitForMeasure();

            if (this.CanBalance(BalanceOperationType.ManageLowerViewport))
            {
                // Manages the visual items from the top edge of the viewport until the end of the viewport plus the bottom visual buffer.
                this.virtualizationStrategy.ManageLowerViewport(true);
            }

            if (this.CanBalance(BalanceOperationType.ManageUpperViewport))
            {
                // Manages the visual space above the viewport's top edge. Checks whether an item inside this buffer should be recycled or realized.
                this.virtualizationStrategy.ManageUpperViewport(true);
            }

            // update viewport
            this.virtualizationStrategy.RecalculateViewportMeasurements();

            if (this.CanBalance(BalanceOperationType.BottomBoundsCheck))
            {
                this.virtualizationStrategy.CheckBottomScrollableBounds();
            }

            if (this.CanBalance(BalanceOperationType.TopBoundsCheck))
            {
                this.virtualizationStrategy.CheckTopScrollableBounds();
            }

            this.previousScrollOffset = this.virtualizationStrategy.LayoutOrientation == Windows.UI.Xaml.Controls.Orientation.Horizontal ?
               this.manipulationContainer.HorizontalOffset :
               this.manipulationContainer.VerticalOffset;

            this.waitingForBalance = false;

            if (this.useAsyncBalance)
            {
                this.EndAsyncBalance();
            }
        }

        internal virtual bool CanBalance(BalanceOperationType operationType)
        {
            if (this.scheduledRemoveAnimations.Count == 0)
            {
                return true;
            }

            if ((operationType == BalanceOperationType.ManageLowerViewport) ||
                (operationType == BalanceOperationType.ManageUpperViewport))
            {
                return true;
            }

            if (this.firstItemCache == null)
            {
                return false;
            }

            foreach (SingleItemAnimationContext context in this.scheduledRemoveAnimations)
            {
                if (context.AssociatedItem.CurrentOffset < this.firstItemCache.CurrentOffset)
                {
                    return false;
                }

                if (context.AssociatedItem.CurrentOffset > this.lastItemCache.CurrentOffset)
                {
                    return false;
                }
            }

            return true;
        }

        internal virtual void OnNeededItemsRealized()
        {
            if ((this.itemAnimationModeCache & ItemAnimationMode.PlayOnNewSource) != 0)
            {
                this.OnItemAddedBatchAnimation();
            }
        }

        internal void BalanceVisualSpace()
        {
            if (!this.IsOperational())
            {
                return;
            }

            this.ManageViewport();

            if (this.itemAddedBatchAnimationScheduled && !this.useAsyncBalance)
            {
                var temp = this.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () =>
                    {
                        if (this.itemAddedBatchAnimationScheduled)
                        {
                            this.OnNeededItemsRealized();
                        }
                    });
            }
        }

        internal void OnContainerStateChanged(RadVirtualizingDataControlItem container, IDataSourceItem item, ItemState state)
        {
            container.UpdateItemState(state);

            if (state == ItemState.Realized)
            {
                container.Attach(this);
            }
            else if (state == ItemState.Recycled)
            {
                container.Detach();
            }

            this.OnItemStateChanged(item.Value, state);
        }

        internal void ScheduleAsyncBalance()
        {
            if (this.waitingForBalance)
            {
                return;
            }

            this.waitingForBalance = true;
        }

        internal void RecycleAllItems()
        {
            while (this.realizedItems.Count > 0)
            {
                this.RecycleFirstItem();
            }
        }

        internal void RecycleFirstItem()
        {
            this.ClearContainerForItemOverride(this.firstItemCache, this.firstItemCache.associatedDataItem);
        }

        internal void RecycleLastItem()
        {
            this.ClearContainerForItemOverride(this.lastItemCache, this.lastItemCache.associatedDataItem);
        }

        internal IDataSourceItem GetInitialVirtualizationItem()
        {
            if (this.initialVirtualizationItem != null)
            {
                // The initial virtualization item can be accessed only once after set. Upon initial access the store for it is cleared.
                IDataSourceItem itemToReturn = this.initialVirtualizationItem;
                this.initialVirtualizationItem = null;
                return itemToReturn;
            }

            return null;
        }

        internal void PrepareContainerForItemInternal(RadVirtualizingDataControlItem element, IDataSourceItem item)
        {
            this.PrepareContainerForItemOverride(element, item);
        }

        internal void ClearContainerForItemInternal(RadVirtualizingDataControlItem container, IDataSourceItem item)
        {
            this.ClearContainerForItemOverride(container, item);
        }

        internal RadVirtualizingDataControlItem GenerateContainerForItem(IDataSourceItem item)
        {
            RadVirtualizingDataControlItem cp = this.GetContainerForItemOverride();
            this.OnContainerStateChanged(cp, item, ItemState.Realizing);
            this.itemsPanel.Children.Add(cp);
            this.PrepareContainerForItemOverride(cp, item);
            return cp;
        }

        /// <summary>
        /// Called when the virtualization state of a given data item is changed.
        /// </summary>
        /// <param name="item">The data item that has an updated state.</param>
        /// <param name="state">The new state.</param>
        protected virtual void OnItemStateChanged(object item, ItemState state)
        {
            EventHandler<ItemStateChangedEventArgs> eh = this.ItemStateChanged;
            if (eh != null)
            {
                eh(this, new ItemStateChangedEventArgs() { DataItem = item, State = state });
            }
        }

        /// <summary>Creates or identifies the element that is used to display the given
        /// item.</summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected virtual RadVirtualizingDataControlItem GetContainerForItemOverride()
        {
            RadVirtualizingDataControlItem cp = new RadVirtualizingDataControlItem();
            return cp;
        }

        /// <summary>Undoes the effects of the <see cref="M:System.Windows.Controls.ItemsControl.PrepareContainerForItemOverride(RadVirtualizingDataControlItem,System.Object)"/>
        /// method.</summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item.</param>
        protected virtual void ClearContainerForItemOverride(RadVirtualizingDataControlItem element, IDataSourceItem item)
        {
            this.virtualizationStrategy.RecycleItem(element);
        }

        /// <summary>Prepares the specified element to display the specified item. </summary>
        /// <param name="element">The element used to display the specified item.</param>
        /// <param name="item">The item to display.</param>
        protected virtual void PrepareContainerForItemOverride(RadVirtualizingDataControlItem element, IDataSourceItem item)
        {
            element.BindToDataItem(item);

            this.PrepareStyle(element, item);

            if (item.Value != RadListSource.UnsetObject)
            {
                this.PrepareDataItem(element, item);
            }
        }

        private void OnCompositionTarget_Rendering(object sender, object e)
        {
        }

        private double GetBottomMostAnimatedItemPosition()
        {
            double bottom = 0;

            foreach (SingleItemAnimationContext context in this.scheduledRemoveAnimations)
            {
                double currentBottom = context.AssociatedItem.verticalOffsetCache + context.RealizedLength;
                if (currentBottom > bottom)
                {
                    bottom = currentBottom;
                }
            }

            return bottom;
        }

        private IDataSourceItem GetTopVisibleItem()
        {
            if (this.realizedItems.Count == 0)
            {
                return null;
            }

            RadVirtualizingDataControlItem currentTopMostElement = this.virtualizationStrategy.GetTopVisibleContainer();

            if (currentTopMostElement != null)
            {
                return currentTopMostElement.associatedDataItem;
            }

            return null;
        }

        private void EndAsyncBalance()
        {
            this.useAsyncBalance = false;
        }
    }
}