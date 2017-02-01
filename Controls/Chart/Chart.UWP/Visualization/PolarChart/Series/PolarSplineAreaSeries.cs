using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents series which define a area with smooth curves among points.
    /// </summary>
    public class PolarSplineAreaSeries : PolarAreaSeries
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PolarSplineAreaSeries"/> class.
        /// </summary>
        public PolarSplineAreaSeries()
        {
            this.DefaultStyleKey = typeof(PolarSplineAreaSeries);
        }

        internal override PolarLineRenderer CreateRenderer()
        {
            return new PolarSplineRenderer();
        }
    }
}
