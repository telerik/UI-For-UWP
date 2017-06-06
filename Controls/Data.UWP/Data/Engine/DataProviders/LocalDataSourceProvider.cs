using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Telerik.Core.Data;
using Telerik.Data.Core.Engine;
using Telerik.Data.Core.Fields;
using Windows.Foundation.Collections;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Provides a data grouping access to local source such as an IList of instances of user defined classes.
    /// </summary>
    internal class LocalDataSourceProvider : DataProviderBase, IDisposable
    {
        protected bool refreshRequested;
        protected object itemsSource;
        protected IDataEngine engine;
        protected IFieldInfoData descriptionsData;

        protected DataSettings<PropertyFilterDescriptionBase, PropertyGroupDescriptionBase, PropertyAggregateDescriptionBase, SortDescription> settings;

        private ManualResetEventSlim manualResetEventSlim = new ManualResetEventSlim(true);
        private List<NotifyCollectionChangedEventArgs> pendingCollectionChanges = new List<NotifyCollectionChangedEventArgs>();
        private List<Tuple<object, PropertyChangedEventArgs>> pendingPropertyChanges = new List<Tuple<object, PropertyChangedEventArgs>>();

        private List<Tuple<WeakReference, List<PropertyChangedEventArgs>>> deferredItemsChanges = new List<Tuple<WeakReference, List<PropertyChangedEventArgs>>>();

        private IValueProvider valueProvider;

        private bool isSingleThreaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDataSourceProvider" /> class.
        /// </summary>
        public LocalDataSourceProvider() : this(new ParallelDataEngine())
        {
        }

        internal LocalDataSourceProvider(IDataEngine engine) : this(engine, null)
        {
        }

        internal LocalDataSourceProvider(IDataEngine engine, IFieldDescriptionProvider fieldInfoProvider) : this(new DataSettings<PropertyFilterDescriptionBase, PropertyGroupDescriptionBase, PropertyAggregateDescriptionBase, SortDescription>(), engine, fieldInfoProvider)
        {
        }

        internal LocalDataSourceProvider(DataSettings<PropertyFilterDescriptionBase, PropertyGroupDescriptionBase, PropertyAggregateDescriptionBase, SortDescription> settings, IDataEngine engine, IFieldDescriptionProvider fieldInfoProvider) : base(settings, fieldInfoProvider)
        {
            this.settings = settings;

            if (engine != null)
            {
                this.engine = engine;
                this.engine.Completed += new EventHandler<DataEngineCompletedEventArgs>(this.OnCompleted);
            }
        }

        /// <summary>
        /// Raised when data view has changed (e.g. Item was added/removed from collection).
        /// </summary>
        public override IValueProvider ValueProvider
        {
            get
            {
                return this.valueProvider;
            }
        }

        /// <inheritdoc />
        public override object State
        {
            get
            {
                return this.ItemsSource;
            }
        }

        public override IGroupFactory GroupFactory
        {
            get
            {
                if (this.settings.GroupFactory == null)
                {
                    this.settings.GroupFactory = new DataGroupFactory();
                }
                return this.settings.GroupFactory;
            }
            set
            {
                this.settings.GroupFactory = value;
            }
        }

        /// <summary>
        /// The item source for the grouping.
        /// </summary>
        public override object ItemsSource
        {
            get
            {
                return this.itemsSource;
            }

            set
            {
                if (this.itemsSource != value)
                {
                    this.itemsSource = value;

                    this.OnItemsSourceChanged(value);
                }
            }
        }

        /// <summary>
        /// A list of <see cref="PropertyFilterDescriptionBase"/> that specified how the data items should be filtered.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Design choice.")]
        public override Collection<PropertyFilterDescriptionBase> FilterDescriptions
        {
            get
            {
                return this.settings.FilterDescriptions;
            }
        }

        /// <summary>
        /// A list of <see cref="PropertyGroupDescriptionBase"/> that specified how items should be grouped.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Design choice.")]
        public override Collection<PropertyGroupDescriptionBase> GroupDescriptions
        {
            get
            {
                return this.settings.RowGroupDescriptions;
            }
        }

        /// <summary>
        /// A list of <see cref="PropertySortDescription"/> that specified how items should be sorted.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Design choice.")]
        public override Collection<SortDescription> SortDescriptions
        {
            get
            {
                return this.settings.SortDescriptions;
            }
        }

        /// <summary>
        /// A list of <see cref="PropertyAggregateDescription"/> that specified how the data should be aggregated.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Design choice.")]
        public override Collection<PropertyAggregateDescriptionBase> AggregateDescriptions
        {
            get
            {
                return this.settings.AggregateDescriptions;
            }
        }

        public override IFieldInfoData FieldDescriptions
        {
            get
            {
                return this.descriptionsData;
            }
        }

        /// <summary>
        /// The data wrapper around the items source.
        /// </summary>
        public override IDataSourceView DataView { get; set; }

        /// <summary>
        /// Gets or sets the execution mode of the data engine.
        /// When true data engine will execute all code in single (the calling) thread.
        /// When false data engine will execute asynchronously and in parallel.
        /// </summary>
        public override bool IsSingleThreaded
        {
            get
            {
                return this.isSingleThreaded;
            }
            set
            {
                this.isSingleThreaded = value;
            }
        }

        /// <inheritdoc />
        internal override IDataResults Results
        {
            get
            {
                return this.engine;
            }
        }

        /// <summary>
        /// Gets a list of <see cref="PropertyGroupDescriptionBase"/> that specified how the data should be grouped by rows.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Design choice.")]
        internal Collection<PropertyGroupDescriptionBase> RowGroupDescriptions
        {
            get
            {
                return this.settings.RowGroupDescriptions;
            }
        }

        /// <summary>
        /// Gets a list of <see cref="PropertyGroupDescriptionBase"/> that specified how the data should be grouped by columns.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Design choice.")]
        internal Collection<PropertyGroupDescriptionBase> ColumnGroupDescriptions
        {
            get
            {
                return this.settings.ColumnGroupDescriptions;
            }
        }

        protected bool AreDescriptionsReady
        {
            get
            {
                return this.FieldDescriptionsProvider != null && this.FieldDescriptionsProvider.IsReady;
            }
        }
 
        /// <inheritdoc />
        public override void BlockUntilRefreshCompletes()
        {
            this.engine.WaitForParallel();
        }

        public override void SuspendPropertyChanges(object item)
        {
            if (!this.deferredItemsChanges.Where(c => c.Item1.IsAlive && c.Item1.Target == item).Any())
            {
                this.deferredItemsChanges.Add(new Tuple<WeakReference, List<PropertyChangedEventArgs>>(new WeakReference(item), new List<PropertyChangedEventArgs>()));
            }
        }

        public override void ResumePropertyChanges(object item)
        {
            var pair = this.deferredItemsChanges.Where(c => c.Item1.IsAlive && c.Item1.Target == item).FirstOrDefault();

            this.deferredItemsChanges.RemoveAll(c => !c.Item1.IsAlive || c.Item1.Target == item);

            if (pair != null)
            {
                foreach (var change in pair.Item2)
                {
                    this.ProcessPropertyChanged(pair.Item1.Target, change);
                }
            }
        }

        /// <summary>
        /// Local data source provider has nothing especially to do during the editing operations.
        /// </summary>
        public override void BeginEditOperation(object item)
        {
            this.SuspendPropertyChanges(item);
        }

        public override void CancelEditOperation(object item)
        {
            this.ResumePropertyChanges(item);
        }

        public override void CommitEditOperation(object item)
        {
            this.ResumePropertyChanges(item);
        }

        public Collection<PropertyFilterDescriptionBase> GetFilterDescriptions()
        {
            return this.settings.FilterDescriptions;
        }

        public Collection<PropertyGroupDescriptionBase> GetGroupDescriptions()
        {
            return this.settings.RowGroupDescriptions;
        }

        public Collection<SortDescription> GetSortDescriptions()
        {
            return this.settings.SortDescriptions;
        }

        public Collection<PropertyAggregateDescriptionBase> GetAggregateDescriptions()
        {
            return this.settings.AggregateDescriptions;
        }

        public Collection<PropertyGroupDescriptionBase> GetRowGroupDescriptions()
        {
            return this.settings.RowGroupDescriptions;
        }

        public Collection<PropertyGroupDescriptionBase> GetColumnGroupDescriptions()
        {
            return this.settings.ColumnGroupDescriptions;
        }

        public void Dispose()
        {
            this.manualResetEventSlim.Dispose();
        }

        /// <inheritdoc />
        internal override IFieldDescriptionProvider CreateFieldDescriptionsProvider()
        {
            return new LocalDataSourceFieldDescriptionsProvider();
        }

        /// <inheritdoc />
        internal override FilterDescription GetFilterDescriptionForFieldDescription(IDataFieldInfo description)
        {
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            return new PropertyFilterDescription() { PropertyName = description.Name };
        }

        /// <inheritdoc />
        internal override IAggregateDescription GetAggregateDescriptionForFieldDescription(IDataFieldInfo description)
        {
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            var aggregateDescription = new PropertyAggregateDescription() { PropertyName = description.Name };

            if (!FieldInfoHelper.IsNumericType(description.DataType))
            {
                ////aggregateDescription.AggregateFunction = AggregateFunctions.Count;
            }

            return aggregateDescription;
        }

        /// <inheritdoc />
        internal override SortDescription GetSortDescriptionForFieldDescription(IDataFieldInfo description)
        {
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            return new PropertySortDescription() { PropertyName = description.Name };
        }

        /// <inheritdoc />
        internal override IGroupDescription GetGroupDescriptionForFieldDescription(IDataFieldInfo description)
        {
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            return new PropertyGroupDescription() { PropertyName = description.Name };
        }

        /// <inheritdoc />
        internal override IEnumerable<object> GetAggregateFunctionsForAggregateDescription(IAggregateDescription aggregateDescription)
        {
            yield break;
        }

        /// <inheritdoc />
        internal override void SetAggregateFunctionToAggregateDescription(IAggregateDescription aggregateDescription, object aggregateFunction)
        {
            PropertyAggregateDescriptionBase padb = aggregateDescription as PropertyAggregateDescriptionBase;
            AggregateFunction af = aggregateFunction as AggregateFunction;
            if (padb != null && af != null)
            {
                padb.AggregateFunction = af;
            }
        }

        /// <inheritdoc />
        protected override void RefreshOverride(DataChangeFlags dataChangeFlags)
        {
            if (this.ItemsSource == null)
            {
                return;
            }

            if (this.descriptionsData == null)
            {
                this.refreshRequested = true;
                this.RefreshFieldDescriptions();

                return;
            }

            this.InitializeDescriptions();
            this.GenerateAndExecuteDataEngineRequest();
        }

        protected void OnItemsSourceChanged(object newValue)
        {
            if (newValue == null)
            {
                var state = this.GenerateParallelState();
                this.engine.Clear(state); 
            }

            this.RefreshInternalView(true);
            this.RefreshFieldDescriptions();
            this.Invalidate(); /* ItemsSource changed */
        }

        protected void GenerateAndExecuteDataEngineRequest()
        {
            this.OnStatusChanged(new DataProviderStatusChangedEventArgs(DataProviderStatus.ProcessingData, false, null));

            // rebuild the current view
            var parallelInitialState = this.GenerateParallelState();
            if (this.IsSingleThreaded)
            {
                this.engine.RebuildCube(parallelInitialState);
            }
            else
            {
                this.engine.RebuildCubeParallel(parallelInitialState);
            }

            this.valueProvider = parallelInitialState.ValueProvider;
            this.refreshRequested = false;
        }

        protected void InitializeDescriptions()
        {
            // TODO: The member access should be assigned when the fields provider change or the settings report new description is added.
            InitializeMemberAccess(this.SortDescriptions, this.descriptionsData);
            InitializeMemberAccess(this.RowGroupDescriptions, this.descriptionsData);
            InitializeMemberAccess(this.ColumnGroupDescriptions, this.descriptionsData);
            InitializeMemberAccess(this.AggregateDescriptions, this.descriptionsData);
            InitializeMemberAccess(this.FilterDescriptions, this.descriptionsData);
        }

        protected void RefreshInternalView(bool resetStatus)
        {
            if (this.DataView != null)
            {
                if (this.DataView.BatchDataProvider != null)
                {
                    this.DataView.BatchDataProvider.StatusChanged -= this.BatchProvider_StatusChanged;
                    this.DataView.BatchDataProvider.Dispose();
                }

                this.DataView.CollectionChanged -= this.DataView_CollectionChanged;
                this.DataView.CollectionChanging -= this.DataView_CollectionChanging;
                this.DataView.ItemPropertyChanged -= this.DataView_ItemPropertyChanged;
                IDataSourceCurrency dataViewCurrency = this.DataView as IDataSourceCurrency;
                if (dataViewCurrency != null)
                {
                    dataViewCurrency.CurrentChanged -= this.DataView_CurrentChanged;
                }
                this.DataView.Dispose();
            }

            if (resetStatus)
            {
                this.ResetStatus();
            }

            this.DataView = DataSourceViewFacotry.CreateDataSourceView(this.ItemsSource ?? Enumerable.Empty<object>());

            if (this.DataView.BatchDataProvider != null)
            {
                this.DataView.BatchDataProvider.StatusChanged += this.BatchProvider_StatusChanged;
            }

            this.DataView.CollectionChanged += this.DataView_CollectionChanged;
            this.DataView.CollectionChanging += this.DataView_CollectionChanging;
            this.DataView.ItemPropertyChanged += this.DataView_ItemPropertyChanged;
            IDataSourceCurrency dsc = this.DataView as IDataSourceCurrency;
            if (dsc != null)
            {
                dsc.CurrentChanged += this.DataView_CurrentChanged;
            }
        }

        protected void RaiseViewChanged(List<AddRemoveResult> changes, CollectionChange action)
        {
            base.RaiseViewChanged(this, changes, action);
        }

        protected void RefreshFieldDescriptions()
        {
            this.descriptionsData = null;

            IDataProvider provider = this;
            if (this.FieldDescriptionsProvider != null && !this.FieldDescriptionsProvider.IsBusy)
            {
                if (this.ItemsSource != null)
                {
                    this.OnStatusChanged(new DataProviderStatusChangedEventArgs(DataProviderStatus.Initializing, false, null));
                   
                    provider.FieldDescriptionsProvider.GetDescriptionsDataAsyncCompleted += this.FieldDescriptionsProvider_GetDescriptionsDataAsyncCompleted;
                }

                provider.FieldDescriptionsProvider.GetDescriptionsDataAsync(this.State);
            }
        }

        private static void InitializeMemberAccess(IEnumerable<DescriptionBase> descriptions, IFieldInfoData fieldInfoData)
        {
            IMemberAccess access;
            foreach (var item in descriptions)
            {
                access = fieldInfoData.GetFieldDescriptionByMember(item.GetMemberName()) as IMemberAccess;
                if (access != null)
                {
                    item.MemberAccess = access;
                }
            }
        }

        private void DataView_CollectionChanging(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                this.RaiseViewChanging(this, e.OldItems, CollectionChange.ItemRemoved);
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                this.RaiseViewChanging(this, e.NewItems, CollectionChange.ItemInserted);
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.RaiseViewChanging(this, e.OldItems, CollectionChange.Reset);
            }
        }

        private void OnCompleted(object sender, DataEngineCompletedEventArgs e)
        {
            var newStatus = DataProviderBase.GetDataProviderStatusFromEngineStatus(e.Status);
            var exception = Enumerable.FirstOrDefault(e.InnerExceptions);

            if (newStatus == DataProviderStatus.Ready)
            {
                try
                {
                    this.manualResetEventSlim.Reset();
                    this.ProcessPendingChanges();
                }
                finally
                {
                    this.manualResetEventSlim.Set();
                }
            }
            else if (newStatus == DataProviderStatus.Faulted)
            {
                try
                {
                    this.manualResetEventSlim.Reset();
                    if (!this.AreDescriptionsReady)
                    {
                        this.ResetDescriptions();
                    }

                    this.ResetPendingChanges();
                }
                finally
                {
                    this.manualResetEventSlim.Set();
                }
            }

            this.OnStatusChanged(new DataProviderStatusChangedEventArgs(newStatus, true, exception));
        }

        private void ProcessPendingChanges()
        {

            while (this.pendingCollectionChanges.Count > 0)
            {
                var pch = this.pendingCollectionChanges[0];
                this.pendingCollectionChanges.RemoveAt(0);
                this.ProcessCollectionChanged(pch);
            }

            while (this.pendingPropertyChanges.Count > 0)
            {
                var tuple = this.pendingPropertyChanges[0];
                this.pendingPropertyChanges.RemoveAt(0);
                this.ProcessPropertyChanged(tuple.Item1, tuple.Item2);
            }
        }

        private void BatchProvider_StatusChanged(object sender, BatchLoadingEventArgs e)
        {
            switch (e.Status)
            {
                case BatchLoadingStatus.ItemsRequested:
                    this.OnStatusChanged(new DataProviderStatusChangedEventArgs(DataProviderStatus.RequestingData, false, null));
                    break;
                case BatchLoadingStatus.ItemsLoaded:
                    this.OnStatusChanged(new DataProviderStatusChangedEventArgs(DataProviderStatus.DataLoadingCompleted, true, null));
                    this.Refresh();
                    break;
                case BatchLoadingStatus.ItemsLoadFailed:
                    this.OnStatusChanged(new DataProviderStatusChangedEventArgs(DataProviderStatus.Faulted, false, null));
                    break;
                default:
                    break;
            }
        }

        private void DataView_ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.refreshRequested)
            {
                return;
            }

            // We use Ready for DataProceesed and DescriptionsRetreived. If DescriptionsRetreived becomes async (like in OLAP) this will fail
            // so we need to add another state in DataProviderStatus enum.
            this.manualResetEventSlim.Wait();
            if (this.Status == DataProviderStatus.Ready)
            {
                this.ProcessPropertyChanged(sender, e);
            }
            else
            {
                this.pendingPropertyChanges.Add(Tuple.Create(sender, e));
            }
        }

        private void ProcessPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var deferredInfo = this.deferredItemsChanges.Where(c => c.Item1.IsAlive && c.Item1.Target == sender).FirstOrDefault();
            if (deferredInfo != null)
            {
                deferredInfo.Item2.Add(e);
                return;
            }

            string propertyName = e.PropertyName;

            Func<DescriptionBase, bool> predicate = d => object.Equals(d.PropertyName, propertyName);

            bool isSorted = this.SortDescriptions.Any(predicate);

            bool isGrouped = this.GroupDescriptions.Any(predicate);
            bool isAggregate = this.AggregateDescriptions.Any(predicate);

            // TODO: We have delegate filters only and do not have the knowledge about a property for filter.
            bool isFiltered = this.FilterDescriptions.Any();
            bool doExhaustiveSearch = isFiltered || isGrouped || isSorted;

            if (isSorted || isGrouped || isAggregate || isFiltered)
            {
                // We need to do exhaustiveSearch if Filter, Group or Sort property has changed.
                // As we don't have propertyName on FilterDescription we do search if we have any filter
                int itemIndex;
                if (isSorted || isGrouped || isAggregate)
                {
                    itemIndex = -1;
                }
                else
                {
                    itemIndex = this.DataView.InternalList.IndexOf(sender);
                }

                List<object> items = new List<object>(1) { sender };

                // If changed property is sorted it will no longer be at the correct sorted position.
                this.RemoveItems(itemIndex, items, doExhaustiveSearch, false);
                this.InsertItems(itemIndex, items);
            }
            else
            {
                // NOTE: We use empty AddRemoveResult because on ItemChanged we do RecycleAll.
                // If later we decide that we want single change then this will have to change.
                this.RaiseViewChanged(null, CollectionChange.ItemChanged);
            }
        }

        private void DataView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.refreshRequested)
            {
                return;
            }

            // We use Ready for DataProceesed and DescriptionsRetreived. If DescriptionsRetreived becomes async (like in OLAP) this will fail
            // so we need to add another state in DataProviderStatus enum.
            this.manualResetEventSlim.Wait();
            if (this.Status == DataProviderStatus.Ready)
            {
                this.ProcessCollectionChanged(e);
            }
            else
            {
                this.pendingCollectionChanges.Add(e);
            }
        }

        private void DataView_CurrentChanged(object sender, object e)
        {
            this.RaiseCurrentChanged(sender, e);
        }

        private void ProcessCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!this.AreDescriptionsReady)
            {
                this.ResetDescriptions();
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.InsertItems(e.NewStartingIndex, e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.RemoveItems(e.OldStartingIndex, e.OldItems, false, true);
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    this.RemoveItems(e.OldStartingIndex, e.OldItems, false, true);
                    this.InsertItems(e.NewStartingIndex, e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.pendingCollectionChanges.Clear();
                    var state = this.GenerateParallelState();
                    this.engine.Clear(state);
                    this.RefreshInternalView(false);
                    this.RaiseViewChanged(null, CollectionChange.Reset);
                    this.refreshRequested = true;
                    break;
                default:
                    break;
            }
        }

        private void ResetDescriptions()
        {
            this.ResetPendingChanges();

            this.ResetFieldDescriptionsProvider();
            this.ResetStatus();
            this.RefreshFieldDescriptions();
            this.Invalidate();

            this.OnFieldDescriptionsChanged();
        }

        private void ResetPendingChanges()
        {
            this.pendingCollectionChanges.Clear();
            this.pendingPropertyChanges.Clear();
        }

        private void InsertItems(int newStartingIndex, IList newItems)
        {
            List<AddRemoveResult> changes = this.engine.InsertItems(newStartingIndex, newItems);
            this.RaiseViewChanged(changes, CollectionChange.ItemInserted);
        }

        private void RemoveItems(int oldStartingIndex, IList oldItems, bool doExhaustiveSearch, bool canUseComparer)
        {
            List<AddRemoveResult> changes = this.engine.RemoveItems(oldStartingIndex, oldItems, doExhaustiveSearch, canUseComparer);
            this.RaiseViewChanged(changes, CollectionChange.ItemRemoved);
        }

        private void ResetStatus()
        {
            this.OnStatusChanged(new DataProviderStatusChangedEventArgs(DataProviderStatus.Uninitialized, true, null));
        }

        private void FieldDescriptionsProvider_GetDescriptionsDataAsyncCompleted(object sender, GetDescriptionsDataCompletedEventArgs e)
        {
            var filedDescriptionProvider = sender as IFieldDescriptionProvider;

            this.StopListeningForGetDescriptionsData(filedDescriptionProvider);

            IDataProvider provider = this;
            if (provider.FieldDescriptionsProvider != filedDescriptionProvider)
            {
                return;
            }

            if (e.Error == null)
            {
                this.descriptionsData = e.DescriptionsData;
                this.InitializeDescriptions();

                this.OnStatusChanged(new DataProviderStatusChangedEventArgs(DataProviderStatus.DescriptionsReady, false, null));

                if (this.refreshRequested)
                {
                    this.Refresh();
                }
            }
            else
            {
                this.OnStatusChanged(new DataProviderStatusChangedEventArgs(DataProviderStatus.Uninitialized, false, e.Error));
            }
        }

        private void StopListeningForGetDescriptionsData(IFieldDescriptionProvider filedDescriptionProvider)
        {
            if (filedDescriptionProvider != null)
            {
                filedDescriptionProvider.GetDescriptionsDataAsyncCompleted -= this.FieldDescriptionsProvider_GetDescriptionsDataAsyncCompleted;
            }
        }

        private ParallelState GenerateParallelState()
        {
            var sortDescriptionClones = Enumerable.ToList(Enumerable.Select(this.SortDescriptions, (l) => (SortDescription)l.Clone()));
            var rowGroupDescriptionClones = Enumerable.ToList(Enumerable.Select(this.RowGroupDescriptions, (l) => (PropertyGroupDescriptionBase)l.Clone()));
            var columnGroupDescriptionClones = Enumerable.ToList(Enumerable.Select(this.ColumnGroupDescriptions, (l) => (PropertyGroupDescriptionBase)l.Clone()));
            var aggregateDescriptionClones = Enumerable.ToList(Enumerable.Select(this.AggregateDescriptions, (l) => (PropertyAggregateDescriptionBase)l.Clone()));
            var filterDescriptionClones = Enumerable.ToList(Enumerable.Select(this.FilterDescriptions, (l) => (PropertyFilterDescriptionBase)l.Clone()));

            IComparer<object> sortComparer = sortDescriptionClones.Count > 0 ? new SortFieldComparer(sortDescriptionClones, null) : null;

            var provider = new LocalSourceValueProvider()
            {
                RowGroupDescriptions = rowGroupDescriptionClones,
                ColumnGroupDescriptions = columnGroupDescriptionClones,
                AggregateDescriptions = aggregateDescriptionClones,
                FilterDescriptions = filterDescriptionClones,
                SortComparer = sortComparer,
                GroupFactory = this.GroupFactory
            };

            ParallelState initalState = new ParallelState()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                TaskScheduler = System.Threading.Tasks.TaskScheduler.Default,
                RowGroupDescriptions = rowGroupDescriptionClones,
                ColumnGroupDescriptions = columnGroupDescriptionClones,
                AggregateDescriptions = aggregateDescriptionClones,
                FilterDescriptions = filterDescriptionClones,
                ValueProvider = provider,
                DataView = this.DataView
            };

            return initalState;
        }

        private class LocalSourceValueProvider : IValueProvider
        {
            public IGroupFactory GroupFactory { get; set; }

            public IComparer<object> SortComparer { get; set; }

            public IReadOnlyList<PropertyGroupDescriptionBase> RowGroupDescriptions { get; set; }

            public IReadOnlyList<PropertyGroupDescriptionBase> ColumnGroupDescriptions { get; set; }

            public IReadOnlyList<PropertyAggregateDescriptionBase> AggregateDescriptions { get; set; }

            public IReadOnlyList<PropertyFilterDescriptionBase> FilterDescriptions { get; set; }

            Tuple<GroupComparer, SortOrder> IValueProvider.GetRowGroupNameComparerAndSortOrder(int level)
            {
                IGroupDescription description = this.RowGroupDescriptions[level];
                return Tuple.Create(description.GroupComparer, description.SortOrder);
            }

            Tuple<GroupComparer, SortOrder> IValueProvider.GetColumnGroupNameComparerAndSortOrder(int level)
            {
                IGroupDescription description = this.ColumnGroupDescriptions[level];
                return Tuple.Create(description.GroupComparer, description.SortOrder);
            }

            IEnumerable IValueProvider.GetRowGroupNames(object item)
            {
                for (int level = 0; level < this.RowGroupDescriptions.Count; level++)
                {
                    yield return this.RowGroupDescriptions[level].GroupNameFromItem(item, level);
                }
            }

            IEnumerable IValueProvider.GetColumnGroupNames(object item)
            {
                for (int level = 0; level < this.ColumnGroupDescriptions.Count; level++)
                {
                    yield return this.ColumnGroupDescriptions[level].GroupNameFromItem(item, level);
                }
            }

            object IValueProvider.GetAggregateValue(int index, object item)
            {
                return this.AggregateDescriptions[index].GetValueForItem(item);
            }

            AggregateValue IValueProvider.CreateAggregateValue(int index)
            {
                // NOTE: If we dispatch the AggregateValue through the IValueProvider anyway we could remove the CreateAggregate from the AggregateDescriptionBase and use the following:
                // return this.AggregateDescriptions[index].AggregateFunction.CreateAggregate();
                return this.AggregateDescriptions[index].CreateAggregate();
            }

            string IValueProvider.GetAggregateStringFormat(int index)
            {
                return this.AggregateDescriptions[index].GetEffectiveFormat();
            }

            int IValueProvider.GetFiltersCount()
            {
                return this.FilterDescriptions == null ? 0 : this.FilterDescriptions.Count;
            }

            object[] IValueProvider.GetFilterItems(object fact)
            {
                int descriptionsCount = ((IValueProvider)this).GetFiltersCount();
                if (descriptionsCount == 0)
                {
                    return null;
                }
                else
                {
                    object[] filterItems = new object[descriptionsCount];

                    for (int i = 0; i < descriptionsCount; i++)
                    {
                        var filterDescription = this.FilterDescriptions[i];
                        filterItems[i] = filterDescription.GetFilterItem(fact);
                    }

                    return filterItems;
                }
            }

            bool IValueProvider.PassesFilter(object[] items)
            {
                int descriptionsCount = ((IValueProvider)this).GetFiltersCount();
                int itemsCount = items == null ? 0 : items.Length;

                if (itemsCount != descriptionsCount)
                {
                    throw new ArgumentException("Length should be the same as of the FilterDescriptions.", "items");
                }

                for (int i = 0; i < descriptionsCount; i++)
                {
                    var filterDescription = this.FilterDescriptions[i];
                    var filterItem = items[i];

                    if (!filterDescription.PassesFilter(filterItem))
                    {
                        return false;
                    }
                }

                return true;
            }

            IReadOnlyList<IAggregateDescription> IValueProvider.GetAggregateDescriptions()
            {
                return this.AggregateDescriptions;
            }

            IComparer<object> IValueProvider.GetSortComparer()
            {
                return this.SortComparer;
            }

            IGroupFactory IValueProvider.GetGroupFactory()
            {
                return this.GroupFactory;
            }
        }
    }
}