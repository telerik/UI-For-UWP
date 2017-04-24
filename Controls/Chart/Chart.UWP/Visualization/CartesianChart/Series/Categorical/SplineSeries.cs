using System;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Visualizes a collection of data points using a smooth <see cref="Windows.UI.Xaml.Shapes.Line"/> shape.
    /// </summary>
    public class SplineSeries : LineSeries
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SplineSeries"/> class.
        /// </summary>
        public SplineSeries()
        {
            this.DefaultStyleKey = typeof(SplineSeries);
        }

        internal override LineRenderer CreateRenderer()
        {
            return new SplineRenderer();
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new SplineSeriesAutomationPeer(this);
        }
    }
}
