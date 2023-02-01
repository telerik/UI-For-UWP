﻿using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents series which define a line with smooth curves among points.
    /// </summary>
    public class RadarSplineSeries : RadarLineSeries
    {
        /// <summary>
        /// Identifies the <see cref="SplineTension"/> property.
        /// </summary>   
        public static readonly DependencyProperty SplineTensionProperty =
            DependencyProperty.Register("SplineTension", typeof(double), typeof(RadarSplineSeries), new PropertyMetadata(SplineHelper.DefaultTension, OnSplineTensionChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="RadarSplineSeries"/> class.
        /// </summary>
        public RadarSplineSeries()
        {
            this.DefaultStyleKey = typeof(RadarSplineSeries);
        }

        /// <summary>
        /// Gets or sets the <see cref="SplineTension"/> that is used to determine the tension of the additional spline points.
        /// The default value is 0.5d.
        /// </summary>
        public double SplineTension
        {
            get { return (double)this.GetValue(SplineTensionProperty); }
            set { this.SetValue(SplineTensionProperty, value); }
        }

        internal override RadarLineRenderer CreateRenderer()
        {
            return new RadarSplineRenderer()
            {
                splineTension = this.SplineTension
            };
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadarSplineSeriesAutomationPeer(this);
        }

        private static void OnSplineTensionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadarSplineSeries series = (RadarSplineSeries)d;
            RadarSplineRenderer renderer = (RadarSplineRenderer)series.renderer;
            renderer.splineTension = (double)e.NewValue;
            series.InvalidateCore();
        }
    }
}
