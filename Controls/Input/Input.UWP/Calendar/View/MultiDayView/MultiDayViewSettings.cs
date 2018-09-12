using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Telerik.Core.Data;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents a class that enables a user to set specific properties for the multi day view.
    /// </summary>
    public class MultiDayViewSettings : RadDependencyObject, ICollectionChangedListener, IPropertyChangedListener
    {
        /// <summary>
        /// Identifies the <c cref="VisibleDaysProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibleDaysProperty =
            DependencyProperty.Register(nameof(VisibleDays), typeof(int), typeof(MultiDayViewSettings), new PropertyMetadata(DefaultMultiDayViewVisibleDays, OnVisibleDaysChanged));

        /// <summary>
        /// Identifies the <c cref="TimerRulerTickLengthProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TimerRulerTickLengthProperty =
            DependencyProperty.Register(nameof(TimerRulerTickLength), typeof(TimeSpan), typeof(MultiDayViewSettings), new PropertyMetadata(TimeSpan.FromHours(1), OnTimerRulerTickLengthChanged));

        /// <summary>
        /// Identifies the <c cref="DayStartTimeProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DayStartTimeProperty =
            DependencyProperty.Register(nameof(DayStartTime), typeof(TimeSpan), typeof(MultiDayViewSettings), new PropertyMetadata(TimeSpan.Zero, OnDayStartTimeChanged));

        /// <summary>
        /// Identifies the <c cref="DayEndTimeProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DayEndTimeProperty =
            DependencyProperty.Register(nameof(DayEndTime), typeof(TimeSpan), typeof(MultiDayViewSettings), new PropertyMetadata(TimeSpan.FromHours(24), OnDayEndTimeChanged));

        /// <summary>
        /// Identifies the <c cref="TimeLinesSpacingProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TimeLinesSpacingProperty =
            DependencyProperty.Register(nameof(TimeLinesSpacing), typeof(int), typeof(MultiDayViewSettings), new PropertyMetadata(DefaultTimeLinesInterval, OnTimeLinesSpacingChanged));

        /// <summary>
        /// Identifies the <c cref="AllDayAppointmentMinHeightProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllDayAppointmentMinHeightProperty =
            DependencyProperty.Register(nameof(AllDayAppointmentMinHeight), typeof(int), typeof(MultiDayViewSettings), new PropertyMetadata(DefaultAllDayAppointmentHeight, OnAllDayAppointmentMinHeightChanged));

        /// <summary>
        /// Identifies the <c cref="AllDayMaxVisibleRowsProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllDayMaxVisibleRowsProperty =
            DependencyProperty.Register(nameof(AllDayMaxVisibleRows), typeof(int), typeof(MultiDayViewSettings), new PropertyMetadata(DefaultAllDayMaxVisibleRows, OnAllDayMaxVisibleRowsChanged));

        /// <summary>
        /// Identifies the <c cref="AllDayAppointmentSpacingProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllDayAppointmentSpacingProperty =
            DependencyProperty.Register(nameof(AllDayAppointmentSpacing), typeof(int), typeof(MultiDayViewSettings), new PropertyMetadata(DefaultAllDayAppointmentSpacing, OnAllDayAppointmentSpacingChanged));

        /// <summary>
        /// Identifies the <c cref="ShowAllDayAreaProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowAllDayAreaProperty =
            DependencyProperty.Register(nameof(ShowAllDayArea), typeof(bool), typeof(MultiDayViewSettings), new PropertyMetadata(true, OnShowAllDayAreaChanged));

        /// <summary>
        /// Identifies the <c cref="ShowCurrentTimeIndicator"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowCurrentTimeIndicatorProperty =
            DependencyProperty.Register(nameof(ShowCurrentTimeIndicator), typeof(bool), typeof(MultiDayViewSettings), new PropertyMetadata(true, OnShowCurrentTimeIndicator));

        /// <summary>
        /// Identifies the <see cref="MultiDayViewHeaderText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MultiDayViewHeaderTextProperty =
            DependencyProperty.Register(nameof(MultiDayViewHeaderText), typeof(string), typeof(MultiDayViewSettings), new PropertyMetadata(string.Empty, OnMultiDayViewHeaderTextPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="NavigationStep"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationStepProperty =
            DependencyProperty.Register(nameof(NavigationStep), typeof(int), typeof(MultiDayViewSettings), new PropertyMetadata(DefaultNavigationStep, OnNavigationStepPropertyChanged));

        /// <summary>
        /// Identifies the SpecialSlotsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty SpecialSlotsSourceProperty =
            DependencyProperty.Register(nameof(SpecialSlotsSource), typeof(IEnumerable<Slot>), typeof(MultiDayViewSettings), new PropertyMetadata(null, OnSpecialSlotsSourcePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="SpecialSlotStyleSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SpecialSlotStyleSelectorProperty =
            DependencyProperty.Register(nameof(SpecialSlotStyleSelector), typeof(StyleSelector), typeof(MultiDayViewSettings), new PropertyMetadata(null, OnSpecialSlotStyleSelectorPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="TimeRulerItemStyleSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TimeRulerItemStyleSelectorProperty =
            DependencyProperty.Register(nameof(TimeRulerItemStyleSelector), typeof(CalendarTimeRulerItemStyleSelector), typeof(MultiDayViewSettings), new PropertyMetadata(null, OnTimeRulerItemStyleSelectorChanged));

        /// <summary>
        /// Identifies the <see cref="CurrentTimeIndicatorStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentTimeIndicatorStyleProperty =
            DependencyProperty.Register(nameof(CurrentTimeIndicatorStyle), typeof(Style), typeof(MultiDayViewSettings), new PropertyMetadata(null, OnCurrentTimeIndicatorStyleChanged));

        /// <summary>
        /// Identifies the <see cref="TodaySlotStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TodaySlotStyleProperty =
            DependencyProperty.Register(nameof(TodaySlotStyle), typeof(Style), typeof(MultiDayViewSettings), new PropertyMetadata(null, OnTodaySlotStyleChanged));

        /// <summary>
        /// Identifies the <see cref="AllDayAreaBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllDayAreaBackgroundProperty =
            DependencyProperty.Register(nameof(AllDayAreaBackground), typeof(Brush), typeof(MultiDayViewSettings), new PropertyMetadata(null, OnAllDayAreaBackgroundChanged));

        /// <summary>
        /// Identifies the <see cref="TimelineBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TimelineBackgroundProperty =
            DependencyProperty.Register(nameof(TimelineBackground), typeof(Brush), typeof(MultiDayViewSettings), new PropertyMetadata(null, OnTimelineBackgroundChanged));

        /// <summary>
        /// Identifies the <see cref="AllDayAreaBorderStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllDayAreaBorderStyleProperty =
            DependencyProperty.Register(nameof(AllDayAreaBorderStyle), typeof(Style), typeof(MultiDayViewSettings), new PropertyMetadata(null, OnAllDayAreaBorderStyleChanged));

        /// <summary>
        /// Identifies the <see cref="AllDayAreaTextStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllDayAreaTextStyleProperty =
            DependencyProperty.Register(nameof(AllDayAreaTextStyle), typeof(Style), typeof(MultiDayViewSettings), new PropertyMetadata(null, OnAllDayAreaTextStyleChanged));

        /// <summary>
        /// Identifies the <see cref="AllDayAreaText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllDayAreaTextProperty =
            DependencyProperty.Register(nameof(AllDayAreaText), typeof(string), typeof(MultiDayViewSettings), new PropertyMetadata(DefaultAllDayText, OnAllDayAreaTextChanged));

        /// <summary>
        /// Identifies the <see cref="WeekendsVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WeekendsVisibleProperty =
            DependencyProperty.Register(nameof(WeekendsVisible), typeof(bool), typeof(MultiDayViewSettings), new PropertyMetadata(true, OnWeekendsVisibleChanged));

        internal const string DefaultAllDayText = "All-day";
        internal RadCalendar owner;
        internal MultiDayViewUpdateFlag updateFlag;
        internal DispatcherTimer timer;

        internal StyleSelector defaultSpecialSlotStyleSelector;
        internal CalendarTimeRulerItemStyleSelector defaultTimeRulerItemStyleSelector;
        internal Style defaultCurrentTimeIndicatorStyle;
        internal Style defaultAllDayAreaBorderStyle;
        internal Style defaultAllDayAreaTextStyle;
        internal Style defaulTodaySlotStyle;

        private const int DefaultMultiDayViewVisibleDays = 7;
        private const int DefaultNavigationStep = 7;
        private const int MinimumtMultiDayViewVisibleDays = 1;
        private const int DefaultTimeLinesInterval = 90;
        private const int MinimumTicksPerDay = 15;
        private const int DefaultAllDayAppointmentHeight = 30;
        private const int DefaultAllDayAppointmentSpacing = 2;
        private const int DefaultAllDayMaxVisibleRows = 2;

        private StyleSelector specialSlotStyleSelectorCache;
        private CalendarTimeRulerItemStyleSelector timeRulerItemStyleSelectorCache;
        private Style currentTimeIndicatorStyleCache;
        private Style allDayAreaBorderStyleCache;
        private Style allDayAreaTextStyleCache;
        private Style todaySlotStyleCache;
        private WeakCollectionChangedListener specialSlotsCollectionChangedListener;
        private List<WeakPropertyChangedListener> specialSlotsPropertyChangedListeners = new List<WeakPropertyChangedListener>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiDayViewSettings"/> class.
        /// </summary>
        public MultiDayViewSettings()
        {
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(1.0);
            this.timer.Tick += this.TimerCallback;
        }

        /// <summary>
        /// Gets or sets the step of the week view part visualized when multi-day view is set as mode for the Calendar.
        /// </summary>
        public int VisibleDays
        {
            get
            {
                return (int)this.GetValue(VisibleDaysProperty);
            }
            set
            {
                this.SetValue(VisibleDaysProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tick length of the TimeRuler. 
        /// </summary>
        public TimeSpan TimerRulerTickLength
        {
            get
            {
                return (TimeSpan)this.GetValue(TimerRulerTickLengthProperty);
            }
            set
            {
                this.SetValue(TimerRulerTickLengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets start time of the day of the time ruler.
        /// </summary>
        public TimeSpan DayStartTime
        {
            get
            {
                return (TimeSpan)this.GetValue(DayStartTimeProperty);
            }
            set
            {
                this.SetValue(DayStartTimeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets end time of the day of the time ruler.
        /// </summary>
        public TimeSpan DayEndTime
        {
            get
            {
                return (TimeSpan)this.GetValue(DayEndTimeProperty);
            }
            set
            {
                this.SetValue(DayEndTimeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the spacing between the time lines in the day area.
        /// </summary>
        public int TimeLinesSpacing
        {
            get
            {
                return (int)this.GetValue(TimeLinesSpacingProperty);
            }
            set
            {
                this.SetValue(TimeLinesSpacingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum height of the appointments in the all day area.
        /// </summary>
        public int AllDayAppointmentMinHeight
        {
            get
            {
                return (int)this.GetValue(AllDayAppointmentMinHeightProperty);
            }
            set
            {
                this.SetValue(AllDayAppointmentMinHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of rows visible in the all day area.
        /// </summary>
        public int AllDayMaxVisibleRows
        {
            get
            {
                return (int)this.GetValue(AllDayMaxVisibleRowsProperty);
            }
            set
            {
                this.SetValue(AllDayMaxVisibleRowsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the spacing between the appointments in the all day area.
        /// </summary>
        public int AllDayAppointmentSpacing
        {
            get
            {
                return (int)this.GetValue(AllDayAppointmentSpacingProperty);
            }
            set
            {
                this.SetValue(AllDayAppointmentSpacingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the all day area should be visualized or not.
        /// </summary>
        public bool ShowAllDayArea
        {
            get
            {
                return (bool)this.GetValue(ShowAllDayAreaProperty);
            }
            set
            {
                this.SetValue(ShowAllDayAreaProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether current time indicator should be visualized.
        /// </summary>
        public bool ShowCurrentTimeIndicator
        {
            get
            {
                return (bool)this.GetValue(ShowCurrentTimeIndicatorProperty);
            }
            set
            {
                this.SetValue(ShowCurrentTimeIndicatorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the format for the navigation header of the multiday view of this calendar instance.
        /// </summary>
        public string MultiDayViewHeaderText
        {
            get
            {
                return (string)this.GetValue(MultiDayViewHeaderTextProperty);
            }
            set
            {
                this.SetValue(MultiDayViewHeaderTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the step that is used for navigating between the days. Please, note that the step could not be higher than the <see cref="VisibleDays" />.
        /// </summary>
        public int NavigationStep
        {
            get
            {
                return (int)this.GetValue(NavigationStepProperty);
            }
            set
            {
                this.SetValue(NavigationStepProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the special slots source.
        /// </summary>
        /// <value>The special slots source.</value>
        public IEnumerable<Slot> SpecialSlotsSource
        {
            get
            {
                return (IEnumerable<Slot>)this.GetValue(SpecialSlotsSourceProperty);
            }
            set
            {
                this.SetValue(SpecialSlotsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the StyleSelector that will be used for setting custom style-selection logic for a style that is applied
        /// to each generated SpecialSlot.
        /// </summary>
        public StyleSelector SpecialSlotStyleSelector
        {
            get
            {
                return this.specialSlotStyleSelectorCache;
            }
            set
            {
                this.SetValue(SpecialSlotStyleSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the CalendarTimeRulerItemStyleSelector that will be used for setting custom style for the items in the time ruler.
        /// </summary>
        public CalendarTimeRulerItemStyleSelector TimeRulerItemStyleSelector
        {
            get
            {
                return this.timeRulerItemStyleSelectorCache;
            }
            set
            {
                this.SetValue(TimeRulerItemStyleSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Style that gets applied on the current time indicator.
        /// </summary>
        public Style CurrentTimeIndicatorStyle
        {
            get
            {
                return this.currentTimeIndicatorStyleCache;
            }
            set
            {
                this.SetValue(CurrentTimeIndicatorStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Style that gets applied on the current time indicator.
        /// </summary>
        public Style TodaySlotStyle
        {
            get
            {
                return this.todaySlotStyleCache;
            }
            set
            {
                this.SetValue(TodaySlotStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Background of the all-day area.
        /// </summary>
        public Brush AllDayAreaBackground
        {
            get
            {
                return (Brush)GetValue(AllDayAreaBackgroundProperty);
            }
            set
            {
                this.SetValue(AllDayAreaBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Background of the timeline area.
        /// </summary>
        public Brush TimelineBackground
        {
            get
            {
                return (Brush)GetValue(TimelineBackgroundProperty);
            }
            set
            {
                this.SetValue(TimelineBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Style that gets applied on the Border placed below the all day area.
        /// </summary>
        public Style AllDayAreaBorderStyle
        {
            get
            {
                return this.allDayAreaBorderStyleCache;
            }
            set
            {
                this.SetValue(AllDayAreaBorderStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Style that gets applied on the text of the all day area.
        /// </summary>
        public Style AllDayAreaTextStyle
        {
            get
            {
                return this.allDayAreaTextStyleCache;
            }
            set
            {
                this.SetValue(AllDayAreaTextStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text that gets visualized on the all day area.
        /// </summary>
        public string AllDayAreaText
        {
            get
            {
                return (string)GetValue(AllDayAreaTextProperty);
            }
            set
            {
                this.SetValue(AllDayAreaTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the weekends need to be visualized.
        /// </summary>
        public bool WeekendsVisible
        {
            get
            {
                return (bool)this.GetValue(WeekendsVisibleProperty);
            }
            set 
            {
                this.SetValue(WeekendsVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="RadCalendar" /> has been loaded.
        /// </summary>
        public bool IsOwnerLoaded
        {
            get
            {
                if (this.owner != null && this.owner.IsLoaded && this.owner.IsTemplateApplied 
                    && this.owner.Model.IsTreeLoaded)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Implementation of the <see cref="ICollectionChangedListener" /> interface.
        /// </summary>
        /// <param name="sender">The collection sending the event.</param>
        /// <param name="e">The event args.</param>
        public void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Slot slot in e.NewItems)
                    {
                        WeakPropertyChangedListener newListener = WeakPropertyChangedListener.CreateIfNecessary(slot, this);
                        if (newListener != null)
                        {
                            this.specialSlotsPropertyChangedListeners.Add(newListener);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (this.specialSlotsPropertyChangedListeners != null && this.specialSlotsPropertyChangedListeners.Count > 0)
                    {
                        foreach (Slot slot in e.OldItems)
                        {
                            WeakPropertyChangedListener oldPropertyListener = this.specialSlotsPropertyChangedListeners[e.OldStartingIndex];
                            if (oldPropertyListener != null)
                            {
                                this.specialSlotsPropertyChangedListeners.Remove(oldPropertyListener);
                                oldPropertyListener.Detach();
                                oldPropertyListener = null;
                            }
                        }
                    }

                    this.owner.timeRulerLayer?.RecycleSlots(e.OldItems.Cast<Slot>());
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    WeakPropertyChangedListener propertyListener = this.specialSlotsPropertyChangedListeners[e.OldStartingIndex];
                    if (propertyListener != null)
                    {
                        this.specialSlotsPropertyChangedListeners.Remove(propertyListener);
                        propertyListener.Detach();
                        propertyListener = null;
                    }

                    WeakPropertyChangedListener listener = WeakPropertyChangedListener.CreateIfNecessary(e.NewItems[0], this);
                    if (listener != null)
                    {
                        this.specialSlotsPropertyChangedListeners.Add(listener);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }

            if (sender == this.SpecialSlotsSource)
            {
                this.Invalide(MultiDayViewUpdateFlag.AffectsSpecialSlots);
            }
        }

        /// <summary>
        /// Implementation of the <see cref="IPropertyChangedListener" /> interface.
        /// </summary>
        /// <param name="sender">The sender of the property changed.</param>
        /// <param name="e">The arguments of the event.</param>
        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Slot)
            {
                this.Invalide(MultiDayViewUpdateFlag.AffectsSpecialSlots);
            }
        }

        internal void SetDefaultStyleValues()
        {
            ResourceDictionary dictionary = RadCalendar.MultiDayViewResources;
            this.defaultTimeRulerItemStyleSelector = this.defaultTimeRulerItemStyleSelector ?? (CalendarTimeRulerItemStyleSelector)dictionary["CalendarTimeRulerItemStyleSelector"];
            this.defaultCurrentTimeIndicatorStyle = this.defaultCurrentTimeIndicatorStyle ?? (Style)dictionary["CurrentTimeIndicatorStyle"];
            this.defaultAllDayAreaBorderStyle = this.defaultAllDayAreaBorderStyle ?? (Style)dictionary["AllDayAreaBorderStyle"];
            this.defaultAllDayAreaTextStyle = this.defaultAllDayAreaTextStyle ?? (Style)dictionary["DefaultAllDayTextBlockStyle"];
            this.defaulTodaySlotStyle = this.defaulTodaySlotStyle ?? (Style)dictionary["TodaySlotStyle"];
        }

        internal void DetachEvents()
        {
            if (this.timer != null)
            {
                this.timer.Tick -= this.TimerCallback;
            }

            this.specialSlotsCollectionChangedListener?.Detach();
            if (this.specialSlotsPropertyChangedListeners != null && this.specialSlotsPropertyChangedListeners.Count > 0)
            {
                foreach (WeakPropertyChangedListener weakPropertyChangedListener in this.specialSlotsPropertyChangedListeners)
                {
                    weakPropertyChangedListener.Detach();
                }
            }
        }

        internal void Invalide(MultiDayViewUpdateFlag flag)
        {
            RadCalendar calendar = this.owner;
            if (calendar != null && calendar.IsTemplateApplied && calendar.Model.IsTreeLoaded)
            {
                if (calendar.IsLoaded)
                {
                    CalendarMultiDayViewModel multiDayViewModel = this.owner.Model.multiDayViewModel;
                    if (flag == MultiDayViewUpdateFlag.All)
                    {
                        multiDayViewModel.CalendarCells?.Clear();
                        multiDayViewModel.CalendarHeaderCells?.Clear();
                        multiDayViewModel.CalendarDecorations?.Clear();
                        multiDayViewModel.allDayAppointmentInfos?.Clear();
                        multiDayViewModel.appointmentInfos?.Clear();

                        calendar.contentLayer?.RecycleAllVisuals();
                    }

                    if (flag == MultiDayViewUpdateFlag.AffectsTimeRuler)
                    {
                        this.RecycleTimeRulerVisuals();
                        multiDayViewModel.timeRulerItems?.Clear();
                        multiDayViewModel.timerRulerLines?.Clear();
                        multiDayViewModel.appointmentInfos?.Clear();
                    }

                    if (flag == MultiDayViewUpdateFlag.AffectsAppointments)
                    {
                        multiDayViewModel.appointmentInfos?.Clear();
                        multiDayViewModel.allDayAppointmentInfos?.Clear();
                        calendar.allDayAreaLayer?.RecycleAppointments();
                    }

                    calendar.Model.multiDayViewModel.updateFlag = flag;
                    calendar.timeRulerLayer.shouldArrange = true;
                    calendar.allDayAreaLayer.shouldArrange = true;
                }

                calendar.Invalidate();
            }
        }

        private static void OnVisibleDaysChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;

            int value = (int)args.NewValue;
            if (value > DefaultMultiDayViewVisibleDays)
            {
                settings.ChangePropertyInternally(MultiDayViewSettings.VisibleDaysProperty, DefaultMultiDayViewVisibleDays);
            }
            else if (value < MinimumtMultiDayViewVisibleDays)
            {
                settings.ChangePropertyInternally(MultiDayViewSettings.VisibleDaysProperty, MinimumtMultiDayViewVisibleDays);
            }
            else
            {
                settings.Invalide(MultiDayViewUpdateFlag.All);
                settings.owner?.UpdateNavigationHeaderContent();

                if (value < settings.NavigationStep)
                {
                    settings.ChangePropertyInternally(MultiDayViewSettings.NavigationStepProperty, value);
                }
            }
        }

        private static void OnTimerRulerTickLengthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;

            long ticks = ((TimeSpan)args.NewValue).Ticks;
            if (ticks > TimeSpan.TicksPerDay)
            {
                settings.ChangePropertyInternally(MultiDayViewSettings.TimerRulerTickLengthProperty, TimeSpan.TicksPerDay);
            }
            else if (ticks < TimeSpan.FromMinutes(MinimumTicksPerDay).Ticks)
            {
                settings.ChangePropertyInternally(MultiDayViewSettings.TimerRulerTickLengthProperty, TimeSpan.FromMinutes(MinimumTicksPerDay));
            }
            else
            {
                settings.Invalide(MultiDayViewUpdateFlag.AffectsTimeRuler);
            }
        }

        private static void OnDayStartTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;

            long ticks = ((TimeSpan)args.NewValue).Ticks;
            if (ticks > settings.DayEndTime.Ticks)
            {
                throw new ArgumentException("Start date must be earlier than end date.");
            }

            settings.UpdateDayRange(ticks, MultiDayViewSettings.DayStartTimeProperty);
        }

        private static void OnDayEndTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;

            long ticks = ((TimeSpan)args.NewValue).Ticks;
            if (ticks < settings.DayStartTime.Ticks)
            {
                throw new ArgumentException("Start date must be earlier than end date.");
            }

            settings.UpdateDayRange(ticks, MultiDayViewSettings.DayEndTimeProperty);
        }

        private static void OnTimeLinesSpacingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;
            settings.Invalide(MultiDayViewUpdateFlag.AffectsTimeRuler);
        }

        private static void OnShowAllDayAreaChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;
            settings.Invalide(MultiDayViewUpdateFlag.AffectsAppointments);
        }

        private static void OnAllDayAppointmentMinHeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;
            settings.Invalide(MultiDayViewUpdateFlag.AffectsAppointments);
        }

        private static void OnAllDayMaxVisibleRowsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;
            settings.Invalide(MultiDayViewUpdateFlag.AffectsAppointments);
        }

        private static void OnAllDayAppointmentSpacingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;
            settings.Invalide(MultiDayViewUpdateFlag.AffectsAppointments);
        }

        private static void OnShowCurrentTimeIndicator(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;
            bool shouldVisualizeCurrentTimeIndicator = (bool)args.NewValue;
            if (shouldVisualizeCurrentTimeIndicator)
            {
                settings.timer.Start();
            }
            else
            {
                settings.timer.Stop();
            }

            settings.Invalide(MultiDayViewUpdateFlag.AffectsCurrentTimeIndicator);
        }

        private static void OnMultiDayViewHeaderTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;
            if (settings.IsOwnerLoaded)
            {
                settings.owner.UpdateNavigationHeaderContent();
            }
        }

        private static void OnNavigationStepPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;

            int navigationStep = (int)args.NewValue;
            if (navigationStep > settings.VisibleDays)
            {
                settings.ChangePropertyInternally(MultiDayViewSettings.NavigationStepProperty, settings.VisibleDays);
            }
            else if (navigationStep < MinimumtMultiDayViewVisibleDays)
            {
                settings.ChangePropertyInternally(MultiDayViewSettings.NavigationStepProperty, MinimumtMultiDayViewVisibleDays);
            }
        }

        private static void OnSpecialSlotsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;
            INotifyCollectionChanged oldSlotsSource = args.OldValue as INotifyCollectionChanged;
            if (oldSlotsSource != null)
            {
                var listener = settings.specialSlotsCollectionChangedListener;
                if (listener != null)
                {
                    listener.Detach();
                    listener = null;
                }

                int count = settings.specialSlotsPropertyChangedListeners != null ? settings.specialSlotsPropertyChangedListeners.Count : 0;
                while (count > 0)
                {
                    var propertyListener = settings.specialSlotsPropertyChangedListeners[0];
                    settings.specialSlotsPropertyChangedListeners.RemoveAt(0);
                    propertyListener.Detach();
                    propertyListener = null;
                    count--;
                }
            }

            INotifyCollectionChanged newSlotsSource = args.NewValue as INotifyCollectionChanged;
            if (newSlotsSource != null)
            {
                settings.specialSlotsCollectionChangedListener = WeakCollectionChangedListener.CreateIfNecessary(newSlotsSource, settings);

                foreach (Slot slot in (IEnumerable<Slot>)newSlotsSource)
                {
                    var listener = WeakPropertyChangedListener.CreateIfNecessary(slot, settings);
                    if (listener != null)
                    {
                        settings.specialSlotsPropertyChangedListeners.Add(listener);
                    }
                }
            }

            if (oldSlotsSource != null && settings.IsOwnerLoaded)
            {
                settings.owner.timeRulerLayer.RecycleSlots((IEnumerable<Slot>)oldSlotsSource);
            }

            settings.Invalide(MultiDayViewUpdateFlag.AffectsSpecialSlots);
        }

        private static void OnSpecialSlotStyleSelectorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;
            settings.specialSlotStyleSelectorCache = (StyleSelector)args.NewValue;

            if (settings.IsOwnerLoaded)
            {
                settings.owner.timeRulerLayer.UpdateSlots(settings.SpecialSlotsSource);
            }
        }

        private static void OnTimeRulerItemStyleSelectorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;
            settings.timeRulerItemStyleSelectorCache = (CalendarTimeRulerItemStyleSelector)args.NewValue;

            if (settings.IsOwnerLoaded)
            {
                settings.owner.timeRulerLayer.UpdateUI();
            }
        }

        private static void OnCurrentTimeIndicatorStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;
            settings.currentTimeIndicatorStyleCache = (Style)args.NewValue;

            if (settings.IsOwnerLoaded)
            {
                settings.owner.timeRulerLayer.UpdateCurrentTimeIndicator();
            }
        }

        private static void OnTodaySlotStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;
            settings.todaySlotStyleCache = (Style)args.NewValue;
            if (settings.IsOwnerLoaded)
            {
                settings.owner.timeRulerLayer.UpdateTodaySlot();
            }
        }

        private static void OnAllDayAreaBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;
            settings.UpdateAppearance(MultiDayViewUpdateFlag.AffectsAppointments);
        }

        private static void OnTimelineBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;
            settings.UpdateAppearance(MultiDayViewUpdateFlag.AffectsTimeRuler);
        }

        private static void OnAllDayAreaBorderStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;
            settings.allDayAreaBorderStyleCache = (Style)args.NewValue;

            if (settings.IsOwnerLoaded)
            {
                CalendarModel model = settings.owner.Model;
                CalendarMultiDayViewModel multiDayViewModel = model.multiDayViewModel;
                settings.owner.timeRulerLayer.UpdateTimeRulerDecorations(multiDayViewModel, model.AreDayNamesVisible);
            }
        }

        private static void OnAllDayAreaTextStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;
            settings.allDayAreaTextStyleCache = (Style)args.NewValue;
            MultiDayViewSettings.UpdateAllDayAreaText(settings);
        }

        private static void OnAllDayAreaTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;
            MultiDayViewSettings.UpdateAllDayAreaText(settings);
        }

        private static void OnWeekendsVisibleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MultiDayViewSettings settings = (MultiDayViewSettings)sender;
            settings.Invalide(MultiDayViewUpdateFlag.All);
            settings.owner?.UpdateNavigationHeaderContent();
        }

        private static void UpdateAllDayAreaText(MultiDayViewSettings settings)
        {
            if (settings.IsOwnerLoaded)
            {
                CalendarModel model = settings.owner.Model;
                CalendarMultiDayViewModel multiDayViewModel = model.multiDayViewModel;
                settings.owner.timeRulerLayer.UpdateTimeRulerAllDayText(multiDayViewModel.allDayLabelLayout);
            }
        }

        private void UpdateDayRange(long ticks, DependencyProperty propertForUpdate)
        {
            if (ticks > TimeSpan.TicksPerDay)
            {
                this.ChangePropertyInternally(propertForUpdate, TimeSpan.TicksPerDay);
            }
            else if (ticks < 0)
            {
                this.ChangePropertyInternally(propertForUpdate, TimeSpan.Zero);
            }
            else
            {
                this.Invalide(MultiDayViewUpdateFlag.AffectsTimeRuler);
            }
        }

        private void UpdateAppearance(MultiDayViewUpdateFlag flag)
        {
            RadCalendar calendar = this.owner;
            if (calendar != null && calendar.IsTemplateApplied && calendar.Model.IsTreeLoaded)
            {
                if (calendar.IsLoaded)
                {
                    if (flag == MultiDayViewUpdateFlag.AffectsAppointments)
                    {
                        XamlAllDayAreaLayer allDayArea = this.owner.allDayAreaLayer;
                        allDayArea.UpdatePanelBackground(this.AllDayAreaBackground);
                    }

                    if (flag == MultiDayViewUpdateFlag.AffectsTimeRuler)
                    {
                        XamlMultiDayViewLayer timeRulerArea = this.owner.timeRulerLayer;
                        timeRulerArea.UpdatePanelsBackground(this.TimelineBackground);
                    }
                }
            }
        }

        private void TimerCallback(object sender, object e)
        {
            this.Invalide(MultiDayViewUpdateFlag.AffectsCurrentTimeIndicator);
        }

        private void RecycleTimeRulerVisuals()
        {
            this.owner.timeRulerLayer?.RecycleTimeRulerLines(this.owner.Model.multiDayViewModel.timerRulerLines);
            this.owner.timeRulerLayer?.RecycleTimeRulerItems(this.owner.Model.multiDayViewModel.timeRulerItems);
        }
    }
}
