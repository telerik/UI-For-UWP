using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives.LoopingList;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Input.DateTimePickers
{
    /// <summary>
    /// A special <see cref="RadLoopingList"/> instance that visualizes DateTime values.
    /// </summary>
    [TemplatePart(Name = "PART_Panel", Type = typeof(LoopingPanel))]
    public class DateTimeList : RadLoopingList
    {
        /// <summary>
        /// Identifies the <see cref="Step"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register(nameof(Step), typeof(int), typeof(DateTimeList), new PropertyMetadata(1, OnStepChanged));

        // Timer is static as there is only one UI thread and one list focused at a time.
        private static DispatcherTimer typeDelayTimer;

        private DateTimeComponentType componentType;
        private bool updatingValue;
        private DateTimePicker owner;
        private string currentTypedInput;
        private CoreWindow characterHookedWindow;
        private DateTime currentListUtcValue;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "The static field needs additional initialization")]
        static DateTimeList()
        {
            typeDelayTimer = new DispatcherTimer();
            typeDelayTimer.Interval = TimeSpan.FromSeconds(1);
            typeDelayTimer.Tick += OnTypeDelayTimerTick;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeList"/> class.
        /// </summary>
        public DateTimeList()
        {
            this.DefaultStyleKey = typeof(DateTimeList);
            this.componentType = DateTimeComponentType.Day;
            this.currentListUtcValue = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            this.IsExpanded = false;
        }

        /// <summary>
        /// Gets or sets the component of the DateTime structure visualized by this list.
        /// </summary>
        public DateTimeComponentType ComponentType
        {
            get
            {
                return this.componentType;
            }
            set
            {
                if (this.componentType == value)
                {
                    return;
                }

                this.componentType = value;
                this.OnComponentTypeChanged();
            }
        }

        /// <summary>
        /// Gets or sets the time step. The time step
        /// determines the step between to possible selection values
        /// in the current <see cref="DateTimeList"/> instance.
        /// </summary>
        public int Step
        {
            get
            {
                return (int)this.GetValue(StepProperty);
            }
            set
            {
                this.SetValue(StepProperty, value);
            }
        }

        internal DateTimePicker Owner
        {
            get
            {
                return this.owner;
            }
        }

        internal DateTime UtcListValue
        {
            get
            {
                return this.currentListUtcValue;
            }
        }

        internal bool IsCharacterReceivedAttached
        {
            get
            {
                return this.characterHookedWindow != null;
            }
        }

        internal void Attach(DateTimePicker newOwner)
        {
            this.owner = newOwner;
            this.currentListUtcValue = newOwner.SelectorUtcValue;
            this.SetStepFromDateTime(this.owner.Step);
        }

        internal void Detach()
        {
            this.owner = null;
        }

        internal virtual void UpdateListWithStep()
        {
            int logicalCount = this.GetLogicalCountCore();

            if (this.Step <= 0 || this.Step > logicalCount)
            {
                throw new ArgumentException("Component Step must be positive and must not be greater than the original logical count of items.");
            }
            if (this.itemsPanel != null)
            {
                this.UpdateLogicalCount(false);
                this.itemsPanel.UpdateVisualItemCount();
                bool isLoopingEnabled = this.IsLoopingEnabled;
                bool newLoopingEnabledValue = this.GetCanLoop();
                if (isLoopingEnabled != newLoopingEnabledValue)
                {
                    this.IsLoopingEnabled = newLoopingEnabledValue;
                }
                else if (this.IsLoaded)
                {
                    this.itemsPanel.UpdateWheelCore(this.itemsPanel.visualOffset, true);
                }
                else
                {
                    this.itemsPanel.Reset();
                }
            }
        }
        internal void ChangeValue(DateTime newUtcValue, bool silently, bool userSelection)
        {
            if (this.updatingValue)
            {
                return;
            }

            this.updatingValue = silently;
            this.SetNewValue(newUtcValue, userSelection);
            this.updatingValue = false;
        }

        internal virtual void OnComponentTypeChanged()
        {
            if (this.owner != null)
            {
                this.SetStepFromDateTime(this.owner.Step);
            }
            else
            {
                this.Step = (int)StepProperty.GetMetadata(typeof(int)).DefaultValue;
            }

            this.IsLoopingEnabled = this.GetCanLoop();
        }

        internal void UpdateSelectedIndex(LoopingListSelectionChangeReason reason)
        {
            int index = this.GetIndexFromCurrentValueForComponentType(this.componentType);

            // Ensure that list is with valid count when swithcing between months with diferent number of days.
            if (this.IsTemplateApplied && this.itemsPanel.VisualCount > 0)
            {
                this.UpdateLogicalCount(false);
                this.UpdateListWithStep();
            }

            this.UpdateSelection(index, this.GetVisualIndex(index), reason);
        }

        internal override LoopingListItem CreateVisualItemInstance()
        {
            return new DateTimeListItem();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        internal DateTime GetDateTimeAt(int logicalIndex)
        {
            if (this.owner == null)
            {
                throw new InvalidOperationException();
            }

            int logicalIndexBasedOnStep = this.GetDateComponentValueFromLogicalIndexBasedOnStepBehavior(this.owner.GetBehaviorForComponent(this.componentType), logicalIndex);

            return this.owner.calendarValidator.CoerceDateTimeWithStep(logicalIndexBasedOnStep, this.componentType, this.currentListUtcValue);
        }

        internal bool IsRealChange(DateTime newValue)
        {
            // TO DO: shouldn't we remove this method?
            return newValue != this.currentListUtcValue;
        }

        internal void SetStepFromDateTime(DateTimeOffset date)
        {
            switch (this.componentType)
            {
                case DateTimeComponentType.Day:
                    this.Step = date.Day;
                    break;
                case DateTimeComponentType.Month:
                    this.Step = date.Month;
                    break;
                case DateTimeComponentType.Year:
                    this.Step = date.Year;
                    break;
                case DateTimeComponentType.Hour:
                    this.Step = date.Hour == 0 ? 1 : date.Hour;
                    break;
                case DateTimeComponentType.Minute:
                    this.Step = date.Minute == 0 ? 1 : date.Minute;
                    break;
                case DateTimeComponentType.Second:
                    this.Step = date.Second == 0 ? 1 : date.Second;
                    break;
                default:
                    this.Step = 1;
                    break;
            }
        }

        /// <inheritdoc />
        internal override void UpdateSelection(int newSelectedLogicalIndex, int newSelectedVisualIndex, LoopingListSelectionChangeReason reason)
        {
            if (reason == LoopingListSelectionChangeReason.PrivateNoNotify)
            {
                if (!this.CanChangeSelectedIndex(newSelectedLogicalIndex))
                {
                    newSelectedLogicalIndex++;
                }
            }

            if (reason == LoopingListSelectionChangeReason.VisualItemClick ||
                reason == LoopingListSelectionChangeReason.VisualItemSnappedToMiddle ||
                reason == LoopingListSelectionChangeReason.Private ||
                reason == LoopingListSelectionChangeReason.PrivateNoNotify)
            {
                this.UpdateValue(newSelectedLogicalIndex);
            }

            base.UpdateSelection(newSelectedLogicalIndex, newSelectedVisualIndex, reason);
        }

        /// <summary>
        /// Creates a new <see cref="LoopingListDataItem"/> instance.
        /// </summary>
        internal override LoopingListDataItem CreateDataItem(int logicalIndex)
        {
            return new DateTimeItem(this, this.GetDateTimeAt(logicalIndex));
        }

        /// <summary>
        /// Determines whether the logical item, representing the specified logical index should be enabled or not.
        /// </summary>
        /// <param name="logicalIndex">The logical index that defines the data to check for.</param>
        internal override bool IsItemEnabled(int logicalIndex)
        {
            DateTime dateTimeValue = this.GetDateTimeAt(logicalIndex);

            if (this.owner == null)
            {
                return false;
            }

            DateTime minValue = this.owner.calendarValidator.GetMinValueWithStep();
            DateTime maxValue = this.owner.calendarValidator.GetMaxValueWithStep();

            return dateTimeValue >= minValue && dateTimeValue <= maxValue;
        }

        /// <summary>
        /// Retrieves the logical count of the current data source (if any).
        /// </summary>
        internal override int GetLogicalCount()
        {
            int logicalCount = this.GetLogicalCountCore();
            int newLogicalCount = logicalCount / this.Step;
            var stepBehavior = this.owner.GetBehaviorForComponent(this.componentType);
            if (stepBehavior != StepBehavior.Multiples)
            {
                if (stepBehavior == StepBehavior.StartFromBase)
                {
                    newLogicalCount += logicalCount % this.Step != 0 ? 1 : 0;
                }
                else if (this.Step != 1)
                {
                    newLogicalCount += 1;
                }
            }

            return newLogicalCount;
        }

        /// <summary>
        /// Allows to minimize the overhead of creating new instances whenever a logical index changes by formatting an existing data item instance.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Method is internal")]
        internal override void UpdateDataItem(LoopingListDataItem dataItem, LoopingListItem visualItem)
        {
            DateTimeItem item = dataItem as DateTimeItem;
            DateTime date = this.GetDateTimeAt(visualItem.LogicalIndex);
            item.Update(date);

            base.UpdateDataItem(dataItem, visualItem);
        }

        /// <summary>
        /// Attempts to parse the provided input to an integer number and to map it to a logical index. Exposed as internal for testing purposes.
        /// </summary>
        internal bool TrySelectTypedInput(string input)
        {
            // TODO: How to select a AMP/PM designator?
            if (this.componentType == DateTimeComponentType.AMPM)
            {
                return false;
            }

            if (string.IsNullOrEmpty(input))
            {
                Debug.Assert(RadControl.IsInTestMode, "Must have typed input at this point.");
                return false;
            }

            int newIndex;
            int maxIndex;

            if (!int.TryParse(input, out newIndex))
            {
                return false;
            }

            newIndex = this.ShiftTypedValueToZeroBase(newIndex);
            maxIndex = this.GetLogicalCountCore() == 12 ? 12 : this.GetLogicalCountCore() - 1;
            newIndex = Math.Min(newIndex, maxIndex);
            newIndex = Math.Max(0, newIndex);

            DateTime newValue = this.GetDateTimeAt(newIndex);
            newValue = this.owner.calendarValidator.CoerceDateTime(newValue);
            newValue = this.owner.GetSelectorValueWithStepApplied(newValue);

            int prevIndex = this.SelectedIndex;

            this.ChangeValue(newValue, false, false);

            return prevIndex != newIndex;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate"/> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.UpdateSelectedIndex(LoopingListSelectionChangeReason.PrivateNoNotify);
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Loaded"/> event. Allows inheritors to provide their specific logic.
        /// </summary>
        protected override void LoadCore()
        {
            base.LoadCore();

            if (this.IsTemplateApplied)
            {
                bool newLoopingEnabledValue = this.GetCanLoop();
                if (this.IsLoopingEnabled != newLoopingEnabledValue)
                {
                    this.IsLoopingEnabled = newLoopingEnabledValue;
                }
            }
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Unloaded" /> event. Allows inheritors to provide their specific logic.
        /// </summary>
        protected override void UnloadCore()
        {
            base.UnloadCore();

            this.ResetCurrentTypedInput();
            this.DetachCharacterReceived();
        }

        /// <summary>
        /// Called before the GotFocus event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            this.DetachCharacterReceived();

            this.characterHookedWindow = CoreWindow.GetForCurrentThread();
            if (this.characterHookedWindow != null)
            {
                this.characterHookedWindow.CharacterReceived += this.OnCharacterReceived;
            }
        }

        /// <summary>
        /// Called before the LostFocus event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            this.ResetCurrentTypedInput();
            this.DetachCharacterReceived();
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DateTimeListAutomationPeer(this);
        }

        private static void OnStepChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            DateTimeList list = (DateTimeList)sender;

            if (!list.IsTemplateApplied)
            {
                return;
            }

            list.UpdateListWithStep();
        }

        private static void OnTypeDelayTimerTick(object sender, object e)
        {
            typeDelayTimer.Stop();

            DateTimeList focusedList = FocusManager.GetFocusedElement() as DateTimeList;
            if (focusedList != null)
            {
                focusedList.ResetCurrentTypedInput();
            }
        }

        private int GetLogicalIndexFromComponentValueWithoutStepApplied(DateTimeComponentType type)
        {
            var calendar = this.owner.calendarValidator.Calendar;
            calendar.SetDateTime(this.currentListUtcValue);

            switch (type)
            {
                case DateTimeComponentType.Day:
                    return calendar.Day - calendar.FirstDayInThisMonth + 1;
                case DateTimeComponentType.Month:
                    return calendar.Month - calendar.FirstMonthInThisYear + 1;
                case DateTimeComponentType.Year:
                    return calendar.Year - calendar.FirstYearInThisEra + 1;
                case DateTimeComponentType.Hour:
                    return (calendar.NumberOfPeriodsInThisDay == 2 && calendar.Hour == 12) ? 0 : calendar.Hour;
                case DateTimeComponentType.Minute:
                    return calendar.Minute;
                case DateTimeComponentType.AMPM:
                    // 0 if AM, 1 if PM
                    return calendar.Period - 1;
                default:
                    return 0;
            }
        }

        private void OnCharacterReceived(CoreWindow sender, CharacterReceivedEventArgs args)
        {
            typeDelayTimer.Stop();
            typeDelayTimer.Start();

            char virtKeyChar = Convert.ToChar(args.KeyCode);
            if (char.IsDigit(virtKeyChar))
            {
                this.currentTypedInput += virtKeyChar;
                args.Handled = this.TrySelectTypedInput(this.currentTypedInput);
            }
        }

        private void SetNewValue(DateTime newUtcValue, bool selectionChanged)
        {
            if (newUtcValue.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException();
            }

            var newValueFromStep = this.owner.GetSelectorValueWithStepApplied(newUtcValue);

            if (!this.IsRealChange(newValueFromStep))
            {
                return;
            }

            var oldValue = this.currentListUtcValue;
            this.currentListUtcValue = newValueFromStep;

            if (!selectionChanged)
            {
                var reason = this.updatingValue ? LoopingListSelectionChangeReason.PrivateNoNotify : LoopingListSelectionChangeReason.Private;
                this.UpdateSelectedIndex(reason);
            }

            if (this.UpdateLogicalCount(true))
            {
                this.CenterCurrentItem(false);
            }
            else
            {
                this.UpdateData(true);
            }

            this.owner.UpdateDateTimeListsValue(this.currentListUtcValue, this);
        }

        private bool GetCanLoop()
        {
            if (!this.IsTemplateApplied)
            {
                return true;
            }

            return this.GetLogicalCount() > this.itemsPanel.VisualCount - 2;
        }

        private void UpdateValue(int selectedIndex)
        {
            if (this.updatingValue)
            {
                return;
            }

            this.ChangeValue(this.GetDateTimeAt(selectedIndex), true, true);
        }

        private int GetLogicalCountCore()
        {
            var calendar = this.owner.calendarValidator.Calendar;
            calendar.SetDateTime(this.currentListUtcValue);
            switch (this.componentType)
            {
                case DateTimeComponentType.Year:
                    return calendar.NumberOfYearsInThisEra;
                case DateTimeComponentType.Month:
                    return calendar.NumberOfMonthsInThisYear;
                case DateTimeComponentType.Day:
                    return calendar.NumberOfDaysInThisMonth;
                case DateTimeComponentType.Hour:
                    return calendar.NumberOfHoursInThisPeriod;
                case DateTimeComponentType.Minute:
                    return calendar.NumberOfMinutesInThisHour;
                case DateTimeComponentType.AMPM:
                    return calendar.NumberOfPeriodsInThisDay;
                default:
                    return 0;
            }
        }

        private int GetDateComponentValueFromLogicalIndexBasedOnStepBehavior(StepBehavior behavior, int logicalIndex)
        {
            switch (this.componentType)
            {
                case DateTimeComponentType.Day:
                case DateTimeComponentType.Month:
                case DateTimeComponentType.Year:
                    switch (behavior)
                    {
                        case StepBehavior.Multiples:
                            return (logicalIndex + 1) * this.Step;
                        case StepBehavior.BaseAndMultiples:
                            return logicalIndex == 0 || this.Step == 1 ? logicalIndex + 1 : logicalIndex * this.Step;
                        case StepBehavior.StartFromBase:
                            return (logicalIndex * this.Step) + 1;
                        default:
                            return logicalIndex * this.Step;
                    }
                case DateTimeComponentType.Minute:
                case DateTimeComponentType.Hour:
                case DateTimeComponentType.AMPM:
                    return logicalIndex * this.Step;
            }

            return logicalIndex;
        }

        private int GetIndexFromCurrentValueForComponentType(DateTimeComponentType type)
        {
            int index = 0;
            int componentValue = this.GetLogicalIndexFromComponentValueWithoutStepApplied(type);
            var stepBehavior = this.owner.GetBehaviorForComponent(this.componentType);
            switch (this.componentType)
            {
                case DateTimeComponentType.Day:
                case DateTimeComponentType.Month:
                case DateTimeComponentType.Year:

                    if (this.Step == 1)
                    {
                        return componentValue - 1;
                    }

                    if (stepBehavior == StepBehavior.Multiples)
                    {
                        index = Math.Max((componentValue / this.Step) - 1, 0);
                    }
                    else
                    {
                        index = Math.Min(componentValue / this.Step, this.GetLogicalCount() - 1);
                    }

                    break;
                case DateTimeComponentType.Hour:
                    index = componentValue / this.Step;
                    break;
                case DateTimeComponentType.Minute:
                    index = componentValue / this.Step;
                    break;
                case DateTimeComponentType.AMPM:
                    // 0 if AM, 1 if PM
                    return componentValue;
            }

            return index;
        }

        private int ShiftTypedValueToZeroBase(int value)
        {
            switch (this.componentType)
            {
                case DateTimeComponentType.Day:
                case DateTimeComponentType.Month:
                case DateTimeComponentType.Year:
                    value--;
                    break;
                case DateTimeComponentType.Hour:
                    if (this.GetLogicalCountCore() == 12)
                    {
                        if (value == 12)
                        {
                            value = 0;
                        }
                    }
                    break;
            }

            return value;
        }

        private void ResetCurrentTypedInput()
        {
            typeDelayTimer.Stop();
            this.currentTypedInput = string.Empty;
        }

        private void DetachCharacterReceived()
        {
            if (this.characterHookedWindow != null)
            {
                this.characterHookedWindow.CharacterReceived -= this.OnCharacterReceived;
                this.characterHookedWindow = null;
            }
        }
    }
}