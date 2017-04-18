using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer info class for calendar cells showing DateTime.
    /// </summary>
    public class CalendarSelectableCellnfoAutomationPeer : CalendarCellInfoBaseAutomationPeer, ISelectionItemProvider
    {
        private string automationId;       
        internal CalendarCellModel CellModel { get; set; }

        /// <summary>
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarSelectableCellnfoAutomationPeer"/> class.
        /// </summary>
        /// <param name="parentPeer">Parent CalendarViewHostAutomationPeer.</param>
        /// <param name="cellModel">The model of the calendar cell.</param>
        internal CalendarSelectableCellnfoAutomationPeer(CalendarViewHostAutomationPeer parent, CalendarCellModel cellModel) : base(parent, cellModel)
        {
            this.CellModel = cellModel;
        }      

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "calendar cell";
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(CalendarCellModel);
        }

        /// <inheritdoc />
        protected override string GetAutomationIdCore()
        {
            if (this.automationId == null)
            {
                this.automationId = string.Format(nameof(CalendarCellModel) + "_{0}_{1}_{2}", this.CellModel.RowIndex, this.CellModel.ColumnIndex, this.CellModel.Date);
            }

            return this.automationId;
        }   

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.SelectionItem || patternInterface == PatternInterface.TableItem)
            {
                return this;
            }

            return base.GetPatternCore(patternInterface);
        }

        #region ISelectionItemProvider
        /// <summary>
        /// Selectes the date of the CellModel of the peer.
        /// </summary>
        public void AddToSelection()
        {
            if (!this.IsSelected)
            {
                this.CalendarViewHostPeer.CalendarOwner.SelectionService.Select(new CalendarDateRange(this.CellModel.Date, this.CellModel.Date));
                this.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, false, true);
            }
        }

        /// <summary>
        /// Deselect the selection in the calendar. Currently RadCalendar has no notion of deselecting single date.
        /// </summary>
        public void RemoveFromSelection()
        {
            // Calendar has no notion of deselecting a single cell.
            if (this.IsSelected)
            {
                this.CalendarViewHostPeer.CalendarOwner.SelectionService.ClearSelection();
                this.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, true, false);
            }
        }

        /// <summary>
        /// Selectes the date of the CellModel of the peer.
        /// </summary>
        public void Select()
        {
            if (!this.IsSelected)
            {
                this.CalendarViewHostPeer.CalendarOwner.SelectionService.SelectCell(this.CellModel);
                this.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, false, true);
            }
        }

        /// <summary>
        /// Indicates whether an item is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.CellModel.IsSelected;
            }
        }

        /// <summary>
        /// Gets the IRawElementProviderSimple - provider of the RadCalendar.
        /// </summary>
        public IRawElementProviderSimple SelectionContainer
        {
            get
            {
                return this.ContainingGrid;
            }
        }
        #endregion      
    }
}
