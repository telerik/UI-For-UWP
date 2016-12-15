namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents a strongly-typed collection of <see cref="AggregateDescriptorBase"/> instances.
    /// </summary>
    public class AggregateDescriptorCollection : DataDescriptorCollection<AggregateDescriptorBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateDescriptorCollection" /> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal AggregateDescriptorCollection(IDataDescriptorsHost owner) : base(owner)
        {
        }
    }
}