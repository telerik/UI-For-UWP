using System;
using System.Collections.Generic;
using Telerik.Core;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// Represents a context which holds the appointments for each calendar cell.
    /// </summary>
    public class CalendarAppointmentInfo : ViewModelBase
    {
        internal RadRect layoutSlot;
        internal int columnIndex;
        internal CalendarCellModel cell;
        internal bool isArranged;
        internal IAppointment childAppointment;
        internal int? arrangeColumnIndex;
        internal bool hasPrevDay;
        internal bool hasNextDay;

        private string detailText;
        private string subject;
        private bool isAllDay;
        private DateTime? date;
        private LinkedList<IAppointment> appointments;
        private Brush brush;

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
        /// Gets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public string Subject
        {
            get
            {
                return this.subject;
            }
            internal set
            {
                this.subject = value;
                this.OnPropertyChanged(nameof(this.Subject));
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

        /// <summary>
        /// Gets the color of the appointment.
        /// </summary>
        public Brush Brush
        {
            get
            {
                return this.brush;
            }
            internal set
            {
                this.brush = value;
                this.OnPropertyChanged(nameof(this.Brush));
            }
        }

        /// <summary>
        /// Gets a value indicating whether the appointment is all day.
        /// </summary>
        public bool IsAllDay
        {
            get
            {
                return this.isAllDay;
            }
            internal set
            {
                this.isAllDay = value;
                this.OnPropertyChanged(nameof(this.IsAllDay));
            }
        }
    }
}
