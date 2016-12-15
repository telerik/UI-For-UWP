using Windows.Devices.Input;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// Represents a context, passed to the commands associated with calendar navigation (navigate to previous view level) and calendar cell model.
    /// </summary>
    public class CalendarCellTapContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarCellTapContext"/> class.
        /// </summary>
        /// <param name="cell">The cell model.</param>
        public CalendarCellTapContext(CalendarCellModel cell)
        {
            this.CellModel = cell;
        }

        internal CalendarCellTapContext()
        {
        }

        /// <summary>
        /// Gets the calendar cell view model associated with the current cell tapped.
        /// </summary>
        public CalendarCellModel CellModel { get; internal set; }
    }
}
