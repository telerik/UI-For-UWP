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

        private static bool IsDateBetweenRange(DateTime date, DateTime start, DateTime end)
        {
            if (date < start)
            {
                return false;
            }

            if (date > end)
            {
                return false;
            }

            return true;
        }

        internal TimeSlotTapContext GetSlotContextFromPoint(Point hitPoint, double offset)
        {
            var calendar = this.Owner;
            var model = calendar.Model;

            var calendarTimeRulerItems = model.multiDayViewModel.timeRulerItems;
            var cellModels = model.CalendarCells;

            var multiDayViewSettings = calendar.MultiDayViewSettings;
            var visibleDays = multiDayViewSettings.VisibleDays;

            var timeRulerItem = HitTestService.GetTimeRulerItemFromPoint(hitPoint, calendarTimeRulerItems);
            var calendarCellIndex = HitTestService.GetCellIndexFromPoint(hitPoint, cellModels);
            var realIndex = calendarCellIndex + visibleDays;
            var calendarCell = cellModels[realIndex];

            DateTime slotDate = calendarCell.Date;
            TimeSpan slotStartTime = timeRulerItem.StartTime;
            TimeSpan slotEndTime = timeRulerItem.EndTime;

            var startDate = slotDate.Add(slotStartTime);
            var endDate = slotDate.Add(slotEndTime);

            var exactStartDate = startDate;
            var exactEndDate = endDate;

            var specialSlots = multiDayViewSettings.SpecialSlotsSource;
            bool isReadOnly = false;
            if (specialSlots != null)
            {
                foreach (var specialSlot in specialSlots)
                {
                    var layoutSlot = specialSlot.layoutSlot;
                    if (layoutSlot.Y <= hitPoint.Y && layoutSlot.Y + layoutSlot.Height >= hitPoint.Y 
                        && layoutSlot.X <= (hitPoint.X + offset) && layoutSlot.X + layoutSlot.Width >= (hitPoint.X + offset))
                    {
                        isReadOnly = true;
                        break;
                    }
                    else
                    {
                        if (specialSlot.IsReadOnly)
                        {
                            if (IsDateBetweenRange(specialSlot.Start, startDate, endDate) && exactEndDate > specialSlot.Start)
                            {
                                exactEndDate = specialSlot.Start;
                            }

                            if (IsDateBetweenRange(specialSlot.End, startDate, endDate) && exactStartDate < specialSlot.End)
                            {
                                exactStartDate = specialSlot.End;
                            }
                        }
                    }
                }
            }

            return new TimeSlotTapContext(startDate, endDate, exactStartDate, exactEndDate, isReadOnly);
        }
    }
}
