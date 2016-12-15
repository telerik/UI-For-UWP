using Telerik.Core;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal class HitTestService : ServiceBase<RadCalendar>
    {
        internal HitTestService(RadCalendar owner)
            : base(owner)
        {
        }

        public static CalendarCellModel GetCellFromPoint(Point point, ElementCollection<CalendarCellModel> cellModels)
        {
            foreach (CalendarCellModel cell in cellModels)
            {
                if (cell.layoutSlot.Contains(point.X, point.Y))
                {
                    return cell;
                }
            }

            return null;
        }
    }
}
