using System;
using System.Diagnostics;
using System.Linq;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Primitives;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Behavior that control all logical operations related to dragging DataGrid elements.
    /// </summary>
    public class DataGridDragBehavior : AttachableObject<RadDataGrid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridDragBehavior" /> class.
        /// </summary>
        public DataGridDragBehavior()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridDragBehavior" /> class.
        /// </summary>
        /// <param name="owner">Behavior owner.</param>
        internal DataGridDragBehavior(RadDataGrid owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Determines whether this Grid can start drag to the specified column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>Whether drag operation can start.</returns>
        public virtual bool CanStartDrag(DataGridColumn column)
        {
            if (column == null || this.Owner == null)
            {
                return false;
            }

            return (column.HeaderControl != null && column.HeaderControl.IsSelected || this.Owner.ColumnDataOperationsMode == ColumnDataOperationsMode.Inline) &&
                ((this.Owner.UserColumnReorderMode == DataGridUserColumnReorderMode.Interactive && column.CanUserReorder) || column.CanGroupBy);
        }

        /// <summary>
        /// Determines whether this Grid can start drag to the specified column within a flyout.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>Whether drag operation can start.</returns>
        public virtual bool CanStartDragInFlyout(DataGridColumn column)
        {
            if (column == null || this.Owner == null)
            {
                return false;
            }

            return this.Owner.UserColumnReorderMode == DataGridUserColumnReorderMode.Interactive && column.CanUserReorder;
        }

        /// <summary>
        /// Gets the drag visual for specified column header.
        /// </summary>
        /// <param name="header">The header.</param>
        public virtual FrameworkElement GetDragVisual(DataGridColumnHeader header)
        {
            return null;
        }

        /// <summary>
        /// Determines whether this Grid can start resize the specified column.
        /// </summary>
        /// <param name="column">The specified column.</param>
        /// <returns>Whether resize operation can start.</returns>
        public virtual bool CanStartResize(DataGridColumn column)
        {
            return column.CanUserResize;
        }

        /// <summary>
        /// Called when DataGrid started column resize operation.
        /// </summary>
        /// <param name="column">The specified column.</param>
        public virtual void OnColumnResizeStarted(DataGridColumn column)
        {
            column.SizeMode = DataGridColumnSizeMode.Fixed;
            column.Width = column.ActualWidth;
        }

        /// <summary>
        /// Called during DataGrid column resize operation.
        /// </summary>
        /// <param name="column">The specified column.</param>
        /// <param name="initialColumnSize">The initial size of the column.</param>
        /// <param name="totalResizeChange">The value by which the size is changed.</param>
        public virtual void OnColumnResizing(DataGridColumn column, double initialColumnSize, double totalResizeChange)
        {
            column.Width = Math.Max(column.HeaderControl.MinWidth, initialColumnSize + totalResizeChange);
        }

        /// <summary>
        /// Called when the column resize operation completes.
        /// </summary>
        /// <param name="column">The resized column.</param>
        /// <param name="widthChange">The column width change. Can be negative when the column size has been reduced.</param>
        public virtual void OnColumnResizeEnded(DataGridColumn column, double widthChange)
        {
        }

        /// <inheritdoc/>
        public virtual void OnColumnResizeHandleDoubleTapped(DataGridColumn column)
        {
            column.Width = DataGridColumn.DefaultWidth;
            column.SizeMode = DataGridColumnSizeMode.Auto;
        }

        /// <summary>
        /// Called when column drag started.
        /// </summary>
        /// <param name="column">The column.</param>
        public virtual void OnDragStarted(DataGridColumn column)
        {
        }

        /// <summary>
        /// Determines whether this instance can reorder column the specified destination column.
        /// </summary>
        /// <param name="sourceColumn">The source column.</param>
        /// <param name="destinationColumn">The destination column.</param>
        public virtual bool CanReorder(DataGridColumn sourceColumn, DataGridColumn destinationColumn)
        {
            if (this.Owner == null || sourceColumn == null)
            {
                return false;
            }

            return this.Owner.UserColumnReorderMode == DataGridUserColumnReorderMode.Interactive && sourceColumn.CanUserReorder;
        }

        /// <summary>
        /// Called when DataGrid column is reordered.
        /// </summary>
        /// <param name="sourceColumn">The source column.</param>
        /// <param name="destinationColumn">The destination column.</param>
        public virtual void OnReordered(DataGridColumn sourceColumn, DataGridColumn destinationColumn)
        {
        }

        /// <summary>
        /// Determines RadDataGrid can group by specified column.
        /// </summary>
        /// <param name="column">The column.</param>
        public virtual bool CanGroupBy(DataGridColumn column)
        {
            if (column == null || this.Owner == null)
            {
                return false;
            }

            var groupDescriptor = column.GetGroupDescriptor();

            return column.CanGroupBy && groupDescriptor != null && !this.Owner.GroupDescriptors.Contains(groupDescriptor);
        }

        /// <summary>
        /// Called when column is grouped.
        /// </summary>
        /// <param name="column">The column.</param>
        public virtual void OnGroupedBy(DataGridColumn column)
        {
        }

        /// <summary>
        /// Called when drag drop operation completed.
        /// </summary>
        /// <param name="header">The source header being dragged.</param>
        /// <param name="dragSuccessful">Determines whether current drag operation completed successfully.</param>
        public virtual void OnDragDropCompleted(DataGridColumnHeader header, bool dragSuccessful)
        {
            this.Owner.ServicePanel.IsColumnDragging = false;
        }

        /// <summary>
        /// Groups DataGrid by the specified column.
        /// </summary>
        /// <param name="column">The column.</param>
        public virtual void GroupBy(DataGridColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            var descriptor = column.GetGroupDescriptor();
            if (descriptor != null)
            {
                this.Owner.GroupDescriptors.Add(descriptor);
                this.OnGroupedBy(column);
            }
        }

        /// <summary>
        /// Reorders the group descriptors of the DataGrid.
        /// </summary>
        /// <param name="sourceIndex">Index of the source descriptor.</param>
        /// <param name="destinationIndex">Index of the destination descriptor.</param>
        public virtual void ReorderGroupDescriptor(int sourceIndex, int destinationIndex)
        {
            var sourceDescriptor = this.Owner.GroupDescriptors[sourceIndex];
            this.Owner.GroupDescriptors.RemoveAt(sourceIndex);

            this.Owner.GroupDescriptors.Insert(destinationIndex, sourceDescriptor);
        }

        /// <summary>
        /// Determines whether this Grid can start reorder the specified descriptor.
        /// </summary>
        /// <param name="groupDescriptorBase">The specified descriptor.</param>
        /// <returns>Whether reorder operation can start.</returns>
        public virtual bool CanStartReorder(GroupDescriptorBase groupDescriptorBase)
        {
            return true;
        }

        /// <summary>
        /// Determines whether this Grid can start reorder the specified column.
        /// </summary>
        /// <param name="column">The specified column.</param>
        /// <returns>Whether reorder operation can start.</returns>
        public virtual bool CanStartReorder(DataGridColumn column)
        {
            return this.Owner.UserColumnReorderMode == DataGridUserColumnReorderMode.Interactive && column.CanUserReorder;
        }

        /// <summary>
        /// Gets the reorder visual for the specified groupHeader.
        /// </summary>
        /// <param name="dataGridFlyoutGroupHeader">The data grid flyout header.</param>
        public virtual FrameworkElement GetReorderVisual(DataGridFlyoutHeader dataGridFlyoutGroupHeader)
        {
            return null;
        }

        /// <summary>
        /// Called when DataGrid flyout items started reorder operation.
        /// </summary>
        /// <param name="groupDescriptorBase">The group descriptor base.</param>
        public virtual void OnReorderStarted(GroupDescriptorBase groupDescriptorBase)
        {
        }

        /// <summary>
        /// Determines whether the specified descriptors can be reordered through the DataGrid group flyout.
        /// </summary>
        /// <param name="sourceDescriptor">The source descriptor.</param>
        /// <param name="destinationDescriptor">The destination descriptor.</param>
        public virtual bool CanReorder(GroupDescriptorBase sourceDescriptor, GroupDescriptorBase destinationDescriptor)
        {
            return true;
        }

        /// <summary>
        /// Called when reorder operation complete. Note that the operation may not be successful.
        /// </summary>
        /// <param name="flyoutHeader">The flyout header.</param>
        /// <param name="reorderSuccessful">Determines whether current reorder operation completed successfully.</param>
        public virtual void OnReorderCompleted(DataGridFlyoutHeader flyoutHeader, bool reorderSuccessful)
        {
        }

        /// <summary>
        /// Reorders the columns of RadDataGrid.
        /// </summary>
        /// <param name="sourceIndex">Index of the source column.</param>
        /// <param name="destinationIndex">Index of the destination column.</param>
        public virtual void ReorderColumn(int sourceIndex, int destinationIndex)
        {
            this.Owner.Columns.Move(sourceIndex, destinationIndex);
        }

        /// <summary>
        /// Reorders visible columns in RadDataGrid.
        /// </summary>
        /// <param name="sourceIndex">Index of the source column within the collection of visible columns.</param>
        /// <param name="destinationIndex">Index of the target column within the collection of visible columns.</param>
        public virtual void ReorderVisibleColumn(int sourceIndex, int destinationIndex)
        {
            sourceIndex = this.GetIndexInFullColumnsCollection(sourceIndex);
            destinationIndex = this.GetIndexInFullColumnsCollection(destinationIndex);
            this.Owner.Columns.Move(sourceIndex, destinationIndex);
        }

        internal void OnDragStarted(DataGridColumnHeader header)
        {
            if (this.CanGroupBy(header.Column))
            {
                this.Owner.ServicePanel.IsColumnDragging = true;
                DragDrop.SetDragPositionMode(header, DragPositionMode.Free);
            }
            else
            {
                // Only reorder is allowed.
                DragDrop.SetDragPositionMode(header, DragPositionMode.RailX);
            }

            this.OnDragStarted(header.Column);
        }

        private int GetIndexInFullColumnsCollection(int sourceIndex)
        {
            int result = sourceIndex;

            foreach (var column in this.Owner.Columns)
            {
                if (!column.IsVisible)
                {
                    result++;
                }
                else
                {
                    sourceIndex--;
                }

                if (sourceIndex < 0)
                {
                    break;
                }
            }

            return result;
        }
    }
}