using Telerik.UI.Xaml.Controls.Primitives;
namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal partial class SelectionService : ServiceBase<RadCalendar>
    {
        internal void SelectCell(CalendarCellModel cellModel)
        {
            this.Owner.SelectedDateRange = new CalendarDateRange(cellModel.Date, cellModel.Date);
        }
    }
}
