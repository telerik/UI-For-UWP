using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Core;
using Telerik.Core.Data;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;

namespace Telerik.Data.Core
{
    internal class CollectionViewDataSourceView : IDataSourceView, IWeakEventListener, IDataSourceCurrency
    {
        private IList<object> internalList;
        private WeakEventHandler<IVectorChangedEventArgs> collectionChangedEventHandler;
        private WeakEventHandlerList<PropertyChangedEventArgs> propertyChangedEventHandler;
        private WeakEventHandler<object> currentItemChangedEventHandler;
        private ICollectionView source;
        private List<Tuple<object, NotifyCollectionChangedEventArgs>> pendingCollectionChanges = new List<Tuple<object, NotifyCollectionChangedEventArgs>>();

        private List<Tuple<object, int, int>> sourceGroups = new List<Tuple<object, int, int>>();

        private bool itemSupportsPropertyChanged;
        private bool supportsPropertyChangedInitialized;

        public CollectionViewDataSourceView(ICollectionView collectionView)
        {
            this.source = collectionView;
            bool isSourceGrouped = collectionView.CollectionGroups != null;

            IObservableVector<object> iov = collectionView as IObservableVector<object>;
            if (iov != null)
            {
                this.collectionChangedEventHandler = new WeakEventHandler<IVectorChangedEventArgs>(iov, this, KnownEvents.VectorChanged);
            }

            this.currentItemChangedEventHandler = new WeakEventHandler<object>(collectionView, this, KnownEvents.CurrentItemChanged);

            this.propertyChangedEventHandler = new WeakEventHandlerList<PropertyChangedEventArgs>(this, KnownEvents.PropertyChanged);

            this.internalList = new List<object>(collectionView.Count);

            if (isSourceGrouped)
            {
                foreach (ICollectionViewGroup group in collectionView.CollectionGroups)
                {
                    foreach (var item in group.GroupItems)
                    {
                        this.internalList.Add(item);
                    }

                    int startinIndex = this.sourceGroups.Count == 0 ? 0 : this.sourceGroups[this.sourceGroups.Count - 1].Item3;

                    this.sourceGroups.Add(new Tuple<object, int, int>(group.Group, startinIndex, group.GroupItems.Count + startinIndex));
                }
            }

            foreach (var item in collectionView)
            {
                if (!this.supportsPropertyChangedInitialized)
                {
                    this.itemSupportsPropertyChanged = item is INotifyPropertyChanged;
                    this.supportsPropertyChangedInitialized = true;
                }

                this.AddPropertyChangedHandler(item);

                if (!isSourceGrouped)
                {
                    this.internalList.Add(item);
                }
            }

            this.InitializeBatchDataLoader(collectionView as ISupportIncrementalLoading);
        }

