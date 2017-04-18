using System;
using System.Collections.Generic;
using System.Diagnostics;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Input.DateTimePickers;
using Telerik.UI.Xaml.Controls.Primitives.LoopingList;
using Windows.Foundation;
using Windows.Globalization;
using Windows.System;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Input
{
    public abstract partial class DateTimePicker
    {
        /// <summary>
        /// Identifies the <see cref="ItemLength"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemLengthProperty =
            DependencyProperty.Register(nameof(ItemLength), typeof(double), typeof(DateTimePicker), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Identifies the <see cref="ItemSpacing"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemSpacingProperty =
            DependencyProperty.Register(nameof(ItemSpacing), typeof(double), typeof(DateTimePicker), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Identifies the <see cref="ItemCount"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemCountProperty =
            DependencyProperty.Register(nameof(ItemCount), typeof(int), typeof(DateTimePicker), new PropertyMetadata(3));

        /// <summary>
        /// Identifies the SelectorOrder dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectorOrderProperty =
            DependencyProperty.Register(nameof(SelectorOrder), typeof(string), typeof(DateTimePicker), new PropertyMetadata(string.Empty, OnSelectorOrderPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="SelectorDefaultValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectorDefaultValueProperty =
            DependencyProperty.Register(nameof(SelectorDefaultValue), typeof(object), typeof(DateTimePicker), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SelectorHeader"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectorHeaderProperty =
            DependencyProperty.Register(nameof(SelectorHeader), typeof(object), typeof(DateTimePicker), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SelectorHeaderTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectorHeaderTemplateProperty =
            DependencyProperty.Register(nameof(SelectorHeaderTemplate), typeof(DataTemplate), typeof(DateTimePicker), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IsOpen"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(DateTimePicker), new PropertyMetadata(false, OnIsOpenChanged));

        /// <summary>
        /// Identifies the <see cref="SelectorBackgroundStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectorBackgroundStyleProperty =
            DependencyProperty.Register(nameof(SelectorBackgroundStyle), typeof(Style), typeof(DateTimePicker), new PropertyMetadata(null));

        internal int availableListsCount;
        internal Popup popup;
        internal List<DateTimeList> dateTimeLists = new List<DateTimeList>();

        private const string PickerButtonPartName = "PART_PickerButton";
        private const string PopupPartName = "PART_Popup";
        private const string SelectorLayoutRootPartName = "PART_SelectorLayoutRoot";
        private const string SelectorHeaderPartName = "PART_SelectorHeader";
        private const string DeviceFamilyMobileName = "Windows.Mobile";

#if WINDOWS_PHONE_APP
        private const string CommandBarInfoPartName = "PART_CommandBarInfo";
        private const string CommandBarOKButtonPartName = "PART_CommandBarOKButton";
        private const string CommandBarCancelButtonPartName = "PART_CommandBarCancelButton";
#endif

#if !WINDOWS_PHONE_APP
        private const string SelectorOKButtonPartName = "PART_SelectorOKButton";
        private const string SelectorCancelButtonPartName = "PART_SelectorCancelButton";
        private const string SelectorsButtonsPanelPartName = "PART_SelectorButtonsPanel";
#endif

        private static readonly object PopupChildTag = new object();

        private DateTime selectorUtcValue;
        private byte updatingSelection;
        private bool closedInternally;
        private Button pickerButton;
        private Border selectorLayoutRoot;
        private ContentPresenter selectorHeaderPresenter;

#if WINDOWS_PHONE_APP
        private AppBar defaultAppBar;
        private Page currentPage;
        private CommandBarInfo commandBarInfo;
        private AppBarButton commandBarOKButton;
        private AppBarButton commandBarCancelButton;

#endif

#if !WINDOWS_PHONE_APP
        private Panel selectorButtonsPanel;
        private Button selectorOKButton;
        private Button selectorCancelButton;

#endif

        /// <summary>
        /// This event is raised when the selector part of the <see cref="DateTimePicker"/> control has been closed.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// This event is raised when the selector part of the <see cref="DateTimePicker"/> control has been opened.
        /// </summary>
        public event EventHandler Opened;

        /// <summary>
        /// Gets or sets the <see cref="Style"/> instance that defines the fill and border of the Selector part of the control.
        /// The Style should target the <see cref="Border"/> type.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadDatePicker x:Name="datePicker"&gt;
        ///     &lt;telerikInput:RadDatePicker.SelectorBackgroundStyle&gt;
        ///         &lt;Style TargetType="Border"&gt;
        ///             &lt;Setter Property="Background" Value="Bisque"/&gt;
        ///             &lt;Setter Property="BorderBrush" Value="RoyalBlue"/&gt;
        ///             &lt;Setter Property="BorderThickness" Value="5"/&gt;
        ///         &lt;/Style&gt;
        ///     &lt;/telerikInput:RadDatePicker.SelectorBackgroundStyle&gt;
        /// &lt;/telerikInput:RadDatePicker&gt;
        /// </code>
        /// </example>
        public Style SelectorBackgroundStyle
        {
            get
            {
                return this.GetValue(SelectorBackgroundStyleProperty) as Style;
            }
            set
            {
                this.SetValue(SelectorBackgroundStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the length (width and height) of the <see cref="DateTimeItem"/> that appear in the selector part of the picker.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadDatePicker x:Name="datePicker" ItemLength="100"/&gt;
        /// </code>
        /// <code language="c#">
        /// datePicker.ItemLength = 100;
        /// </code>
        /// </example>
        public double ItemLength
        {
            get
            {
                return (double)this.GetValue(ItemLengthProperty);
            }
            set
            {
                this.SetValue(ItemLengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the margin of the <see cref="DateTimeItem"/> that appear in the selector part of the <see cref="DateTimePicker"/>.
        /// </summary>
        /// <value>
        /// Default value: 5.
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadDatePicker x:Name="datePicker" ItemSpacing="15"/&gt;
        /// </code>
        /// <code language="c#">
        /// datePicker.ItemSpacing = 15;
        /// </code>
        /// </example>
        public double ItemSpacing
        {
            get
            {
                return (double)this.GetValue(ItemSpacingProperty);
            }
            set
            {
                this.SetValue(ItemSpacingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selector format. 
        /// This value defines which <see cref="DateTimeItem"/> parts of the selector will be visible and how they will be ordered.
        /// For example setting "ym" will display the Year and Month items in a <see cref="RadDatePicker"/> instance.
        /// </summary>
        /// <value>
        /// <para>
        /// Options available for <see cref="RadDatePicker"/>: <c>"y"</c> (year), <c>"m"</c> (month), <c>"d"</c> (day)
        /// </para>
        /// <para>
        /// Options available for <see cref="RadTimePicker"/>: <c>"h"</c>,<c>"H"</c> (hour), <c>"m"</c> (minute), <c>"t"</c> (time part - AM/PM)
        /// </para>
        /// </value>
        /// <remarks>
        /// When the format <see cref="string"/> contains <c>"H"</c>, then the time part is not displayed.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadDatePicker x:Name="datePicker" SelectorOrder="th"/&gt;
        /// </code>
        /// <code language="C#">
        /// datePicker.SelectorOrder = "th";</code>
        /// </example>
        public string SelectorOrder
        {
            get
            {
                return (string)this.GetValue(SelectorOrderProperty);
            }
            set
            {
                this.SetValue(SelectorOrderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the selector part of the control is opened.
        /// </summary>
        /// <value>
        /// Default value: false.
        /// </value>
        /// <remarks>
        /// If the <see cref="DisplayMode"/> property is set to <see cref="DateTimePickerDisplayMode.Inline"/>, the selector part is always visible.
        /// </remarks>
        public bool IsOpen
        {
            get
            {
                return (bool)this.GetValue(IsOpenProperty);
            }
            set
            {
                this.SetValue(IsOpenProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="DateTime"/> struct that represents
        /// the default value displayed in the selector part when opened. The default value is shown
        /// when the <see cref="DateTimePicker.Value"/> property is set to <c>null</c>.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadDatePicker x:Name="datePicker" /&gt;
        /// </code>
        /// <code language="c#">
        /// datePicker.SelectorDefaultValue = new DateTime(2020, 10, 11);
        /// </code>
        /// </example>
        public DateTime? SelectorDefaultValue
        {
            get
            {
                return (DateTime?)this.GetValue(SelectorDefaultValueProperty);
            }
            set
            {
                this.SetValue(SelectorDefaultValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header displayed in the selector part of the <see cref="DateTimePicker"/> instance.
        /// </summary>
        /// <value>
        /// Default value: <c>null</c>
        /// </value>
        /// <remarks>
        /// If the value is <c>null</c>, the default displayed text is "Select Date"/"Select Time"
        /// for <see cref="RadDatePicker"/>/<see cref="RadTimePicker"/>.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadDatePicker x:Name="datePicker" SelectorHeader="Enter your birthday"/&gt;
        /// </code>
        /// <code language="c#">
        /// datePicker.SelectorHeader = "Enter your birthday:";
        /// </code>
        /// </example>
        public object SelectorHeader
        {
            get
            {
                return this.GetValue(SelectorHeaderProperty);
            }
            set
            {
                this.SetValue(SelectorHeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="DataTemplate"/> 
        /// class that represents the template used to visualize the
        /// content defined by the <see cref="SelectorHeader"/> property.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadDatePicker&gt;
        ///     &lt;telerikInput:RadDatePicker.SelectorHeaderTemplate&gt;
        ///         &lt;DataTemplate&gt;
        ///             &lt;TextBlock Text="Enter your birthday" Foreground="Orange"
        ///                        FontSize="20" FontWeight="Bold" Margin="10,10,10,10"/&gt;
        ///         &lt;/DataTemplate&gt;
        ///     &lt;/telerikInput:RadDatePicker.SelectorHeaderTemplate&gt;
        /// &lt;/telerikInput:RadDatePicker&gt;
        /// </code>
        /// </example>
        public DataTemplate SelectorHeaderTemplate
        {
            get
            {
                return this.GetValue(SelectorHeaderTemplateProperty) as DataTemplate;
            }
            set
            {
                this.SetValue(SelectorHeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the number of items visible within the selector part of the picker.
        /// This property is used to determine the height of the selector part when opened.
        /// </summary>
        /// <value>
        /// <para>Default value: 3</para>
        /// <para>
        /// When the value is set to negative number, the selector part maximizes its height to fit the height of the current top-level Window.
        /// </para>
        /// </value>
        /// <remarks>
        /// The calculated height will not exceed the height of the current top-level Window.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadDatePicker x:Name="datePicker" ItemCount="8"/&gt;
        /// </code>
        /// <code language="c#">
        /// datePicker.ItemCount = 8;
        /// </code>
        /// </example>
        public int ItemCount
        {
            get
            {
                return (int)this.GetValue(ItemCountProperty);
            }
            set
            {
                this.SetValue(ItemCountProperty, value);
            }
        }

        /// <summary>
        /// Gets the Popup instance used to display the component when its DisplayMode is Standard.
        /// </summary>
        internal Popup Popup
        {
            get
            {
                return this.popup;
            }
        }

        /// <summary>
        /// Gets the DateTimePickerButton instance that represents the inline(picker) part of the button. Exposed for testing purposes.
        /// </summary>
        internal FrameworkElement PickerButton
        {
            get
            {
                return this.pickerButton;
            }
        }

        /// <summary>
        /// Gets the OK button present in the Selector part. Exposed for testing purposes.
        /// </summary>
        internal FrameworkElement OKButton
        {
            get
            {
#if WINDOWS_PHONE_APP
                return this.commandBarOKButton;
#endif
#if !WINDOWS_PHONE_APP
                return this.selectorOKButton;
#endif
            }
        }

        /// <summary>
        /// Gets the Cancel button present in the Selector part. Exposed for testing purposes.
        /// </summary>
        internal FrameworkElement CancelButton
        {
            get
            {
#if WINDOWS_PHONE_APP
                return this.commandBarCancelButton;
#endif
#if !WINDOWS_PHONE_APP
                return this.selectorCancelButton;
#endif
            }
        }

        /// <summary>
        /// Gets container of the buttons in the selector. Exposed for testing purposes.
        /// </summary>
        internal FrameworkElement ButtonsPanel
        {
            get
            {
#if WINDOWS_PHONE_APP
                return this.defaultAppBar;
#endif
#if !WINDOWS_PHONE_APP
                return this.selectorButtonsPanel;
#endif
            }
        }

        /// <summary>
        /// Gets Border that represents the layout root in the Selector part. Exposed for testing purposes.
        /// </summary>
        internal FrameworkElement SelectorLayoutRoot
        {
            get
            {
                return this.selectorLayoutRoot;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Selector Popup has been closed by its IsLightDismissEnabled routine or by a click over the OK/Cancel button. Exposed for testing purposes.
        /// </summary>
        internal bool IsSelectorPopupClosedInternally
        {
            get
            {
                return this.closedInternally;
            }
        }

        /// <summary>
        /// Gets the current DateTime value displayed by the Selector DateTime lists. Exposed for testing purposes.
        /// </summary>
        internal DateTime? DateTimeListsValue
        {
            get
            {
                if (this.IsTemplateApplied)
                {
                    return this.dateTimeLists[0].UtcListValue;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the current Selector min value with step applied. It may differ from the current MinValue.
        /// </summary>
        internal DateTime SelectorUtcValue
        {
            get
            {
                return this.selectorUtcValue;
            }
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Gets the page that hosts this window.
        /// </summary>
        protected Page HostPage
        {
            get
            {
                if (this.currentPage == null)
                {
                    this.currentPage = this.FindPage() as Page;
                }

                return this.currentPage;
            }
        }

#endif

        internal abstract string GetDisplayValueFormatFromCulture();

        internal abstract string GetValueStringForNonGregorianCalendars(Windows.Globalization.Calendar calendar);

        /// <summary>
        /// Retrieves the DateTimeList instance associated with the specified DateTimeComponentType. Internal for testing purposes.
        /// </summary>
        internal DateTimeList GetDateTimeListByComponent(DateTimeComponentType componentType)
        {
            foreach (DateTimeList list in this.dateTimeLists)
            {
                if (list.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
                {
                    continue;
                }

                if (list.ComponentType == componentType)
                {
                    return list;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the DateTimeList instance at the specified position as stored in the dateTimeLists field. Internal for testing purposes.
        /// </summary>
        internal DateTimeList GetDateTimeListAt(int index)
        {
            if (index < 0 || index >= this.dateTimeLists.Count)
            {
                throw new IndexOutOfRangeException();
            }

            return this.dateTimeLists[index];
        }

        internal void RegisterDateTimeList(DateTimeList list)
        {
            if (this.dateTimeLists.Contains(list))
            {
                return;
            }

            double spacing = this.ItemSpacing;
            list.Margin = new Thickness(spacing, 0, spacing, 0);

            this.dateTimeLists.Add(list);
            list.Attach(this);

            list.SelectedIndexChanged += this.OnDateTimeListSelectedIndexChanged;
            list.IsExpandedChanged += this.OnDateTimeListIsExpandedChanged;
        }

        internal void UnregisterDateTimeList(DateTimeList list)
        {
            if (!this.dateTimeLists.Contains(list))
            {
                return;
            }

            this.dateTimeLists.Remove(list);
            list.Detach();

            list.SelectedIndexChanged -= this.OnDateTimeListSelectedIndexChanged;
            list.IsExpandedChanged -= this.OnDateTimeListIsExpandedChanged;
        }

        internal void UpdateDateTimeListsValue(DateTime newValue, DateTimeList selectionChanger)
        {
            foreach (DateTimeList list in this.dateTimeLists)
            {
                if (list != selectionChanger)
                {
                    if (list.IsRealChange(newValue))
                    {
                        // -> list.currentUtcValue != newValue;
                        this.selectorUtcValue = newValue;
                        list.ChangeValue(newValue, true, false);
                        return;
                    }
                }
            }
        }

        internal string GetValueStringFromCalendarCore()
        {
            var calendar = this.calendarValidator.Calendar.Clone();
            if (this.Value.HasValue)
            {
                calendar.SetDateTime(DateTime.SpecifyKind(this.Value.Value, DateTimeKind.Utc));
            }
            else
            {
                calendar.SetDateTime(this.GetSelectorDefaultValue());
            }

            return this.GetValueStringFromCalendar(calendar);
        }

        internal abstract string GetDefaultSelectorOrder();

        internal abstract DateTime GetSelectorValueWithStepApplied(DateTime utcValue);

        internal string ComposeSelectorsOrderString()
        {
            string explicitOrder = this.SelectorOrder;
            if (!string.IsNullOrEmpty(explicitOrder))
            {
                return explicitOrder;
            }

            return this.GetDefaultSelectorOrder();
        }

        /// <summary>
        /// Updates the order (that is the grid column) of each selector, depending on the current Culture and SelectorOrder.
        /// </summary>
        internal virtual void UpdateSelectorsOrder()
        {
        }

        /// <summary>
        /// Exposes the core functionality behind the OK->Click event. Internal for testing purposes.
        /// </summary>
        internal void PerformSelectorOKButtonClick()
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            this.CloseSelector(true, true);
        }

        /// <summary>
        /// Encapsulates the core functionality behind the Cancel->Click event. Internal for testing purposes.
        /// </summary>
        internal void PerformSelectorCancelButtonClick()
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            this.CloseSelector(false, true);
        }

        /// <summary>
        /// Encapsulates the core functionality behind the PickerButton->Click event. Internal for testing purposes.
        /// </summary>
        internal void PerformPickerButtonClick()
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            if (this.IsReadOnly)
            {
                return;
            }

            if (this.popup != null)
            {
                this.PrepareSelector();
                this.ChangePropertyInternally(IsOpenProperty, true);
            }
        }

        /// <summary>
        /// Encapsulates the core functionality behind the KeyDown event of the CoreWindow when the Selector popup is displayed. Internal for testing purposes.
        /// </summary>
        internal bool PerformSelectorKeyDown(VirtualKey key)
        {
            if (key == VirtualKey.Escape)
            {
                this.CloseSelector(false, true);
                return true;
            }

            if (key == VirtualKey.Enter)
            {
                this.CloseSelector(true, true);
                return true;
            }

            if (key == VirtualKey.Left)
            {
                this.ExpandLastOrPreviousList();
                return true;
            }

            if (key == VirtualKey.Right)
            {
                this.ExpandFirstOrNextList();

                return true;
            }

            if (key == VirtualKey.Down || key == VirtualKey.Up 
                || key == VirtualKey.Home || key == VirtualKey.End)
            {
                this.SelectCurrentIndex();
                return true;
            }

            return false;
        }

        internal Size GetSelectorSize()
        {
            var view = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            if (AnalyticsInfo.VersionInfo.DeviceFamily.Equals(DeviceFamilyMobileName) && view != null)
            {
                //TODO refactor this to use width and height and compensate for ofset of the bounds x and y. Also consider resize the view if entering/exiting full screen.
                return new Size(view.VisibleBounds.Right, view.VisibleBounds.Bottom);
            }
            else
            {
                return this.CalculateSelectorSize();
            }
        }

        internal DateTime GetValueFromKind(DateTime date)
        {
            if (this.Value.HasValue)
            {
                return DateTime.SpecifyKind(date, this.Value.Value.Kind);
            }
            else
            {
                return DateTime.SpecifyKind(date, DateTimeKind.Local);
            }
        }

#if !WINDOWS_PHONE_APP
        /// <summary>
        /// Called before the KeyDown event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);

            if (e == null)
            {
                return;
            }

            if (this.DisplayMode == DateTimePickerDisplayMode.Standard)
            {
                return;
            }

            e.Handled = this.PerformSelectorKeyDown(e.Key);
        }

#endif

        /// <summary>
        /// Gets the string that represents the <see cref="Value"/> according to the <paramref name="calendar"/>.
        /// Override this method to change the default <see cref="ValueString"/>.
        /// </summary>
        protected virtual string GetValueStringFromCalendar(Windows.Globalization.Calendar calendar)
        {
            // TODO: Expose FormatProvider property, so that the users can implement one for non-gregorian calendars
            if (this.CalendarIdentifier == CalendarIdentifiers.Gregorian)
            {
                if (string.IsNullOrEmpty(this.DisplayValueFormat))
                {
                    return calendar.GetUtcDateTime().ToString(this.GetDisplayValueFormatFromCulture(), new System.Globalization.CultureInfo(calendar.ResolvedLanguage));
                }
                else
                {
                    return calendar.GetUtcDateTime().ToString(this.DisplayValueFormat, new System.Globalization.CultureInfo(calendar.ResolvedLanguage));
                }
            }
            else
            {
                return this.GetValueStringForNonGregorianCalendars(calendar);
            }
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Unloaded"/> event. Allows inheritors to provide their specific logic.
        /// </summary>
        protected override void UnloadCore()
        {
            base.UnloadCore();

            this.UnhookCoreWindowEvents();
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            this.UnhookCoreWindowEvents();

#if WINDOWS_PHONE_APP
            this.commandBarOKButton.Click -= this.OnSelectorOKButtonClick;
            this.commandBarCancelButton.Click -= this.OnSelectorCancelButtonClick;
#endif

#if !WINDOWS_PHONE_APP
            this.selectorOKButton.Click -= this.OnSelectorOKButtonClick;
            this.selectorCancelButton.Click -= this.OnSelectorCancelButtonClick;
#endif

            this.pickerButton.Click -= this.OnPickerButtonClick;
            this.popup.Opened -= this.OnPopupOpened;
            this.popup.Closed -= this.OnPopupClosed;
            this.selectorLayoutRoot.PointerWheelChanged -= this.OnPopupPointerWheelChanged;

            while (this.dateTimeLists.Count > 0)
            {
                this.UnregisterDateTimeList(this.dateTimeLists[0]);
            }
        }

        /// <summary>
        /// Resolves the control's template parts.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.pickerButton = this.GetTemplatePartField<Button>(PickerButtonPartName);
            applied = applied && this.pickerButton != null;

            this.popup = this.GetTemplatePartField<Popup>(PopupPartName);
            applied = applied && this.popup != null;

#if WINDOWS_PHONE_APP
            this.commandBarInfo = this.GetTemplatePartField<CommandBarInfo>(CommandBarInfoPartName);
            applied = applied && this.commandBarInfo != null;

            this.commandBarOKButton = this.GetTemplatePartField<AppBarButton>(CommandBarOKButtonPartName);
            applied = applied && this.commandBarOKButton != null;

            this.commandBarCancelButton = this.GetTemplatePartField<AppBarButton>(CommandBarCancelButtonPartName);
            applied = applied && this.commandBarCancelButton != null;
#endif

#if !WINDOWS_PHONE_APP
            this.selectorOKButton = this.GetTemplatePartField<Button>(SelectorOKButtonPartName);
            applied = applied && this.selectorOKButton != null;

            this.selectorCancelButton = this.GetTemplatePartField<Button>(SelectorCancelButtonPartName);
            applied = applied && this.selectorCancelButton != null;

            this.selectorButtonsPanel = this.GetTemplatePartField<Panel>(SelectorsButtonsPanelPartName);
            applied = applied && this.selectorButtonsPanel != null;
#endif

            this.selectorLayoutRoot = this.GetTemplatePartField<Border>(SelectorLayoutRootPartName);
            applied = applied && this.selectorLayoutRoot != null;

            this.selectorHeaderPresenter = this.GetTemplatePartField<ContentPresenter>(SelectorHeaderPartName);
            applied = applied && this.selectorHeaderPresenter != null;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate"/> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.pickerButton.Click += this.OnPickerButtonClick;
            
            this.selectorOKButton.Click += this.OnSelectorOKButtonClick;
            this.selectorCancelButton.Click += this.OnSelectorCancelButtonClick;
            
            this.popup.Opened += this.OnPopupOpened;
            this.popup.Closed += this.OnPopupClosed;

            this.selectorLayoutRoot.PointerWheelChanged += this.OnPopupPointerWheelChanged;

            this.SyncPickerButtonContent();
            this.UpdateSelectorsOrder();

            // sort the lists by their Grid.Column property (needed for proper keyboard navigation)
            this.dateTimeLists.Sort(this.CompareDateTimeListByGridColumn);
            this.UpdateDateTimeListsTabMode();

            this.selectorOKButton.Content = InputLocalizationManager.Instance.GetString("OKText");
            this.selectorCancelButton.Content = InputLocalizationManager.Instance.GetString("CancelText");

            if (this.DisplayMode == DateTimePickerDisplayMode.Inline)
            {
                this.UpdateDisplayMode();
                return;
            }
            
            if (this.IsOpen)
            {
                // IsOpen is set in XAML, update the selector size and position
                this.PrepareSelector();
                this.OnPopupOpened(null, null);

                this.InvokeAsync(() =>
                {
                    this.UpdateSelectorLayout();
                });
            }
        }

        private static void SelectNextDateTimeItem(DateTimeList list)
        {
            int selectedIndex = list.SelectedIndex;
            selectedIndex++;

            if (selectedIndex == list.ItemsPanel.LogicalCount)
            {
                if (!list.IsLoopingEnabled)
                {
                    return;
                }
                selectedIndex = 0;
            }

            TryUpdateSelection(list, selectedIndex);
        }

        private static void SelectPreviousDateTimeItem(DateTimeList list)
        {
            int selectedIndex = list.SelectedIndex;
            selectedIndex--;

            if (selectedIndex < 0)
            {
                if (!list.IsLoopingEnabled)
                {
                    return;
                }

                selectedIndex = list.ItemsPanel.LogicalCount - 1;
            }

            TryUpdateSelection(list, selectedIndex);
        }

        private static void TryUpdateSelection(DateTimeList list, int newSelectedIndex)
        {
            if (list.CanChangeSelectedIndex(newSelectedIndex))
            {
                list.UpdateSelection(newSelectedIndex, list.GetVisualIndex(newSelectedIndex), LoopingListSelectionChangeReason.Private);
            }
        }

        private static void ExpandList(DateTimeList list)
        {
            // We have the IsInTestMode check because the Focus operation is asyncronous and hard to test in synchronous unit tests
            if (RadControl.IsInTestMode)
            {
                list.IsExpanded = true;
            }
            else
            {
                list.Focus(FocusState.Programmatic);
            }
        }

        private static void OnIsOpenChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var picker = sender as DateTimePicker;
            if (picker.IsInternalPropertyChange || !picker.IsTemplateApplied)
            {
                return;
            }

            DateTimePickerAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(picker) as DateTimePickerAutomationPeer;
            if (peer != null)
            {
                peer.RaiseExpandCollapseAutomationEvent(!((bool)args.NewValue), (bool)args.NewValue);
            }

            if ((bool)args.NewValue)
            {
                picker.PrepareSelector();
            }
        }
        
        private Size CalculateSelectorSize()
        {
            double itemLength = this.ItemLength;
            double itemSpacing = this.ItemSpacing;
            int itemCount = this.ItemCount;
            Thickness padding = this.selectorLayoutRoot.Padding;
            Thickness borderThickness = this.selectorLayoutRoot.BorderThickness;
            Rect windowBounds = Window.Current.Bounds;
            double height;
            if (itemCount <= 0)
            {
                // non-positive item count means that the selector is vertically stretched
                height = windowBounds.Height;
            }
            else
            {
                height = itemCount * itemLength + (itemCount - 1) * itemSpacing * 2 + itemLength;
                if (this.selectorButtonsPanel.DesiredSize.Height == 0)
                {
                    this.selectorButtonsPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }
                if (this.selectorHeaderPresenter.DesiredSize.Height == 0)
                {
                    this.selectorHeaderPresenter.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }
                height += this.selectorHeaderPresenter.DesiredSize.Height;
                height += this.selectorButtonsPanel.DesiredSize.Height + borderThickness.Top + borderThickness.Bottom;
                height = Math.Min(windowBounds.Height, height);
            }
            double width = itemLength * 3 + 6 * itemSpacing + padding.Left + padding.Right + borderThickness.Left + borderThickness.Right;
            width = Math.Min(windowBounds.Width, width);
            if (width < this.ActualWidth)
            {
                width = this.ActualWidth;
            }
            // TODO: This is very HACKY, it overcomes an anti-alias problem with single item within the lists
            // TODO: Check with next version of the framework
            if (height % 2 == 0)
            {
                height += 0.5;
            }
            if (width % 2 == 0)
            {
                width += 0.5;
            }
            return new Size(width, height);
        }
        private FrameworkElement FindPage()
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame != null && frame.Content is Page)
            {
                return frame.Content as Page;
            }

            return ElementTreeHelper.FindVisualDescendant<Page>(Window.Current.Content);
        }

        private void PrepareSelector()
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            DateTime newValue;
            if (this.Value.HasValue)
            {
                newValue = this.utcValue;
            }
            else
            {
                newValue = DateTime.SpecifyKind(this.GetSelectorDefaultValue(), DateTimeKind.Utc);
                this.utcValue = newValue;
            }

            this.selectorUtcValue = this.GetSelectorValueWithStepApplied(newValue);
            this.UpdateDateTimeListsValue(this.selectorUtcValue, null);

            if (this.displayModeCache == DateTimePickerDisplayMode.Inline)
            {
                return;
            }

            this.HookCoreWindowEvents();

#if !WINDOWS_PHONE_APP
            // explicitly go to Normal state for the Selector OK and Cancel buttons
            VisualStateManager.GoToState(this.selectorOKButton, "Normal", false);
            VisualStateManager.GoToState(this.selectorCancelButton, "Normal", false);
#endif

            this.UpdateSelectorLayout();
        }

        private void UpdateSelectorLayout()
        {
            Size popupSize = this.GetSelectorSize();
            this.selectorLayoutRoot.Width = popupSize.Width;
            this.selectorLayoutRoot.Height = popupSize.Height;

            this.UpdateSelectorPosition(popupSize);
        }

        private void HookCoreWindowEvents()
        {
            CoreWindow root = Window.Current.CoreWindow;

            if (root != null)
            {
                root.KeyDown += this.OnCoreWindowKeyDown;
            }
        }

        private void UnhookCoreWindowEvents()
        {
            CoreWindow root = Window.Current.CoreWindow;

            if (root != null)
            {
                root.KeyDown -= this.OnCoreWindowKeyDown;
            }
        }

        private void OnCoreWindowKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (!this.IsOpen)
            {
                Debug.Assert(false, "This event should be handled ONLY if the popup is opened");
                this.UnhookCoreWindowEvents();
                return;
            }

            args.Handled = this.PerformSelectorKeyDown(args.VirtualKey);
        }

        private void SelectCurrentIndex()
        {
            int index = this.GetExpandedIndex();
            if (index == -1)
            {
                return;
            }

            TryUpdateSelection(this.dateTimeLists[index], this.dateTimeLists[index].SelectedIndex);
        }

        private void ExpandLastOrPreviousList()
        {
            if (this.availableListsCount <= 1)
            {
                return;
            }

            int expandedIndex = this.GetExpandedIndex();
            if (expandedIndex == -1)
            {
                expandedIndex = this.dateTimeLists.Count - 1;
            }
            else
            {
                while (true)
                {
                    expandedIndex--;

                    if (expandedIndex < 0)
                    {
                        expandedIndex = this.dateTimeLists.Count - 1;
                    }

                    DateTimeList list = this.dateTimeLists[expandedIndex];
                    if (!list.IsExpanded && list.Visibility == Visibility.Visible)
                    {
                        break;
                    }
                }
            }

            ExpandList(this.dateTimeLists[expandedIndex]);
        }

        private void ExpandFirstOrNextList()
        {
            if (this.availableListsCount <= 1)
            {
                return;
            }

            int expandedIndex = this.GetExpandedIndex();
            if (expandedIndex == -1)
            {
                expandedIndex = 0;
            }
            else
            {
                while (true)
                {
                    expandedIndex++;

                    if (expandedIndex == this.dateTimeLists.Count)
                    {
                        expandedIndex = 0;
                    }

                    DateTimeList list = this.dateTimeLists[expandedIndex];
                    if (!list.IsExpanded && list.Visibility == Visibility.Visible)
                    {
                        break;
                    }
                }
            }

            ExpandList(this.dateTimeLists[expandedIndex]);
        }

        private int GetExpandedIndex()
        {
            for (int i = this.dateTimeLists.Count - 1; i >= 0; i--)
            {
                DateTimeList list = this.dateTimeLists[i];
                if (list.IsExpanded && list.Visibility == Visibility.Visible)
                {
                    return i;
                }
            }

            return -1;
        }

        private double GetSelectorWidth(bool clampToWindow)
        {
#if WINDOWS_PHONE_APP
            return Window.Current.Bounds.Width;
#endif
#if !WINDOWS_PHONE_APP
            double itemLength = this.ItemLength;
            double itemSpacing = this.ItemSpacing;
            int itemCount = this.ItemCount;
            Thickness padding = this.selectorLayoutRoot != null ? this.selectorLayoutRoot.Padding : new Thickness();
            Thickness borderThickness = this.selectorLayoutRoot != null ? this.selectorLayoutRoot.BorderThickness : new Thickness();
            double width = itemLength * 3 + 6 * itemSpacing + padding.Left + padding.Right + borderThickness.Left + borderThickness.Right;
            if (clampToWindow)
            {
                Window current = Window.Current;
                if (current != null)
                {
                    width = Math.Min(current.Bounds.Width, width);
                }
            }
            return width;
#endif
        }

        private void UpdateSelectorPosition(Size popupSize)
        {
            Point location = this.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0));

#if WINDOWS_PHONE_APP
           PositionPopupOnWholeScreen(location);
#endif
#if WINDOWS_APP
            PositionPopupOverPickerButton(popupSize, location);
#endif
#if WINDOWS_UWP

           var view = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            if (view != null && view.VisibleBounds.Width < 720)
            {
                PositionPopupOnWholeScreen(location);
            }
            else
            {
                PositionPopupOverPickerButton(popupSize, location);
            }
#endif
        }

        private void PositionPopupOverPickerButton(Size popupSize, Point location)
        {
#if WINDOWS_UWP
            Rect screen = ApplicationView.GetForCurrentView().VisibleBounds;
#else
            Rect screen = Window.Current.Bounds;
#endif
            double width = this.ActualWidth;
            if (width == 0)
            {
                width = popupSize.Width;
            }
            double offsetX = (width - popupSize.Width) / 2;
            if (location.X + offsetX < 0)
            {
                offsetX = 0;
            }
            else if (location.X + offsetX > screen.Width)
            {
                offsetX -= (location.X + offsetX) - screen.Width;
            }
            double offsetY = (this.selectorLayoutRoot.Height - this.ActualHeight) / 2;
            if (location.Y - offsetY < 0)
            {
                offsetY = location.Y;
            }
            else if (location.Y + this.ActualHeight + offsetY > screen.Height)
            {
                offsetY += (location.Y + this.ActualHeight + offsetY) - screen.Height;
            }
            this.popup.HorizontalOffset = offsetX;
            this.popup.VerticalOffset = -offsetY;
        }

        private void PositionPopupOnWholeScreen(Point location)
        {
            this.popup.HorizontalOffset = -location.X;
            this.popup.VerticalOffset = -location.Y;
        }

        private void CloseSelector(bool accept, bool focus)
        {
#if !WINDOWS_PHONE_APP
            if (this.DisplayMode == DateTimePickerDisplayMode.Inline)
            {
                return;
            }
#endif
            this.closedInternally = true;
            this.UnhookCoreWindowEvents();

            if (accept)
            {
                this.Value = this.GetValueFromKind(this.selectorUtcValue);
            }

            this.ChangePropertyInternally(IsOpenProperty, false);
            foreach (DateTimeList list in this.dateTimeLists)
            {
                list.IsExpanded = false;
            }

            if (focus)
            {
                this.pickerButton.Focus(FocusState.Keyboard);
            }
        }

        private void OnSelectorCancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.PerformSelectorCancelButtonClick();
        }

        private void OnSelectorOKButtonClick(object sender, RoutedEventArgs e)
        {
            this.PerformSelectorOKButtonClick();
        }

        private void SyncPickerButtonContent()
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            if (this.Value.HasValue)
            {
                this.pickerButton.ContentTemplate = null;
                this.pickerButton.Content = this.ValueString;
            }
            else
            {
                this.pickerButton.Content = this.GetValue(EmptyContentProperty);
                this.pickerButton.ContentTemplate = this.EmptyContentTemplate;
            }
        }

        private void OnPickerButtonClick(object sender, RoutedEventArgs e)
        {
            this.PerformPickerButtonClick();
        }

        private void OnDateTimeListIsExpandedChanged(object sender, EventArgs e)
        {
            DateTimeList senderAsList = sender as DateTimeList;
            if (!senderAsList.IsExpanded)
            {
                return;
            }

            foreach (DateTimeList list in this.dateTimeLists)
            {
                if (list != senderAsList)
                {
                    list.IsExpanded = false;
                }
            }
        }

        private void OnDateTimeListSelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.updatingSelection > 0)
            {
                return;
            }

            DateTimeList typedSender = sender as DateTimeList;
            this.UpdateSelectorValue(typedSender, typedSender.UtcListValue);
        }

        private void UpdateSelectorValue(DateTimeList selectionChanger, DateTime newValue)
        {
            this.updatingSelection++;
            this.UpdateDateTimeListsValue(newValue, selectionChanger);
            this.selectorUtcValue = selectionChanger.UtcListValue;
            this.updatingSelection--;
#if !WINDOWS_PHONE_APP
            if (this.DisplayMode == DateTimePickerDisplayMode.Inline)
            {
                this.Value = this.GetValueFromKind(this.selectorUtcValue);
            }
#endif
        }

        private void OnPopupClosed(object sender, object e)
        {
            if (this.displayModeCache == DateTimePickerDisplayMode.Inline)
            {
                return;
            }

            if (!this.closedInternally)
            {
                // The IsLightDismissEnabled routine is triggerred and the popup is closed by the system.
                // It seems that the framework clears the IsOpen dependency property and we should bind it again :)
                Binding b = new Binding();
                b.Source = this;
                b.Path = new PropertyPath("IsOpen");
                this.popup.SetBinding(Popup.IsOpenProperty, b);

                this.CloseSelector(false, false);
            }

#if WINDOWS_PHONE_APP
            this.HostPage.BottomAppBar = this.defaultAppBar;
#endif

            EventHandler eh = this.Closed;
            if (eh != null)
            {
                eh(this, EventArgs.Empty);
            }
        }

        private void OnPopupOpened(object sender, object e)
        {
            if (this.displayModeCache == DateTimePickerDisplayMode.Inline)
            {
                return;
            }

            this.closedInternally = false;

            // focus the first available datetime list
            foreach (DateTimeList list in this.dateTimeLists)
            {
                if (list.Visibility == Windows.UI.Xaml.Visibility.Visible)
                {
                    list.Focus(FocusState.Keyboard);
                    break;
                }
            }

#if WINDOWS_PHONE_APP
            this.defaultAppBar = this.HostPage.BottomAppBar;
            this.HostPage.BottomAppBar = this.commandBarInfo.PickerCommandBar;
#endif

            EventHandler eh = this.Opened;
            if (eh != null)
            {
                eh(this, EventArgs.Empty);
            }
        }

        private void OnPopupPointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (this.displayModeCache == DateTimePickerDisplayMode.Standard && !this.IsOpen)
            {
                return;
            }

            var point = e.GetCurrentPoint(this.popup);
            var list = this.GetListUnderPointer(point.Position);

            if (list == null)
            {
                return;
            }

            if (!list.IsExpanded)
            {
                ExpandList(list);
                return;
            }

            if (point.Properties.MouseWheelDelta > 0)
            {
                SelectPreviousDateTimeItem(list);
            }
            else
            {
                SelectNextDateTimeItem(list);
            }
        }

        private DateTimeList GetListUnderPointer(Point pointerPosition)
        {
            foreach (DateTimeList list in this.dateTimeLists)
            {
                Rect localListRect = new Rect(0, 0, list.ActualWidth, list.ActualHeight);
                Rect transformedListRect = list.TransformToVisual(this.popup).TransformBounds(localListRect);

                if (transformedListRect.Contains(pointerPosition))
                {
                    return list;
                }
            }

            return null;
        }

        private int CompareDateTimeListByGridColumn(DateTimeList list1, DateTimeList list2)
        {
            int col1 = Grid.GetColumn(list1);
            int col2 = Grid.GetColumn(list2);

            return col1.CompareTo(col2);
        }

        private void UpdateDisplayMode()
        {
            Panel layoutRoot = this.popup.Parent as Panel;
            if (layoutRoot == null)
            {
                Debug.Assert(false, "No layout root associated with the Control template.");
                return;
            }

            if (this.DisplayMode == DateTimePickerDisplayMode.Inline)
            {
                FrameworkElement content = this.popup.Child as FrameworkElement;
                if (content == null)
                {
                    Debug.Assert(false, "No popup child");
                    return;
                }

                this.popup.Child = null;

                content.Tag = PopupChildTag;

                // TODO: Clearing the transitions through code is tricky
                content.Transitions = null;

                // Remove the Width and Height properties specified by PrepareSelector routine
                content.ClearValue(WidthProperty);
                content.ClearValue(HeightProperty);

                layoutRoot.Children.Add(content);
                Grid.SetRow(content, 1);

                if (!this.Value.HasValue)
                {
                    this.Value = this.GetValueFromKind(this.GetSelectorDefaultValue());
                }

                this.PrepareSelector();
                this.InvalidateMeasure();
                return;
            }

            UIElement popupChild = null;
            foreach (FrameworkElement child in layoutRoot.Children)
            {
                if (child.Tag == PopupChildTag)
                {
                    popupChild = child;
                    break;
                }
            }

            if (popupChild != null)
            {
                layoutRoot.Children.Remove(popupChild);
                popupChild.ClearValue(TransitionsProperty);
                this.popup.Child = popupChild;
            }

            this.InvalidateMeasure();
        }

        private void UpdateDateTimeListsTabMode()
        {
            int tabIndex = 1;
            foreach (DateTimeList list in this.dateTimeLists)
            {
                list.TabIndex = tabIndex++;
            }
        }

        private DateTime GetSelectorDefaultValue()
        {
            DateTime? defaultValue = this.SelectorDefaultValue;
            if (defaultValue.HasValue)
            {
                return DateTime.SpecifyKind(defaultValue.Value, DateTimeKind.Utc);
            }

            return this.calendarValidator.CoerceDateTime(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc));
        }
    }
}