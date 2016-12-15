using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class RadialSegment
    {
        internal RadialLayoutSlot LayoutSlot { get; set; }

        internal RadialMenuItem TargetItem { get; set; }

        internal object Visual { get; set; }
    }
}
