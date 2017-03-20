using System;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// This is a base class for the radial indicators.
    /// It keeps a cache of the attached properties of the radial
    /// range. It defines neither visualization nor behavior so it is marked abstract.
    /// </summary>
    public abstract class RadialGaugeIndicator : BarGaugeIndicator
    {
        private Point center = new Point();
        private double radius;
        private double minAngle;
        private double maxAngle;
        private double radiusScale;

        /// <summary>
        /// Initializes a new instance of the RadialGaugeIndicator class.
        /// </summary>
        protected RadialGaugeIndicator()
        {
        }

        /// <summary>
        /// Gets a cached value of the MinAngle attached property.
        /// </summary>
        protected double MinAngle
        {
            get
            {
                return this.minAngle;
            }
        }

        /// <summary>
        /// Gets a cached value of the MaxAngle attached property.
        /// </summary>
        protected double MaxAngle
        {
            get
            {
                return this.maxAngle;
            }
        }

        /// <summary>
        /// Gets a cached value of the center of the parent range.
        /// </summary>
        protected Point Center
        {
            get
            {
                return this.center;
            }
        }

        /// <summary>
        /// Gets a cached value of the radius of the parent range.
        /// </summary>
        protected double Radius
        {
            get
            {
                return this.radius;
            }
        }

        /// <summary>
        /// Gets a cached value of the IndicatorRadiusScale attached property.
        /// </summary>
        protected double RadiusScale
        {
            get
            {
                return this.radiusScale;
            }
        }

        /// <summary>
        /// This is a virtual method that resets the state of the indicator.
        /// The parent range is responsible to (indirectly) call this method when
        /// a property of importance changes.
        /// <param name="availableSize">A size which can be used by the update logic.</param>
        /// </summary>
        /// <remarks>
        /// The linear range for example triggers the UpdateOverride() method
        /// of its indicators when its Orientation property changes.
        /// </remarks>
        internal override void UpdateOverride(Size availableSize)
        {
            base.UpdateOverride(availableSize);
            this.PrivateUpdate();
        }

        internal override void OnValueChanged(double newValue, double oldValue)
        {
            base.OnValueChanged(newValue, oldValue);

            this.UpdateAngleRestrictions();
        }

        internal override void OnStartValueChanged(double newStartValue, double oldStartValue)
        {
            base.OnStartValueChanged(newStartValue, oldStartValue);

            this.UpdateAngleRestrictions();
        }

        /// <summary>
        /// Called in the measure layout pass to determine the desired size.
        /// </summary>
        /// <param name="availableSize">The available size that was given by the layout system.</param>
        /// <returns>Returns the desired size of the indicator.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size res = base.MeasureOverride(availableSize);

            this.CalculateRadiusAndCenter(res);

            return res;
        }

        /// <summary>
        /// Called in the arrange pass of the layout system.
        /// </summary>
        /// <param name="finalSize">The final size that was given by the layout system.</param>
        /// <returns>The final size of the panel.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.PrivateUpdate();
            return base.ArrangeOverride(finalSize);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadialGaugeIndicatorAutomationPeer(this);
        }

        private void CalculateRadiusAndCenter(Size availableSize)
        {
            double availableWidth = availableSize.Width;
            double availableHeight = availableSize.Height;

            this.radius = Math.Min(availableWidth, availableHeight) / 2;
            this.center.X = availableWidth / 2;
            this.center.Y = availableHeight / 2;
        }

        private void PrivateUpdate()
        {
            this.radiusScale = RadRadialGauge.GetIndicatorRadiusScale(this);
            this.UpdateAngleRestrictions();
        }

        private void UpdateAngleRestrictions()
        {
            var gauge = this.Owner.OwnerGauge as RadRadialGauge;

            var gaugeMinAngle = gauge.MinAngle;
            var gaugeMaxAngle = gauge.MaxAngle;

            var physicalLength = gaugeMaxAngle - gaugeMinAngle;
            var logicalLength = Math.Abs(this.Owner.OwnerGauge.MaxValue - this.Owner.OwnerGauge.MinValue);

            var logicalStartValue = this.StartValue - this.Owner.OwnerGauge.MinValue;
            var startValueAngle = RadGauge.MapLogicalToPhysicalValue(logicalStartValue, physicalLength, logicalLength);
            var actualValueAngle = RadGauge.MapLogicalToPhysicalValue(this.ActualValue - this.Owner.OwnerGauge.MinValue, physicalLength, logicalLength);

            this.minAngle = gaugeMinAngle + startValueAngle;
            this.maxAngle = gaugeMinAngle + actualValueAngle;
        }
    }
}
