using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class RadPolarPoint
    {
        public RadPolarPoint(double radius, double angle)
        {
            this.Radius = radius;
            this.Angle = angle;
        }

        public double Radius { get; set; }
        public double Angle { get; set; }
    }
}
