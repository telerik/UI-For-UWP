using System;
using System.ComponentModel;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// This class represents a radial bar gauge indicator.
    /// </summary>
    [TemplatePart(Name = "PART_Path", Type = typeof(Path))]
    [TemplatePart(Name = "PART_Figure", Type = typeof(PathFigure))]
    public class RadialBarGaugeIndicator : RadialGaugeIndicator
    {
        internal ArcSegment arc = new ArcSegment();
        internal PathFigure figure;

        private const string PathPartName = "PART_Path";
        private const string FigurePartName = "PART_Figure";

        private Path path;

        /// <summary>
        /// Initializes a new instance of the RadialBarGaugeIndicator class.
        /// </summary>
        public RadialBarGaugeIndicator()
        {
            this.DefaultStyleKey = typeof(RadialBarGaugeIndicator);
            this.arc.SweepDirection = SweepDirection.Clockwise;
        }

        /// <summary>
        /// Updates an arc segment with the provided data.
        /// </summary>
        /// <param name="context">A context for the arc to be updated. the arc to update is a part of the context.</param>
        internal static void UpdateArc(UpdateArcContext context)
        {
            context.angle = context.arc.SweepDirection == SweepDirection.Clockwise ? context.angle : -context.angle;

            var arcWidth = Math.Abs(context.angle - RadialGaugePanel.ConvertDegreesToRadians(context.minAngle));

            // Ensure that start and endpoint will not match, so that arc is displayed.
            if (arcWidth >= 2 * Math.PI)
            {
                context.angle -= Math.Sign(context.angle - context.minAngle) * 0.01;
            }

            Point arcEndPoint = RadialGaugePanel.CreateRotatedPoint(context.angle, context.radius, context.center, 1);

            context.arc.IsLargeArc = arcWidth > Math.PI;
            context.arc.Size = new Size(context.radius, context.radius);
            context.arc.Point = arcEndPoint;
        }

        /// <summary>
        /// A virtual method that is called when the value of this indicator changes.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="oldValue">The old value.</param>
        internal override void OnValueChanged(double newValue, double oldValue)
        {
            base.OnValueChanged(newValue, oldValue);

            this.UpdateCore();
        }

        internal override void OnStartValueChanged(double newStartValue, double oldStartValue)
        {
            base.OnStartValueChanged(newStartValue, oldStartValue);

            this.UpdateCore();
        }

        /// <summary>
        /// A virtual method that is called when the thickness of this indicator changes.
        /// </summary>
        /// <param name="newThickness">The new thickness.</param>
        internal override void OnThicknessChanged(double newThickness)
        {
            base.OnThicknessChanged(newThickness);

            this.path.StrokeThickness = newThickness;
        }

        /// <summary>
        /// A virtual method that is called when the color of this indicator changes.
        /// </summary>
        /// <param name="newColor">The new color.</param>
        internal override void OnBrushChanged(Brush newColor)
        {
            base.OnBrushChanged(newColor);

            this.path.Stroke = newColor;
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

            this.UpdateCore();
        }

        /// <summary>
        /// Initializes the template parts of the indicator (see the TemplatePart attributes for more information).
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            base.ApplyTemplateCore();

            this.path = this.GetTemplatePartField<Path>(PathPartName) as Path;
            bool applied = this.path != null;

            this.figure = this.GetTemplatePartField<PathFigure>(FigurePartName) as PathFigure;
            applied = applied && this.figure != null;

            if (applied)
            {
                this.figure.Segments.Add(this.arc);
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
            finalSize = base.ArrangeOverride(finalSize);

            this.UpdateCore();

            return finalSize;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadialBarGaugeIndicatorAutomationPeer(this);
        }

        private void UpdateCore()
        {
            var startAngle = RadialGaugePanel.ConvertDegreesToRadians(this.MinAngle);
            double angle = RadialGaugePanel.CalculateAngle(this.CreateCalculateAngleContext(this.ActualValue));

            if (startAngle > angle)
            {
                var tmp = startAngle;
                startAngle = angle;
                angle = tmp;
            }

            this.figure.StartPoint = RadialGaugePanel.CreateRotatedPoint(startAngle, this.Radius, this.Center, this.RadiusScale);
            RadialBarGaugeIndicator.UpdateArc(this.CreateUpdateArcContext(angle, startAngle / Math.PI * 180));
        }

        private UpdateArcContext CreateUpdateArcContext(double angle)
        {
            return this.CreateUpdateArcContext(angle, this.MinAngle);
        }

        private UpdateArcContext CreateUpdateArcContext(double angle, double startAngle)
        {
            UpdateArcContext arcContext = new UpdateArcContext();
            arcContext.angle = angle;
            arcContext.center = this.Center;
            arcContext.radius = this.Radius * this.RadiusScale;
            arcContext.arc = this.arc;
            arcContext.minAngle = startAngle;

            return arcContext;
        }

        private CalculateAngleContext CreateCalculateAngleContext(double value)
        {
            var gauge = (RadRadialGauge)this.Owner.OwnerGauge;

            var startValue = Math.Max(this.StartValue, this.Owner.OwnerGauge.MinValue);
            var gaugeLogicalLength = gauge.MaxValue - gauge.MinValue;
            var gaugePhysicalLength = gauge.MaxAngle - gauge.MinAngle;

            CalculateAngleContext result = new CalculateAngleContext(
                startValue,
                value,
                this.MinAngle,
                this.MaxAngle,
                gaugeLogicalLength,
                gaugePhysicalLength);

            return result;
        }
    }
}
