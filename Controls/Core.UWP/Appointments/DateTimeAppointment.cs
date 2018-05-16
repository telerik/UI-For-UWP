using System;
using Windows.UI.Xaml.Media;

namespace Telerik.Core
{
    /// <summary>
    /// A base Appointment class that implements IAppointment.
    /// </summary>
    public class DateTimeAppointment : ViewModelBase, IAppointment
    {
        private DateTime startDate;
        private DateTime endDate;
        private string description;
        private string subject;
        private Brush color;
        private bool isAllDay;

        /// <summary>
        /// Initializes a new instance of the DateTimeAppointment class.
        /// </summary>
        /// <param name="start">The start date of the appointment.</param>
        /// <param name="end">The end date of the appointment.</param>
        public DateTimeAppointment(DateTime start, DateTime end) : this(start, end, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DateTimeAppointment class.
        /// </summary>
        /// <param name="start">The start date of the appointment.</param>
        /// <param name="end">The end date of the appointment.</param>
        /// <param name="description">The appointment description.</param>
        public DateTimeAppointment(DateTime start, DateTime end, string description)
        {
            if (start > end)
            {
                throw new ArgumentException("Start date must be earlier than end date.");
            }

            this.StartDate = start;
            this.EndDate = end;
            this.Description = description;
        }

        /// <summary>
        /// Gets or sets the start date of this appointment.
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                return this.startDate;
            }
            set
            {
                if (this.startDate != value)
                {
                    this.startDate = value;
                    this.OnPropertyChanged(nameof(this.StartDate));
                }
            }
        }

        /// <summary>
        /// Gets or sets the end date of this appointment.
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                return this.endDate;
            }
            set
            {
                if (this.endDate != value)
                {
                    this.endDate = value;
                    this.OnPropertyChanged(nameof(this.EndDate));
                }
            }
        }

        /// <summary>
        /// Gets or sets the description of this appointment.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                if (this.description != value)
                {
                    this.description = value;
                    this.OnPropertyChanged(nameof(this.Description));
                }
            }
        }

        /// <summary>
        /// Gets or sets the description of this appointment.
        /// </summary>
        public string Subject
        {
            get
            {
                return this.subject;
            }
            set
            {
                if (this.subject != value)
                {
                    this.subject = value;
                    this.OnPropertyChanged(nameof(this.Subject));
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color of the appointment.
        /// </summary>
        public Brush Color
        {
            get
            {
                return this.color;
            }
            set
            {
                if (this.color != value)
                {
                    this.color = value;
                    this.OnPropertyChanged(nameof(this.Color));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the appointment is all day.
        /// </summary>
        public bool IsAllDay
        {
            get
            {
                return this.isAllDay;
            }
            set
            {
                if (this.isAllDay != value)
                {
                    this.isAllDay = value;
                    this.OnPropertyChanged(nameof(this.IsAllDay));
                }
            }
        }
    }
}