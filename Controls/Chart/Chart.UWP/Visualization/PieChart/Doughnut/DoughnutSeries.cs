using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Charting;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Markup;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a chart series that visualize data points using arcs that form a doughnut.
    /// </summary>
    [ContentProperty(Name = "DataPoints")]
    public class DoughnutSeries : PieSeries
    {
        /// <summary>
        /// Identifies the <see cref="InnerRadiusFactor"/> property.
        /// </summary>
        public static readonly DependencyProperty InnerRadiusFactorProperty =
            DependencyProperty.Register(nameof(InnerRadiusFactor), typeof(double), typeof(DoughnutSeries), new PropertyMetadata(0.5, OnInnerRadiusFactorChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="DoughnutSeries"/> class.
        /// </summary>
        public DoughnutSeries()
        {
            this.DefaultStyleKey = typeof(PieSeries);
        }

        /// <summary>
        /// Gets or sets the inner radius factor (that is the space that remains empty) of the series. The value is in logical units, in the range of [0, 1].
        /// </summary>
        public double InnerRadiusFactor
        {
            get
            {
                return (double)this.GetValue(InnerRadiusFactorProperty);
            }
            set
            {
                this.SetValue(InnerRadiusFactorProperty, value);
            }
        }

        internal override PieSegment CreateSegment()
        {
            return new DoughnutSegment();
        }

        internal override PieUpdateContext CreateUpdateContext()
        {
            return new DoughnutUpdateContext();
        }

        internal override PieUpdateContext SetupUpdateContext(RadSize availableSize, Size updatedAvailableSize, PieUpdateContext context)
        {
            var doghnutContext = base.SetupUpdateContext(availableSize, updatedAvailableSize, context) as DoughnutUpdateContext;

            if (context != null)
            {
                doghnutContext.InnerRadiusFactor = this.InnerRadiusFactor;
            }

            return doghnutContext;
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new DoughnutSeriesDataSource();
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DoughnutSeriesAutomationPeer(this);
        }

        private static void OnInnerRadiusFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DoughnutSeries series = d as DoughnutSeries;

            double radius = (double)e.NewValue;
            double clampedRadius = radius;

            if (clampedRadius < 0)
            {
                clampedRadius = 0;
            }
            else if (clampedRadius > 1)
            {
                clampedRadius = 1;
            }

            if (clampedRadius != radius)
            {
                series.SetValue(InnerRadiusFactorProperty, clampedRadius);
                return;
            }

            series.InvalidateCore();
        }
    }
}
