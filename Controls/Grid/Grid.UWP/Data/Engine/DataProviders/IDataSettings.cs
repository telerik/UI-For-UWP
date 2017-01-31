using System;
using System.Collections;
using System.ComponentModel;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents an interface for controlling data settings like group descriptions, aggregate descriptions, etc.
    /// </summary>
    internal interface IDataSettings : INotifyPropertyChanged, ISupportInitialize
    {
        /// <summary>
        /// An event that notifies some of the
        /// <see cref="FilterDescriptions"/>,
        /// <see cref="RowGroupDescriptions"/>,
        /// <see cref="ColumnGroupDescriptions"/>,
        /// <see cref="AggregateDescriptions"/>,
        /// <see cref="AggregatesLevel"/>
        /// or <see cref="AggregatesPosition"/> has changed.
        /// Notifications are raised even in <see cref="BeginEdit"/> scope.
        /// </summary>
        event EventHandler<EventArgs> DescriptionsChanged;

        /// <summary>
        /// Notifies when this or one of the children is changed.
        /// </summary>
        event EventHandler<SettingsChangedEventArgs> SettingsChanged;

        IGroupFactory GroupFactory
        {
            get;
        }

        /// <summary>
        /// Gets the data filter descriptions list.
        /// </summary>
        IList FilterDescriptions { get; }

        /// <summary>
        /// Gets the data row group descriptions list.
        /// </summary>
        IList RowGroupDescriptions { get; }

        /// <summary>
        /// Gets the data column group descriptions list.
        /// </summary>
        IList ColumnGroupDescriptions { get; }

        /// <summary>
        /// Gets the data aggregate descriptions list.
        /// </summary>
        IList AggregateDescriptions { get; }

        /// <summary>
        /// Gets the data sort descriptions list.
        /// </summary>
        IList SortDescriptions { get; }

        /// <summary>
        /// Gets or sets the position where groups for the aggregates should be placed.
        /// </summary>
        int AggregatesLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating where the aggregate groups should be positioned.
        /// </summary>
        DataAxis AggregatesPosition { get; set; }

        /// <summary>
        /// Enters the <see cref="IDataSettings"/> in a new editing scope.
        /// Use when applying multiple changes to the data settings.
        /// <example>
        /// using(dataSettings.BeginEdit())
        /// {
        ///     // Apply multiple changes to dataSettings here.
        /// }
        /// </example>
        /// </summary>
        /// <returns>An edit scope token that you must <see cref="IDisposable.Dispose"/> when you are done with the editing.</returns>
        IDisposable BeginEdit();
    }
}