using System;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents series which define an area with smooth curves among points.
    /// </summary>
    public class SplineAreaSeries : AreaSeries
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SplineAreaSeries"/> class.
        /// </summary>
        public SplineAreaSeries()
        {
            this.DefaultStyleKey = typeof(SplineAreaSeries);
        }

        internal override LineRenderer CreateRenderer()
        {
            return new SplineAreaRenderer();
        }
    }
}
