using System;
using Telerik.Charting;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Visualizes a collection of data points using a <see cref="Windows.UI.Xaml.Shapes.Line"/> shape.
    /// </summary>
    public class LineSeries : CategoricalStrokedSeries
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineSeries"/> class.
        /// </summary>
        public LineSeries()
        {
            this.DefaultStyleKey = typeof(LineSeries);
        }

        /// <summary>
        /// Gets the human-readable family of this instance. For example Area is the family for Area, SplineArea and PolarArea series.
        /// </summary>
        internal override string Family
        {
            get
            {
                return ChartPalette.LineFamily;
            }
        }

        internal override Windows.UI.Xaml.FrameworkElement SeriesVisual
        {
            get
            {
                return this.renderer.strokeShape;
            }
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new LineSeriesAutomationPeer(this);
        }
    }
}
