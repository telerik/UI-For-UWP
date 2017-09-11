using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Core;
using Telerik.Core.Data;
using Telerik.Data.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid.Model
{
    internal partial class GridModel : RootElement, ITable
    {
        internal static readonly DoubleArithmetics DoubleArithmetics = new DoubleArithmetics(IndexStorage.PrecisionMultiplier);

        internal static readonly double DefaultRowHeight = 40;
        internal static readonly double DefaultColumnWidth = 80;

        internal readonly IGridView GridView;
        internal readonly IGeneratorsHost GeneratorsHost;

        private bool executeOperationsSyncroniously;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        public GridModel(IGridView gridView, IGeneratorsHost generatorsHost, bool shouldExecuteSyncroniously)
        {
            this.executeOperationsSyncroniously = shouldExecuteSyncroniously;
            this.TrackPropertyChanged = true;

            this.GroupFactory = new DataGroupFactory();

            this.GeneratorsHost = generatorsHost;
            this.GridView = gridView;
            this.View = gridView;

            this.rowLayout = new CompactLayout(new GroupHierarchyAdapter(), DefaultRowHeight);
            this.columnLayout = new CompactLayout(new GroupHierarchyAdapter(), DefaultColumnWidth);

            this.columns = new DataGridColumnCollection(this);

            this.RowPool = new RowModelPool(this, new RowModelGenerator(generatorsHost.RowItemGenerator), this.rowLayout);
            this.ColumnPool = new ColumnHeaderPool(this, new ColumnModelGenerator(generatorsHost.ColumnItemGenerator), this.columnLayout);

            this.CellsController = new CellsController<GridCellModel>(this, new CellModelGenerator(generatorsHost.CellItemGenerator));
            this.CellsController.ColumnPool = this.ColumnPool;
            this.CellsController.RowPool = this.RowPool;

            this.EditRowPool = new EditRowPool(this.GeneratorsHost.EditRowItemGenerator, this);

            this.CellEditorsController = new CellEditorsController(this, new CellEditorModelGenerator(generatorsHost.CellEditorItemGenerator));
            this.CellEditorsController.ColumnPool = this.ColumnPool;
            this.CellEditorsController.RowPool = this.EditRowPool;

            // TODO: Consider providing it in the constructor or as a property of IGridView interface.
            this.DecorationsController = new DecorationsController(gridView.LineDecorationsPresenter);
            this.selectionPresenter = new SelectionRegionController(gridView.SelectionDecorationsPresenter);

            this.FrozenDecorationsController = new DecorationsController(gridView.FrozenLineDecorationsPresenter);
            this.FrozenSelectionPresenter = new SelectionRegionController(gridView.FrozenSelectionDecorationsPresenter);

            this.sortDescriptors = new SortDescriptorCollection(this);
            this.sortDescriptors.CollectionChanged += this.OnSortDescriptorsCollectionChanged;

            this.groupDescriptors = new GroupDescriptorCollection(this);
            this.groupDescriptors.CollectionChanged += this.OnGroupDescriptorsCollectionChanged;

            this.filterDescriptors = new FilterDescriptorCollection(this);
            this.filterDescriptors.CollectionChanged += this.OnFilterDescriptorsCollectionChanged;

            this.aggregateDescriptors = new AggregateDescriptorCollection(this);
            this.aggregateDescriptors.CollectionChanged += this.OnAggregateDescriptorsCollectionChanged;

            this.VerticalBufferScale = 1;
        }

        public double PhysicalVerticalOffset
        {
            get
            {
                return this.GridView.PhysicalVerticalOffset;
            }
        }

        public double PhysicalHorizontalOffset
        {
            get
            {
                return this.GridView.PhysicalHorizontalOffset;
            }
        }

        /// <summary>
        /// Gets the factory used to create groups when a group descriptor is applied.
        /// </summary>
        public IGroupFactory GroupFactory { get; private set; }

        internal double FrozenColumnsWidth
        {
            get
            {
                if (this.FrozenColumnCount > 0 && this.ColumnPool.GetFrozenDisplayedElements().Any())
                {
                    return this.ColumnPool.GetFrozenDisplayedElements().Last().Value.First().LayoutSlot.Right;
                }

                return 0;
            }
        }

        public void LoadElementTree()
        {
            if (this.IsTreeLoaded)
            {
                return;
            }

            this.Load(this);
        }

        public IEnumerable<GridHeaderCellModel> ForEachColumnHeaderCell()
        {
            foreach (var pair in this.ColumnPool.GetDisplayedElements())
            {
                foreach (var cell in pair.Value)
                {
                    yield return cell;
                }
            }
        }

        public IEnumerable<GridRowModel> ForEachRow()
        {
            foreach (var pair in this.RowPool.GetDisplayedElements())
            {
                foreach (var row in pair.Value)
                {
                    yield return row;
                }
            }
        }

        public IEnumerable<GridCellModel> ForEachRowCell(GridRowModel row)
        {
            return this.CellsController.GetCellsForRow(row.ItemInfo.Slot);
        }

        public IEnumerable<GridCellModel> ForEachFrozenRowCell(GridRowModel row)
        {
            return this.CellsController.GetCellsForRow(row.ItemInfo.Slot).Where(c => c.Column != null && c.Column.IsFrozen);
        }

        internal override ModifyChildrenResult CanAddChild(Node child)
        {
            if (child is GridRowModel)
            {
                return ModifyChildrenResult.Accept;
            }

            return base.CanAddChild(child);
        }

        internal RadSize MeasureContent(object owner, object content)
        {
            return this.GridView.MeasureContent(owner, content);
        }

        public bool HasExpandedRowDetails(int index)
        {
            var model = this.RowPool.GetDisplayedElement(index);

            if (model != null && model.ItemInfo.Item != null)
            {
                return this.GridView.RowDetailsService.HasExpandedRowDetails(model.ItemInfo.Item);
            }

            return false;
        }

        public void SetHeightForLine(int line, double value)
        {
            this.CellsController.UpdateSlotHeight(line, value);
        }
    }
}
