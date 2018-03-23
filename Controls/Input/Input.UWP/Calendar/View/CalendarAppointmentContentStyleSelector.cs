namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    public class CalendarAppointmentContentStyleSelector
    {
        /// <summary>
        /// Evaluates the appearance settings to be applied on the respective appointment content.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="container">The RadCalendar container.</param>
        public virtual CalendarAppointmentContentStyle SelectStyle(CalendarAppointmentInfo context, AppointmentControl container)
        {
            return null;
        }
    }
}
