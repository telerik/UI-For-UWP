using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    internal class ListViewItemHittestStrategy : DragHitTestStrategy
    {
        private IDragDropElement source;

        public ListViewItemHittestStrategy(IDragDropElement source, UIElement rootElement)
            : base(rootElement)
        {
            this.source = source;
        }

        protected override IEnumerable<IDragDropElement> HitTest(Rect globalHitTestBounds, Point restrictedPointerPosition)
        {
            return new List<IDragDropElement> { this.source };
        }
    }
}
