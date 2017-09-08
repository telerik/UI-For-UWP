using System;

namespace Telerik.Core.Data
{
    internal partial class RadListSource
    {
        private CurrencyManagementMode currencyMode;
        private IDataSourceItem currentItem;
        private IDataSourceItem previousCurrentItem;
        private EventHandler currentItemChangedEvent;
        private EventHandler<CurrentItemChangingEventArgs> currentItemChangingEvent;

        /// <summary>
        /// Raised when the <see cref="CurrentItem"/> property of this source changes.
        /// </summary>
        public event EventHandler CurrentItemChanged
        {
            add
            {
                this.currentItemChangedEvent += value;
            }
            remove
            {
                this.currentItemChangedEvent -= value;
            }
        }

        /// <summary>
        /// Raised when the <see cref="CurrentItem"/> property is about to be changed.
        /// </summary>
        public event EventHandler<CurrentItemChangingEventArgs> CurrentItemChanging
        {
            add
            {
                this.currentItemChangingEvent += value;
            }
            remove
            {
                this.currentItemChangingEvent -= value;
            }
        }

        /// <summary>
        /// Gets or sets the mode used to update the current item within the collection.
        /// </summary>
        public CurrencyManagementMode CurrencyMode
        {
            get
            {
                return this.currencyMode;
            }
            set
            {
                if (this.currencyMode == value)
                {
                    return;
                }

                this.currencyMode = value;
                this.SynchronizeCurrent();
            }
        }

        /// <summary>
        /// Gets the current item in the collection.
        /// </summary>
        public IDataSourceItem CurrentItem
        {
            get
            {
                return this.currentItem;
            }
        }

        /// <summary>
        /// Sets the specified item as <see cref="CurrentItem"/> property.
        /// </summary>
        /// <param name="item">The item that should become current.</param>
        /// <returns>True if operation was successful, false otherwise.</returns>
        public bool SetCurrentItem(IDataSourceItem item)
        {
            return this.SetCurrentItemCore(item, true);
        }

        /// <summary>
        /// Sets the <see cref="CurrentItem"/> property to the first item in the collection.
        /// </summary>
        /// <returns>True if operation was successful, false otherwise.</returns>
        public bool MoveCurrentToFirst()
        {
            if (this.items.Count == 0)
            {
                return false;
            }

            return this.SetCurrentItemCore(this.items[0], true);
        }

        /// <summary>
        /// Sets the <see cref="CurrentItem"/> property to the last item in the collection.
        /// </summary>
        /// <returns>True if operation was successful, false otherwise.</returns>
        public bool MoveCurrentToLast()
        {
            if (this.items.Count == 0)
            {
                return false;
            }

            return this.SetCurrentItemCore(this.items[this.items.Count - 1], true);
        }

        /// <summary>
        /// Sets the <see cref="CurrentItem"/> property to the next item after the current one.
        /// </summary>
        /// <returns>True if operation was successful, false otherwise.</returns>
        public bool MoveCurrentToNext()
        {
            IDataSourceItem current = this.CurrentItem;
            if (current != null)
            {
                return this.SetCurrentItemCore(current.Next, true);
            }

            return false;
        }

        /// <summary>
        /// Sets the <see cref="CurrentItem"/> property to the previous item before the current one.
        /// </summary>
        /// <returns>True if operation was successful, false otherwise.</returns>
        public bool MoveCurrentToPrevious()
        {
            IDataSourceItem current = this.CurrentItem;
            if (current != null)
            {
                return this.SetCurrentItemCore(current.Previous, true);
            }

            return false;
        }

        internal void UpdateCurrentItem(IDataSourceItem newCurrentItem)
        {
            if (this.currencyMode == CurrencyManagementMode.None)
            {
                return;
            }

            this.SetCurrentItemCore(newCurrentItem, false);
        }

