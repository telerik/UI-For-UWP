using System;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// Represents a Gauge that arranges its ticks, labels and indicators in a circle.
    /// </summary>
    [TemplatePart(Name = "PART_Panel", Type = typeof(LinearGaugePanel))]
    public class RadLinearGauge : RadGauge
    {
        /// <summary>
        /// Identifies the Orientation property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.RegisterAttached("Orientation", typeof(Orientation), typeof(RadLinearGauge), new PropertyMetadata(Orientation.Horizontal, OnOrientationPropertyChanged));

        /// <summary>
        /// Identifies the IndicatorOffset property.
        /// </summary>
        public static readonly DependencyProperty IndicatorOffsetProperty =
            DependencyProperty.RegisterAttached("IndicatorOffset", typeof(double), typeof(RadLinearGauge), new PropertyMetadata(0d, OnIndicatorOffsetPropertyChanged));

        /// <summary>
        /// Identifies the LabelOffset property.
        /// </summary>
        public static readonly DependencyProperty LabelOffsetProperty =
            DependencyProperty.Register(nameof(LabelOffset), typeof(double), typeof(RadLinearGauge), new PropertyMetadata(15d, OnLabelOffsetPropertyChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="RadLinearGauge"/> class.
        /// </summary>
        public RadLinearGauge()
        {
            this.DefaultStyleKey = typeof(RadLinearGauge);
        }

        /// <summary>
        /// Gets or sets an offset that is used to nudge the labels to the right or left (top/bottom if Orientation is horizontal)
        /// of the ticks.
        /// </summary>
        /// <remarks>
        /// Setting LabelOffset to 0 in horizontal mode will move the labels to the top edge of the range.
        /// Setting it to 2 on the other hand will move them to the bottom. In essence this is a factor
        /// by which the position of the labels will be multiplied by.
        /// </remarks>
        public double LabelOffset
        {
            get
            {
                return (double)this.GetValue(LabelOffsetProperty);
            }
            set
            {
                this.SetValue(LabelOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets the type of this range. For this type it is always Linear.
        /// </summary>
        internal override GaugeType GaugeType
        {
            get
            {
                return GaugeType.Linear;
            }
        }

        /// <summary>
        /// Sets the IndicatorOffset attached property to the provided indicator.
        /// </summary>
        /// <param name="indicator">The indicator for which to set the offset.</param>
        /// <param name="value">The offset of the indicator.</param>
        /// <remarks>
        /// Setting IndicatorOffset to 0 in horizontal mode will move the indicators to the top edge of the range.
        /// Setting it to 2 on the other hand will move them to the bottom. In essence this is a factor
        /// by which the position of the indicators will be multiplied by.
        /// </remarks>
        public static void SetIndicatorOffset(DependencyObject indicator, double value)
        {
            if (indicator == null)
            {
                throw new ArgumentNullException(nameof(indicator));
            }

            indicator.SetValue(IndicatorOffsetProperty, value);
        }

        /// <summary>
        /// Gets the IndicatorOffset of the provided indicator.
        /// </summary>
        /// <param name="indicator">The indicator from which the offset will be obtained.</param>
        /// <returns>Returns the offset the provided indicator.</returns>
        public static double GetIndicatorOffset(DependencyObject indicator)
        {
            if (indicator == null)
            {
                throw new ArgumentNullException(nameof(indicator));
            }
            return (double)indicator.GetValue(IndicatorOffsetProperty);
        }

        /// <summary>
        /// Gets the Orientation value for the provided indicator.
        /// </summary>
        public static Orientation GetOrientation(DependencyObject indicator)
        {
            if (indicator == null)
            {
                throw new ArgumentNullException(nameof(indicator));
            }

            return (Orientation)indicator.GetValue(OrientationProperty);
        }

        /// <summary>
        /// Sets the specified Orientation value to the provided indicator.
        /// </summary>
        public static void SetOrientation(DependencyObject indicator, Orientation value)
        {
            if (indicator == null)
            {
                throw new ArgumentNullException(nameof(indicator));
            }

            indicator.SetValue(OrientationProperty, value);
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate"/> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            // Orientation orientation = GetOrientation(this);
            // SetOrientation(this.Panel, orientation);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadLinearGaugeAutomationPeer(this);
        }

        private static void OnOrientationPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            LinearGaugePanel panel = sender as LinearGaugePanel;
            if (panel == null)
            {
                return;
            }

            panel.OnOrientationChanged((Orientation)args.NewValue, (Orientation)args.OldValue);
        }

        private static void OnLabelOffsetPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            LinearGaugePanel range = sender as LinearGaugePanel;
            if (range == null)
            {
                return;
            }

            range.OnLabelOffsetChanged((double)args.NewValue, (double)args.OldValue);
        }

        private static void OnIndicatorOffsetPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GaugeIndicator indicator = sender as GaugeIndicator;
            if (indicator == null)
            {
                return;
            }

            indicator.ScheduleUpdate();
        }
    }
}
