using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class CoordinatesUtils
    {
        internal static RadPolarPoint GetCenterPosition(RadialLayoutSlot segment)
        {
            var radius = (segment.OuterRadius + segment.InnerRadius) / 2;
            var angle = segment.StartAngle + segment.SweepAngle / 2;

            return new RadPolarPoint(radius, angle);
        }
    }
}
