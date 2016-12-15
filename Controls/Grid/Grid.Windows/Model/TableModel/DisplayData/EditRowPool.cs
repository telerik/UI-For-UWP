using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Core;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class EditRowPool : INodePool<GridRowModel>
    {
        private KeyValuePair<int, List<GridRowModel>> dummyElement;
        private KeyValuePair<int, List<GridRowModel>> displayedElement;
        private IRenderInfo renderInfo;
        private IUIContainerGenerator<GridEditRowModel, RowGenerationContext> generator;
        private ITable table;
        private GridEditRowModel recycledRow;
        private GridEditRowModel frozenRecycledRow;
        private bool initialized;

        public EditRowPool(IUIContainerGenerator<GridEditRowModel, RowGenerationContext> generator, ITable tableBase)
        {
            this.generator = generator;
            this.table = tableBase;

            this.renderInfo = new IndexStorage(2);

            this.dummyElement = new KeyValuePair<int, List<GridRowModel>>(0, new List<GridRowModel> { new GridEditRowModel() });
        }

        public ItemInfo ReadOnlyItemInfo { get; set; }

        public IRenderInfo RenderInfo
        {
            get
            {
                return this.renderInfo;
            }
        }

        public double AvailableLength
        {
            get
            {
                return double.PositiveInfinity;
            }
        }

        public double HiddenPixels
        {
            get
            {
                return 0;
            }
        }

        public int ViewportItemCount
        {
            get
            {
                return 1;
            }
        }

        internal GridEditRowModel EditRow { get; private set; }

        internal GridEditRowModel FrozenEditRow { get; private set; }

        public IEnumerable<KeyValuePair<int, List<GridRowModel>>> GetDisplayedElements()
        {
            if (this.EditRow != null)
            {
                yield return this.dummyElement;
                yield return this.displayedElement;
            }
            yield break;
        }

        public GridRowModel GetDisplayedElement(int slot)
        {
            if (this.EditRow.ItemInfo.Slot == slot)
            {
                return this.EditRow;
            }

            return null;
        }

        public bool IsItemCollapsed(int cellSlot)
        {
            return false;
        }

        public void RefreshRenderInfo(double defaultValue)
        {
            this.RenderInfo.ResetToDefaultValues(null, defaultValue);
        }

        public RadRect GetPreviousDisplayedLayoutSlot(int slot)
        {
            if (this.ReadOnlyItemInfo.Slot == slot)
            {
                return this.EditRow.LayoutSlot;
            }

            return RadRect.Invalid;
        }

        internal void ArrangeEditRow()
        {
            this.table.Arrange(this.EditRow);

            if (this.FrozenEditRow != null)
            {
                this.table.Arrange(this.FrozenEditRow);
            }
        }

        internal void ShowRows()
        {
            if (this.EditRow != null)
            {
                this.generator.MakeVisible(this.EditRow);
            }

            if (this.FrozenEditRow != null)
            {
                this.generator.MakeVisible(this.FrozenEditRow);
            }
        }

        internal void RecycleRows()
        {
            if (this.EditRow != null)
            {
                this.table.RecycleRow(this.EditRow.ItemInfo);
                this.RecycleEditRow();
            }

            if (this.FrozenEditRow != null)
            {
                this.table.RecycleRow(this.FrozenEditRow.ItemInfo);
                this.RecycleFrozenEditRow();
            }
        }

        internal void UpdateEditRow(ItemInfo info, RadPoint startPosition, bool hasFrozenColumns)
        {
            this.renderInfo.Update(0, startPosition.Y);

            if (!this.initialized)
            {
                this.initialized = true;

                var editRow = this.GetDecorator(new RowGenerationContext(info, false));

                editRow.ItemInfo = new ItemInfo { Item = info.Item, Slot = 1 };
                editRow.ReadOnlyRowInfo = info;

                this.EditRow = editRow;

                var frozenEditRow = this.GetFrozenDecorator(new RowGenerationContext(info, true));

                frozenEditRow.ItemInfo = new ItemInfo { Item = info.Item, Slot = 1 };
                frozenEditRow.ReadOnlyRowInfo = info;
                
                this.FrozenEditRow = frozenEditRow;
            }

            this.generator.PrepareContainerForItem(this.EditRow);
            var size = this.table.Measure(this.EditRow);
            var height = this.table.GenerateCellsForRow(1, size.Height, this.EditRow);

            // var frozenHeight = this.table.GenerateCellsForRow(1, frozenSize.Height, this.FrozenEditRow);
            // var updatedHeight = Math.Max(height, frozenHeight);
            var updatedHeight = height;
            this.renderInfo.Update(1, updatedHeight);

            this.EditRow.DesiredSize = new RadSize(size.Width, updatedHeight);
            this.EditRow.layoutSlot = new RadRect(startPosition.X, startPosition.Y, this.EditRow.DesiredSize.Width, this.EditRow.DesiredSize.Height);

            this.generator.PrepareContainerForItem(this.FrozenEditRow);
            var frozenSize = this.table.Measure(this.FrozenEditRow);
            this.FrozenEditRow.DesiredSize = new RadSize(size.Width, updatedHeight);
            this.FrozenEditRow.layoutSlot = new RadRect(startPosition.X, startPosition.Y, this.FrozenEditRow.DesiredSize.Width, this.FrozenEditRow.DesiredSize.Height);

            if (!hasFrozenColumns)
            {
                this.generator.MakeHidden(this.FrozenEditRow);
            }

            this.displayedElement = new KeyValuePair<int, List<GridRowModel>>(1, new List<GridRowModel> { this.EditRow, this.FrozenEditRow });
        }

        internal void FinishEdit()
        {
            this.ReadOnlyItemInfo = ItemInfo.Invalid;
            this.RecycleRows();
            this.initialized = false;
        }

        private void RecycleEditRow()
        {
            if (this.EditRow != null)
            {
                this.generator.ClearContainerForItem(this.EditRow);
                this.generator.MakeHidden(this.EditRow);
                this.recycledRow = this.EditRow;
                this.EditRow = null;
                this.RefreshRenderInfo(this.table.RowHeight);
            }
        }

        private void RecycleFrozenEditRow()
        {
            if (this.FrozenEditRow != null)
            {
                this.generator.ClearContainerForItem(this.FrozenEditRow);
                this.generator.MakeHidden(this.FrozenEditRow);
                this.frozenRecycledRow = this.FrozenEditRow;
                this.FrozenEditRow = null;
                this.RefreshRenderInfo(this.table.RowHeight);
            }
        }

        private GridEditRowModel GetDecorator(RowGenerationContext context)
        {
            GridEditRowModel row;

            if (this.recycledRow != null)
            {
                row = this.recycledRow;
                this.generator.MakeVisible(row);
                this.recycledRow = null;
            }
            else
            {
                row = new GridEditRowModel();
                var containerType = this.generator.GetContainerTypeForItem(context);
                row.Container = this.generator.GenerateContainerForItem(context, containerType);

                row.ContainerType = containerType;
            }

            return row;
        }

        private GridEditRowModel GetFrozenDecorator(RowGenerationContext context)
        {
            GridEditRowModel row;

            if (this.frozenRecycledRow != null)
            {
                row = this.frozenRecycledRow;
                this.generator.MakeVisible(row);
                this.frozenRecycledRow = null;
            }
            else
            {
                row = new GridEditRowModel() { IsFrozen = true };
                var containerType = this.generator.GetContainerTypeForItem(context);
                row.Container = this.generator.GenerateContainerForItem(context, containerType);

                row.ContainerType = containerType;
            }

            return row;
        }
    }
}