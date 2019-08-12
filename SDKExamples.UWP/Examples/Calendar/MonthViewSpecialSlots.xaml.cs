using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Input.Calendar;

namespace SDKExamples.UWP.Calendar
{
    public sealed partial class MonthViewSpecialSlots : ExamplePageBase
    {
        public MonthViewSpecialSlots()
        {
            this.InitializeComponent();
            this.DataContext = new MonthViewSpecialSlotsViewModel();
        }
    }

    public class MonthViewSpecialSlotsViewModel : ViewModelBase
    {
        private ObservableCollection<Slot> nonWorkingHours;

        public MonthViewSpecialSlotsViewModel()
        {
            this.GenerateSpecialSlots();

            this.SelectionModes = new ObservableCollection<CalendarSelectionMode>();
            this.SelectionModes.Add(CalendarSelectionMode.None);
            this.SelectionModes.Add(CalendarSelectionMode.Single);
            this.SelectionModes.Add(CalendarSelectionMode.Multiple);

            this.DisplayModes = new ObservableCollection<CalendarDisplayMode>();
            this.DisplayModes.Add(CalendarDisplayMode.MonthView);
            this.DisplayModes.Add(CalendarDisplayMode.MultiDayView);
            this.DisplayModes.Add(CalendarDisplayMode.CenturyView);
            this.DisplayModes.Add(CalendarDisplayMode.DecadeView);
            this.DisplayModes.Add(CalendarDisplayMode.YearView);

            this.AddSlotCommand = new Command(this.OnAddSlotCommandExecuted);
            this.DeleteSlotCommand = new Command(this.OnDeleteSlotCommandExecuted);
            this.UpdateSlotCommand = new Command(this.OnUpdateSlotCommandExecuted);
            this.ClearSlotsCommand = new Command(this.OnClearSlotsCommandExecuted);
            this.SetSlotsToNullCommand = new Command(this.OnSetSlotsToNullCommandExecuted);
            this.ChangeSlotsSourceCommand = new Command(this.OnChangeSlotsSourceCommandExecuted);
        }

        public ObservableCollection<Slot> NonWorkingHours
        {
            get
            {
                return this.nonWorkingHours;
            }
            set
            {
                if (this.nonWorkingHours != value)
                {
                    this.nonWorkingHours = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<CalendarSelectionMode> SelectionModes { get; set; }
        public ObservableCollection<CalendarDisplayMode> DisplayModes { get; set; }
        public ICommand AddSlotCommand { get; set; }
        public ICommand DeleteSlotCommand { get; set; }
        public ICommand UpdateSlotCommand { get; set; }
        public ICommand ClearSlotsCommand { get; set; }
        public ICommand SetSlotsToNullCommand { get; set; }
        public ICommand ChangeSlotsSourceCommand { get; set; }

        private void GenerateSpecialSlots()
        {
            var firstDateOfCurrentWeek = this.GetFirstDayOfCurrentWeek(DateTime.Now.Date, DayOfWeek.Monday);
            DateTime start = firstDateOfCurrentWeek.AddHours(8);
            DateTime end = firstDateOfCurrentWeek.AddHours(18);

            this.NonWorkingHours = new ObservableCollection<Slot>();
            for (int i = 0; i < 5; i++)
            {
                var date = firstDateOfCurrentWeek.AddDays(i);
                this.NonWorkingHours.Add(new Slot(date, start.AddDays(i)) { IsReadOnly = i == 0 });
                this.NonWorkingHours.Add(new Slot(end.AddDays(i), date.AddHours(24).AddSeconds(-1)));
            }

            this.NonWorkingHours.Add(new Slot(start.Date.AddDays(5), start.AddDays(5))
            {
                IsReadOnly = true
            });
            this.NonWorkingHours.Add(new Slot(end.AddDays(5), end.Date.AddDays(5).AddHours(24).AddSeconds(-1)));
        }

        private void OnChangeSlotsSourceCommandExecuted(object obj)
        {
            this.GenerateSpecialSlots();
        }

        private void OnSetSlotsToNullCommandExecuted(object obj)
        {
            this.NonWorkingHours = null;
        }

        private void OnClearSlotsCommandExecuted(object obj)
        {
            this.NonWorkingHours?.Clear();
        }

        private void OnUpdateSlotCommandExecuted(object obj)
        {
            if (this.NonWorkingHours != null && this.NonWorkingHours.Count > 0)
            {
                var lastItem = this.NonWorkingHours.Last();
                lastItem.IsReadOnly = !lastItem.IsReadOnly;
            }
        }

        private void OnDeleteSlotCommandExecuted(object obj)
        {
            if (this.NonWorkingHours != null && this.NonWorkingHours.Count > 0)
            {
                this.NonWorkingHours.RemoveAt(0);
            }
        }

        private void OnAddSlotCommandExecuted(object obj)
        {
            if (this.NonWorkingHours != null)
            {
                if (this.NonWorkingHours.Count > 0)
                {
                    var lastItem = this.NonWorkingHours.Last();
                    var slot = new Slot(lastItem.Start.AddDays(1), lastItem.End.AddDays(1));
                    this.NonWorkingHours.Add(slot);
                }
                else
                {
                    var currentDate = DateTime.Now.Date;
                    var slot = new Slot(currentDate, currentDate.AddHours(10));
                    this.NonWorkingHours.Add(slot);
                }
            }
        }

        private DateTime GetFirstDayOfCurrentWeek(DateTime date, DayOfWeek startDayOfWeek)
        {
            DayOfWeek currentDayOfWeek = date.DayOfWeek;
            int daysToSubtract = currentDayOfWeek - startDayOfWeek;
            if (daysToSubtract <= 0)
            {
                daysToSubtract += 7;
            }

            return date.Date == DateTime.MinValue.Date ? date : date.AddDays(-daysToSubtract);
        }
    }

    public class MonthViewSpecialSlotsCellStateSelector : CalendarCellStateSelector
    {
        protected override void SelectStateCore(CalendarCellStateContext context, RadCalendar container)
        {
            if (context.Date.DayOfWeek == DayOfWeek.Wednesday && context.IsSpecial)
            {
                context.IsSpecial = false;
            }

            base.SelectStateCore(context, container);
        }
    }

    public class MonthViewSpecialSlotCellStyleSelector : CalendarCellStyleSelector
    {
        public CalendarCellStyle GreenStyle { get; set; }

        protected override void SelectStyleCore(CalendarCellStyleContext context, RadCalendar container)
        {
            var monthContext = context as CalendarMonthCellStyleContext;
            if (monthContext != null && monthContext.Date.DayOfWeek == DayOfWeek.Saturday && monthContext.IsSpecialReadOnly)
            {
                monthContext.CellStyle = this.GreenStyle;
            }

            base.SelectStyleCore(context, container);
        }
    }
}
