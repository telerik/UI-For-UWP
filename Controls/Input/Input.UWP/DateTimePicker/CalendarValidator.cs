using System;
using System.Collections.Generic;

namespace Telerik.UI.Xaml.Controls.Input
{
    internal class CalendarValidator
    { 
        private DateTime minSupportedCalendarValue;
        private DateTime maxSupportedCalendarValue;

        private Windows.Globalization.Calendar maxCalendar;
        private Windows.Globalization.Calendar minCalendar;
        private Windows.Globalization.Calendar currentCalendar;
        private Windows.Globalization.Calendar minCalendarWithStep;
        private Windows.Globalization.Calendar maxCalendarWithStep;

        private DateTimeOffset step;
        private StepBehavior yearStepBehavior;
        private StepBehavior monthStepBehavior;
        private StepBehavior dayStepBehavior;

        internal CalendarValidator(Windows.Globalization.Calendar calendar)
        {
            this.Step = new DateTimeOffset(1, 1, 1, 1, 1, 1, new TimeSpan(0));
            this.Calendar = calendar.Clone();
            this.Calendar.ChangeTimeZone("UTC");
            this.Initialize();
        }

        internal Windows.Globalization.Calendar Calendar
        {
            get
            {
                return this.currentCalendar;
            }
            private set
            {
                if (this.currentCalendar != value)
                {
                    this.HandleCalendarChanged(this.currentCalendar, value);
                }
            }
        }

        internal DateTimeOffset Step
        {
            get
            {
                return this.step;
            }
            set
            {
                if (this.step != value)
                {
                    this.step = value;
                    this.UpdateValuesWithStep();
                }
            }
        }

        internal StepBehavior YearStepBehavior
        {
            get
            {
                return this.yearStepBehavior;
            }
            set
            {
                if (this.yearStepBehavior != value)
                {
                    this.yearStepBehavior = value;
                    this.UpdateValuesWithStep();
                }
            }
        }

        internal StepBehavior MonthStepBehavior
        {
            get
            {
                return this.monthStepBehavior;
            }
            set
            {
                if (this.monthStepBehavior != value)
                {
                    this.monthStepBehavior = value;
                    this.UpdateValuesWithStep();
                }
            }
        }

        internal StepBehavior DayStepBehavior
        {
            get
            {
                return this.dayStepBehavior;
            }
            set
            {
                if (this.dayStepBehavior != value)
                {
                    this.dayStepBehavior = value;
                    this.UpdateValuesWithStep();
                }
            }
        }

        internal void ChangeNumeralSystem(string numeralSystem)
        {
            if (this.currentCalendar.NumeralSystem != numeralSystem)
            {
                this.Calendar.NumeralSystem = numeralSystem;
                this.maxCalendar.NumeralSystem = numeralSystem;
                this.minCalendar.NumeralSystem = numeralSystem;
                this.currentCalendar.NumeralSystem = numeralSystem;
                this.minCalendarWithStep.NumeralSystem = numeralSystem;
                this.maxCalendarWithStep.NumeralSystem = numeralSystem;
            }
        }

        internal void ChangeLanguage(List<string> languages)
        {
            this.Calendar = new Windows.Globalization.Calendar(languages, this.currentCalendar.GetCalendarSystem(), this.currentCalendar.GetClock());
        }

        internal void ChangeClock(string newValue)
        {
            if (this.currentCalendar.GetClock() != newValue)
            {
                this.Calendar.ChangeClock(newValue);
                this.maxCalendar.ChangeClock(newValue);
                this.minCalendar.ChangeClock(newValue);
                this.currentCalendar.ChangeClock(newValue);
                this.minCalendarWithStep.ChangeClock(newValue);
                this.maxCalendarWithStep.ChangeClock(newValue);
            }
        }

        internal void ChangeCalendarSystem(string newValue)
        {
            if (this.currentCalendar.GetCalendarSystem() != newValue)
            {
                var oldMaxValue = this.maxCalendar.GetUtcDateTime();
                var oldMinValue = this.minCalendar.GetUtcDateTime();

                this.Calendar.SetToNow();
                this.Calendar.ChangeCalendarSystem(newValue);
                this.UpdateCalendars(this.Calendar, oldMaxValue, oldMinValue);
            }
        }

        internal DateTime GetMaxValueWithStep()
        {
            return this.maxCalendarWithStep.GetUtcDateTime();
        }

        internal DateTime GetMaxValue()
        {
            return this.maxCalendar.GetUtcDateTime();
        }

        internal DateTime GetMinValueWithStep()
        {
            return this.minCalendarWithStep.GetUtcDateTime();
        }

        internal DateTime GetMinValue()
        {
            return this.minCalendar.GetUtcDateTime();
        }

        internal bool SetCoerceMaxValue(DateTime maxUtcValue)
        {
            if (maxUtcValue.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException();
            }

            bool isCoerced = false;
            if (this.maxSupportedCalendarValue < maxUtcValue)
            {
                this.maxCalendar.SetToMax();
                isCoerced = true;
            }
            else if (this.minCalendar.CompareDateTime(maxUtcValue) != 1)
            {
                this.maxCalendar.SetDateTime(maxUtcValue);
            }

            this.UpdateMaxCalendarWithStep();
            return isCoerced;
        }

