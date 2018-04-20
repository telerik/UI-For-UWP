using System.Collections;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Provides a view over the already computed data within a <see cref="RadDataGrid"/> instance.
    /// </summary>
    public interface IDataView : IDataViewCollection
    {
        /// <summary>
        /// Gets the computed aggregate value associated with the provided aggregate index.
        /// </summary>
        /// <param name="aggregateIndex">The zero-based index of the <see cref="AggregateDescriptorBase"/> within the <see cref="RadDataGrid.AggregateDescriptors"/> collection.</param>
        /// <param name="group">The <see cref="IDataGroup"/> instance to get the value for. Pass null to retrieve the computed Grand Total value.</param>
        object GetAggregateValue(int aggregateIndex, IDataGroup group);

        /// <summary>
        /// Gets the computed aggregate values for all the <see cref="PropertyAggregateDescriptor"/> instances associated with the provided property name.
        /// </summary>
        /// <param name="propertyName">The name of the property to filter aggregates by.</param>
        /// <param name="group">The <see cref="IDataGroup"/> instance to get the value for. Pass null to retrieve the computed Grand Total value.</param>
        IEnumerable GetAggregateValues(string propertyName, IDataGroup group);
    }
}
