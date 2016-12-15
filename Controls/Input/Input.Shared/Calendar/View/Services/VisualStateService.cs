using Telerik.UI.Xaml.Controls.Primitives;
namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal class VisualStateService : ServiceBase<RadCalendar>
    {
        internal VisualStateService(RadCalendar owner)
            : base(owner)
        {
        }

        internal void UpdateHoverDecoration(CalendarCellModel hoveredNode)
        {
            this.Owner.visualStateLayer.UpdateHoverDecoration(hoveredNode);
        }

        internal void UpdateHoldDecoration(CalendarCellModel hoveredCellModel)
        {
            this.Owner.visualStateLayer.UpdateHoldDecoration(hoveredCellModel);
        }
    }
}
