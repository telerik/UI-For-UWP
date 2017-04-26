using Telerik.UI.Xaml.Controls.Input.Calendar;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for CalendarNavigationControl.
    /// </summary>
    public class CalendarNavigationControlAutomationPeer : RadControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarNavigationControlAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">CalendarNavigationControl owner.</param>
        public CalendarNavigationControlAutomationPeer(CalendarNavigationControl owner) : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "calendar navigation control";
        }
    }
}
