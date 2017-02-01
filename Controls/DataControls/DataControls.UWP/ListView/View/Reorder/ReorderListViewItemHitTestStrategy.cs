using System.Collections.Generic;
using System.Linq;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    internal class ReorderListViewItemHitTestStrategy : DragHitTestStrategy
    {
        private IDragDropElement source;

        public ReorderListViewItemHitTestStrategy(IDragDropElement source, UIElement rootElement)
            : base(rootElement)
        {
            this.source = source;
        }

        protected override IEnumerable<IDragDropElement> HitTest(Rect globalHitTestBounds, Point restrictedPointerPosition)
        {
            var elements = VisualTreeHelper.FindElementsInHostCoordinates(restrictedPointerPosition, this.RootElement).OfType<IDragDropElement>().Where(c => !c.SkipHitTest);

            return elements.Where(e => e != this.source);
        }
    }
}
