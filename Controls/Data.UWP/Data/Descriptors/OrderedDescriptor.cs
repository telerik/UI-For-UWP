using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents a data descriptor that support a sort order.
    /// </summary>
    public abstract class OrderedDescriptor : DataDescriptor
    {
        private SortOrder sortOrder;

        /// <summary>
        /// Gets or sets the current sort order.
        /// </summary>
        public SortOrder SortOrder
        {
            get
            {
                return this.sortOrder;
            }
            set
            {
                if (this.sortOrder == value)
                {
                    return;
                }

                this.sortOrder = value;
                this.OnPropertyChanged();
            }
        }

        ////internal static SortDirection SortOrderToSortDirection(SortOrder order)
        ////{
        ////    return order == SortOrder.Ascending ? SortDirection.Ascending : SortDirection.Descending;
        ////}
    }
}
