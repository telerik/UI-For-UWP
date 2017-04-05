using System;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// Represents a Gauge that arranges its ticks, labels and indicators in a circle.
    /// </summary>
    [TemplatePart(Name = "PART_Panel", Type = typeof(RadialGaugePanel))]
    public class RadRadialGauge : RadGauge
    {
        /// <summary>
        /// Identifies the MaxAngle attached property.
        /// </summary>
        public static readonly DependencyProperty MaxAngleProperty =
            DependencyProperty.Register(nameof(MaxAngle), typeof(double), typeof(RadRadialGauge), new PropertyMetadata(180.0, OnAnglePropertyChanged));

        /// <summary>
        /// Identifies the MinAngle attached property.
        /// </summary>
        public static readonly DependencyProperty MinAngleProperty =
            DependencyProperty.Register(nameof(MinAngle), typeof(double), typeof(RadRadialGauge), new PropertyMetadata(0.0d, OnAnglePropertyChanged));

        /// <summary>
        /// Identifies the IndicatorRadiusScale attached property.
        /// </summary>
        public static readonly DependencyProperty IndicatorRadiusScaleProperty =
            DependencyProperty.RegisterAttached("IndicatorRadiusScale", typeof(double), typeof(RadRadialGauge), new PropertyMetadata(1d, OnIndicatorRadiusScalePropertyChanged));

        /// <summary>
        /// Identifies the TickRadiusScale property.
        /// </summary>
        public static readonly DependencyProperty TickRadiusScaleProperty =
            DependencyProperty.Register(nameof(TickRadiusScale), typeof(double), typeof(RadRadialGauge), new PropertyMetadata(1d, OnRadiusScalePropertyChanged));

        /// <summary>
        /// Identifies the LabelRadiusScale property.
        /// </summary>
        public static readonly DependencyProperty LabelRadiusScaleProperty =
            DependencyProperty.Register(nameof(LabelRadiusScale), typeof(double), typeof(RadRadialGauge), new PropertyMetadata(1d, OnRadiusScalePropertyChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="RadRadialGauge"/> class.
        /// </summary>
        public RadRadialGauge()
        {
            this.DefaultStyleKey = typeof(RadRadialGauge);
        }

        /// <summary>
        /// Gets or sets a scale factor that will be multiplied by the radius
        /// of this range in order to position the ticks.
        /// </summary>
        /// <remarks>
        /// Setting this property to 1 will cause the ticks to be placed as
        /// far away from the center as possible. Setting it to 0 will cause all
        /// ticks to be placed at the center. The radius of the range is half
        /// the value of the minimum between the width and height of the layout slot.
        /// </remarks>
        public double TickRadiusScale
        {
            get
            {
                return (double)this.GetValue(TickRadiusScaleProperty);
            }

            set
            {
                this.SetValue(TickRadiusScaleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a scale factor that will be multiplied by the radius
        /// of this range in order to position the labels.
        /// </summary>
        /// <remarks>
        /// Setting this property to 1 will cause the labels to be placed as
        /// far away from the center as possible. Setting it to 0 will cause all
        /// labels to be placed at the center. The radius of the range is half
        /// the value of the minimum between the width and height of the layout slot.
        /// </remarks>
        public double LabelRadiusScale
        {
            get
            {
                return (double)this.GetValue(LabelRadiusScaleProperty);
            }

            set
            {
                this.SetValue(LabelRadiusScaleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the MaxAngle property.
        /// </summary>
        public double MaxAngle
        {
            get
            {
                return (double)this.GetValue(MaxAngleProperty);
            }

            set
            {
                this.SetValue(MaxAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the MinAngle property.
        /// </summary>
        public double MinAngle
        {
            get
            {
                return (double)this.GetValue(MinAngleProperty);
            }

            set
            {
                this.SetValue(MinAngleProperty, value);
            }
        }

        internal override GaugeType GaugeType
        {
            get
            {
                return GaugeType.Radial;
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadRadialGaugeAutomationPeer(this);
        }

        /// <summary>
        /// Gets the IndicatorRadiusScale attached property.
        /// </summary>
        /// <param name="indicator">The indicator for which to obtain the IndicatorRadiusScale.</param>
        /// <returns>Returns the IndicatorRadiusScale of the provided indicator.</returns>
        public static double GetIndicatorRadiusScale(DependencyObject indicator)
        {
            if (indicator == null)
            {
                throw new ArgumentNullException(nameof(indicator));
            }
            return (double)indicator.GetValue(IndicatorRadiusScaleProperty);
        }

        /// <summary>
        /// Sets the IndicatorRadiusScale attached property.
        /// </summary>
        /// <param name="indicator">The indicator on which to set the IndicatorRadiusScale property.</param>
        /// <param name="scale">The IndicatorRadiusScale value which will be set on the indicator.</param>
        public static void SetIndicatorRadiusScale(DependencyObject indicator, double scale)
        {
            if (indicator == null)
            {
                throw new ArgumentNullException(nameof(indicator));
            }
            indicator.SetValue(IndicatorRadiusScaleProperty, scale);
        }

        private static void OnAnglePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadRadialGauge gauge = sender as RadRadialGauge;
            if (!gauge.IsTemplateApplied)
            {
                return;
            }

            gauge.Panel.ScheduleUpdate();
        }

        private static void OnIndicatorRadiusScalePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GaugeIndicator indicator = sender as GaugeIndicator;
            if (indicator == null)
            {
                return;
            }

            indicator.ScheduleUpdate();
        }

        private static void OnRadiusScalePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadRadialGauge gauge = sender as RadRadialGauge;
            if (!gauge.IsTemplateApplied)
            {
                return;
            }

            gauge.Panel.ScheduleUpdate();
        }
    }
}
