using System;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    /// <summary>
    /// Defines the known commands that are available within a <see cref="RadDataGrid"/> control.
    /// </summary>
    public enum CommandId
    {
        /// <summary>
        /// The command is not familiar to the grid. 
        /// </summary>
        Unknown,

        /// <summary>
        /// A command associated with the Tap event that occurs over a column header.
        /// </summary>
        ColumnHeaderTap,

        /// <summary>
        /// A command associated with the Tap event that occurs over a group header.
        /// </summary>
        GroupHeaderTap,

        /// <summary>
        /// A command associated with the Tap event that occurs over a cell.
        /// </summary>
        CellTap,

        /// <summary>
        /// A command associated with the DoubleTap event that occurs over a cell.
        /// </summary>
        CellDoubleTap,

        /// <summary>
        /// A command associated with the Tap event over a group header that resides in the Grouping UI flyout, accessible from the service panel on the left.
        /// </summary>
        FlyoutGroupHeaderTap,

        /// <summary>
        /// A command associated with the Tap event over the Filter Button within a <see cref="Primitives.DataGridColumnHeader"/> control.
        /// </summary>
        FilterButtonTap,

        /// <summary>
        /// A command associated with the Tap event over the Filter and ClearFilter buttons within a DataGridFilteringFlyout control.
        /// </summary>
        FilterRequested,

        /// <summary>
        /// A command associated with the PointerOver event that occurs over a cell.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "CellPointer")]
        CellPointerOver,

        /// <summary>
        /// A command associated with the Auto-generate columns process.
        /// </summary>
        GenerateColumn,

        /// <summary>
        /// A command associated with the DataBindingComplete event that occurs within a <see cref="RadDataGrid"/> instance.
        /// </summary>
        DataBindingComplete,

        /// <summary>
        /// A command associated with the begin edit action that occurs within a <see cref="RadDataGrid"/> instance.
        /// </summary>
        BeginEdit,

        /// <summary>
        /// A command associated with the cancel edit action that occurs within a <see cref="RadDataGrid"/> instance.
        /// </summary>
        CancelEdit,

        /// <summary>
        /// A command associated with the commit edit action that occurs within a <see cref="RadDataGrid"/> instance.
        /// </summary>
        CommitEdit,

        /// <summary>
        /// A command associated with the validating cell action that occurs within a <see cref="RadDataGrid"/> instance.
        /// </summary>
        ValidateCell,

        /// <summary>
        /// A command associated with requesting and loading more data when available that occurs within a <see cref="RadDataGrid"/> instance.
        /// </summary>
        LoadMoreData,

        /// <summary>
        /// A command associated with the KeyDown event processing in a <see cref="RadDataGrid"/> instance.
        /// </summary>
        KeyDown,

        /// <summary>
        /// A command associated with the Holding event which occurs over a cell.
        /// </summary>
        CellHolding,

        /// <summary>
        /// A command associated with the action which occurs in a ColumnHeader Flyout.
        /// </summary>
        ColumnHeaderAction,

        /// <summary>
        /// A command associated with the action which occurs with a Cell's Flyout.
        /// </summary>
        CellFlyoutAction,

        /// <summary>
        /// A command associated with the action of hiding or showing a column in a <see cref="RadDataGrid"/> instance.
        /// </summary>
        ToggleColumnVisibility
    }
}
