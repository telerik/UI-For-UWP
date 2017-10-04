using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Telerik.UI.Xaml.Controls.Input.Calendar.Commands;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents a control that enables a user to select a date by using a visual calendar display.
    /// </summary>
    [TemplatePart(Name = "PART_CalendarViewHost", Type = typeof(CalendarViewHost))]
    public partial class RadCalendar : RadControl, IView, ICultureAware
    {
        /// <summary>
        /// Identifies the <see cref="DisplayDateStart"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayDateStartProperty =
            DependencyProperty.Register(nameof(DisplayDateStart), typeof(DateTime), typeof(RadCalendar), new PropertyMetadata(new DateTime(1900, 1, 1), OnDisplayDateStartPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="DisplayDateEnd"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayDateEndProperty =
            DependencyProperty.Register(nameof(DisplayDateEnd), typeof(DateTime), typeof(RadCalendar), new PropertyMetadata(new DateTime(2099, 12, 31), OnDisplayDateEndPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="DisplayDate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayDateProperty =
            DependencyProperty.Register(nameof(DisplayDate), typeof(DateTime), typeof(RadCalendar), new PropertyMetadata(DateTime.Today, OnDisplayDatePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="DisplayMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register(nameof(DisplayMode), typeof(CalendarDisplayMode), typeof(RadCalendar), new PropertyMetadata(CalendarDisplayMode.MonthView, OnDisplayModePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="GridLinesBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GridLinesBrushProperty =
            DependencyProperty.Register(nameof(GridLinesBrush), typeof(Brush), typeof(RadCalendar), new PropertyMetadata(null, OnGridLinesBrushPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="GridLinesThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GridLinesThicknessProperty =
            DependencyProperty.Register(nameof(GridLinesThickness), typeof(double), typeof(RadCalendar), new PropertyMetadata(2d, OnGridLinesThicknessPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="GridLinesVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GridLinesVisibilityProperty =
            DependencyProperty.Register(nameof(GridLinesVisibility), typeof(GridLinesVisibility), typeof(RadCalendar), new PropertyMetadata(GridLinesVisibility.Both, OnGridLinesVisibilityPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="SelectionMode"/> dependency property. 
        /// </summary> 
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(nameof(SelectionMode), typeof(CalendarSelectionMode), typeof(RadCalendar), new PropertyMetadata(CalendarSelectionMode.Single, OnSelectionModePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="PointerOverCellStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointerOverCellStyleProperty =
            DependencyProperty.Register(nameof(PointerOverCellStyle), typeof(CalendarCellStyle), typeof(RadCalendar), new PropertyMetadata(null, OnPointerOverCellStylePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="NormalCellStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NormalCellStyleProperty =
            DependencyProperty.Register(nameof(NormalCellStyle), typeof(CalendarCellStyle), typeof(RadCalendar), new PropertyMetadata(null, OnNormalCellStylePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="AnotherViewCellStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AnotherViewCellStyleProperty =
            DependencyProperty.Register(nameof(AnotherViewCellStyle), typeof(CalendarCellStyle), typeof(RadCalendar), new PropertyMetadata(null, OnAnotherViewCellStylePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="BlackoutCellStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BlackoutCellStyleProperty =
            DependencyProperty.Register(nameof(BlackoutCellStyle), typeof(CalendarCellStyle), typeof(RadCalendar), new PropertyMetadata(null, OnBlackoutCellStylePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="BlackoutCellStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedCellStyleProperty =
            DependencyProperty.Register(nameof(SelectedCellStyle), typeof(CalendarCellStyle), typeof(RadCalendar), new PropertyMetadata(null, OnSelectedCellStylePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="HighlightedCellStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HighlightedCellStyleProperty =
            DependencyProperty.Register(nameof(HighlightedCellStyle), typeof(CalendarCellStyle), typeof(RadCalendar), new PropertyMetadata(null, OnHighlightedCellStylePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="CurrentCellStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentCellStyleProperty =
            DependencyProperty.Register(nameof(CurrentCellStyle), typeof(CalendarCellStyle), typeof(RadCalendar), new PropertyMetadata(null, OnCurrentCellStylePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="DayNameCellStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DayNameCellStyleProperty =
            DependencyProperty.Register(nameof(DayNameCellStyle), typeof(CalendarCellStyle), typeof(RadCalendar), new PropertyMetadata(null, OnDayNameCellStylePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="WeekNumberCellStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WeekNumberCellStyleProperty =
            DependencyProperty.Register(nameof(WeekNumberCellStyle), typeof(CalendarCellStyle), typeof(RadCalendar), new PropertyMetadata(null, OnWeekNumberCellStylePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="CellStyleSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CellStyleSelectorProperty =
            DependencyProperty.Register(nameof(CellStyleSelector), typeof(CalendarCellStyleSelector), typeof(RadCalendar), new PropertyMetadata(null, OnCellStyleSelectorPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="HeaderContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderContentTemplateProperty =
            DependencyProperty.Register(nameof(HeaderContentTemplate), typeof(DataTemplate), typeof(RadCalendar), new PropertyMetadata(null, OnHeaderContentTemplatePropertyChanged));
        
        /// <summary>
        /// Identifies the <see cref="HeaderContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderContentProperty =
            DependencyProperty.Register(nameof(HeaderContent), typeof(object), typeof(RadCalendar), new PropertyMetadata(null, OnHeaderContentPropertyChanged));
        
        /// <summary>
        /// Identifies the <see cref="CellStateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CellStateSelectorProperty =
            DependencyProperty.Register(nameof(CellStateSelector), typeof(CalendarCellStateSelector), typeof(RadCalendar), new PropertyMetadata(null, OnCellStateSelectorPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="DayNameCellStyleSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DayNameCellStyleSelectorProperty =
            DependencyProperty.Register(nameof(DayNameCellStyleSelector), typeof(CalendarDayNameCellStyleSelector), typeof(RadCalendar), new PropertyMetadata(null, OnDayNameCellStyleSelectorPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="WeekNumberCellStyleSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WeekNumberCellStyleSelectorProperty =
            DependencyProperty.Register(nameof(WeekNumberCellStyleSelector), typeof(CalendarWeekNumberCellStyleSelector), typeof(RadCalendar), new PropertyMetadata(null, OnWeekNumberStyleSelectorPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="IsTodayHighlighted"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTodayHighlightedProperty =
            DependencyProperty.Register(nameof(IsTodayHighlighted), typeof(bool), typeof(RadCalendar), new PropertyMetadata(true, OnIsTodayHighlightedPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="MonthViewHeaderFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MonthViewHeaderFormatProperty =
            DependencyProperty.Register(nameof(MonthViewHeaderFormat), typeof(string), typeof(RadCalendar), new PropertyMetadata(DefaultMonthViewHeaderFormatString, OnCalendarViewHeaderFormatPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="YearViewHeaderFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty YearViewHeaderFormatProperty =
            DependencyProperty.Register(nameof(YearViewHeaderFormat), typeof(string), typeof(RadCalendar), new PropertyMetadata(DefaultYearViewHeaderFormatString, OnCalendarViewHeaderFormatPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="DecadeViewHeaderFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DecadeViewHeaderFormatProperty =
            DependencyProperty.Register(nameof(DecadeViewHeaderFormat), typeof(string), typeof(RadCalendar), new PropertyMetadata(DefaultDecadeViewHeaderFormatString, OnCalendarViewHeaderFormatPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="CenturyViewHeaderFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenturyViewHeaderFormatProperty =
            DependencyProperty.Register(nameof(CenturyViewHeaderFormat), typeof(string), typeof(RadCalendar), new PropertyMetadata(DefaultCenturyViewHeaderFormatString, OnCalendarViewHeaderFormatPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="MonthViewCellFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MonthViewCellFormatProperty =
            DependencyProperty.Register(nameof(MonthViewCellFormat), typeof(string), typeof(RadCalendar), new PropertyMetadata(CalendarModel.DefaultMonthViewCellFormatString, OnMonthViewCellFormatPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="YearViewCellFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty YearViewCellFormatProperty =
            DependencyProperty.Register(nameof(YearViewCellFormat), typeof(string), typeof(RadCalendar), new PropertyMetadata(CalendarModel.DefaultYearViewCellFormatString, OnYearViewCellFormatPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="DecadeViewCellFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DecadeViewCellFormatProperty =
            DependencyProperty.Register(nameof(DecadeViewCellFormat), typeof(string), typeof(RadCalendar), new PropertyMetadata(CalendarModel.DefaultDecadeViewCellFormatString, OnDecadeViewCellFormatPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="CenturyViewCellFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenturyViewCellFormatProperty =
            DependencyProperty.Register(nameof(CenturyViewCellFormat), typeof(string), typeof(RadCalendar), new PropertyMetadata(CalendarModel.DefaultCenturyViewCellFormatString, OnCenturyViewCellFormatPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="DayNamesVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DayNamesVisibilityProperty =
            DependencyProperty.Register(nameof(DayNamesVisibility), typeof(Visibility), typeof(RadCalendar), new PropertyMetadata(Visibility.Visible, OnDayNamesVisibilityPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="NavigationArrowsVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationArrowsVisibilityProperty =
            DependencyProperty.Register(nameof(NavigationArrowsVisibility), typeof(Visibility), typeof(RadCalendar), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the <see cref="WeekNumbersVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WeekNumbersVisibilityProperty =
            DependencyProperty.Register(nameof(WeekNumbersVisibility), typeof(Visibility), typeof(RadCalendar), new PropertyMetadata(Visibility.Collapsed, OnWeekNumbersVisibilityPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="DayNameFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DayNameFormatProperty =
            DependencyProperty.Register(nameof(DayNameFormat), typeof(CalendarDayNameFormat), typeof(RadCalendar), new PropertyMetadata(CalendarDayNameFormat.AbbreviatedName, OnDayNameFormatPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="WeekNumberFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WeekNumberFormatProperty =
            DependencyProperty.Register(nameof(WeekNumberFormat), typeof(string), typeof(RadCalendar), new PropertyMetadata(CalendarModel.DefaultWeekNumberFormatString, OnWeekNumberFormatPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="SelectedDateRange"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDateRangeProperty =
            DependencyProperty.Register(nameof(SelectedDateRange), typeof(object), typeof(RadCalendar), new PropertyMetadata(null, OnSelectedDateRangePropertyChanged));

        /// <summary>
        /// Identifies the <c cref="AppointmentSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AppointmentSourceProperty =
            DependencyProperty.Register(nameof(AppointmentSource), typeof(AppointmentSource), typeof(RadCalendar), new PropertyMetadata(null, OnAppointmentSourceChanged));

        /// <summary>
        /// Identifies the <c cref="AppointmentTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AppointmentTemplateSelectorProperty =
            DependencyProperty.Register(nameof(AppointmentTemplateSelector), typeof(AppointmentTemplateSelector), typeof(RadCalendar), new PropertyMetadata(null, OnAppointmentTemplateSelectorChanged));

        internal static readonly Size InfinitySize = new Size(double.PositiveInfinity, double.PositiveInfinity);

        internal readonly VisualStateService VisualStateService;
        internal readonly SelectionService SelectionService;
        internal readonly CurrencyService CurrencyService;

        internal XamlVisualStateLayer visualStateLayer;
        internal XamlDecorationLayer decorationLayer;
        internal XamlContentLayer contentLayer;
        internal XamlAppointmentLayer appointmentLayer;
        internal CalendarNavigationControl navigationPanel;
        internal List<CalendarDateRange> unattachedSelectedRanges;
        internal CalendarViewHost calendarViewHost;

        private const string DefaultMonthViewHeaderFormatString = "{0:MMMM yyyy}";
        private const string DefaultYearViewHeaderFormatString = "{0:yyyy}";
        private const string DefaultDecadeViewHeaderFormatString = "{0:yyyy} ~ {1:yyyy}";
        private const string DefaultCenturyViewHeaderFormatString = "{0:yyyy} ~ {1:yyyy}";

        private const string CalendarViewHostPartName = "PART_CalendarViewHost";
        private const string NavigationControlPanelName = "navigationControl";

        private readonly HitTestService hitTestService;
        private readonly InputService inputService;
        private CommandService commandService;

        private XamlHeaderContentLayer headerContentLayer;

        private CalendarModel model;
        private Size availableCalendarViewSize;

        private CalendarLayoutContext lastLayoutContext;
        private bool invalidateScheduled, invalidatePresentersScheduled;
        private bool arrangePassed;
        private CultureInfo currentCulture = CultureInfo.CurrentCulture;

        private bool isTodayHighlightedCache = true;
        private DateTime displayDateStartCache = new DateTime(1900, 1, 1);
        private DateTime displayDateEndCache = new DateTime(2099, 12, 31);
        private Brush gridLinesBrushCache;
        private CalendarCellStyleSelector cellStyleSelectorCache;
        private CalendarCellStateSelector cellStateSelectorCache;
        private CalendarDayNameCellStyleSelector dayNameCellStyleSelectorCache;
        private CalendarWeekNumberCellStyleSelector weekNumberCellStyleSelectorCache;
        private CalendarCellStyle pointerOverCellStyleCache, normalCellStyleCache, anotherViewCellStyleCache, blackoutCellStyleCache, selectedCellStyleCache, highlightedCellStyleCache, currentCellStyleCache;
        private CalendarCellStyle dayNameCellStyleCache, weekNumberCellStyleCache;

        private CalendarCellModel highlightedCellCache;
        private DateTime pointerOverDateCache;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RadCalendar"/> class.
        /// </summary>
        public RadCalendar()
        {
            this.DefaultStyleKey = typeof(RadCalendar);

            this.model = new CalendarModel(this);

            this.VisualStateService = new VisualStateService(this);
            this.SelectionService = new SelectionService(this);
            this.commandService = new CommandService(this);
            this.hitTestService = new HitTestService(this);
            this.inputService = new InputService(this);
            this.CurrencyService = new CurrencyService(this);
        }

        /// <summary>
        /// Occurs when the collection returned by the <see cref="SelectedDateRanges"/> property is changed.
        /// </summary>
        public event EventHandler<CurrentSelectionChangedEventArgs> SelectionChanged
        {
            add
            {
                this.SelectionService.SelectionChanged += value;
            }
            remove
            {
                this.SelectionService.SelectionChanged -= value;
            }
        }

        /// <summary>
        /// Gets the <see cref="CommandService"/> instance that manages the commanding behavior of this instance.
        /// </summary>
        public CommandService CommandService
        {
            get
            {
                return this.commandService;
            }
        }

        /// <summary>
        /// Gets the collection with all the custom commands registered with the <see cref="CommandService"/>. 
        /// Custom commands have higher priority than the built-in (default) ones.
        /// </summary>
        public CommandCollection<RadCalendar> Commands
        {
            get
            {
                return this.CommandService.UserCommands;
            }
        }

        /// <summary>
        /// Gets or sets the first date to be displayed. The current view will not allow 
        /// navigation to dates before this value.
        /// </summary>
        /// <value>
        /// The default value is 1900/1/1.
        /// </value>
        /// <remarks>
        /// Any date out of the display range is marked as a blackout date.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar x:Name="calendar"/&gt;
        /// </code>
        /// <code language="c#">
        /// this.calendar.DisplayDateStart = new DateTime(2010, 1, 1);
        /// </code>
        /// </example>
        public DateTime DisplayDateStart
        {
            get
            {
                return this.displayDateStartCache;
            }
            set
            {
                this.SetValue(DisplayDateStartProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an AppointmentSource object that will be used
        /// to display appointment data in the visual representation of a date.
        /// </summary>
        public AppointmentSource AppointmentSource
        {
            get
            {
                return (AppointmentSource)this.GetValue(RadCalendar.AppointmentSourceProperty);
            }

            set
            {
                this.SetValue(RadCalendar.AppointmentSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an AppointmentTemplateSelector object that will be used
        /// to display different templates for each appointment data in the visual representation of a date.
        /// </summary>
        public AppointmentTemplateSelector AppointmentTemplateSelector
        {
            get
            {
                return (AppointmentTemplateSelector)this.GetValue(RadCalendar.AppointmentTemplateSelectorProperty);
            }

            set
            {
                this.SetValue(RadCalendar.AppointmentTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the last date to be displayed. The current view will not allow navigation to dates after this value.
        /// </summary>
        /// <value>
        /// The default value is 2099/12/31.
        /// </value>
        /// <remarks>
        /// Any date out of the display range is marked as a blackout date.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar x:Name="calendar"/&gt;
        /// </code>
        /// <code language="c#">
        /// this.calendar.DisplayDateEnd = new DateTime(2030, 1, 1);
        /// </code>
        /// </example>
        public DateTime DisplayDateEnd
        {
            get
            {
                return this.displayDateEndCache;
            }
            set
            {
                this.SetValue(DisplayDateEndProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current date to display.
        /// </summary>
        /// <value>
        /// The default value is <see cref="System.DateTime.Today"/>.
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar x:Name="calendar"/&gt;
        /// </code>
        /// <code language="c#">
        /// this.calendar.DisplayDate = new DateTime(2030, 1, 1);
        /// </code>
        /// </example>
        public DateTime DisplayDate
        {
            get
            {
                return (DateTime)this.GetValue(DisplayDateProperty);
            }
            set
            {
                this.SetValue(DisplayDateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the display mode of the calendar control.
        /// </summary>
        /// <value>
        /// The default value is <see cref="CalendarDisplayMode.MonthView"/>.
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar DisplayMode="DecadeView"/&gt;
        /// </code>
        /// </example>
        /// <seealso cref="CalendarDisplayMode"/>
        public CalendarDisplayMode DisplayMode
        {
            get
            {
                return (CalendarDisplayMode)this.GetValue(DisplayModeProperty);
            }
            set
            {
                this.SetValue(DisplayModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the horizontal and vertical gridline decorations.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar GridLinesThickness="5"/&gt;
        /// </code>
        /// </example>
        /// <seealso cref="GridLinesBrush"/>
        /// <seealso cref="GridLinesVisibility"/>
        public double GridLinesThickness
        {
            get
            {
                return (double)this.GetValue(GridLinesThicknessProperty);
            }
            set
            {
                this.SetValue(GridLinesThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> value that defines the appearance of the horizontal and vertical grid lines of this instance.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar.GridLinesBrush&gt;
        ///     &lt;SolidColorBrush Color="Orange"/&gt;
        /// &lt;/telerikInput:RadCalendar.GridLinesBrush&gt;
        /// </code>
        /// </example>
        /// <seealso cref="GridLinesThickness"/>
        /// <seealso cref="GridLinesVisibility"/>
        public Brush GridLinesBrush
        {
            get
            {
                return this.gridLinesBrushCache;
            }
            set
            {
                this.SetValue(GridLinesBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Telerik.UI.Xaml.Controls.Primitives.GridLinesVisibility"/> value that defines which grid lines are currently displayed.
        /// </summary>
        /// <value>
        /// Default value: <see cref="Telerik.UI.Xaml.Controls.Primitives.GridLinesVisibility.Both"/>
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar GridLinesVisibility="Vertical"/&gt;
        /// </code>
        /// </example>
        /// <see cref="GridLinesBrush"/>
        /// <seealso cref="GridLinesThickness"/>
        public GridLinesVisibility GridLinesVisibility
        {
            get
            {
                return (GridLinesVisibility)this.GetValue(GridLinesVisibilityProperty);
            }
            set
            {
                this.SetValue(GridLinesVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="CalendarCellStyle"/> that defines the appearance settings 
        /// applied to calendar cells when the pointer is over them.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar&gt;
        ///     &lt;telerikInput:RadCalendar.PointerOverCellStyle&gt;
        ///         &lt;telerikInput:CalendarCellStyle&gt;
        ///             &lt;telerikInput:CalendarCellStyle.ContentStyle&gt;
        ///                 &lt;Style TargetType="TextBlock"&gt;
        ///                     &lt;Setter Property="Margin" Value="7,0,4,4"/&gt;
        ///                     &lt;Setter Property="FontSize" Value="18" /&gt;
        ///                     &lt;Setter Property="FontWeight" Value="Bold" /&gt;
        ///                     &lt;Setter Property="Foreground" Value="RoyalBlue" /&gt;
        ///                     &lt;Setter Property="TextAlignment" Value="Center" /&gt;
        ///                     &lt;Setter Property="VerticalAlignment" Value="Center" /&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerikInput:CalendarCellStyle.ContentStyle&gt;
        ///             &lt;telerikInput:CalendarCellStyle.DecorationStyle&gt;
        ///                 &lt;Style TargetType="Border"&gt;
        ///                     &lt;Setter Property="Background" Value="PaleGreen" /&gt;
        ///                     &lt;Setter Property="BorderBrush" Value="SkyBlue"/&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerikInput:CalendarCellStyle.DecorationStyle&gt;
        ///         &lt;/telerikInput:CalendarCellStyle&gt;
        ///     &lt;/telerikInput:RadCalendar.PointerOverCellStyle&gt;
        /// &lt;/telerikInput:RadCalendar&gt;
        /// </code>
        /// </example>
        /// <remarks>
        /// If you want to customize the appearance only of some special calendar cells that fulfill 
        /// this criteria, you should inherit from the <see cref="CalendarCellStyleSelector"/> class 
        /// and assign the custom instance to the <see cref="CellStyleSelector"/> property.
        /// </remarks>
        /// <seealso cref="CalendarCellStyle"/>
        public CalendarCellStyle PointerOverCellStyle
        {
            get
            {
                return this.pointerOverCellStyleCache;
            }
            set
            {
                this.SetValue(PointerOverCellStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="CalendarCellStyle"/> that defines the appearance settings 
        /// applied to all regular calendar cells that belong to the current view.
        /// </summary>
        /// <remarks>
        /// If you want to customize the appearance only of some special calendar cells that fulfill 
        /// this criteria, you should inherit from the <see cref="CalendarCellStyleSelector"/> class 
        /// and assign the custom instance to the <see cref="CellStyleSelector"/> property.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar&gt;
        ///     &lt;telerikInput:RadCalendar.NormalCellStyle&gt;
        ///         &lt;telerikInput:CalendarCellStyle&gt;
        ///             &lt;telerikInput:CalendarCellStyle.ContentStyle&gt;
        ///                 &lt;Style TargetType="TextBlock"&gt;
        ///                     &lt;Setter Property="Margin" Value="7,0,4,4"/&gt;
        ///                     &lt;Setter Property="FontSize" Value="18" /&gt;
        ///                     &lt;Setter Property="FontWeight" Value="Bold" /&gt;
        ///                     &lt;Setter Property="Foreground" Value="RoyalBlue" /&gt;
        ///                     &lt;Setter Property="TextAlignment" Value="Center" /&gt;
        ///                     &lt;Setter Property="VerticalAlignment" Value="Center" /&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerikInput:CalendarCellStyle.ContentStyle&gt;
        ///             &lt;telerikInput:CalendarCellStyle.DecorationStyle&gt;
        ///                 &lt;Style TargetType="Border"&gt;
        ///                     &lt;Setter Property="Background" Value="PaleGreen" /&gt;
        ///                     &lt;Setter Property="BorderBrush" Value="SkyBlue"/&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerikInput:CalendarCellStyle.DecorationStyle&gt;
        ///         &lt;/telerikInput:CalendarCellStyle&gt;
        ///     &lt;/telerikInput:RadCalendar.NormalCellStyle&gt;
        /// &lt;/telerikInput:RadCalendar&gt;
        /// </code>
        /// </example>
        /// <seealso cref="CalendarCellStyle"/>
        /// <seealso cref="CalendarCellStyleSelector"/>
        public CalendarCellStyle NormalCellStyle
        {
            get
            {
                return this.normalCellStyleCache;
            }
            set
            {
                this.SetValue(NormalCellStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="CalendarCellStyle"/> that defines the appearance settings applied to all calendar cells that belong to another view.
        /// </summary>
        /// <remarks>
        /// If you want to customize the appearance only of some special calendar cells that fulfill this criteria, you should inherit from 
        /// the <see cref="CalendarCellStyleSelector"/> class and assign the custom instance to the <see cref="CellStyleSelector"/> property.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar&gt;
        ///     &lt;telerikInput:RadCalendar.AnotherViewCellStyle&gt;
        ///         &lt;telerikInput:CalendarCellStyle&gt;
        ///             &lt;telerikInput:CalendarCellStyle.ContentStyle&gt;
        ///                 &lt;Style TargetType="TextBlock"&gt;
        ///                     &lt;Setter Property="Margin" Value="7,0,4,4"/&gt;
        ///                     &lt;Setter Property="FontSize" Value="18" /&gt;
        ///                     &lt;Setter Property="FontWeight" Value="Bold" /&gt;
        ///                     &lt;Setter Property="Foreground" Value="RoyalBlue" /&gt;
        ///                     &lt;Setter Property="TextAlignment" Value="Center" /&gt;
        ///                     &lt;Setter Property="VerticalAlignment" Value="Center" /&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerikInput:CalendarCellStyle.ContentStyle&gt;
        ///             &lt;telerikInput:CalendarCellStyle.DecorationStyle&gt;
        ///                 &lt;Style TargetType="Border"&gt;
        ///                     &lt;Setter Property="Background" Value="PaleGreen" /&gt;
        ///                     &lt;Setter Property="BorderBrush" Value="SkyBlue"/&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerikInput:CalendarCellStyle.DecorationStyle&gt;
        ///         &lt;/telerikInput:CalendarCellStyle&gt;
        ///     &lt;/telerikInput:RadCalendar.AnotherViewCellStyle&gt;
        /// &lt;/telerikInput:RadCalendar&gt;
        /// </code>
        /// </example>
        /// <seealso cref="CalendarCellStyle"/>
        /// <seealso cref="CalendarCellStyleSelector"/>
        public CalendarCellStyle AnotherViewCellStyle
        {
            get
            {
                return this.anotherViewCellStyleCache;
            }
            set
            {
                this.SetValue(AnotherViewCellStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="CalendarCellStyle"/> that defines the appearance settings applied to all calendar cells that are disabled.
        /// </summary>
        /// <remarks>
        /// If you want to customize the appearance only of some special calendar cells that fulfill this criteria, you should inherit from 
        /// the <see cref="CalendarCellStyleSelector"/> class and assign the custom instance to the <see cref="CellStyleSelector"/> property.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar&gt;
        ///     &lt;telerikInput:RadCalendar.BlackoutCellStyle &gt;
        ///         &lt;telerikInput:CalendarCellStyle&gt;
        ///             &lt;telerikInput:CalendarCellStyle.ContentStyle&gt;
        ///                 &lt;Style TargetType="TextBlock"&gt;
        ///                     &lt;Setter Property="Margin" Value="7,0,4,4"/&gt;
        ///                     &lt;Setter Property="FontSize" Value="18" /&gt;
        ///                     &lt;Setter Property="FontWeight" Value="Bold" /&gt;
        ///                     &lt;Setter Property="Foreground" Value="Gray" /&gt;
        ///                     &lt;Setter Property="TextAlignment" Value="Center" /&gt;
        ///                     &lt;Setter Property="VerticalAlignment" Value="Center" /&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerikInput:CalendarCellStyle.ContentStyle&gt;
        ///             &lt;telerikInput:CalendarCellStyle.DecorationStyle&gt;
        ///                 &lt;Style TargetType="Border"&gt;
        ///                     &lt;Setter Property="Background" Value="#DDDDDD" /&gt;
        ///                     &lt;Setter Property="BorderBrush" Value="SkyBlue"/&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerikInput:CalendarCellStyle.DecorationStyle&gt;
        ///         &lt;/telerikInput:CalendarCellStyle&gt;
        ///     &lt;/telerikInput:RadCalendar.BlackoutCellStyle&gt;
        /// &lt;/telerikInput:RadCalendar&gt;
        /// </code>
        /// </example>
        /// <seealso cref="CalendarCellStyle"/>
        /// <seealso cref="CalendarCellStyleSelector"/>
        public CalendarCellStyle BlackoutCellStyle
        {
            get
            {
                return this.blackoutCellStyleCache;
            }
            set
            {
                this.SetValue(BlackoutCellStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="CalendarCellStyle"/> that defines the appearance settings applied to all calendar cells that belong
        /// to the current view and are selected.
        /// </summary>
        /// <remarks>
        /// If you want to customize the appearance only of some special calendar cells that fulfill this criteria, you should inherit from 
        /// the <see cref="CalendarCellStyleSelector"/> class and assign the custom instance to the <see cref="CellStyleSelector"/> property.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar&gt;
        ///     &lt;telerikInput:RadCalendar.SelectedCellStyle&gt;
        ///         &lt;telerikInput:CalendarCellStyle&gt;
        ///             &lt;telerikInput:CalendarCellStyle.ContentStyle&gt;
        ///                 &lt;Style TargetType="TextBlock"&gt;
        ///                     &lt;Setter Property="Margin" Value="7,0,4,4"/&gt;
        ///                     &lt;Setter Property="FontSize" Value="18" /&gt;
        ///                     &lt;Setter Property="FontWeight" Value="Bold" /&gt;
        ///                     &lt;Setter Property="Foreground" Value="RoyalBlue" /&gt;
        ///                     &lt;Setter Property="TextAlignment" Value="Center" /&gt;
        ///                     &lt;Setter Property="VerticalAlignment" Value="Center" /&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerikInput:CalendarCellStyle.ContentStyle&gt;
        ///             &lt;telerikInput:CalendarCellStyle.DecorationStyle&gt;
        ///                 &lt;Style TargetType="Border"&gt;
        ///                     &lt;Setter Property="Background" Value="PaleGreen" /&gt;
        ///                     &lt;Setter Property="BorderBrush" Value="SkyBlue"/&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerikInput:CalendarCellStyle.DecorationStyle&gt;
        ///         &lt;/telerikInput:CalendarCellStyle&gt;
        ///     &lt;/telerikInput:RadCalendar.SelectedCellStyle&gt;
        /// &lt;/telerikInput:RadCalendar&gt;
        /// </code>
        /// </example>
        /// <seealso cref="CalendarCellStyle"/>
        /// <seealso cref="CalendarCellStyleSelector"/>
        public CalendarCellStyle SelectedCellStyle
        {
            get
            {
                return this.selectedCellStyleCache;
            }
            set
            {
                this.SetValue(SelectedCellStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="CalendarCellStyle"/> that defines the appearance settings applied to all calendar cells that belong
        /// to the current view and are highlighted.
        /// </summary>
        /// <remarks>
        /// If you want to customize the appearance only of some special calendar cells that fulfill this criteria, you should inherit from 
        /// the <see cref="CalendarCellStyleSelector"/> class and assign the custom instance to the <see cref="CellStyleSelector"/> property.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar&gt;
        ///     &lt;telerikInput:RadCalendar.HighlightedCellStyle&gt;
        ///         &lt;telerikInput:CalendarCellStyle&gt;
        ///             &lt;telerikInput:CalendarCellStyle.ContentStyle&gt;
        ///                 &lt;Style TargetType="TextBlock"&gt;
        ///                     &lt;Setter Property="Margin" Value="7,0,4,4"/&gt;
        ///                     &lt;Setter Property="FontSize" Value="18" /&gt;
        ///                     &lt;Setter Property="FontWeight" Value="Bold" /&gt;
        ///                     &lt;Setter Property="Foreground" Value="RoyalBlue" /&gt;
        ///                     &lt;Setter Property="TextAlignment" Value="Center" /&gt;
        ///                     &lt;Setter Property="VerticalAlignment" Value="Center" /&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerikInput:CalendarCellStyle.ContentStyle&gt;
        ///             &lt;telerikInput:CalendarCellStyle.DecorationStyle&gt;
        ///                 &lt;Style TargetType="Border"&gt;
        ///                     &lt;Setter Property="Background" Value="PaleGreen" /&gt;
        ///                     &lt;Setter Property="BorderBrush" Value="SkyBlue"/&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerikInput:CalendarCellStyle.DecorationStyle&gt;
        ///         &lt;/telerikInput:CalendarCellStyle&gt;
        ///     &lt;/telerikInput:RadCalendar.HighlightedCellStyle&gt;
        /// &lt;/telerikInput:RadCalendar&gt;
        /// </code>
        /// </example>
        /// <seealso cref="CalendarCellStyle"/>
        /// <seealso cref="CalendarCellStyleSelector"/>
        public CalendarCellStyle HighlightedCellStyle
        {
            get
            {
                return this.highlightedCellStyleCache;
            }
            set
            {
                this.SetValue(HighlightedCellStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="CalendarCellStyle"/> that defines the appearance settings applied to the currently focused cell in the calendar view.
        /// </summary>
        /// <remarks>
        /// If you want to customize the appearance only of some special calendar cells that fulfill this criteria, you should inherit from 
        /// the <see cref="CalendarCellStyleSelector"/> class and assign the custom instance to the <see cref="CellStyleSelector"/> property.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar&gt;
        ///     &lt;telerikInput:RadCalendar.CurrentCellStyle&gt;
        ///         &lt;telerikInput:CalendarCellStyle&gt;
        ///             &lt;telerikInput:CalendarCellStyle.ContentStyle&gt;
        ///                 &lt;Style TargetType="TextBlock"&gt;
        ///                     &lt;Setter Property="Margin" Value="7,0,4,4"/&gt;
        ///                     &lt;Setter Property="FontSize" Value="18" /&gt;
        ///                     &lt;Setter Property="FontWeight" Value="Bold" /&gt;
        ///                     &lt;Setter Property="Foreground" Value="RoyalBlue" /&gt;
        ///                     &lt;Setter Property="TextAlignment" Value="Center" /&gt;
        ///                     &lt;Setter Property="VerticalAlignment" Value="Center" /&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerikInput:CalendarCellStyle.ContentStyle&gt;
        ///             &lt;telerikInput:CalendarCellStyle.DecorationStyle&gt;
        ///                 &lt;Style TargetType="Border"&gt;
        ///                     &lt;Setter Property="Background" Value="PaleGreen" /&gt;
        ///                     &lt;Setter Property="BorderBrush" Value="SkyBlue"/&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerikInput:CalendarCellStyle.DecorationStyle&gt;
        ///         &lt;/telerikInput:CalendarCellStyle&gt;
        ///     &lt;/telerikInput:RadCalendar.CurrentCellStyle&gt;
        /// &lt;/telerikInput:RadCalendar&gt;
        /// </code>
        /// </example>
        /// <seealso cref="CalendarCellStyle"/>
        /// <seealso cref="CalendarCellStyleSelector"/>
        public CalendarCellStyle CurrentCellStyle
        {
            get
            {
                return this.currentCellStyleCache;
            }
            set
            {
                this.SetValue(CurrentCellStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="CalendarCellStyle"/> that defines the appearance settings applied to all column header cells that represent the day names when this
        /// calendar instance is in <see cref="CalendarDisplayMode.MonthView"/> mode.
        /// </summary>
        /// <remarks>
        /// You can toggle the visibility of the day names headers through the <see cref="DayNamesVisibility"/> property (by default they are visible).
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar&gt;
        ///     &lt;telerikInput:RadCalendar.DayNameCellStyle&gt;
        ///         &lt;telerikInput:CalendarCellStyle&gt;
        ///             &lt;telerikInput:CalendarCellStyle.ContentStyle&gt;
        ///                 &lt;Style TargetType="TextBlock"&gt;
        ///                     &lt;Setter Property="Margin" Value="7,0,4,4"/&gt;
        ///                     &lt;Setter Property="FontSize" Value="15" /&gt;
        ///                     &lt;Setter Property="Foreground" Value="RoyalBlue" /&gt;
        ///                     &lt;Setter Property="TextAlignment" Value="Center" /&gt;
        ///                     &lt;Setter Property="VerticalAlignment" Value="Center" /&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerikInput:CalendarCellStyle.ContentStyle&gt;
        ///             &lt;telerikInput:CalendarCellStyle.DecorationStyle&gt;
        ///                 &lt;Style TargetType="Border"&gt;
        ///                     &lt;Setter Property="Background" Value="PaleGreen" /&gt;
        ///                     &lt;Setter Property="BorderBrush" Value="SkyBlue"/&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerikInput:CalendarCellStyle.DecorationStyle&gt;
        ///         &lt;/telerikInput:CalendarCellStyle&gt;
        ///     &lt;/telerikInput:RadCalendar.DayNameCellStyle&gt;
        /// &lt;/telerikInput:RadCalendar&gt;
        /// </code>
        /// </example>
        /// <seealso cref="CalendarCellStyle"/>
        /// <seealso cref="DayNamesVisibility"/>
        public CalendarCellStyle DayNameCellStyle
        {
            get
            {
                return this.dayNameCellStyleCache;
            }
            set
            {
                this.SetValue(DayNameCellStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="CalendarCellStyle"/> that defines the appearance settings applied to all row header cells that represent the week numbers when this
        /// calendar instance is in <see cref="CalendarDisplayMode.MonthView"/> mode.
        /// </summary>
        /// <remarks>
        /// You can toggle the visibility of the week number headers through the <see cref="WeekNumbersVisibility"/> property (by default they are collapsed).
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar WeekNumbersVisibility="Visible"&gt;
        ///     &lt;telerikInput:RadCalendar.WeekNumberCellStyle&gt;
        ///         &lt;telerikInput:CalendarCellStyle&gt;
        ///             &lt;telerikInput:CalendarCellStyle.ContentStyle&gt;
        ///                 &lt;Style TargetType="TextBlock"&gt;
        ///                     &lt;Setter Property="Margin" Value="7,0,4,4"/&gt;
        ///                     &lt;Setter Property="FontSize" Value="15" /&gt;
        ///                     &lt;Setter Property="Foreground" Value="RoyalBlue" /&gt;
        ///                     &lt;Setter Property="TextAlignment" Value="Center" /&gt;
        ///                     &lt;Setter Property="VerticalAlignment" Value="Center" /&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerikInput:CalendarCellStyle.ContentStyle&gt;
        ///             &lt;telerikInput:CalendarCellStyle.DecorationStyle&gt;
        ///                 &lt;Style TargetType="Border"&gt;
        ///                     &lt;Setter Property="Background" Value="PaleGreen" /&gt;
        ///                     &lt;Setter Property="BorderBrush" Value="SkyBlue"/&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerikInput:CalendarCellStyle.DecorationStyle&gt;
        ///         &lt;/telerikInput:CalendarCellStyle&gt;
        ///     &lt;/telerikInput:RadCalendar.WeekNumberCellStyle&gt;
        /// &lt;/telerikInput:RadCalendar&gt;
        /// </code>
        /// </example>
        /// <seealso cref="CalendarCellStyle"/>
        /// <seealso cref="WeekNumbersVisibility"/>
        public CalendarCellStyle WeekNumberCellStyle
        {
            get
            {
                return this.weekNumberCellStyleCache;
            }
            set
            {
                this.SetValue(WeekNumberCellStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the logic for customizing the appearance for each calendar cell.
        /// </summary>
        /// <example>
        /// <para>This example demonstrates how to set custom style to the Sundays.</para>
        /// <para>First create a class that inherits from the <see cref="CalendarCellStyleSelector"/> class:</para>
        /// <code language="c#">
        /// public class CustomStyleSelector : CalendarCellStyleSelector
        /// {
        ///     public CalendarCellStyle SundayStyle { get; set; }
        /// 
        ///     protected override void SelectStyleCore(CalendarCellStyleContext context, Telerik.UI.Xaml.Controls.Input.RadCalendar container)
        ///     {
        ///         if (context.Date.DayOfWeek == DayOfWeek.Sunday)
        ///         {
        ///             context.CellStyle = SundayStyle;
        ///             context.ApplyCellTemplateDecorations = false;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para>Then you can create an instance of this class and set its SundayStyle property in XAML:</para>
        /// <code language="xaml">
        /// &lt;local:CustomStyleSelector x:Key="CustomStyleSelector"&gt;
        ///     &lt;local:CustomStyleSelector.SundayStyle&gt;
        ///         &lt;telerik:CalendarCellStyle&gt;
        ///             &lt;telerik:CalendarCellStyle.ContentStyle&gt;
        ///                 &lt;Style TargetType="TextBlock"&gt;
        ///                     &lt;Setter Property="Margin" Value="7,0,4,4"/&gt;
        ///                     &lt;Setter Property="FontSize" Value="18" /&gt;
        ///                     &lt;Setter Property="FontWeight" Value="Bold" /&gt;
        ///                     &lt;Setter Property="Foreground" Value="RoyalBlue" /&gt;
        ///                     &lt;Setter Property="TextAlignment" Value="Center" /&gt;
        ///                     &lt;Setter Property="VerticalAlignment" Value="Center" /&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerik:CalendarCellStyle.ContentStyle&gt;
        ///             &lt;telerik:CalendarCellStyle.DecorationStyle&gt;
        ///                 &lt;Style TargetType="Border"&gt;
        ///                     &lt;Setter Property="Background" Value="PaleGreen" /&gt;
        ///                     &lt;Setter Property="BorderBrush" Value="SkyBlue"/&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerik:CalendarCellStyle.DecorationStyle&gt;
        ///         &lt;/telerik:CalendarCellStyle&gt;
        ///     &lt;/local:CustomStyleSelector.SundayStyle&gt;
        /// &lt;/local:CustomStyleSelector&gt;
        /// </code>
        /// <para>Finally, use the CustomStyleSelector class in the RadCalendar control:</para>
        /// <code language="xaml">
        /// &lt;telerik:RadCalendar CellStyleSelector="{StaticResource CustomStyleSelector}" /&gt;
        /// </code>
        /// </example>
        /// <remarks>
        /// This will only change the appearance of the cells, to change their state, you have to set the <see cref="CellStateSelector"/> property.
        /// </remarks>
        public CalendarCellStyleSelector CellStyleSelector
        {
            get
            {
                return this.cellStyleSelectorCache;
            }
            set
            {
                this.SetValue(CellStyleSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Style the content of the navigation header.
        /// </summary>
        public DataTemplate HeaderContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(HeaderContentTemplateProperty);
            }
            set
            {
                this.SetValue(HeaderContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content of the navigation header.
        /// </summary>
        public object HeaderContent
        {
            get
            {
                return (object)GetValue(HeaderContentProperty);
            }
            set
            {
                SetValue(HeaderContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the custom logic for customizing the state (behavior) for each calendar cell.
        /// </summary>
        /// <example>
        /// <para>Thee following example demonstrates how to set the state of all Sundays to BlackOut cells.</para>
        /// <para>First create a class that inherits from the <see cref="CalendarCellStateSelector"/> class:</para>
        /// <code language="c#">
        /// public class CustomCellStateSelector : CalendarCellStateSelector
        /// {
        ///     protected override void SelectStateCore(CalendarCellStateContext context, RadCalendar container)
        ///     {
        ///         if (context.Date.DayOfWeek == DayOfWeek.Sunday)
        ///         {
        ///             context.IsBlackout = true;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para>Then use this class in the RadCalendar control:</para>
        /// <code language="xaml">
        /// &lt;telerik:RadCalendar&gt;
        ///     &lt;telerik:RadCalendar.CellStateSelector&gt;
        ///         &lt;local:CustomCellStateSelector/&gt;
        ///     &lt;/telerik:RadCalendar.CellStateSelector&gt;
        /// &lt;/telerik:RadCalendar&gt;
        /// </code>
        /// </example>
        /// <seealso cref="CellStyleSelector"/>
        public CalendarCellStateSelector CellStateSelector
        {
            get
            {
                return this.cellStateSelectorCache;
            }
            set
            {
                this.SetValue(CellStateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the custom logic for customizing the state (behavior) for each day name cell.
        /// </summary>
        public CalendarDayNameCellStyleSelector DayNameCellStyleSelector
        {
            get
            {
                return this.dayNameCellStyleSelectorCache;
            }
            set
            {
                this.SetValue(DayNameCellStyleSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the custom logic for customizing the state (behavior) for each week number cell.
        /// </summary>
        public CalendarWeekNumberCellStyleSelector WeekNumberCellStyleSelector
        {
            get
            {
                return this.weekNumberCellStyleSelectorCache;
            }
            set
            {
                this.SetValue(WeekNumberCellStyleSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current date will be highlighted.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar IsTodayHighlighted="True"/&gt;
        /// </code>
        /// </example>
        public bool IsTodayHighlighted
        {
            get
            {
                return this.isTodayHighlightedCache;
            }
            set
            {
                this.SetValue(IsTodayHighlightedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selection mode of the calendar control.
        /// </summary>
        /// <value>
        /// Default value: <see cref="CalendarSelectionMode.Single"/>
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar SelectionMode="Multiple"/&gt;
        /// </code>
        /// </example>
        /// <seealso cref="CalendarSelectionMode"/>
        public CalendarSelectionMode SelectionMode
        {
            get
            {
                return (CalendarSelectionMode)this.GetValue(SelectionModeProperty);
            }
            set
            {
                this.SetValue(SelectionModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the format for the navigation header of the month view of this calendar instance.
        /// </summary>
        /// <value>
        /// The default value is "{0:MMMM yyyy}".
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar MonthViewHeaderFormat="{}{0:M/y}"/&gt;
        /// </code>
        /// </example>
        public string MonthViewHeaderFormat
        {
            get
            {
                return (string)this.GetValue(MonthViewHeaderFormatProperty);
            }
            set
            {
                this.SetValue(MonthViewHeaderFormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the format for the navigation header of the year view of this calendar instance.
        /// </summary>
        /// <value>
        /// The default value is "{0:yyyy}".
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar YearViewHeaderFormat="{}{0:yyyy \year}"/&gt;
        /// </code>
        /// </example>
        public string YearViewHeaderFormat
        {
            get
            {
                return (string)this.GetValue(YearViewHeaderFormatProperty);
            }
            set
            {
                this.SetValue(YearViewHeaderFormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the format for the navigation header of the decade view of this calendar instance.
        /// </summary>
        /// <value>
        /// The default value is "{0:yyyy} ~ {1:yyyy}".
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar DecadeViewHeaderFormat="{}{0:yyyy}/{1:yyyy}"/&gt;
        /// </code>
        /// </example>
        public string DecadeViewHeaderFormat
        {
            get
            {
                return (string)this.GetValue(DecadeViewHeaderFormatProperty);
            }
            set
            {
                this.SetValue(DecadeViewHeaderFormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the format for the navigation header of the century view of this calendar instance.
        /// </summary>
        /// <value>
        /// The default value is "{0:yyyy} ~ {1:yyyy}".
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar CenturyViewHeaderFormat="{}{0:yyyy} -- {1:yyyy}"/&gt;
        /// </code>
        /// </example>
        public string CenturyViewHeaderFormat
        {
            get
            {
                return (string)this.GetValue(CenturyViewHeaderFormatProperty);
            }
            set
            {
                this.SetValue(CenturyViewHeaderFormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the format for the cell content of the month view of this calendar instance.
        /// </summary>
        /// <value>
        /// The default value is "{0:%d}".
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar MonthViewCellFormat="{}{0:m}"/&gt;
        /// </code>
        /// </example>
        public string MonthViewCellFormat
        {
            get
            {
                return (string)this.GetValue(MonthViewCellFormatProperty);
            }
            set
            {
                this.SetValue(MonthViewCellFormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the format for the cell content of the year view of this calendar instance.
        /// </summary>
        /// <value>
        /// The default value is "{0:MMMM}".
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar YearViewCellFormat="{}{0:y}"/&gt;
        /// </code>
        /// </example>
        public string YearViewCellFormat
        {
            get
            {
                return (string)this.GetValue(YearViewCellFormatProperty);
            }
            set
            {
                this.SetValue(YearViewCellFormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the format for the cell content of the decade view of this calendar instance.
        /// </summary>
        /// <value>
        /// The default value is "{0:yyyy}".
        /// </value>
        /// <example>
        /// <code>
        /// &lt;telerikInput:RadCalendar DecadeViewCellFormat="{}'{0:yy}"/&gt;
        /// </code>
        /// </example>
        public string DecadeViewCellFormat
        {
            get
            {
                return (string)this.GetValue(DecadeViewCellFormatProperty);
            }
            set
            {
                this.SetValue(DecadeViewCellFormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the format for the cell content of the century view of this calendar instance.
        /// </summary>
        /// <value>
        /// The default value is "{0:yyyy} ~ {1:yyyy}".
        /// </value>
        /// <example>
        /// <code>
        /// &lt;telerikInput:RadCalendar CenturyViewCellFormat="{}'{0:yy}"/&gt;
        /// </code>
        /// </example>
        public string CenturyViewCellFormat
        {
            get
            {
                return (string)this.GetValue(CenturyViewCellFormatProperty);
            }
            set
            {
                this.SetValue(CenturyViewCellFormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the day name column headers should be visible when this calendar instance is 
        /// in <see cref="CalendarDisplayMode.MonthView"/> mode.
        /// </summary>
        /// <remarks>
        /// The day name column headers are visible by default.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar DayNamesVisibility="Collapsed"/&gt;
        /// </code>
        /// </example>
        public Visibility DayNamesVisibility
        {
            get
            {
                return (Visibility)this.GetValue(DayNamesVisibilityProperty);
            }
            set
            {
                this.SetValue(DayNamesVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the navigation arrows are visible.
        /// </summary>
        /// <remarks>
        /// The navigation arrows are visible by default.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar NavigationArrowsVisibility="Visible"/&gt;
        /// </code>
        /// </example>
        public Visibility NavigationArrowsVisibility
        {
            get
            {
                return (Visibility)this.GetValue(NavigationArrowsVisibilityProperty);
            }
            set
            {
                this.SetValue(NavigationArrowsVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the week number row headers should be visible when this calendar instance is 
        /// in <see cref="CalendarDisplayMode.MonthView"/> mode.
        /// </summary>
        /// <remarks>
        /// The week number column headers are collapsed by default.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar WeekNumbersVisibility="Visible"/&gt;
        /// </code>
        /// </example>
        public Visibility WeekNumbersVisibility
        {
            get
            {
                return (Visibility)this.GetValue(WeekNumbersVisibilityProperty);
            }
            set
            {
                this.SetValue(WeekNumbersVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the days of the week formatting in the calendar column headers in <see cref="CalendarDisplayMode.MonthView"/> mode.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar DayNameFormat="FullName"/&gt;
        /// </code>
        /// </example>
        public CalendarDayNameFormat DayNameFormat
        {
            get
            {
                return (CalendarDayNameFormat)this.GetValue(DayNameFormatProperty);
            }
            set
            {
                this.SetValue(DayNameFormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the week number formatting in the calendar row headers in <see cref="CalendarDisplayMode.MonthView"/> mode.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar WeekNumbersVisibility="Visible" WeekNumberFormat="{}week {0}"/&gt;
        /// </code>
        /// </example>
        public string WeekNumberFormat
        {
            get
            {
                return (string)this.GetValue(WeekNumberFormatProperty);
            }
            set
            {
                this.SetValue(WeekNumberFormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the first <see cref="CalendarDateRange"/> in the current selection or returns null if the selection is empty.
        /// </summary>
        /// <remarks>
        /// Setting <see cref="SelectedDateRange"/> property in a calendar that supports multiple selections clears existing selected ranges 
        /// and sets the selection to the range specified.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar x:Name="calendar"/&gt;
        /// </code>
        /// <code language="c#">
        /// var selectedRange = this.calendar.SelectedDateRange;
        /// </code>
        /// </example>
        public CalendarDateRange? SelectedDateRange
        {
            get
            {
                if (this.SelectedDateRanges.Count > 0)
                {
                    return this.SelectedDateRanges[0];
                }

                return null;
            }
            set
            {
                this.SetValue(SelectedDateRangeProperty, value);
            }
        }

        /// <summary>
        /// Gets a collection of the selected <see cref="CalendarDateRange"/> instances for the calendar control.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar x:Name="calendar"/&gt;
        /// </code>
        /// <code language="c#">
        /// var selectedRange = this.calendar.SelectedDateRanges;
        /// </code>
        /// </example>
        public CalendarDateRangeCollection SelectedDateRanges
        {
            get
            {
                return this.SelectionService.selectedDateRanges;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        double IView.ViewportWidth
        {
            get
            {
                return this.availableCalendarViewSize.Width;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        double IView.ViewportHeight
        {
            get
            {
                return this.availableCalendarViewSize.Height;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        bool IElementPresenter.IsVisible
        {
            get
            {
                return this.Visibility == Visibility.Visible;
            }
        }

        CultureInfo ICultureAware.CurrentCulture
        {
            get
            {
                return this.currentCulture;
            }
        }

        internal CalendarModel Model
        {
            get
            {
                return this.model;
            }
        }

        internal bool HasHorizontalGridLines
        {
            get
            {
                return (this.GridLinesVisibility & GridLinesVisibility.Horizontal) == GridLinesVisibility.Horizontal;
            }
        }

        internal bool HasVerticalGridLines
        {
            get
            {
                return (this.GridLinesVisibility & GridLinesVisibility.Vertical) == GridLinesVisibility.Vertical;
            }
        }

        internal bool ShouldRenderCurrentVisuals
        {
            get
            {
                return this.isCalendarViewFocused && this.FocusState == Windows.UI.Xaml.FocusState.Keyboard;
            }
        }

        internal Size CalendarViewSize
        {
            get
            {
                return this.availableCalendarViewSize;
            }
        }

        /// <summary>
        /// Invalidates the current visual representation of the calendar and schedules a new update that will run asynchronously.
        /// </summary>
        /// <remarks>
        /// Useful if a custom visual logic is applied (Style/State selector for example) based on some external conditions.
        /// </remarks>
        public void InvalidateUI()
        {
            this.Invalidate();
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void IElementPresenter.RefreshNode(object node)
        {
            this.Invalidate();
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        RadSize IElementPresenter.MeasureContent(object owner, object content)
        {
            // we know how to measure only header cells content
            return this.headerContentLayer.MeasureContent(owner, content);
        }

        /// <summary>
        /// A callback, used by the <see cref="CultureService" /> class to notify for a change in the CultureName property.
        /// </summary>
        void ICultureAware.OnCultureChanged(CultureInfo oldValue, CultureInfo newValue)
        {
            CultureInfo newCulture = newValue;
            if (newCulture == null)
            {
                // cannot have null as current culture - fallback to the CurrentCulture.
                newCulture = CultureInfo.CurrentCulture;
            }

            this.currentCulture = newCulture;
            this.model.Culture = this.currentCulture;

            this.UpdateNavigationHeaderContent();
        }

        internal void Invalidate()
        {
            this.lastLayoutContext = CalendarLayoutContext.Invalid;

            if (this.invalidateScheduled || !this.arrangePassed || this.IsUnloaded)
            {
                return;
            }

            if (DesignMode.DesignModeEnabled)
            {
                // TODO: This is hacky, the InvokeAsync method fails, so directly invalidate the Arrange
                this.availableCalendarViewSize = new Size(0, 0);
                this.InvalidateArrange();
            }
            else
            {
                this.invalidateScheduled = this.InvokeAsync(this.OnInvalidated);
            }
        }

        internal void EvaluateCustomCellSelectors(IEnumerable<CalendarCellModel> cellsToUpdate = null)
        {
            IEnumerable<CalendarCellModel> modifiedCells = cellsToUpdate;
            if (modifiedCells == null)
            {
                modifiedCells = this.Model.CalendarCells;
            }

            foreach (CalendarCellModel cell in modifiedCells)
            {
                CalendarCellStateContext stateContext = this.CreateCurrentCellStateContext(cell);
                if (this.CellStateSelector != null)
                {
                    this.CellStateSelector.SelectState(stateContext, this);
                }

                if (stateContext.IsBlackout && this.DisplayMode == CalendarDisplayMode.MonthView)
                {
                    this.SelectionService.selectedDateRanges.SplitRangeByDate(cell);
                }

                CalendarCellStyleContext styleContext = this.CreateCurrentCellStyleContext(cell, stateContext);
                if (this.CellStyleSelector != null)
                {
                    this.CellStyleSelector.SelectStyle(styleContext, this);
                }
            }
        }

        /// <summary>
        /// Performs partial update of the presenters associated with the calendar cells passed as argument.
        /// </summary>
        internal void UpdatePresenters(IEnumerable<CalendarCellModel> cellsToUpdate)
        {
            this.EvaluateCustomCellSelectors(cellsToUpdate);

            this.decorationLayer.UpdateUI(cellsToUpdate);
            this.contentLayer.UpdateUI(cellsToUpdate);
        }

        internal CalendarCellModel GetCellByDate(DateTime dateToSearch)
        {
            // NOTE: Ignore time part for calendar calculations.
            dateToSearch = dateToSearch.Date;

            foreach (CalendarCellModel cell in this.model.CalendarCells)
            {
                if (cell.Date == dateToSearch)
                {
                    return cell;
                }
            }

            return null;
        }

        internal bool CoerceDateWithinDisplayRange(ref DateTime dateToCoerce)
        {
            bool coerced = false;

            // NOTE: Ignore time part for calendar calculations.
            if (dateToCoerce.Date < this.DisplayDateStart.Date)
            {
                dateToCoerce = this.DisplayDateStart;
                coerced = true;
            }
            else if (dateToCoerce.Date > this.DisplayDateEnd.Date)
            {
                dateToCoerce = this.DisplayDateEnd;
                coerced = true;
            }

            return coerced;
        }

        /// <summary>
        /// Exposed for testing purposes.
        /// </summary>
        internal bool IsBlackoutDate(CalendarCellModel cell)
        {
            bool isBlackout = false;

            DateTime displayDateStartDatePart = this.DisplayDateStart.Date;
            DateTime displayDateEndDatePart = this.DisplayDateEnd.Date;

            switch (this.DisplayMode)
            {
                case CalendarDisplayMode.YearView:
                    if (cell.Date < CalendarMathHelper.GetFirstDateOfMonth(displayDateStartDatePart) ||
                        cell.Date > CalendarMathHelper.GetFirstDateOfMonth(displayDateEndDatePart))
                    {
                        isBlackout = true;
                    }
                    break;
                case CalendarDisplayMode.DecadeView:
                    if (cell.Date < CalendarMathHelper.GetFirstDateOfYear(displayDateStartDatePart) ||
                        cell.Date > CalendarMathHelper.GetFirstDateOfYear(displayDateEndDatePart))
                    {
                        isBlackout = true;
                    }
                    break;
                case CalendarDisplayMode.CenturyView:
                    if (cell.Date < CalendarMathHelper.GetFirstDateOfDecade(displayDateStartDatePart) ||
                        cell.Date > CalendarMathHelper.GetFirstDateOfDecade(displayDateEndDatePart))
                    {
                        isBlackout = true;
                    }
                    break;
                default:
                    if (cell.Date < displayDateStartDatePart || cell.Date > displayDateEndDatePart)
                    {
                        isBlackout = true;
                    }
                    break;
            }

            return isBlackout;
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.calendarViewHost = this.GetTemplatePartField<CalendarViewHost>(CalendarViewHostPartName);
            applied = applied && this.calendarViewHost != null;

            this.navigationPanel = this.GetTemplateChild(NavigationControlPanelName) as CalendarNavigationControl;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            if (this.navigationPanel != null)
            {
                this.navigationPanel.Owner = this;
                this.UpdateNavigationHeaderContent();
                this.UpdateNavigationPreviousNextButtonsState();
            }

            if (this.decorationLayer == null)
            {
                this.decorationLayer = new XamlDecorationLayer();
            }

            this.AddLayer(this.decorationLayer, this.calendarViewHost);

            if (this.visualStateLayer == null)
            {
                this.visualStateLayer = new XamlVisualStateLayer();
            }

            this.AddLayer(this.visualStateLayer, this.decorationLayer.VisualContainer);

            if (this.headerContentLayer == null)
            {
                this.headerContentLayer = new XamlHeaderContentLayer();
            }

            this.AddLayer(this.headerContentLayer, this.calendarViewHost);

            if (this.contentLayer == null)
            {
                this.contentLayer = new XamlContentLayer();
                this.inputService.AttachToContentPanel(this.contentLayer.VisualElement);
            }

            this.AddLayer(this.contentLayer, this.calendarViewHost);

            if (this.appointmentLayer == null)
            {
                this.appointmentLayer = new XamlAppointmentLayer();
            }

            this.AddLayer(this.appointmentLayer, this.contentLayer.AnimatableContainer);

            this.calendarViewHost.SizeChanged += this.CalendarViewHostSizeChanged;
            this.calendarViewHost.PointerPressed += this.OnCalendarViewHostPointerPressed;
            this.FetchNewAppointments();
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            if (this.navigationPanel != null)
            {
                this.navigationPanel.Owner = null;
            }

            this.calendarViewHost.SizeChanged -= this.CalendarViewHostSizeChanged;
            this.calendarViewHost.PointerPressed -= this.OnCalendarViewHostPointerPressed;

            this.inputService.DetachFromContentPanel();

            RadCalendar.RemoveLayer(this.contentLayer, this.calendarViewHost);
            RadCalendar.RemoveLayer(this.headerContentLayer, this.calendarViewHost);
            RadCalendar.RemoveLayer(this.decorationLayer, this.calendarViewHost);
            RadCalendar.RemoveLayer(this.visualStateLayer, this.calendarViewHost);
            RadCalendar.RemoveLayer(this.appointmentLayer, this.calendarViewHost);
            base.UnapplyTemplateCore();
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Unloaded" /> event. Allows inheritors to provide their specific logic.
        /// </summary>
        protected override void UnloadCore()
        {
            base.UnloadCore();

            this.availableCalendarViewSize = new Size(0, 0);
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);

            Size oldSize = this.availableCalendarViewSize;
            this.availableCalendarViewSize = this.CalculateAvailableCalendarViewSize(finalSize);

            if (oldSize == this.availableCalendarViewSize)
            {
                return finalSize;
            }

            // NOTE: We need to set the size explicitly so hit-testing works correctly.
            this.contentLayer.VisualContainer.Width = this.availableCalendarViewSize.Width;
            this.contentLayer.VisualContainer.Height = this.availableCalendarViewSize.Height;

            // NOTE: We need to set the size explicitly so animation works correctly.
            this.contentLayer.AnimatableContainer.Width = this.availableCalendarViewSize.Width;
            this.contentLayer.AnimatableContainer.Height = this.availableCalendarViewSize.Height;

            this.CallUpdateUI();

            this.arrangePassed = true;

            return finalSize;
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadCalendarAutomationPeer(this);
        }

        private static void OnDisplayDateStartPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;
            DateTime newDisplayDateStart = (DateTime)args.NewValue;

            if (newDisplayDateStart.Date > calendar.DisplayDateEnd.Date)
            {
                calendar.DisplayDateStart = calendar.DisplayDateEnd;
                return;
            }

            calendar.displayDateStartCache = newDisplayDateStart;
            calendar.UpdateNavigationPreviousNextButtonsState();
            calendar.InvalidatePresenters();

            DateTime displayDate = calendar.DisplayDate;
            if (calendar.CoerceDateWithinDisplayRange(ref displayDate))
            {
                calendar.DisplayDate = displayDate;
            }
        }

        private static void OnDisplayDateEndPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;
            DateTime newDisplayDateEnd = (DateTime)args.NewValue;

            if (newDisplayDateEnd.Date < calendar.DisplayDateStart.Date)
            {
                calendar.DisplayDateEnd = calendar.DisplayDateStart;
                return;
            }

            calendar.displayDateEndCache = newDisplayDateEnd;
            calendar.UpdateNavigationPreviousNextButtonsState();
            calendar.InvalidatePresenters();

            DateTime displayDate = calendar.DisplayDate;
            if (calendar.CoerceDateWithinDisplayRange(ref displayDate))
            {
                calendar.DisplayDate = displayDate;
            }
        }

        private static void OnDisplayDatePropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue.Equals(args.OldValue))
            {
                return;
            }

            RadCalendar calendar = (RadCalendar)target;
            DateTime newDisplayDate = (DateTime)args.NewValue;

            if (calendar.CoerceDateWithinDisplayRange(ref newDisplayDate))
            {
                calendar.DisplayDate = newDisplayDate;
                return;
            }

            calendar.model.DisplayDate = newDisplayDate;

            calendar.UpdateNavigationHeaderContent();
            calendar.UpdateNavigationPreviousNextButtonsState();

            if (calendar.IsTemplateApplied)
            {
                calendar.visualStateLayer.ClearHoverState();
            }

            DateTime oldDisplayDate = (DateTime)args.OldValue;

            if (oldDisplayDate.Year != newDisplayDate.Year || oldDisplayDate.Month != newDisplayDate.Month)
            {
                calendar.FetchNewAppointments();
            }

            if (AutomationPeer.ListenerExists(AutomationEvents.PropertyChanged))
            {
                var peer = FrameworkElementAutomationPeer.FromElement(calendar) as RadCalendarAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseValuePropertyChangedEvent(oldDisplayDate.ToString(), newDisplayDate.ToString());
                }
            }
        }

        private static void OnDisplayModePropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue.Equals(args.NewValue))
            {
                return;
            }

            RadCalendar calendar = (RadCalendar)target;

            calendar.model.DisplayMode = (CalendarDisplayMode)args.NewValue;

            calendar.UpdateNavigationHeaderContent();
            calendar.UpdateNavigationPreviousNextButtonsState();

            if (calendar.IsTemplateApplied)
            {
                calendar.contentLayer.RecycleAllVisuals();
                calendar.visualStateLayer.ClearHoverState();
            }

            RadCalendarAutomationPeer calendarPeer = FrameworkElementAutomationPeer.FromElement(calendar) as RadCalendarAutomationPeer;
            if (calendarPeer != null)
            {
                calendarPeer.ClearCache();
            }
        }

        private static void OnCalendarViewHeaderFormatPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;

            calendar.UpdateNavigationHeaderContent();
        }

        private static void OnGridLinesVisibilityPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)target;

            calendar.model.GridLinesVisibility = (GridLinesVisibility)args.NewValue;
        }

        private static void OnGridLinesThicknessPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)target;

            calendar.model.GridLinesThickness = (double)args.NewValue;
        }

        private static void OnGridLinesBrushPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)target;
            calendar.gridLinesBrushCache = args.NewValue as Brush;

            if (calendar.IsTemplateApplied)
            {
                calendar.decorationLayer.UpdateUI();
            }
        }

        private static void OnSelectionModePropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue.Equals(args.NewValue))
            {
                return;
            }

            RadCalendar calendar = (RadCalendar)target;

            if (calendar.IsTemplateApplied)
            {
                // TODO: Handle the actual selection range update?
                calendar.inputService.CancelDrag();
                calendar.SelectionService.ClearSelection();
            }
        }

        private static void OnAppointmentSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadCalendar calendar = (RadCalendar)sender;
            calendar.AppointmentSource = (AppointmentSource)e.NewValue;

            calendar.FetchNewAppointments();

            if (calendar.appointmentLayer != null)
            {
                calendar.appointmentLayer.UpdateUI();
            }
        }

        private static void OnAppointmentTemplateSelectorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadCalendar calendar = (RadCalendar)sender;
            calendar.AppointmentTemplateSelector = (AppointmentTemplateSelector)e.NewValue;

            if (calendar.appointmentLayer != null)
            {
                calendar.appointmentLayer.UpdateUI();
            }
        }

        private static void OnPointerOverCellStylePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;

            calendar.pointerOverCellStyleCache = args.NewValue as CalendarCellStyle;
        }

        private static void OnNormalCellStylePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;

            calendar.normalCellStyleCache = args.NewValue as CalendarCellStyle;

            calendar.InvalidatePresenters();
        }

        private static void OnAnotherViewCellStylePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;

            calendar.anotherViewCellStyleCache = args.NewValue as CalendarCellStyle;

            calendar.InvalidatePresenters();
        }

        private static void OnBlackoutCellStylePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;

            calendar.blackoutCellStyleCache = args.NewValue as CalendarCellStyle;

            calendar.InvalidatePresenters();
        }

        private static void OnSelectedCellStylePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;

            calendar.selectedCellStyleCache = args.NewValue as CalendarCellStyle;

            calendar.InvalidatePresenters();
        }

        private static void OnHighlightedCellStylePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;

            calendar.highlightedCellStyleCache = args.NewValue as CalendarCellStyle;

            calendar.InvalidatePresenters();
        }

        private static void OnCurrentCellStylePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;

            calendar.currentCellStyleCache = args.NewValue as CalendarCellStyle;

            calendar.InvalidatePresenters();
        }

        private static void OnDayNameCellStylePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;

            calendar.dayNameCellStyleCache = args.NewValue as CalendarCellStyle;

            // NOTE: Updating presenters is not enough as changing this style could potentially trigger need for updating the calendar cell models
            calendar.Invalidate();
        }

        private static void OnWeekNumberCellStylePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;

            calendar.weekNumberCellStyleCache = args.NewValue as CalendarCellStyle;

            // NOTE: Updating presenters is not enough as changing this style could potentially trigger need for updating the calendar cell models
            calendar.Invalidate();
        }

        private static void OnCellStyleSelectorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;

            calendar.cellStyleSelectorCache = args.NewValue as CalendarCellStyleSelector;

            calendar.InvalidatePresenters();
        }

        private static void OnCellStateSelectorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;

            calendar.cellStateSelectorCache = args.NewValue as CalendarCellStateSelector;

            // NOTE: Updating the presenters is not enough as we need to potentially reset user-set flags for the calendar cell models.
            calendar.Invalidate();
        }

        private static void OnHeaderContentTemplatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;
            calendar.UpdateNavigationHeaderContent();
        }

        private static void OnHeaderContentPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;
            calendar.UpdateNavigationHeaderContent();
        }

        private static void OnDayNameCellStyleSelectorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;

            calendar.dayNameCellStyleSelectorCache = args.NewValue as CalendarDayNameCellStyleSelector;

            // NOTE: Updating the presenters is not enough as we need to potentially reset user-set flags for the calendar cell models.
            calendar.Invalidate();
        }

        private static void OnWeekNumberStyleSelectorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;

            calendar.weekNumberCellStyleSelectorCache = args.NewValue as CalendarWeekNumberCellStyleSelector;

            // NOTE: Updating the presenters is not enough as we need to potentially reset user-set flags for the calendar cell models.
            calendar.Invalidate();
        }

        private static void OnIsTodayHighlightedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;

            calendar.isTodayHighlightedCache = (bool)args.NewValue;

            if (calendar.DisplayMode == CalendarDisplayMode.MonthView)
            {
                calendar.InvalidatePresenters();
            }
        }

        private static void OnMonthViewCellFormatPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;
            calendar.model.MonthViewCellFormat = (string)args.NewValue;
        }

        private static void OnYearViewCellFormatPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;
            calendar.model.YearViewCellFormat = (string)args.NewValue;
        }

        private static void OnDecadeViewCellFormatPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;
            calendar.model.DecadeViewCellFormat = (string)args.NewValue;
        }

        private static void OnCenturyViewCellFormatPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;
            calendar.model.CenturyViewCellFormat = (string)args.NewValue;
        }

        private static void OnDayNamesVisibilityPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;
            calendar.model.AreDayNamesVisible = calendar.DayNamesVisibility == Visibility.Visible;
        }

        private static void OnWeekNumbersVisibilityPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;
            calendar.model.AreWeekNumbersVisible = calendar.WeekNumbersVisibility == Visibility.Visible;
        }

        private static void OnDayNameFormatPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;
            calendar.model.DayNameFormat = (CalendarDayNameFormat)args.NewValue;
        }

        private static void OnWeekNumberFormatPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;
            calendar.model.WeekNumberFormat = args.NewValue as string;
        }

        private static DateTime GetFirstDayofMonth(DateTime selectedDate, System.Globalization.Calendar calendar)
        {
            return new DateTime(calendar.GetYear(selectedDate), calendar.GetMonth(selectedDate), 1, 1, 1, 1);
        }

        private static void OnSelectedDateRangePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadCalendar calendar = (RadCalendar)sender;

            CalendarDateRange? newRange = args.NewValue as CalendarDateRange?;
            if (newRange.HasValue)
            {
                calendar.SelectedDateRanges.Clear(false);
                calendar.SelectedDateRanges.Add(newRange.Value);
            }
            else
            {
                calendar.SelectionService.ClearSelection();
            }
        }

        private static void RemoveLayer(CalendarLayer layer, Panel parent)
        {
            layer.DetachUI(parent);
            layer.Owner = null;
        }

        private void FetchNewAppointments()
        {
            if (this.AppointmentSource != null && this.IsTemplateApplied)
            {
                DateTime startDate = GetFirstDayofMonth(this.DisplayDate, this.currentCulture.Calendar);
                this.AppointmentSource.AllAppointments = this.AppointmentSource.FetchData(startDate,
                    startDate.Month == DateTime.MaxValue.Month && startDate.Year == DateTime.MaxValue.Year ? startDate : startDate.AddMonths(1));
            }
        }

        private void AddLayer(CalendarLayer layer, Panel parent)
        {
            layer.Owner = this;
            layer.AttachUI(parent);
        }

        private void OnInvalidated()
        {
            if (!this.IsLoaded || !this.IsTemplateApplied)
            {
                this.invalidateScheduled = false;
                return;
            }

            if (!this.arrangePassed)
            {
                this.InvalidateMeasure();
            }
            else
            {
                this.CallUpdateUI();
            }

            this.invalidateScheduled = false;
        }

        private void CallUpdateUI()
        {
            if (!this.IsTemplateApplied)
            {
                // the template may not be applied if the control is edited in Blend
                return;
            }

            this.UpdateCalendar();

            CalendarLayoutContext context = new CalendarLayoutContext(this.availableCalendarViewSize);
            this.UpdateUI(context);
        }

        private void UpdateCalendar()
        {
            if (!this.model.IsTreeLoaded)
            {
                this.model.LoadElementTree();
            }

            this.model.Arrange();
        }

        private void UpdateUI(CalendarLayoutContext context)
        {
            if (this.lastLayoutContext.AvailableSize == InfinitySize ||
                this.lastLayoutContext.AvailableSize != context.AvailableSize)
            {
                this.UpdatePresenters();
            }

            this.lastLayoutContext = context;
        }

        private void InvalidatePresenters()
        {
            if (this.invalidateScheduled || this.invalidatePresentersScheduled || !this.arrangePassed || this.IsUnloaded)
            {
                return;
            }

            this.invalidatePresentersScheduled = this.InvokeAsync(this.OnPresentersInvalidated);
        }

        private void OnPresentersInvalidated()
        {
            if (this.invalidateScheduled || !this.IsLoaded || !this.IsTemplateApplied)
            {
                this.invalidatePresentersScheduled = false;
                return;
            }

            this.UpdatePresenters();

            this.invalidatePresentersScheduled = false;
        }

        private void UpdatePresenters()
        {
            this.AttachPreselectedDateRanges();

            this.SelectionService.UpdateSelectedCells();

            this.EvaluateHeaderCellStyles();
            this.EvaluateCustomCellSelectors();

            this.decorationLayer.UpdateUI();
            this.headerContentLayer.UpdateUI();
            this.contentLayer.UpdateUI();
            this.appointmentLayer.UpdateUI();
        }

        private void AttachPreselectedDateRanges()
        {
            if (this.unattachedSelectedRanges == null || this.unattachedSelectedRanges.Count == 0)
            {
                return;
            }

            this.SelectedDateRanges.SuspendUpdate();

            foreach (var range in this.unattachedSelectedRanges)
            {
                this.SelectedDateRanges.Add(range);
            }

            this.unattachedSelectedRanges.Clear();

            this.SelectedDateRanges.ResumeUpdate();
        }

        private CalendarCellStateContext CreateCurrentCellStateContext(CalendarCellModel cell)
        {
            if (cell.Date == this.pointerOverDateCache)
            {
                cell.IsPointerOver = true;
            }

            if (this.highlightedCellCache != null && this.highlightedCellCache.Date != DateTime.Today)
            {
                if (cell.IsPointerOver)
                {
                    this.pointerOverDateCache = cell.Date;
                }
                this.Invalidate();
                this.highlightedCellCache = null;
            }

            CalendarCellStateContext context = new CalendarCellStateContext(cell);

            // NOTE: DisplayDateStart / End property change will trigger cell state evaluation but will not invalidate the models
            // so we need to clear the flag explicitly in case it was set for a certain cell and does not need to be set given the current conditions.
            context.IsBlackout = this.IsBlackoutDate(cell);
            
            if (cell.Date == DateTime.Today && this.IsTodayHighlighted && this.DisplayMode == CalendarDisplayMode.MonthView)
            {
                this.highlightedCellCache = cell;
                context.IsHighlighted = true;
            }

            return context;
        }

        private CalendarCellStyleContext CreateCurrentCellStyleContext(CalendarCellModel cell, CalendarCellStateContext stateContext)
        {
            CalendarCellStyleContext context = new CalendarCellStyleContext(stateContext);
            context.CalculatedDecorationCellStyle = this.EvaluateCellDecorationStyle(cell);
            context.CalculatedContentCellStyle = this.EvaluateCellContentStyle(cell);

            cell.Context = context;

            return context;
        }

        private Style EvaluateCellDecorationStyle(CalendarCellModel cell)
        {
            Style style = null;

            // NOTE: CalendarCellModel.IsCurrent / IsPointerOver flags render additional decoration visuals and therefore are not handled here.
            if (cell.IsBlackout)
            {
                if (this.BlackoutCellStyle != null)
                {
                    style = this.BlackoutCellStyle.DecorationStyle;
                }
            }
            else if (cell.IsSelected)
            {
                if (this.SelectedCellStyle != null)
                {
                    style = this.SelectedCellStyle.DecorationStyle;
                }
            }
            else if (cell.IsFromAnotherView)
            {
                if (this.AnotherViewCellStyle != null)
                {
                    style = this.AnotherViewCellStyle.DecorationStyle;
                }
            }
            else if (cell.IsHighlighted)
            {
                if (this.HighlightedCellStyle != null)
                {
                    style = this.HighlightedCellStyle.DecorationStyle;
                }
            }
            else
            {
                if (this.NormalCellStyle != null)
                {
                    style = this.NormalCellStyle.DecorationStyle;
                }
            }

            return style;
        }

        private Style EvaluateCellContentStyle(CalendarCellModel cell)
        {
            Style style = null;

            // NOTE: As we render single content visual, we need to make sure that the style we are applying is meaningful
            // It is completely OK that some visual states (IsPointerOver, IsCurrent) do not specify ContentStyle -- in this case we still need to apply 
            // meaningful state for the next flag in the priority queue.
            if (cell.IsPointerOver && this.PointerOverCellStyle != null && this.PointerOverCellStyle.ContentStyle != null)
            {
                style = this.PointerOverCellStyle.ContentStyle;
            }
            else if (cell.IsCurrent && this.ShouldRenderCurrentVisuals && this.CurrentCellStyle != null && this.CurrentCellStyle.ContentStyle != null)
            {
                style = this.CurrentCellStyle.ContentStyle;
            }
            else if (cell.IsBlackout && this.BlackoutCellStyle != null && this.BlackoutCellStyle.ContentStyle != null)
            {
                style = this.BlackoutCellStyle.ContentStyle;
            }
            else if (cell.IsSelected && this.SelectedCellStyle != null && this.SelectedCellStyle.ContentStyle != null)
            {
                style = this.SelectedCellStyle.ContentStyle;
            }
            else if (cell.IsFromAnotherView && this.AnotherViewCellStyle != null && this.AnotherViewCellStyle.ContentStyle != null)
            {
                style = this.AnotherViewCellStyle.ContentStyle;
            }
            else if (cell.IsHighlighted && this.HighlightedCellStyle != null && this.HighlightedCellStyle.ContentStyle != null)
            {
                style = this.HighlightedCellStyle.ContentStyle;
            }
            else if (this.NormalCellStyle != null && this.NormalCellStyle.ContentStyle != null)
            {
                style = this.NormalCellStyle.ContentStyle;
            }

            return style;
        }

        private void EvaluateHeaderCellStyles()
        {
            if (this.model.CalendarHeaderCells == null)
            {
                return;
            }

            foreach (CalendarHeaderCellModel headerCell in this.model.CalendarHeaderCells)
            {
                if (headerCell.layoutSlot.IsSizeValid())
                {
                    this.CreateHeaderCellStyleContext(headerCell);
                }
            }
        }

        private CalendarCellStyleContext CreateHeaderCellStyleContext(CalendarHeaderCellModel cell)
        {
            CalendarCellStyleContext context = new CalendarCellStyleContext();

            if (cell.Type == CalendarHeaderCellType.DayName)
            {
                if (this.DayNameCellStyleSelector != null)
                {
                    var defaultDayNameCellStyle = this.DayNameCellStyle.ContentStyle;
                    var userDefinedDayNameCellStyle = this.DayNameCellStyleSelector.SelectStyle(cell.Label, this);

                    if (userDefinedDayNameCellStyle == null)
                    {
                        context.CalculatedContentCellStyle = defaultDayNameCellStyle;
                    }
                    else
                    {
                        // Merge user-defined style and default style
                        userDefinedDayNameCellStyle.BasedOn = defaultDayNameCellStyle;
                        context.CalculatedContentCellStyle = userDefinedDayNameCellStyle;
                    }
                }
                else if (this.DayNameCellStyle != null)
                {
                    context.CalculatedDecorationCellStyle = this.DayNameCellStyle.DecorationStyle;
                    context.CalculatedContentCellStyle = this.DayNameCellStyle.ContentStyle;
                }
            }
            else
            {
                if (this.WeekNumberCellStyleSelector != null)
                {
                    var defaultWeekNumberCellStyle = this.WeekNumberCellStyle.ContentStyle;
                    var userDefinedWeekNumberCellStyle = this.WeekNumberCellStyleSelector.SelectStyle(cell.Label, this);

                    if (userDefinedWeekNumberCellStyle == null)
                    {
                        context.CalculatedContentCellStyle = defaultWeekNumberCellStyle;
                    }
                    else
                    {
                        // Merge user-defined style and default style
                        userDefinedWeekNumberCellStyle.BasedOn = defaultWeekNumberCellStyle;
                        context.CalculatedContentCellStyle = userDefinedWeekNumberCellStyle;
                    }
                }
                else if (this.WeekNumberCellStyle != null)
                {
                    context.CalculatedDecorationCellStyle = this.WeekNumberCellStyle.DecorationStyle;
                    context.CalculatedContentCellStyle = this.WeekNumberCellStyle.ContentStyle;
                }
            }

            cell.Context = context;

            return context;
        }

        private Size CalculateAvailableCalendarViewSize(Size availableSize)
        {
            availableSize.Width -= this.BorderThickness.Left + this.BorderThickness.Right + this.Padding.Left + this.Padding.Right;
            availableSize.Height -= this.BorderThickness.Top + this.BorderThickness.Bottom + this.Padding.Top + this.Padding.Bottom;

            if (this.navigationPanel != null)
            {
                availableSize.Height -= this.navigationPanel.ActualHeight;
            }

            return availableSize;
        }

        private void UpdateNavigationHeaderContent()
        {
            if (this.navigationPanel == null)
            {
                return;
            }

            if (this.HeaderContent == null)
            {
                string headerContent = null;

                switch (this.DisplayMode)
                {
                    case CalendarDisplayMode.MonthView:
                        headerContent = string.Format(this.currentCulture, this.MonthViewHeaderFormat, this.DisplayDate);
                        break;
                    case CalendarDisplayMode.YearView:
                        headerContent = string.Format(this.currentCulture, this.YearViewHeaderFormat, this.DisplayDate);
                        break;
                    case CalendarDisplayMode.DecadeView:
                        DateTime decadeStart = CalendarMathHelper.GetFirstDateOfDecade(this.DisplayDate);
                        DateTime decadeEnd = decadeStart.AddYears(9);

                        headerContent = string.Format(this.currentCulture, this.DecadeViewHeaderFormat, decadeStart, decadeEnd);
                        break;
                    case CalendarDisplayMode.CenturyView:
                        DateTime centuryStart = CalendarMathHelper.GetFirstDateOfCentury(this.DisplayDate);
                        DateTime centuryEnd = centuryStart.AddYears(99);

                        headerContent = string.Format(this.currentCulture, this.CenturyViewHeaderFormat, centuryStart, centuryEnd);
                        break;
                }

                this.navigationPanel.HeaderContent = headerContent;
            }
            else
            {
                this.navigationPanel.HeaderContent = this.HeaderContent;
            }
           
            this.navigationPanel.HeaderContentTemplate = this.HeaderContentTemplate;
        }

        private void UpdateNavigationPreviousNextButtonsState()
        {
            if (this.navigationPanel == null)
            {
                return;
            }

            DateTime previousDate = CalendarMathHelper.IncrementByView(this.DisplayDate, -1, this.DisplayMode);
            this.CoerceDateWithinDisplayRange(ref previousDate);
            this.navigationPanel.IsNavigationToPreviousViewEnabled = CalendarMathHelper.IsCalendarViewChanged(this.DisplayDate, previousDate, this.DisplayMode);

            DateTime nextDate = CalendarMathHelper.IncrementByView(this.DisplayDate, 1, this.DisplayMode);
            this.CoerceDateWithinDisplayRange(ref nextDate);
            this.navigationPanel.IsNavigationToNextViewEnabled = CalendarMathHelper.IsCalendarViewChanged(this.DisplayDate, nextDate, this.DisplayMode);
        }

        private void CalendarViewHostSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double navigationPanelHeight = 0d;
            if (this.navigationPanel != null)
            {
                navigationPanelHeight = this.navigationPanel.ActualHeight;
            }

            // NOTE: We need to be able to overlap the outer border of the control with the cell decoration visuals.
            // NOTE: We need to be able to display the hold clue visualization for the first row of cells properly (multiple selection)
            RectangleGeometry clip = new RectangleGeometry();
            clip.Rect = new Rect(
                -this.BorderThickness.Left,
                -(this.BorderThickness.Top + navigationPanelHeight),
                (int)(this.calendarViewHost.ActualWidth + this.BorderThickness.Left + this.BorderThickness.Right + .5),
                (int)(this.calendarViewHost.ActualHeight + navigationPanelHeight + this.BorderThickness.Top + this.BorderThickness.Bottom + .5));

            this.calendarViewHost.Clip = clip;
        }

        private void OnCalendarViewHostPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            this.TryFocus(FocusState.Pointer);

            // NOTE: Not handling this causes the calendar control to lose focus for some reason
            e.Handled = true;
        }
    }
}
