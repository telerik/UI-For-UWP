using System;
using System.Globalization;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal class CalendarModel : RootElement
    {
        internal const string DefaultMonthViewCellFormatString = "{0:%d}";
        internal const string DefaultYearViewCellFormatString = "{0:MMMM}";
        internal const string DefaultDecadeViewCellFormatString = "{0:yyyy}";
        internal const string DefaultCenturyViewCellFormatString = "{0:yyyy} ~ {1:yyyy}";
        internal const string DefaultWeekNumberFormatString = "{0}";

        internal static readonly int GridLinesVisibilityPropertyKey = PropertyKeys.Register(typeof(GridLinesVisibility), "GridLinesVisibility", CalendarInvalidateFlags.All);
        internal static readonly int GridLinesThicknessPropertyKey = PropertyKeys.Register(typeof(double), "GridLinesThickness", CalendarInvalidateFlags.All);
        internal static readonly int DisplayModePropertyKey = PropertyKeys.Register(typeof(CalendarDisplayMode), "DisplayMode", CalendarInvalidateFlags.All);

        internal static readonly int MonthViewCellFormatPropertyKey = PropertyKeys.Register(typeof(string), "MonthViewCellFormat", CalendarInvalidateFlags.InvalidateContent);
        internal static readonly int YearViewCellFormatPropertyKey = PropertyKeys.Register(typeof(string), "YearViewCellFormat", CalendarInvalidateFlags.InvalidateContent);
        internal static readonly int DecadeViewCellFormatPropertyKey = PropertyKeys.Register(typeof(string), "DecadeViewCellFormat", CalendarInvalidateFlags.InvalidateContent);
        internal static readonly int CenturyViewCellFormatPropertyKey = PropertyKeys.Register(typeof(string), "CenturyViewCellFormat", CalendarInvalidateFlags.InvalidateContent);

        // NOTE: We do not want to invalidate the view models unconditionally on DisplayDate property change, therefore we are not using the invalidate flags logic.
        internal static readonly int DisplayDatePropertyKey = PropertyKeys.Register(typeof(DateTime), "DisplayDate");
        internal static readonly int CulturePropertyKey = PropertyKeys.Register(typeof(CultureInfo), "Culture", CalendarInvalidateFlags.InvalidateContent);
        internal static readonly int DayNameFormatPropertyKey = PropertyKeys.Register(typeof(CalendarDayNameFormat), "DayNameFormat", CalendarInvalidateFlags.InvalidateContent);
        internal static readonly int WeekNumberFormatPropertyKey = PropertyKeys.Register(typeof(string), "WeekNumberFormat", CalendarInvalidateFlags.All);

        internal static readonly int AreDayNamesVisiblePropertyKey = PropertyKeys.Register(typeof(bool), "AreDayNamesVisible", CalendarInvalidateFlags.All);
        internal static readonly int AreWeekNumbersVisiblePropertyKey = PropertyKeys.Register(typeof(bool), "AreWeekNumbersVisible", CalendarInvalidateFlags.All);

        internal CalendarMonthViewModel monthViewModel;
        internal CalendarYearViewModel yearViewModel;
        internal CalendarDecadeViewModel decadeViewModel;
        internal CalendarCenturyViewModel centuryViewModel;

        internal CalendarViewModel currentViewModel;

        public CalendarModel(IView calendar)
        {
            this.DisplayDate = DateTime.Today;

            this.TrackPropertyChanged = true;

            this.View = calendar;

            this.monthViewModel = new CalendarMonthViewModel();
            this.children.Add(this.monthViewModel);

            this.yearViewModel = new CalendarYearViewModel();
            this.children.Add(this.yearViewModel);

            this.decadeViewModel = new CalendarDecadeViewModel();
            this.children.Add(this.decadeViewModel);

            this.centuryViewModel = new CalendarCenturyViewModel();
            this.children.Add(this.centuryViewModel);

            this.UpdateCurrentView();
        }

        public RadRect AnimatableContentClip
        {
            get;
            set;
        }

        public CultureInfo Culture
        {
            get
            {
                return this.GetTypedValue<CultureInfo>(CulturePropertyKey, CultureInfo.CurrentCulture);
            }
            set
            {
                this.SetValue(CulturePropertyKey, value);
            }
        }

        public DateTime DisplayDate
        {
            get
            {
                return this.GetTypedValue<DateTime>(DisplayDatePropertyKey, DateTime.Today);
            }
            set
            {
                this.SetValue(DisplayDatePropertyKey, value);
            }
        }

        public CalendarDayNameFormat DayNameFormat
        {
            get
            {
                return this.GetTypedValue<CalendarDayNameFormat>(DayNameFormatPropertyKey, CalendarDayNameFormat.AbbreviatedName);
            }
            set
            {
                this.SetValue(DayNameFormatPropertyKey, value);
            }
        }

        public string WeekNumberFormat
        {
            get
            {
                return this.GetTypedValue<string>(WeekNumberFormatPropertyKey, DefaultWeekNumberFormatString);
            }
            set
            {
                this.SetValue(WeekNumberFormatPropertyKey, value);
            }
        }

        public CalendarDisplayMode DisplayMode
        {
            get
            {
                return this.GetTypedValue<CalendarDisplayMode>(DisplayModePropertyKey, CalendarDisplayMode.MonthView);
            }
            set
            {
                this.SetValue(DisplayModePropertyKey, value);
            }
        }

        public bool AreDayNamesVisible
        {
            get
            {
                return this.GetTypedValue<bool>(AreDayNamesVisiblePropertyKey, true);
            }
            set
            {
                this.SetValue(AreDayNamesVisiblePropertyKey, value);
            }
        }

        public bool AreWeekNumbersVisible
        {
            get
            {
                return this.GetTypedValue<bool>(AreWeekNumbersVisiblePropertyKey, false);
            }
            set
            {
                this.SetValue(AreWeekNumbersVisiblePropertyKey, value);
            }
        }

        public GridLinesVisibility GridLinesVisibility
        {
            get
            {
                return this.GetTypedValue<GridLinesVisibility>(GridLinesVisibilityPropertyKey, GridLinesVisibility.Both);
            }
            set
            {
                this.SetValue(GridLinesVisibilityPropertyKey, value);
            }
        }

        public double GridLinesThickness
        {
            get
            {
                return this.GetTypedValue<double>(GridLinesThicknessPropertyKey, 2d);
            }
            set
            {
                this.SetValue(GridLinesThicknessPropertyKey, value);
            }
        }

        public string MonthViewCellFormat
        {
            get
            {
                return this.GetTypedValue<string>(MonthViewCellFormatPropertyKey, DefaultMonthViewCellFormatString);
            }
            set
            {
                this.SetValue(MonthViewCellFormatPropertyKey, value);
            }
        }

        public string YearViewCellFormat
        {
            get
            {
                return this.GetTypedValue<string>(YearViewCellFormatPropertyKey, DefaultYearViewCellFormatString);
            }
            set
            {
                this.SetValue(YearViewCellFormatPropertyKey, value);
            }
        }

        public string DecadeViewCellFormat
        {
            get
            {
                return this.GetTypedValue<string>(DecadeViewCellFormatPropertyKey, DefaultDecadeViewCellFormatString);
            }
            set
            {
                this.SetValue(DecadeViewCellFormatPropertyKey, value);
            }
        }

        public string CenturyViewCellFormat
        {
            get
            {
                return this.GetTypedValue<string>(CenturyViewCellFormatPropertyKey, DefaultCenturyViewCellFormatString);
            }
            set
            {
                this.SetValue(CenturyViewCellFormatPropertyKey, value);
            }
        }

        public override IElementPresenter Presenter
        {
            get
            {
                return this.View;
            }
        }

        public ElementCollection<CalendarCellModel> CalendarCells
        {
            get
            {
                return this.currentViewModel.CalendarCells;
            }
        }

        public int ColumnCount
        {
            get
            {
                return this.currentViewModel.ColumnCount;
            }
        }

        public int RowCount
        {
            get
            {
                return this.currentViewModel.RowCount;
            }
        }

        public ElementCollection<CalendarHeaderCellModel> CalendarHeaderCells
        {
            get
            {
                if (this.DisplayMode == CalendarDisplayMode.MonthView)
                {
                    return this.monthViewModel.CalendarHeaderCells;
                }

                return null;
            }
        }

        public ElementCollection<CalendarGridLine> CalendarDecorations
        {
            get
            {
                return this.currentViewModel.CalendarDecorations;
            }
        }

        public void LoadElementTree()
        {
            if (this.IsTreeLoaded)
            {
                return;
            }

            this.Load(this);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Key == DisplayDatePropertyKey)
            {
                DateTime oldDisplayDate = DateTime.MinValue;
                if (e.OldValue != null)
                {
                    oldDisplayDate = (DateTime)e.OldValue;
                }

                DateTime newDisplayDate = DateTime.MinValue;
                if (e.NewValue != null)
                {
                    newDisplayDate = (DateTime)e.NewValue;
                }

                if (CalendarMathHelper.IsCalendarViewChanged(oldDisplayDate, newDisplayDate, this.DisplayMode))
                {
                    this.Invalidate();
                }               
            }
            else if (e.Key == DisplayModePropertyKey)
            {
                this.UpdateCurrentView();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        internal override void PreviewMessage(Message message)
        {
            if (!this.IsTreeLoaded)
            {
                message.StopDispatch = true;
            }
            else
            {
                if (message.Id == Node.PropertyChangedMessage)
                {
                    CalendarInvalidateFlags flags = PropertyKeys.GetPropertyFlags<CalendarInvalidateFlags>((message.Data as RadPropertyEventArgs).Key);
                    this.Invalidate(flags);
                }
            }
        }

        internal void Invalidate(CalendarInvalidateFlags flags)
        {
            if (!this.IsTreeLoaded)
            {
                return;
            }

            if ((flags & CalendarInvalidateFlags.InvalidateContent) == CalendarInvalidateFlags.InvalidateContent)
            {
                this.Invalidate();
            }
        }

        internal override RadRect ArrangeOverride(RadRect rect)
        {
            this.currentViewModel.Arrange(rect);

            return rect;
        }

        internal override ModifyChildrenResult CanAddChild(Node child)
        {
            if (child is CalendarViewModel)
            {
                return ModifyChildrenResult.Accept;
            }

            return base.CanAddChild(child);
        }

        internal DayOfWeek GetFirstDayOfWeek()
        {
            return this.Culture.DateTimeFormat.FirstDayOfWeek;
        }

        internal CalendarWeekRule GetCalendarWeekRule()
        {
            return this.Culture.DateTimeFormat.CalendarWeekRule;
        }

        internal RadSize MeasureContent(object owner, object content)
        {
            return this.View.MeasureContent(owner, content);
        }

        private void UpdateCurrentView()
        {
            switch (this.DisplayMode)
            {
                case CalendarDisplayMode.YearView:
                    this.currentViewModel = this.yearViewModel;
                    break;
                case CalendarDisplayMode.DecadeView:
                    this.currentViewModel = this.decadeViewModel;
                    break;
                case CalendarDisplayMode.CenturyView:
                    this.currentViewModel = this.centuryViewModel;
                    break;
                default:
                    this.currentViewModel = this.monthViewModel;
                    break;
            }
        }
    }
}
