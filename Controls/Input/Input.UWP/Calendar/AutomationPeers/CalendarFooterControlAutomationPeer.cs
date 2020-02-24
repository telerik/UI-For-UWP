using Telerik.UI.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Input.Calendar.AutomationPeers
{
    /// <summary>
    /// AutomationPeer class for CalendarFooterControl.
    /// </summary>
    public class CalendarFooterControlAutomationPeer : RadControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarFooterControlAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">CalendarFooterControl owner.</param>
        public CalendarFooterControlAutomationPeer(CalendarFooterControl owner) : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "calendar footer control";
        }
    }
}
