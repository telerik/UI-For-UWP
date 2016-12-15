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
    public class PolarSplineSeries : PolarLineSeries
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PolarSplineSeries"/> class.
        /// </summary>
        public PolarSplineSeries()
        {
            this.DefaultStyleKey = typeof(PolarSplineSeries);
        }

        internal override PolarLineRenderer CreateRenderer()
        {
            return new PolarSplineRenderer();
        }
    }
}
