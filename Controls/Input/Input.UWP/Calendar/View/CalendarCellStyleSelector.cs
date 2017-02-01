namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// Provides a way to customize the calendar cell appearance (style or template) based on user-defined conditional rules.
    /// </summary>
    public class CalendarCellStyleSelector
    {
        /// <summary>
        /// Evaluates the appearance settings to be applied on the respective calendar cell.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="container">The RadCalendar container.</param>
        public void SelectStyle(CalendarCellStyleContext context, RadCalendar container)
        {
            if (context == null || container == null)
            {
                return;
            }

            this.SelectStyleCore(context, container);
        }

        /// <summary>
        /// When implemented by a derived class, provides a way to tap into the default calendar cell appearance logic through the passed
        /// <see cref="CalendarCellStyleContext" /> argument instance. For example you need to set the <see cref="CalendarCellStyleContext.CellStyle" /> or
        /// <see cref="CalendarCellStyleContext.CellTemplate" /> properties to customize / override the default appearance of the respective calendar cell.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="container">The RadCalendar container.</param>
        protected virtual void SelectStyleCore(CalendarCellStyleContext context, RadCalendar container)
        {
        }
    }
}