        internal bool SetCoerceMinValue(DateTime minUtcValue)
        {
            if (minUtcValue.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException();
            }

            bool isCoerced = false;
            if (this.maxSupportedCalendarValue < minUtcValue)
            {
                this.minCalendar.SetToMax();
                isCoerced = true;
            }
            else if (this.maxCalendar.CompareDateTime(minUtcValue) != -1)
            {
                this.minCalendar.SetDateTime(minUtcValue);
            }

            this.UpdateMinCalendarWithStep();
            return isCoerced;
        }

        internal bool IsValueInRange(DateTime utcValue)
        {
            if (utcValue.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException();
            }

            return this.minCalendar.CompareDateTime(utcValue) != 1 && this.maxCalendar.CompareDateTime(utcValue) != -1;
        }

        internal DateTime CoerceDateTime(DateTime utcValue)
        {
            if (utcValue.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException();
            }

            if (this.minCalendarWithStep.CompareDateTime(utcValue) == 1)
            {
                return this.minCalendarWithStep.GetUtcDateTime();
            }

            if (this.maxCalendarWithStep.CompareDateTime(utcValue) == -1)
            {
                return this.maxCalendarWithStep.GetUtcDateTime();
            }

            return utcValue;
        }

        internal int GetStepRemainderForComponent(int componentValue, int minComponentValue, StepBehavior behavior, int step)
        {
            if (step == 1)
            {
                return 0;
            }

            var remainder = 0;
            switch (behavior)
            {
                case StepBehavior.Multiples:
                    remainder = componentValue % step;
                    break;
                case StepBehavior.StartFromBase:
                    remainder = (componentValue - minComponentValue) % step;
                    break;
                case StepBehavior.BaseAndMultiples:
                    remainder = componentValue == minComponentValue ? 0 : componentValue % step;
                    break;
            }

            return remainder;
        }

        internal DateTime GetDateTimeValueWithDateStepApplied(DateTime utcValue)
        {
            utcValue = this.CoerceDateTime(utcValue);
            var calendar = this.currentCalendar.Clone();
            calendar.SetDateTime(utcValue);

            var minYear = this.minCalendarWithStep.Year;
            var maxYear = this.maxCalendarWithStep.Year;

            var yearStep = this.GetStepForComponentType(DateTimeComponentType.Year);
            var monthStep = this.GetStepForComponentType(DateTimeComponentType.Month);
            var dayStep = this.GetStepForComponentType(DateTimeComponentType.Day);

            var yearRemainder = this.GetStepRemainderForComponent(calendar.Year, calendar.FirstYearInThisEra, this.YearStepBehavior, yearStep);
            var currentYear = calendar.Year - yearRemainder;

            // construct valid starting year
            if (currentYear < minYear)
            {
                currentYear += yearStep;
                if (currentYear > maxYear)
                {
                    throw new InvalidOperationException();
                }
            }

            // keep the start calendar
            var startCalendar = calendar.Clone();
            startCalendar.AddYears(currentYear - startCalendar.Year);

            while (currentYear >= minYear)
            {
                var yearUpdate = currentYear - calendar.Year;
                calendar.AddYears(currentYear - calendar.Year);
                if (yearUpdate < 0)
                {
                    // go to previous year => reset month & day to max
                    calendar.Month = calendar.LastMonthInThisYear;
                    calendar.Day = calendar.LastDayInThisMonth;
                }

                if (this.FindValidMonth(calendar, monthStep, dayStep))
                {
                    return calendar.GetUtcDateTime();
                }

                currentYear -= yearStep;
            }

            calendar = startCalendar.Clone();
            currentYear = calendar.Year;

            while (currentYear <= maxYear)
            {
                var yearUpdate = currentYear - calendar.Year;
                calendar.AddYears(currentYear - calendar.Year);
                if (yearUpdate > 0)
                {
                    // go to previous year => reset month & day to max
                    calendar.Month = calendar.FirstMonthInThisYear;
                    calendar.Day = calendar.FirstDayInThisMonth;
                }

                if (this.FindValidMonth(calendar, monthStep, dayStep))
                {
                    return calendar.GetUtcDateTime();
                }

                currentYear += yearStep;
            }

            // TO DO: what to do here?
            throw new InvalidOperationException();
        }

