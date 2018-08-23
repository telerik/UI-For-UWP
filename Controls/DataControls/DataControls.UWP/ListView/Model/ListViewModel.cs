using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Telerik.Core;
using Telerik.Core.Data;
using Telerik.Data.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Data.ListView;
using Telerik.UI.Xaml.Controls.Data.ListView.Commands;
using Telerik.UI.Xaml.Controls.Data.ListView.Model;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal partial class ListViewModel : IDisposable
    {
        internal static readonly DoubleArithmetics DoubleArithmetics = new DoubleArithmetics(IndexStorage.PrecisionMultiplier);

        internal bool pendingMeasure;
        internal LayoutController layoutController;

        private DataChangeFlags dataChangeFlags;
        private bool isDataProviderUpdating;
        private bool hasPendingDataRefresh;
        private LocalDataSourceProvider localDataProvider;
        private IDataProvider externalDataProvider;
        private HashSet<Group> collapsedGroups;
        private object itemsSource;
        private BatchLoadingMode dataLoadingMode;
        private int dataLoadingBufferSize = 10;
        private bool executeOperationsSyncroniously;
        private bool isCurrentItemSynchronizing = false;

        public ListViewModel(IListView view, bool shouldExecuteSyncroniously)
        {
            this.executeOperationsSyncroniously = shouldExecuteSyncroniously;
            this.View = view;

            this.BufferScale = 1;

            this.layoutController = new LayoutController(this.View, this);
            this.collapsedGroups = new HashSet<Group>();

            this.InitializeDescriptors();
        }

        public bool ShouldDisplayIncrementalLoadingIndicator
        {
            get
            {
                if (this.CurrentDataProvider != null)
                {
                    if (this.DataLoadingMode == BatchLoadingMode.Explicit || this.GroupDescriptors.Count > 0)
                    {
                        return this.View.CommandService.CanExecuteCommand(CommandId.LoadMoreData, new LoadMoreDataContext());
                    }
                }
                return false;
            }
        }

        public bool IsDataProviderUpdating
        {
            get
            {
                return this.isDataProviderUpdating;
            }
        }

        public int DataLoadingBufferSize
        {
            get
            {
                return this.dataLoadingBufferSize;
            }
            set
            {
                if (this.dataLoadingBufferSize != value)
                {
                    this.dataLoadingBufferSize = value;
                    this.SetLayoutSource();

                    this.View.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
                }
            }
        }

        public BatchLoadingMode DataLoadingMode
        {
            get
            {
                return this.dataLoadingMode;
            }
            set
            {
                if (this.dataLoadingMode != value)
                {
                    this.dataLoadingMode = value;
                    this.SetLayoutSource();

                    this.View.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
                }
            }
        }

        internal IListView View { get; set; }

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

        internal object ItemsSource
        {
            get
            {
                return this.itemsSource;
            }
        }

        internal double CurrentOffset { get; set; }

        internal int ItemsCount
        {
            get
            {
                return this.layoutController.strategy.Layout.GetLines(0, true).Count();
            }
        }

        public GeneratedItemModel GetDisplayedElement(int slot, int id)
        {
            return this.layoutController.strategy.GetDisplayedElement(slot, id);
        }

        public IEnumerable<GeneratedItemModel> ForEachDisplayedElement()
        {
            foreach (var pair in this.layoutController.GetDisplayedElements())
            {
                foreach (var model in pair.Value)
                {
                    yield return model;
                }
            }
        }

        public void Dispose()
        {
            this.localDataProvider.Dispose();
        }

        internal void OnItemsSourceChanged(object newSource)
        {
            if (this.itemsSource != null)
            {
                // TODO: Clean up items if it is required, else GC will do its work
            }

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

            // refresh the data
            this.RefreshLayout();
        }

        internal void SetLayoutSource()
        {
            if (this.CurrentDataProvider == null)
            {
                return;
            }

            int groupDescriptionCount = this.CurrentDataProvider.Settings.RowGroupDescriptions.Count;
            bool keepCollapsedState = (this.dataChangeFlags & DataChangeFlags.Group) == DataChangeFlags.None;

            if (this.ShouldDisplayIncrementalLoadingIndicator)
            {
                this.layoutController.Layout.AddStrategy(new IncrementalLoadingStrategy());
            }
            else
            {
                this.layoutController.Layout.RemoveWhere(c => c is IncrementalLoadingStrategy);
            }

            if (this.CurrentDataProvider.Results.Root.RowGroup != null)
            {
                this.layoutController.SetSource(this.CurrentDataProvider.Results.Root.RowGroup.Items, groupDescriptionCount, keepCollapsedState);
            }
        }

        internal void StopAllAnimations()
        {
            this.layoutController.StopAnimations();
        }

        internal void RefreshData()
        {
            if (this.CurrentDataProvider != null &&
                (this.CurrentDataProvider.Status == DataProviderStatus.Ready ||
                 this.CurrentDataProvider.Status == DataProviderStatus.DescriptionsReady))
            {
                this.CurrentDataProvider.DeferUpdates = false;

                this.CurrentDataProvider.Refresh(this.dataChangeFlags);

                this.View.UpdateService.RemoveUpdateFlag((int)ListView.UpdateFlags.AffectsData);
                this.CurrentDataProvider.DeferUpdates = true;
                this.View.CurrencyService.OnRefreshData();
            }
            else
            {
                this.hasPendingDataRefresh = true;
            }
        }

        internal void InvalidateCellsDesiredSize()
        {
        }

        internal RadSize MeasureContent(RadSize newAvailableSize)
        {
            var size = this.layoutController.MeasureContent(newAvailableSize);

            if (this.ShouldAutoRequestItems(null))
            {
                this.View.CommandService.ExecuteCommand(CommandId.LoadMoreData, new LoadMoreDataContext());
            }

            // NOTE: If we decide that we won't zero the length of collapsed rows then this 'TotalLineCount - 1' will be incorrect.
            // this.desiredSize = new RadSize(this.columnLayout.RenderInfo.OffsetFromIndex(this.columnLayout.TotalLineCount - 1), this.rowLayout.RenderInfo.OffsetFromIndex(this.rowLayout.TotalLineCount - 1));
            // return this.desiredSize;
            return size;
        }

        internal bool ShouldAutoRequestItems(uint? batchSize)
        {
            // In grouped scenario IncrementalLoading is Explicit regardless of its BathcLoadingMode.
            if (!this.isDataProviderUpdating && this.CurrentDataProvider != null && this.CurrentDataProvider.DataView != null && this.DataLoadingMode == BatchLoadingMode.Auto && this.GroupDescriptors.Count == 0)
            {
                if (this.CurrentDataProvider.DataView.BatchDataProvider != null)
                {
                    var lastRequestedIndex = this.GetIndexToRequest(batchSize, true);

                    return this.CurrentDataProvider.DataView.BatchDataProvider.ShouldRequestItems(lastRequestedIndex, this.DataLoadingBufferSize);
                }
            }

            return false;
        }

        internal void ArrangeContent(RadSize adjustedfinalSize)
        {
            this.layoutController.ArrangeContent(adjustedfinalSize);
        }

        internal void OnLayoutDefinitionChanged(LayoutDefinitionBase oldLayoutDefinition, LayoutDefinitionBase newLayoutDefinition)
        {
            this.layoutController.OnLayoutDefinitionChanged(oldLayoutDefinition, newLayoutDefinition);
        }

        internal void OnOrientationChanged(Orientation orientation)
        {
            this.layoutController.OnOrientationChanged(orientation);
        }

        internal void OnUpdate(ListView.UpdateFlags updateFlags)
        {
            this.layoutController.OnUpdate(updateFlags);
        }

        internal void UpdateRequestedItems(uint? batchSize, bool forceRequest)
        {
            if (this.CurrentDataProvider != null && this.CurrentDataProvider.DataView != null && this.CurrentDataProvider.DataView.BatchDataProvider != null)
            {
                var indexToRequest = this.GetIndexToRequest(batchSize, forceRequest);

                this.CurrentDataProvider.DataView.BatchDataProvider.RequestItems(indexToRequest, this.DataLoadingBufferSize);
            }
        }

        internal ItemInfo? FindItemInfo(object item)
        {
            if (!this.IsDataReady || item == null)
            {
                return null;
            }

            int index;
            bool isExpanded = true;

            if (item is IDataGroup)
            {
                index = this.layoutController.strategy.Layout.GetGroupInfo(item).Index;
            }
            else
            {
                int childIndex = -1;
                int groupIndex = 0;

                Telerik.Data.Core.DataGroup group = null;
                if (this.groupDescriptors.Count > 0)
                {
                    group = this.FindItemParentGroup(item);
                    if (group != null)
                    {
                        if (this.layoutController.strategy.Layout.IsCollapsed(group))
                        {
                            isExpanded = false;
                        }
                        else
                        {
                            groupIndex = this.layoutController.strategy.Layout.GetGroupInfo(group).Index + 1;
                            childIndex = group.IndexOf(item, this.CurrentDataProvider.ValueProvider.GetSortComparer());
                        }
                    }
                }
                else
                {
                    group = this.CurrentDataProvider.Results.Root.RowGroup as Telerik.Data.Core.DataGroup;
                    childIndex = group.IndexOf(item, this.CurrentDataProvider.ValueProvider.GetSortComparer());
                }

                index = groupIndex + childIndex;
            }

            if (!isExpanded)
            {
                return null;
            }

            index -= this.layoutController.strategy.Layout.GetCollapsedSlotsCount(0, index);
            index = this.layoutController.strategy.GetElementFlatIndex(index);

            return this.FindDataItemFromIndex(index, item);
        }

        internal Telerik.Data.Core.DataGroup FindItemParentGroup(object item)
        {
            if (!this.IsDataReady || item == null)
            {
                return null;
            }

            // no groups, item is expanded
            if (this.groupDescriptors.Count == 0)
            {
                return null;
            }

            // coordinate is struct so it cannot be empty.
            var coordinate = this.CurrentDataProvider.Results.Root;

            var rootGroup = coordinate.RowGroup as Group;
            if (rootGroup == null)
            {
                return null;
            }

            var currentGroup = rootGroup;

            for (int i = 0; i < this.groupDescriptors.Count; i++)
            {
                PropertyGroupDescriptionBase propertyGroupDescriptor = this.groupDescriptors[i].EngineDescription as PropertyGroupDescriptionBase;
                object groupName = propertyGroupDescriptor.GroupNameFromItem(item, i);

                Group subGroup;
                if (currentGroup.TryGetGroup(groupName, out subGroup))
                {
                    currentGroup = subGroup;
                }

                if (subGroup == null)
                {
                    Debug.Assert(false, "No group for item?");
                    return null;
                }
            }

            return currentGroup as Telerik.Data.Core.DataGroup;
        }

        internal void ScrollIndexIntoViewCore(ScrollIntoViewOperation<ItemInfo?> scrollOperation)
        {
            var update = new DelegateUpdate<UpdateFlags>(() =>
            {
                if (scrollOperation.ScrollAttempts < ScrollIntoViewOperation<ItemInfo?>.MaxScrollAttempts)
                {
                    if (this.IsIndexInView(scrollOperation))
                    {
                        if (scrollOperation.CompletedAction != null)
                        {
                            this.View.UpdateService.RegisterUpdate(new DelegateUpdate<UpdateFlags>(scrollOperation.CompletedAction) { Flags = UpdateFlags.AffectsScrollPosition });
                        }
                    }
                    else
                    {
                        this.ScrollIndexIntoView(scrollOperation);
                        scrollOperation.ScrollAttempts++;

                        this.ScrollIndexIntoViewCore(scrollOperation);
                    }
                }
            })
            {
                RequiresValidMeasure = true,
                Flags = UpdateFlags.AffectsScrollPosition
            };

            this.View.UpdateService.RegisterUpdate(update);
        }

        internal bool IsIndexInView(ScrollIntoViewOperation<ItemInfo?> operation)
        {
            var element = this.layoutController.strategy.GetDisplayedElement(operation.RequestedItem.Value.Slot, operation.RequestedItem.Value.Id);
            if (element == null)
            {
                return false;
            }

            var frozenContainersLength = 0;

            bool isIndexIntoView;
            if (this.View.Orientation == Orientation.Vertical)
            {
                isIndexIntoView = DoubleArithmetics.IsGreaterThanOrEqual(element.LayoutSlot.Y, this.View.ScrollOffset + frozenContainersLength) &&
                    DoubleArithmetics.IsLessThanOrEqual(element.LayoutSlot.Y + element.LayoutSlot.Height, this.View.ScrollOffset + this.View.ViewportHeight);
            }
            else
            {
                isIndexIntoView = DoubleArithmetics.IsGreaterThanOrEqual(element.LayoutSlot.X, this.View.ScrollOffset + frozenContainersLength) &&
                    DoubleArithmetics.IsLessThanOrEqual(element.LayoutSlot.X + element.LayoutSlot.Width, this.View.ScrollOffset + this.View.ViewportWidth);
            }

            return isIndexIntoView;
        }

        internal ItemInfo? FindDataItemFromIndex(int index, object dataItem = null)
        {
            var enumerator = this.layoutController.strategy.Layout.GetLines(index, true).GetEnumerator();

            ItemInfo? info = null;
            while (enumerator.MoveNext())
            {
                foreach (var item in enumerator.Current)
                {
                    if (item.ItemType == GroupType.BottomLevel)
                    {
                        if (dataItem == null || object.Equals(item.Item, dataItem))
                        {
                            info = item;
                            break;
                        }
                    }
                }

                if (info != null)
                {
                    break;
                }
            }

            return info;
        }

        internal void RecycleAllContainers()
        {
            this.layoutController.strategy.FullyRecycle();
        }

        internal void ScrollIndexIntoView(ScrollIntoViewOperation<ItemInfo?> operation)
        {
            var index = operation.RequestedItem.Value.Slot;
            var frozenContainersLength = 0;

            var itemLength = this.layoutController.strategy.Layout.PhysicalLengthForSlot(index);
            var offsetToScroll = this.layoutController.strategy.Layout.PhysicalOffsetFromSlot(index) - itemLength;

            if (DoubleArithmetics.IsLessThan(operation.InitialScrollOffset + frozenContainersLength, offsetToScroll))
            {
                if (index > 0)
                {
                    offsetToScroll -= this.View.Orientation == Orientation.Vertical ? this.View.ViewportHeight : this.View.ViewportWidth;
                    offsetToScroll += itemLength;
                }
            }
            else if (DoubleArithmetics.IsLessThanOrEqual(offsetToScroll, operation.InitialScrollOffset + frozenContainersLength))
            {
                offsetToScroll -= frozenContainersLength;
            }

            var scrollPosition = this.View.Orientation == Orientation.Vertical ? new RadPoint(this.View.ScrollOffset, Math.Max(0, offsetToScroll)) : new RadPoint(Math.Max(0, offsetToScroll), this.View.ScrollOffset);

            this.View.SetScrollPosition(scrollPosition, true, true);
        }

        private static IEnumerable<Group> EnumerataDataGroups(Group group)
        {
            if (!group.IsBottomLevel)
            {
                foreach (Group parent in group.Items)
                {
                    yield return parent;

                    var children = EnumerataDataGroups(parent);

                    foreach (var child in children)
                    {
                        yield return child;
                    }
                }
            }
        }

        private IEnumerable<Group> EnumerataDataGroups()
        {
            if (this.CurrentDataProvider != null)
            {
                var group = this.CurrentDataProvider.Results.Root.RowGroup as Group;

                if (group != null)
                {
                    return EnumerataDataGroups(group);
                }
            }

            return Enumerable.Empty<Group>();
        }

        private void SaveCollapsedState()
        {
            this.collapsedGroups.Clear();

            foreach (var group in this.EnumerataDataGroups())
            {
                if (this.layoutController.Layout.IsCollapsed(group))
                {
                    this.collapsedGroups.Add(group);
                }
            }
        }

        private void RestoreCollapsedState()
        {
            foreach (var group in this.EnumerataDataGroups())
            {
                if (this.collapsedGroups.Contains(group))
                {
                    this.layoutController.Layout.Collapse(group);
                }
            }

            this.collapsedGroups.Clear();
        }

        private void UpdateProviderField(IDataProvider provider)
        {
            provider.IsSingleThreaded = this.executeOperationsSyncroniously;
            provider.DeferUpdates = true;

            provider.StatusChanged += this.OnDataProviderStatusChanged;
            provider.ViewChanged += this.OnDataProviderViewChanged;
            provider.ViewChanging += this.OnDataProviderViewChanging;
            provider.CurrentChanged += this.OnDataProviderCurrentItemChanged;

            this.View.CurrencyService.CurrentChanged += this.CurrencyService_CurrentChanged;

            this.sortDescriptors.DescriptionCollection = provider.SortDescriptions;
            this.groupDescriptors.DescriptionCollection = provider.GroupDescriptions;
            this.filterDescriptors.DescriptionCollection = provider.FilterDescriptions;
            this.aggregateDescriptors.DescriptionCollection = provider.AggregateDescriptions;
        }

        private void CreateDefaultDataProvider()
        {
            this.localDataProvider = new LocalDataSourceProvider();
            this.UpdateProviderField(this.localDataProvider as IDataProvider);
        }

        private void CurrencyService_CurrentChanged(object sender, EventArgs e)
        {
            if (!this.isCurrentItemSynchronizing)
            {
                IDataSourceCurrency dsc = this.CurrentDataProvider.DataView as IDataSourceCurrency;
                if (dsc != null)
                {
                    dsc.ChangeCurrentItem(this.View.CurrencyService.CurrentItem);
                }
            }
        }

        private void OnDataProviderCurrentItemChanged(object sender, object e)
        {
            ICollectionView view = sender as ICollectionView;
            if (view != null)
            {
                this.isCurrentItemSynchronizing = true;
                this.View.CurrencyService.MoveCurrentTo(view.CurrentItem);
                this.isCurrentItemSynchronizing = false;
            }
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

        private void EndDataUpdate(DataProviderStatusChangedEventArgs e)
        {
            if (e.NewStatus == DataProviderStatus.Faulted || e.NewStatus == DataProviderStatus.Uninitialized)
            {
                this.layoutController.ScheduleCleanUp();
                this.ResumeUpdateService(ListView.UpdateFlags.AllButData);
                return;
            }

            if (!this.isDataProviderUpdating)
            {
                // System.Diagnostics.Debug.Assert(false, "Flag not raised properly.");
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
            this.View.UpdateService.DispatchOnUIThread(
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

                    this.SetLayoutSource();
                    this.RestoreCollapsedState();

                    ListView.UpdateFlags uiUpdateflags = ListView.UpdateFlags.AffectsContent;

                    this.dataChangeFlags = DataChangeFlags.None;
                    this.isDataProviderUpdating = false;

                    System.Diagnostics.Debug.Assert(!uiUpdateflags.HasFlag(ListView.UpdateFlags.AffectsData), "The AffectsData flag should not be raised here!");
                    this.ResumeUpdateService(uiUpdateflags);

                    this.NotifyView(this.CurrentDataProvider.Status);
                });
        }

        private void ResumeUpdateService(ListView.UpdateFlags flags)
        {
            this.View.UpdateService.RegisterUpdate((int)flags);
            this.View.UpdateService.ResumeUpdates();
        }

        private void BeginDataUpdate()
        {
            // we will try to batch all subsequent updates
            if (!this.isDataProviderUpdating)
            {
                // suspend subsequent updates since we will need a Ready notification from the data engine to resume updates
                this.View.UpdateService.SuspendUpdates();
                this.SaveCollapsedState();
                this.isDataProviderUpdating = true;
            }
        }

        private void NotifyView(DataProviderStatus status)
        {
            this.View.UpdateService.DispatchOnUIThread(false, () => this.View.OnDataStatusChanged(status));
        }

        private void RefreshLayout()
        {
            if (this.itemsSource != null && this.CurrentDataProvider != null &&
                (this.CurrentDataProvider.Status == DataProviderStatus.DescriptionsReady || this.CurrentDataProvider.Status == DataProviderStatus.Ready))
            {
                this.layoutController.strategy.IsItemsSourceChanging = true;
                this.RefreshData();
            }
            else
            {
                this.layoutController.ScheduleCleanUp();
            }
        }

        private int GetIndexToRequest(uint? batchSize, bool forceRequest)
        {
            if (batchSize.HasValue)
            {
                return (int)batchSize.Value - 1;
            }
            else if (forceRequest)
            {
                var generatedItemsLength = this.View.ScrollOffset + this.layoutController.AvailableLength;

                int slotToRequest = this.layoutController.Layout.TotalSlotCount > 0 ? (int)this.layoutController.Layout.SlotFromPhysicalOffset(generatedItemsLength, true) : 1;
                int itemsCountToRequest = this.layoutController.Layout.IndexFromSlot(slotToRequest);
                itemsCountToRequest = this.layoutController.Layout.TotalSlotCount > 0 ? itemsCountToRequest : (int)(generatedItemsLength / this.layoutController.Layout.DefaultItemLength);

                var indexToRequest = itemsCountToRequest - this.layoutController.Layout.GroupCount;
                return indexToRequest;
            }

            return (this.CurrentDataProvider.Results.Root.RowGroup == null || this.CurrentDataProvider.Results.Root.RowGroup.Items.Count == 0) && this.DataLoadingMode == BatchLoadingMode.Auto ? 0 : -1;
        }
    }
}