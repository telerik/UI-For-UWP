using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Calendar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomEventInformation : ExamplePageBase
    {
        public CustomEventInformation()
        {
            this.InitializeComponent();
        }
    }

    public class CustomCalendarCellStyleSelector : CalendarCellStyleSelector
    {
        public DataTemplate EventTemplate { get; set; }

        protected override void SelectStyleCore(CalendarCellStyleContext context, Telerik.UI.Xaml.Controls.Input.RadCalendar container)
        {
            var events = (container.DataContext as ViewModelCalendarEvents).Events;

            if (events.Where(e => e.Date == context.Date).Count() > 0)
            {
                context.CellTemplate = this.EventTemplate;
            }
        }
    }

    public class CellModelToEventConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cellModel = value as CalendarCellModel;

            // Get a reference to the calendar container
            var calendar = cellModel.Presenter as RadCalendar;

            // Then you can get a reference to its DataContext (i.e. the page view model that holds the event information)
            var events = (calendar.DataContext as ViewModelCalendarEvents).Events;

            // return custom label for event cells
            var eventInfo = events.Where(e => e.Date == cellModel.Date).FirstOrDefault();
            if (eventInfo != null)
            {
                return eventInfo.Title;
            }

            // return default label for regular cells
            return cellModel.Label;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ViewModelCalendarEvents
    {
        public ViewModelCalendarEvents()
        {
            this.CreateEvents();
        }

        private void CreateEvents()
        {
            List<EventInfo> data = new List<EventInfo>();
            data.Add(new EventInfo() { Date = DateTime.Today.AddDays(2), Title = "Some Event", Details = "Some Details..." });
            data.Add(new EventInfo() { Date = DateTime.Today.AddDays(3), Title = "Other Event", Details = "Other Details..." });
            this.Events = data;
        }

        public List<EventInfo> Events { get; set; }
    }

    public class EventInfo
    {
        public DateTime Date { get; set; }

        public string Title { get; set; }

        public string Details { get; set; }
    }
}
