using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid.Model
{
    internal partial class GridModel
    { 
        internal SelectionRegionController FrozenSelectionPresenter;

        // Currently this set keeps only individual cells/rows.
        private SortedSet<SelectionRegionInfo> selectionRegionsInfos = new SortedSet<SelectionRegionInfo>(new SelectionRegionInfoComparer());
        private List<SelectionRegionInfo> mergedSelectionRegions = new List<SelectionRegionInfo>();
        private SelectionRegionController selectionPresenter;

        internal SelectionRegionController SelectionPresenter
        {
            get
            {
                return this.selectionPresenter;
            }
        }

        internal void BuildSelectionRegions(IEnumerable<ItemInfo> realizedRows, IEnumerable<ItemInfo> realizedColumns)
        {
            switch (this.GridView.SelectionService.SelectionUnit)
            {
                case DataGridSelectionUnit.Row:
                    this.BuildRowSelectionRegions(realizedRows);
                    break;
                case DataGridSelectionUnit.Cell:
                    this.BuildCellSelectionRegions(realizedRows, realizedColumns);
                    break;
                default:
                    break;
            }
        }

        internal void BuildRowSelectionRegions(IEnumerable<ItemInfo> realizedRows)
        {
            ////TODO: replace this with sorted set with custom comparer
            var selectedItemsInView = new List<ItemInfo>();

            foreach (var selectedItem in this.GridView.SelectionService.selectedRowsSet)
            {
                var selectedInfos = realizedRows.Where(c => c.Item == selectedItem);

                if (selectedInfos.Any())
                {
                    selectedItemsInView.Add(selectedInfos.First());
                }
            }

            this.mergedSelectionRegions.Clear();

            this.MergeRowSelectionRegions(selectedItemsInView.OrderBy(c => c.LayoutInfo.Line));
        }

        private void MergeRowSelectionRegions(IEnumerable<ItemInfo> selectedRowItems)
        {
            foreach (var item in selectedRowItems)
            {
                bool merged = false;
                foreach (var prevRowItem in this.mergedSelectionRegions)
                {
                    if (prevRowItem.EndItem.RowItemInfo.LayoutInfo.Line + 1 == item.LayoutInfo.Line || item.LayoutInfo.Line <= prevRowItem.EndItem.RowItemInfo.LayoutInfo.Line)
                    {
                        prevRowItem.EndItem = new DataGridCellInfo(item, null);
                        merged = true;
                    }
                }
                if (!merged)
                {
                    var region = new SelectionRegionInfo();

                    region.StartItem = new DataGridCellInfo(item, null);
                    region.EndItem = new DataGridCellInfo(item, null);
                    this.mergedSelectionRegions.Add(region);
                }
            }
        }

        private void BuildCellSelectionRegions(IEnumerable<ItemInfo> realizedRows, IEnumerable<ItemInfo> realizedColumns)
        {
            // TODO: replace this with sorted set with custom comparer
            var selectedItemsInView = new List<DataGridCellInfo>();

            foreach (var selectedItem in this.GridView.SelectionService.selectedCellsSet)
            {
                var visibleByColumn = realizedColumns.Where(c => c.Item == selectedItem.Column);

                foreach (var item in visibleByColumn)
                {
                    var selectedInfos = realizedRows.Where(c => c.Item == selectedItem.Item);

                    if (selectedInfos.Any())
                    {
                        selectedItem.RowItemInfo = selectedInfos.First();
                        selectedItemsInView.Add(selectedItem);
                    }
                }
            }

            this.mergedSelectionRegions.Clear();

            this.MergeCellSelectionRegions(selectedItemsInView.OrderBy(c => c.Column.ItemInfo.LayoutInfo.Line).OrderBy(c => c.RowItemInfo.LayoutInfo.Line));
        }

        private void MergeCellSelectionRegions(IEnumerable<DataGridCellInfo> selectedCellItems)
        {
            this.mergedSelectionRegions.Clear();

            var mergedItemsByRows = new List<SelectionRegionInfo>();

            // merge regions by rows considering selectionRegionsInfos represents only simple cells
            int rowLine = -1;
            int columnLine = -1;
            SelectionRegionInfo startRegionInfo = null;
            foreach (var item in selectedCellItems)
            {
                if (rowLine < 0 || item.RowItemInfo.LayoutInfo.Line != rowLine ||
                    item.Column.ItemInfo.LayoutInfo.Line != columnLine)
                {
                    startRegionInfo = new SelectionRegionInfo() { StartItem = item, EndItem = item };
                    rowLine = item.RowItemInfo.LayoutInfo.Line;
                    columnLine = item.Column.ItemInfo.LayoutInfo.Line;
                    mergedItemsByRows.Add(startRegionInfo);
                }
                else
                {
                    startRegionInfo.EndItem = item;
                }

                columnLine++;
            }

            foreach (var item in mergedItemsByRows)
            {
                bool merged = false;
                foreach (var prevRowItem in this.mergedSelectionRegions)
                {
                    if (prevRowItem.EndItem.RowItemInfo.LayoutInfo.Line + 1 == item.StartItem.RowItemInfo.LayoutInfo.Line)
                    {
                        if ((prevRowItem.StartItem.Column == null && prevRowItem.EndItem.Column == null &&
                             item.StartItem.Column == null && item.EndItem.Column == null) ||
                            (prevRowItem.StartItem.Column.ItemInfo.LayoutInfo.Line == item.StartItem.Column.ItemInfo.LayoutInfo.Line &&
                             prevRowItem.EndItem.Column.ItemInfo.LayoutInfo.Line == item.EndItem.Column.ItemInfo.LayoutInfo.Line))
                        {
                            prevRowItem.EndItem = new DataGridCellInfo(item.StartItem.RowItemInfo, prevRowItem.EndItem.Column);
                            merged = true;
                        }
                    }
                }
                if (!merged)
                {
                    this.mergedSelectionRegions.Add(item);
                }
            }
        }
    }
}