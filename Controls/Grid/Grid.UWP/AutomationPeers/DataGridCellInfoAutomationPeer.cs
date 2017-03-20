using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Windows.Foundation;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    public class DataGridCellInfoAutomationPeer : AutomationPeer, ISelectionItemProvider, IGridItemProvider, IInvokeProvider
    {
        private readonly int column;
        private readonly RadDataGrid radDataGrid;
        private readonly int row;
        private readonly object item;

        private TextBlockAutomationPeer childTextBlockPeer;
        private string automationId;
        
        /// <summary>
        /// The Row index of the Cell.
        /// </summary>
        internal int Row
        {
            get
            {
                return this.row;
            }
        }

        /// <summary>
        /// The Column index of the Cell.
        /// </summary>
        internal int Column
        {
            get
            {
                return this.column;
            }
        }

        /// <summary>
        /// The TextBlock peer of the Cell.
        /// </summary>
        internal TextBlockAutomationPeer ChildTextBlockPeer
        {
            get
            {
                return this.childTextBlockPeer;
            }
            set
            {
                this.childTextBlockPeer = value;
            }
        }

        /// <summary>
        /// The item of the cell.
        /// </summary>
        internal object Item
        {
            get
            {
                return this.item;
            }
        }

        public DataGridCellInfoAutomationPeer(int row, int column, RadDataGridAutomationPeer radDataGridAutomationPeer, object item)
            : base()
        {
            this.radDataGrid = radDataGridAutomationPeer.OwnerDataGrid;
            this.row = row;
            this.column = column;
            this.item = item;
        }

        /// <summary>
        /// ISelectionItemProvider implementation.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                var cell = this.radDataGrid.Model.CellsController.GetCellsForRow(this.row).Where(a => a.Column.ItemInfo.Slot == this.column).FirstOrDefault();
                if (cell != null && this.radDataGrid.SelectionMode != DataGridSelectionMode.None)
                {
                    if (this.radDataGrid.SelectionMode == DataGridSelectionMode.Single)
                    {
                        if (this.radDataGrid.SelectionUnit == DataGridSelectionUnit.Row && this.radDataGrid.SelectedItem!= null 
                            && this.radDataGrid.SelectedItem == cell.ParentRow.ItemInfo.Item)
                        {
                            return true;
                        }

                        var selectedCellInfo = this.radDataGrid.SelectedItem as DataGridCellInfo;
                        if (this.radDataGrid.SelectionUnit == DataGridSelectionUnit.Cell && selectedCellInfo != null 
                            && selectedCellInfo.Value.Equals(cell.Value))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (this.radDataGrid.SelectionUnit == DataGridSelectionUnit.Row 
                            && this.radDataGrid.SelectedItems.Contains(cell.ParentRow.ItemInfo.Item))
                        {
                            return true;
                        }
                        else if (this.radDataGrid.SelectionUnit == DataGridSelectionUnit.Cell 
                            && this.radDataGrid.SelectedItems.OfType<DataGridCellInfo>().Any(a => a.Value.Equals(cell.Value)))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// ISelectionItemProvider implementation.
        /// </summary>
        public IRawElementProviderSimple SelectionContainer
        {
            get
            {
                if (this.radDataGrid != null)
                {
                    return this.ProviderFromPeer(FrameworkElementAutomationPeer.CreatePeerForElement(this.radDataGrid));

                }
                return null;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// IGridItemProvider implementation.
        /// </summary>
        int IGridItemProvider.Column
        {
            get
            {
                return this.column;
            }
        }

        /// <summary>
        /// IGridItemProvider implementation.
        /// </summary>
        public int ColumnSpan
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// IGridItemProvider implementation.
        /// </summary>
        public IRawElementProviderSimple ContainingGrid
        {
            get
            {
                if (this.radDataGrid != null)
                {
                    return this.ProviderFromPeer(FrameworkElementAutomationPeer.CreatePeerForElement(this.radDataGrid));

                }
                return null;
            }
        }

        /// <summary>
        /// IGridItemProvider implementation.
        /// </summary>
        int IGridItemProvider.Row
        {
            get
            {
                return this.row;
            }
        }

        /// <summary>
        /// IGridItemProvider implementation.
        /// </summary>
        public int RowSpan
        {
            get
            {
                return 0;
            }
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "data grid cell";
        }

        /// <inheritdoc />
        protected override bool HasKeyboardFocusCore()
        {
            return true;
        }

        /// <inheritdoc />
        protected override string GetAcceleratorKeyCore()
        {
            return string.Empty;
        }

        /// <inheritdoc />
        protected override string GetAccessKeyCore()
        {
            return string.Empty;
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.DataItem;
        }

        /// <inheritdoc />
        protected override string GetAutomationIdCore()
        {
            if (this.automationId == null)
            {
                this.automationId = string.Format("RadDataGridCell_Cell_{0}_{1}", this.row, this.column);
            }

            return this.automationId;
        }

        /// <inheritdoc />
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            if (this.childTextBlockPeer != null)
            {
                var children = new List<AutomationPeer>();
                children.Add(this.childTextBlockPeer);
                return children;
            }
            return null;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(DataGridCellInfo);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return "datagrid cell";
        }

        /// <inheritdoc />
        protected override string GetItemTypeCore()
        {
            return string.Empty;
        }

        /// <inheritdoc />
        protected override AutomationPeer GetLabeledByCore()
        {
            return null;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var cell = this.radDataGrid.Model.CellsController.GetCellsForRow(this.row).Where(a => a.Column.ItemInfo.Slot == this.column).FirstOrDefault();
            if (cell != null && cell.Value != null)
            {
                return cell.Value.ToString();
            }

            return string.Empty;
        }

        /// <inheritdoc />
        protected override Rect GetBoundingRectangleCore()
        {
            if (this.ChildTextBlockPeer != null)
            {
                return this.ChildTextBlockPeer.GetBoundingRectangle();
            }
            
            return base.GetBoundingRectangleCore();
        }

        /// <inheritdoc />
        protected override AutomationOrientation GetOrientationCore()
        {
            return AutomationOrientation.None;
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.SelectionItem
                || patternInterface == PatternInterface.GridItem
                || (this.radDataGrid.UserEditMode != DataGridUserEditMode.None && patternInterface == PatternInterface.Invoke))
            {
                return this;
            }

            return base.GetPatternCore(patternInterface);
        }

        /// <inheritdoc />
        protected override bool IsContentElementCore()
        {
            return true;
        }

        /// <inheritdoc />
        protected override bool IsControlElementCore()
        {
            return true;
        }

        /// <inheritdoc />
        protected override bool IsEnabledCore()
        {
            return true;
        }

        /// <inheritdoc />
        protected override bool IsOffscreenCore()
        {
            return false;
        }

        /// <inheritdoc />
        protected override bool IsPasswordCore()
        {
            return false;
        }

        /// <inheritdoc />
        protected override bool IsRequiredForFormCore()
        {
            return false;
        }

        /// <summary>
        /// ISelectionItemProvider implementation.
        /// </summary>
        public void AddToSelection()
        {
            this.SelectCell();
        }

        /// <summary>
        /// ISelectionItemProvider implementation.
        /// </summary>
        public void RemoveFromSelection()
        {
            var cell = this.radDataGrid.Model.CellsController.GetCellsForRow(this.row).Where(a => a.Column.ItemInfo.Slot == this.column).FirstOrDefault();
            if (cell != null)
            {
                if (this.radDataGrid.SelectionUnit == DataGridSelectionUnit.Row)
                {
                    this.radDataGrid.DeselectItem(cell.ParentRow.ItemInfo.Item);
                }
                else
                {
                    this.radDataGrid.DeselectCell(new DataGridCellInfo(cell));
                }
            }
        }

        /// <summary>
        /// ISelectionItemProvider implementation.
        /// </summary>
        public void Select()
        {
            this.SelectCell();
        }

        private void SelectCell()
        {
            var cell = this.radDataGrid.Model.CellsController.GetCellsForRow(this.row).Where(a => a.Column.ItemInfo.Slot == this.column).FirstOrDefault();
            if (cell != null)
            {
                this.radDataGrid.OnCellTap(new DataGridCellInfo(cell));
            }
        }

        /// <summary>
        /// IInvokeProvider implementation.
        /// </summary>
        public void Invoke()
        {
            var cell = this.radDataGrid.Model.CellsController.GetCellsForRow(this.row).Where(a => a.Column.ItemInfo.Slot == this.column).FirstOrDefault();
            if (cell != null)
            {
                this.radDataGrid.BeginEdit(new DataGridCellInfo(cell), ActionTrigger.DoubleTap, null);
            }
        }
           
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseValuePropertyChangedEvent(bool oldValue, bool newValue)
        {
            this.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, oldValue, newValue);
        }
    }
}
