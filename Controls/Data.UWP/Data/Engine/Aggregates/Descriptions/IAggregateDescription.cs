using Telerik.Data.Core.Totals;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Specify the set of properties and methods that a AggregateDescription should implement.
    /// </summary>
    internal interface IAggregateDescription : IDescriptionBase
    {
        /// <summary>
        /// Gets or sets the TotalFormat.
        /// </summary>
        TotalFormat TotalFormat { get; set; }
    }
}