using System;
using System.Collections;
using System.Collections.Generic;
using Telerik.Core;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Provides a view over the already computed data over the ItemsSource.
    /// </summary>
    public interface IDataViewCollection : IEnumerable, ISupportCurrentItem
    {
        /// <summary>
        /// Gets a value indicating whether all the internal data operations are completed and the view may be properly accessed.
        /// </summary>
        bool IsDataReady
        {
            get;
        }

        /// <summary>
        /// Gets the top-level items within the view. These might be either <see cref="IDataGroup"/> instances or data items if no grouping is applied.
        /// </summary>
        IReadOnlyList<object> Items
        {
            get;
        }

        /// <summary>
        /// Gets the <see cref="IDataGroup"/> instance where the specified item resides. 
        /// Will be null if no grouping is applied or the item does not belong to the ItemsSource.
        /// </summary>
        /// <param name="item">The data item to search for.</param>
        IDataGroup GetParentGroup(object item);

        /// <summary>
        /// Enumerates all the present IDataGroup instances using depth-first approach.
        /// </summary>
        /// <param name="condition">An optional condition that may be used to filter the results.</param>
        IEnumerable<IDataGroup> GetGroups(Predicate<IDataGroup> condition = null);

        /// <summary>
        /// Determines whether the provided group is considered "Expanded" - that is to have its ChildItems available - within the UI.
        /// </summary>
        bool GetIsExpanded(IDataGroup group);

        /// <summary>
        /// Attempts to expand the provided <see cref="IDataGroup"/> instance.
        /// </summary>
        void ExpandGroup(IDataGroup group);

        /// <summary>
        /// Attempts to collapse the provided <see cref="IDataGroup"/> instance.
        /// </summary>
        void CollapseGroup(IDataGroup group);

        /// <summary>
        /// Expands all the groups.
        /// </summary>
        void ExpandAll();

        /// <summary>
        /// Collapses all the groups.
        /// </summary>
        void CollapseAll();

        /// <summary>
        /// Expands the chain of groups where the specified item resides.
        /// </summary>
        void ExpandItem(object item);

        /// <summary>
        /// Collapses the immediate groups that contains the specified item.
        /// </summary>
        void CollapseItem(object item);
    }
}