        internal DateTime GetDateTimeValueWithTimeStepApplied(DateTime utcValue)
        {
            utcValue = this.CoerceDateTime(utcValue);
            var calendar = this.currentCalendar.Clone();
            calendar.SetDateTime(utcValue);

            var hourStep = this.GetStepForComponentType(DateTimeComponentType.Hour);
            var minuteStep = this.GetStepForComponentType(DateTimeComponentType.Minute);

            var isMinDate = calendar.Year == this.minCalendarWithStep.Year && calendar.Month == this.minCalendarWithStep.Month && calendar.Day == this.minCalendarWithStep.Day;
            var isMaxDate = calendar.Year == this.maxCalendarWithStep.Year && calendar.Month == this.maxCalendarWithStep.Month && calendar.Day == this.maxCalendarWithStep.Day;

            var minPeriod = isMinDate ? this.minCalendarWithStep.Period : calendar.FirstPeriodInThisDay;
            var maxPeriod = isMaxDate ? this.maxCalendarWithStep.Period : calendar.LastPeriodInThisDay;

            var currentPeriod = calendar.Period;

            // keep the start calendar
            var startCalendar = calendar.Clone();

            while (currentPeriod >= minPeriod)
            {
                var periodUpdate = currentPeriod - calendar.Period;
                calendar.AddPeriods(currentPeriod - calendar.Period);
                if (periodUpdate < 0)
                {
                    // go to previous period => reset hours & minutes to max
                    calendar.Hour = calendar.LastHourInThisPeriod;
                    calendar.Minute = calendar.LastMinuteInThisHour;
                }

                if (this.FindValidHour(calendar, hourStep, minuteStep, isMinDate, isMaxDate))
                {
                    return calendar.GetUtcDateTime();
                }

                currentPeriod--;
            }

            calendar = startCalendar;
            currentPeriod = calendar.Period;

            while (currentPeriod <= maxPeriod)
            {
                var periodsUpdate = currentPeriod - calendar.Period;
                calendar.AddPeriods(periodsUpdate);

                if (periodsUpdate > 0)
                {
                    // go to next period => reset hours & minutes to min
                    calendar.Hour = this.minCalendarWithStep.FirstHourInThisPeriod;
                    calendar.Minute = this.minCalendarWithStep.FirstMinuteInThisHour;
                }

                if (this.FindValidHour(calendar, hourStep, minuteStep, isMinDate, isMaxDate))
                {
                    return calendar.GetUtcDateTime();
                }

                currentPeriod++;
            }

            throw new InvalidOperationException("No valid date found.");
        }

        internal DateTime CoerceDateTimeWithStep(int logicalIndexBasedOnBehavior, DateTimeComponentType componentType, DateTime value)
        {
            if (this.minCalendarWithStep.CompareDateTime(value) == 1)
            {
                value = this.minCalendarWithStep.GetUtcDateTime();
            }
            else if (this.maxCalendarWithStep.CompareDateTime(value) == -1)
            {
                value = this.maxCalendarWithStep.GetUtcDateTime();
            }

            this.Calendar.SetDateTime(value);
            switch (componentType)
            {
                case DateTimeComponentType.Day:
                    logicalIndexBasedOnBehavior += this.Calendar.FirstDayInThisMonth - 1;
                    var day = Math.Min(this.Calendar.NumberOfDaysInThisMonth, logicalIndexBasedOnBehavior);
                    this.Calendar.AddDays(day - this.Calendar.Day);
                    return this.Calendar.GetUtcDateTime();
                case DateTimeComponentType.Month:
                    logicalIndexBasedOnBehavior += this.Calendar.FirstMonthInThisYear - 1;
                    var month = Math.Min(this.Calendar.NumberOfMonthsInThisYear, logicalIndexBasedOnBehavior);
                    this.Calendar.AddMonths(month - this.Calendar.Month);
                    if (this.Calendar.Year == this.minCalendarWithStep.Year &&
                        this.Calendar.Month == this.minCalendarWithStep.Month &&
                        this.Calendar.Day < this.minCalendarWithStep.Day)
                    {
                        this.Calendar.Day = this.minCalendarWithStep.Day;
                    }
                    if (this.Calendar.Year == this.maxCalendarWithStep.Year &&
                        this.Calendar.Month == this.maxCalendarWithStep.Month &&
                        this.Calendar.Day > this.maxCalendarWithStep.Day)
                    {
                        this.Calendar.Day = this.maxCalendarWithStep.Day;
                    }
                    return this.Calendar.GetUtcDateTime();
                case DateTimeComponentType.Year:
                    logicalIndexBasedOnBehavior += this.Calendar.FirstYearInThisEra - 1;
                    var year = logicalIndexBasedOnBehavior;
                    this.Calendar.AddYears(year - this.Calendar.Year);
                    if ((this.Calendar.Year == this.minCalendarWithStep.Year) &&
                        ((this.Calendar.Month < this.minCalendarWithStep.Month) ||
                         (this.Calendar.Month == this.minCalendarWithStep.Month &&
                          this.Calendar.Day < this.minCalendarWithStep.Day)))
                    {
                        return this.minCalendarWithStep.GetUtcDateTime();
                    }
                    if ((this.Calendar.Year == this.maxCalendarWithStep.Year) &&
                        ((this.Calendar.Month > this.maxCalendarWithStep.Month) ||
                         (this.Calendar.Month == this.maxCalendarWithStep.Month &&
                          this.Calendar.Day > this.maxCalendarWithStep.Day)))
                    {
                        return this.maxCalendarWithStep.GetUtcDateTime();
                    }
                    return this.Calendar.GetUtcDateTime();
                case DateTimeComponentType.Hour:
                    int calendarHour = this.Calendar.ZeroBasedHour();
                    int hour = logicalIndexBasedOnBehavior;
                    this.Calendar.AddHours(hour - calendarHour);
                    var date = this.Calendar.GetUtcDateTime().Date;
                    var isMinDate = date == this.minCalendarWithStep.GetUtcDateTime().Date;
                    var isMaxDate = date == this.maxCalendarWithStep.GetUtcDateTime().Date;
                    if (isMinDate && this.Calendar.Hour == this.minCalendarWithStep.Hour && this.Calendar.Minute < this.minCalendarWithStep.Minute)
                    {
                        return this.minCalendarWithStep.GetUtcDateTime();
                    }
                    if (isMaxDate && this.Calendar.Hour == this.maxCalendarWithStep.Hour && this.Calendar.Minute > this.maxCalendarWithStep.Minute)
                    {
                        return this.maxCalendarWithStep.GetUtcDateTime();
                    }
                    return this.Calendar.GetUtcDateTime();
                case DateTimeComponentType.Minute:
                    int minute = logicalIndexBasedOnBehavior;
                    this.Calendar.AddMinutes(minute - this.Calendar.Minute);
                    return this.Calendar.GetUtcDateTime();
                case DateTimeComponentType.AMPM:
                    int period = logicalIndexBasedOnBehavior + this.Calendar.FirstPeriodInThisDay;
                    this.Calendar.AddPeriods(period - this.Calendar.Period);
                    return this.Calendar.GetUtcDateTime();
            }

            return value;
        }

