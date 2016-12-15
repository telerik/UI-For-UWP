using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Specifies the possible flags in a data change operation within a data component.
    /// </summary>
    [Flags]
    public enum DataChangeFlags
    {
        /// <summary>
        /// No change has occurred.
        /// </summary>
        None = 0,

        /// <summary>
        /// The change is associated with a Sorting operation.
        /// </summary>
        PropertyChanged = 1,

        /// <summary>
        /// The change is associated with a Sorting operation.
        /// </summary>
        Sort = PropertyChanged << 1,

        /// <summary>
        /// The change is associated with a Grouping operation.
        /// </summary>
        Group = Sort << 1,

        /// <summary>
        /// The change is associated with a Filtering operation.
        /// </summary>
        Filter = Group << 1,

        /// <summary>
        /// The change is associated with adding or removing an AggregateDescriptor from the Descriptors collection.
        /// </summary>
        Aggregate = Filter << 1,

        /// <summary>
        /// The source of the data has changed.
        /// </summary>
        Source = Aggregate << 1,
    }
}
