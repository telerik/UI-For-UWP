﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for RadCalendar.
    /// </summary>
    public class RadCalendarAutomationPeer : RadControlAutomationPeer, IMultipleViewProvider, ITableProvider, IGridProvider, ISelectionProvider, IValueProvider
    {
        internal readonly List<CalendarCellInfoBaseAutomationPeer> childrenCache;

        /// <summary>
        /// Initializes a new instance of the RadCalendarAutomationPeer class.
        /// </summary>
        /// <param name="owner">Owning RadCalendar.</param>
        public RadCalendarAutomationPeer(RadCalendar owner)
            : base(owner)
        {
            this.childrenCache = new List<CalendarCellInfoBaseAutomationPeer>();
        }

        /// <summary>
        /// IValueProvider implementation.
        /// </summary>
        public bool IsReadOnly => !this.CalendarOwner.IsEnabled;

        /// <summary>
        /// IValueProvider implementation.
        /// </summary>
        public string Value => this.CalendarOwner.DisplayDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

        /// <summary>
        /// Gets the index of the current view.
        /// </summary>
        public int CurrentView
        {
            get
            {
                return (int)this.CalendarOwner.DisplayMode;
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether the UI Automation provider allows more than one child element to be selected concurrently.
        /// </summary>
        public bool CanSelectMultiple
        {
            get
            {
                return this.CalendarOwner.SelectionMode == CalendarSelectionMode.Multiple;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the UI Automation provider requires at least one child element to be selected.
        /// </summary>
        public bool IsSelectionRequired
        {
            get
            {
                return false;
            }
        }
        
        /// <summary>
        /// Gets the number of columns in the grid.
        /// </summary>
        public int ColumnCount
        {
            get
            {
                return this.CalendarOwner.Model.ColumnCount;
            }
        }

        /// <summary>
        /// Gets the number of rows in the grid.
        /// </summary>
        public int RowCount
        {
            get
            {
                return this.CalendarOwner.Model.RowCount;
            }
        }
        
        /// <summary>
        /// Gets whether data in a table should be read primarily by row or by column.
        /// </summary>
        public RowOrColumnMajor RowOrColumnMajor
        {
            get
            {
                return Windows.UI.Xaml.Automation.RowOrColumnMajor.RowMajor;
            }
        }

        private RadCalendar CalendarOwner
        {
            get
            {
                return (RadCalendar)this.Owner;
            }
        }

        /// <summary>
        /// Array of all view's indices.
        /// </summary>
        public int[] GetSupportedViews()
        {
            int[] supportedViews = new int[4];

            supportedViews[0] = (int)CalendarDisplayMode.CenturyView;
            supportedViews[1] = (int)CalendarDisplayMode.DecadeView;
            supportedViews[2] = (int)CalendarDisplayMode.MonthView;
            supportedViews[3] = (int)CalendarDisplayMode.YearView;

            return supportedViews;
        }

        /// <summary>
        /// Sets the current view given an index.
        /// </summary>
        public void SetCurrentView(int viewId)
        {
            this.CalendarOwner.DisplayMode = (CalendarDisplayMode)viewId;
        }

        /// <summary>
        /// Retrieves a UI Automation provider for each child element that is selected.
        /// </summary>
        public IRawElementProviderSimple[] GetSelection()
        {
            List<IRawElementProviderSimple> providers = new List<IRawElementProviderSimple>();
            foreach (CalendarSelectableCellnfoAutomationPeer peer in this.childrenCache.OfType<CalendarSelectableCellnfoAutomationPeer>().Where(x => x.IsSelected))
            {
                providers.Add(this.ProviderFromPeer(peer));
            }
            return providers.ToArray();
        }

        /// <summary>
        /// Gets the name of the view given the index of the view.
        /// </summary>
        public string GetViewName(int viewId)
        {
            string[] viewNames = Enum.GetNames(typeof(CalendarDisplayMode));

            if (viewId < 0 || viewId >= viewNames.Length)
            {
                return string.Empty;
            }

            return viewNames[viewId];
        }

        /// <summary>
        /// Gets the provider of the peer for given row and column indices.
        /// </summary>
        /// <param name="row">The index of the peer's row.</param>
        /// <param name="column">The index of the peer's column.</param>
        public IRawElementProviderSimple GetItem(int row, int column)
        {
            CalendarSelectableCellnfoAutomationPeer peer = this.childrenCache
                .OfType<CalendarSelectableCellnfoAutomationPeer>().FirstOrDefault(x => (x.CellModel.RowIndex == row && x.CellModel.ColumnIndex == column));
            if (peer != null)
            {
                return this.ProviderFromPeer(peer);
            }

            return null;
        }

        /// <summary>
        /// List of providers of the column headers.
        /// </summary>
        public IRawElementProviderSimple[] GetColumnHeaders()
        {
            List<CalendarHeaderCellInfoAutomationPeer> peers = this.childrenCache.OfType<CalendarHeaderCellInfoAutomationPeer>()
                                .Where(x => x.HeaderCellModel.Type == CalendarHeaderCellType.DayName).ToList();

            if (peers.Count == 0)
            {
                return null;
            }

            IRawElementProviderSimple[] providers = new IRawElementProviderSimple[peers.Count];
            for (int i = 0; i < providers.Length; i++)
            {
                providers[i] = this.ProviderFromPeer(peers[i]);
            }

            return providers;
        }

        /// <summary>
        /// List of providers of the row headers.
        /// </summary>
        public IRawElementProviderSimple[] GetRowHeaders()
        {
            List<CalendarHeaderCellInfoAutomationPeer> peers = this.childrenCache.OfType<CalendarHeaderCellInfoAutomationPeer>()
                                .Where(x => x.HeaderCellModel.Type == CalendarHeaderCellType.WeekNumber).ToList();

            if (peers.Count == 0)
            {
                return null;
            }

            IRawElementProviderSimple[] providers = new IRawElementProviderSimple[peers.Count];
            for (int i = 0; i < providers.Length; i++)
            {
                providers[i] = this.ProviderFromPeer(peers[i]);
            }

            return providers;
        }

        /// <summary>
        /// IValueProvider implementation.
        /// </summary>
        public void SetValue(string value)
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("Read-only Calendar");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            DateTime parsedValue;
            if (DateTime.TryParse(value, out parsedValue))
            {
                this.CalendarOwner.DisplayDate = parsedValue;
            }
        }

        internal void ClearCache()
        {
            this.childrenCache.Clear();
        }

        internal void RaiseSelectionEvents(CurrentSelectionChangedEventArgs args)
        {
            if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected))
            {
                CalendarSelectableCellnfoAutomationPeer peer = this.GetOrCreatePeerFromDateTime(args.NewSelection);
                if (peer != null)
                {
                    peer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
                    peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
                }
            }

            if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection))
            {
                CalendarSelectableCellnfoAutomationPeer peer = this.GetOrCreatePeerFromDateTime(args.NewSelection);
                if (peer != null)
                {
                    peer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
                    peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
                }
            }
        }

        internal void RaiseAutomationFocusEvent(DateTime currentDate)
        {
            CalendarSelectableCellnfoAutomationPeer infoPeer = this.GetOrCreatePeerFromDateTime(currentDate);
            if (infoPeer != null)
            {
                infoPeer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
            }
        }
        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseValuePropertyChangedEvent(string oldValue, string newValue)
        {
            this.RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, oldValue, newValue);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad calendar";
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Calendar;
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.MultipleView || patternInterface == PatternInterface.Table
                || patternInterface == PatternInterface.Grid || patternInterface == PatternInterface.Selection
                || patternInterface == PatternInterface.Value)
            {
                return this;
            }
            return base.GetPatternCore(patternInterface);
        }

        /// <inheritdoc />
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            List<AutomationPeer> peers = new List<AutomationPeer>();
            var navigationControlPeer = FrameworkElementAutomationPeer.CreatePeerForElement(this.CalendarOwner.navigationPanel);
            if (navigationControlPeer != null)
            {
                navigationControlPeer.GetChildren();
                peers.Add(navigationControlPeer);
            }

            var viewHostPeer = FrameworkElementAutomationPeer.CreatePeerForElement(this.CalendarOwner.calendarViewHost);
            if (viewHostPeer != null)
            {
                viewHostPeer.GetChildren();
                peers.Add(viewHostPeer);
            }

            return peers;
        }

        private CalendarSelectableCellnfoAutomationPeer GetOrCreatePeerFromDateTime(DateTime date)
        {
            CalendarSelectableCellnfoAutomationPeer peer = this.childrenCache.OfType<CalendarSelectableCellnfoAutomationPeer>().FirstOrDefault(x => x.CellModel.Date == date);
            if (peer == null && this.CalendarOwner.Model.CalendarCells != null)
            {
                CalendarCellModel model = this.CalendarOwner.Model.CalendarCells.FirstOrDefault(cell => cell.Date == date);
                if (model != null)
                {
                    CalendarViewHostAutomationPeer hostPeer = (CalendarViewHostAutomationPeer)FrameworkElementAutomationPeer.FromElement(this.CalendarOwner.calendarViewHost);
                    peer = new CalendarSelectableCellnfoAutomationPeer(hostPeer, model);
                    this.childrenCache.Add(peer);
                }
            }          
            return peer;
        }
    }
}
