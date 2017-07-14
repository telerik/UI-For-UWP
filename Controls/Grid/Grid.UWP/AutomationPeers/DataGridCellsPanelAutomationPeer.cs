using System.Collections.Generic;
using System.Linq;
using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="DataGridCellsPanel"/>.
    /// </summary>
    public class DataGridCellsPanelAutomationPeer : FrameworkElementAutomationPeer
    {
        private readonly RadDataGrid dataGrid;

        /// <summary>
        /// Initializes a new instance of the DataGridCellsPanelAutomationPeer class.
        /// </summary>
        public DataGridCellsPanelAutomationPeer(DataGridCellsPanel owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataGridCellsPanelAutomationPeer class.
        /// </summary>
        public DataGridCellsPanelAutomationPeer(DataGridCellsPanel owner, RadDataGrid dataGrid) 
            : this(owner)
        {
            this.dataGrid = dataGrid;
        }

        private DataGridCellsPanel DataGridCellsPanel
        {
            get
            {
                return this.Owner as DataGridCellsPanel;
            }
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(DataGridCellsPanel);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "datagrid cells panel";
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.List;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
            {
                return nameCore;
            }

            return nameof(DataGridCellsPanel);
        }

        /// <inheritdoc />
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            List<AutomationPeer> children = new List<AutomationPeer>();
            List<AutomationPeer> dataGridContentLayerPanelChildren = new List<AutomationPeer>();

            var dataGridCellsPanelAutomationPeerChildren = base.GetChildrenCore();

	        if (dataGridCellsPanelAutomationPeerChildren.Count > 0)
            {
	            var dataGridContentLayerPanelPeer = dataGridCellsPanelAutomationPeerChildren.FirstOrDefault(a => a.GetName() == nameof(DataGridContentLayerPanel)) as DataGridContentLayerPanelAutomationPeer;
	            if (dataGridContentLayerPanelPeer != null)
                {
                    dataGridContentLayerPanelChildren = dataGridContentLayerPanelPeer.GetChildren().ToList();
                }
            }

            if (this.DataGridCellsPanel != null)
            {
                var dataGridPeers = FrameworkElementAutomationPeer.FromElement(this.dataGrid) as RadDataGridAutomationPeer;
                if (dataGridPeers != null && dataGridPeers.childrenCache != null)
                {
                    var rows = this.dataGrid.Model.RowPool.GetDisplayedItems();
                    foreach (var row in rows)
                    {
                        var cellsForRow = this.dataGrid.Model.CellsController.GetCellsForRow(row.Slot);
                        if (cellsForRow.Any())
                        {
                            foreach (var cell in cellsForRow)
                            {
                                DataGridCellInfoAutomationPeer peer = dataGridPeers.childrenCache.FirstOrDefault(a => a.Row == row.Slot && a.Column == cell.Column.ItemInfo.Slot);
                                if (peer == null)
                                {
                                    peer = new DataGridCellInfoAutomationPeer(row.Slot, cell.Column.ItemInfo.Slot, dataGridPeers, row.Item);
                                    dataGridPeers.childrenCache.Add(peer);
                                }

                                var cellContainer = cell.Container as TextBlock;
                                if (cellContainer != null)
                                {
                                    var tbPeer = FrameworkElementAutomationPeer.FromElement(cellContainer) as TextBlockAutomationPeer;
                                    if (tbPeer != null)
                                    {
                                        peer.ChildTextBlockPeer = tbPeer;
                                    }
                                }

                                children.Add(peer);
                            }
                        }
                    }
                }
            }

            if (dataGridContentLayerPanelChildren.Count > 0)
            {
                dataGridContentLayerPanelChildren.RemoveAll(a => a.GetClassName() == nameof(Windows.UI.Xaml.Controls.TextBlock));
                children.AddRange(dataGridContentLayerPanelChildren);
            }

            return children;
        }
    }
}
