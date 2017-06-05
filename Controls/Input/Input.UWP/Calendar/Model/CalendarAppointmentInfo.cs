using System;
using System.Collections.Generic;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// Represents a context which holds the appointments for each calendar cell.
    /// </summary>
    public class CalendarAppointmentInfo : ViewModelBase
    {
        private string detailText;
        private DateTime? date;
        private LinkedList<IAppointment> appointments;

        /// <summary>
        /// Gets the appointments.
        /// </summary>
        /// <value>The appointment.</value>
        public LinkedList<IAppointment> Appointments
        {
            get
            {
                return this.appointments;
            }
            internal set
            {
                this.appointments = value;
                this.OnPropertyChanged(nameof(this.Appointments));
            }
        }

        /// <summary>
        /// Gets the detail text.
        /// </summary>
        /// <value>The detail text.</value>
        public string DetailText
        {
            get
            {
                return this.detailText;
            }
            internal set
            {
                this.detailText = value;
                this.OnPropertyChanged(nameof(this.DetailText));
            }
        }

        /// <summary>
        /// Gets the date associated with the calendar cell.
        /// </summary>
        public DateTime? Date
        {
            get
            {
                return this.date;
            }
            internal set
            {
                this.date = value;
                this.OnPropertyChanged(nameof(this.Date));
            }
        }
    }
}
