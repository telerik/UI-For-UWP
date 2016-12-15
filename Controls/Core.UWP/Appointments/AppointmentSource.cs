using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Telerik.Core
{
    /// <summary>
    /// Defines basic logic for retrieving a collection of IAppointment objects.
    /// </summary>
    public abstract class AppointmentSource
    {
        private ObservableCollection<IAppointment> allAppointments = new ObservableCollection<IAppointment>();

        /// <summary>
        /// Gets a list of all the appointments from the real data source wrapped in IAppointment objects.
        /// </summary>
        public ObservableCollection<IAppointment> AllAppointments
        {
            get
            {
                return this.allAppointments;
            }

            internal set
            {
                this.allAppointments = value;
            }
        }

        /// <summary>
        /// Creates a collection of IAppointment objects based on an appointment predicate.
        /// </summary>
        /// <param name="appointmentFilter">A predicate that determines which appointments will be present in the resulting collection.</param>
        /// <returns>Returns a collection of IAppointment objects.</returns>
        /// <remarks>
        /// Inheritors must populate the AllAppointments list, otherwise this method will always return an empty collection.
        /// </remarks>
        public LinkedList<IAppointment> GetAppointments(Func<IAppointment, bool> appointmentFilter)
        {
            LinkedList<IAppointment> result = new LinkedList<IAppointment>();

            if (appointmentFilter != null)
            {
                foreach (IAppointment current in this.allAppointments)
                {
                    if (appointmentFilter(current))
                    {
                        result.AddLast(current);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// An abstract method that inheritors must implement in order to
        /// fetch new appointments from the native appointment source.
        /// </summary>
        /// <param name="startDate">The start date for the new appointments.</param>
        /// <param name="endDate">The end date for the new appointments.</param>
        /// <returns>Appointments data.</returns>
        public abstract ObservableCollection<IAppointment> FetchData(DateTime startDate, DateTime endDate);
    }
}