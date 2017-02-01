using System;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// Provides a way to customize the calendar cell state (behavior) based on user-defined conditional rules.
    /// </summary>
    public class CalendarCellStateSelector
    {
        /// <summary>
        /// Evaluates the state (behavior) to be applied on the respective calendar cell.
        /// </summary>
        /// <param name="container">The <see cref="RadCalendar"/> instance that contains the respective cell.</param>
        /// <param name="context">The <see cref="CalendarCellStateContext"/> associated with the respective cell.</param>
        public void SelectState(CalendarCellStateContext context, RadCalendar container)
        {
            if (container == null)
            {
                return;
            }

            this.SelectStateCore(context, container);
        }

        /// <summary>
        /// When implemented by a derived class, provides a way to tap into the default calendar cell state (behavior) logic through the passed
        /// <see cref="CalendarCellStateContext"/> argument instance. For example you can mark the respective calendar cell as not available for selection by
        /// setting the <see cref="CalendarCellStateContext.IsBlackout"/> property, or you can highlight a calendar cell by setting the
        /// <see cref="CalendarCellStateContext.IsHighlighted"/> property.
        /// </summary>
        protected virtual void SelectStateCore(CalendarCellStateContext context, RadCalendar container)
        {
        }
    }
}
