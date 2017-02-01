using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    internal class ColumnHeaderHittestStrategy : DragHitTestStrategy
    {
        private IDragDropElement source;

        public ColumnHeaderHittestStrategy(IDragDropElement source, UIElement rootElement)
            : base(rootElement)
        {
            this.source = source;
        }

        protected override IEnumerable<IDragDropElement> HitTest(Rect globalHitTestBounds, Point restrictedPointerPosition)
        {
            var elements = base.HitTest(globalHitTestBounds, restrictedPointerPosition);

            if (elements.All(c => c is DataGridColumnHeader || c is DataGridColumnHeaderPanel))
            {
                elements = VisualTreeHelper.FindElementsInHostCoordinates(restrictedPointerPosition, this.RootElement).OfType<IDragDropElement>();
            }

            return elements.Where(e => e != this.source);
        }
    }
}