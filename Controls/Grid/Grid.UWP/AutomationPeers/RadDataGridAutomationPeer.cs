using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Automation.Peers
{
    public class RadDataGridAutomationPeer : RadControlAutomationPeer, IGridProvider, ISelectionProvider, ITableProvider
    {
        internal List<DataGridCellInfoAutomationPeer> childrenCache;

        /// <inheritdoc />
        public RadDataGridAutomationPeer(RadDataGrid owner)
            : base(owner)
        {
        }

        internal RadDataGrid OwnerDataGrid
        {
            get
            {
                return (RadDataGrid)this.Owner;
            }
        }

        /// <summary>
        /// ISelectionProvider implementation.
        /// </summary>
        public bool CanSelectMultiple
        {
            get
            {
                return this.OwnerDataGrid.SelectionMode != DataGridSelectionMode.Single && this.OwnerDataGrid.SelectionMode != DataGridSelectionMode.None;
            }
        }

        /// <summary>
        /// IGridProvider implementation.
        /// </summary>
        public int ColumnCount
        {
            get
            {
                return this.OwnerDataGrid.Columns.Count;
            }
        }

        /// <summary>
        /// ISelectionProvider implementation.
        /// </summary>
        public bool IsSelectionRequired
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// IGridProvider implementation.
        /// </summary>
        public int RowCount
        {
            get
            {
                return this.OwnerDataGrid.Model.RowPool.GetDisplayedElements().Count();
            }
        }

        /// <summary>
        /// ITableProvider implementation.
        /// </summary>
        public RowOrColumnMajor RowOrColumnMajor
        {
            get
            {
                return RowOrColumnMajor.RowMajor;
            }
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Table || patternInterface == PatternInterface.Grid
                || patternInterface == PatternInterface.Selection)
            {
                return this;
            }

            return base.GetPatternCore(patternInterface);
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(RadDataGrid);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(RadDataGrid);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad data grid";
        }

        /// <inheritdoc />
        protected override string GetAutomationIdCore()
        {
            var automationId = base.GetAutomationIdCore();
            if (!string.IsNullOrEmpty(automationId))
            {
                return automationId;
            }

            return nameof(RadDataGrid);
        }
        
        /// <summary>
        /// ITableProvider implementation.
        /// </summary>
        public IRawElementProviderSimple[] GetColumnHeaders()
        {
            var displayedColumns = this.OwnerDataGrid.ColumnHeadersHost.Children;
            IRawElementProviderSimple[] providers = new IRawElementProviderSimple[displayedColumns.Count];

            if (displayedColumns != null)
            {
                for (int i = 0; i < displayedColumns.Count; i++)
                {
                    var column = displayedColumns[i] as DataGridColumnHeader;
                    if (column != null)
                    {
                        AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(column);
                        if (peer != null)
                        {
                            providers[i] = this.ProviderFromPeer(peer);
                        }
                    }
                }
            }

            return providers;
        }

        /// <summary>
        /// IGridProvider implementation.
        /// </summary>
        public IRawElementProviderSimple GetItem(int rowIndex, int columnIndex)
        {
            IGridProvider gridProvider = (IGridProvider)this;
            int rowCount = gridProvider.RowCount;
            int columnCount = gridProvider.ColumnCount;

            if (rowIndex < 0 || rowIndex >= rowCount)
            {
                throw new ArgumentOutOfRangeException("row");
            }

            if (columnIndex < 0 || columnIndex >= columnCount)
            {
                throw new ArgumentOutOfRangeException("column");
            }

            var cellsForRow = this.OwnerDataGrid.Model.CellsController.GetCellsForRow(rowIndex);
            if (cellsForRow.Count() > 0)
            {
                var cell = cellsForRow.Where(a => a.Column.ItemInfo.Slot == columnIndex).FirstOrDefault();
                if (cell != null)
                {
                    var gridCellInfoPeer = this.childrenCache.Where(a => a.Row == cell.ParentRow.ItemInfo.Slot && a.Column == cell.Column.ItemInfo.Slot).First() as DataGridCellInfoAutomationPeer;
                    if (gridCellInfoPeer != null)
                    {
                        return this.ProviderFromPeer(gridCellInfoPeer);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// ITableProvider implementation.
        /// </summary>
        public IRawElementProviderSimple[] GetRowHeaders()
        {
            var groupHeadersHost = this.OwnerDataGrid.GroupHeadersHost as Panel;
            if (groupHeadersHost != null && groupHeadersHost.Children.Count > 0)
            {
                var dataGridContentLayerPanel = groupHeadersHost.Children.Where(a => a is DataGridContentLayerPanel).FirstOrDefault() as DataGridContentLayerPanel;
                if (dataGridContentLayerPanel != null)
                {
                    IRawElementProviderSimple[] providers = new IRawElementProviderSimple[dataGridContentLayerPanel.Children.Count];
                    for (int i = 0; i < dataGridContentLayerPanel.Children.Count; i++)
                    {
                        var groupColumn = dataGridContentLayerPanel.Children[i] as DataGridGroupHeader;
                        if (groupColumn != null)
                        {
                            AutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(groupColumn);
                            if (peer != null)
                            {
                                providers[i] = this.ProviderFromPeer(peer);
                            }
                        }
                    }

                    return providers;
                }
                
            }

            return null;
        }

        /// <summary>
        /// ISelectionProvider implementation.
        /// </summary>
        public IRawElementProviderSimple[] GetSelection()
        {
            var providerSamples = new List<IRawElementProviderSimple>();
            if (this.childrenCache != null)
            {
                if (this.OwnerDataGrid.SelectionUnit == DataGridSelectionUnit.Cell)
                {
                    foreach (var selectedCellInfo in this.OwnerDataGrid.selectionService.selectedCellsSet)
                    {
                        if (selectedCellInfo != null)
                        {
                            var gridCellInfoPeer = this.childrenCache.Where(a => a.Row == selectedCellInfo.RowItemInfo.Slot && a.Column == selectedCellInfo.Column.ItemInfo.Slot).First() as DataGridCellInfoAutomationPeer;
                            if (gridCellInfoPeer != null)
                            {
                                providerSamples.Add(this.ProviderFromPeer(gridCellInfoPeer));
                            }
                        }
                    }
                }
                else
                {
                    foreach (var selectedItem in this.OwnerDataGrid.selectionService.selectedRowsSet)
                    {
                        if (selectedItem != null)
                        {
                            var gridCellInfoPeers = this.childrenCache.Where(a => a.Item == selectedItem);
                            if (gridCellInfoPeers.Count() > 0)
                            {
                                foreach (var gridCellInfoPeer in gridCellInfoPeers)
                                {
                                    if (gridCellInfoPeer != null)
                                    {
                                        providerSamples.Add(this.ProviderFromPeer(gridCellInfoPeer));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return providerSamples.ToArray();
        }

        /// <inheritdoc />
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            if (this.childrenCache == null)
            {
                this.childrenCache = new List<DataGridCellInfoAutomationPeer>();
            }

            var children = base.GetChildrenCore().ToList();
            if (children != null && children.Count > 0)
            {
                children.RemoveAll(x => x.GetClassName() == nameof(DataGridBusyOverlayControl)
                || x.GetClassName() == nameof(DataGridAutoDataLoadingControl)
                || x.GetClassName() == nameof(SelectionRegionBorderControl)
                || x.GetClassName() == nameof(SelectionRegionBackgroundControl)
                || x.GetClassName() == nameof(DataGridCurrencyControl));

                var scrollViewerPeer = children.Where(x => x.GetClassName() == nameof(ScrollViewer)).FirstOrDefault();
                if (scrollViewerPeer != null)
                {
                    var scrollViewerChildren = scrollViewerPeer.GetChildren();
                    if (scrollViewerChildren.Count > 0)
                    {
                        var dataGridCellsPanelPeer = scrollViewerChildren.Where(a => a.GetClassName() == nameof(DataGridCellsPanel)).FirstOrDefault();
                        if (dataGridCellsPanelPeer != null)
                        {
                            dataGridCellsPanelPeer.GetChildren();
                        }
                    }
                }

                if (this.childrenCache.Count > 0)
                {
                    foreach (var cellPeer in this.childrenCache)
                    {
                        if (!string.IsNullOrEmpty(cellPeer.GetName()))
                        {
                            cellPeer.GetChildren();
                        }
                    }
                }
            }

            return children;
        }
    }
}
