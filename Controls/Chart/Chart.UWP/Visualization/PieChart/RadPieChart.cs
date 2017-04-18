using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Telerik.Charting;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Chart;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Markup;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a special chart that visualizes its data points using arc segments.
    /// </summary>
    [ContentProperty(Name = "Series")]
    public class RadPieChart : RadChartBase
    {
        private ChartAreaModel pieArea;
        private PieSeriesCollection series;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadPieChart"/> class.
        /// </summary>
        public RadPieChart()
        {
            this.DefaultStyleKey = typeof(RadPieChart);
            this.series = new PieSeriesCollection(this);
        }

        /// <summary>
        /// Gets all the data points plotted by this chart.
        /// </summary>
        public PieSeriesCollection Series
        {
            get
            {
                return this.series;
            }
        }

        internal override LegendItemCollection LegendInfosInternal
        {
            get
            {
                // TODO: Consider whether Multiple Pie Series should be handled.
                var pieSeries = this.Series.OfType<PieSeries>().FirstOrDefault();
                if (pieSeries != null)
                {
                    return pieSeries.LegendInfos;
                }

                return null;
            }
        }

        internal override IList SeriesInternal
        {
            get
            {
                return this.series;
            }
        }

        internal override RadRect PlotAreaDecorationSlot
        {
            get
            {
                if (this.series.Count > 0)
                {
                    return this.series[0].PieRect;
                }

                return base.PlotAreaDecorationSlot;
            }
        }

        /// <summary>
        /// Creates the model of the plot area.
        /// </summary>
        internal override ChartAreaModel CreateChartAreaModel()
        {
            this.pieArea = new ChartAreaModel();
            return this.pieArea;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadPieChartAutomationPeer(this);
        }
    }
}
