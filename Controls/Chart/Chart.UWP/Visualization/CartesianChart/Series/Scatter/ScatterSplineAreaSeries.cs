using System;
using Telerik.Charting;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a chart series which visualize <see cref="ScatterDataPoint"/> instances by an area shape. Points are connected with smooth curve segments.
    /// </summary>
    public class ScatterSplineAreaSeries : ScatterAreaSeries
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScatterSplineAreaSeries"/> class.
        /// </summary>
        public ScatterSplineAreaSeries()
        {
            this.DefaultStyleKey = typeof(ScatterSplineAreaSeries);
        }

        internal override LineRenderer CreateRenderer()
        {
            return new SplineAreaRenderer();
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ScatterSplineAreaSeriesAutomationPeer(this);
        }
    }
}
