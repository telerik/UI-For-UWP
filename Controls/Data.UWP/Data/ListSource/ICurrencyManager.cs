using System.Collections;

namespace Telerik.Core.Data
{
    /// <summary>
    /// Defines a collection that adds support for tracking the current item within the collection.
    /// </summary>
    public interface ICurrencyManager : ICollection
    {
        /// <summary>
        /// Gets or sets the mode this instance uses to update its current item.
        /// </summary>
        CurrencyManagementMode CurrencyMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the item that is marked as Current.
        /// </summary>
        IDataSourceItem CurrentItem
        {
            get;
        }

        /// <summary>
        /// Attempts to set the specified item as the current one.
        /// </summary>
        /// <param name="item">The item instance that should be set as current.</param>
        /// <returns>True if operation was successful, false otherwise.</returns>
        bool SetCurrentItem(IDataSourceItem item);

        /// <summary>
        /// Attempts to set the current item to the first item within the collection.
        /// </summary>
        /// <returns>True if operation was successful, false otherwise.</returns>
        bool MoveCurrentToFirst();

        /// <summary>
        /// Attempts to set the current item to the next one that follows the current item.
        /// </summary>
        /// <returns>True if operation was successful, false otherwise.</returns>
        bool MoveCurrentToNext();

        /// <summary>
        /// Attempts to set the current item to the previous one before the current item.
        /// </summary>
        /// <returns>True if operation was successful, false otherwise.</returns>
        bool MoveCurrentToPrevious();

        /// <summary>
        /// Attempts to set the current item to the last item within the collection.
        /// </summary>
        /// <returns>True if operation was successful, false otherwise.</returns>
        bool MoveCurrentToLast();
    }
}
