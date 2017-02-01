using System;
using System.Collections.Generic;
using System.Text;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// Provides a way to customize the calendar appointment control appearance (style or template) based on user-defined conditional rules.
    /// </summary>
    public abstract class AppointmentTemplateSelector
    {
        /// <summary>
        /// Evaluates the appearance settings to be applied on the respective calendar appointment control.
        /// </summary>
        /// <param name="context">The CalendarAppointmentInfo context.</param>
        /// <param name="cell">The CalendarCellModel cell.</param>
        public DataTemplate SelectTemplate(CalendarAppointmentInfo context, CalendarCellModel cell)
        {
            if (cell == null)
            {
                return null;
            }

            return this.SelectTemplateCore(context, cell);
        }

        /// <summary>
        /// When implemented by a derived class, provides a way to tap into the default calendar appointment control appearance logic through the passed
        /// <see cref="CalendarAppointmentInfo" /> argument instance.
        /// </summary>
        /// <param name="context">The CalendarAppointmentInfo context.</param>
        /// <param name="cell">The CalendarCellModel cell.</param>
        protected abstract DataTemplate SelectTemplateCore(CalendarAppointmentInfo context, CalendarCellModel cell);
    }
}
