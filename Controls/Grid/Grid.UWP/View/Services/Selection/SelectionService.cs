﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Core;
using Telerik.Data.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class SelectionService : Telerik.UI.Xaml.Controls.Primitives.ServiceBase<RadDataGrid>
    {
        internal HashSet<object> selectedRowsSet;
        internal HashSet<DataGridCellInfo> selectedCellsSet;

        private SelectedItemCollection selectedItems;

        internal SelectionService(RadDataGrid owner)
            : base(owner)
        {
            if (this.Owner == null)
            {
                throw new ArgumentNullException("Selection service cannot operate without owner");
            }

            this.selectedItems = new SelectedItemCollection();
            this.selectedItems.AllowMultipleSelect = ( this.Owner.SelectionMode == DataGridSelectionMode.Multiple ||
                                                       this.Owner.SelectionMode == DataGridSelectionMode.Extended );
            this.selectedItems.SelectionUnit = this.Owner.SelectionUnit;

            this.selectedItems.CollectionChanged += this.OnSelectedItemsCollectionChanged;

            this.selectedRowsSet = new HashSet<object>();
            this.selectedCellsSet = new HashSet<DataGridCellInfo>();
        }

        internal event EventHandler<DataGridSelectionChangedEventArgs> SelectionChanged;

        public SelectedItemCollection SelectedItems
        {
            get
            {
                return this.selectedItems;
            }
        }

        internal DataGridSelectionUnit SelectionUnit
        {
            get
            {
                return this.Owner.SelectionUnit;
            }
        }

        internal DataGridSelectionMode SelectionMode
        {
            get
            {
                return this.Owner.SelectionMode;
            }
        }

        internal async void Select(GridCellModel gridCellModel, bool toggleSelection = true)
        {
            var dataGridPeer = FrameworkElementAutomationPeer.FromElement(this.Owner) as RadDataGridAutomationPeer;
            if (dataGridPeer != null && dataGridPeer.childrenCache != null)
            {
                if (dataGridPeer.childrenCache.Count == 0)
                {
                    dataGridPeer.GetChildren();
                }

                var cellPeer = dataGridPeer.childrenCache.FirstOrDefault(a => a.Row == gridCellModel.ParentRow.ItemInfo.Slot && a.Column == gridCellModel.Column.ItemInfo.Slot) as DataGridCellInfoAutomationPeer;
                if (cellPeer != null && cellPeer.ChildTextBlockPeer != null)
                {
                    await Dispatcher.RunAsync(
                        Windows.UI.Core.CoreDispatcherPriority.Normal,
                        () => 
                        {
                            cellPeer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
                            cellPeer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
                            cellPeer.RaiseValuePropertyChangedEvent(false, true);
                        });
                }
            }

            switch (this.Owner.SelectionUnit)
            {
                case DataGridSelectionUnit.Row:
                    this.SelectItem((gridCellModel.Parent as GridRowModel).ItemInfo.Item, true, toggleSelection);
                    break;
                case DataGridSelectionUnit.Cell:
                    var cellInfo = new DataGridCellInfo(gridCellModel.ParentRow.ItemInfo, gridCellModel.Column);
                    this.SelectCellInfo(cellInfo, true, toggleSelection);
                    break;
                default:
                    throw new ArgumentException("Unknown selection unit type", "this.Owner.SelectionUnit");
            }
        }

        /// <summary>
        /// Selects a range of cells/rows
        /// </summary>
        /// <param name="fromColumn">The column to start selecting from</param>
        /// <param name="fromRow">The row to start selecting from</param>
        /// <param name="toColumn">The column to select to</param>
        /// <param name="toRow">The row to select to</param>
        internal void SelectRange( int fromColumn, int fromRow, int toColumn, int toRow )
        {
            // This ensures that we only select the columns in the range
            ClearSelection();

            // If the selection row's index is higher than the previous selection
            if (fromRow <= toRow)
            {
                // Loop through all the rows and select each row within the selection
                for (int index = fromRow; index <= toRow; index++)
                {
                    SelectRangeUnits(index, fromColumn, toColumn);
                }
            }
            else
            {
                // Loop through all the rows and select each row within the selection
                for (int index = fromRow; index >= toRow; index--)
                {
                    SelectRangeUnits(index, fromColumn, toColumn);
                }
            }
        }

        internal void SelectItem(object item, bool select, bool uiSelect)
        {
            if (!this.CanSelectDataItem(item))
            {
                return;
            }

            this.SelectRowUnit(item, select, uiSelect);
            this.OnSelectionChanged();
        }

        internal void SelectCellInfo(DataGridCellInfo cellInfo, bool select, bool uiSelect)
        {
            if (!this.CanSelectCellInfo(cellInfo))
            {
                return;
            }

            this.SelectCellUnit(cellInfo, select, uiSelect);
            this.OnSelectionChanged();
        }

        internal void OnSelectionUnitChanged(DataGridSelectionUnit dataGridSelectionUnit)
        {
            this.ClearSelection();
            this.selectedItems.SelectionUnit = dataGridSelectionUnit;
        }

        internal void OnSelectionModeChanged(DataGridSelectionMode dataGridSelectionMode)
        {
            if (dataGridSelectionMode == DataGridSelectionMode.Single || dataGridSelectionMode == DataGridSelectionMode.None)
            {
                this.ClearSelection();
            }

            this.selectedItems.AllowMultipleSelect = ( dataGridSelectionMode == DataGridSelectionMode.Multiple ||
                                                       dataGridSelectionMode == DataGridSelectionMode.Extended );
        }

        internal void SelectAll()
        {
            if (this.SelectionMode == DataGridSelectionMode.Single || this.SelectionMode == DataGridSelectionMode.None)
            {
                return;
            }

            this.ClearSelection();

            switch (this.Owner.SelectionUnit)
            {
                case DataGridSelectionUnit.Row:
                    this.SelectAllRowItems();
                    break;
                case DataGridSelectionUnit.Cell:
                    this.SelectAllCellItems();
                    break;
                default:
                    break;
            }
        }

        internal void ClearSelection()
        {
            this.selectedRowsSet.Clear();
            this.selectedCellsSet.Clear();

            if (this.selectedItems.Count > 0)
            {
                this.selectedItems.Clear();
                this.Owner.ChangePropertyInternally(RadDataGrid.SelectedItemProperty, null);
            }

            this.OnSelectionChanged();
        }

        internal void OnSelectedItemChanged(object oldValue, object newValue)
        {
            if (newValue == null && this.selectedItems.Count > 0)
            {
                this.ClearSelection();
            }
            else if (!this.CanSelectItem(newValue))
            {
                this.Owner.ChangePropertyInternally(RadDataGrid.SelectedItemProperty, oldValue);
            }
            else
            {
                this.ClearSelection();
                switch (this.Owner.SelectionUnit)
                {
                    case DataGridSelectionUnit.Row:
                        this.SelectItem(newValue, true, false);
                        break;
                    case DataGridSelectionUnit.Cell:
                        this.SelectCellInfo(newValue as DataGridCellInfo, true, false);
                        break;
                    default:
                        break;
                }
            }
        }

        private bool CanSelectItem(object item)
        {
            var cellinfo = item as DataGridCellInfo;
            return cellinfo != null ? this.CanSelectCellInfo(cellinfo) : this.CanSelectDataItem(item);
        }

        private void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.selectedItems.SuspendUpdate();

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    this.UpdateItemsSet(e.OldItems, e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.Refresh();
                    break;
                case NotifyCollectionChangedAction.Move:
                default:
                    break;
            }
            this.selectedItems.ResumeUpdate();
        }

        private void Refresh()
        {
            IEnumerable<object> removedItems = null;
            IEnumerable<object> addedItems = null;

            switch (this.Owner.SelectionUnit)
            {
                case DataGridSelectionUnit.Row:

                    removedItems = this.selectedRowsSet.ToArray();

                    this.selectedCellsSet.Clear();
                    this.selectedRowsSet.Clear();
                    foreach (var item in this.SelectedItems)
                    {
                        this.selectedRowsSet.Add(item);
                    }

                    // Select different Instance of enumerator to ensure that users does not have access to the private instance of set.
                    addedItems = this.selectedRowsSet.Select(c => c);
                    break;
                case DataGridSelectionUnit.Cell:

                    removedItems = this.selectedCellsSet.ToArray();

                    this.selectedCellsSet.Clear();
                    this.selectedRowsSet.Clear();
                    foreach (var item in this.SelectedItems)
                    {
                        this.VerifyItemType(item);
                        this.selectedCellsSet.Add(item as DataGridCellInfo);
                    }

                    // Select different Instance of enumerator to ensure that users does not have access to the private instance of set.
                    addedItems = this.selectedCellsSet.Select(c => c);
                    break;
                default:
                    break;
            }

            this.OnSelectedItemsChanged(removedItems, addedItems);
            this.OnSelectionChanged();
        }

        private void UpdateItemsSet(IList removedItems, IList addedItems)
        {
            IEnumerable<object> itemsToRemove = removedItems == null ? Enumerable.Empty<object>() : removedItems.OfType<object>();
            IEnumerable<object> itemsToAdd = addedItems == null ? Enumerable.Empty<object>() : addedItems.OfType<object>();

            switch (this.Owner.SelectionUnit)
            {
                case DataGridSelectionUnit.Row:
                    foreach (var item in itemsToRemove)
                    {
                        this.SelectRowUnit(item, false, false);
                    }

                    foreach (var item in itemsToAdd)
                    {
                        this.SelectRowUnit(item, true, false);
                    }
                    break;
                case DataGridSelectionUnit.Cell:
                    foreach (var item in itemsToRemove)
                    {
                        this.SelectCellInfo(item as DataGridCellInfo, false, false);
                    }

                    foreach (var item in itemsToAdd)
                    {
                        this.VerifyItemType(item);
                        this.SelectCellInfo(item as DataGridCellInfo, true, false);
                    }
                    break;
                default:
                    break;
            }

            this.OnSelectedItemsChanged(itemsToRemove, itemsToAdd);
        }

        private void OnSelectedItemsChanged(IEnumerable<object> removedItems, IEnumerable<object> addedItems)
        {
            this.UpdateSelectedItem();

            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this.Owner, new DataGridSelectionChangedEventArgs(removedItems, addedItems));
            }
        }

        private void UpdateSelectedItem()
        {
            if (this.SelectedItems.Count > 0)
            {
                if (this.Owner.SelectedItem == null || !this.SelectedItems.Contains(this.Owner.SelectedItem))
                {
                    this.Owner.ChangePropertyInternally(RadDataGrid.SelectedItemProperty, this.SelectedItems.First());
                }
            }
            else
            {
                this.Owner.ChangePropertyInternally(RadDataGrid.SelectedItemProperty, null);
            }
        }

        private void VerifyItemType(object item)
        {
            var cellInfo = item as DataGridCellInfo;
            if (cellInfo == null || this.Owner.SelectionUnit != DataGridSelectionUnit.Cell)
            {
                throw new InvalidOperationException("Cannot select item if selection unit is cell");
            }
        }

        private bool CanSelectCellInfo(DataGridCellInfo cell)
        {
            return cell != null && this.Owner.SelectionMode != DataGridSelectionMode.None && this.Owner.SelectionUnit == DataGridSelectionUnit.Cell;
        }

        private void SelectCellUnit(DataGridCellInfo cellInfo, bool select, bool uiSelect)
        {
            if (!this.CanSelectCellInfo(cellInfo))
            {
                return;
            }

            switch (this.Owner.SelectionMode)
            {
                case DataGridSelectionMode.Single:
                    this.SelectSingleCellUnit(cellInfo, select);
                    break;
                case DataGridSelectionMode.Multiple:
                case DataGridSelectionMode.Extended:
                    this.SelectMultipleCellUnits(cellInfo, select, uiSelect);
                    break;
                default:
                    break;
            }
        }

        private void SelectSingleCellUnit(DataGridCellInfo cellInfo, bool select)
        {
            this.selectedCellsSet.Clear();

            this.SelectSingleItemCore(cellInfo, select);

            if (select)
            {
                this.selectedCellsSet.Add(cellInfo);
            }
        }

        private void SelectMultipleCellUnits(DataGridCellInfo cellInfo, bool select, bool uiSelect)
        {
            if (!select || (uiSelect && this.selectedCellsSet.Contains(cellInfo)))
            {
                this.selectedCellsSet.Remove(cellInfo);
                this.selectedItems.Remove(cellInfo);
            }
            else if (select)
            {
                this.selectedCellsSet.Add(cellInfo);
                this.selectedItems.Add(cellInfo);
            }
        }

        private void SelectAllCellItems()
        {
            var newSelectedItems = this.selectedCellsSet.ToArray();

            // TODO: schedule update if data is not ready.
            foreach (var item in this.Owner.GetDataView())
            {
                if (item is IDataGroup)
                {
                    continue;
                }

                foreach (var column in this.Owner.Columns)
                {
                    this.SelectCellUnit(new DataGridCellInfo(item, column), true, false);
                }
            }

            this.OnSelectedItemsChanged(newSelectedItems, this.selectedCellsSet.Select(c => c));

            this.OnSelectionChanged();
        }

        private bool CanSelectDataItem(object item)
        {
            return item != null && this.Owner.SelectionMode != DataGridSelectionMode.None && this.Owner.SelectionUnit == DataGridSelectionUnit.Row;
        }

        private void SelectRowUnit(object rowItem, bool select, bool toggleSelection)
        {
            switch (this.Owner.SelectionMode)
            {
                case DataGridSelectionMode.Single:
                    this.SelectSingleRowUnit(rowItem, select);
                    break;
                case DataGridSelectionMode.Multiple:
                case DataGridSelectionMode.Extended:
                    this.SelectMultipleRowUnits(rowItem, select, toggleSelection);
                    break;
                default:
                    break;
            }
        }

        private void SelectSingleRowUnit(object rowItem, bool select)
        {
            this.selectedRowsSet.Clear();

            this.SelectSingleItemCore(rowItem, select);

            if (select)
            {
                this.selectedRowsSet.Add(rowItem);
            }
        }

        private void SelectSingleItemCore(object item, bool select)
        {
            if (this.selectedItems.Count > 1)
            {
                this.selectedItems.Clear();
            }

            if (select)
            {
                this.selectedRowsSet.Add(item);

                if (this.selectedItems.Count == 1)
                {
                    this.selectedItems[0] = item;
                }
                else
                {
                    this.selectedItems.Add(item);
                }
            }
            else if (this.selectedItems.Count == 1)
            {
                this.selectedItems.RemoveAt(0);
            }
        }

        private void SelectMultipleRowUnits(object rowInfo, bool select, bool toggleSelection)
        {
            var info = rowInfo;

            if (!select || (toggleSelection && this.selectedRowsSet.Contains(info)))
            {
                this.selectedRowsSet.Remove(info);
                this.selectedItems.Remove(info);
            }
            else if (select)
            {
                this.selectedRowsSet.Add(info);
                this.selectedItems.Add(info);
            }
        }

        private void SelectAllRowItems()
        {
            var newSelectedItems = this.selectedRowsSet.ToArray();

            // TODO: schedule update if data is not ready.
            foreach (var item in this.Owner.GetDataView())
            {
                if (item is IDataGroup)
                {
                    continue;
                }

                this.SelectRowUnit(item, true, false);
            }

            this.OnSelectedItemsChanged(newSelectedItems, this.selectedRowsSet.Select(c => c));

            this.OnSelectionChanged();
        }

        /// <summary>
        /// Aids in the range selection process and will select based on the SelectionUnit's value
        /// </summary>
        /// <param name="row">The row for the selection</param>
        /// <param name="fromColumn">The column to start selecting from</param>
        /// <param name="toColumn">The column to select to</param>
        private void SelectRangeUnits(int row, int fromColumn, int toColumn)
        {
            if (SelectionUnit == DataGridSelectionUnit.Row)
            {
                // Since we are in row selection mode just select the entire row.
                SelectRangeCells(row);
            }
            else
            {
                // If we are selecting cells, check if their column is before or after the current column
                if (fromColumn <= toColumn)
                {
                    // The selected to column is after the currently select column
                    for (int columnIndex = fromColumn; columnIndex <= toColumn; columnIndex++)
                    {
                        SelectRangeCells(row, columnIndex);
                    }
                }
                else
                {
                    // The selected to column is before the currently select column
                    for (int columnIndex = fromColumn; columnIndex >= toColumn; columnIndex--)
                    {
                        SelectRangeCells(row, columnIndex);
                    }
                }
            }
        }

        private void SelectRangeCells(int row, int column = -1)
        {
            // The correct GridCellModel based on if we are doing a row selection or cell selection
            GridCellModel model = column == -1 ?
                this.Owner.Model.CellsController.GetCellsForRow(row).FirstOrDefault() :
                this.Owner.Model.CellsController.GetCellsForRow(row).FirstOrDefault(a => a.Column.ItemInfo.Slot == column);

            if (model != null)
            {
                // Perform the selection
                this.Select(model, false);
            }
        }

        private void OnSelectionChanged()
        {
            this.Owner.updateService.RegisterUpdate(UpdateFlags.AffectsDecorations);
        }
    }
}
