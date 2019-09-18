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

        public static CalendarTimeRulerItem GetTimeRulerItemFromPoint(Point hitPoint, ElementCollection<CalendarTimeRulerItem> calendarTimeRulerItems, double halfTextHeight, double lineThickness)
        {
            foreach (CalendarTimeRulerItem item in calendarTimeRulerItems)
            {
                var layoutSlot = item.layoutSlot;
                var layoutY = layoutSlot.Y + halfTextHeight - lineThickness / 2;
                if (layoutY <= hitPoint.Y && layoutY + layoutSlot.Height >= hitPoint.Y)
                {
                    return item;
                }
            }

            return null;
        }

        internal TimeSlotTapContext GetSlotContextFromPoint(Point hitPoint)
        {
            var calendar = this.Owner;
            var model = calendar.Model;

            var calendarTimeRulerItems = model.multiDayViewModel.timeRulerItems;
            var cellModels = model.CalendarCells;

            var multiDayViewSettings = calendar.MultiDayViewSettings;
            var visibleDays = multiDayViewSettings.VisibleDays;

            var halfTextHeight = model.multiDayViewModel.halfTextHeight;
            var thickness = calendar.GridLinesThickness;
            var timeRulerItem = HitTestService.GetTimeRulerItemFromPoint(hitPoint, calendarTimeRulerItems, halfTextHeight, thickness);
            if (timeRulerItem == null)
            {
                return null;
            }

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

            var exactTime = GetExactTimeFromPoint(hitPoint, timeRulerItem, halfTextHeight, thickness);
            var exactDate = calendarCell.Date.AddMilliseconds(exactTime.TotalMilliseconds);

            var specialSlots = multiDayViewSettings.SpecialSlotsSource;
            bool isReadOnly = false;
            if (specialSlots != null)
            {
                foreach (var specialSlot in specialSlots)
                {
                    if (exactDate >= specialSlot.Start && exactDate <= specialSlot.End)
                    {
                        if (specialSlot.IsReadOnly)
                        {
                            isReadOnly = true;
                            break;
                        }
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

        private static TimeSpan GetExactTimeFromPoint(Point hitPoint, CalendarTimeRulerItem item, double halfTextHeight, double lineThickness)
        {
            var layoutSlot = item.layoutSlot;
            var layoutSlotY = layoutSlot.Y + halfTextHeight - lineThickness / 2;

            var yDiff = hitPoint.Y - layoutSlotY;
            var coeff = yDiff / layoutSlot.Height;
            TimeSpan totalSlotTimeLenght = item.EndTime - item.StartTime;
            var exactTimeMilliseconds = totalSlotTimeLenght.TotalMilliseconds * coeff;
            var exactTime = TimeSpan.FromMilliseconds(exactTimeMilliseconds);

            return item.StartTime.Add(exactTime);
        }
    }
}
