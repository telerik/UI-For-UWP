using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Windows.UI.Xaml.Data;

namespace Telerik.Core.Data
{
    /// <summary>
    /// Implements a simple list-based data source that provides currency management and implements the <see cref="System.Collections.Specialized.INotifyCollectionChanged"/> interface.
    /// </summary>
    internal partial class RadListSource : DisposableObject, IEnumerable, ICollection, INotifyCollectionChanged, ICurrencyManager, IWeakEventListener
    {
        internal static readonly object UnsetObject = new object();

        private const uint SettingSourceStateKey = DisposableObjectStateKey << 1;
        private const uint RefreshingStateKey = SettingSourceStateKey << 1;
        private const uint UpdatingCurrentStateKey = RefreshingStateKey << 1;

        private IEnumerable sourceCollection;
        private ICollectionView sourceCollectionAsCollectionView;
        private List<IDataSourceItem> items;

        private WeakEventHandler<NotifyCollectionChangedEventArgs> collectionChangedHandler;
        private WeakEventHandler<EventArgs> currentChangedHandler;

        private IDataSourceItemFactory itemFactory;

        private NotifyCollectionChangedEventHandler collectionChangedEvent;
        private EventHandler<ItemPropertyChangedEventArgs> itemPropertyChangedChangedEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadListSource"/> class.
        /// </summary>
        public RadListSource()
        {
            this.itemFactory = new DataSourceItemFactory();
            this.currencyMode = CurrencyManagementMode.LocalAndExternal;

            this.items = new List<IDataSourceItem>();
        }

        ~RadListSource()
        {
            if (this.collectionChangedHandler != null)
            {
                this.collectionChangedHandler.Unsubscribe();
            }

            if (this.currentChangedHandler != null)
            {
                this.currentChangedHandler.Unsubscribe();
            }

            this.UnhookPropertyChanged();
        }

