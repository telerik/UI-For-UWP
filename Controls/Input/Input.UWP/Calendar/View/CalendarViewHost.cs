using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// Represents the topmost panel that holds the calendar view cells.
    /// </summary>
    public class CalendarViewHost : Canvas
    {
        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            RadCalendar calendar = ElementTreeHelper.FindVisualAncestor<RadCalendar>(this);
            if (calendar != null)
            {
                return new CalendarViewHostAutomationPeer(this, calendar);
            }
            return null;
        }
    }
}
