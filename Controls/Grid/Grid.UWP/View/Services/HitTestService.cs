using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents a <see cref="ServiceBase{RadDataGrid}"/> instance that exposes methods used to hit-test for grid rows and cells from given a physical location.
    /// </summary>
    public class HitTestService : ServiceBase<RadDataGrid>
    {
        private int suspendedCount;

        internal HitTestService(RadDataGrid owner) : base(owner)
        {
            this.suspendedCount = 0;
        }

        /// <summary>
        /// Determines whether the service is operational (may provide its functionality).
        /// </summary>
        protected override bool IsOperational
        {
            get
            {
                return base.IsOperational && this.Owner.IsLoaded && this.suspendedCount == 0;
            }
        }

        /// <summary>
        /// Retrieves the object instance from the items source that is represented by the grid row containing the provided physical location.
        /// </summary>
        /// <param name="point">The physical location.</param>
        /// <remarks>
        /// The coordinates are relative to the current <see cref="RadDataGrid"/> instance.
        /// </remarks>
        public object RowItemFromPoint(Point point)
        {
            if (!this.IsOperational)
            {
                return null;
            }

            var cell = this.GetCellFromPoint(this.TransformToCellsPanel(point));
            if (cell == null || cell.ParentRow == null || cell.ParentRow.ItemInfo.IsCollapsible)
            {
                return null;
            }

            return cell.ParentRow.ItemInfo.Item;
        }

        /// <summary>
        /// Retrieves the <see cref="DataGridCellInfo"/> instance that is presented by the grid cell containing the provided physical location.
        /// </summary>
        /// <param name="point">The physical location.</param>
        /// <remarks>
        /// The coordinates are relative to the current <see cref="RadDataGrid"/> instance.
        /// </remarks>
        public DataGridCellInfo CellInfoFromPoint(Point point)
        {
            if (!this.IsOperational)
            {
                return null;
            }

            var cell = this.GetCellFromPoint(this.TransformToCellsPanel(point));
            if (cell == null)
            {
                return null;
            }

            return new DataGridCellInfo(cell);
        }

        internal GridCellModel GetCellFromPoint(RadPoint point)
        {
            if (!this.IsOperational || this.IsOverEditor(point))
            {
                return null;
            }

            var row = this.GetRowFromPoint(point);
            if (row == null)
            {
                return null;
            }

            var frozenPoint = new RadPoint(point.X - this.Owner.ScrollViewer.HorizontalOffset, point.Y);

            foreach (var cell in this.Owner.Model.ForEachFrozenRowCell(row))
            {
                if (cell.layoutSlot.Contains(frozenPoint.X, frozenPoint.Y))
                {
                    return cell;
                }
            }

            foreach (var cell in this.Owner.Model.ForEachRowCell(row))
            {
                if (cell.layoutSlot.Contains(point.X, point.Y))
                {
                    return cell;
                }
            }

            return null;
        }

        internal GridRowModel GetRowFromPoint(RadPoint point)
        {
            if (!this.IsOperational)
            {
                return null;
            }

            foreach (var row in this.Owner.Model.ForEachRow())
            {
                if (row.layoutSlot.Contains(point.X, point.Y))
                {
                    return row;
                }
            }

            return null;
        }

        internal GridHeaderCellModel GetColumnHeaderCellFromPoint(RadPoint point)
        {
            if (!this.IsOperational)
            {
                return null;
            }

            foreach (var cell in this.Owner.Model.ForEachColumnHeaderCell())
            {
                if (cell.layoutSlot.Contains(point.X, point.Y))
                {
                    return cell;
                }
            }

            return null;
        }

        internal IEnumerable<GridCellModel> GetCellsFromRect(RadRect rect)
        {
            if (!this.IsOperational)
            {
                yield break;
            }

            var model = this.Owner.Model;

            foreach (var row in model.ForEachRow())
            {
                foreach (var cell in model.ForEachRowCell(row))
                {
                    if (cell.layoutSlot.IntersectsWith(rect))
                    {
                        yield return cell;
                    }
                }
            }
        }

        internal IEnumerable<GridRowModel> GetRowsFromRect(RadRect rect)
        {
            if (!this.IsOperational)
            {
                yield break;
            }

            foreach (var row in this.Owner.Model.ForEachRow())
            {
                if (row.layoutSlot.IntersectsWith(rect))
                {
                    yield return row;
                }
            }
        }

        internal bool IsOverEditor(RadPoint point)
        {
            if (!this.IsOperational && !this.Owner.editService.IsEditing)
            {
                return false;
            }

            foreach (var slotPair in this.Owner.EditRowLayer.EditorLayoutSlots)
            {
                if (slotPair.Value.Contains(point.X, point.Y))
                {
                    return true;
                }
            }

            return false;
        }

        internal void Suspend()
        {
            this.suspendedCount++;
        }

        internal void Resume()
        {
            if (this.suspendedCount > 0)
            {
                this.suspendedCount--;
            }
        }

        private RadPoint TransformToCellsPanel(Point point)
        {
            var transform = this.Owner.TransformToVisual(this.Owner.CellsPanel);
            point = transform.TransformPoint(point);

            return point.ToRadPoint();
        }
    }
}