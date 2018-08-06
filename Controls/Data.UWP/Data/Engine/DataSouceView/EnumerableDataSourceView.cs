using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
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
        private Dictionary<object, NestedPropertyInfo> nestedObjectInfos;

        private List<Tuple<object, NotifyCollectionChangedEventArgs>> pendingCollectionChanges = new List<Tuple<object, NotifyCollectionChangedEventArgs>>();
        private List<Tuple<object, int, int>> sourceGroups = new List<Tuple<object, int, int>>();

        private bool itemSupportsPropertyChanged;
        private bool supportsPropertyChangedInitialized;
        private bool shouldSubsribeToPropertyChanged;

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
                this.internalList.Add(item);
            }

            this.InitializeBatchDataLoader(enumerableSource as ISupportIncrementalLoading);
        }

        ~EnumerableDataSourceView()
        {
            ((IDataSourceView)this).Dispose();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanging;
        public event PropertyChangedEventHandler ItemPropertyChanged;

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

        object IDataSourceView.GetGroupKey(object item, int level)
        {
            return null;
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
                    if (this.internalList.Contains(inpc))
                    {
                        if (this.nestedObjectInfos.Count > 0)
                        {
                            this.NestedPropertyChanged(sender, sender, ((PropertyChangedEventArgs)args).PropertyName);
                        }

                        this.HandlePropertyChanged(sender, (PropertyChangedEventArgs)args);
                    }
                    else
                    {
                        NestedPropertyInfo info;
                        if (this.nestedObjectInfos.TryGetValue(sender, out info))
                        {
                            string propertyName = ((PropertyChangedEventArgs)args).PropertyName;
                            this.NestedPropertyChanged(sender, info.rootItem, propertyName, info.nestedPropertyPath);
                            PropertyChangedEventArgs arguments = new PropertyChangedEventArgs(info.nestedPropertyPath + propertyName);
                            this.HandlePropertyChanged(info.rootItem, arguments);
                        }
                    }
                }
            }
        }

        void IDataSourceView.SubscribeToItemPropertyChanged()
        {
            this.shouldSubsribeToPropertyChanged = true;
            if (this.nestedObjectInfos == null)
            {
                this.nestedObjectInfos = new Dictionary<object, NestedPropertyInfo>();
            }

            StringBuilder nestedPropertyPath = new StringBuilder();
            for (int i = 0; i < this.internalList.Count; i++)
            {
                object item = this.internalList[i];
                this.SubscribeToINotifyPropertyChanged(item, nestedPropertyPath, i);
            }
        }

        void IDataSourceView.UnsubscribeFromItemPropertyChanged()
        {
            this.shouldSubsribeToPropertyChanged = false;
            foreach (var item in this.internalList)
            {
                this.RemovePropertyChangedHandler(item);
            }

            if (this.nestedObjectInfos != null)
            {
                foreach (var nestedItem in this.nestedObjectInfos.Keys)
                {
                    this.RemovePropertyChangedHandler(nestedItem);
                }

                this.nestedObjectInfos.Clear();
            }
        }

        private void NestedPropertyChanged(object changedItem, object rootItem, string propertyName, string nestedPropertyName = null)
        {
            Type changedObjectType = changedItem.GetType();
            PropertyInfo propertyInfo = changedObjectType.GetRuntimeProperty(propertyName);
            object changedObjectValue = propertyInfo.GetValue(changedItem);
            if (changedObjectValue is INotifyPropertyChanged)
            {
                string pathName = nestedPropertyName + propertyName;
                var keys = this.nestedObjectInfos.Where(a => a.Value.nestedPropertyPath.Contains(pathName) && a.Value.rootItem == rootItem).ToList();
                foreach (var nestedItem in keys)
                {
                    this.RemovePropertyChangedHandler(nestedItem.Key);
                    this.nestedObjectInfos.Remove(nestedItem.Key);
                }

                StringBuilder propertyNameBuilder = new StringBuilder();
                nestedPropertyName += propertyName + ".";
                propertyNameBuilder.Append(nestedPropertyName);

                this.nestedObjectInfos.Add(changedObjectValue, new NestedPropertyInfo(rootItem, nestedPropertyName));
                this.SubscribeToINotifyPropertyChanged(changedObjectValue, propertyNameBuilder, this.internalList.IndexOf(rootItem));

                keys = this.nestedObjectInfos.Where(a => a.Value.nestedPropertyPath.Contains(pathName) && a.Value.rootItem == rootItem).ToList();
            }
        }

        private void SubscribeToINotifyPropertyChanged(object item, StringBuilder nestedPropertyPath, int currentIndex)
        {
            if (!this.supportsPropertyChangedInitialized)
            {
                this.itemSupportsPropertyChanged = item is INotifyPropertyChanged;
                this.supportsPropertyChangedInitialized = true;
            }

            this.AddPropertyChangedHandler(item);

            Type itemType = item.GetType();
            IEnumerable<PropertyInfo> properties = itemType.GetRuntimeProperties();

            object tempItem = item;
            foreach (PropertyInfo info in properties)
            {
                tempItem = info.GetValue(tempItem);
                if (!(tempItem is INotifyPropertyChanged))
                {
                    tempItem = item;
                    continue;
                }

                string infoName = info.Name;
                nestedPropertyPath.Append(info.Name);
                nestedPropertyPath.Append('.');
                string propertyPath = nestedPropertyPath.ToString();

                this.nestedObjectInfos.Add(tempItem, new NestedPropertyInfo(this.internalList[currentIndex], propertyPath));
                this.SubscribeToINotifyPropertyChanged(tempItem, nestedPropertyPath, currentIndex);

                int dotIndex = propertyPath.LastIndexOf('.');
                nestedPropertyPath.Remove(dotIndex - infoName.Length, infoName.Length + 1);
            }
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

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.ItemPropertyChanged?.Invoke(sender, e);
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

            var se = sender;
            var args = e;

            var cc = this.CollectionChanged;
            if (cc != null)
            {
                cc(sender, args);
            }
        }

        private void ClearItemsHandlers()
        {
            for (int i = 0; i < this.internalList.Count; i++)
            {
                object item = this.internalList[i];
                this.RemovePropertyChangedHandler(item);
            }

            if (this.nestedObjectInfos != null)
            {
                foreach (var item in this.nestedObjectInfos.Keys)
                {
                    this.RemovePropertyChangedHandler(item);
                }

                this.nestedObjectInfos.Clear();
            }

            this.internalList.Clear();
        }

        private void InsertItem(int newIndex, object newItem)
        {
            this.internalList.Insert(newIndex, newItem);

            if (this.shouldSubsribeToPropertyChanged)
            {
                if (this.nestedObjectInfos != null && this.nestedObjectInfos.Count > 0)
                {
                    StringBuilder propertyName = new StringBuilder();
                    this.SubscribeToINotifyPropertyChanged(newItem, propertyName, newIndex);
                }
                else
                {
                    this.AddPropertyChangedHandler(newItem);
                }
            }
        }

        private void RemoveItem(int index, object oldItem)
        {
            this.RemovePropertyChangedHandler(oldItem);
            this.internalList.RemoveAt(index);

            if (this.nestedObjectInfos != null && this.nestedObjectInfos.Count > 0)
            {
                var keys = this.nestedObjectInfos.Where(a => a.Value.rootItem == oldItem).ToList();
                foreach (var item in keys)
                {
                    this.RemovePropertyChangedHandler(item.Key);
                    this.nestedObjectInfos.Remove(item.Key);
                }
            }
        }

        private void AddPropertyChangedHandler(object item)
        {
            if (!this.supportsPropertyChangedInitialized)
            {
                this.itemSupportsPropertyChanged = item is INotifyPropertyChanged;
                this.supportsPropertyChangedInitialized = true;
            }

            if (this.itemSupportsPropertyChanged && this.shouldSubsribeToPropertyChanged)
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