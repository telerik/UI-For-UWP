using System;

namespace Telerik.Core
{
    /// <summary>
    /// An interface that defines the minimum properties that an appointment can have.
    /// </summary>
    public interface IAppointment
    {
        /// <summary>
        /// Gets the start date of the appointment.
        /// </summary>
        DateTime StartDate
        {
            get;
        }

        /// <summary>
        /// Gets the end date of the appointment.
        /// </summary>
        DateTime EndDate
        {
            get;
        }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        string Subject
        {
            get;
            set;
        }
    }
}
