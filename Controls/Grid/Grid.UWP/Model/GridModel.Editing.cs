using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Grid;

namespace Telerik.UI.Xaml.Controls.Grid.Model
{
    internal partial class GridModel
    {
        private bool isInEditMode;

        internal GridRowModel EditRow
        {
            get
            {
                return this.EditRowPool.EditRow;
            }
        }

        internal GridRowModel FrozenEditRow
        {
            get
            {
                return this.EditRowPool.FrozenEditRow;
            }
        }

        internal void BeginEdit(ItemInfo rowInfo)
        {
            this.EditRowPool.ReadOnlyItemInfo = rowInfo;
            this.isInEditMode = true;

            this.UpdateEditRow();
        }

        internal void CancelEdit()
        {
            var item = this.FinishEdit();
        }

        internal void CommitEdit()
        {
            var item = this.FinishEdit();
        }

        internal int GetEditRowLine()
        {
            return this.EditRowPool.ReadOnlyItemInfo.LayoutInfo.Line;
        }

        internal IEnumerable<GridCellEditorModel> GetEditCells()
        {
            if (this.EditRow != null)
            {
                return this.CellEditorsController.GetCellsForRow(this.EditRow.ItemInfo.Slot);
            }
            return Enumerable.Empty<GridCellEditorModel>();
        }

        internal void UpdateEditRow(int newIndex)
        {
            if (!this.isInEditMode)
            {
                return;
            }

            if (newIndex > 0)
            {
                var newInfo = this.RowPool.Layout.GetLines(newIndex, true).First().Last();
                this.EditRowPool.ReadOnlyItemInfo = newInfo;
                var offset = newInfo.Slot == 0 ? 0.0 : this.rowLayout.RenderInfo.OffsetFromIndex(newInfo.Slot - 1);

                this.EditRowPool.UpdateEditRow(newInfo, new RadPoint(0, offset), this.FrozenColumnCount > 0);
            }
        }

        private object FinishEdit()
        {
            var editRowHost = this.GetEditRowHost();

            if (editRowHost != null)
            {
                this.ShowRow(editRowHost);
            }

            this.isInEditMode = false;

            foreach (var pair in this.EditRowPool.GetDisplayedElements())
            {
                this.CellEditorsController.RecycleRow(pair.Key);
            }

            this.CellEditorsController.FullyRecycleDecorators();

            var item = this.EditRowPool.ReadOnlyItemInfo.Item;

            this.EditRowPool.FinishEdit();
            this.InvalidateRowsMeasure();

            return item;
        }

        private void ShowRow(GridRowModel row)
        {
            this.GeneratorsHost.RowItemGenerator.SetOpacity(row, 1);

            foreach (var cell in this.CellsController.GetCellsForRow(row.ItemInfo.Slot))
            {
                this.GeneratorsHost.CellItemGenerator.SetOpacity(cell, 1);
            }
        }

        private void HideRow(GridRowModel row)
        {
            this.GeneratorsHost.RowItemGenerator.SetOpacity(row, 0);

            foreach (var cell in this.CellsController.GetCellsForRow(row.ItemInfo.Slot))
            {
                this.GeneratorsHost.CellItemGenerator.SetOpacity(cell, 0);
            }
        }

        private void UpdateEditRow()
        {
            if (!this.isInEditMode)
            {
                return;
            }

            var editRowHost = this.GetEditRowHost();

            if (editRowHost != null)
            {
                this.EditRowPool.UpdateEditRow(editRowHost.ItemInfo, editRowHost.LayoutSlot.Location, this.FrozenColumnCount > 0);
                this.HideRow(editRowHost);
            }
        }

        private GridRowModel GetEditRowHost()
        {
            foreach (var element in this.RowPool.GetDisplayedElements().SelectMany(c => c.Value))
            {
                if (object.ReferenceEquals(element.ItemInfo.Item, this.EditRowPool.ReadOnlyItemInfo.Item))
                {
                    return element;
                }
            }

            return null;
        }

        private void ArrangeEditorsPool(RadSize finalSize)
        {
            if (this.isInEditMode && this.EditRowPool.EditRow != null)
            {
                var line = this.EditRowPool.EditRow.ReadOnlyRowInfo.LayoutInfo.Line;
                var viewRow = this.RowPool.GetDisplayedElement(line);

                if (viewRow != null)
                {
                    this.EditRowPool.ArrangeEditRow();
                    this.CellEditorsController.OnCellsArrange(finalSize);
                }
            }
        }
    }
}
