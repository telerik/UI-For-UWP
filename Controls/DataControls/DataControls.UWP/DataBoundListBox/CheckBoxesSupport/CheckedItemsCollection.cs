using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Telerik.Core.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents an observable collection that holds all checked items in the <see cref="RadDataBoundListBox"/> control.
    /// When items are added or removed they are respectively checked or unchecked.
    /// </summary>
    public class CheckedItemsCollection<T> : IList<T>, INotifyCollectionChanged
    {
        private RadDataBoundListBox owner;
        private List<DataSourceItem> items;

        internal CheckedItemsCollection(RadDataBoundListBox owner)
        {
            this.owner = owner;
            this.items = new List<DataSourceItem>();
        }

        /// <summary>
        /// Occurs when the items list of the collection has changed, or the collection is reset.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return this.items.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                bool isReadonly = this.owner.listSourceFactory.sourceItemCheckedPathWritable != null && !this.owner.listSourceFactory.sourceItemCheckedPathWritable.Value;

                return isReadonly;
            }
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        public T this[int index]
        {
            get
            {
                return (T)this.GetItemAt(index).Value;
            }
            [EditorBrowsable(EditorBrowsableState.Never)]
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The index of the item.</returns>
        public int IndexOf(T item)
        {
            int index = 0;
            object dataItem = this.GetDataItem(item);
            foreach (DataSourceItem listSourceItem in this.items)
            {
                if (listSourceItem.Value.Equals(dataItem))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(T item)
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("Collection cannot be modified because it is read-only. This is because the ItemCheckedPath property does not have a setter.");
            }

            if (!this.owner.OnItemCheckedStateChanging(this.GetDataItem(item), true))
            {
                DataSourceItem handledItem = item as DataSourceItem;

                if (handledItem == null)
                {
                    handledItem = this.owner.ListSource.FindItem(item) as DataSourceItem;
                }

                if (handledItem == null)
                {
                    throw new InvalidOperationException("Item is not part of the source collection.");
                }

                if (handledItem.isChecked)
                {
                    throw new InvalidOperationException("Item already checked.");
                }

                this.SetItemCheckedState(handledItem, true, true, true, CheckedItemsCollectionOperation.Add);
                this.items.Add(handledItem);
                this.OnNotifyCollectionChanged(NotifyCollectionChangedAction.Add, null, item, this.Count - 1);
                this.owner.OnItemCheckedStateChanged(item, true);
            }
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A value indicating whether the item has been successfully removed.</returns>
        public bool Remove(T item)
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("Collection cannot be modified because it is read-only. This is because the ItemCheckedPath property does not have a setter.");
            }

            if (!this.owner.OnItemCheckedStateChanging(this.GetDataItem(item), false))
            {
                DataSourceItem handledItem = item as DataSourceItem;

                if (handledItem == null)
                {
                    handledItem = this.owner.ListSource.FindItem(item) as DataSourceItem;
                }

                if (handledItem == null)
                {
                    throw new InvalidOperationException("Item is not part of the source collection.");
                }

                if (!handledItem.isChecked)
                {
                    throw new InvalidOperationException("Item is not checked.");
                }

                int removeIndex = this.items.IndexOf(handledItem);
                this.items.RemoveAt(removeIndex);
                this.SetItemCheckedState(handledItem, false, true, true, CheckedItemsCollectionOperation.Remove);
                this.OnNotifyCollectionChanged(NotifyCollectionChangedAction.Remove, item, null, removeIndex);
                this.owner.OnItemCheckedStateChanged(item, false);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("Collection cannot be modified because it is read-only. This is because the ItemCheckedPath property does not have a setter.");
            }

            for (int i = this.items.Count - 1; i > -1; i--)
            {
                DataSourceItem item = this.items[i];
                object dataItem = item.Value;
                if (!this.owner.OnItemCheckedStateChanging(dataItem, false))
                {
                    this.SetItemCheckedState(item, false, true, true, CheckedItemsCollectionOperation.Clear);
                    this.items.RemoveAt(i);
                    this.OnNotifyCollectionChanged(NotifyCollectionChangedAction.Reset, dataItem, null, i);
                    this.owner.OnItemCheckedStateChanged(item.Value, false);
                }
            }
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("Collection cannot be modified because it is read-only. This is because the ItemCheckedPath property does not have a setter.");
            }

            DataSourceItem item = this.GetItemAt(index);
            object dataValue = item.Value;

            if (!this.owner.OnItemCheckedStateChanging(dataValue, false))
            {
                this.items.RemoveAt(index);

                this.SetItemCheckedState(item, false, true, true, CheckedItemsCollectionOperation.Remove);

                this.OnNotifyCollectionChanged(NotifyCollectionChangedAction.Remove, dataValue, null, index);

                this.owner.OnItemCheckedStateChanged(dataValue, false);
            }
        }

        /// <summary>
        /// Determines whether the contains the given item.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        ///     <c>true</c> if the collection contains the specified item; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T item)
        {
            object dataItem = this.GetDataItem(item);
            foreach (DataSourceItem listSourceItem in this.items)
            {
                if (listSourceItem.Value.Equals(dataItem))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Copies all items starting from the given array index to the given destination array.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="arrayIndex">Starting index.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            int index = arrayIndex;
            int thisItemsCount = this.items.Count;
            while (index < thisItemsCount)
            {
                array[index - arrayIndex] = (T)this.items[index].Value;
                index++;
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (DataSourceItem item in this.items)
            {
                yield return (T)item.Value;
            }
        }

        /// <summary>
        /// Checks all unchecked items from the data source bound to the current <see cref="RadDataBoundListBox"/> instance.
        /// </summary>
        public void CheckAll()
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("Collection cannot be modified because it is read-only. This is because the ItemCheckedPath property does not have a setter.");
            }

            DataSourceItem item = this.owner.ListSource.GetFirstItem() as DataSourceItem;

            while (item != null)
            {
                this.CheckItem(item);
                item = item.Next as DataSourceItem;
            }
        }

        /// <summary>
        /// Checks a range of items from the data source bound to the current <see cref="RadDataBoundListBox"/> instance.
        /// </summary>
        /// <param name="start">The start index of the item range.</param>
        /// <param name="count">The count of items to check.</param>
        public void CheckRange(int start, int count)
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("Collection cannot be modified because it is read-only. This is because the ItemCheckedPath property does not have a setter.");
            }

            DataSourceItem item = this.owner.ListSource.GetItemAt(start) as DataSourceItem;
            int i = 0;

            while (item != null && i < count)
            {
                this.CheckItem(item);
                i++;
                item = item.Next as DataSourceItem;
            }
        }

        /// <summary>
        /// Unchecks a range of items from the data source bound to the current <see cref="RadDataBoundListBox"/> instance.
        /// </summary>
        /// <param name="start">The start index of the item range.</param>
        /// <param name="count">The count of items to uncheck.</param>
        public void UncheckRange(int start, int count)
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("Collection cannot be modified because it is read-only. This is because the ItemCheckedPath property does not have a setter.");
            }

            DataSourceItem item = this.owner.ListSource.GetItemAt(start) as DataSourceItem;
            int i = 0;

            while (item != null && i < count)
            {
                this.UncheckItem(item);
                i++;
                item = item.Next as DataSourceItem;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Checks the item silently, i.e. without firing the <see cref="RadDataBoundListBox.ItemCheckedStateChanging"/> event.
        /// </summary>
        internal void CheckItemSilently(DataSourceItem item, bool updateContainerState, bool synchWithSource)
        {
            if (!item.isChecked)
            {
                this.SetItemCheckedState(item, true, updateContainerState, synchWithSource, CheckedItemsCollectionOperation.Add);
                this.items.Add(item);
                this.OnNotifyCollectionChanged(NotifyCollectionChangedAction.Add, null, item.Value, this.items.Count - 1);
                this.owner.OnItemCheckedStateChanged(item.Value, true);
            }
        }

        /// <summary>
        /// Unchecks the item silently, i.e. without firing the <see cref="RadDataBoundListBox.ItemCheckedStateChanging"/> event.
        /// </summary>
        internal void UncheckItemSilently(DataSourceItem item, bool updateContainerState, bool syncWithSource)
        {
            if (item.isChecked)
            {
                this.SetItemCheckedState(item, false, updateContainerState, syncWithSource, CheckedItemsCollectionOperation.Remove);
                int indexOfItem = this.items.IndexOf(item);
                this.items.RemoveAt(indexOfItem);
                this.OnNotifyCollectionChanged(NotifyCollectionChangedAction.Remove, item.Value, null, indexOfItem);
                this.owner.OnItemCheckedStateChanged(item.Value, false);
            }
        }

        /// <summary>
        /// Unchecks all items silently, i.e. without firing the <see cref="RadDataBoundListBox.ItemCheckedStateChanging"/>
        /// and <see cref="RadDataBoundListBox.ItemCheckedStateChanged"/> events.
        /// </summary>
        internal void ClearSilently(bool updateSourceObjectCheckedPath)
        {
            foreach (DataSourceItem item in this.items)
            {
                this.SetItemCheckedState(item, false, true, updateSourceObjectCheckedPath, CheckedItemsCollectionOperation.Clear);
            }

            this.items.Clear();
            this.OnNotifyCollectionChanged(NotifyCollectionChangedAction.Reset, null, null, -1);
        }

        private void CheckItem(DataSourceItem item)
        {
            if (!item.isChecked && !this.owner.OnItemCheckedStateChanging(item.Value, true))
            {
                this.SetItemCheckedState(item, true, true, true, CheckedItemsCollectionOperation.Add);
                this.items.Add(item);
                this.OnNotifyCollectionChanged(NotifyCollectionChangedAction.Add, null, item.Value, this.items.Count - 1);
                this.owner.OnItemCheckedStateChanged(item.Value, true);
            }
        }

        private void UncheckItem(DataSourceItem item)
        {
            if (item.isChecked && !this.owner.OnItemCheckedStateChanging(item.Value, false))
            {
                this.SetItemCheckedState(item, false, true, true, CheckedItemsCollectionOperation.Remove);
                int indexOfItem = this.items.IndexOf(item);
                this.items.RemoveAt(indexOfItem);
                this.OnNotifyCollectionChanged(NotifyCollectionChangedAction.Remove, null, item.Value, indexOfItem);
                this.owner.OnItemCheckedStateChanged(item.Value, false);
            }
        }

        private void SetItemCheckedState(DataSourceItem item, bool isChecked, bool updateContainer, bool synchWithSource, CheckedItemsCollectionOperation operation)
        {
            item.isChecked = isChecked;

            if (synchWithSource)
            {
                this.owner.listSourceFactory.UpdateSourceItemIsCheckedProperty(item);
            }

            this.owner.OnItemCheckedStateUpdatedCore(item, operation);

            if (updateContainer)
            {
                this.UpdateContainerVisualState(item);
            }
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        private DataSourceItem GetItemAt(int index)
        {
            return this.items[index];
        }

        private void OnNotifyCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int changeIndex)
        {
            if (this.CollectionChanged != null)
            {
                NotifyCollectionChangedEventArgs args = this.GenerateArgs(action, oldItem, newItem, changeIndex);
                this.CollectionChanged(this, args);
            }
        }

        private NotifyCollectionChangedEventArgs GenerateArgs(NotifyCollectionChangedAction action, object oldItem, object newItem, int changeIndex)
        {
            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                    return new NotifyCollectionChangedEventArgs(action, newItem, changeIndex);

                case NotifyCollectionChangedAction.Remove:
                    return new NotifyCollectionChangedEventArgs(action, oldItem, changeIndex);

                case NotifyCollectionChangedAction.Replace:
                    return new NotifyCollectionChangedEventArgs(action, oldItem, newItem, changeIndex);

                default:

                    return new NotifyCollectionChangedEventArgs(action);
            }
        }

        private void UpdateContainerVisualState(DataSourceItem item)
        {
            RadDataBoundListBoxItem container = this.owner.GetContainerFromDataSourceItem(item) as RadDataBoundListBoxItem;

            if (container != null)
            {
                container.UpdateCheckedState();
            }
        }

        private object GetDataItem(object item)
        {
            DataSourceItem typedItem = item as DataSourceItem;
            if (typedItem != null)
            {
                return typedItem.Value;
            }

            return item;
        }
    }
}
