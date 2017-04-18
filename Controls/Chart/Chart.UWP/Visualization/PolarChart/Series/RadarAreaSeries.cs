using System;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents <see cref="RadarLineSeries"/> that may optionally fill the area, enclosed by all points.
    /// </summary>
    public class RadarAreaSeries : RadarLineSeries, IFilledSeries
    {
        /// <summary>
        /// Identifies the <see cref="Fill"/> property.
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(RadarAreaSeries), new PropertyMetadata(null, OnFillChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="RadarAreaSeries"/> class.
        /// </summary>
        public RadarAreaSeries()
        {
            this.DefaultStyleKey = typeof(RadarAreaSeries);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Fill"/> property has been set locally.
        /// </summary>
        bool IFilledSeries.IsFillSetLocally
        {
            get
            {
                return this.ReadLocalValue(FillProperty) is Brush;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> that defines the interior of the area.
        /// </summary>
        public Brush Fill
        {
            get
            {
                return this.GetValue(FillProperty) as Brush;
            }
            set
            {
                this.SetValue(FillProperty, value);
            }
        }

        /// <summary>
        /// Gets the human-readable family of this instance. For example Area is the family for Area, SplineArea and RadarArea series.
        /// </summary>
        internal override string Family
        {
            get
            {
                return ChartPalette.AreaFamily;
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadarAreaSeriesAutomationPeer(this);
        }

        private static void OnFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadarAreaSeries series = d as RadarAreaSeries;
            series.renderer.strokeShape.Fill = e.NewValue as Brush;

            if (series.isPaletteApplied)
            {
                series.UpdatePalette(true);
            }
        }
    }
}
