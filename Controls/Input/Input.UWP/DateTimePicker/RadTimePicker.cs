using System;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Input.DateTimePickers;
using Telerik.UI.Xaml.Controls.Primitives.LoopingList;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents a control that enables the user to select time values from a range by tapping
    /// on a picker box containing the current value and opening a time selector to select a new value.
    /// </summary>
    [TemplatePart(Name = "PART_PickerButton", Type = typeof(DateTimePickerButton))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_SelectorLayoutRoot", Type = typeof(Border))]
    [TemplatePart(Name = "PART_SelectorHeader", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_MonthList", Type = typeof(DateTimeList))]
    [TemplatePart(Name = "PART_DayList", Type = typeof(DateTimeList))]
    [TemplatePart(Name = "PART_YearList", Type = typeof(DateTimeList))]
    [TemplatePart(Name = "PART_SelectorButtonsPanel", Type = typeof(StackPanel))]
    [TemplatePart(Name = "PART_SelectorOKButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_SelectorCancelButton", Type = typeof(Button))]
    public class RadTimePicker : DateTimePicker
    {
        /// <summary>
        /// Gets or sets a value that specifies whether the clock will be 12 or 24-hour. The value should be taken from <see cref="Windows.Globalization.ClockIdentifiers"/>.
        /// </summary>
        public static readonly DependencyProperty CalendarClockIdentifierProperty =
            DependencyProperty.Register(nameof(CalendarClockIdentifier), typeof(string), typeof(DateTimePicker), new PropertyMetadata(ClockIdentifiers.TwelveHour, OnCalendarClockIdentifierChanged));
        
        private const string DefaultSelectorFormat = "hmt";

        private const string HourListPartName = "PART_HourList";
        private const string MinuteListPartName = "PART_MinuteList";
        private const string AMPPMListPartName = "PART_AMPMList";

        private DateTimeList hourList;
        private DateTimeList minuteList;
        private DateTimeList ampmList;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadTimePicker"/> class.
        /// </summary>
        /// <remarks>
        /// See the <see cref="DateTimePicker"/> class for the inherited members.
        /// </remarks>
        public RadTimePicker()
        {
            this.DefaultStyleKey = typeof(RadTimePicker);
        }

        /// <summary>
        /// Gets or sets a value that specifies whether the clock will be 12 or 24-hour. The value should be from the <see cref="Windows.Globalization.ClockIdentifiers"/> class.
        /// </summary>
        public string CalendarClockIdentifier
        {
            get
            {
                return (string)this.GetValue(CalendarClockIdentifierProperty);
            }
            set
            {
                this.SetValue(CalendarClockIdentifierProperty, value);
            }
        }

        internal override string GetDisplayValueFormatFromCulture()
        {
            return System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern.Replace(":ss", string.Empty);
        }

        internal override string GetValueStringForNonGregorianCalendars(Windows.Globalization.Calendar calendar)
        {
            var valueString = string.Format("{0}:{1}", calendar.HourAsPaddedString(2), calendar.MinuteAsPaddedString(2));
            if (calendar.GetClock() == Windows.Globalization.ClockIdentifiers.TwelveHour)
            {
                valueString = string.Format("{0}{1}", valueString, calendar.PeriodAsString());
            }

            return valueString;
        }

        internal override DateTime GetSelectorValueWithStepApplied(DateTime utcValue)
        {
            return this.calendarValidator.GetDateTimeValueWithTimeStepApplied(utcValue);
        }

        /// <summary>
        /// Updates the order (that is the grid column) of each selector, depending on the current Culture and SelectorOrder.
        /// </summary>
        internal override void UpdateSelectorsOrder()
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            string valueFormat = this.ComposeSelectorsOrderString();

            valueFormat = valueFormat.ToLower();

            int indexOfHours = -1;
            int indexOfMinutes = -1;
            int indexOfAMPM = -1;
            int currentColumnIndex = 0;
            for (int i = 0; i < valueFormat.Length; i++)
            {
                char currentChar = valueFormat[i];

                if (currentChar == 'h' && indexOfHours == -1)
                {
                    indexOfHours = currentColumnIndex++;
                }

                if (currentChar == 'm' && indexOfMinutes == -1)
                {
                    indexOfMinutes = currentColumnIndex++;
                }

                if (currentChar == 't' && indexOfAMPM == -1)
                {
                    indexOfAMPM = currentColumnIndex++;
                }
            }

            if (indexOfHours > -1)
            {
                this.availableListsCount++;
                this.hourList.Visibility = Visibility.Visible;
                Grid.SetColumn(this.hourList, indexOfHours);
            }
            else
            {
                this.hourList.Visibility = Visibility.Collapsed;
            }

            if (indexOfMinutes > -1)
            {
                this.availableListsCount++;
                this.minuteList.Visibility = Visibility.Visible;
                Grid.SetColumn(this.minuteList, indexOfMinutes);
            }
            else
            {
                this.minuteList.Visibility = Visibility.Collapsed;
            }

            if (indexOfAMPM > -1 && this.calendarValidator.Calendar.GetClock() == ClockIdentifiers.TwelveHour)
            {
                this.availableListsCount++;
                this.ampmList.Visibility = Visibility.Visible;
                Grid.SetColumn(this.ampmList, indexOfAMPM);
            }
            else
            {
                this.ampmList.Visibility = Visibility.Collapsed;
            }
        }

        internal override string GetDefaultSelectorOrder()
        {
            return DefaultSelectorFormat;
        }

        internal override StepBehavior GetBehaviorForComponent(DateTimeComponentType dateTimeComponentType)
        {
            return StepBehavior.Multiples;
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadTimePickerAutomationPeer(this);
        }
        
        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate"/> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            this.RegisterDateTimeList(this.hourList);
            this.RegisterDateTimeList(this.minuteList);
            this.RegisterDateTimeList(this.ampmList);

            if (this.ReadLocalValue(DateTimePicker.SelectorHeaderProperty) == DependencyProperty.UnsetValue && this.SelectorHeader == null)
            {
                this.SelectorHeader = InputLocalizationManager.Instance.GetString("TimeSelectorHeader");
            }
            if (this.ReadLocalValue(DateTimePicker.EmptyContentProperty) == DependencyProperty.UnsetValue && this.EmptyContent == null)
            {
                this.EmptyContent = InputLocalizationManager.Instance.GetString("TimeEmptyContent");
            }

            base.OnTemplateApplied();
        }

        /// <summary>
        /// Resolves the control's template parts.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.hourList = this.GetTemplatePartField<DateTimeList>(HourListPartName);
            applied = applied && this.hourList != null;

            this.minuteList = this.GetTemplatePartField<DateTimeList>(MinuteListPartName);
            applied = applied && this.minuteList != null;

            this.ampmList = this.GetTemplatePartField<DateTimeList>(AMPPMListPartName);
            applied = applied && this.ampmList != null;

            return applied;
        }

        private static void OnCalendarClockIdentifierChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var picker = sender as RadTimePicker;
            picker.calendarValidator.ChangeClock((string)args.NewValue);

            picker.UpdateSelectorsOrder();

            foreach (var list in picker.dateTimeLists)
            {
                list.UpdateData(true);
                list.UpdateSelectedIndex(LoopingListSelectionChangeReason.Private);
            }

            picker.UpdateValueString();
        }
    }
}