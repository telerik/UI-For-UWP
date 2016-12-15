using System.Collections.Generic;

namespace Telerik.Core.Data
{
    /// <summary>
    /// Represents a group view model within a collection view source.
    /// </summary>
    public interface IDataSourceGroup : IDataSourceItem
    {
        /// <summary>
        /// Gets the <see cref="IDataSourceItem"/> instances owned by this group.
        /// </summary>
        IList<IDataSourceItem> ChildItems
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the group is expanded (its child items are visible and may be enumerated).
        /// </summary>
        bool IsExpanded
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the group has nested groups or data items only.
        /// </summary>
        bool HasChildGroups
        {
            get;
        }

        /// <summary>
        /// Gets the zero-based level of this group.
        /// </summary>
        int Level
        {
            get;
        }

        /// <summary>
        /// Gets the last child item in this group.
        /// </summary>
        IDataSourceItem LastChildItem
        {
            get;
        }
    }
}
