using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents series which define a line with smooth curves among points.
    /// </summary>
    public class RadarSplineSeries : RadarLineSeries
    {
          /// <summary>
        /// Initializes a new instance of the <see cref="RadarSplineSeries"/> class.
        /// </summary>
        public RadarSplineSeries()
        {
            this.DefaultStyleKey = typeof(RadarSplineSeries);
        }

        internal override RadarLineRenderer CreateRenderer()
        {
            return new RadarSplineRenderer();
        }
    }
}
