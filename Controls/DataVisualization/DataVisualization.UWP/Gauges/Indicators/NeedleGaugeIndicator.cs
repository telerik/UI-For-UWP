using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// Represents a base class for needle indicators. For example and arrow or some
    /// other thin and pointy indicator. This class provides a rotation render transform.
    /// </summary>
    ////[TemplatePart(Name = NeedleRotateTransformPartName, Type = typeof(RotateTransform))]
    public abstract class NeedleGaugeIndicator : RadialGaugeIndicator
    {
        private const string NeedleRotateTransformPartName = "PART_NeedleRotateTransform";
        private RotateTransform needleRotation;

        /// <summary>
        /// Initializes a new instance of the NeedleGaugeIndicator class.
        /// </summary>
        protected NeedleGaugeIndicator()
        {
        }

        /// <summary>
        /// A virtual method that is called when the value of this indicator changes.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="oldValue">The old value.</param>
        internal override void OnValueChanged(double newValue, double oldValue)
        {
            base.OnValueChanged(newValue, oldValue);
            this.UpdateAngle(this.ActualValue - this.Owner.OwnerGauge.MinValue);
        }

        /// <summary>
        /// This is a virtual method that resets the state of the indicator.
        /// The parent range is responsible to (indirectly) call this method when
        /// a property of importance changes.
        /// </summary>
        /// <param name="availableSize">A size which can be used by the update logic.</param>
        /// <remarks>
        /// The linear range for example triggers the UpdateOverride() method
        /// of its indicators when its Orientation property changes.
        /// </remarks>
        internal override void UpdateOverride(Size availableSize)
        {
            base.UpdateOverride(availableSize);

            this.UpdateAngle(this.ActualValue - this.Owner.OwnerGauge.MinValue);
        }

        /// <summary>
        /// Initializes the template parts of the indicator (see the TemplatePart attributes for more information).
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            base.ApplyTemplateCore();

            this.needleRotation = this.GetTemplatePartField<RotateTransform>(NeedleRotateTransformPartName);
            bool applied = this.needleRotation != null;

            if (applied)
            {
                this.UpdateAngle(this.ActualValue);
            }

            return applied;
        }

        /// <summary>
        /// Called in the arrange pass of the layout system.
        /// </summary>
        /// <param name="finalSize">The final size that was given by the layout system.</param>
        /// <returns>The final size of the panel.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.UpdateAngle(this.ActualValue - this.Owner.OwnerGauge.MinValue);
            return base.ArrangeOverride(finalSize);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new NeedleGaugeIndicatorAutomationPeer(this);
        }

        private void UpdateAngle(double value)
        {
            if (this.Owner == null || this.Owner.OwnerGauge == null)
            {
                return;
            }

            var gauge = this.Owner.OwnerGauge as RadRadialGauge;
            var minAngle = gauge.MinAngle;
            var maxAngle = gauge.MaxAngle;

            double angle = RadGauge.MapLogicalToPhysicalValue(value, maxAngle - minAngle, this.MinMaxValueDifference);
            this.needleRotation.Angle = minAngle + angle;
        }
    }
}
