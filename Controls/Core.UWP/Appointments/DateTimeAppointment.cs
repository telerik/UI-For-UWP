using System;

namespace Telerik.Core
{
    /// <summary>
    /// A base Appointment class that implements IAppointment.
    /// </summary>
    public class DateTimeAppointment : IAppointment
    {
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
            if (start >= end)
            {
                throw new ArgumentException("Start date must be earlier than end date.");
            }

            this.StartDate = start;
            this.EndDate = end;
            this.Description = description;
        }

        /// <summary>
        /// Gets the start date of this appointment.
        /// </summary>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// Gets the end date of this appointment.
        /// </summary>
        public DateTime EndDate { get; private set; }

        /// <summary>
        /// Gets the description of this appointment.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets or sets the subject of this appointment.
        /// </summary>
        public string Subject { get; set; }
    }
}