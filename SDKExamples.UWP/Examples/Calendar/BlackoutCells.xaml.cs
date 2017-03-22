using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Input.Calendar;

namespace SDKExamples.UWP.Calendar
{
    public sealed partial class BlackoutCells : ExamplePageBase
    {
        public BlackoutCells()
        {
            this.InitializeComponent();
        }
    }

    public class CustomBlackoutStateSelector : CalendarCellStateSelector
    {
        protected override void SelectStateCore(CalendarCellStateContext context, RadCalendar container)
        {
            if (container.DisplayMode == CalendarDisplayMode.YearView && context.Date.Month % 2 == 0)
            {
                context.IsBlackout = true;
            }

            if (container.DisplayMode == CalendarDisplayMode.MonthView && context.Date.Day % 3 == 0)
            {
                context.IsBlackout = true;
            }
        }
    }
}
