using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal static class NodeExtensions
    {
        public static CalendarModel GetCalendar(this Node node)
        {
            return node.root as CalendarModel;
        }
    }
}
