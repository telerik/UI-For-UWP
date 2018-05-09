using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal class CalendarMultiDayViewModel : CalendarMonthViewModel
    {
        internal ElementCollection<CalendarTimeRulerItem> timeRulerItems;
        internal ElementCollection<CalendarGridLine> timerRulerLines;
        internal List<CalendarAppointmentInfo> appointmentInfos;
        internal List<CalendarAppointmentInfo> allDayAppointmentInfos;

        internal CalendarGridLine horizontalLowerAllDayAreaRulerGridLine;
        internal CalendarGridLine horizontalUpperAllDayAreaRulerGridLine;
        internal CalendarGridLine horizontalTopHeaderRulerGridLine;
        internal CalendarGridLine verticalRulerGridLine;
        internal CalendarGridLine currentTimeIndicator;
        internal Slot todaySlot;
        internal RadRect allDayLabelLayout;

        internal RadRect dayViewLayoutSlot;
        internal double totalAllDayAreaHeight;
        internal double timeRulerWidth;
        internal int allDayAreaRowCount;
        internal MultiDayViewUpdateFlag updateFlag;

        private const int DefaultSpecificColumnCount = 6;
        private static double DefaultCurrentTimeIndicatorHeight = 2d;

        private double halfTextHeight;

        public override int RowCount
        {
            get
            {
                return 1;
            }
        }

        public override int ColumnCount
        {
            get
            {
                return this.Calendar.multiDayViewSettings.VisibleDays;
            }
        }

        public override int SpecificColumnCount
        {
            get
            {
                return CalendarMultiDayViewModel.DefaultSpecificColumnCount;
            }
        }

        public override int BufferItemsCount
        {
            get
            {
                return this.Calendar.multiDayViewSettings.VisibleDays;
            }
        }

        internal override DateTime GetFirstDateToRender(DateTime date)
        {
            DayOfWeek firstDayOfWeek = this.Calendar.GetFirstDayOfWeek();
            DateTime firstDateOfCurrentWeek = CalendarMathHelper.GetFirstDayOfCurrentWeek(date, firstDayOfWeek);

            if (firstDateOfCurrentWeek.Date <= date.Date && firstDateOfCurrentWeek.AddDays(7).Date >= date.Date)
            {
                return date;
            }

            return firstDateOfCurrentWeek;
        }

        internal override DateTime GetNextDateToRender(DateTime date)
        {
            return date.Date == DateTime.MaxValue.Date ? date : date.AddDays(1);
        }

        internal override RadRect ArrangeOverride(RadRect rect)
        {
            this.layoutSlot = rect;
            this.dayViewLayoutSlot = this.UpdateAnimatableContentClip(rect);
            double cellContentWidth = this.dayViewLayoutSlot.Width / this.ColumnCount * this.BufferItemsCount * 2;
            this.timeRulerWidth = this.dayViewLayoutSlot.X;

            this.dayViewLayoutSlot.Width += cellContentWidth;
            this.Calendar.AnimatableContentClip = this.dayViewLayoutSlot;

            if (this.updateFlag == MultiDayViewUpdateFlag.All)
            {
                this.ArrangeCalendarDecorations(this.dayViewLayoutSlot);
                this.ArrangeCalendarContent(this.dayViewLayoutSlot);
                this.ArrangeCalendarHeaders(this.dayViewLayoutSlot);
            }

            rect.Width += cellContentWidth;
            this.ArrangeCalendarTimerRuler(rect);

            if (this.updateFlag != MultiDayViewUpdateFlag.AffectsCurrentTimeIndicator)
            {
                this.ArrangeMultiDayViewCalendarDecorations(this.dayViewLayoutSlot);
            }

            return rect;
        }

        internal void ArrangeAppointments()
        {
            AppointmentSource appointmentSource = this.Calendar.appointmentSource;
            if (this.appointmentInfos == null)
            {
                this.appointmentInfos = new List<CalendarAppointmentInfo>();
            }
            else
            {
                this.appointmentInfos.Clear();
            }

            if (appointmentSource != null)
            {
                MultiDayViewSettings settings = this.Calendar.multiDayViewSettings;
                foreach (var calendarCell in this.CalendarCells)
                {
                    LinkedList<IAppointment> appointmentsPerCell = appointmentSource.GetAppointments((IAppointment appointment) =>
                    {
                        return calendarCell.Date.Date >= appointment.StartDate.Date
                        && calendarCell.Date.Date <= appointment.EndDate.Date && !(appointment.IsAllDay && settings.ShowAllDayArea);
                    });

                    var orderedAppointments = new LinkedList<IAppointment>(appointmentsPerCell.OrderBy(a => a.StartDate));
                    foreach (var appointment in orderedAppointments)
                    {
                        if ((appointment.StartDate.Date == calendarCell.Date.Date && appointment.StartDate.TimeOfDay > settings.DayEndTime)
                            || (appointment.EndDate.Date == calendarCell.Date.Date && appointment.EndDate.TimeOfDay < settings.DayStartTime))
                        {
                            continue;
                        }

                        double startY = this.DateToVerticalPosition(calendarCell.Date.Date, appointment.StartDate);
                        DateTime endDate = appointment.EndDate.TimeOfDay > settings.DayEndTime
                            ? appointment.EndDate.Add(TimeSpan.FromTicks(settings.DayEndTime.Ticks - appointment.EndDate.TimeOfDay.Ticks))
                            : appointment.EndDate;

                        double endY = this.DateToVerticalPosition(calendarCell.Date.Date, endDate);
                        if (startY < endY)
                        {
                            CalendarAppointmentInfo info = new CalendarAppointmentInfo();
                            info.Date = calendarCell.Date;
                            info.Appointments = orderedAppointments;
                            info.columnIndex = calendarCell.ColumnIndex;
                            info.Brush = appointment.Color;
                            info.cell = calendarCell;
                            info.childAppointment = appointment;
                            info.DetailText = appointment.Description;
                            info.Subject = appointment.Subject;
                            info.IsAllDay = appointment.IsAllDay;
                            info.hasPrevDay = appointment.StartDate.Day < calendarCell.Date.Day;
                            info.hasNextDay = appointment.EndDate.Day > calendarCell.Date.Day;

                            int xCoeff = (calendarCell.Date - appointment.StartDate.Date).Days;
                            RadRect layoutSlot = new RadRect(calendarCell.layoutSlot.X - this.timeRulerWidth, startY, calendarCell.layoutSlot.Width, endY - startY);
                            info.layoutSlot = layoutSlot;
                            this.appointmentInfos.Add(info);
                        }
                    }
                }

                this.ArrangeIntersectedAppointments();
            }
        }

        protected override void ArrangeCalendarHeaders(RadRect viewRect)
        {
            this.EnsureCalendarHeaderCells();
            this.ArrangeCalendarColumnHeaders(viewRect);
        }

        private void ArrangeIntersectedAppointments()
        {
            double gridLineThickness = this.Calendar.GridLinesThickness > 0 ? this.Calendar.GridLinesThickness : 1;
            foreach (var appointmentInfo in this.appointmentInfos)
            {
                if (!appointmentInfo.isArranged)
                {
                    List<CalendarAppointmentInfo> intersectedAppointments = this.appointmentInfos.Where(a => a.layoutSlot.IntersectsWith(appointmentInfo.layoutSlot) && a.columnIndex == appointmentInfo.columnIndex).ToList();
                    int intersectedAppointmentsCount = intersectedAppointments.Count;
                    if (intersectedAppointmentsCount > 1)
                    {
                        this.SetColumnsOfIntersectedAppointments(intersectedAppointments, appointmentInfo, 0);
                        int columnsCount = intersectedAppointments.Max(a => a.arrangeColumnIndex.Value) + 1;
                        var orderedIntersectedAppointments = intersectedAppointments.OrderBy(a => a.arrangeColumnIndex).ToList();
                        double maxWidth = appointmentInfo.layoutSlot.Width / columnsCount;
                        for (int i = 0; i < intersectedAppointmentsCount; i++)
                        {
                            CalendarAppointmentInfo intersectedAppointment = orderedIntersectedAppointments[i];
                            if (!intersectedAppointment.isArranged)
                            {
                                RadRect layout = intersectedAppointment.layoutSlot;
                                layout.X = layout.X + (maxWidth * intersectedAppointment.arrangeColumnIndex.Value);
                                layout.Width = maxWidth;

                                intersectedAppointment.layoutSlot = layout;
                                intersectedAppointment.isArranged = true;
                            }
                            else
                            {
                                RadRect layout = intersectedAppointment.layoutSlot;
                                if (layout.Width > maxWidth)
                                {
                                    this.OffsetChildIntersectedAppointments(intersectedAppointment, maxWidth);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OffsetChildIntersectedAppointments(CalendarAppointmentInfo intersectedAppointment, double maxWidth)
        {
            var childIntersectedAppointments = this.appointmentInfos.Where(a => a.layoutSlot.IntersectsWith(intersectedAppointment.layoutSlot)
            && a.columnIndex == intersectedAppointment.columnIndex).ToList();

            RadRect layout = intersectedAppointment.layoutSlot;
           
            double offset = (layout.Width - maxWidth) * intersectedAppointment.arrangeColumnIndex.Value;
            layout.X -= offset;
            layout.Width = maxWidth;
            intersectedAppointment.layoutSlot = layout;

            foreach (CalendarAppointmentInfo app in childIntersectedAppointments)
            {
                if (app.isArranged && app.layoutSlot.Width > maxWidth)
                {
                    this.OffsetChildIntersectedAppointments(app, maxWidth);
                }
            }
        }

        private void SetColumnsOfIntersectedAppointments(IEnumerable<CalendarAppointmentInfo> intersectedAppointments, CalendarAppointmentInfo parentAppointmentInfo, int columnIndex)
        {
            for (int i = 0; i < intersectedAppointments.Count(); i++)
            {
                CalendarAppointmentInfo app = intersectedAppointments.ElementAt(i);
                if (app == parentAppointmentInfo)
                {
                    int index = columnIndex;
                    while (intersectedAppointments.Any(a => a.arrangeColumnIndex == index))
                    {
                        index++;
                    }

                    app.arrangeColumnIndex = index;
                    continue;
                }

                if (app.arrangeColumnIndex == null)
                {
                    IEnumerable<CalendarAppointmentInfo> childIntersectedAppointments = intersectedAppointments
                        .Where(a => a.layoutSlot.IntersectsWith(app.layoutSlot) && a.columnIndex == app.columnIndex);

                    this.SetColumnsOfIntersectedAppointments(childIntersectedAppointments, app, columnIndex + 1);
                }
            }
        }

        private void ArrangeCalendarTimerRuler(RadRect viewRect)
        {
            switch (this.updateFlag)
            {
                case MultiDayViewUpdateFlag.All:
                case MultiDayViewUpdateFlag.AffectsTimeRuler:
                    this.EnsureTimeRulerItems();
                    this.EnsureTimeRulerLines();

                    this.ArrangeTimerRuler(viewRect);
                    this.ArrangeTimerRulerLines(viewRect);

                    this.EnsureSlots();
                    this.EnsureTodaySlot();
                    this.ArrangeAppointments(viewRect);
                    this.ArrangeCurrentTimeIndicator(viewRect);
                    this.ArrangeAllDayAreaText(this.layoutSlot);
                    break;
                case MultiDayViewUpdateFlag.AffectsAppointments:
                    this.ArrangeAppointments(viewRect);
                    this.ArrangeAllDayAreaText(this.layoutSlot);
                    break;
                case MultiDayViewUpdateFlag.AffectsCurrentTimeIndicator:
                    if (this.Calendar.multiDayViewSettings.ShowCurrentTimeIndicator)
                    {
                        this.ArrangeCurrentTimeIndicator(viewRect);
                    }
                    break;
                case MultiDayViewUpdateFlag.AffectsSpecialSlots:
                    this.EnsureSlots();
                    break;
                default:
                    break;
            }
        }

        private void EnsureSlots()
        {
            MultiDayViewSettings settings = this.Calendar.multiDayViewSettings;
            IEnumerable<Slot> slots = settings.SpecialSlotsSource;
            if (slots != null && slots.Count() > 0)
            {
                foreach (Slot slot in slots)
                {
                    slot.layoutSlot = RadRect.Empty;
                }

                foreach (var calendarCell in this.CalendarCells)
                {
                    var slotsPerCell = slots.Where(a => calendarCell.Date.Date >= a.Start.Date && calendarCell.Date.Date <= a.End.Date);
                    foreach (var slot in slotsPerCell)
                    {
                        if ((slot.Start.Date == calendarCell.Date.Date && slot.Start.TimeOfDay > settings.DayEndTime)
                            || (slot.End.Date == calendarCell.Date.Date && slot.End.TimeOfDay < settings.DayStartTime))
                        {
                            continue;
                        }

                        double startY = this.DateToVerticalPosition(calendarCell.Date.Date, slot.Start);
                        DateTime endDate = slot.End.TimeOfDay > settings.DayEndTime
                            ? endDate = slot.End.Add(TimeSpan.FromTicks(settings.DayEndTime.Ticks - slot.End.TimeOfDay.Ticks))
                            : endDate = slot.End;

                        double endY = this.DateToVerticalPosition(calendarCell.Date.Date, endDate);
                        if (startY < endY)
                        {
                            RadRect layoutSlot = new RadRect(calendarCell.layoutSlot.X - this.timeRulerWidth, startY, calendarCell.layoutSlot.Width, endY - startY);
                            slot.layoutSlot = layoutSlot;
                        }
                    }
                }
            }
        }

        private void EnsureTodaySlot()
        {
            MultiDayViewSettings settings = this.Calendar.multiDayViewSettings;
            DateTime today = DateTime.Now.Date;
            if (this.todaySlot == null)
            {
                this.todaySlot = new Slot();
                this.todaySlot.Start = today.AddTicks(settings.DayStartTime.Ticks);
                this.todaySlot.End = today.AddTicks(settings.DayEndTime.Ticks);
            }

            double startY = this.DateToVerticalPosition(today, this.todaySlot.Start);
            DateTime endDate = this.todaySlot.End.TimeOfDay > settings.DayEndTime
                ? endDate = this.todaySlot.End.Add(TimeSpan.FromTicks(settings.DayEndTime.Ticks - this.todaySlot.End.TimeOfDay.Ticks))
                : endDate = this.todaySlot.End;

            double endY = this.DateToVerticalPosition(today, endDate);
            CalendarCellModel todayCell = this.CalendarCells.FirstOrDefault(a => a.Date == today);
            if (todayCell != null && startY < endY)
            {
                RadRect layoutSlot = new RadRect(todayCell.layoutSlot.X - this.timeRulerWidth, startY, todayCell.layoutSlot.Width, endY - startY);
                this.todaySlot.layoutSlot = layoutSlot;
            }
            else if (todayCell == null)
            {
                this.todaySlot.layoutSlot = RadRect.Empty;
            }
        }

        private void ArrangeAllDayAreaText(RadRect viewRect)
        {
            if (this.Calendar.multiDayViewSettings.ShowAllDayArea)
            {
                double cellHeight = this.dayViewLayoutSlot.Height / this.SpecificColumnCount;
                double gridLineThickness = this.Calendar.GridLinesThickness;

                this.allDayLabelLayout = new RadRect(viewRect.X, this.dayViewLayoutSlot.Y + cellHeight, this.dayViewLayoutSlot.X - viewRect.X, this.totalAllDayAreaHeight);
            }
            else
            {
                this.allDayLabelLayout = RadRect.Empty;
            }
        }

        private void ArrangeAppointments(RadRect viewRect)
        {
            this.ArrangeAppointments();

            this.totalAllDayAreaHeight = 0;
            this.allDayAreaRowCount = 0;
            if (this.Calendar.multiDayViewSettings.ShowAllDayArea)
            {
                this.ArrangeAllDayAppointment(viewRect);
            }
        }

        private void ArrangeAllDayAppointment(RadRect viewRect)
        {
            AppointmentSource appointmentSource = this.Calendar.appointmentSource;
            if (this.allDayAppointmentInfos == null)
            {
                this.allDayAppointmentInfos = new List<CalendarAppointmentInfo>();
            }
            else
            {
                this.allDayAppointmentInfos.Clear();
            }

            if (appointmentSource != null)
            {
                MultiDayViewSettings settings = this.Calendar.multiDayViewSettings;
                double appoitmentHeight = settings.AllDayAppointmentMinHeight;

                foreach (var cell in this.CalendarCells)
                {
                    LinkedList<IAppointment> appointments = appointmentSource.GetAppointments((IAppointment appointment) =>
                    {
                        return cell.Date.Date >= appointment.StartDate.Date && cell.Date.Date <= appointment.EndDate.Date && appointment.IsAllDay;
                    });

                    if (appointments.Count > this.allDayAreaRowCount)
                    {
                        this.allDayAreaRowCount = appointments.Count;
                    }

                    var sorterAppointments = appointments.OrderByDescending(a => a.EndDate.Ticks - a.StartDate.Ticks).ToList();
                    var containedInfos = this.allDayAppointmentInfos.Where(a => sorterAppointments.Contains(a.childAppointment));
                    double prevBottom = cell.layoutSlot.Y - this.dayViewLayoutSlot.Y;

                    foreach (var appointment in sorterAppointments)
                    {
                        while (true)
                        {
                            int widthCoeff = (appointment.EndDate - appointment.StartDate).Days;
                            int xCoeff = (cell.Date - appointment.StartDate.Date).Days;
                            RadRect layoutSlot = new RadRect(cell.layoutSlot.X - (xCoeff * cell.layoutSlot.Width), prevBottom, cell.layoutSlot.Width + (cell.layoutSlot.Width * widthCoeff) + this.Calendar.GridLinesThickness, appoitmentHeight);
                            if (containedInfos.FirstOrDefault(a => a.layoutSlot.IntersectsWith(layoutSlot)) == null)
                            {
                                CalendarAppointmentInfo containedInfo = containedInfos.FirstOrDefault(a => a.childAppointment == appointment);
                                if (containedInfo != null)
                                {
                                    break;
                                }

                                CalendarAppointmentInfo info = new CalendarAppointmentInfo();
                                info.Date = cell.Date;
                                info.Appointments = appointments;
                                info.columnIndex = cell.ColumnIndex;
                                info.Brush = appointment.Color;
                                info.cell = cell;
                                info.childAppointment = appointment;
                                info.DetailText = appointment.Description;
                                info.Subject = appointment.Subject;
                                info.IsAllDay = appointment.IsAllDay;

                                info.layoutSlot = layoutSlot;
                                this.allDayAppointmentInfos.Add(info);
                                prevBottom = layoutSlot.Bottom + settings.AllDayAppointmentSpacing;
                                break;
                            }

                            prevBottom = layoutSlot.Bottom + settings.AllDayAppointmentSpacing;
                        }
                    }
                }

                int maxVisibleRows = settings.AllDayMaxVisibleRows;
                this.totalAllDayAreaHeight = this.allDayAreaRowCount > maxVisibleRows
                    ? maxVisibleRows * appoitmentHeight + maxVisibleRows * settings.AllDayAppointmentSpacing + appoitmentHeight / 2
                    : this.allDayAreaRowCount * appoitmentHeight + this.allDayAreaRowCount * settings.AllDayAppointmentSpacing + appoitmentHeight / 2;

                if (this.allDayAreaRowCount == 0)
                {
                    this.totalAllDayAreaHeight = 0;
                }
            }
        }

        private void ArrangeCurrentTimeIndicator(RadRect viewRect)
        {
            if (this.currentTimeIndicator == null)
            {
                this.currentTimeIndicator = new CalendarGridLine();
                this.currentTimeIndicator.IsHorizontal = true;
                this.currentTimeIndicator.root = this.root;
            }

            MultiDayViewSettings settings = this.Calendar.multiDayViewSettings;
            DateTime currentDate = DateTime.Now;

            if (currentDate.TimeOfDay >= settings.DayStartTime && currentDate.TimeOfDay <= settings.DayEndTime)
            {
                double verticalPosition = this.DateToVerticalPosition(currentDate, currentDate);
                this.currentTimeIndicator.Arrange(new RadRect(this.layoutSlot.X, verticalPosition, viewRect.Width, CalendarMultiDayViewModel.DefaultCurrentTimeIndicatorHeight));
            }
            else
            {
                this.currentTimeIndicator.layoutSlot = RadRect.Empty;
            }
        }

        private void ArrangeMultiDayViewCalendarDecorations(RadRect rect)
        {
            if (this.horizontalLowerAllDayAreaRulerGridLine == null && this.verticalRulerGridLine == null
                && this.horizontalUpperAllDayAreaRulerGridLine == null)
            {
                this.horizontalLowerAllDayAreaRulerGridLine = new CalendarGridLine();
                this.horizontalLowerAllDayAreaRulerGridLine.root = this.root;
                this.horizontalLowerAllDayAreaRulerGridLine.IsHorizontal = true;

                this.horizontalUpperAllDayAreaRulerGridLine = new CalendarGridLine();
                this.horizontalUpperAllDayAreaRulerGridLine.root = this.root;
                this.horizontalUpperAllDayAreaRulerGridLine.IsHorizontal = true;

                this.horizontalTopHeaderRulerGridLine = new CalendarGridLine();
                this.horizontalTopHeaderRulerGridLine.root = this.root;
                this.horizontalTopHeaderRulerGridLine.IsHorizontal = true;

                this.verticalRulerGridLine = new CalendarGridLine();
                this.verticalRulerGridLine.root = this.root;
            }

            double cellHeight = rect.Height / this.SpecificColumnCount;
            double gridLineThickness = this.Calendar.GridLinesThickness;
            int gridLineHalfThickness = (int)(gridLineThickness / 2);

            if ((this.Calendar.GridLinesVisibility & GridLinesVisibility.Horizontal) == GridLinesVisibility.Horizontal)
            {
                this.horizontalTopHeaderRulerGridLine.Arrange(new RadRect(this.layoutSlot.X, rect.Y, this.layoutSlot.Width, gridLineThickness));
                this.horizontalUpperAllDayAreaRulerGridLine.Arrange(new RadRect(this.layoutSlot.X, rect.Y + cellHeight, this.layoutSlot.Width, gridLineThickness));
                this.horizontalLowerAllDayAreaRulerGridLine.Arrange(new RadRect(this.layoutSlot.X, rect.Y + cellHeight + this.totalAllDayAreaHeight + gridLineThickness / 2, this.layoutSlot.Width, 0));
            }

            if ((this.Calendar.GridLinesVisibility & GridLinesVisibility.Vertical) == GridLinesVisibility.Vertical)
            {
                this.verticalRulerGridLine.Arrange(new RadRect(rect.X, rect.Y, gridLineThickness, rect.Height));
            }
        }

        private void EnsureTimeRulerItems()
        {
            if (this.timeRulerItems == null)
            {
                this.timeRulerItems = new ElementCollection<CalendarTimeRulerItem>(this);
            }

            if (this.timeRulerItems.Count == 0)
            {
                MultiDayViewSettings settings = this.Calendar.multiDayViewSettings;

                long dayViewAreaTicks = settings.DayEndTime.Ticks - settings.DayStartTime.Ticks;
                double numberOfItems = Math.Ceiling((double)dayViewAreaTicks / (double)settings.TimerRulerTickLength.Ticks);
                for (int i = 0; i < numberOfItems; i++)
                {
                    CalendarTimeRulerItem timeRulerItem = new CalendarTimeRulerItem();
                    this.timeRulerItems.Add(timeRulerItem);
                }
            }
            else
            {
                foreach (CalendarTimeRulerItem timeRulerItem in this.timeRulerItems)
                {
                    timeRulerItem.layoutSlot = RadRect.Empty;
                }
            }
        }

        private void EnsureTimeRulerLines()
        {
            if (this.timerRulerLines == null || this.timerRulerLines.Count == 0)
            {
                if (this.timerRulerLines == null)
                {
                    this.timerRulerLines = new ElementCollection<CalendarGridLine>(this);
                }

                int step = this.timeRulerItems.Count;
                for (int timeRulerLine = 0; timeRulerLine < step - 1; timeRulerLine++)
                {
                    CalendarGridLine rulerLine = new CalendarGridLine();
                    rulerLine.IsHorizontal = true;

                    this.timerRulerLines.Add(rulerLine);
                }
            }
            else
            {
                foreach (CalendarGridLine rulerLine in this.timerRulerLines)
                {
                    rulerLine.layoutSlot = RadRect.Empty;
                }
            }
        }

        private double DateToVerticalPosition(DateTime currentDate, DateTime dateToMeasure)
        {
            CalendarTimeRulerItem currentTimRulerItem = null;
            TimeSpan timeOfDay;
            double verticalPosition = this.halfTextHeight - this.Calendar.GridLinesThickness / 2;
            if (currentDate.Date.Date < dateToMeasure.Date)
            {
                currentTimRulerItem = this.timeRulerItems.Last();
                timeOfDay = currentTimRulerItem.EndTime;
            }
            else if (currentDate.Date.Date > dateToMeasure.Date)
            {
                currentTimRulerItem = this.timeRulerItems.First();
                timeOfDay = currentTimRulerItem.StartTime;
            }
            else
            {
                timeOfDay = dateToMeasure.TimeOfDay;
                currentTimRulerItem = this.timeRulerItems.FirstOrDefault(a => a.StartTime <= timeOfDay && a.EndTime >= timeOfDay);
            }

            if (currentTimRulerItem != null)
            {
                verticalPosition += currentTimRulerItem.layoutSlot.Y;

                TimeSpan totalSlotTimeLenght = currentTimRulerItem.EndTime - currentTimRulerItem.StartTime;
                TimeSpan timeDifference = totalSlotTimeLenght - (currentTimRulerItem.EndTime - timeOfDay);

                double positionCoeff = timeDifference.TotalMilliseconds / totalSlotTimeLenght.TotalMilliseconds;
                if (positionCoeff > 0)
                {
                    double heightCoeff = (double)this.Calendar.multiDayViewSettings.TimerRulerTickLength.Ticks
                        / (double)TimeSpan.FromHours(1).Ticks;
                    double timeItemHeight = this.Calendar.multiDayViewSettings.TimeLinesSpacing * heightCoeff;

                    double slotPositionDifference = timeItemHeight * positionCoeff;
                    verticalPosition += slotPositionDifference;
                }
            }
            else
            {
                verticalPosition = 0;
            }

            return verticalPosition;
        }

        private void ArrangeTimerRuler(RadRect viewRect)
        {
            CalendarModel model = this.Calendar;
            MultiDayViewSettings settings = model.multiDayViewSettings;
            DateTime currentDate = model.DisplayDate.Date;

            double timeWidth = this.dayViewLayoutSlot.X - viewRect.X;
            TimeSpan timeSlotTime = settings.DayStartTime;
            string textToMeasure = string.Format(this.Calendar.Culture, this.Calendar.TimeFormat, currentDate.Add(timeSlotTime));
            RadSize timeTextSize = model.View.MeasureContent(null, textToMeasure);
            this.halfTextHeight = timeTextSize.Height / 2;

            double previousBottom = viewRect.Y - this.halfTextHeight;

            string labelText = string.Empty;
            double heightCoeff;
            double timeItemHeight;
            double oneHourTicks = (double)TimeSpan.FromHours(1).Ticks;
          
            for (int hourIndex = 0; hourIndex < this.timeRulerItems.Count; hourIndex++)
            {
                CalendarTimeRulerItem timerRulerItem = this.timeRulerItems[hourIndex];
                timerRulerItem.StartTime = timeSlotTime;
                timerRulerItem.Label = labelText;

                timeSlotTime = timeSlotTime.Add(TimeSpan.FromTicks(settings.TimerRulerTickLength.Ticks));
                if (timeSlotTime > settings.DayEndTime)
                {
                    timeSlotTime = settings.DayEndTime;
                }

                timerRulerItem.EndTime = timeSlotTime;

                heightCoeff = (timerRulerItem.EndTime - timerRulerItem.StartTime).Ticks / oneHourTicks;
                timeItemHeight = settings.TimeLinesSpacing * heightCoeff;

                timerRulerItem.Arrange(new RadRect(0f, previousBottom, timeWidth, timeItemHeight + this.halfTextHeight));
                previousBottom = timerRulerItem.layoutSlot.Bottom - this.halfTextHeight;

                labelText = string.Format(this.Calendar.Culture, this.Calendar.TimeFormat, currentDate.Add(timeSlotTime));
            }
        }

        private void ArrangeTimerRulerLines(RadRect viewRect)
        {
            var calendar = this.Calendar;
            double heightCoeff = (double)calendar.multiDayViewSettings.TimerRulerTickLength.Ticks / (double)TimeSpan.FromHours(1).Ticks;

            int tileLinesInterval = calendar.multiDayViewSettings.TimeLinesSpacing;
            double timeItemHeight = tileLinesInterval * heightCoeff;

            double gridLineThickness = calendar.GridLinesThickness;
            int gridLineHalfThickness = (int)(gridLineThickness / 2);

            for (int hourIndex = 1; hourIndex <= this.timerRulerLines.Count; hourIndex++)
            {
                CalendarGridLine timeLine = this.timerRulerLines[hourIndex - 1];
                if ((calendar.GridLinesVisibility & GridLinesVisibility.Horizontal) == GridLinesVisibility.Horizontal)
                {
                    timeLine.Arrange(new RadRect(viewRect.X, viewRect.Y + hourIndex * timeItemHeight - gridLineHalfThickness, viewRect.Width, gridLineThickness));
                }
            }
        }
    }
}
