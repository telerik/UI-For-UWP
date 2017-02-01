using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Defines the sort order and the property name to be used as the criteria for sorting a collection.
    /// </summary>
    internal sealed class PropertySortDescription : SortDescription
    {
        protected override Cloneable CreateInstanceCore()
        {
            return new PropertySortDescription();
        }

        /// <inheritdoc />
        protected override void CloneOverride(Cloneable source)
        {
        }
    }
}