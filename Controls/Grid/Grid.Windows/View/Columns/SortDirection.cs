using System;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Defines the current sort direction of a <see cref="DataGridColumn"/> instance. 
    /// This enumeration is used solely for visualization purposes and differs from the SortOrder type that is used for data operations.
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// No sorting is applied.
        /// </summary>
        None,

        /// <summary>
        /// The column is sorted in ascending order.
        /// </summary>
        Ascending,

        /// <summary>
        /// The column is sorted in descending order.
        /// </summary>
        Descending
    }
}