        private void HandleCalendarChanged(Windows.Globalization.Calendar oldCalendar, Windows.Globalization.Calendar newCalendar)
        { 
            DateTime oldMaxValue = DateTime.MaxValue;
            DateTime oldMinValue = DateTime.MinValue;

            if (oldCalendar != null)
            {
                oldMaxValue = this.maxCalendar.GetUtcDateTime();
                oldMinValue = this.minCalendar.GetUtcDateTime();
            }

            if (newCalendar == null)
            {
                throw new ArgumentException();
            }
            
            newCalendar.ChangeTimeZone("UTC");
            this.UpdateCalendars(newCalendar, oldMaxValue, oldMinValue);
        }
        
        private void Initialize()
        {
        }

        private void UpdateCalendars(Windows.Globalization.Calendar calendar, DateTime oldMaxValue, DateTime oldMinValue)
        {
            this.currentCalendar = calendar.Clone();
            this.maxCalendar = calendar.Clone();
            this.minCalendar = calendar.Clone();

            this.maxCalendar.SetToMax();
            this.minCalendar.SetToMin();

            this.maxSupportedCalendarValue = this.maxCalendar.GetUtcDateTime();
            this.minSupportedCalendarValue = this.minCalendar.GetUtcDateTime();

            if (oldMaxValue < this.maxSupportedCalendarValue)
            {
                this.maxCalendar.SetDateTime(oldMaxValue);
            }

            if (oldMinValue > this.minSupportedCalendarValue)
            {
                this.minCalendar.SetDateTime(oldMinValue);
            }

            this.minCalendarWithStep = this.minCalendar.Clone();
            this.maxCalendarWithStep = this.maxCalendar.Clone();
            this.UpdateValuesWithStep();
        }

        private int GetStepForComponentType(DateTimeComponentType componentType)
        {
            switch (componentType)
            {
                case DateTimeComponentType.Day:
                    return this.Step.Day;
                case DateTimeComponentType.Month:
                    return this.Step.Month;
                case DateTimeComponentType.Year:
                    return this.Step.Year;
                case DateTimeComponentType.Hour:
                    return this.Step.Hour == 0 ? 1 : this.Step.Hour;
                case DateTimeComponentType.Minute:
                    return this.Step.Minute == 0 ? 1 : this.Step.Minute;
                case DateTimeComponentType.Second:
                    return this.Step.Second == 0 ? 1 : this.Step.Second;
                default:
                    return 1;
            }
        }

        private void UpdateMaxCalendarWithStep()
        {
            this.maxCalendarWithStep = this.maxCalendar.Clone();
            this.ApplyDateStepToMaxCalendar();
            this.ApplyTimeStepToMaxCalendar();
        }

        private void UpdateMinCalendarWithStep()
        {
            this.minCalendarWithStep = this.minCalendar.Clone();
            this.ApplyDateStepToMinCalendar();
            this.ApplyTimeStepToMinCalendar();
        }

        private void UpdateValuesWithStep()
        {
            if (this.currentCalendar != null)
            {
                this.UpdateMinCalendarWithStep();
                this.UpdateMaxCalendarWithStep();
            }
        }

        #region ApplyTimeStepToMin

        private void ApplyTimeStepToMinCalendar()
        {
            var oldMinCalendar = this.minCalendarWithStep.Clone();

            var areMinMaxDatesEqual = this.minCalendarWithStep.Year == this.maxCalendarWithStep.Year &&
                                      this.minCalendarWithStep.Month == this.maxCalendarWithStep.Month &&
                                      this.minCalendarWithStep.Day == this.maxCalendarWithStep.Day;

            var hourStep = this.GetStepForComponentType(DateTimeComponentType.Hour);
            var minuteStep = this.GetStepForComponentType(DateTimeComponentType.Minute);

            var maxPeriod = areMinMaxDatesEqual ? this.maxCalendarWithStep.Period : this.minCalendarWithStep.LastPeriodInThisDay;
            var currentPeriod = this.minCalendarWithStep.Period;

            while (currentPeriod <= maxPeriod)
            {
                var periodsUpdate = currentPeriod - this.minCalendarWithStep.Period;
                this.minCalendarWithStep.AddPeriods(periodsUpdate);

                if (periodsUpdate > 0)
                {
                    // go to next period => reset hours & minutes to min
                    this.minCalendarWithStep.Hour = this.minCalendarWithStep.FirstHourInThisPeriod;
                    this.minCalendarWithStep.Minute = this.minCalendarWithStep.FirstMinuteInThisHour;
                }

                if (this.FindValidMinHour(hourStep, minuteStep, areMinMaxDatesEqual))
                {
                    return;
                }

                currentPeriod++;
            }

            // TO DO: what to do here?
            this.minCalendarWithStep = oldMinCalendar.Clone();
            throw new InvalidOperationException();
        }

