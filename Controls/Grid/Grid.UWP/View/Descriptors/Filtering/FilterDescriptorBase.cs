using System;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents the base class for all descriptors that define a filtering operation within a <see cref="RadDataGrid"/> component.
    /// </summary>
    public abstract class FilterDescriptorBase : DataDescriptor
    {
        internal override DataChangeFlags UpdateFlags
        {
            get
            {
                return DataChangeFlags.Filter;
            }
        }

        internal abstract bool PassesFilter(object item);

        // TODO GA: Decouple the concrete knowledge for a concrete provider from the decriptor. Use something more generic like serialization to IQueryable expression.
        internal abstract string SerializeToSQLiteWhereCondition();

        internal override bool Equals(DescriptionBase description)
        {
            var filterDescription = description as DelegateFilterDescription;
            if (filterDescription != null)
            {
                return filterDescription.Key == this;
            }

            return false;
        }
    }
}