        internal IDataSourceItem CreateNewOrGetCurrent(object dataItem, ref IDataSourceItem newCurrent)
        {
            if (newCurrent == null && this.previousCurrentItem != null &&
                object.ReferenceEquals(this.previousCurrentItem.Value, dataItem))
            {
                newCurrent = this.previousCurrentItem;

                //// Since we store the previously selected item when a refresh is called, but unhook its property changed event
                //// we need to hook it again in case this item is reused.

                this.previousCurrentItem.Next = null;
                this.previousCurrentItem.Previous = null;

                if ((this.previousCurrentItem as DataSourceItem).propertyChangedHandler == null)
                {
                    ////Reset the whole DataSourceItem.
                    (this.previousCurrentItem as DataSourceItem).HookPropertyChanged();
                }

                return this.previousCurrentItem;
            }

            return this.itemFactory.CreateItem(this, dataItem);
        }

        /// <summary>
        /// Raises the <see cref="CurrentItemChanged"/> event.
        /// </summary>
        protected virtual void OnCurrentItemChanged(EventArgs e)
        {
            if (this.currentItemChangedEvent != null)
            {
                this.currentItemChangedEvent(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="CurrentItemChanging"/> event.
        /// </summary>
        protected virtual void OnCurrentItemChanging(CurrentItemChangingEventArgs args)
        {
            if (this.currentItemChangingEvent != null)
            {
                this.currentItemChangingEvent(this, args);
            }
        }

        /// <summary>
        /// Syncs the current item with an external change (if the source collection implements the <see cref="Windows.UI.Xaml.Data.ICollectionView"/> interface.
        /// </summary>
        protected virtual void SynchronizeCurrent()
        {
            if (this.currencyMode != CurrencyManagementMode.LocalAndExternal)
            {
                this.SetCurrentItemCore(null, false);
                return;
            }

            if (this.sourceCollectionAsCollectionView == null)
            {
                return;
            }

            IDataSourceItem current = this.FindItem(this.sourceCollectionAsCollectionView.CurrentItem);
            this.SetCurrentItemCore(current, false);
        }

        /// <summary>
        /// Performs the core logic of updating the current item.
        /// </summary>
        /// <param name="item">The item that needs to be set as current.</param>
        /// <param name="synchronizeOriginal">True to apply the change to the original <see cref="Windows.UI.Xaml.Data.ICollectionView"/> instance (if any), false otherwise.</param>
        /// <returns>True if operation was successful, false otherwise.</returns>
        protected virtual bool SetCurrentItemCore(IDataSourceItem item, bool synchronizeOriginal)
        {
            if (this[UpdatingCurrentStateKey])
            {
                return false;
            }

            if (!this.IsNewCurrentItem(item))
            {
                return false;
            }

            CurrentItemChangingEventArgs args = new CurrentItemChangingEventArgs(item, synchronizeOriginal);
            this.OnCurrentItemChanging(args);

            if (args.Cancel)
            {
                return false;
            }

            this[UpdatingCurrentStateKey] = true;

            bool success = true;
            if (synchronizeOriginal && 
                this.currencyMode == CurrencyManagementMode.LocalAndExternal &&
                this.sourceCollectionAsCollectionView != null)
            {
                success = this.sourceCollectionAsCollectionView.MoveCurrentTo(item == null ? null : item.Value);
            }

            if (success)
            {
                this.currentItem = item;

                // do not raise CurrentItemChanged if we are currently refreshing the source
                if (!this[RefreshingStateKey])
                {
                    this.OnCurrentItemChanged(EventArgs.Empty);
                }
            }

            this[UpdatingCurrentStateKey] = false;

            return success;
        }

        /// <summary>
        /// Determines whether the specified <see cref="IDataSourceItem"/> instance may be considered as a new Current item.
        /// </summary>
        /// <param name="newCurrent">The item instance to check for.</param>
        protected virtual bool IsNewCurrentItem(IDataSourceItem newCurrent)
        {
            if (newCurrent != this.currentItem)
            {
                return true;
            }

            return newCurrent != null && this.currentItem != null && 
                !object.Equals(newCurrent.Value, this.currentItem.Value);
        }

        private void OnSourceCollectionCurrentChanged(object sender, EventArgs e)
        {
            if (this[UpdatingCurrentStateKey])
            {
                return;
            }

            this.SynchronizeCurrent();
        }

        private void CheckCurrentRemoved(IDataSourceItem removedItem)
        {
            if (this.currentItem != removedItem)
            {
                return;
            }

            // user have removed the current item, update it depending on the currency mode
            IDataSourceItem newCurrent = null;

            this.UpdateCurrentItem(newCurrent);
        }
    }
}
