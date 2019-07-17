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

        public static int GetCellIndexFromPoint(Point point, ElementCollection<CalendarCellModel> cellModels)
        {
            for (int i = 0; i < cellModels.Count; i++)
            {
                var cell = cellModels[i];
                if (cell.layoutSlot.X <= point.X && cell.layoutSlot.X + cell.layoutSlot.Width >= point.X)
                {
                    return i;
                }
            }

            return -1;
        }

        public static CalendarTimeRulerItem GetTimeRulerItemFromPoint(Point hitPoint, ElementCollection<CalendarTimeRulerItem> calendarTimeRulerItems)
        {
            foreach (CalendarTimeRulerItem item in calendarTimeRulerItems)
            {
                var layoutSlot = item.layoutSlot;
                if (layoutSlot.Y <= hitPoint.Y && layoutSlot.Y + layoutSlot.Height >= hitPoint.Y)
                {
                    return item;
                }
            }

            return null;
        }

        internal Slot GetSlotFromPoint(Point hitPoint)
        {
            var calendar = this.Owner;
            var model = calendar.Model;

            var calendarTimeRulerItems = model.multiDayViewModel.timeRulerItems;
            var cellModels = model.CalendarCells;

            var multiDayViewSettings = calendar.MultiDayViewSettings;
            var visibleDays = multiDayViewSettings.VisibleDays;

            var timeRulerItem = HitTestService.GetTimeRulerItemFromPoint(hitPoint, calendarTimeRulerItems);
            var calendarCellIndex = HitTestService.GetCellIndexFromPoint(hitPoint, cellModels);
            var calendarCell = cellModels[calendarCellIndex + visibleDays];

            DateTime slotDate = calendarCell.Date;
            TimeSpan slotStartTime = timeRulerItem.StartTime;
            TimeSpan slotEndTime = timeRulerItem.EndTime;

            Slot slot = new Slot(slotDate.Add(slotStartTime), slotDate.Add(slotEndTime));
            slot.IsReadOnly = this.IsSlotReadOnly(slot);

            return slot;
        }

        private bool IsSlotReadOnly(Slot slot)
        {
            var multiDayViewSettings = this.Owner.MultiDayViewSettings;
            var specialSlots = multiDayViewSettings.SpecialSlotsSource;
            bool isReadOnly = false;
            if (specialSlots != null)
            {
                foreach (var specialSlot in specialSlots)
                {
                    if (specialSlot.IntersectsWith(slot))
                    {
                        if (specialSlot.IsReadOnly)
                        {
                            return true;
                        }
                    }
                }
            }

            return isReadOnly;
        }
    }
}
