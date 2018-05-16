using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Input.Calendar.Commands;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace SDKExamples.UWP.Calendar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MultiDayViewAppointmentsCRUD : ExamplePageBase
    {
        private CultureInfo culture;
        private MultiDayViewAppointmentsCRUDViewModel vm;
        public MultiDayViewAppointmentsCRUD()
        {
            this.InitializeComponent();

            this.vm = new MultiDayViewAppointmentsCRUDViewModel();
            this.DataContext = this.vm;

            this.culture = new CultureInfo("en-US");
            this.culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;

            DateTime firstDateOfCurrentWeek = this.GetFirstDayOfCurrentWeek(this.calendar.DisplayDate, this.culture.DateTimeFormat.FirstDayOfWeek);
            this.calendar.DisplayDate = firstDateOfCurrentWeek;
        }

        internal DateTime GetFirstDayOfCurrentWeek(DateTime date, DayOfWeek startDayOfWeek)
        {
            DayOfWeek currentDayOfWeek = date.DayOfWeek;
            int daysToSubtract = currentDayOfWeek - startDayOfWeek;
            if (daysToSubtract <= 0)
            {
                daysToSubtract += 7;
            }

            return date.Date == DateTime.MinValue.Date ? date : date.AddDays(-daysToSubtract);
        }

        private void TodayTextBlockTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            DateTime firstDateOfCurrentWeek = this.GetFirstDayOfCurrentWeek(DateTime.Now.Date, this.culture.DateTimeFormat.FirstDayOfWeek);
            this.calendar.DisplayDate = firstDateOfCurrentWeek;
        }

        private void ShowNewAppointmentTextBlockTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.vm.AppointmentUIContent = MultiDayViewAppointmentsCRUDViewModel.DefaultAddText;
            this.vm.IsOpen = true;
        }

        private void OnAppointmentUIPopupOpened(object sender, object e)
        {
            FrameworkElement popupChild = this.AppointmentUIPopup.Child as FrameworkElement;
            if (popupChild != null)
            {
                double horizontalOffset = this.calendar.ActualWidth / 2 - popupChild.ActualWidth / 2;
                double verticalOffset = this.calendar.ActualHeight / 2 - popupChild.ActualHeight / 2;

                this.AppointmentUIPopup.HorizontalOffset = horizontalOffset;
                this.AppointmentUIPopup.VerticalOffset = verticalOffset;
            }
        }
    }

    public class MultiDayViewAppointmentsCRUDViewModel : ViewModelBase
    {
        internal static string DefaultAddText = "Add Appointment";
        private static string DefaultEditText = "Edit Appointment";

        private string subject;
        private string description;
        private bool isAllday;
        private DateTime? startDate;
        private DateTime? endDate;
        private bool isOpen;
        private string appointmentUIContent;

        private DateTimeAppointment currentTappedAppointment;
        private bool shouldDeleteTappedAppointment;
        private Random rnd;
        public MultiDayViewAppointmentsCRUDViewModel()
        {
            this.AppointmentBrushes = new ObservableCollection<SolidColorBrush>();
            this.AppointmentBrushes.Add(new SolidColorBrush(Color.FromArgb(0xFF, 0x31, 0x48, 0xCA)));
            this.AppointmentBrushes.Add(new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x52, 0x25)));
            this.AppointmentBrushes.Add(new SolidColorBrush(Color.FromArgb(0xFF, 0x30, 0xBC, 0xFF)));

            this.rnd = new Random();
            DateTime today = DateTime.Now.Date;
            this.Appointments = new CustomAppointmentSource();
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(-3), today.AddDays(-2).AddHours(4)) { Color = this.AppointmentBrushes[rnd.Next(0, 2)], Subject = "App 1", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(-2).AddHours(2), today.AddDays(-2).AddHours(12)) { Color = this.AppointmentBrushes[rnd.Next(0, 2)], Subject = "App 2", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(2).AddHours(5), today.AddDays(2).AddHours(9)) { Color = this.AppointmentBrushes[rnd.Next(0, 2)], Subject = "App 3", Description = "Meeting" });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(-1).AddHours(3), today.AddDays(-1).AddHours(7)) { Color = this.AppointmentBrushes[rnd.Next(0, 2)], Subject = "App 4", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(3).AddHours(10), today.AddDays(3).AddHours(15)) { Color = this.AppointmentBrushes[rnd.Next(0, 2)], Subject = "App 5" });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddHours(14), today.AddHours(18)) { Color = this.AppointmentBrushes[rnd.Next(0, 2)], Subject = "App 6", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddHours(14), today.AddDays(1).AddHours(18)) { Color = this.AppointmentBrushes[rnd.Next(0, 2)], Subject = "App 7", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(-2).AddHours(14), today.AddDays(-2).AddHours(18)) { Color = this.AppointmentBrushes[rnd.Next(0, 2)], Subject = "App 8", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(-2).AddHours(14), today.AddDays(-1).AddHours(18)) { Color = this.AppointmentBrushes[rnd.Next(0, 2)], Subject = "App 9", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(-1).AddHours(14), today.AddHours(18)) { Color = this.AppointmentBrushes[rnd.Next(0, 2)], Subject = "App 10", IsAllDay = true });

            this.AddNewAppointmentCommand = new Command(this.OnAddNewApoitmentCommandExecuted);
            this.AppointmentTapCommand = new Command(this.OnAppointmentTapCommandExecuted);
            this.DeleteAppointmentCommand = new Command(this.OnDeleteAppointmentCommandExecuted);

            this.StartDate = today;
            this.EndDate = today;
        }

        public ICommand AddNewAppointmentCommand { get; set; }
        public ICommand AppointmentTapCommand { get; set; }
        public ICommand DeleteAppointmentCommand { get; set; }

        public ObservableCollection<SolidColorBrush> AppointmentBrushes { get; set; }
        public CustomAppointmentSource Appointments { get; set; }

        public string Subject
        {
            get
            {
                return this.subject;
            }
            set
            {
                if (this.subject != value)
                {
                    this.subject = value;
                    this.OnPropertyChanged(nameof(this.Subject));

                }
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                if (this.description != value)
                {
                    this.description = value;
                    this.OnPropertyChanged(nameof(this.Description));
                }
            }
        }

        public bool IsAllDay
        {
            get
            {
                return this.isAllday;
            }
            set
            {
                if (this.isAllday != value)
                {
                    this.isAllday = value;
                    this.OnPropertyChanged(nameof(this.IsAllDay));
                }
            }
        }

        public DateTime? StartDate
        {
            get
            {
                return this.startDate;
            }
            set
            {
                if (this.startDate != value)
                {
                    this.startDate = value;
                    this.OnPropertyChanged(nameof(this.StartDate));
                }
            }
        }

        public DateTime? EndDate
        {
            get
            {
                return this.endDate;
            }
            set
            {
                if (this.endDate != value)
                {
                    this.endDate = value;
                    this.OnPropertyChanged(nameof(this.EndDate));
                }
            }
        }

        public bool IsOpen
        {
            get
            {
                return this.isOpen;
            }
            set
            {
                if (this.isOpen != value)
                {
                    this.isOpen = value;
                    this.OnPropertyChanged(nameof(this.IsOpen));
                }
            }
        }

        public string AppointmentUIContent
        {
            get
            {
                return this.appointmentUIContent;
            }
            set
            {
                if (this.appointmentUIContent != value)
                {
                    this.appointmentUIContent = value;
                    this.OnPropertyChanged(nameof(this.AppointmentUIContent));
                }
            }
        }

        private void OnAddNewApoitmentCommandExecuted(object obj)
        {
            if (this.AppointmentUIContent == MultiDayViewAppointmentsCRUDViewModel.DefaultEditText)
            {
                this.currentTappedAppointment.Subject = this.Subject;
                this.currentTappedAppointment.Description = this.Description;
                this.currentTappedAppointment.IsAllDay = this.IsAllDay;
                this.currentTappedAppointment.StartDate = this.StartDate.Value;
                this.currentTappedAppointment.EndDate = this.EndDate.Value;
            }
            else
            {
                DateTimeAppointment newAppointment = new DateTimeAppointment(this.StartDate.Value, this.EndDate.Value);
                newAppointment.Subject = this.Subject;
                newAppointment.Description = this.Description;
                newAppointment.IsAllDay = this.IsAllDay;
                newAppointment.Color = this.AppointmentBrushes[rnd.Next(0, 2)];

                this.Appointments.AllAppointments.Add(newAppointment);
            }

            this.Subject = string.Empty;
            this.Description = string.Empty;
            this.IsAllDay = false;

            DateTime today = DateTime.Now.Date;
            this.StartDate = today;
            this.EndDate = today;

            this.IsOpen = false;
        }

        private void OnAppointmentTapCommandExecuted(object obj)
        {
            this.currentTappedAppointment = (DateTimeAppointment)obj;
            if (this.shouldDeleteTappedAppointment)
            {
                this.Appointments.AllAppointments.Remove(this.currentTappedAppointment);
                this.shouldDeleteTappedAppointment = false;
            }
            else
            {
                this.Subject = this.currentTappedAppointment.Subject;
                this.Description = this.currentTappedAppointment.Description;
                this.IsAllDay = this.currentTappedAppointment.IsAllDay;
                this.StartDate = this.currentTappedAppointment.StartDate;
                this.EndDate = this.currentTappedAppointment.EndDate;

                this.AppointmentUIContent = MultiDayViewAppointmentsCRUDViewModel.DefaultEditText;
                this.IsOpen = true;
            }
        }

        private void OnDeleteAppointmentCommandExecuted(object obj)
        {
            this.shouldDeleteTappedAppointment = true;
        }
    }

    public class CustomAppointmentTapCommand : CalendarCommand
    {
        public CustomAppointmentTapCommand()
        {
            this.Id = CommandId.AppointmentTap;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            RadCalendar calendar = this.Owner;
            MultiDayViewAppointmentsCRUDViewModel vm = (MultiDayViewAppointmentsCRUDViewModel)this.Owner.DataContext;
            vm.AppointmentTapCommand.Execute(parameter);
        }
    }

    public class Command : ICommand
    {
        private readonly Predicate<object> canExecute;
        private readonly Action<object> execute;

        public Command(Action<object> execute)
            : this(execute, null)
        {
        }

        public Command(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (this.canExecute == null)
            {
                return true;
            }

            return this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
