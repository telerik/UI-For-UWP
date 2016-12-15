using Telerik.Data.Core;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Used to group items, provide well known groups, sort and filter the groups for a <see cref="LocalDataSourceProvider"/> based on the item's <see cref="Telerik.Data.Core.DescriptionBase.PropertyName"/> value.
    /// </summary>
    internal sealed class PropertyGroupDescription : PropertyGroupDescriptionBase
    {
        /// <inheritdoc />
        protected override Cloneable CreateInstanceCore()
        {
            return new PropertyGroupDescription();
        }

        /// <inheritdoc />
        protected override void CloneOverride(Cloneable source)
        {
        }
    }
}