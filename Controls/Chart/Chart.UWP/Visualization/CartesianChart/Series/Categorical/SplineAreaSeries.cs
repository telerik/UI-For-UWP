using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents series which define an area with smooth curves among points.
    /// </summary>
    public class SplineAreaSeries : AreaSeries
    {
        /// <summary>
        /// Identifies the <see cref="SplineTension"/> property.
        /// </summary>   
        public static readonly DependencyProperty SplineTensionProperty =
            DependencyProperty.Register("SplineTension", typeof(double), typeof(SplineAreaSeries), new PropertyMetadata(SplineHelper.DefaultTension, OnSplineTensionChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="SplineAreaSeries"/> class.
        /// </summary>
        public SplineAreaSeries()
        {
            this.DefaultStyleKey = typeof(SplineAreaSeries);
        }

        /// <summary>
        /// Gets or sets the <see cref="SplineTension"/> that is used to determine the tension of the additional spline points.
        /// The default value is 0.5d. The tension works with relative values between 0 and 1.
        /// Values outside this range will be coerced internally.
        /// </summary>
        public double SplineTension
        {
            get { return (double)this.GetValue(SplineTensionProperty); }
            set { this.SetValue(SplineTensionProperty, value); }
        }

        internal override LineRenderer CreateRenderer()
        {
            return new SplineAreaRenderer()
            {
                splineTension = this.SplineTension
            };
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new SplineAreaSeriesAutomationPeer(this);
        }

        private static void OnSplineTensionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplineAreaSeries series = (SplineAreaSeries)d;
            SplineAreaRenderer renderer = (SplineAreaRenderer)series.renderer;
            renderer.splineTension = RadMath.CoerceValue((double)e.NewValue, SplineHelper.MinTension, SplineHelper.MaxTension);
            series.InvalidateCore();
        }
    }
}
