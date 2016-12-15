using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal struct CalendarLayoutContext
    {
        public static readonly CalendarLayoutContext Invalid = new CalendarLayoutContext(RadCalendar.InfinitySize);

        public Size AvailableSize;

        public CalendarLayoutContext(Size availableSize)
        {
            this.AvailableSize = availableSize;
        }
    }
}
