using Telerik.UI.Xaml.Controls.Input.Calendar;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer clas for RadCalendar header cells.
    /// </summary>
    public class CalendarHeaderCellInfoAutomationPeer : CalendarCellInfoBaseAutomationPeer
    {
        private string automationId;
        internal CalendarHeaderCellModel HeaderCellModel { get; set; }

        /// <summary>
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarHeaderCellInfoAutomationPeer"/> class.
        /// </summary>
        /// <param name="parent">Parent CalendarViewHostAutomationPeer.</param>
        /// <param name="headerCellModel">The model of the calendar header cell.</param>
        internal CalendarHeaderCellInfoAutomationPeer(CalendarViewHostAutomationPeer parent, CalendarHeaderCellModel headerCellModel) : base(parent, headerCellModel)
        {
            this.HeaderCellModel = headerCellModel;
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Header;
        }

        /// <inheritdoc />
        protected override bool IsContentElementCore()
        {
            return false;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "calendar header cell";
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(CalendarHeaderCellModel);
        }

        /// <inheritdoc />
        protected override string GetAutomationIdCore()
        {
            if (this.automationId == null)
            {
                this.automationId = string.Format("CalendarHeaderCell_{0}_{1}", this.HeaderCellModel.Type, this.HeaderCellModel.Label);
            }

            return this.automationId;
        }
    }
}
