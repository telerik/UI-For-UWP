using System;
using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a chart series, which can visualize <see cref="ScatterDataPoint"/> instances by connecting them with smooth curve segments.
    /// </summary>
    public class ScatterSplineSeries : ScatterLineSeries
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScatterSplineSeries"/> class.
        /// </summary>
        public ScatterSplineSeries()
        {
            this.DefaultStyleKey = typeof(ScatterSplineSeries);
        }

        internal override LineRenderer CreateRenderer()
        {
            return new SplineRenderer();
        }
    }
}