        /// <summary>
        /// Raised when an internal change in the collection has occurred.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                this.collectionChangedEvent += value;
            }
            remove
            {
                this.collectionChangedEvent -= value;
            }
        }

        /// <summary>
        /// Raised when a change in a single item property has occurred.
        /// </summary>
        public event EventHandler<ItemPropertyChangedEventArgs> ItemPropertyChanged
        {
            add
            {
                this.itemPropertyChangedChangedEvent += value;
            }
            remove
            {
                this.itemPropertyChangedChangedEvent -= value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the source is in a process of refreshing its items.
        /// </summary>
        public bool IsRefreshing
        {
            get
            {
                return this[RefreshingStateKey];
            }
        }

        /// <summary>
        /// Gets a value indicating whether raw data initialization is running.
        /// </summary>
        public bool IsSettingSource
        {
            get
            {
                return this[SettingSourceStateKey];
            }
        }

        /// <summary>
        /// Gets or sets the raw data associated with this data source.
        /// </summary>
        public IEnumerable SourceCollection
        {
            get
            {
                return this.sourceCollection;
            }
            set
            {
                if (this.sourceCollection != value)
                {
                    this.SetSource(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IDataSourceItemFactory"/> instance used to create <see cref="IDataSourceItem"/> instances, stored in this data source.
        /// </summary>
        public IDataSourceItemFactory ItemFactory
        {
            get
            {
                return this.itemFactory;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                this.itemFactory = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets the count of the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return this.items.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return null;
            }
        }

        internal BatchLoadingProvider<IDataSourceItem> BatchDataProvider { get; set; }

        /// <summary>
        /// Gets a value indicating whether the source collection.
        /// </summary>
        internal virtual bool SupportsItemReorder
        {
            get
            {
                if (this.sourceCollection == null)
                {
                    return false;
                }

                return (this.sourceCollection is IList) && !(this.sourceCollection as IList).IsReadOnly;
            }
        }

        /// <summary>
        /// Gets the list with all data source items.
        /// </summary>
        protected List<IDataSourceItem> Items
        {
            get
            {
                return this.items;
            }
        }

        /// <summary>
        /// Gets the IEnumerable instance to be used when building inner view models (<see cref="IDataSourceItem"/> instances).
        /// </summary>
        protected virtual IEnumerable DataItemsSource
        {
            get
            {
                return this.sourceCollection;
            }
        }

        /// <summary>
        /// Gets the source collection as ICollectionView instance.
        /// </summary>
        protected ICollectionView OriginalCollectionView
        {
            get
            {
                return this.sourceCollectionAsCollectionView;
            }
        }

        /// <summary>
        /// Re-evaluates the current state of the source and re-builds all the data items.
        /// </summary>
        public void Refresh()
        {
            if (!this.IsSuspended && !this[RefreshingStateKey])
            {
                this[RefreshingStateKey] = true;
                this.RefreshOverride();
                this[RefreshingStateKey] = false;
            }
        }

        /// <summary>
        /// Retrieves the IDataSourceItem that wraps the specified data object.
        /// </summary>
        /// <param name="data">The raw data object to search for.</param>
        public IDataSourceItem FindItem(object data)
        {
            IDataSourceItem currentItem = this.GetFirstItem();
            while (currentItem != null)
            {
                if (object.Equals(currentItem.Value, data))
                {
                    return currentItem;
                }

                currentItem = currentItem.Next;
            }

            return null;
        }

        /// <summary>
        /// Gets the <see cref="IEnumerator"/> instance used to iterate through currently available data items.
        /// </summary>
        public virtual IEnumerator GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        void IWeakEventListener.ReceiveEvent(object sender, object e)
        {
            if (e is NotifyCollectionChangedEventArgs)
            {
                this.OnSourceCollectionChanged(sender, e as NotifyCollectionChangedEventArgs);
            }
            else if (e is PropertyChangedEventArgs)
            {
                this.OnItemPropertyChanged(sender, e as PropertyChangedEventArgs);
            }
            else
            {
                this.OnSourceCollectionCurrentChanged(sender, (EventArgs)e);
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        internal void RequestData(int lastRequestedIndex)
        {
            this.BatchDataProvider.RequestItems(lastRequestedIndex, 0);
        }

        internal bool CanShiftItemIndexDown(IDataSourceItem item)
        {
            return item.Index > 0;
        }

        internal bool CanShiftItemIndexUp(IDataSourceItem item)
        {
            return item.Index < this.Count - 1;
        }

        internal void ShiftItemIndexUp(IDataSourceItem item)
        {
            if (!this.SupportsItemReorder)
            {
                throw new InvalidOperationException("The provided source collection does not support item reorder");
            }

            int currentItemIndex = item.Index;

            if (currentItemIndex == this.Count - 1)
            {
                throw new InvalidOperationException("The position of the item cannot be changed as it is at the end of the source collection.");
            }

            ////TODO: move data item in original source as well
            this.Suspend();

            IList sourceCollection = this.sourceCollection as IList;

            sourceCollection.RemoveAt(currentItemIndex);
            sourceCollection.Insert(currentItemIndex + 1, item.Value);

            this.items.RemoveAt(currentItemIndex);
            this.items.Insert(currentItemIndex + 1, item);

            IDataSourceItem nextItem = item.Next;

            item.Next = nextItem.Next;

            if (nextItem.Next != null)
            {
                nextItem.Next.Previous = item;
            }

            nextItem.Next = item;

            if (item.Previous != null)
            {
                item.Previous.Next = nextItem;
            }

            nextItem.Previous = item.Previous;
            item.Previous = nextItem;

            item.Index += 1;
            item.Previous.Index -= 1;

            this.Resume(false);
        }

        internal void ShiftItemIndexDown(IDataSourceItem item)
        {
            if (!this.SupportsItemReorder)
            {
                throw new InvalidOperationException("The provided source collection does not support item reorder");
            }

            int currentItemIndex = item.Index;

            if (currentItemIndex == 0)
            {
                throw new InvalidOperationException("The position of the item cannot be changed as it is at the end of the source collection.");
            }

            ////TODO: move data item in original source as well
            this.Suspend();

            IList sourceCollection = this.sourceCollection as IList;
            sourceCollection.RemoveAt(currentItemIndex);
            sourceCollection.Insert(currentItemIndex - 1, item.Value);

            this.items.RemoveAt(currentItemIndex);
            this.items.Insert(currentItemIndex - 1, item);

            IDataSourceItem prevItem = item.Previous;

            prevItem.Next = item.Next;

            if (item.Next != null)
            {
                item.Next.Previous = prevItem;
            }

            item.Next = prevItem;

            if (prevItem.Previous != null)
            {
                prevItem.Previous.Next = item;
            }

            item.Previous = prevItem.Previous;
            prevItem.Previous = item;

            item.Index -= 1;
            item.Next.Index += 1;

            this.Resume(false);
        }

        internal object RequestDataForItem(IDataSourceItem item)
        {
            IList source = this.sourceCollection as IList;

            if (source == null)
            {
                return null;
            }

            // item might have already been removed from the collection, while its index is still not updated
            // so, check for index bounds
            if (item.Index < source.Count)
            {
                return source[item.Index];
            }

            return RadListSource.UnsetObject;
        }

        internal virtual void BuildDataItems()
        {
            // create IDataSourceItem instance for each raw data object
            IDataSourceItem previous = null;
            int index = 0;

            IEnumerable itemsSource = this.DataItemsSource;
            IDataSourceItem newCurrentItem = null;

            if (itemsSource != null)
            {
                foreach (object dataItem in itemsSource)
                {
                    IDataSourceItem dataSourceItem = this.CreateNewOrGetCurrent(dataItem, ref newCurrentItem);

                    // update the view model with index and prev/next references
                    dataSourceItem.Index = index;
                    if (previous != null)
                    {
                        dataSourceItem.Previous = previous;
                        previous.Next = dataSourceItem;
                    }

                    this.items.Add(dataSourceItem);

                    previous = dataSourceItem;

                    if (this.currencyMode != CurrencyManagementMode.None)
                    {
                        if (this.sourceCollectionAsCollectionView != null &&
                            object.Equals(this.sourceCollectionAsCollectionView.CurrentItem, dataItem))
                        {
                            newCurrentItem = dataSourceItem;
                        }
                    }

                    index++;
                }
            }

            this.UpdateCurrentItem(newCurrentItem);
        }

        /// <summary>
        /// Performs the core refresh logic. Allows inheritors to specify some additional logic or to completely override the existing one.
        /// </summary>
        protected virtual void RefreshOverride()
        {
            this.previousCurrentItem = this.currentItem;
            this.currentItem = null;

            this.UnhookPropertyChanged();

            // core data items creation
            this.items.Clear();
            this.BuildDataItems();

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            // check whether current item actually changed
            if (this.previousCurrentItem != this.currentItem)
            {
                this.OnCurrentItemChanged(EventArgs.Empty);
            }
            this.previousCurrentItem = null;
        }

        /// <summary>
        /// Allows inheritors to perform some additional logic upon attaching to raw data.
        /// </summary>
        protected virtual void AttachDataOverride()
        {
        }

        /// <summary>
        /// Allows inheritors to perform some additional logic upon detaching to raw data.
        /// </summary>
        protected virtual void DetachDataOverride()
        {
        }

        /// <summary>
        /// Raises the <see cref="CollectionChanged"/> event.
        /// </summary>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = this.collectionChangedEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Allows inheritors to perform some additional logic upon change in a single item's property.
        /// </summary>
        protected virtual void ItemPropertyChangedOverride(ItemPropertyChangedEventArgs args)
        {
        }

        /// <summary>
        /// Updates the current instance after a change in the raw data.
        /// Allows inheritors to provide additional logic upon the change.
        /// </summary>
        protected virtual void SourceCollectionChangedOverride(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    this.Refresh();
                    break;
                case NotifyCollectionChangedAction.Add:
                    this.AddItems(e);

                    // raise CollectionChanged with proper arguments
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this.items[e.NewStartingIndex], e.NewStartingIndex));
                    break;
                case NotifyCollectionChangedAction.Remove:

                    // get the corresponding RemovedItems before they are actually updated
                    IDataSourceItem removedItem = this.items[e.OldStartingIndex];

                    this.RemoveItems(e);

                    // raise the CollectionChanged event
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, e.OldStartingIndex));

                    this.CheckCurrentRemoved(removedItem);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.ReplaceItems(e);

                    // raise the CollectionChanged event
                    IDataSourceItem replacedItem = this.items[e.NewStartingIndex];
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, replacedItem, e.OldItems, e.NewStartingIndex));

                    this.CheckCurrentRemoved(replacedItem);
                    break;
                case NotifyCollectionChangedAction.Move:
                    IDataSourceItem affectedItem = this.items[e.OldStartingIndex];
                    this.MoveItems(e);
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, affectedItem, e.NewStartingIndex, e.OldStartingIndex));
                    break;
            }
        }

        /// <summary>
        /// Moves the specified item from its old index to its new index after a change of type Move
        /// has occurred in the original collection.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance that comes with the event.</param>
        protected virtual void MoveItems(NotifyCollectionChangedEventArgs e)
        {
            int insertIndex = e.NewStartingIndex;
            int previousIndex = e.OldStartingIndex;

            IDataSourceItem itemToMove = this.items[previousIndex];
            this.items.RemoveAt(e.OldStartingIndex);
            this.items.Insert(insertIndex, itemToMove);

            // update the moved item's index at its new position
            itemToMove.Index = insertIndex;

            IDataSourceItem newItemAtOldIndex = this.items[previousIndex];
            newItemAtOldIndex.Next = null;
            newItemAtOldIndex.Previous = null;

            if (previousIndex > 0)
            {
                IDataSourceItem neighbourToNewItemAtOldIndex = this.items[previousIndex - 1];
                neighbourToNewItemAtOldIndex.Next = newItemAtOldIndex;
                newItemAtOldIndex.Previous = neighbourToNewItemAtOldIndex;
            }

            if (previousIndex < this.Count - 1)
            {
                IDataSourceItem neighbourToNewItemAtOldIndex = this.items[previousIndex + 1];
                neighbourToNewItemAtOldIndex.Previous = newItemAtOldIndex;
                newItemAtOldIndex.Next = neighbourToNewItemAtOldIndex;
            }

            IDataSourceItem oldItemAtNewIndex = this.items[insertIndex];
            oldItemAtNewIndex.Previous = null;
            oldItemAtNewIndex.Next = null;

            if (insertIndex < this.Count - 1)
            {
                IDataSourceItem rightNeighbour = this.items[insertIndex + 1];
                oldItemAtNewIndex.Next = rightNeighbour;
                rightNeighbour.Previous = oldItemAtNewIndex;
            }

            if (insertIndex > 0)
            {
                IDataSourceItem leftNeighbour = this.items[insertIndex - 1];
                oldItemAtNewIndex.Previous = leftNeighbour;
                leftNeighbour.Next = oldItemAtNewIndex;
            }

            // Update the indices of all items before the new position by reducing them with 1 as the
            // item moved shifts all before it down with 1 index.
            for (int i = insertIndex - 1; i > previousIndex - 1; i--)
            {
                this.items[i].Index -= 1;
            }
        }

        /// <summary>
        /// Adds new items to the source after a change of type Add has occurred in the original collection.
        /// </summary>
        protected virtual void AddItems(NotifyCollectionChangedEventArgs e)
        {
            int insertIndex = e.NewStartingIndex;
            IDataSourceItem previous = null;
            if (insertIndex > 0)
            {
                previous = this.items[insertIndex - 1];
            }

            for (int i = 0; i < e.NewItems.Count; i++)
            {
                IDataSourceItem dataItem = this.itemFactory.CreateItem(this, e.NewItems[i]);
                dataItem.Index = insertIndex;
                dataItem.Previous = previous;
                if (previous != null)
                {
                    previous.Next = dataItem;
                }

                this.items.Insert(insertIndex++, dataItem);

                previous = dataItem;
            }

            // update items' indexes
            this.ShiftIndexes(e.NewStartingIndex + e.NewItems.Count, e.NewItems.Count);

            // update previous/next references
            if (insertIndex < this.items.Count)
            {
                IDataSourceItem next = this.items[insertIndex];
                next.Previous = previous;
                if (previous != null)
                {
                    previous.Next = next;
                }
            }
        }

        /// <summary>
        /// Updates data items after a change of type Replace has occurred in the original collection. 
        /// </summary>
        protected virtual void ReplaceItems(NotifyCollectionChangedEventArgs e)
        {
            // replace old values with the new ones
            int startIndex = e.NewStartingIndex;
            for (int i = 0; i < e.NewItems.Count; i++)
            {
                IDataSourceItem dataItem = this.items[startIndex++];

                // remove propertychanged subscription for old value
                (dataItem as DataSourceItem).UnhookPropertyChanged();

                // change value and hook propertychanged notification
                dataItem.ChangeValue(e.NewItems[i]);
                (dataItem as DataSourceItem).HookPropertyChanged();
            }
        }

        /// <summary>
        /// Removes data items from the source after a change of type Remove has occurred in the original collection.
        /// </summary>
        protected virtual void RemoveItems(NotifyCollectionChangedEventArgs e)
        {
            for (int i = 0; i < e.OldItems.Count; i++)
            {
                IDataSourceItem dataItem = this.items[e.OldStartingIndex];

                (dataItem as DataSourceItem).UnhookPropertyChanged();
                dataItem.Next = null;
                dataItem.Previous = null;

                this.items.RemoveAt(e.OldStartingIndex);
            }

            // update items' indexes
            this.ShiftIndexes(e.OldStartingIndex, -e.OldItems.Count);

            // update previous/next references
            IDataSourceItem previous = null;
            if (e.OldStartingIndex > 0)
            {
                previous = this.items[e.OldStartingIndex - 1];
            }
            IDataSourceItem next = null;
            if (e.OldStartingIndex < this.items.Count)
            {
                next = this.items[e.OldStartingIndex];
            }

            if (next != null)
            {
                next.Previous = previous;
            }
            if (previous != null)
            {
                previous.Next = next;
            }
        }

        /// <summary>
        /// Gets all the events, exposed by this instance. Used to clean-up event subscriptions upon disposal.
        /// </summary>
        protected override void CollectEvents(List<Delegate> events)
        {
            base.CollectEvents(events);

            events.Add(this.collectionChangedEvent);
            events.Add(this.itemPropertyChangedChangedEvent);
            events.Add(this.currentItemChangingEvent);
            events.Add(this.currentItemChangedEvent);
        }

        /// <summary>
        /// Notifies that this instance is no longer suspended.
        /// Allows inheritors to provide their own update logic.
        /// </summary>
        /// <param name="update">True if an Update is requested, false otherwise.</param>
        protected override void OnResumed(bool update)
        {
            base.OnResumed(update);

            if (update)
            {
                this.Refresh();
            }
        }

        private void InitializeBatchDataLoader(ISupportIncrementalLoading incrementalLoadingSource)
        {
            if (incrementalLoadingSource != null)
            {
                this.BatchDataProvider = new BatchLoadingProvider<IDataSourceItem>(incrementalLoadingSource, this.items);
            }
        }

        private void UnhookPropertyChanged()
        {
            if (this.items.Count == 0)
            {
                return;
            }

            DataSourceItem firstItem = this.items[0] as DataSourceItem;
            while (firstItem != null)
            {
                firstItem.UnhookPropertyChanged();
                firstItem = firstItem.Next as DataSourceItem;
            }
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsSuspended)
            {
                return;
            }

            ItemPropertyChangedEventArgs args = new ItemPropertyChangedEventArgs(sender, e.PropertyName);
            this.ItemPropertyChangedOverride(args);

            // raise the event
            if (this.itemPropertyChangedChangedEvent != null)
            {
                this.itemPropertyChangedChangedEvent(this, args);
            }
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.itemFactory.OnOwningListSourceCollectionChanged(e);

            if (!this.IsSuspended)
            {
                this.SourceCollectionChangedOverride(e);
            }
        }

        private void AttachData(IEnumerable data)
        {
            if (this.sourceCollection != null)
            {
                this.DetachData(this.sourceCollection);
            }

            this.sourceCollection = data;

            if (this.sourceCollection != null)
            {
                INotifyCollectionChanged collectionChanged = data as INotifyCollectionChanged;
                if (collectionChanged != null)
                {
                    this.collectionChangedHandler = new WeakEventHandler<NotifyCollectionChangedEventArgs>(collectionChanged, this, KnownEvents.CollectionChanged);
                }

                this.sourceCollectionAsCollectionView = data as ICollectionView;
                if (this.sourceCollectionAsCollectionView != null)
                {
                    this.currentChangedHandler = new WeakEventHandler<EventArgs>(this.sourceCollectionAsCollectionView, this, KnownEvents.VectorChanged);
                }
            }

            this.InitializeBatchDataLoader(this.sourceCollection as ISupportIncrementalLoading);

            this.AttachDataOverride();
            this.Refresh();
        }

        private void DetachData(IEnumerable data)
        {
            if (this.collectionChangedHandler != null)
            {
                this.collectionChangedHandler.Unsubscribe();
                this.collectionChangedHandler = null;
            }

            if (this.currentChangedHandler != null)
            {
                this.currentChangedHandler.Unsubscribe();
                this.currentChangedHandler = null;
            }

            if (this.BatchDataProvider != null)
            {
                this.BatchDataProvider.Dispose();
            }

            this.DetachDataOverride();
        }

        private void SetSource(IEnumerable value)
        {
            this[SettingSourceStateKey] = true;

            // reset current position
            this.currentItem = null;

            // rebuild data
            this.AttachData(value);

            this[SettingSourceStateKey] = false;
        }

        private void ShiftIndexes(int start, int offset)
        {
            // shift indexes
            int count = this.items.Count;
            for (int i = start; i < count; i++)
            {
                this.items[i].Index += offset;
            }
        }
    }
}