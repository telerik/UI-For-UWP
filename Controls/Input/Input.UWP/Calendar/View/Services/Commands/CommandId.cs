namespace Telerik.UI.Xaml.Controls.Input.Calendar.Commands
{
    /// <summary>
    /// Defines the known commands that are available within a <see cref="RadCalendar"/> control.
    /// </summary>
    public enum CommandId
    {
        /// <summary>
        /// The command is not familiar to the calendar. 
        /// </summary>
        Unknown,

        /// <summary>
        /// A command associated with the action of moving the calendar to a specific date (on the same level).
        /// </summary>
        MoveToDate,

        /// <summary>
        /// A command associated with the action of moving the calendar to previous calendar view (on the same level).
        /// </summary>
        MoveToPreviousView,

        /// <summary>
        /// A command associated with the action of moving the calendar to next calendar view (on the same level).
        /// </summary>
        MoveToNextView,

        /// <summary>
        /// A command associated with the action of moving the calendar to upper calendar view level (e.g. from Month to Year view).
        /// </summary>
        MoveToUpperView,

        /// <summary>
        /// A command associated with the action of moving the calendar to lower calendar view level (e.g. from Year to Month view).
        /// </summary>
        MoveToLowerView,

        /// <summary>
        /// A command associated with the PointerOver event that occurs over a calendar cell.
        /// </summary>
        CellPointerOver,

        /// <summary>
        /// A command associated with the Tap event that occurs on tapping a calendar cell.
        /// </summary>
        CellTap,

        /// <summary>
        /// A command associated with the Tap event that occurs on tapping an appointment.
        /// </summary>
        AppointmentTap,

        /// <summary>
        /// A command associated with the Tap event that occurs on tapping a time slot.
        /// </summary>
        TimeSlotTap
    }
}
