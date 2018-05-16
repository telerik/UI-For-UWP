using System;
using System.Globalization;
using System.Linq;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal class CalendarMonthViewModel : CalendarViewModel
    {
        // TODO: Non-gregorian calendar infrastructure support
        internal const int DefaultRowCount = 6;
        internal const int DefaultColumnCount = 7;

        private const int WeekNumberMeasureString = 52;

        private ElementCollection<CalendarHeaderCellModel> calendarHeaderCells;

        public override int RowCount
        {
            get
            {
                return DefaultRowCount;
            }
        }

        public override int ColumnCount
        {
            get
            {
                return DefaultColumnCount;
            }
        }

        public ElementCollection<CalendarHeaderCellModel> CalendarHeaderCells
        {
            get
            {
                return this.calendarHeaderCells;
            }
        }

        internal override DateTime GetFirstDateToRender(DateTime date)
        {
            DayOfWeek firstDayOfWeekToUse = this.Calendar.GetFirstDayOfWeek();

            DateTime monthStartDate = CalendarMathHelper.GetFirstDateOfMonth(date);

            int daysToSubtract = (int)monthStartDate.DayOfWeek - (int)firstDayOfWeekToUse;
            if (daysToSubtract <= 0)
            {
                daysToSubtract += 7;
            }

            return monthStartDate.Date == DateTime.MinValue.Date ? monthStartDate : monthStartDate.AddDays(-daysToSubtract);
        }

        internal override DateTime GetNextDateToRender(DateTime dateToRender)
        {
            return dateToRender.Date == DateTime.MaxValue.Date ? dateToRender : dateToRender.AddDays(1);
        }

        internal override void PrepareCalendarCell(CalendarCellModel cell, DateTime date)
        {
            cell.Date = date;
            cell.Label = string.Format(this.Calendar.Culture, this.Calendar.MonthViewCellFormat, date);

            if (this.Calendar.DisplayMode != CalendarDisplayMode.MultiDayView)
            {
                cell.IsFromAnotherView = date.Month != this.Calendar.DisplayDate.Month;
            }
        }

        internal void EnsureCalendarHeaderCells()
        {
            if (this.calendarHeaderCells == null)
            {
                this.calendarHeaderCells = new ElementCollection<CalendarHeaderCellModel>(this);
            }

            if (this.calendarHeaderCells.Count == 0)
            {
                int itemCount = this.ColumnCount + this.RowCount + (this.BufferItemsCount * 2);
                for (int cellIndex = 0; cellIndex < itemCount; cellIndex++)
                {
                    CalendarHeaderCellModel cell = new CalendarHeaderCellModel();

                    this.calendarHeaderCells.Add(cell);
                }
            }
            else
            {
                foreach (CalendarHeaderCellModel cell in this.calendarHeaderCells)
                {
                    cell.layoutSlot = RadRect.Empty;
                }
            }
        }

        internal void ArrangeCalendarColumnHeaders(RadRect viewRect)
        {
            int itemIndex = 0;
            this.ArrangeCalendarColumnHeaders(viewRect, ref itemIndex);
        }

        protected override RadRect UpdateAnimatableContentClip(RadRect rect)
        {
            RadRect clipRect = this.GetCalendarViewRect(rect);
            this.Calendar.AnimatableContentClip = clipRect;

            return clipRect;
        }

        protected override void ArrangeCalendarHeaders(RadRect viewRect)
        {
            this.EnsureCalendarHeaderCells();

            int itemIndex = 0;

            this.ArrangeCalendarColumnHeaders(viewRect, ref itemIndex);
            this.ArrangeCalendarRowHeaders(viewRect, itemIndex);
        }

        private void ArrangeCalendarColumnHeaders(RadRect viewRect, ref int itemIndex)
        {
            CalendarModel model = this.Calendar;
            if (!model.AreDayNamesVisible)
            {
                return;
            }

            double previousRight = viewRect.X;
            double cellWidth = viewRect.Width / (this.ColumnCount + (this.BufferItemsCount * 2));
            double cellHeight = viewRect.Y;
            int firstDayOfWeek = (int)model.Culture.DateTimeFormat.FirstDayOfWeek;

            for (int columnIndex = 0; columnIndex < this.ColumnCount + (this.BufferItemsCount * 2); columnIndex++)
            {
                CalendarHeaderCellModel calendarCell = this.calendarHeaderCells[itemIndex];
                calendarCell.Type = CalendarHeaderCellType.DayName;

                string label = string.Empty;
                if (model.DisplayMode != CalendarDisplayMode.MultiDayView)
                {
                    int dayNameIndex = (firstDayOfWeek + columnIndex) % this.ColumnCount;
                    label = this.GetFormattedDayName(dayNameIndex);
                }
                else
                {
                    CalendarCellModel cellOfHeader = this.CalendarCells.FirstOrDefault(a => a.ColumnIndex == columnIndex);
                    if (cellOfHeader != null)
                    {
                        label = this.GetFormattedDayName((int)cellOfHeader.Date.DayOfWeek);
                    }
                }

                calendarCell.Label = label;

                double horizontalDifference = columnIndex * cellWidth - previousRight + viewRect.X;
                calendarCell.Arrange(new RadRect(previousRight, 0d, cellWidth + horizontalDifference, cellHeight));

                this.SnapToGridLines(calendarCell, -1, columnIndex);

                itemIndex++;
                previousRight = calendarCell.layoutSlot.Right;
            }
        }

        private string GetFormattedDayName(int dayNameIndex)
        {
            CalendarModel model = this.Calendar;
            if (model.DayNameFormat == CalendarDayNameFormat.FullName)
            {
                return model.Culture.DateTimeFormat.DayNames[dayNameIndex];
            }
            else if (model.DayNameFormat == CalendarDayNameFormat.FirstLetter)
            {
                string dayName = model.Culture.DateTimeFormat.DayNames[dayNameIndex];
                return dayName.Substring(0, 1);
            }
            else
            {
                return model.Culture.DateTimeFormat.AbbreviatedDayNames[dayNameIndex];
            }
        }

        private void ArrangeCalendarRowHeaders(RadRect viewRect, int itemIndex)
        {
            if (!this.Calendar.AreWeekNumbersVisible)
            {
                return;
            }

            double previousBottom = viewRect.Y;
            double cellWidth = viewRect.X;
            double cellHeight = viewRect.Height / this.RowCount;
            DayOfWeek firstDayOfWeekToUse = this.Calendar.GetFirstDayOfWeek();
            CalendarWeekRule weekRuleToUse = this.Calendar.GetCalendarWeekRule();

            DateTime weekNumberDate = new DateTime(this.Calendar.DisplayDate.Year, this.Calendar.DisplayDate.Month, 1);
            int daysDifference = firstDayOfWeekToUse - weekNumberDate.DayOfWeek;
            if (daysDifference == 0)
            {
                weekNumberDate = weekNumberDate.AddDays(-7);
            }

            if (daysDifference <= 0)
            {
                daysDifference += 7;
            }
            
            int weekNumber;
            for (int rowIndex = 0; rowIndex < this.RowCount; rowIndex++)
            {
                CalendarHeaderCellModel calendarCell = this.calendarHeaderCells[itemIndex];
                calendarCell.Type = CalendarHeaderCellType.WeekNumber;

                weekNumber = this.Calendar.Culture.Calendar.GetWeekOfYear(weekNumberDate, weekRuleToUse, firstDayOfWeekToUse);

                if (weekNumberDate.AddDays(daysDifference - 1).Year > weekNumberDate.Year && weekRuleToUse == CalendarWeekRule.FirstDay)
                {
                    weekNumber = 1;
                }

                calendarCell.Label = string.Format(this.Calendar.Culture, this.Calendar.WeekNumberFormat, weekNumber);
                
                double verticalDifference = rowIndex * cellHeight - previousBottom + viewRect.Y;
                calendarCell.Arrange(new RadRect(0d, previousBottom, cellWidth, cellHeight + verticalDifference));

                this.SnapToGridLines(calendarCell, rowIndex, -1);

                itemIndex++;
                weekNumberDate = weekNumberDate.AddDays(7);
                previousBottom = calendarCell.layoutSlot.Bottom;
            }
        }

        private RadRect GetCalendarViewRect(RadRect availableRect)
        {
            double dayNamesPanelHeight = 0d;
            CalendarModel calendar = this.Calendar;
            if (calendar.AreDayNamesVisible)
            {
                dayNamesPanelHeight = calendar.View.MeasureContent(CalendarHeaderCellType.DayName, calendar.Culture.DateTimeFormat.AbbreviatedDayNames[0]).Height;
            }

            double panelWidth = 0d;
            if (calendar.DisplayMode == CalendarDisplayMode.MonthView && calendar.AreWeekNumbersVisible)
            {
                string stringToMeasure = string.Format(calendar.Culture, calendar.WeekNumberFormat, WeekNumberMeasureString);
                panelWidth = calendar.View.MeasureContent(CalendarHeaderCellType.WeekNumber, stringToMeasure).Width;
            }
            else if (calendar.DisplayMode == CalendarDisplayMode.MultiDayView)
            {
                MultiDayViewSettings settings = this.Calendar.multiDayViewSettings;
                TimeSpan endTime = settings.DayEndTime;
                string stringToMeasure = string.Format(this.Calendar.Culture, this.Calendar.TimeFormat, calendar.DisplayDate.Date.Add(endTime));
                panelWidth = calendar.View.MeasureContent(null, stringToMeasure).Width;

                if (!string.IsNullOrEmpty(settings.AllDayAreaText))
                {
                    stringToMeasure = MultiDayViewSettings.DefaultAllDayText;
                    double allDayWidth = calendar.View.MeasureContent(null, stringToMeasure).Width;
                    if (allDayWidth > panelWidth)
                    {
                        panelWidth = allDayWidth;
                    }
                }
            }

            double width = Math.Max(availableRect.Width - panelWidth, 0d);
            double height = Math.Max(availableRect.Height - dayNamesPanelHeight, 0d);

            return new RadRect(panelWidth, dayNamesPanelHeight, width, height);
        }
    }
}
