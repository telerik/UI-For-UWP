using Telerik.UI.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Input.Calendar.AutomationPeers
{
    public class CalendarFooterControlAutomationPeer : RadControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarNavigationControlAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">CalendarNavigationControl owner.</param>
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
