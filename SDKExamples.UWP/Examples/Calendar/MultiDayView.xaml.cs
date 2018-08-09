using System;
using System.Collections.ObjectModel;
using System.Linq;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Calendar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MultiDayView : ExamplePageBase
    {
        private ViewModel vm;
        private Random rnd;
        public MultiDayView()
        {
            this.InitializeComponent();

            this.vm = new ViewModel();
            this.DataContext = this.vm;

            this.rnd = new Random();
        }

        private void ScrollToAppointmentBtnClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var appointments = this.vm.Appointments.AllAppointments;
            int indexToScrollTo = this.rnd.Next(1, this.vm.Appointments.AllAppointments.Count);

            IAppointment app = this.vm.Appointments.AllAppointments[indexToScrollTo];
            this.calendar.ScrollAppointmentIntoView(app);

            if (this.scrollToTb.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
            {
                this.scrollToTb.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }

            this.scrollToTb.Text = "Scrolled to: " + app.Subject;
        }

        private void ChangeViewBtnClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (this.calendar.DisplayMode == Telerik.UI.Xaml.Controls.Input.CalendarDisplayMode.MultiDayView)
            {
                this.calendar.DisplayMode = Telerik.UI.Xaml.Controls.Input.CalendarDisplayMode.MonthView;
            }
            else
            {
                this.calendar.DisplayMode = Telerik.UI.Xaml.Controls.Input.CalendarDisplayMode.MultiDayView;
            }
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (this.calendar.DayNamesVisibility == Windows.UI.Xaml.Visibility.Collapsed)
            {
                this.calendar.DayNamesVisibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                this.calendar.DayNamesVisibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }
    }

    public class ViewModel : ViewModelBase
    {
        private int selectedVisibleDays;
        private string selectedTickLength;
        private string startTimeValue;
        private string endTimeValue;
        private GridLinesVisibility selectedGridLinesVisibility;
        private int selectedThickness;
        private int selectedTimeRulerLineSpacing;
        private bool showAllDayArea;
        private int selectedAppHeight;
        private int selectedMaxRow;
        private int selectedAllDaySpacing;
        private bool showCurrentTimeIndicator;
        private int selectedNavigationStep;
        private bool weekendsVisible;

        public ViewModel()
        {
            DateTime today = DateTime.Now.Date;
            this.Appointments = new CustomAppointmentSource();
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(-3), today.AddDays(-2).AddHours(4)) { Color = new SolidColorBrush(Colors.Red), Subject = "App 1", Description = "Test", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(-2).AddHours(2), today.AddDays(-2).AddHours(12)) { Color = new SolidColorBrush(Colors.LightBlue), Subject = "App 2", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(2).AddHours(5), today.AddDays(2).AddHours(9)) { Color = new SolidColorBrush(Colors.LightGreen), Subject = "App 3" });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(-1).AddHours(3), today.AddDays(-1).AddHours(7)) { Color = new SolidColorBrush(Colors.Blue), Subject = "App 4", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(3).AddHours(10), today.AddDays(3).AddHours(15)) { Color = new SolidColorBrush(Colors.Orange), Subject = "App 5" });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddHours(14), today.AddHours(18)) { Color = new SolidColorBrush(Colors.DarkOrange), Subject = "App 6", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddHours(14), today.AddDays(2).AddHours(18)) { Color = new SolidColorBrush(Colors.AliceBlue), Subject = "App 7", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(-2).AddHours(14), today.AddDays(-2).AddHours(18)) { Color = new SolidColorBrush(Colors.BurlyWood), Subject = "App 8", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(-2).AddHours(14), today.AddDays(-1).AddHours(18)) { Color = new SolidColorBrush(Colors.Cornsilk), Subject = "App 9", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(-1).AddHours(14), today.AddHours(18)) { Color = new SolidColorBrush(Colors.DarkOliveGreen), Subject = "App 10", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(-15).AddHours(14), today.AddHours(18)) { Color = new SolidColorBrush(Colors.Brown), Subject = "App 11", IsAllDay = true });

            this.DisplayDays = this.InitializeIntCollection();
            this.Thicknesses = this.InitializeIntCollection();
            this.NavigationSteps = this.InitializeIntCollection();

            this.SelectedThickness = this.Thicknesses[1];
            this.SelectedVisibleDays = this.DisplayDays[0];
            this.SelectedNavigationStep = this.NavigationSteps[0];

            this.TickLengths = this.InitializeTimeSpans();
            this.SelectedTickLength = this.TickLengths[0];

            this.StartTimeValues = this.InitializeTimeSpans();
            this.StartTimeValue = this.StartTimeValues[0];

            this.EndTimeValues = this.InitializeTimeSpans();
            this.EndTimeValue = this.EndTimeValues[this.EndTimeValues.Count - 1];

            this.GridLinesVisibility = new ObservableCollection<GridLinesVisibility>(Enum.GetValues(typeof(GridLinesVisibility)).Cast<GridLinesVisibility>().ToList());
            this.SelectedGridLinesVisibility = this.GridLinesVisibility[this.GridLinesVisibility.Count - 1];

            this.TimeRulerItemsSpacings = new ObservableCollection<int>();
            for (int i = 10; i <= 150; i += 10)
            {
                this.TimeRulerItemsSpacings.Add(i);
            }

            this.SelectedTimeRulerLineSpacing = this.TimeRulerItemsSpacings[9];

            this.ShowAllDayArea = true;

            this.AppHeights = new ObservableCollection<int>(new int[] { 10, 20, 30, 40 });
            this.SelectedAppHeight = this.AppHeights[1];

            this.MaxVisibleRows = new ObservableCollection<int>(new int[] { 1, 2, 3, 4, 5});
            this.SelectedMaxRow = this.MaxVisibleRows[1];

            this.AllDayAppSpacing = new ObservableCollection<int>(new int[] { 1, 2, 4, 6, 8, 10 });
            this.SelectedAllDaySpacing = this.AllDayAppSpacing[1];

            this.WeekendsVisible = true;
        }

        public CustomAppointmentSource Appointments { get; set; }
        public ObservableCollection<int> DisplayDays { get; set; }
        public ObservableCollection<string> TickLengths { get; set; }
        public ObservableCollection<string> StartTimeValues { get; set; }
        public ObservableCollection<string> EndTimeValues { get; set; }
        public ObservableCollection<GridLinesVisibility> GridLinesVisibility { get; set; }
        public ObservableCollection<int> Thicknesses { get; set; }
        public ObservableCollection<int> TimeRulerItemsSpacings { get; set; }
        public ObservableCollection<int> AppHeights { get; set; }
        public ObservableCollection<int> MaxVisibleRows { get; set; }
        public ObservableCollection<int> AllDayAppSpacing { get; set; }
        public ObservableCollection<int> NavigationSteps { get; set; }

        public int SelectedVisibleDays
        {
            get
            {
                return this.selectedVisibleDays;
            }
            set
            {
                if (this.selectedVisibleDays != value)
                {
                    this.selectedVisibleDays = value;
                    this.OnPropertyChanged(nameof(this.SelectedVisibleDays));
                }
            }
        }

        public string SelectedTickLength
        {
            get
            {
                return this.selectedTickLength;
            }
            set
            {
                if (this.selectedTickLength != value)
                {
                    this.selectedTickLength = value;
                    this.OnPropertyChanged(nameof(this.SelectedTickLength));
                }
            }
        }

        public string StartTimeValue
        {
            get
            {
                return this.startTimeValue;
            }
            set
            {
                if (this.startTimeValue != value)
                {
                    this.startTimeValue = value;
                    this.OnPropertyChanged(nameof(this.StartTimeValue));
                }
            }
        }

        public string EndTimeValue
        {
            get
            {
                return this.endTimeValue;
            }
            set
            {
                if (this.endTimeValue != value)
                {
                    this.endTimeValue = value;
                    this.OnPropertyChanged(nameof(this.EndTimeValue));
                }
            }
        }

        public GridLinesVisibility SelectedGridLinesVisibility
        {
            get
            {
                return this.selectedGridLinesVisibility;
            }
            set
            {
                if (this.selectedGridLinesVisibility != value)
                {
                    this.selectedGridLinesVisibility = value;
                    this.OnPropertyChanged(nameof(this.SelectedGridLinesVisibility));
                }
            }
        }

        public int SelectedThickness
        {
            get
            {
                return this.selectedThickness;
            }
            set
            {
                if (this.selectedThickness != value)
                {
                    this.selectedThickness = value;
                    this.OnPropertyChanged(nameof(this.SelectedThickness));
                }
            }
        }

        public int SelectedTimeRulerLineSpacing
        {
            get
            {
                return this.selectedTimeRulerLineSpacing;
            }
            set
            {
                if (this.selectedTimeRulerLineSpacing != value)
                {
                    this.selectedTimeRulerLineSpacing = value;
                    this.OnPropertyChanged(nameof(this.SelectedTimeRulerLineSpacing));
                }
            }
        }

        public bool ShowAllDayArea
        {
            get
            {
                return this.showAllDayArea;
            }
            set
            {
                if (this.showAllDayArea != value)
                {
                    this.showAllDayArea = value;
                    this.OnPropertyChanged(nameof(this.ShowAllDayArea));
                }
            }
        }

        public int SelectedAppHeight
        {
            get
            {
                return this.selectedAppHeight;
            }
            set
            {
                if (this.selectedAppHeight != value)
                {
                    this.selectedAppHeight = value;
                    this.OnPropertyChanged(nameof(this.SelectedAppHeight));
                }
            }
        }

        public int SelectedMaxRow
        {
            get
            {
                return this.selectedMaxRow;
            }
            set
            {
                if (this.selectedMaxRow != value)
                {
                    this.selectedMaxRow = value;
                    this.OnPropertyChanged(nameof(this.SelectedMaxRow));
                }
            }
        }

        public int SelectedAllDaySpacing
        {
            get
            {
                return this.selectedAllDaySpacing;
            }
            set
            {
                if (this.selectedAllDaySpacing != value)
                {
                    this.selectedAllDaySpacing = value;
                    this.OnPropertyChanged(nameof(this.SelectedAllDaySpacing));
                }
            }
        }

        public bool ShowCurrentTimeIndicator
        {
            get
            {
                return this.showCurrentTimeIndicator;
            }
            set
            {
                if (this.showCurrentTimeIndicator != value)
                {
                    this.showCurrentTimeIndicator = value;
                    this.OnPropertyChanged(nameof(this.ShowCurrentTimeIndicator));
                }
            }
        }

        public int SelectedNavigationStep
        {
            get
            {
                return this.selectedNavigationStep;
            }
            set
            {
                if (this.selectedNavigationStep != value)
                {
                    this.selectedNavigationStep = value;
                    this.OnPropertyChanged(nameof(this.SelectedNavigationStep));
                }
            }
        }

        public bool WeekendsVisible
        {
            get
            {
                return this.weekendsVisible;
            }
            set
            {
                if (this.weekendsVisible != value)
                {
                    this.weekendsVisible = value;
                    this.OnPropertyChanged(nameof(this.WeekendsVisible));
                }
            }
        }

        private ObservableCollection<string> InitializeTimeSpans()
        {
            ObservableCollection<string> items = new ObservableCollection<string>();

            int stepTicks = 30;
            TimeSpan initialTime = TimeSpan.FromMinutes(stepTicks);
            while (initialTime.Ticks < TimeSpan.TicksPerDay)
            {
                string timeString = initialTime.ToString(@"hh\:mm");
                items.Add(timeString);
                initialTime = initialTime.Add(TimeSpan.FromMinutes(stepTicks));
            }

            return items;
        }

        private ObservableCollection<int> InitializeIntCollection()
        {
            ObservableCollection<int> items = new ObservableCollection<int>();
            for (int i = 1; i <= 7; i++)
            {
                items.Add(i);
            }

            return items;
        }
    }

    public class CustomAppointmentSource : AppointmentSource
    {
        internal ObservableCollection<IAppointment> appointments;

        public CustomAppointmentSource()
        {
            this.appointments = new ObservableCollection<IAppointment>();
        }

        public override ObservableCollection<IAppointment> FetchData(DateTime startDate, DateTime endDate)
        {
            return this.appointments;
        }
    }

    public class StringToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string time = (string)value;
            return TimeSpan.Parse(time);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
