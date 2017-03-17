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
    /// Represents a control that enables the user to select date values from a range by tapping on a picker box containing the current value and opening a date selector to select a new value.
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
    public class RadDatePicker : DateTimePicker
    {
        /// <summary>
        /// Identifies the <see cref="DayStepBehavior"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DayStepBehaviorProperty =
            DependencyProperty.Register(nameof(DayStepBehavior), typeof(StepBehavior), typeof(RadDatePicker), new PropertyMetadata(StepBehavior.Multiples, OnDayStepBehaviorChanged));

        /// <summary>
        /// Identifies the <see cref="MonthStepBehavior"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MonthStepBehaviorProperty =
            DependencyProperty.Register(nameof(MonthStepBehavior), typeof(StepBehavior), typeof(RadDatePicker), new PropertyMetadata(StepBehavior.Multiples, OnMonthStepBehaviorChanged));

        /// <summary>
        /// Identifies the <see cref="YearStepBehavior"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty YearStepBehaviorProperty =
            DependencyProperty.Register(nameof(YearStepBehavior), typeof(StepBehavior), typeof(RadDatePicker), new PropertyMetadata(StepBehavior.Multiples, OnYearStepBehaviorChanged));

        private const string DefaultSelectorFormat = "mdy";

        private const string DayListPartName = "PART_DayList";
        private const string MonthListPartName = "PART_MonthList";
        private const string YearListPartName = "PART_YearList";

        private DateTimeList dayList;
        private DateTimeList monthList;
        private DateTimeList yearList;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadDatePicker"/> class.
        /// </summary>
        /// <remarks>
        /// See the <see cref="DateTimePicker"/> class for the inherited members.
        /// </remarks>
        public RadDatePicker()
        {
            this.DefaultStyleKey = typeof(RadDatePicker);
            this.calendarValidator.DayStepBehavior = this.DayStepBehavior;
            this.calendarValidator.MonthStepBehavior = this.MonthStepBehavior;
            this.calendarValidator.YearStepBehavior = this.YearStepBehavior;
        }

        /// <summary>
        /// Gets or sets a value from the <see cref="StepBehavior"/> enumeration
        /// that defines how the day step is evaluated. See the descriptions of the different values
        /// exposed by the <see cref="StepBehavior"/> enum for further details.
        /// </summary>
        public StepBehavior DayStepBehavior
        {
            get
            {
                return (StepBehavior)this.GetValue(DayStepBehaviorProperty);
            }
            set
            {
                this.SetValue(DayStepBehaviorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value from the <see cref="StepBehavior"/> enumeration
        /// that defines how the month step is evaluated. See the descriptions of the different values
        /// exposed by the <see cref="StepBehavior"/> enum for further details.
        /// </summary>
        public StepBehavior MonthStepBehavior
        {
            get
            {
                return (StepBehavior)this.GetValue(MonthStepBehaviorProperty);
            }
            set
            {
                this.SetValue(MonthStepBehaviorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value from the <see cref="StepBehavior"/> enumeration
        /// that defines how the year step is evaluated. See the descriptions of the different values
        /// exposed by the <see cref="StepBehavior"/> enum for further details.
        /// </summary>
        public StepBehavior YearStepBehavior
        {
            get
            {
                return (StepBehavior)this.GetValue(YearStepBehaviorProperty);
            }
            set
            {
                this.SetValue(YearStepBehaviorProperty, value);
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadDatePickerAutomationPeer(this);
        }

        internal override string GetDisplayValueFormatFromCulture()
        {
            return System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern;
        }

        internal override string GetValueStringForNonGregorianCalendars(Windows.Globalization.Calendar calendar)
        {
            return string.Format("{0}/{1}/{2}", calendar.MonthAsNumericString(), calendar.DayAsString(), calendar.YearAsString());
        }

        internal override DateTime GetSelectorValueWithStepApplied(DateTime utcValue)
        {
            return this.calendarValidator.GetDateTimeValueWithDateStepApplied(utcValue);
        }

        internal override string GetDefaultSelectorOrder()
        {
            return DefaultSelectorFormat;
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

            int indexOfMonths = -1;
            int indexOfDays = -1;
            int indexOfYears = -1;
            int currentColumnIndex = 0;
            for (int i = 0; i < valueFormat.Length; i++)
            {
                char currentChar = valueFormat[i];

                if (currentChar == 'm' && indexOfMonths == -1)
                {
                    indexOfMonths = currentColumnIndex++;
                }

                if (currentChar == 'd' && indexOfDays == -1)
                {
                    indexOfDays = currentColumnIndex++;
                }

                if (currentChar == 'y' && indexOfYears == -1)
                {
                    indexOfYears = currentColumnIndex++;
                }
            }

            if (indexOfMonths > -1)
            {
                this.availableListsCount++;
                this.monthList.Visibility = Visibility.Visible;
                Grid.SetColumn(this.monthList, indexOfMonths);
            }
            else
            {
                this.monthList.Visibility = Visibility.Collapsed;
            }

            if (indexOfDays > -1)
            {
                this.availableListsCount++;
                this.dayList.Visibility = Visibility.Visible;
                Grid.SetColumn(this.dayList, indexOfDays);
            }
            else
            {
                this.dayList.Visibility = Visibility.Collapsed;
            }

            if (indexOfYears > -1)
            {
                this.availableListsCount++;
                this.yearList.Visibility = Visibility.Visible;
                Grid.SetColumn(this.yearList, indexOfYears);
            }
            else
            {
                this.yearList.Visibility = Visibility.Collapsed;
            }
        }

        internal override StepBehavior GetBehaviorForComponent(DateTimeComponentType dateTimeComponentType)
        {
            switch (dateTimeComponentType)
            {
                case DateTimeComponentType.Day: return this.DayStepBehavior;
                case DateTimeComponentType.Month: return this.MonthStepBehavior;
                case DateTimeComponentType.Year: return this.YearStepBehavior;
                default: return StepBehavior.Multiples;
            }
        }

        /// <summary>
        /// Resolves the control's template parts.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.dayList = this.GetTemplatePartField<DateTimeList>(DayListPartName);
            applied = applied && this.dayList != null;

            this.monthList = this.GetTemplatePartField<DateTimeList>(MonthListPartName);
            applied = applied && this.monthList != null;

            this.yearList = this.GetTemplatePartField<DateTimeList>(YearListPartName);
            applied = applied && this.yearList != null;

            return applied;
        }

        /// <summary>
        /// Resolves the control's template parts.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            this.RegisterDateTimeList(this.monthList);
            this.RegisterDateTimeList(this.dayList);
            this.RegisterDateTimeList(this.yearList);

            if (this.ReadLocalValue(DateTimePicker.SelectorHeaderProperty) == DependencyProperty.UnsetValue && this.SelectorHeader == null)
            {
                this.SelectorHeader = InputLocalizationManager.Instance.GetString("DateSelectorHeader");
            }
            if (this.ReadLocalValue(DateTimePicker.EmptyContentProperty) == DependencyProperty.UnsetValue && this.EmptyContent == null)
            {
                this.EmptyContent = InputLocalizationManager.Instance.GetString("DateEmptyContent");
            }

            base.OnTemplateApplied();
        }

        private static void OnDayStepBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var picker = (RadDatePicker)d;
            var stepBehavior = (StepBehavior)e.NewValue;
            picker.calendarValidator.DayStepBehavior = stepBehavior;
            if (picker.dayList != null)
            {
                picker.dayList.UpdateListWithStep();
            }
            var selectorValue = picker.GetSelectorValueWithStepApplied(picker.SelectorUtcValue);
            picker.UpdateDateTimeListsValue(selectorValue, null);
        }

        private static void OnMonthStepBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var picker = (RadDatePicker)d;
            var stepBehavior = (StepBehavior)e.NewValue;
            picker.calendarValidator.MonthStepBehavior = (StepBehavior)e.NewValue;
            var selectorValue = picker.GetSelectorValueWithStepApplied(picker.SelectorUtcValue);
            if (picker.monthList != null)
            {
                picker.monthList.UpdateListWithStep();
            }
            picker.UpdateDateTimeListsValue(selectorValue, null);
        }

        private static void OnYearStepBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var picker = (RadDatePicker)d;
            var stepBehavior = (StepBehavior)e.NewValue;
            picker.calendarValidator.YearStepBehavior = (StepBehavior)e.NewValue;
            if (picker.yearList != null)
            {
                picker.yearList.UpdateListWithStep();
            }
            var selectorValue = picker.GetSelectorValueWithStepApplied(picker.SelectorUtcValue);
            picker.UpdateDateTimeListsValue(selectorValue, null);
        }
    }
}