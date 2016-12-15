using System;
using System.Collections.Generic;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Defines the abstraction of a group of items within a data component.
    /// </summary>
    public interface IDataGroup
    {
        /// <summary>
        /// Gets the key of the group.
        /// </summary>
        object Key { get; }

        /// <summary>
        /// Gets the zero-based level of this group in the Group Tree as defined by the GroupDescriptors collection within a data component.
        /// </summary>
        int Level { get; }

        /// <summary>
        /// Gets the child items of the group. These might be either groups or data items.
        /// </summary>
        IReadOnlyList<object> ChildItems { get; }

        /// <summary>
        /// Gets the parent group (if any).
        /// </summary>
        IDataGroup ParentGroup { get; }
    }
}