using System;
using System.Collections.ObjectModel;
using System.Linq;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Grid
{
    public partial class RadDataGrid
    {
        /// <summary>
        /// Identifies the SelectionUnit dependency property. 
        /// </summary>
        public static readonly DependencyProperty SelectionUnitProperty =
            DependencyProperty.Register(nameof(SelectionUnit), typeof(DataGridSelectionUnit), typeof(RadDataGrid), new PropertyMetadata(DataGridSelectionUnit.Row, OnSelectionUnitChanged));

        /// <summary>
        /// Identifies the SelectionMode dependency property. 
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(nameof(SelectionMode), typeof(DataGridSelectionMode), typeof(RadDataGrid), new PropertyMetadata(DataGridSelectionMode.Single, OnSelectionModeChanged));

        /// <summary>
        /// Identifies the SelectedItem dependency property. 
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(RadDataGrid), new PropertyMetadata(null, OnSelectedItemChanged));

        internal SelectionService selectionService;

        /// <summary>
        /// Gets or sets the selection unit of the DataGrid. The default value is <c>DataGridSelectionUnit.Row</c>
        /// </summary>
        /// <example>
        /// XAML
        /// <code>
        /// &lt;telerikGrid:RadDataGrid x:Name="dataGrid" SelectionUnit="Cell"/&gt;
        /// </code>
        /// C#
        /// <code>
        /// this.dataGrid.SelectionUnit = DataGridSelectionUnit.Cell;
        /// </code>
        /// </example>
        public DataGridSelectionUnit SelectionUnit
        {
            get
            {
                return (DataGridSelectionUnit)this.GetValue(SelectionUnitProperty);
            }
            set
            {
                this.SetValue(SelectionUnitProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected item of the DataGrid.
        /// </summary>
        /// <remarks>
        /// When the value of the <see cref="SelectionUnit"/> is <see cref="DataGridSelectionUnit.Cell"/>, the selected item is of type <see cref="DataGridCellInfo"/>.
        /// When the value of the <see cref="SelectionUnit"/> is <see cref="DataGridSelectionUnit.Row"/>, the selected item is of the same type as the business object.
        /// </remarks>
        public object SelectedItem
        {
            get
            {
                return (object)this.GetValue(SelectedItemProperty);
            }
            set
            {
                this.SetValue(SelectedItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selection mode of the DataGrid. The default value is <c>DataGridSelectionMode.Single</c>.
        /// </summary>
        /// <example>
        /// XAML
        /// <code>
        ///  &lt;telerikGrid:RadDataGrid x:Name="dataGrid" SelectionMode="Multiple"/&gt;
        /// </code>
        /// C#
        /// <code>
        /// this.dataGrid.SelectionMode = DataGridSelectionMode.Multiple;
        /// </code>
        /// </example>
        public DataGridSelectionMode SelectionMode
        {
            get
            {
                return (DataGridSelectionMode)this.GetValue(SelectionModeProperty);
            }
            set
            {
                this.SetValue(SelectionModeProperty, value);
            }
        }

        /// <summary>
        /// Gets the currently selected items within this instance.
        /// The type of items within the collection depends on the current <see cref="SelectionUnit"/> value:
        ///     - The data item (row) when the selection unit is <see cref="DataGridSelectionUnit.Row"/>.
        ///     - A <see cref="DataGridCellInfo"/> object when the selection unit is <see cref="DataGridSelectionUnit.Cell"/>.
        /// </summary>
        public ObservableCollection<object> SelectedItems
        {
            get
            {
                return this.selectionService.SelectedItems;
            }
        }

        SelectionService ISelectionService.SelectionService
        {
            get
            {
                return this.selectionService;
            }
        }

        /// <summary>
        /// Selects the specified data item and adds it in the <see cref="SelectedItems"/> collection.
        /// </summary>
        /// <remarks>
        /// In order to select a Row, the <see cref="SelectionUnit"/> value should be <c>DataGridSelectionUnit.Row</c>.
        /// </remarks>
        public void SelectItem(object item)
        {
            this.selectionService.SelectItem(item, true, false);
        }

        /// <summary>
        /// Removes the selection for the specified data item and removes it from the <see cref="SelectedItems"/> collection.
        /// </summary>
        public void DeselectItem(object item)
        {
            var dataGridPeer = FrameworkElementAutomationPeer.FromElement(this) as RadDataGridAutomationPeer;
            if (dataGridPeer != null && dataGridPeer.childrenCache != null && dataGridPeer.childrenCache.Count > 0)
            {
                var cellPeer = dataGridPeer.childrenCache.Where(a => a.Item == item).FirstOrDefault() as DataGridCellInfoAutomationPeer;
                if (cellPeer != null && cellPeer.ChildTextBlockPeer != null)
                {
                    cellPeer.RaiseValuePropertyChangedEvent(true, false);
                }
            }

            this.selectionService.SelectItem(item, false, false);
        }

        /// <summary>
        /// Selects the grid cell as defined by the specified cell info.
        /// </summary>
        /// <remarks>
        /// In order to select a cell, the <see cref="SelectionUnit"/> value should be <c>DataGridSelectionUnit.Cell</c>.
        /// </remarks>
        public void SelectCell(DataGridCellInfo item)
        {
            this.selectionService.SelectCellInfo(item, true, false);
        }

        /// <summary>
        /// Removes the selection for the grid cell defined by the specified cell info.
        /// </summary>
        public void DeselectCell(DataGridCellInfo item)
        {
            var dataGridPeer = FrameworkElementAutomationPeer.FromElement(this) as RadDataGridAutomationPeer;
            if (dataGridPeer != null && dataGridPeer.childrenCache != null && dataGridPeer.childrenCache.Count > 0)
            {
                var cellPeer = dataGridPeer.childrenCache.Where(a => a.Row == item.RowItemInfo.Slot && a.Column == item.Column.ItemInfo.Slot).FirstOrDefault() as DataGridCellInfoAutomationPeer;
                if (cellPeer != null && cellPeer.ChildTextBlockPeer != null)
                {
                    cellPeer.RaiseValuePropertyChangedEvent(true, false);
                }
            }

            this.selectionService.SelectCellInfo(item, false, false);
        }

        /// <summary>
        /// Selects all the items as defined by the <see cref="SelectionMode"/> and <see cref="SelectionUnit"/>
        /// </summary>
        public void SelectAll()
        {
            this.selectionService.SelectAll();
        }

        /// <summary>
        /// Clears the current selection state.
        /// </summary>
        public void DeselectAll()
        {
            this.selectionService.ClearSelection();
        }

        private static void OnSelectionUnitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // TODO
            var grid = d as RadDataGrid;

            // Ensurance that itemscontrols bound to this property will not force incorrect refresh when the new and old value are same
            if (!e.OldValue.Equals(e.NewValue))
            {
                grid.selectionService.OnSelectionUnitChanged((DataGridSelectionUnit)e.NewValue);
            }
        }

        private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // TODO
            var grid = d as RadDataGrid;
            var newValue = (DataGridSelectionMode)e.NewValue;

            // Ensurance that itemscontrols bound to this property will not force incorrect refresh when the new and old value are same
            if (!e.OldValue.Equals(e.NewValue))
            {
                grid.selectionService.OnSelectionModeChanged((DataGridSelectionMode)e.NewValue);
            }
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;

            if (!grid.IsInternalPropertyChange)
            {
                grid.selectionService.OnSelectedItemChanged(e.OldValue, e.NewValue);
            }
            grid.CurrencyService.OnSelectedItemChanged(e.NewValue);
        }
    }
}