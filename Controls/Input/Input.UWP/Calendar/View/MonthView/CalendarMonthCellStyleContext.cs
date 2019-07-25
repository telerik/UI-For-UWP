using System.Collections.Generic;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// Represents a context passed to a <see cref="CalendarMonthCellStyleContext"/> instance.
    /// </summary>
    public class CalendarMonthCellStyleContext : CalendarCellStyleContext
    {
        internal CalendarMonthCellStyleContext(CalendarCellStateContext context)
            : base(context)
        {
            this.IsSpecial = context.IsSpecial;
            this.IsSpecialReadOnly = context.IsSpecialReadOnly;
        }

        /// <summary>
        /// Gets a value indicating whether the associated calendar cell is special.
        /// </summary>
        public bool IsSpecial
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating whether the associated calendar cell is special and read-only.
        /// </summary>
        public bool IsSpecialReadOnly
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value of the special slots that are associated with the cell.
        /// </summary>
        public IEnumerable<Slot> SpecialSlots
        {
            get;
            internal set;
        }
    }
}
