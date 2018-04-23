using System;
using System.Collections.ObjectModel;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Telerik.UI.Xaml.Controls.Input.Calendar.Commands;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Calendar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MultiDayViewStyling : ExamplePageBase
    {
        private CurrentViewModel vm;
        public MultiDayViewStyling()
        {
            this.InitializeComponent();

            this.vm = new CurrentViewModel();
            this.DataContext = this.vm;

            this.calendar.TimeFormat = "{0:hh:mm:ss tt}";
        }

        private void AddNewAppClicked(object sender, RoutedEventArgs e)
        {
            DateTime today = DateTime.Now.Date;
            DateTimeAppointment app = new DateTimeAppointment(today.AddHours(1), today.AddHours(4)) { Color = new SolidColorBrush(Colors.Pink), Subject = "App 0" };
            this.vm.Appointments.AllAppointments.Add(app);
        }

        private void AddNewSlotClicked(object sender, RoutedEventArgs e)
        {
            this.vm.NonWorkingHours.Add(new Slot(DateTime.Now.Date.AddHours(9), TimeSpan.FromHours(4)));
        }

        private void ChangeAppointmentSourceClicked(object sender, RoutedEventArgs e)
        {
            CustomAppointmentSource newSource = new CustomAppointmentSource();
            DateTime today = DateTime.Now.Date;
            int step = 5;
            Random rnd = new Random();
            Byte[] b = new Byte[3];
            for (int i = 0; i < 100; i++)
            {
                rnd.NextBytes(b);
                DateTimeAppointment app = new DateTimeAppointment(today.AddHours(step + i), today.AddHours(step + step/2 + i))
                {
                    Color = new SolidColorBrush(Color.FromArgb(255, b[0], b[1], b[2])),
                    Subject = "App " + i
                };

                if (i % 10 == 0)
                {
                    step++;
                    app.IsAllDay = true;
                }

                newSource.appointments.Add(app);
            }

            this.calendar.AppointmentSource = newSource;
        }

        private void ChangeSpecialSlotSourceClicked(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Slot> newSlots = new ObservableCollection<Slot>();
            newSlots.Add(new Slot(DateTime.Now.Date.AddHours(12), TimeSpan.FromHours(4)));

            this.vm.NonWorkingHours = newSlots;
        }

        private void ChangeAppPropertyClicked(object sender, RoutedEventArgs e)
        {
            DateTimeAppointment app = (DateTimeAppointment)this.vm.Appointments.AllAppointments[0];
            app.Color = new SolidColorBrush(Colors.Wheat);
            app.Subject = "Changed subject";
            app.StartDate = app.StartDate.AddDays(1);

            app = (DateTimeAppointment)this.vm.Appointments.AllAppointments[2];
            app.Color = new SolidColorBrush(Colors.Brown);
            app.Subject = "Changed subject 2";
            app.StartDate = app.StartDate.AddDays(-1);
        }

        private void ChangeSlotPropertyClicked(object sender, RoutedEventArgs e)
        {
            Slot slot = this.vm.NonWorkingHours[0];
            slot.Start = slot.Start.AddHours(2);
        }
    }

    public class CurrentViewModel : ViewModelBase
    {
        private ObservableCollection<Slot> nonWorkingHours;

        public CurrentViewModel()
        {
            DateTime today = DateTime.Now.Date;
            this.Appointments = new CustomAppointmentSource();
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(-3), today.AddDays(-2).AddHours(4)) { Color = new SolidColorBrush(Colors.Red), Subject = "App 1", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(-2).AddHours(2), today.AddDays(-2).AddHours(12)) { Color = new SolidColorBrush(Colors.LightBlue), Subject = "App 2", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(2).AddHours(5), today.AddDays(2).AddHours(9)) { Color = new SolidColorBrush(Colors.LightGreen), Subject = "App 3", Description = "Some text" });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(-1).AddHours(3), today.AddDays(-1).AddHours(7)) { Color = new SolidColorBrush(Colors.Blue), Subject = "App 4", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(3).AddHours(10), today.AddDays(3).AddHours(15)) { Color = new SolidColorBrush(Colors.Orange), Subject = "App 5" });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddHours(14), today.AddHours(18)) { Color = new SolidColorBrush(Colors.DarkOrange), Subject = "App 6", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddHours(14), today.AddDays(1).AddHours(18)) { Color = new SolidColorBrush(Colors.AliceBlue), Subject = "App 7", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(-2).AddHours(14), today.AddDays(-2).AddHours(18)) { Color = new SolidColorBrush(Colors.BurlyWood), Subject = "App 8", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(-2).AddHours(14), today.AddDays(-1).AddHours(18)) { Color = new SolidColorBrush(Colors.Cornsilk), Subject = "App 9", IsAllDay = true });
            this.Appointments.appointments
                .Add(new DateTimeAppointment(today.AddDays(-1).AddHours(14), today.AddHours(18)) { Color = new SolidColorBrush(Colors.DarkOliveGreen), Subject = "App 10", IsAllDay = true });

            DateTime start = today.AddHours(8);
            DateTime end = today.AddHours(18);

            this.NonWorkingHours = new ObservableCollection<Slot>();

            for (int i = 0; i < 7; i++)
            {
                this.NonWorkingHours.Add(new Slot(today.AddDays(i), start.AddDays(i)));
                this.NonWorkingHours.Add(new Slot(end.AddDays(i), today.AddDays(i).AddHours(24)));
            }
        }

        public CustomAppointmentSource Appointments { get; set; }
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
                    this.OnPropertyChanged(nameof(this.NonWorkingHours));
                }
            }
        }
    }

    public class NonWorkingHoursSpecialSlotStyleSelector : StyleSelector
    {
        public Style NonWorkingHours { get; set; }
        public Style SpecialHours { get; set; }

        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            Slot slot = (Slot)item;
            if (slot.Start.Hour < 8 || slot.Start.Hour >= 18)
            {
                return this.NonWorkingHours;
            }

            return this.SpecialHours;
        }
    }

    public class CustomAppointmentTemplateSelector : AppointmentTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }

        public DataTemplate SpecialTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(CalendarAppointmentInfo context, CalendarCellModel cell)
        {
            if (string.IsNullOrEmpty(context.DetailText))
            {
                return null;
            }

            if (context.Date.HasValue && context.Date.Value.Date == DateTime.Now.Date)
            {
                return this.DefaultTemplate;
            }

            return this.SpecialTemplate;
        }
    }

    public class CustomAppointmentHeaderTemplateSelector : AppointmentTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }

        public DataTemplate SpecialTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(CalendarAppointmentInfo context, CalendarCellModel cell)
        {
            if (string.IsNullOrEmpty(context.Subject))
            {
                return null;
            }

            if (context.Date.HasValue && context.Date.Value.Date == DateTime.Now.Date)
            {
                return this.DefaultTemplate;
            }

            return this.SpecialTemplate;
        }
    }

    public class SlotTapCommand : CalendarCommand
    {
        public SlotTapCommand()
        {
            this.Id = CommandId.TimeSlotTap;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public async override void Execute(object parameter)
        {
            base.Execute(parameter);

            Slot slot = (Slot)parameter;
            ContentDialog dialog = new ContentDialog
            {
                Title = "SlotTapped",
                Content = "Slot Start: " + slot.Start + " End: " + slot.End + " was tapped.",
                PrimaryButtonText = "OK"
            };

            await dialog.ShowAsync();
        }
    }

    public class AppointmentTapCommand : CalendarCommand
    {
        public AppointmentTapCommand()
        {
            this.Id = CommandId.AppointmentTap;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public async override void Execute(object parameter)
        {
            base.Execute(parameter);

            IAppointment appInfo = (IAppointment)parameter;
            ContentDialog dialog = new ContentDialog
            {
                Title = "AppointmentTap",
                Content = "Appointment " + appInfo.Subject + " was tapped.",
                PrimaryButtonText = "OK"
            };

            await dialog.ShowAsync();
        }
    }

    public class CustomAppointmentStyleSelector : StyleSelector
    {
        public Style DefaultStyle { get; set; }

        public Style AllDayStyle { get; set; }

        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            CalendarAppointmentInfo info = (CalendarAppointmentInfo)item;
            if (info.IsAllDay)
            {
                return this.AllDayStyle;
            }

            return this.DefaultStyle;
        }
    }
}
