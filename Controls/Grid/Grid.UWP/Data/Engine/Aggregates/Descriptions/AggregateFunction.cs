using System;
using System.ComponentModel;
using Telerik.Data.Core.Aggregates;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Describes the supported aggregate functions available for LocalDataSourceProvider.
    /// <seealso cref="PropertyAggregateDescriptionBase"/>.
    /// </summary>
    internal abstract class AggregateFunction : SettingsNode
    {
        /// <summary>
        /// Gets a string format suitable to format the value of the <see cref="AggregateValue"/>s produced by that <see cref="AggregateFunction"/>.
        /// </summary>
        /// <param name="dataType">The type of the data items.</param>
        /// <param name="format">A string format selected by other means. You may keep or discard it.</param>
        /// <returns>A string.</returns>
        public virtual string GetStringFormat(Type dataType, string format)
        {
            return null;
        }

        /// <summary>
        /// Creates an AggregateValue supported by that AggregateFunction.
        /// </summary>
        /// <returns>An <see cref="AggregateValue"/>.</returns>
        protected internal virtual AggregateValue CreateAggregate(Type dataType)
        {
            return null;
        }
    }
}