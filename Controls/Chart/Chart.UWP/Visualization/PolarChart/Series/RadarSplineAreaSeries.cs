using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents series which define a area with smooth curves among points.
    /// </summary>
    public class RadarSplineAreaSeries : RadarAreaSeries
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadarSplineAreaSeries"/> class.
        /// </summary>
        public RadarSplineAreaSeries()
        {
            this.DefaultStyleKey = typeof(RadarSplineAreaSeries);
        }

        internal override RadarLineRenderer CreateRenderer()
        {
            return new RadarSplineRenderer();
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadarSplineAreaSeriesAutomationPeer(this);
        }
    }
}
