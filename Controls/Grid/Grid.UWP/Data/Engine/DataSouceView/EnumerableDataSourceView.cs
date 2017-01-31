using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Telerik.Core;
using Telerik.Core.Data;
using Windows.UI.Xaml.Data;

namespace Telerik.Data.Core
{
    internal class EnumerableDataSourceView : IDataSourceView, IWeakEventListener
    {
        private List<object> internalList;
        private WeakEventHandler<NotifyCollectionChangedEventArgs> collectionChangedEventHandler;
        private WeakEventHandlerList<PropertyChangedEventArgs> propertyChangedEventHandler;
        private bool itemSupportsPropertyChanged;
        private bool supportsPropertyChangedInitialized;

        public EnumerableDataSourceView(IEnumerable enumerableSource)
        {
            IList list = enumerableSource as IList;
            int count = list != null ? list.Count : 4;

            this.internalList = new List<object>(count);
            INotifyCollectionChanged incc = enumerableSource as INotifyCollectionChanged;
            if (incc != null)
            {
                this.collectionChangedEventHandler = new WeakEventHandler<NotifyCollectionChangedEventArgs>(incc, this, KnownEvents.CollectionChanged);
            }

            this.propertyChangedEventHandler = new WeakEventHandlerList<PropertyChangedEventArgs>(this, KnownEvents.PropertyChanged);


            foreach (var item in enumerableSource)
            {
                if (!supportsPropertyChangedInitialized)
                {
                    this.itemSupportsPropertyChanged = item is INotifyPropertyChanged;
                    supportsPropertyChangedInitialized = true;
                }

                this.AddPropertyChangedHandler(item);
                this.internalList.Add(item);
            }

            this.InitializeBatchDataLoader(enumerableSource as ISupportIncrementalLoading);
        }

        ~EnumerableDataSourceView()
        {
            ((IDataSourceView)this).Dispose();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler ItemPropertyChanged;

        public IList<object> InternalList
        {
            get
            {
                return this.ListStorage;
            }
        }

        public BatchLoadingProvider<object> BatchDataProvider { get; private set; }

        protected virtual IList<object> ListStorage
        {
            get
            {
                return this.internalList;
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

        void IWeakEventListener.ReceiveEvent(object sender, object args)
        {
            INotifyCollectionChanged incc = sender as INotifyCollectionChanged;
            if (incc != null)
            {
                this.HandleCollectionChanged(sender, (NotifyCollectionChangedEventArgs)args);
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

        private void InitializeBatchDataLoader(ISupportIncrementalLoading supportIncrementalLoading)
        {
            if (supportIncrementalLoading != null)
            {
                this.BatchDataProvider = new BatchLoadingProvider<object>(supportIncrementalLoading, this.InternalList);
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
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        this.InsertItem(e.NewStartingIndex + i, e.NewItems[i]);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        this.RemoveItem(e.OldStartingIndex, e.OldItems[i]);
                    }

                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    this.RemoveItem(e.OldStartingIndex, this.internalList[e.OldStartingIndex]);
                    this.InsertItem(e.NewStartingIndex, e.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.ClearItemsHandlers();
                    break;
                default:
                    break;
            }

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

        private void RemoveItem(int index, object oldItem)
        {
            this.RemovePropertyChangedHandler(oldItem);
            this.internalList.RemoveAt(index);
        }

        private void AddPropertyChangedHandler(object item)
        {
            if (!supportsPropertyChangedInitialized)
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