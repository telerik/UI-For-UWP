using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Describes the aggregation of items using a property name as the criteria.
    /// </summary>
    internal sealed class PropertyAggregateDescription : PropertyAggregateDescriptionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyAggregateDescription" /> class.
        /// </summary>
        public PropertyAggregateDescription()
        {
        }

        /// <inheritdoc />
        protected override Cloneable CreateInstanceCore()
        {
            return new PropertyAggregateDescription();
        }

        /// <inheritdoc />
        protected override void CloneOverride(Cloneable source)
        {
        }
    }
}