        private bool FindValidMinHour(int hourStep, int minuteStep, bool areMinMaxDatesEqual)
        {
            var clock = this.minCalendarWithStep.GetClock();
            int maxHour;

            if (areMinMaxDatesEqual && this.minCalendarWithStep.Period == this.maxCalendarWithStep.Period)
            {
                maxHour = this.maxCalendarWithStep.ZeroBasedHour();
            }
            else
            {
                maxHour = DateTimeHelper.GetZeroBasedHour(clock, this.minCalendar.LastHourInThisPeriod);
            }

            var firstHour = DateTimeHelper.GetZeroBasedHour(clock, this.minCalendarWithStep.FirstHourInThisPeriod);
            var currentHour = this.minCalendarWithStep.ZeroBasedHour();

            var hourRemainder = this.GetStepRemainderForComponent(currentHour, firstHour, StepBehavior.StartFromBase, hourStep);

            currentHour += (hourStep - hourRemainder) % hourStep;

            if (currentHour > maxHour)
            {
                return false;
            }

            while (currentHour <= maxHour)
            {
                var hoursUpdate = currentHour - this.minCalendarWithStep.ZeroBasedHour();
                this.minCalendarWithStep.AddHours(hoursUpdate);

                if (hoursUpdate > 0)
                {
                    this.minCalendarWithStep.Minute = this.minCalendarWithStep.FirstMinuteInThisHour;
                }

                if (this.FindValidMinMinute(minuteStep, areMinMaxDatesEqual))
                {
                    return true;
                }

                currentHour += hourStep;
            }

            return false;
        }

        private bool FindValidMinMinute(int minuteStep, bool areMinMaxDatesEqual)
        {
            int maxMinute;
            if (areMinMaxDatesEqual && this.minCalendarWithStep.Period == this.maxCalendarWithStep.Period && this.minCalendarWithStep.Hour == this.maxCalendarWithStep.Hour)
            {
                maxMinute = this.maxCalendarWithStep.Minute;
            }
            else
            {
                maxMinute = this.minCalendarWithStep.LastMinuteInThisHour;
            }
            var currentMinute = this.minCalendarWithStep.Minute;
            var minuteRemainder = this.GetStepRemainderForComponent(this.minCalendarWithStep.Minute, this.minCalendarWithStep.FirstMinuteInThisHour, StepBehavior.StartFromBase, minuteStep);

            currentMinute += (minuteStep - minuteRemainder) % minuteStep;

            if (currentMinute > maxMinute)
            {
                return false;
            }

            this.minCalendarWithStep.Minute = currentMinute;
            return true;
        }

        #endregion

        #region ApplyTimeStepToMax

        private void ApplyTimeStepToMaxCalendar()
        {
            var oldMaxCalendar = this.minCalendarWithStep.Clone();

            var areMinMaxDatesEqual = this.minCalendarWithStep.Year == this.maxCalendarWithStep.Year &&
                                      this.minCalendarWithStep.Month == this.maxCalendarWithStep.Month &&
                                      this.minCalendarWithStep.Day == this.maxCalendarWithStep.Day;

            var hourStep = this.GetStepForComponentType(DateTimeComponentType.Hour);
            var minuteStep = this.GetStepForComponentType(DateTimeComponentType.Minute);

            var minPeriod = areMinMaxDatesEqual ? this.minCalendarWithStep.Period : this.maxCalendarWithStep.FirstPeriodInThisDay;
            var currentPeriod = this.maxCalendarWithStep.Period;

            while (currentPeriod >= minPeriod)
            {
                var periodUpdate = currentPeriod - this.maxCalendarWithStep.Period;
                this.maxCalendarWithStep.AddPeriods(periodUpdate);

                if (periodUpdate < 0)
                {
                    // go to previous period => reset hours & minutes to max
                    this.maxCalendarWithStep.Hour = this.maxCalendarWithStep.LastHourInThisPeriod;
                    this.maxCalendarWithStep.Minute = this.maxCalendarWithStep.LastMinuteInThisHour;
                }

                if (this.FindValidMaxHour(hourStep, minuteStep, areMinMaxDatesEqual))
                {
                    return;
                }

                currentPeriod--;
            }

            // TO DO: what to do here?
            this.maxCalendarWithStep = oldMaxCalendar.Clone();
            throw new InvalidOperationException();
        }

