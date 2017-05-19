using System;
using Telerik.UI.Xaml.Controls.Input.DateTimePickers;
using Telerik.UI.Xaml.Controls.Primitives.LoopingList;
using Windows.Globalization;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents a data item within a DateTimeList.
    /// </summary>
    internal class DateTimeItem : LoopingListDataItem
    {
        private string headerText;
        private string contentText;
        private DateTimeList owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeItem"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="value">The value.</param>
        public DateTimeItem(DateTimeList owner, DateTime value)
        {
            this.owner = owner;
            this.Update(value);
        }

        /// <summary>
        /// Gets the header of the item.
        /// </summary>
        public string HeaderText
        {
            get
            {
                return this.headerText;
            }
        }

        /// <summary>
        /// Gets the content of the item.
        /// </summary>
        public string ContentText
        {
            get
            {
                return this.contentText;
            }
        }

        /// <summary>
        /// Forces re-evaluation of the HeaderText and ContentText properties.
        /// </summary>
        public void Update(DateTime newUtcValue)
        {
            if (this.owner.Owner == null)
            {
                return;
            }

            // is this ok: owner.Owner.calendarValidator.Calendar?
            var calendar = this.owner.Owner.calendarValidator.Calendar;
            calendar.SetDateTime(newUtcValue);
            this.UpdateText(calendar);
        }

        private void UpdateText(Windows.Globalization.Calendar calendar)
        {
            switch (this.owner.ComponentType)
            {
                case DateTimeComponentType.Day:
                    this.headerText = calendar.DayAsPaddedString(2);
                    this.contentText = calendar.DayOfWeekAsSoloString();
                    break;
                case DateTimeComponentType.Month:
                    this.headerText = calendar.MonthAsPaddedNumericString(2);
                    this.contentText = calendar.MonthAsSoloString();
                    break;
                case DateTimeComponentType.Year:
                    this.headerText = calendar.YearAsPaddedString(4);
                    if (calendar.GetCalendarSystem() == Windows.Globalization.CalendarIdentifiers.Gregorian &&
                        DateTime.IsLeapYear(calendar.GetUtcDateTime().Year))
                    {
                        this.contentText = InputLocalizationManager.Instance.GetString("LeapYear");
                    }
                    else
                    {
                        this.contentText = " ";
                    }
                    break;
                case DateTimeComponentType.Hour:
                    this.headerText = calendar.HourAsString();
                    this.contentText = string.Empty;
                    break;
                case DateTimeComponentType.Minute:
                    this.headerText = calendar.MinuteAsPaddedString(2);
                    this.contentText = string.Empty;
                    break;
                case DateTimeComponentType.AMPM:
                    this.headerText = calendar.PeriodAsString();
                    this.contentText = string.Empty;
                    break;
            }

            this.OnPropertyChanged(nameof(this.HeaderText));
            this.OnPropertyChanged(nameof(this.ContentText));
        }
    }
}