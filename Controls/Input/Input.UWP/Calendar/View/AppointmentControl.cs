using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// Represents the custom <see cref="AppointmentControl"/> implementation used to visualize the UI of the appointments in a cell.
    /// </summary>
    public class AppointmentControl : RadContentControl
    {
        internal RadCalendar calendar;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppointmentControl"/> class.
        /// </summary>
        public AppointmentControl()
        {
            this.DefaultStyleKey = typeof(AppointmentControl);
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            if (this.calendar != null)
            {
                this.calendar.CommandService.ExecuteCommand(Commands.CommandId.AppointmentTap, this.Content);
                e.Handled = true;
            }

            base.OnTapped(e);
        }
    }
}