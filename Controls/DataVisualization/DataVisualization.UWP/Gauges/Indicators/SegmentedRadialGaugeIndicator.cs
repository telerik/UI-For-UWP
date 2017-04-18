using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// This class represents a circle indicator with different segments.
    /// </summary>
    [ContentProperty(Name = "Segments")]
    public class SegmentedRadialGaugeIndicator : SegmentedGaugeIndicator
    {
        private double radius;
        private Point center;

        /// <summary>
        /// Represents Min calculated angle according the Value, StartValue and gauge MinValue defined.
        /// </summary>
        private double minAngle;

        /// <summary>
        /// Represents Max calculated angle according the Value, StartValue and gauge MaxValue defined.
        /// </summary>
        private double maxAngle;
        private double radiusScale;

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentedRadialGaugeIndicator"/> class.
        /// </summary>
        public SegmentedRadialGaugeIndicator()
        {
            this.DefaultStyleKey = typeof(SegmentedRadialGaugeIndicator);
        }

        /// <summary>
        /// This method is called so that a segmented indicator can synchronize
        /// its visual state with its current value.
        /// </summary>
        /// <param name="value">The value to synchronize with.</param>
        internal override void SyncWithValue(double value)
        {
            double currentAngle = RadialGaugePanel.CalculateAngle(this.CreateCalculateAngleContext(value));

            foreach (SegmentInfo info in this.SegmentInfos)
            {
                ArcSegment arc = (ArcSegment)info.PathSegment;
                if (currentAngle >= info.Start && currentAngle <= info.End)
                {
                    RadialBarGaugeIndicator.UpdateArc(this.CreateUpdateArcContext(currentAngle, arc));
                    arc.IsLargeArc = Math.Abs(currentAngle - info.Start) > Math.PI;
                }
                else if (currentAngle > info.End)
                {
                    RadialBarGaugeIndicator.UpdateArc(this.CreateUpdateArcContext(info.End, arc));
                    arc.IsLargeArc = Math.Abs(info.End - info.Start) > Math.PI;
                }
                else if (currentAngle <= info.Start)
                {
                    RadialBarGaugeIndicator.UpdateArc(this.CreateUpdateArcContext(info.Start, arc));
                    arc.IsLargeArc = false;
                }
            }
        }

        /// <summary>
        /// This method is called whenever the segments
        /// need to recreate their visual representation.
        /// </summary>
        /// <param name="availableSize">The available size which the visual parts can occupy.</param>
        internal override void ResetSegments(Size availableSize)
        {
            this.PrepareReset(availableSize);
            this.SegmentInfos.Clear();

            var currentstartAngle = this.minAngle;
            var angleSpan = this.maxAngle - this.minAngle;

            foreach (BarIndicatorSegment segment in this.Segments)
            {
                if (segment.Path == null)
                {
                    Debug.Assert(false, "Missing segment path.");
                    continue;
                }

                double lengthRatio = segment.Length / this.TotalSegmentLength;
                double angle = currentstartAngle + (angleSpan * lengthRatio);

                PathFigure figure = new PathFigure();
                double startAngleInRadians = RadialGaugePanel.ConvertDegreesToRadians(currentstartAngle);
                figure.StartPoint = RadialGaugePanel.CreateRotatedPoint(startAngleInRadians, this.radius, this.center, this.radiusScale);

                ArcSegment newArc = new ArcSegment();
                newArc.Point = figure.StartPoint;
                newArc.SweepDirection = SweepDirection.Clockwise;
                this.SegmentInfos.Add(new SegmentInfo() { PathSegment = newArc, Start = startAngleInRadians, End = RadialGaugePanel.ConvertDegreesToRadians(angle) });

                figure.Segments.Add(newArc);

                PathGeometry pathGeom = new PathGeometry();
                pathGeom.Figures.Add(figure);

                Path path = segment.Path;
                path.Stroke = segment.Stroke;
                path.StrokeThickness = segment.Thickness;
                path.Data = pathGeom;

                currentstartAngle = angle;
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new SegmentedRadialGaugeIndicatorAutomationPeer(this);
        }

        private void PrepareReset(Size availableSize)
        {
            this.radiusScale = RadRadialGauge.GetIndicatorRadiusScale(this);
            this.center = new Point(availableSize.Width / 2, availableSize.Height / 2);
            this.radius = Math.Min(availableSize.Width, availableSize.Height) / 2;

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

        private UpdateArcContext CreateUpdateArcContext(double angle, ArcSegment arc)
        {
            UpdateArcContext result = new UpdateArcContext();
            result.angle = angle;
            result.arc = arc;
            result.center = this.center;
            result.minAngle = this.minAngle;
            result.radius = this.radius * this.radiusScale;
            return result;
        }

        private CalculateAngleContext CreateCalculateAngleContext(double value)
        {
            var gauge = (RadRadialGauge)this.Owner.OwnerGauge;

            var gaugeLogicalLength = gauge.MaxValue - gauge.MinValue;
            var gaugePhysicalLength = gauge.MaxAngle - gauge.MinAngle;

            CalculateAngleContext result = new CalculateAngleContext(
                this.Owner.OwnerGauge.MinValue,
                value,
                this.minAngle,
                this.maxAngle,
                gaugeLogicalLength,
                gaugePhysicalLength);

            return result;
        }
    }
}