        private bool FindValidMaxHour(int hourStep, int minuteStep, bool areMinMaxDatesEqual)
        {
            var clock = this.maxCalendarWithStep.GetClock();
            int minHour;
            if (areMinMaxDatesEqual && this.minCalendarWithStep.Period == this.maxCalendarWithStep.Period)
            {
                minHour = this.maxCalendarWithStep.ZeroBasedHour();
            }
            else
            {
                minHour = DateTimeHelper.GetZeroBasedHour(clock, this.maxCalendarWithStep.FirstHourInThisPeriod);
            }

            var firstHour = DateTimeHelper.GetZeroBasedHour(clock, this.maxCalendarWithStep.FirstHourInThisPeriod);
            var currentHour = this.maxCalendarWithStep.ZeroBasedHour();

            var hourRemainder = this.GetStepRemainderForComponent(currentHour, firstHour, StepBehavior.StartFromBase, hourStep);

            currentHour -= hourRemainder;

            if (currentHour < minHour)
            {
                return false;
            }

            while (currentHour >= minHour)
            {
                var hoursUpdate = currentHour - this.maxCalendarWithStep.ZeroBasedHour();
                this.maxCalendarWithStep.AddHours(hoursUpdate);

                if (hoursUpdate < 0)
                {
                    this.maxCalendarWithStep.Minute = this.maxCalendarWithStep.LastMinuteInThisHour;
                }

                if (this.FindValidMaxMinute(minuteStep, areMinMaxDatesEqual))
                {
                    return true;
                }

                currentHour -= hourStep;
            }

            return false;
        }

        private bool FindValidMaxMinute(int minuteStep, bool areMinMaxDatesEqual)
        {
            int minMinute;
            if (areMinMaxDatesEqual && this.minCalendarWithStep.Period == this.maxCalendarWithStep.Period && this.minCalendarWithStep.Hour == this.maxCalendarWithStep.Hour)
            {
                minMinute = this.minCalendarWithStep.Minute;
            }
            else
            {
                minMinute = this.maxCalendarWithStep.FirstMinuteInThisHour;
            }

            var minuteRemainder = this.GetStepRemainderForComponent(this.maxCalendarWithStep.Minute, this.maxCalendarWithStep.FirstMinuteInThisHour, StepBehavior.StartFromBase, minuteStep);

            var currentMinute = this.maxCalendarWithStep.Minute - minuteRemainder;

            if (currentMinute < minMinute)
            {
                return false;
            }

            this.maxCalendarWithStep.Minute = currentMinute;
            return true;
        }

        #endregion
        
        private bool FindValidHour(Windows.Globalization.Calendar calendar, int hourStep, int minuteStep, bool isMinDate, bool isMaxDate)
        {
            var clock = calendar.GetClock();

            var minHour = isMinDate && calendar.Period == this.minCalendarWithStep.Period ? this.minCalendarWithStep.ZeroBasedHour() : DateTimeHelper.GetZeroBasedHour(clock, calendar.FirstHourInThisPeriod);
            var maxHour = isMaxDate && calendar.Period == this.maxCalendarWithStep.Period ? this.maxCalendarWithStep.ZeroBasedHour() : DateTimeHelper.GetZeroBasedHour(clock, calendar.LastHourInThisPeriod);

            var firstHour = DateTimeHelper.GetZeroBasedHour(clock, calendar.FirstHourInThisPeriod);
            var currentHour = calendar.ZeroBasedHour();

            var hourRemainder = this.GetStepRemainderForComponent(currentHour, firstHour, StepBehavior.StartFromBase, hourStep);

            currentHour -= hourRemainder;

            if (currentHour < minHour)
            {
                currentHour += hourStep;
                if (currentHour > maxHour)
                {
                    return false;
                }
            }

            // keep the start calendar
            var startCalendar = calendar.Clone();
            startCalendar.SetZeroBasedHour(currentHour);

            while (currentHour >= minHour)
            {
                var hoursUpdate = currentHour - calendar.ZeroBasedHour();
                calendar.AddHours(hoursUpdate);
                if (hoursUpdate < 0)
                {
                    calendar.Minute = calendar.LastMinuteInThisHour;
                }

                if (this.FindValidMinute(calendar, minuteStep, isMinDate, isMaxDate))
                {
                    return true;
                }

                currentHour -= hourStep;
            }

            calendar = startCalendar.Clone();
            currentHour = calendar.Hour;

            while (currentHour <= maxHour)
            {
                var hoursUpdate = currentHour - calendar.ZeroBasedHour();
                calendar.AddHours(hoursUpdate);
                if (hoursUpdate > 0)
                {
                    calendar.Minute = calendar.FirstMinuteInThisHour;
                }

                if (this.FindValidMinute(calendar, minuteStep, isMinDate, isMaxDate))
                {
                    return true;
                }

                currentHour += hourStep;
            }

            return false;
        }

