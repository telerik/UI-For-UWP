using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal class CalendarDragInitializer : DragInitializer
    {
        private const double DefaultCalendarStartTreshold = 30.0;
        private Point lastPressedPoint;
        private InputService owner;

        public CalendarDragInitializer(InputService owner, UIElement element)
            : base(element, DefaultCalendarStartTreshold)
        {
            this.owner = owner;
        }

        protected override void StartDrag(object sender, PointerRoutedEventArgs e)
        {
            this.owner.OnDragStarted(this.lastPressedPoint, e);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        protected override void OnPressed(PointerRoutedEventArgs e)
        {
            var hitPoint = e.GetCurrentPoint(this.owner.Owner.contentLayer.VisualElement).Position;

            if (hitPoint != null)
            {
                this.lastPressedPoint = hitPoint;
            }
        }
    }
}
