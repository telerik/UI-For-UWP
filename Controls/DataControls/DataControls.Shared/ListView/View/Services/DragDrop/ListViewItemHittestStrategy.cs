using System;
using System.Collections.Generic;
using System.Text;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

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