        private bool FindValidMinute(Windows.Globalization.Calendar calendar, int minuteStep, bool isMinDate, bool isMaxDate)
        {
            var minMinute = isMinDate && calendar.Period == this.minCalendarWithStep.Period && calendar.Hour == this.minCalendarWithStep.Hour ? this.minCalendarWithStep.Minute : calendar.FirstMinuteInThisHour;
            var maxMinute = isMaxDate && calendar.Period == this.maxCalendarWithStep.Period && calendar.Hour == this.maxCalendarWithStep.Hour ? this.maxCalendarWithStep.Minute : calendar.LastMinuteInThisHour;

            var minuteRemainder = this.GetStepRemainderForComponent(calendar.Minute, calendar.FirstMinuteInThisHour, StepBehavior.StartFromBase, minuteStep);

            var currentMinute = calendar.Minute - minuteRemainder;

            if (currentMinute < minMinute)
            {
                currentMinute += minuteStep;
                if (currentMinute > maxMinute)
                {
                    return false;
                }
            }

            calendar.Minute = currentMinute;
            return true;
        }

        #region ApplyDateStepToMin

        private void ApplyDateStepToMinCalendar()
        {
            var oldMinCalendar = this.minCalendarWithStep.Clone();

            var yearStep = this.GetStepForComponentType(DateTimeComponentType.Year);
            var monthStep = this.GetStepForComponentType(DateTimeComponentType.Month);
            var dayStep = this.GetStepForComponentType(DateTimeComponentType.Day);

            var maxYear = this.maxCalendarWithStep.Year;

            var currentYear = this.minCalendarWithStep.Year;
            var yearRemainder = this.GetStepRemainderForComponent(this.minCalendarWithStep.Year, this.minCalendarWithStep.FirstYearInThisEra, this.YearStepBehavior, yearStep);
            currentYear += (yearStep - yearRemainder) % yearStep;

            while (currentYear <= maxYear)
            {
                var yearsUpdate = currentYear - this.minCalendarWithStep.Year;
                this.minCalendarWithStep.AddYears(yearsUpdate);

                if (yearsUpdate > 0)
                {
                    // go to next year => reset month & day to min
                    this.minCalendarWithStep.Month = this.minCalendarWithStep.FirstMonthInThisYear;
                    this.minCalendarWithStep.Day = this.minCalendarWithStep.FirstDayInThisMonth;
                }

                if (this.FindValidMinMonth(monthStep, dayStep))
                {
                    return;
                }

                currentYear++;
            }

            // TO DO: what to do here?
            this.minCalendarWithStep = oldMinCalendar.Clone();
            throw new InvalidOperationException();
        }

        private bool FindValidMinMonth(int monthStep, int dayStep)
        {
            var maxMonth = this.minCalendarWithStep.Year == this.maxCalendarWithStep.Year ? this.maxCalendarWithStep.Month : this.minCalendarWithStep.LastMonthInThisYear;

            var currentMonth = this.minCalendarWithStep.Month;
            var monthRemainder = this.GetStepRemainderForComponent(currentMonth, this.minCalendarWithStep.FirstMonthInThisYear, this.MonthStepBehavior, monthStep);

            currentMonth += (monthStep - monthRemainder) % monthStep;

            if (currentMonth > maxMonth)
            {
                return false;
            }

            while (currentMonth <= maxMonth)
            {
                var monthsUpdate = currentMonth - this.minCalendarWithStep.Month;
                this.minCalendarWithStep.AddMonths(monthsUpdate);

                if (monthsUpdate > 0)
                {
                    this.minCalendarWithStep.Day = this.minCalendarWithStep.FirstDayInThisMonth;
                }

                if (this.FindValidMinDay(dayStep))
                {
                    return true;
                }

                currentMonth += monthStep;
            }

            return false;
        }

        private bool FindValidMinDay(int dayStep)
        {
            var maxDay = this.minCalendarWithStep.Year == this.maxCalendarWithStep.Year && this.minCalendarWithStep.Month == this.maxCalendarWithStep.Month ? this.maxCalendarWithStep.Day : this.minCalendarWithStep.LastDayInThisMonth;

            var currentDay = this.minCalendarWithStep.Day;
            var dayRemainder = this.GetStepRemainderForComponent(this.minCalendarWithStep.Day, this.minCalendarWithStep.FirstDayInThisMonth, this.DayStepBehavior, dayStep);

            currentDay += (dayStep - dayRemainder) % dayStep;

            if (currentDay > maxDay)
            {
                return false;
            }

            this.minCalendarWithStep.Day = currentDay;
            return true;
        }

        #endregion

        #region ApplyDateStepToMax

        private void ApplyDateStepToMaxCalendar()
        {
            var oldMaxCalendar = this.minCalendarWithStep.Clone();

            var yearStep = this.GetStepForComponentType(DateTimeComponentType.Year);
            var monthStep = this.GetStepForComponentType(DateTimeComponentType.Month);
            var dayStep = this.GetStepForComponentType(DateTimeComponentType.Day);

            var minYear = this.minCalendarWithStep.Year;

            var yearRemainder = this.GetStepRemainderForComponent(this.maxCalendarWithStep.Year, this.maxCalendarWithStep.FirstYearInThisEra, this.YearStepBehavior, yearStep);
            var currentYear = this.maxCalendarWithStep.Year - yearRemainder;

            while (currentYear >= minYear)
            {
                var yearsUpdate = currentYear - this.maxCalendarWithStep.Year;
                this.maxCalendarWithStep.AddYears(yearsUpdate);

                if (yearsUpdate < 0)
                {
                    // go to previous year => reset month & day to max
                    this.maxCalendarWithStep.Month = this.maxCalendarWithStep.LastMonthInThisYear;
                    this.maxCalendarWithStep.Day = this.maxCalendarWithStep.LastDayInThisMonth;
                }

                if (this.FindValidMaxMonth(monthStep, dayStep))
                {
                    return;
                }

                currentYear--;
            }

            // TO DO: what to do here?
            this.maxCalendarWithStep = oldMaxCalendar.Clone();
            throw new InvalidOperationException();
        }

