using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Telerik.Data.Core.Engine;
using Telerik.Data.Core.Fields;
using Windows.Foundation.Collections;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Base implementation of <see cref="IDataProvider"/>.
    /// </summary>
    internal abstract partial class DataProviderBase : IDataProvider, INotifyPropertyChanged, ISupportInitialize
    {
        // NOTE: These are available in the SettingsNode... Perhaps we could make the DataProviderBase a SettingsNode and drop the reimplementation here?
        private bool invalidated;
        private int deferLevel;
        private bool isInitializing;
        private bool deferUpdates;

        private DataProviderStatus status;
        private IFieldDescriptionProvider fieldDescriptionsProvider;

        internal DataProviderBase(IDataSettings settings, IFieldDescriptionProvider fieldInfoProvider)
        {
            this.Settings = settings;
            this.Settings.SettingsChanged += this.OnDataSettingsChanged;
            this.Settings.PropertyChanged += this.OnDataSettingsPropertyChanged;
            this.Status = DataProviderStatus.Uninitialized;
            this.fieldDescriptionsProvider = fieldInfoProvider;
        }

        internal DataProviderBase(IDataSettings settings) : this(settings, null)
        {
        }

        /// <inheritdoc />
        public event EventHandler<DataProviderStatusChangedEventArgs> StatusChanged;

        /// <inheritdoc />
        public event EventHandler FieldDescriptionsChanged;

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        public abstract IDataSourceView DataView { get; set; }

        public abstract Collection<PropertyFilterDescriptionBase> FilterDescriptions { get; }

        public abstract Collection<PropertyGroupDescriptionBase> GroupDescriptions { get; }

        public abstract Collection<SortDescription> SortDescriptions { get; }

        public abstract Collection<PropertyAggregateDescriptionBase> AggregateDescriptions { get; }

        public abstract IValueProvider ValueProvider { get; }

        /// <inheritdoc />
        public IFieldDescriptionProvider FieldDescriptionsProvider
        {
            get
            {
                if (this.fieldDescriptionsProvider == null)
                {
                    this.fieldDescriptionsProvider = this.CreateFieldDescriptionsProvider();
                }

                return this.fieldDescriptionsProvider;
            }

            set
            {
                var oldProvider = this.fieldDescriptionsProvider;
                this.fieldDescriptionsProvider = value;
                this.OnFieldDescriptionsProviderChanged(oldProvider, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether changes to this <see cref="IDataProvider"/> will trigger automatic <see cref="Refresh"/>.
        /// </summary>
        public bool DeferUpdates
        {
            get
            {
                return this.deferUpdates;
            }
            set
            {
                if (this.deferUpdates != value)
                {
                    this.deferUpdates = value;
                    this.OnPropertyChanged(nameof(this.DeferUpdates));
                }
            }
        }

        /// <inheritdoc />
        public abstract object State { get; }

        /// <inheritdoc />
        public DataProviderStatus Status
        {
            get
            {
                return this.status;
            }

            private set
            {
                if (this.status != value)
                {
                    this.status = value;
                    this.OnPropertyChanged(nameof(this.Status));
                }
            }
        }

        /// <inheritdoc />
        internal DataAxis AggregatesPosition
        {
            get
            {
                return this.Settings.AggregatesPosition;
            }

            set
            {
                this.Settings.AggregatesPosition = value;
            }
        }

        /// <inheritdoc />
        internal int AggregatesLevel
        {
            get
            {
                return this.Settings.AggregatesLevel;
            }

            set
            {
                this.Settings.AggregatesLevel = value;
            }
        }

        internal abstract IDataResults Results { get; }

        /// <inheritdoc />
        internal IDataSettings Settings { get; private set; }

        /// <inheritdoc />
        public void BeginInit()
        {
            if (this.isInitializing)
            {
                throw new InvalidOperationException("Nested BeginInit is not supported. Use DeferRefresh() instead.");
            }

            this.isInitializing = true;
        }

        /// <inheritdoc />
        public void EndInit()
        {
            if (!this.isInitializing)
            {
                throw new InvalidOperationException("EndInit without BeginInit is not supported.");
            }

            this.isInitializing = false;
            this.OnInitializationCompleted();
        }

        /// <inheritdoc />
        public void Refresh(DataChangeFlags dataChangeFlags = DataChangeFlags.None)
        {
            try
            {
                this.RefreshOverride(dataChangeFlags);
            }
            finally
            {
                this.ClearPendingChanges();
            }
        }

        /// <inheritdoc />
        public abstract void BlockUntilRefreshCompletes();

        /// <summary>
        /// Enters a defer cycle that you can use to merge changes to the provider and delay automatic refresh.
        /// </summary>
        /// <returns>An <see cref="IDisposable"/> object that you can use to dispose of the calling object.</returns>
        public IDisposable DeferRefresh()
        {
            this.deferLevel++;
            return new DeferHelper(this);
        }

        internal static DataProviderStatus GetDataProviderStatusFromEngineStatus(DataEngineStatus engineStatus)
        {
            DataProviderStatus status;

            switch (engineStatus)
            {
                case DataEngineStatus.InProgress:
                    status = DataProviderStatus.ProcessingData;
                    break;
                case DataEngineStatus.Completed:
                    status = DataProviderStatus.Ready;
                    break;
                case DataEngineStatus.Faulted:
                default:
                    status = DataProviderStatus.Faulted;
                    break;
            }

            return status;
        }

        /// <summary>
        /// Called when FieldDescriptionsProvider is changed.
        /// </summary>
        internal virtual void OnFieldDescriptionsProviderChanged(IFieldDescriptionProvider oldProvider, IFieldDescriptionProvider newProvider)
        {
            this.OnPropertyChanged(nameof(this.FieldDescriptionsProvider));
            if (this.deferLevel == 0 && !this.deferUpdates)
            {
                this.Refresh();
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="IFieldDescriptionProvider"/> for this <see cref="IDataProvider"/>.
        /// </summary>
        internal abstract IFieldDescriptionProvider CreateFieldDescriptionsProvider();

        /// <inheritdoc />
        internal abstract IAggregateDescription GetAggregateDescriptionForFieldDescription(IDataFieldInfo description);

        /// <inheritdoc />
        internal abstract IGroupDescription GetGroupDescriptionForFieldDescription(IDataFieldInfo description);

        /// <inheritdoc />
        internal abstract FilterDescription GetFilterDescriptionForFieldDescription(IDataFieldInfo description);

        /// <inheritdoc />
        internal abstract SortDescription GetSortDescriptionForFieldDescription(IDataFieldInfo description);

        /// <inheritdoc />
        internal abstract IEnumerable<object> GetAggregateFunctionsForAggregateDescription(IAggregateDescription aggregateDescription);

        /// <inheritdoc />
        internal abstract void SetAggregateFunctionToAggregateDescription(IAggregateDescription aggregateDescription, object aggregateFunction);

        /// <summary>
        /// Notify that changes were applied that would alter the data results.
        /// Queues an automatic <see cref="Refresh"/>.
        /// </summary>
        protected void Invalidate()
        {
            if (!this.invalidated)
            {
                this.AddPendingChange();
                this.OnEditCompleted();
            }
        }

        protected void ResetFieldDescriptionsProvider()
        {
            this.fieldDescriptionsProvider = null;
        }

        protected void OnFieldDescriptionsChanged()
        {
            var eh = this.FieldDescriptionsChanged;
            if (eh != null)
            {
                eh(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Recreates the <see cref="Results" />.
        /// </summary>
        protected abstract void RefreshOverride(DataChangeFlags dataChangeFlags);

        protected void RaiseViewChanged(object sender, List<AddRemoveResult> changes, CollectionChange action)
        {
            if (this.ViewChanged != null)
            {
                this.ViewChanged(sender, new ViewChangedEventArgs(changes, action));
            }
        }

        protected void RaiseViewChanging(object sender, IList changedItems, CollectionChange action)
        {
            if (this.ViewChanging != null)
            {
                this.ViewChanging(sender, new ViewChangingEventArgs(changedItems, action));
            }
            else
            {
                if (action == CollectionChange.ItemRemoved || action == CollectionChange.Reset)
                {
                    this.DataView.ProcessPendingCollectionChange();
                }
            }
        }

        protected void RaiseCurrentChanged(object sender, object e)
        {
            var handler = this.CurrentChanged;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        /// Raises the Completed event.
        /// </summary>
        protected virtual void OnStatusChanged(DataProviderStatusChangedEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args", "DataProviderStatusChangedEventArgs shoud not be null");
            }
            if (this.Status == args.NewStatus)
            {
                return;
            }

            this.Status = args.NewStatus;

            var handler = this.StatusChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Raises PropertyChanged event.
        /// </summary>
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnDataSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName))
            {
                this.OnPropertyChanged(null);
            }
            else
            {
                switch (e.PropertyName)
                {
                    case "AggregatesPosition":
                        this.OnPropertyChanged(nameof(this.AggregatesPosition));
                        break;
                    case "AggregatesLevel":
                        this.OnPropertyChanged(nameof(this.AggregatesLevel));
                        break;
                }
            }
        }

        private void OnDataSettingsChanged(object sender, SettingsChangedEventArgs e)
        {
            // TODO: Get change details and push them along...
            // Some changes may be processed by partial recomputations...
            this.Invalidate();
        }

        private void AddPendingChange()
        {
            this.invalidated = true;
        }

        private bool HasPendingChanges()
        {
            return this.invalidated;
        }

        private void ClearPendingChanges()
        {
            this.invalidated = false;
        }

        private void OnInitializationCompleted()
        {
            if (this.deferLevel == 0 && this.HasPendingChanges())
            {
                this.Refresh();
            }
        }

        private void OnEditCompleted()
        {
            if (this.deferLevel == 0 && !this.isInitializing && !this.DeferUpdates && this.HasPendingChanges())
            {
                this.Refresh();
            }
        }

        private void EndDefer()
        {
            this.deferLevel--;
            this.OnEditCompleted();
        }

        private sealed class DeferHelper : IDisposable
        {
            private DataProviderBase provider;

            public DeferHelper(DataProviderBase providerBase)
            {
                this.provider = providerBase;
            }

            public void Dispose()
            {
                if (this.provider != null)
                {
                    this.provider.EndDefer();
                    this.provider = null;
                }

                GC.SuppressFinalize(this);
            }
        }
    }
}