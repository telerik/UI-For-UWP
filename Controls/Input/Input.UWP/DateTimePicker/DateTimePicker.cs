using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives.LoopingList;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// This class represents a control that provides functionality for choosing
    /// dates or times. It supports value ranges and value formats.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors")]
    public abstract partial class DateTimePicker : RadHeaderedControl
    {
        /// <summary>
        /// Identifies the <see cref="Step"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register(nameof(Step), typeof(DateTimeOffset), typeof(DateTimePicker), new PropertyMetadata(new DateTimeOffset(1, 1, 1, 1, 1, 1, TimeSpan.Zero), OnStepChanged));

        /// <summary>
        /// Identifies the <see cref="DisplayMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register(nameof(DisplayMode), typeof(DateTimePickerDisplayMode), typeof(DateTimePicker), new PropertyMetadata(DateTimePickerDisplayMode.Standard, OnDisplayModeChanged));

        /// <summary>
        /// Identifies the IsReadOnly dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(DateTimePicker), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="MinValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(nameof(MinValue), typeof(DateTime), typeof(DateTimePicker), new PropertyMetadata(DateTime.MinValue, OnMinValueChanged));

        /// <summary>
        /// Identifies the <see cref="MaxValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(nameof(MaxValue), typeof(DateTime), typeof(DateTimePicker), new PropertyMetadata(DateTime.MaxValue, OnMaxValueChanged));

        /// <summary>
        /// Identifies the <see cref="ValueString"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueStringProperty =
            DependencyProperty.Register(nameof(ValueString), typeof(string), typeof(DateTimePicker), new PropertyMetadata(string.Empty, OnValueStringChanged));

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(DateTimePicker), new PropertyMetadata(null, OnValueChanged));

        /// <summary>
        /// Identifies the <see cref="DisplayValueFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayValueFormatProperty =
            DependencyProperty.Register(nameof(DisplayValueFormat), typeof(string), typeof(DateTimePicker), new PropertyMetadata(string.Empty, OnDisplayValueFormatChanged));

        /// <summary>
        /// Identifies the <see cref="EmptyContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EmptyContentProperty =
            DependencyProperty.Register(nameof(EmptyContent), typeof(object), typeof(DateTimePicker), new PropertyMetadata(null, OnEmptyContentChanged));

        /// <summary>
        /// Identifies the <see cref="EmptyContentTemplateProperty"/>.
        /// </summary>
        public static readonly DependencyProperty EmptyContentTemplateProperty =
            DependencyProperty.Register(nameof(EmptyContentTemplate), typeof(DataTemplate), typeof(DateTimePicker), new PropertyMetadata(null, OnEmptyContentTemplateChanged));

        /// <summary>
        /// Identifies the <see cref="AutoSizeWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoSizeWidthProperty =
            DependencyProperty.Register(nameof(AutoSizeWidth), typeof(bool), typeof(DateTimePicker), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="CalendarIdentifier"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CalendarIdentifierProperty =
            DependencyProperty.Register(nameof(CalendarIdentifier), typeof(string), typeof(DateTimePicker), new PropertyMetadata(CalendarIdentifiers.Gregorian, OnCalendarIdentifierChanged));

        /// <summary>
        /// Identifies the <see cref="CalendarLanguage"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CalendarLanguageProperty =
            DependencyProperty.Register(nameof(CalendarLanguage), typeof(string), typeof(DateTimePicker), new PropertyMetadata(Windows.Globalization.ApplicationLanguages.Languages[0], OnCalendarLanguageChanged));

        /// <summary>
        /// Identifies the <see cref="CalendarNumeralSystem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CalendarNumeralSystemProperty =
            DependencyProperty.Register(nameof(CalendarNumeralSystem), typeof(string), typeof(DateTimePicker), new PropertyMetadata(NumeralSystemIdentifiers.Latn, OnCalendarNumeralSystemChanged));

        internal CalendarValidator calendarValidator;

        private DateTime utcValue;
        private DateTimePickerDisplayMode displayModeCache = DateTimePickerDisplayMode.Standard;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimePicker"/> class.
        /// </summary>
        protected DateTimePicker()
        {
            var languages = new List<string> { this.CalendarLanguage };
            var calendar = new Windows.Globalization.Calendar(languages, this.CalendarIdentifier, ClockIdentifiers.TwelveHour);
            this.calendarValidator = new CalendarValidator(calendar);
            this.selectorUtcValue = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            this.utcValue = this.selectorUtcValue;
            Window.Current.SizeChanged += this.Current_SizeChanged;
        }

        /// <summary>
        /// This event is raised when the <see cref="Value"/> property has changed.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Gets or sets the mode that defines how the component is displayed within the visual tree.
        /// The default value is <see cref="DateTimePickerDisplayMode.Standard"/>.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadDatePicker x:Name="datePicker" DisplayMode="Standard"/&gt;
        /// </code>
        /// <code language="c#">
        /// datePicker.DisplayMode = DateTimePickerDisplayMode.Standard;
        /// </code>
        /// </example>
        /// <seealso cref="DateTimePickerDisplayMode"/>
        public DateTimePickerDisplayMode DisplayMode
        {
            get
            {
                return this.displayModeCache;
            }
            set
            {
                this.SetValue(DisplayModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the calendar numeral system.
        /// The default value is <see cref="Windows.Globalization.NumeralSystemIdentifiers.Latn"/>.
        /// </summary>
        public string CalendarNumeralSystem
        {
            get
            {
                return (string)this.GetValue(CalendarNumeralSystemProperty);
            }
            set
            {
                this.SetValue(CalendarNumeralSystemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the calendar identifier.
        /// The default value is <see cref="Windows.Globalization.CalendarIdentifiers.Gregorian"/>.
        /// </summary>
        public string CalendarIdentifier
        {
            get
            {
                return (string)this.GetValue(CalendarIdentifierProperty);
            }
            set
            {
                this.SetValue(CalendarIdentifierProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the calendar language.
        /// The default value is <see cref="Windows.Globalization.Language.CurrentInputMethodLanguageTag"/>.
        /// </summary>
        public string CalendarLanguage
        {
            get
            {
                return (string)this.GetValue(CalendarLanguageProperty);
            }
            set
            {
                this.SetValue(CalendarLanguageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="DateTimeOffset"/> struct
        /// that represents the date or time amount used as an interval
        /// to create the selectable options in the date/time component selectors.
        /// </summary>
        public DateTimeOffset Step
        {
            get
            {
                return (DateTimeOffset)this.GetValue(StepProperty);
            }
            set
            {
                this.SetValue(StepProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control will automatically calculate its
        /// width so that it equals the width of the Selector part. The default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// Default value: <c>true</c>
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadDatePicker x:Name="datePicker" AutoSizeWidth="False"/&gt;
        /// </code>
        /// <code language="c#">
        /// datePicker.AutoSizeWidth = false;
        /// </code>
        /// </example>
        public bool AutoSizeWidth
        {
            get
            {
                return (bool)this.GetValue(AutoSizeWidthProperty);
            }
            set
            {
                this.SetValue(AutoSizeWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is in
        /// read only mode. If set to true, the control does not allow the user
        /// to modify its value.
        /// </summary>
        /// <value>
        /// Default value: <c>false</c>.
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadDatePicker x:Name="datePicker" IsReadOnly="True"/&gt;
        /// </code>
        /// <code language="c#">
        /// datePicker.IsReadOnly = true;
        /// </code>
        /// </example>
        public bool IsReadOnly
        {
            get
            {
                return (bool)this.GetValue(IsReadOnlyProperty);
            }
            set
            {
                this.SetValue(IsReadOnlyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the null text of the control. The null text is displayed when there is no value defined.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadDatePicker x:Name="datePicker" EmptyContent="No date is selected."/&gt;
        /// </code>
        /// <code language="c#">
        /// datePicker.EmptyContent = "No date is selected.";
        /// </code>
        /// </example>
        public object EmptyContent
        {
            get
            {
                return this.GetValue(EmptyContentProperty);
            }
            set
            {
                this.SetValue(EmptyContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> defining the visual appearance of the empty content of the control.
        /// </summary>
        /// <value>The empty content <see cref="DataTemplate"/>.</value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadDatePicker&gt;
        ///     &lt;telerikInput:RadDatePicker.EmptyContentTemplate&gt;
        ///         &lt;DataTemplate&gt;
        ///             &lt;TextBlock Text="No content" Foreground="Red" FontWeight="Bold"/&gt;
        ///         &lt;/DataTemplate&gt;
        ///     &lt;/telerikInput:RadDatePicker.EmptyContentTemplate&gt;
        /// &lt;/telerikInput:RadDatePicker&gt;
        /// </code>
        /// </example>
        public DataTemplate EmptyContentTemplate
        {
            get
            {
                return this.GetValue(EmptyContentTemplateProperty) as DataTemplate;
            }
            set
            {
                this.SetValue(EmptyContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets the string representation of the current value. The string representation is
        /// the current value formatted according to the settings of the device and the control.
        /// </summary>
        /// <value>
        /// <para>
        /// Default value: <c>null</c>
        /// </para>
        /// <para>
        /// When no date is selected, the text displayed in the button part is defined by the
        /// <see cref="EmptyContent"/> or the <see cref="EmptyContentTemplate"/> properties.
        /// </para>
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadDatePicker x:Name="datePicker"/&gt;
        /// </code>
        /// <code language="c#">
        /// datePicker.Value = new DateTime(2013,1,1);
        /// string date = datePicker.ValueString;// "1/1/2013"
        /// </code>
        /// </example>
        public string ValueString
        {
            get
            {
                return (string)this.GetValue(ValueStringProperty);
            }
        }

        /// <summary>
        /// Gets or sets the maximum value of the <see cref="Value"/> property range for the control.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadDatePicker x:Name="datePicker"/&gt;
        /// </code>
        /// <code language="c#">
        /// datePicker.MaxValue = new DateTime(2014, 1, 1);
        /// </code>
        /// </example>
        public DateTime MaxValue
        {
            get
            {
                return (DateTime)this.GetValue(MaxValueProperty);
            }
            set
            {
                this.SetValue(MaxValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum value of the <see cref="Value"/> property range for the control.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadDatePicker x:Name="datePicker"/&gt;
        /// </code>
        /// <code language="c#">
        /// datePicker.MaxValue = new DateTime(2012, 1, 1);
        /// </code>
        /// </example>
        public DateTime MinValue
        {
            get
            {
                return (DateTime)this.GetValue(MinValueProperty);
            }
            set
            {
                this.SetValue(MinValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the date value.
        /// </summary>
        /// <value>
        /// The default value is <c>null</c> or defined by the <see cref="EmptyContent"/> or <see cref="EmptyContentTemplate"/> properties.
        /// </value>
        /// <remarks>
        /// The range of the value is defined by the <see cref="MinValue"/> and <see cref="MaxValue"/> properties.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadDatePicker x:Name="datePicker"/&gt;
        /// </code>
        /// <code language="c#">
        /// datePicker.Value = new DateTime(2013, 1, 1);
        /// </code>
        /// </example>
        [SuppressMessage("Microsoft.Naming", "CA1721")]
        public DateTime? Value
        {
            get
            {
                return (DateTime?)this.GetValue(ValueProperty);
            }
            set
            {
                this.SetValue(ValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the format for the string being displayed
        /// The default value is <see cref="String.Empty"/>.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadDatePicker x:Name="datePicker" DisplayValueFormat="dd-MM-yyyy"/&gt;
        /// </code>
        /// <code language="c#">
        /// datePicker.DisplayValueFormat = "dd-MM-yyyy";
        /// </code>
        /// </example>
        public string DisplayValueFormat
        {
            get
            {
                return (string)this.GetValue(DisplayValueFormatProperty);
            }
            set
            {
                this.SetValue(DisplayValueFormatProperty, value);
            }
        }

        internal DateTime UtcValue
        {
            get
            {
                return this.utcValue;
            }
        }

        internal void UpdateValueString()
        {
            if (!this.Value.HasValue)
            {
                this.SetValue(ValueStringProperty, string.Empty);
                return;
            }

            this.SetValue(ValueStringProperty, this.GetValueStringFromCalendarCore());
        }

        internal abstract StepBehavior GetBehaviorForComponent(DateTimeComponentType dateTimeComponentType);

        /// <summary>
        /// Provides the behavior for the Measure pass of the layout cycle.
        /// </summary>
        protected override Size MeasureOverride(Size availableSize)
        {
            bool isInline = false;

#if WINDOWS_PHONE_APP
            if (this.ReadLocalValue(ItemLengthProperty) == DependencyProperty.UnsetValue)
            {
                var listsCount = 3;
                this.ItemLength = (Window.Current.Bounds.Width - 12 * this.ItemSpacing) / listsCount;
            }
#else
            isInline = this.DisplayMode == DateTimePickerDisplayMode.Inline;

            if (isInline)
            {
                if (double.IsInfinity(availableSize.Height))
                {
                    availableSize.Height = this.GetSelectorSize().Height;
                }
                else if (this.VerticalAlignment != VerticalAlignment.Stretch)
                {
                    double selectorHeight = this.GetSelectorSize().Height;
                    if (selectorHeight < availableSize.Height)
                    {
                        availableSize.Height = selectorHeight;
                    }
                }
            }
#endif

            Size baseSize = base.MeasureOverride(availableSize);

            if (!isInline && this.AutoSizeWidth)
            {
                double popupWidth = this.GetSelectorWidth(true);
                if (popupWidth > baseSize.Width && popupWidth <= availableSize.Width)
                {
                    baseSize.Width = popupWidth;
                }
            }

            return baseSize;
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DateTimePickerAutomationPeer(this);
        }

        private static void OnCalendarNumeralSystemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var picker = sender as DateTimePicker;
            picker.calendarValidator.ChangeNumeralSystem((string)args.NewValue);

            foreach (var list in picker.dateTimeLists)
            {
                list.UpdateData(true);
            }

            picker.UpdateValueString();
        }

        private static void OnCalendarIdentifierChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var picker = sender as DateTimePicker;
            picker.calendarValidator.ChangeCalendarSystem((string)args.NewValue);
            picker.CoerceDateTimes();

            foreach (var list in picker.dateTimeLists)
            {
                list.UpdateData(true);
                list.UpdateSelectedIndex(LoopingListSelectionChangeReason.Private);
            }

            picker.UpdateValueString();
        }

        private static void OnCalendarLanguageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var picker = sender as DateTimePicker;
            var languages = new List<string> { (string)args.NewValue };
            picker.calendarValidator.ChangeLanguage(languages);

            foreach (var list in picker.dateTimeLists)
            {
                list.UpdateData(true);
            }

            picker.UpdateValueString();
        }

        private static void OnValueChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            DateTimePicker picker = sender as DateTimePicker;

            var newValue = (DateTime?)args.NewValue;

            if (newValue.HasValue && newValue != null)
            {
                picker.utcValue = DateTime.SpecifyKind(newValue.Value, DateTimeKind.Utc);
            }
            else
            {
                picker.utcValue = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            }

            if (picker.IsInternalPropertyChange)
            {
                return;
            }

            var oldValue = (DateTime?)args.OldValue;

            picker.HandleValueChange(oldValue, newValue);

            if (AutomationPeer.ListenerExists(AutomationEvents.PropertyChanged))
            {
                var peer = FrameworkElementAutomationPeer.FromElement(picker) as DateTimePickerAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseSelectionAutomationEvent(oldValue.ToString(), newValue.ToString());
                }
            }
        }

        private static void OnMinValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            DateTimePicker picker = sender as DateTimePicker;
            var newValue = (DateTime)args.NewValue;
            var isCoerced = !picker.calendarValidator.SetCoerceMinValue(DateTime.SpecifyKind(newValue, DateTimeKind.Utc));

            if (picker.IsInternalPropertyChange)
            {
                return;
            }

            if (isCoerced)
            {
                var coercedValue = picker.calendarValidator.GetMinValue();
                picker.ChangePropertyInternally(MinValueProperty, coercedValue);
            }

            picker.CheckCoerceValue();
        }

        private static void OnMaxValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            DateTimePicker picker = sender as DateTimePicker;
            var newValue = (DateTime)args.NewValue;
            var isCoerced = !picker.calendarValidator.SetCoerceMaxValue(DateTime.SpecifyKind(newValue, DateTimeKind.Utc));

            if (picker.IsInternalPropertyChange)
            {
                return;
            }

            if (isCoerced)
            {
                var coercedValue = picker.calendarValidator.GetMaxValue();
                picker.ChangePropertyInternally(MaxValueProperty, coercedValue);
            }

            picker.CheckCoerceValue();
        }

        private static void OnSelectorOrderPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var picker = sender as DateTimePicker;
            picker.UpdateSelectorsOrder();
        }

        private static void OnStepChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var picker = sender as DateTimePicker;
            picker.calendarValidator.Step = (DateTimeOffset)args.NewValue;

            foreach (var list in picker.dateTimeLists)
            {
                list.SetStepFromDateTime((DateTimeOffset)args.NewValue);
                list.UpdateSelectedIndex(LoopingListSelectionChangeReason.PrivateNoNotify);
            }
        }

        private static void OnValueStringChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            DateTimePicker picker = sender as DateTimePicker;
            if (picker.IsTemplateApplied)
            {
                picker.SyncPickerButtonContent();
            }
        }

        private static void OnEmptyContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            DateTimePicker picker = sender as DateTimePicker;
            picker.SyncPickerButtonContent();
        }

        private static void OnEmptyContentTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            DateTimePicker picker = sender as DateTimePicker;
            picker.SyncPickerButtonContent();
        }

        private static void OnDisplayValueFormatChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            DateTimePicker picker = sender as DateTimePicker;
            picker.UpdateValueString();
        }

        private static void OnDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if !WINDOWS_PHONE_APP
            DateTimePicker picker = d as DateTimePicker;
            picker.displayModeCache = (DateTimePickerDisplayMode)e.NewValue;
            if (!picker.IsTemplateApplied)
            {
                return;
            }
            picker.UpdateDisplayMode();
#endif
        }

        private void CoerceDateTimes()
        {
            this.utcValue = this.calendarValidator.CoerceDateTime(this.utcValue);
            this.selectorUtcValue = this.calendarValidator.GetDateTimeValueWithDateStepApplied(this.utcValue);
        }

        private void HandleValueChange(DateTime? oldValue, DateTime? newValue)
        {
            if (newValue.HasValue && !this.calendarValidator.IsValueInRange(this.utcValue))
            {
                this.ChangePropertyInternally(ValueProperty, oldValue);

                // Cause the designer to notify the user that the set value is not
                // in the allowed value range.
                if (DesignMode.DesignModeEnabled)
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                this.UpdateValueString();
                if (this.displayModeCache == DateTimePickerDisplayMode.Inline || this.IsOpen)
                {
                    this.PrepareSelector();
                }

                this.FireValueChanged();
            }
        }

        private void CheckCoerceValue()
        {
            if (!this.Value.HasValue || this.calendarValidator.IsValueInRange(this.utcValue))
            {
                return;
            }

            DateTime coercedValue = this.calendarValidator.CoerceDateTime(this.utcValue);
            this.SetValue(ValueProperty, new DateTime?(this.GetValueFromKind(coercedValue)));
        }

        private void FireValueChanged()
        {
            EventHandler eh = this.ValueChanged;
            if (eh != null)
            {
                eh(this, EventArgs.Empty);
            }
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            this.IsOpen = false;
        }
    }
}