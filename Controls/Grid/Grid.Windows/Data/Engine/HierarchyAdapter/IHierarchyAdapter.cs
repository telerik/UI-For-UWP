using System.Collections.Generic;

namespace Telerik.Data.Core.Layouts
{
    /// <summary>
    /// Describes a hierarchy.
    /// </summary>
    internal interface IHierarchyAdapter
    {
        /// <summary>
        /// Get an enumeration with the child items of the provided <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The item children are requested for.</param>
        /// <returns>The children of the <paramref name="item"/>.</returns>
        IEnumerable<object> GetItems(object item);

        /// <summary>
        /// Gets a child of <paramref name="item"/> at the <paramref name="index"/>.
        /// </summary>
        /// <param name="item">The item child is requested for.</param>
        /// <param name="index">The index of the requested child.</param>
        /// <returns>The child of <paramref name="item"/> at <paramref name="index"/>.</returns>
        object GetItemAt(object item, int index);
    }
}