using System;
using System.Globalization;
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

            cell.IsFromAnotherView = date.Month != this.Calendar.DisplayDate.Month;
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
            if (!this.Calendar.AreDayNamesVisible)
            {
                return;
            }

            double previousRight = viewRect.X;
            double cellWidth = viewRect.Width / this.ColumnCount;
            double cellHeight = viewRect.Y;
            int firstDayOfWeek = (int)this.Calendar.Culture.DateTimeFormat.FirstDayOfWeek;

            for (int columnIndex = 0; columnIndex < this.ColumnCount; columnIndex++)
            {
                CalendarHeaderCellModel calendarCell = this.calendarHeaderCells[itemIndex];
                calendarCell.Type = CalendarHeaderCellType.DayName;
                calendarCell.Label = this.GetFormattedDayName(columnIndex, firstDayOfWeek);

                double horizontalDifference = columnIndex * cellWidth - previousRight + viewRect.X;
                calendarCell.Arrange(new RadRect(previousRight, 0d, cellWidth + horizontalDifference, cellHeight));

                this.SnapToGridLines(calendarCell, -1, columnIndex);

                itemIndex++;
                previousRight = calendarCell.layoutSlot.Right;
            }
        }

        private string GetFormattedDayName(int columnIndex, int firstDayOfWeek)
        {
            int dayNameIndex = (firstDayOfWeek + columnIndex) % this.ColumnCount;

            if (this.Calendar.DayNameFormat == CalendarDayNameFormat.FullName)
            {
                return this.Calendar.Culture.DateTimeFormat.DayNames[dayNameIndex];
            }
            else if (this.Calendar.DayNameFormat == CalendarDayNameFormat.FirstLetter)
            {
                string dayName = this.Calendar.Culture.DateTimeFormat.DayNames[dayNameIndex];
                return dayName.Substring(0, 1);
            }
            else
            {
                return this.Calendar.Culture.DateTimeFormat.AbbreviatedDayNames[dayNameIndex];
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
            if (this.Calendar.AreDayNamesVisible)
            {
                dayNamesPanelHeight = this.Calendar.View.MeasureContent(CalendarHeaderCellType.DayName, this.Calendar.Culture.DateTimeFormat.AbbreviatedDayNames[0]).Height;
            }

            double weekNumbersPanelWidth = 0d;
            if (this.Calendar.AreWeekNumbersVisible)
            {
                string stringToMeasure = string.Format(this.Calendar.Culture, this.Calendar.WeekNumberFormat, WeekNumberMeasureString);
                weekNumbersPanelWidth = this.Calendar.View.MeasureContent(CalendarHeaderCellType.WeekNumber, stringToMeasure).Width;
            }

            double width = Math.Max(availableRect.Width - weekNumbersPanelWidth, 0d);
            double height = Math.Max(availableRect.Height - dayNamesPanelHeight, 0d);

            return new RadRect(weekNumbersPanelWidth, dayNamesPanelHeight, width, height);
        }

        private void EnsureCalendarHeaderCells()
        {
            if (this.calendarHeaderCells == null)
            {
                this.calendarHeaderCells = new ElementCollection<CalendarHeaderCellModel>(this);

                int itemCount = this.ColumnCount + this.RowCount;
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
    }
}
