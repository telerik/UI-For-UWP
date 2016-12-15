namespace Telerik.Core
{
    /// <summary>
    /// Defines a type that has a notation for a Current item. This notation is typical for data-bound components like RadDataGrid.
    /// </summary>
    public interface ISupportCurrentItem
    {
        /// <summary>
        /// Gets the object instance that is considered Current.
        /// </summary>
        object CurrentItem
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the current item is within the data view. For example if a filtering operation is applied, the current item may not be visible.
        /// </summary>
        bool IsCurrentItemInView
        {
            get;
        }

        /// <summary>
        /// Attempts to set the <see cref="CurrentItem"/> to the provided object instance.
        /// </summary>
        /// <param name="item">The object instance to set as current.</param>
        /// <returns>True if the operation succeeded, false otherwise.</returns>
        bool MoveCurrentTo(object item);

        /// <summary>
        /// Attempts to move the <see cref="CurrentItem"/> to the first item in the view.
        /// </summary>
        /// <returns>True if the operation succeeded, false otherwise.</returns>
        bool MoveCurrentToFirst();

        /// <summary>
        /// Attempts to move the <see cref="CurrentItem"/> to the last item in the view.
        /// </summary>
        /// <returns>True if the operation succeeded, false otherwise.</returns>
        bool MoveCurrentToLast();

        /// <summary>
        /// Attempts to move the <see cref="CurrentItem"/> to the item next to the <see cref="CurrentItem"/> in the view.
        /// </summary>
        /// <returns>True if the operation succeeded, false otherwise.</returns>
        bool MoveCurrentToNext();

        /// <summary>
        /// Attempts to move the <see cref="CurrentItem"/> to the item previous to the <see cref="CurrentItem"/> in the view.
        /// </summary>
        /// <returns>True if the operation succeeded, false otherwise.</returns>
        bool MoveCurrentToPrevious();
    }
}
