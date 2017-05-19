using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Telerik.Core.Data;
using Telerik.Data.Core;
using Telerik.Data.Core.Fields;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid.Model
{
    internal partial class GridModel : IDataDescriptorsHost
    {
        private LocalDataSourceProvider localDataProvider;
        private IDataProvider externalDataProvider;
        private bool isDataProviderUpdating;
        private bool hasPendingDataRefresh;
        private bool updatingData;
        private DataChangeFlags dataChangeFlags;
        private object itemsSource;
        private SortDescriptorCollection sortDescriptors;
        private GroupDescriptorCollection groupDescriptors;
        private FilterDescriptorCollection filterDescriptors;
        private AggregateDescriptorCollection aggregateDescriptors;
        private bool isCurrentItemSynchronizing = false;

        public object ItemsSource
        {
            get
            {
                return this.itemsSource;
            }
        }

        public bool IsDataProviderUpdating
        {
            get
            {
                return this.isDataProviderUpdating;
            }
        }

        public SortDescriptorCollection SortDescriptors
        {
            get
            {
                return this.sortDescriptors;
            }
        }

        public GroupDescriptorCollection GroupDescriptors
        {
            get
            {
                return this.groupDescriptors;
            }
        }

        public FilterDescriptorCollection FilterDescriptors
        {
            get
            {
                return this.filterDescriptors;
            }
        }

        public AggregateDescriptorCollection AggregateDescriptors
        {
            get
            {
                return this.aggregateDescriptors;
            }
        }

        public bool IsDataReady
        {
            get
            {
                return this.itemsSource != null && !this.isDataProviderUpdating;
            }
        }

        public IEnumerable<IDataDescriptorPeer> DescriptorPeers
        {
            get
            {
                return this.columns.OfType<IDataDescriptorPeer>();
            }
        }

        IDataProvider IDataDescriptorsHost.CurrentDataProvider
        {
            get
            {
                return this.CurrentDataProvider;
            }
        }

        internal IDataProvider CurrentDataProvider
        {
            get
            {
                if (this.localDataProvider != null)
                {
                    return this.localDataProvider;
                }

                return this.externalDataProvider;
            }
        }

        internal bool ShouldDisplayIncrementalLoadingIndicator
        {
            get
            {
                if (this.CurrentDataProvider != null)
                {
                    if (this.DataLoadingMode == BatchLoadingMode.Explicit || this.GroupDescriptors.Count > 0)
                    {
                        return this.GridView.CommandService.CanExecuteCommand(CommandId.LoadMoreData, new LoadMoreDataContext());
                    }
                }
                return false;
            }
        }

        void IDataDescriptorsHost.OnDataDescriptorPropertyChanged(DataDescriptor descriptor)
        {
            this.dataChangeFlags |= descriptor.UpdateFlags;
            this.GridView.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsData);
        }
        
        internal IEnumerable ForEachDataDescriptor()
        {
            foreach (var descriptor in this.filterDescriptors)
            {
                yield return descriptor;
            }

            foreach (var descriptor in this.sortDescriptors)
            {
                yield return descriptor;
            }

            foreach (var descriptor in this.groupDescriptors)
            {
                yield return descriptor;
            }

            foreach (var descriptor in this.aggregateDescriptors)
            {
                yield return descriptor;
            }
        }

        internal void RefreshData()
        {
            if (this.CurrentDataProvider != null &&
                (this.CurrentDataProvider.Status == DataProviderStatus.Ready ||
                 this.CurrentDataProvider.Status == DataProviderStatus.DescriptionsReady))
            {
                this.CurrentDataProvider.DeferUpdates = false;

                this.CurrentDataProvider.Refresh(this.dataChangeFlags);

                this.GridView.UpdateService.RemoveUpdateFlag((int)UpdateFlags.AffectsData);
                this.CurrentDataProvider.DeferUpdates = true;
            }
            else
            {
                this.hasPendingDataRefresh = true;
            }
        }

        internal void OnItemsSourceChanged(object newSource)
        {
            this.areColumnsGenerated = false;
            this.dataChangeFlags |= DataChangeFlags.Source;

            // external providers have precedence over local data provider
            IExternalItemsSource extItemSource = newSource as IExternalItemsSource;
            if (extItemSource != null)
            {
                // localDataProvider should be avoided in model logic from this moment on
                if (this.localDataProvider != null)
                {
                    this.localDataProvider.StatusChanged -= this.OnDataProviderStatusChanged;
                    this.localDataProvider = null;
                }

                if (newSource != this.externalDataProvider)
                {
                    this.externalDataProvider = extItemSource.GetExternalProvider();
                    this.UpdateProviderField(this.externalDataProvider);
                    this.externalDataProvider.Refresh(this.dataChangeFlags);
                    this.itemsSource = this.externalDataProvider.ItemsSource;
                }
            }
            else
            {
                this.itemsSource = newSource;

                if (this.localDataProvider == null)
                {
                    // create default data provider (Local (in-memory) data)
                    this.CreateDefaultDataProvider();
                }

                this.localDataProvider.ItemsSource = this.itemsSource;
            }

            this.UpdateRequestedItems(null, false);
            this.CheckDataDescriptorsCompatibility();

            // refresh the data
            this.RefreshLayout();
        }

        internal bool ShouldRequestItems(uint? batchSize)
        {
            // In grouped scenario IncrementalLoading is Explicit regardless of its BathcLoadingMode.
            if (this.CurrentDataProvider != null && this.CurrentDataProvider.DataView != null && this.DataLoadingMode == BatchLoadingMode.Auto && this.GroupDescriptors.Count == 0)
            {
                var lastRequestedIndex = this.GetIndexToRequest(batchSize, true);

                if (this.CurrentDataProvider.DataView.BatchDataProvider != null)
                {
                    return this.CurrentDataProvider.DataView.BatchDataProvider.ShouldRequestItems(lastRequestedIndex, 0);
                }
                else
                {
                    return lastRequestedIndex >= this.CurrentDataProvider.DataView.InternalList.Count;
                }
            }

            return false;
        }

        internal void UpdateRequestedItems(uint? batchSize, bool forceRequest)
        {
            if (this.CurrentDataProvider != null && this.CurrentDataProvider.DataView != null && this.CurrentDataProvider.DataView.BatchDataProvider != null)
            {
                var indexToRequest = this.GetIndexToRequest(batchSize, forceRequest);

                this.CurrentDataProvider.DataView.BatchDataProvider.RequestItems(indexToRequest, 0);
            }
        }

        internal void OnDataDescriptorPropertyChanged(DataDescriptor sender)
        {
            this.dataChangeFlags |= sender.UpdateFlags;
            this.GridView.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsData);
        }

        private void CleanUp()
        {
            this.ClearAutoGeneratedColumns();
            this.columnLayout.SetSource(null);
            this.rowLayout.SetSource(null, 0, TotalsPosition.None, 0, 1, false, false);

            this.GridView.UpdateService.RegisterUpdate((int)UpdateFlags.AllButData);
        }

        private int GetIndexToRequest(uint? batchSize, bool forceRequest)
        {
            if (batchSize.HasValue)
            {
                return (int)batchSize.Value - 1;
            }
            else if (forceRequest)
            {
                var generatedItemsLength = this.PhysicalVerticalOffset + this.GridView.ViewportHeight * 1.5;

                int itemsCountToRequest = (int)this.rowLayout.IndexFromPhysicalOffset(generatedItemsLength, true);

                itemsCountToRequest = this.rowLayout.RenderInfo.Count > 0 ? itemsCountToRequest : (int)(generatedItemsLength / this.rowLayout.DefaultItemLength);

                var indexToRequest = itemsCountToRequest - this.rowLayout.GroupCount;
                return indexToRequest;
            }

            return -1;
        }

        private void RefreshLayout()
        {
            if (this.itemsSource != null && this.CurrentDataProvider != null &&
                (this.CurrentDataProvider.Status == DataProviderStatus.DescriptionsReady || this.CurrentDataProvider.Status == DataProviderStatus.Ready))
            {
                this.RefreshData();
            }
            else
            {
                this.CleanUp();
            }
        }

        private void UpdateProviderField(IDataProvider provider)
        {
            provider.IsSingleThreaded = this.executeOperationsSyncroniously;
            provider.DeferUpdates = true;
            provider.GroupFactory = this.GroupFactory;

            provider.StatusChanged += this.OnDataProviderStatusChanged;
            provider.ViewChanged += this.OnDataProviderViewChanged;
            provider.CurrentChanged += this.OnDataProviderCurrentItemChanged;
            provider.FieldDescriptionsChanged += this.OnFieldDescriptionsChanged;
            this.GridView.CurrencyService.CurrentChanged += this.CurrencyService_CurrentChanged;

            this.sortDescriptors.DescriptionCollection = provider.SortDescriptions;
            this.groupDescriptors.DescriptionCollection = provider.GroupDescriptions;
            this.filterDescriptors.DescriptionCollection = provider.FilterDescriptions;
            this.aggregateDescriptors.DescriptionCollection = provider.AggregateDescriptions;
        }
        
        private void CurrencyService_CurrentChanged(object sender, EventArgs e)
        {
            if (!this.isCurrentItemSynchronizing)
            {
                IDataSourceCurrency dsc = this.CurrentDataProvider.DataView as IDataSourceCurrency;
                if (dsc != null)
                {
                    dsc.ChangeCurrentItem(this.GridView.CurrencyService.CurrentItem);
                }
            }
        }

        private void OnDataProviderCurrentItemChanged(object sender, object e)
        {
            ICollectionView view = sender as ICollectionView;
            if (view != null)
            {
                this.isCurrentItemSynchronizing = true;
                this.GridView.CurrencyService.MoveCurrentTo(view.CurrentItem);
                this.isCurrentItemSynchronizing = false;
            }
        }

        private void CreateDefaultDataProvider()
        {
            this.localDataProvider = new LocalDataSourceProvider();
            this.UpdateProviderField(this.localDataProvider as IDataProvider);
        }

        private void OnDataProviderViewChanged(object sender, ViewChangedEventArgs e)
        {
            if (!this.IsDataReady)
            {
                return;
            }

            if (e.Action == CollectionChange.Reset)
            {
                this.GridView.UpdateService.RegisterUpdate((int)UpdateFlags.All);
            }
            else if (e.Action == CollectionChange.ItemChanged)
            {
                this.GridView.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
                this.dataChangeFlags |= DataChangeFlags.PropertyChanged;
            }
            else
            {
                var changes = e.Changes;
                for (int i = 0; i < changes.Count; i++)
                {
                    var change = changes[i];
                    object changedItem = change.ChangedCoordinate.RowGroup;
                    object addRemoveItem = change.AddedOrRemovedCoordinate.RowGroup;

                    // We use only RowGroupIndex for RowLayout and ColumnGroupIndex for ColumnLayout.
                    // For now only rowLayout is updated.
                    int index = change.AddRemoveRowGroupIndex;

                    AddRemoveLayoutResult result;
                    switch (e.Action)
                    {
                        case CollectionChange.ItemInserted:
                            result = this.rowLayout.AddItem(changedItem, addRemoveItem, index);
                            this.GridView.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsContent);

                            break;
                        case CollectionChange.ItemRemoved:
                            result = this.rowLayout.RemoveItem(changedItem, addRemoveItem, index);

                            this.GridView.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
                            break;
                        default:
                            Debug.Assert(true, "Not expected action. Only Add, Remove or Reset are supported");
                            break;
                    }
                }
            }

            this.GridView.CurrencyService.OnDataViewChanged(e);

            ////TODO: Consider what should we do in reset action.
            this.GridView.CommandService.ExecuteCommand(CommandId.DataBindingComplete, new DataBindingCompleteEventArgs() { ChangeFlags = this.dataChangeFlags, DataView = this.GridView.GetDataView() });
        }

        private void OnDataProviderStatusChanged(object sender, DataProviderStatusChangedEventArgs e)
        {
            if (e.NewStatus == DataProviderStatus.RequestingData || e.NewStatus == DataProviderStatus.ProcessingData)
            {
                this.NotifyView(e.NewStatus);
            }

            if (e.NewStatus == DataProviderStatus.Initializing || e.NewStatus == DataProviderStatus.ProcessingData)
            {
                this.BeginDataUpdate();
            }
            else
            {
                if (e.NewStatus == DataProviderStatus.Uninitialized)
                {
                    if (this.itemsSource != null)
                    {
                        this.BeginDataUpdate();
                    }
                    else
                    {
                        this.EndDataUpdate(e);
                    }
                }
                else
                {
                    this.EndDataUpdate(e);
                }
            }
        }

        private void OnFieldDescriptionsChanged(object sender, EventArgs e)
        {
            this.areColumnsGenerated = false;
            if (this.CurrentDataProvider == null)
            {
                return;
            }

            if (this.IsDataReady)
            {
                this.RefreshLayout();
            }
            else
            {
                this.hasPendingDataRefresh = true;
            }
        }

        private void SetRowLayoutSource()
        {
            if (this.CurrentDataProvider == null || this.columns.Count == 0)
            {
                return;
            }

            int groupDescriptionCount = this.CurrentDataProvider.Settings.RowGroupDescriptions.Count;
            bool keepCollapsedState = (this.dataChangeFlags & DataChangeFlags.Group) == DataChangeFlags.None;
           
            if (this.ShouldDisplayIncrementalLoadingIndicator)
            {
                this.rowLayout.LayoutStrategies.Add(new PlaceholderStrategy());
            }
            else
            {
                this.rowLayout.LayoutStrategies.RemoveWhere(c => c is PlaceholderStrategy);
            }

            this.rowLayout.SetSource(this.CurrentDataProvider.Results.Root.RowGroup.Items, groupDescriptionCount, TotalsPosition.None, 0, 1, false, keepCollapsedState);
        }

        private void EndDataUpdate(DataProviderStatusChangedEventArgs e)
        {
            if (e.NewStatus == DataProviderStatus.Faulted || e.NewStatus == DataProviderStatus.Uninitialized)
            {
                this.CleanUp();
                this.ResumeUpdateService(UpdateFlags.AllButData);
                return;
            }

            if (!this.isDataProviderUpdating)
            {
                Debug.Assert(RadControl.IsInTestMode, "Flag not raised properly.");
                return;
            }

            if (e.NewStatus != DataProviderStatus.Ready || !e.ResultsChanged)
            {
                return;
            }

            //// NOTE: This call comes from another thread and everything related to the UI should be dispatched on the UI thread.
            //// It will also clear the IndexTree which will definitely throw NullReference exception if some UI operation is requested before Dispatcher is invoked.
            //// To solve this issue it will be better to create new Layout class and replace the existing one when dispatched to the Main thread.
            //// this.SetLayoutSource();
            this.GridView.UpdateService.DispatchOnUIThread(
                true,
                () =>
                {
                    if (this.CurrentDataProvider.Status != DataProviderStatus.Ready)
                    {
                        // This may happen if during the Dispatching on UI thread we have a change in the provider - e.g. ItemsSource change.
                        return;
                    }

                    if (this.hasPendingDataRefresh)
                    {
                        this.hasPendingDataRefresh = false;
                        this.RefreshData();
                        return;
                    }

                    this.updatingData = true;

                    this.GenerateColumns();
                    this.SetRowLayoutSource();

                    UpdateFlags uiUpdateflags = UpdateFlags.AffectsContent | UpdateFlags.AffectsDecorations;

                    // check the data update flags and create the appropriate UI update
                    if (this.dataChangeFlags.HasFlag(DataChangeFlags.Group) || this.dataChangeFlags.HasFlag(DataChangeFlags.Filter))
                    {
                        uiUpdateflags |= UpdateFlags.AffectsColumnsWidth;
                    }

                    var eventArgs = new DataBindingCompleteEventArgs() { ChangeFlags = this.dataChangeFlags, DataView = this.GridView.GetDataView() };

                    this.dataChangeFlags = DataChangeFlags.None;
                    this.isDataProviderUpdating = false;
                    this.updatingData = false;

                    this.GridView.CommandService.ExecuteCommand(CommandId.DataBindingComplete, eventArgs);

                    Debug.Assert(!uiUpdateflags.HasFlag(UpdateFlags.AffectsData), "The AffectsData flag should not be raised here!");
                    this.ResumeUpdateService(uiUpdateflags);

                    this.NotifyView(this.CurrentDataProvider.Status);
                });
        }

        private void ResumeUpdateService(UpdateFlags flags)
        {
            this.GridView.UpdateService.RegisterUpdate((int)flags);
            this.GridView.UpdateService.ResumeUpdates();
        }

        private void BeginDataUpdate()
        {
            // we will try to batch all subsequent updates
            if (!this.isDataProviderUpdating)
            {
                // suspend subsequent updates since we will need a Ready notification from the data engine to resume updates
                this.GridView.UpdateService.SuspendUpdates();

                this.isDataProviderUpdating = true;
            }
        }

        private void NotifyView(DataProviderStatus status)
        {
            this.GridView.UpdateService.DispatchOnUIThread(false, () => this.GridView.OnDataStatusChanged(status));
        }

        private void OnSortDescriptorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.dataChangeFlags |= DataChangeFlags.Sort;
            this.GridView.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsData);

            this.GridView.ProcessDataChangeFlags(this.dataChangeFlags);
        }

        private void OnGroupDescriptorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.dataChangeFlags |= DataChangeFlags.Group;
            this.GridView.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsData);

            this.GridView.ProcessDataChangeFlags(this.dataChangeFlags);
        }

        private void OnFilterDescriptorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.dataChangeFlags |= DataChangeFlags.Filter;
            this.GridView.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsData);

            this.GridView.ProcessDataChangeFlags(this.dataChangeFlags);
        }

        private void OnAggregateDescriptorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.dataChangeFlags |= DataChangeFlags.Aggregate;
            this.GridView.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsData);

            this.GridView.ProcessDataChangeFlags(this.dataChangeFlags);
        }

        private void CheckDataDescriptorsCompatibility()
        {
            this.CheckDataDescriptorsCompatibility(this.groupDescriptors);
            this.CheckDataDescriptorsCompatibility(this.sortDescriptors);
            this.CheckDataDescriptorsCompatibility(this.filterDescriptors);
        }

        private void CheckDataDescriptorsCompatibility(IList descriptors)
        {
            // group descriptors
            for (int i = 0; i < descriptors.Count; i++)
            {
                var descriptor = descriptors[i] as DataDescriptor;
                if (descriptor.IsDelegate)
                {
                    continue;
                }

                IDataFieldInfo field = null;

                if (this.CurrentDataProvider.FieldDescriptions != null)
                {
                    field = this.CurrentDataProvider.FieldDescriptions.GetFieldDescriptionByMember(descriptor.EngineDescription.PropertyName);
                }

                if (field == null || !field.Equals(descriptor.EngineDescription.MemberAccess as IDataFieldInfo))
                {
                    // descriptor is incompatible, remove it
                    descriptors.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}