using System;
using Telerik.UI.Xaml.Controls.Grid.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents an IGridExternalEditor class.
    /// </summary>
    public interface IGridExternalEditor
    {
        /// <summary>
        /// Occurs when an edit operation on the item is cancelled.
        /// </summary>
        event EventHandler EditCancelled;

        /// <summary>
        /// Occurs when an edit operation on the item is committed.
        /// </summary>
        event EventHandler EditCommitted;

        /// <summary>
        /// Gets or sets the position of the ExternalEditor.
        /// </summary>
        ExternalEditorPosition Position { get; set; }

        /// <summary>
        /// Begins edit on the passed item.
        /// </summary>
        void BeginEdit(object item, RadDataGrid owner);

        /// <summary>
        /// Cancels edit on the passed item.
        /// </summary>
        void CancelEdit();

        /// <summary>
        /// Commits edit on the passed item.
        /// </summary>
        void CommitEdit();
    }
}
