using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Primitives.DragDrop
{
    internal class DragStartingContext : EventArgs
    {
        public IDragSurface DragSurface { get; set; }
        public object Payload { get; set; }
        public FrameworkElement DragVisual { get; set; }
        public DragHitTestStrategy HitTestStrategy { get; set; }
    }
}
