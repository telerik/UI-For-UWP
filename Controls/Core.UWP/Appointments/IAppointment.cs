using System;
using Windows.UI.Xaml.Media;

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

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        /// <value>The subject.</value>
        string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color of the appointment.
        /// </summary>
        Brush Color
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the appointment is all day.
        /// </summary>
        bool IsAllDay
        {
            get;
            set;
        }
    }
}