        private bool FindValidMaxMonth(int monthStep, int dayStep)
        {
            var minMonth = this.minCalendarWithStep.Year == this.maxCalendarWithStep.Year ? this.maxCalendarWithStep.Month : this.maxCalendarWithStep.FirstMonthInThisYear;

            var currentMonth = this.maxCalendarWithStep.Month;
            var monthRemainder = this.GetStepRemainderForComponent(currentMonth, this.maxCalendarWithStep.FirstMonthInThisYear, this.MonthStepBehavior, monthStep);

            currentMonth -= monthRemainder;

            if (currentMonth < minMonth)
            {
                return false;
            }

            while (currentMonth >= minMonth)
            {
                var monthsUpdate = currentMonth - this.maxCalendarWithStep.Month;
                this.maxCalendarWithStep.AddMonths(monthsUpdate);

                if (monthsUpdate < 0)
                {
                    this.maxCalendarWithStep.Day = this.maxCalendarWithStep.LastDayInThisMonth;
                }

                if (this.FindValidMaxDay(dayStep))
                {
                    return true;
                }

                currentMonth -= monthStep;
            }

            return false;
        }

        private bool FindValidMaxDay(int dayStep)
        {
            var minDay = this.minCalendarWithStep.Year == this.maxCalendarWithStep.Year && this.minCalendarWithStep.Month == this.maxCalendarWithStep.Month ? this.minCalendarWithStep.Day : this.maxCalendarWithStep.FirstDayInThisMonth;
            var dayRemainder = this.GetStepRemainderForComponent(this.maxCalendarWithStep.Day, this.maxCalendarWithStep.FirstDayInThisMonth, this.DayStepBehavior, dayStep);

            var currentDay = this.maxCalendarWithStep.Day - dayRemainder;

            if (currentDay < minDay)
            {
                return false;
            }

            this.maxCalendarWithStep.Day = currentDay;
            return true;
        }

        #endregion

        #region FindValidValueWithDateStepApplied

        private bool FindValidMonth(Windows.Globalization.Calendar calendar, int monthStep, int dayStep)
        {
            var minMonth = calendar.Year == this.minCalendarWithStep.Year ? this.minCalendarWithStep.Month : calendar.FirstMonthInThisYear;
            var maxMonth = calendar.Year == this.maxCalendarWithStep.Year ? this.maxCalendarWithStep.Month : calendar.LastMonthInThisYear;

            var monthRemainder = this.GetStepRemainderForComponent(calendar.Month, calendar.FirstMonthInThisYear, this.MonthStepBehavior, monthStep);

            var currentMonth = calendar.Month - monthRemainder;

            while (currentMonth < minMonth)
            {
                currentMonth += monthStep;
                if (currentMonth > maxMonth)
                {
                    return false;
                }
            }

            // keep the start calendar
            var startCalendar = calendar.Clone();
            startCalendar.AddMonths(currentMonth - startCalendar.Month);

            while (currentMonth >= minMonth)
            {
                var monthsUpdate = currentMonth - calendar.Month;
                calendar.AddMonths(monthsUpdate);

                if (monthsUpdate < 0)
                {
                    calendar.Day = calendar.LastDayInThisMonth;
                }

                if (this.FindValidDay(calendar, dayStep))
                {
                    return true;
                }

                currentMonth -= monthStep;
            }

            calendar = startCalendar.Clone();
            currentMonth = calendar.Month;

            while (currentMonth <= maxMonth)
            {
                var monthsUpdate = currentMonth - calendar.Month;
                calendar.AddMonths(monthsUpdate);

                if (monthsUpdate > 0)
                {
                    calendar.Day = calendar.FirstDayInThisMonth;
                }

                if (this.FindValidDay(calendar, dayStep))
                {
                    return true;
                }

                currentMonth += monthStep;
            }

            return false;
        }

        private bool FindValidDay(Windows.Globalization.Calendar calendar, int dayStep)
        {
            var minDay = calendar.Year == this.minCalendarWithStep.Year && calendar.Month == this.minCalendarWithStep.Month ? this.minCalendarWithStep.Day : calendar.FirstDayInThisMonth;
            var maxDay = calendar.Year == this.maxCalendarWithStep.Year && calendar.Month == this.maxCalendarWithStep.Month ? this.maxCalendarWithStep.Day : calendar.LastDayInThisMonth;

            var dayRemainder = this.GetStepRemainderForComponent(calendar.Day, calendar.FirstDayInThisMonth, this.DayStepBehavior, dayStep);

            var currentDay = calendar.Day - dayRemainder;

            if (currentDay < minDay)
            {
                currentDay += dayStep;
                if (currentDay > maxDay)
                {
                    return false;
                }
            }

            calendar.Day = currentDay;
            return true;
        }
        
        #endregion
    }
}