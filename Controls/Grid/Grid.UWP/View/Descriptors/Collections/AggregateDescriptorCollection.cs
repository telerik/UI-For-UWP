using Telerik.UI.Xaml.Controls.Grid.Model;

namespace Telerik.UI.Xaml.Controls.Grid
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
        internal AggregateDescriptorCollection(GridModel owner)
            : base(owner)
        {
        }
    }
}
