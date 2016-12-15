using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;

namespace Telerik.UI.Xaml.Controls.Grid.Drag
{
    internal class DragSurfaceRequestedEventArgs
    {
        public IDragSurface DragSurface { get; set; }
    }
}
