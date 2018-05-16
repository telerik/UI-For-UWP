using System;
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

        internal static Slot GetSlotFromPoint(Point hitPoint, ElementCollection<CalendarTimeRulerItem> calendarTimeRulerItems, ElementCollection<CalendarCellModel> calendarCells, int visibleDays)
        {
            Slot slot = new Slot();
            foreach (CalendarTimeRulerItem item in calendarTimeRulerItems)
            {
                if (item.layoutSlot.Y <= hitPoint.Y && item.layoutSlot.Y + item.layoutSlot.Height >= hitPoint.Y)
                {
                    int cellsCount = calendarCells.Count;
                    for (int i = 0; i < cellsCount; i++)
                    {
                        CalendarCellModel cell = calendarCells[i];
                        if (cell.layoutSlot.X <= hitPoint.X && cell.layoutSlot.X + cell.layoutSlot.Width >= hitPoint.X
                            && i + visibleDays < cellsCount)
                        {
                            DateTime date = cell.Date.AddDays(visibleDays);
                            TimeSpan startTime = item.StartTime;
                            slot.Start = new DateTime(date.Year, date.Month, date.Day, startTime.Hours, startTime.Minutes, startTime.Seconds);

                            TimeSpan endTime = item.EndTime;
                            slot.End = new DateTime(date.Year, date.Month, date.Day, endTime.Hours, endTime.Minutes, endTime.Seconds);
                        }
                    }
                }
            }

            return slot;
        }
    }
}
