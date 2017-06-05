using System.Collections.Generic;
using System.Globalization;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation peer base class for RadCalendar cells.
    /// </summary>
    public class CalendarCellInfoBaseAutomationPeer : AutomationPeer, IGridItemProvider
    {
        private TextBlockAutomationPeer childTextBlockPeer;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarCellInfoBaseAutomationPeer"/> class.
        /// </summary>
        /// <param name="parentPeer">Parent CalendarViewHostAutomationPeer.</param>
        /// <param name="cellModel">The model of the calendar cell.</param>
        internal CalendarCellInfoBaseAutomationPeer(CalendarViewHostAutomationPeer parentPeer, CalendarNode cellModel) : base()
        {
            this.CalendarViewHostPeer = parentPeer;
            this.CellNode = cellModel;
        }

        #region IGridItemProvder
        /// <summary>
        /// Gets the column number in the grid of cells.
        /// </summary>
        public int Column
        {
            get
            {
                return this.CellNode.ColumnIndex;
            }
        }

        /// <summary>
        /// Gets the column span in the grid of cells.
        /// </summary>
        public int ColumnSpan
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the IRawElementProviderSimple - provider of the RadCalendar.
        /// </summary>
        public IRawElementProviderSimple ContainingGrid
        {
            get
            {
                if (this.CalendarViewHostPeer != null && this.CalendarViewHostPeer.CalendarOwner != null)
                {
                    return this.ProviderFromPeer(FrameworkElementAutomationPeer.CreatePeerForElement(this.CalendarViewHostPeer.CalendarOwner));
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the row number in the grid of cells.
        /// </summary>
        public int Row
        {
            get
            {
                return this.CellNode.RowIndex;
            }
        }

        /// <summary>
        /// Gets the row span in the grid of cells.
        /// </summary>
        public int RowSpan
        {
            get
            {
                return 1;
            }
        }

        internal CalendarViewHostAutomationPeer CalendarViewHostPeer { get; set; }
        internal CalendarNode CellNode { get; set; }

        /// <summary>
        /// Gets or sets TextBlock peer of the Cell.
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
        #endregion

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var headerCellModel = this.CellNode as CalendarHeaderCellModel;
            if (headerCellModel != null && headerCellModel.Type == CalendarHeaderCellType.DayName && !string.IsNullOrEmpty(this.CellNode.Label))
            {
                return this.CellNode.Label;
            }

            if (this.CellNode.Context != null && !string.IsNullOrEmpty(this.CellNode.Context.Date.ToString()))
            {
                return this.CellNode.Context.Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            return string.Empty;
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.DataItem;
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
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            if (this.ChildTextBlockPeer != null)
            {
                var children = new List<AutomationPeer>();
                children.Add(this.ChildTextBlockPeer);
                return children;
            }
            return null;
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
        protected override AutomationOrientation GetOrientationCore()
        {
            return AutomationOrientation.None;
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

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.GridItem)
            {
                return this;
            }

            return base.GetPatternCore(patternInterface);
        }

        /// <inheritdoc />
        protected override Rect GetBoundingRectangleCore()
        {
            if (IsWindowsPhone())
            {
                if (this.ChildTextBlockPeer != null)
                {
                    return this.ChildTextBlockPeer.GetBoundingRectangle();
                }

                return base.GetBoundingRectangleCore();
            }
            else
            {
                RadRect r = this.CellNode.layoutSlot;
                Point calendarTopLeft = this.CalendarViewHostPeer.OwnerCalendarHost.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0));

                return new Rect(new Point(calendarTopLeft.X + r.X, calendarTopLeft.Y + r.Y), new Size(r.Width, r.Height));
            }
        }

        private static bool IsWindowsPhone()
        {
            return ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons");
        }
    }
}
