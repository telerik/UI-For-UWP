using System.Collections.Generic;

namespace Telerik.Data.Core
{
    /// <summary>
    /// A data group abstraction.
    /// </summary>
    internal interface IGroup
    {
        /// <summary>
        /// Gets a value indicating whether this group has any subgroups.
        /// </summary>
        /// <returns>
        /// true if this group is at the bottom level and does not have any subgroups;
        /// otherwise, false.
        /// </returns>
        bool IsBottomLevel { get; }

        /// <summary>
        /// Gets the name of this <see cref="IGroup"/>.
        /// </summary>
        object Name { get; }

        /// <summary>
        /// Gets the items contained within this <see cref="IGroup"/>.
        /// </summary>
        IReadOnlyList<object> Items { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IGroup"/> has items.
        /// </summary>
        bool HasItems { get; }

        /// <summary>
        /// Gets the parent <see cref="IGroup"/>. This instance would be in its parent's <see cref="Items"/> list.
        /// </summary>
        IGroup Parent { get; }

        /// <summary>
        /// Gets the type of the group.
        /// </summary>
        GroupType Type { get; }

        /// <summary>
        /// Gets the level of the <see cref="IGroup"/>.
        /// </summary>
        int Level { get; }
    }

    internal static class IGroupExtensions
    {
        public static int GetLevel(IGroup group)
        {
            int level = 0;
            var parent = group.Parent;
            while (parent != null)
            {
                level++;
                parent = parent.Parent;
            }

            return level;
        }
    }
}