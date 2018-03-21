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
        internal bool isIntersected;
        internal IAppointment childAppointment;
        internal int? arrangeColumnIndex;

        private string detailText;
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
    }
}
