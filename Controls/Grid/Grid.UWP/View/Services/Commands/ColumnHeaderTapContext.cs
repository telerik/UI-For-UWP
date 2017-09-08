using System;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    /// <summary>
    /// Encapsulates the context of a <see cref="DataGridCommand"/> associated with the <see cref="CommandId.ColumnHeaderTap"/> event.
    /// </summary>
    public class ColumnHeaderTapContext
    {
        /// <summary>
        /// Gets or sets the <see cref="DataGridColumn"/> instance which header is actually Tapped.
        /// </summary>
        public DataGridColumn Column
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether a user sort operation is allowed.
        /// </summary>
        public bool CanSort
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can sort by multiple keys or not.
        /// </summary>
        public bool IsMultipleSortAllowed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether if flyout has been opened before the column tap event(In the tap event we automatically close it).
        /// </summary>
        internal bool IsFlyoutOpen
        {
            get;
            set;
        }
    }
}
