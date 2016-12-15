using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class RadialLayoutSlot
    {
        internal static RadialLayoutSlot Invalid = new RadialLayoutSlot();

        public double StartAngle { get; set; }
        public double SweepAngle { get; set; }
        public double InnerRadius { get; set; }
        public double OuterRadius { get; set; }
    }
}
