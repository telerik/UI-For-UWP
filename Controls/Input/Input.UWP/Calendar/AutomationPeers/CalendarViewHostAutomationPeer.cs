using System.Collections.Generic;
using System.Linq;
using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for CalendarViewHost.
    /// </summary>
    public class CalendarViewHostAutomationPeer : FrameworkElementAutomationPeer
    {
        internal RadCalendar CalendarOwner { get; set; }
        internal CalendarViewHost OwnerCalendarHost
        {
            get
            {
                return (CalendarViewHost)this.Owner;
            }            
        }

        /// <summary>
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarViewHostAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">CalendarViewHost owner.</param>
        public CalendarViewHostAutomationPeer(CalendarViewHost owner) : base(owner)
        {
        }

        /// <summary>
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarViewHostAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">CalendarViewHost owner.</param>
        /// <param name="calendarOwner">RadCalendar owner.</param>
        public CalendarViewHostAutomationPeer(CalendarViewHost owner, RadCalendar calendarOwner) : this(owner)
        {
            this.CalendarOwner = calendarOwner;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "calendar view host panel";
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
                return nameCore;

            return nameof(CalendarViewHost);
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            // List because it plays a role of list container of cell peers.
            return AutomationControlType.List;
        }

        /// <inheritdoc />
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            List<AutomationPeer> children = new List<AutomationPeer>();
            RadCalendarAutomationPeer calendarPeer = FrameworkElementAutomationPeer.FromElement(this.CalendarOwner) as RadCalendarAutomationPeer;

            IList<AutomationPeer> textBlockPeers = this.GetTextBlockPeers();

            foreach (CalendarCellModel cellModel in this.CalendarOwner.Model.currentViewModel.CalendarCells)
            {
                CalendarCellInfoBaseAutomationPeer peer = calendarPeer.childrenCache.OfType<CalendarCellInfoBaseAutomationPeer>()
                                .Where(a => this.AreModelsEqual(a.CellNode, cellModel)).FirstOrDefault();

                if (peer == null)
                {
                    peer = this.GetPeerByCurrentView(cellModel);
                    calendarPeer.childrenCache.Add(peer);
                }

                TextBlock matchingTextBlock = this.CalendarOwner.contentLayer.realizedCalendarCellDefaultPresenters[cellModel];
                if (matchingTextBlock != null)
                {
                    peer.ChildTextBlockPeer = FrameworkElementAutomationPeer.FromElement(matchingTextBlock) as TextBlockAutomationPeer;
                }

                peer.GetChildren();
                children.Add(peer);
            }

            CalendarMonthViewModel monthModel = this.CalendarOwner.Model.currentViewModel as CalendarMonthViewModel;
            if (monthModel != null)
            {
                foreach (CalendarHeaderCellModel headerModel in monthModel.CalendarHeaderCells)
                {
                    CalendarHeaderCellInfoAutomationPeer peer = calendarPeer.childrenCache.OfType<CalendarHeaderCellInfoAutomationPeer>()
                        .Where(a => this.AreHeaderModelsEqual(a.HeaderCellModel, headerModel)).FirstOrDefault();

                    if (peer == null)
                    {
                        peer = new CalendarHeaderCellInfoAutomationPeer(this, headerModel);
                        calendarPeer.childrenCache.Add(peer);
                    }

                    children.Add(peer);
                }
            }

            return children;
        }

        private CalendarCellInfoBaseAutomationPeer GetPeerByCurrentView(CalendarCellModel cellModel)
        {
            if (this.CalendarOwner.DisplayMode == CalendarDisplayMode.MonthView)
            {
                return new CalendarSelectableCellnfoAutomationPeer(this, cellModel);
            }
            else
            {
                return new CalendarInvokableCellInfoAutomationPeer(this, cellModel);
            }
        }

        private IList<AutomationPeer> GetTextBlockPeers()
        {
            return this.CalendarOwner.contentLayer.realizedCalendarCellDefaultPresenters.Values
                .Where(e => e.Visibility == Visibility.Visible)
                       .Select(e => FrameworkElementAutomationPeer.CreatePeerForElement(e))
                       .Where(ap => ap != null)
                       .ToList();
        }

        private bool AreModelsEqual(CalendarNode a, CalendarCellModel b)
        {
            return a.RowIndex == b.RowIndex &&
                   a.ColumnIndex == b.ColumnIndex &&
                   a.Label == b.Label;
        }

        private bool AreHeaderModelsEqual(CalendarHeaderCellModel a, CalendarHeaderCellModel b)
        {
            return a.Type == b.Type && a.Label == b.Label;
        }
    }
}