        ~CollectionViewDataSourceView()
        {
            ((IDataSourceView)this).Dispose();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanging;
        public event PropertyChangedEventHandler ItemPropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event EventHandler<object> CurrentChanged;

        public IList<object> InternalList
        {
            get
            {
                return this.ListStorage;
            }
        }

        public IBatchLoadingProvider BatchDataProvider { get; private set; }
        
        public List<Tuple<object, int, int>> SourceGroups
        {
            get
            {
                return this.sourceGroups;
            }
        }

        protected IList<object> ListStorage
        {
            get
            {
                return this.internalList;
            }
        }

        public void ProcessPendingCollectionChange()
        {
            if (this.pendingCollectionChanges.Count > 0)
            {
                this.ProcessCollectionChanged(this.pendingCollectionChanges[0].Item1, this.pendingCollectionChanges[0].Item2);
                this.pendingCollectionChanges.RemoveAt(0);
            }
        }

        void IDataSourceView.Dispose()
        {
            if (this.collectionChangedEventHandler != null)
            {
                this.collectionChangedEventHandler.Unsubscribe();
                this.collectionChangedEventHandler = null;
            }

            this.ClearItemsHandlers();

            this.propertyChangedEventHandler = null;
        }

        public void ChangeCurrentItem(object item)
        {
            this.source.MoveCurrentTo(item);
        }
        
        void IWeakEventListener.ReceiveEvent(object sender, object args)
        {
            NotifyCurrentItemChangedEventArgs currentItemChangedArgs = args as NotifyCurrentItemChangedEventArgs;
            if (currentItemChangedArgs != null)
            {
                this.HandleCurrentItemChanged(sender, currentItemChangedArgs);
            }
            else
            {
                IObservableVector<object> iov = sender as IObservableVector<object>;
                if (iov != null)
                {
                    IVectorChangedEventArgs vectorArgs = args as IVectorChangedEventArgs;
                    NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Add;
                    int index = (int)vectorArgs.Index;
                    var collectionView = sender as ICollectionView;
                    var changedItems = new List<object>();
                    var oldItems = new List<object>();
                    NotifyCollectionChangedEventArgs args2 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                    switch (vectorArgs.CollectionChange)
                    {
                        case CollectionChange.ItemChanged:
                            action = NotifyCollectionChangedAction.Replace;
                            changedItems.Add(collectionView[index]);
                            oldItems.Add(this.InternalList[index]);
                            args2 = new NotifyCollectionChangedEventArgs(action, changedItems, oldItems, index);
                            break;
                        case CollectionChange.ItemInserted:
                            action = NotifyCollectionChangedAction.Add;
                            changedItems.Add(collectionView[index]);
                            args2 = new NotifyCollectionChangedEventArgs(action, changedItems, index);
                            break;
                        case CollectionChange.ItemRemoved:
                            action = NotifyCollectionChangedAction.Remove;
                            oldItems.Add(this.InternalList[index]);
                            args2 = new NotifyCollectionChangedEventArgs(action, oldItems, index);
                            break;
                        case CollectionChange.Reset:
                            action = NotifyCollectionChangedAction.Reset;
                            args2 = new NotifyCollectionChangedEventArgs(action);
                            break;
                    }

                    this.HandleCollectionChanged(sender, args2);
                }
                else
                {
                    INotifyPropertyChanged inpc = sender as INotifyPropertyChanged;
                    if (inpc != null)
                    {
                        this.HandlePropertyChanged(sender, (PropertyChangedEventArgs)args);
                    }
                }
            }
        }

        object IDataSourceView.GetGroupKey(object item, int level)
        {
            if (this.source != null && this.source.CollectionGroups != null && level == 0)
            {
                foreach (ICollectionViewGroup group in this.source.CollectionGroups)
                {
                    if (group.GroupItems.Contains(item))
                    {
                        return group.Group;
                    }
                }
            }

            return null;
        }

        private void InitializeBatchDataLoader(ISupportIncrementalLoading supportIncrementalLoading)
        {
            if (supportIncrementalLoading != null)
            {
                this.BatchDataProvider = new BatchLoadingProvider<object>(supportIncrementalLoading, this.InternalList);
            }
            else
            {
                this.BatchDataProvider = new ManualBatchLoadingProvider<object>(this.InternalList);
            }
        }

        private void HandleCurrentItemChanged(object sender, NotifyCurrentItemChangedEventArgs e)
        {
            var handler = this.CurrentChanged;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var pc = this.ItemPropertyChanged;
            if (pc != null)
            {
                pc(sender, e);
            }
        }

        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
            {
                var tuple = new Tuple<object, NotifyCollectionChangedEventArgs>(sender, e);
                this.pendingCollectionChanges.Add(tuple);

                var ccg = this.CollectionChanging;
                if (ccg != null)
                {
                    ccg(sender, e);
                }
            }
            else
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    var ccg = this.CollectionChanging;
                    if (ccg != null)
                    {
                        ccg(sender, e);
                    }
                }
                this.ProcessCollectionChanged(sender, e);
            }
        }

        private void ProcessCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    this.InsertItem(e.NewStartingIndex, (sender as ICollectionView)[e.NewStartingIndex]);

                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.RemoveItem(e.OldStartingIndex);

                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.ChangeItem(e.OldStartingIndex, (sender as ICollectionView)[e.OldStartingIndex]);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.ClearItemsHandlers();
                    break;
                default:
                    break;
            }

            var se = sender;
            var args = e;

            var cc = this.CollectionChanged;
            if (cc != null)
            {
                cc(sender, e);
            }
        }

        private void ClearItemsHandlers()
        {
            for (int i = 0; i < this.internalList.Count; i++)
            {
                object item = this.internalList[i];
                this.RemovePropertyChangedHandler(item);
            }

            this.internalList.Clear();
        }

        private void InsertItem(int newIndex, object newItem)
        {
            this.AddPropertyChangedHandler(newItem);
            this.internalList.Insert(newIndex, newItem);
        }

        private void RemoveItem(int index)
        {
            this.RemovePropertyChangedHandler(this.internalList[index]);
            this.internalList.RemoveAt(index);
        }

        private void ChangeItem(int index, object newItem)
        {
            this.AddPropertyChangedHandler(newItem);
            this.internalList[index] = newItem;
        }

        private void AddPropertyChangedHandler(object item)
        {
            if (!this.supportsPropertyChangedInitialized)
            {
                this.itemSupportsPropertyChanged = item is INotifyPropertyChanged;
                this.supportsPropertyChangedInitialized = true;
            }

            if (this.itemSupportsPropertyChanged)
            {
                this.propertyChangedEventHandler.Subscribe(item);
            }
        }

        private void RemovePropertyChangedHandler(object item)
        {
            if (this.itemSupportsPropertyChanged)
            {
                this.propertyChangedEventHandler.Unsubscribe(item);
            }
        }
    }
}
