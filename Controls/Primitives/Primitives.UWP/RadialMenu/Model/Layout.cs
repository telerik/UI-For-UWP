using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal abstract class Layout
    {
        public double StartAngle { get; set; }
        public abstract RadialLayoutSlot GetLayoutSlotAtPosition(RingModelBase model, RadialSegment segment);
    }
}
