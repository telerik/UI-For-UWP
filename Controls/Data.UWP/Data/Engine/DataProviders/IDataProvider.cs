using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Telerik.Data.Core.Fields;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Provides data access for data grouping.
    /// </summary>
    internal interface IDataProvider : INotifyPropertyChanged, ISupportInitialize
    {
        /// <summary>
        /// Occurs when the current operation has completed.
        /// </summary>
        event EventHandler<DataProviderStatusChangedEventArgs> StatusChanged;

        /// <summary>
        /// Raised when data view has changed (e.g. Item was added/removed from collection).
        /// </summary>
        event EventHandler<ViewChangedEventArgs> ViewChanged;

        /// <summary>
        /// Raised before data view has changed (e.g. Item was added/removed from collection).
        /// </summary>
        event EventHandler<ViewChangingEventArgs> ViewChanging;

        /// <summary>
        /// Raised when current item has changed.
        /// </summary>
        event System.EventHandler<object> CurrentChanged;

        /// <summary>
        /// Occurs when field descriptions is changed.
        /// </summary>
        event EventHandler FieldDescriptionsChanged;

        /// <summary>
        /// Gets the status of this instance.
        /// </summary>
        DataProviderStatus Status { get; }

        /// <summary>
        /// Gets the results from the last grouping.
        /// </summary>
        IDataResults Results { get; }
        
        /// <summary>
        /// Gets the <see cref="IDataSettings"/> instance that is being used.
        /// </summary>
        IDataSettings Settings { get; }

        /// <summary>
        /// Gets or sets the <see cref="IFieldDescriptionProvider"/> instance that is being used.
        /// </summary>
        /// <value><see cref="IFieldDescriptionProvider"/> instance.</value>
        IFieldDescriptionProvider FieldDescriptionsProvider { get; set; }

        /// <summary>
        /// Gets or sets a value indicating where the aggregate groups should be positioned.
        /// </summary>
        DataAxis AggregatesPosition { get; set; }

        /// <summary>
        /// Gets or sets the position where groups for the aggregates should be placed.
        /// </summary>
        int AggregatesLevel { get; set; }

        /// <summary>
        /// Gets the state object that is provided to <see cref="Telerik.Data.Core.Fields.IFieldDescriptionProvider.GetDescriptionsDataAsync"/> method.
        /// </summary>
        /// <returns>The object that will be passed to <see cref="Telerik.Data.Core.Fields.IFieldDescriptionProvider.GetDescriptionsDataAsync"/> method.</returns>
        object State { get; }

        /// <summary>
        /// Gets or sets a value indicating whether changes to the grouping settings would trigger computations immediately when invalidated or on explicit <see cref="Refresh"/>.
        /// </summary>
        bool DeferUpdates { get; set; }

        bool IsSingleThreaded { get; set; }

        IGroupFactory GroupFactory { get; set; }

        object ItemsSource { get; set; }

        IFieldInfoData FieldDescriptions { get; }

        IValueProvider ValueProvider { get; }

        IDataSourceView DataView { get; set; }

        /// <summary>
        /// Gets a list of <see cref="PropertyFilterDescriptionBase"/> that specified how the data items should be filtered.
        /// </summary>
        Collection<PropertyFilterDescriptionBase> FilterDescriptions { get; }

        /// <summary>
        /// Gets a list of <see cref="PropertyGroupDescriptionBase"/> that specified how items should be grouped.
        /// </summary>
        Collection<PropertyGroupDescriptionBase> GroupDescriptions { get; }

        /// <summary>
        /// Gets a list of <see cref="PropertySortDescription"/> that specified how items should be sorted.
        /// </summary>
        Collection<SortDescription> SortDescriptions { get; }

        /// <summary>
        /// Gets a list of <see cref="PropertyAggregateDescription"/> that specified how the data should be aggregated.
        /// </summary>
        Collection<PropertyAggregateDescriptionBase> AggregateDescriptions { get; }

        /// <summary>
        /// Force recalculation operation.
        /// </summary>
        void Refresh(DataChangeFlags dataChangeFlags = DataChangeFlags.None);

        /// <summary>
        /// Block the calling thread until all calculations performed by calling <see cref="Telerik.Data.Core.IDataProvider.Refresh"/> method completes.
        /// </summary>
        void BlockUntilRefreshCompletes();

        void SuspendPropertyChanges(object item);

        void ResumePropertyChanges(object item);

        /// <summary>
        /// Edit row operation states.
        /// </summary>
        void BeginEditOperation(object item);
        void CancelEditOperation(object item);
        void CommitEditOperation(object item);

        /// <summary>
        /// Enters a defer cycle that you can use to merge changes to the data provider and delay automatic refresh.
        /// </summary>
        /// <returns>An IDisposable object that you can use to dispose of the calling object.</returns>
        IDisposable DeferRefresh();

        /// <summary>
        /// Creates and returns an aggregate description suitable for the supplied field description.
        /// </summary>
        /// <param name="description">A <see cref="IDataFieldInfo"/> instance.</param>
        /// <returns>An <see cref="IAggregateDescription"/> instance.</returns>
        IAggregateDescription GetAggregateDescriptionForFieldDescription(IDataFieldInfo description);

        /// <summary>
        /// Creates and returns a group description suitable for the supplied field description.
        /// </summary>
        /// <param name="description">A <see cref="IDataFieldInfo"/> instance.</param>
        /// <returns>An <see cref="IGroupDescription"/> instance.</returns>
        IGroupDescription GetGroupDescriptionForFieldDescription(IDataFieldInfo description);

        /// <summary>
        /// Returns a filter description suitable for the supplied field description.
        /// </summary>
        /// <param name="description">A <see cref="IDataFieldInfo"/> instance.</param>
        /// <returns>An <see cref="FilterDescription"/> instance.</returns>
        FilterDescription GetFilterDescriptionForFieldDescription(IDataFieldInfo description);

        /// <summary>
        /// Returns a sort description suitable for the supplied field description.
        /// </summary>
        /// <param name="description">A <see cref="IDataFieldInfo"/> instance.</param>
        /// <returns>An <see cref="SortDescription"/> instance.</returns>
        SortDescription GetSortDescriptionForFieldDescription(IDataFieldInfo description);

        /// <summary>
        /// Returns a list of suitable functions for the supplied aggregate description.
        /// </summary>
        /// <param name="aggregateDescription">The <see cref="IAggregateDescription"/>.</param>
        /// <returns>A list of possible aggregate functions.</returns>
        IEnumerable<object> GetAggregateFunctionsForAggregateDescription(IAggregateDescription aggregateDescription);

        /// <summary>
        /// Set the <paramref name="aggregateFunction"/> retrieved from <see cref="GetAggregateFunctionsForAggregateDescription"/> to the <paramref name="aggregateDescription"/>.
        /// </summary>
        /// <param name="aggregateDescription">The <see cref="IAggregateDescription"/>.</param>
        /// <param name="aggregateFunction">The aggregate function.</param>
        void SetAggregateFunctionToAggregateDescription(IAggregateDescription aggregateDescription, object aggregateFunction);
    }
}