using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// This class represents a line indicator with different segments.
    /// </summary>
    [ContentProperty(Name = "Segments")]
    public class SegmentedLinearGaugeIndicator : SegmentedGaugeIndicator
    {
        private double extent;
        private bool isHorizontal = true;
        private double minValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentedLinearGaugeIndicator"/> class.
        /// </summary>
        public SegmentedLinearGaugeIndicator()
        {
            this.DefaultStyleKey = typeof(SegmentedLinearGaugeIndicator);
        }

        /// <summary>
        /// A virtual method that is called when the Owner of this indicator changes.
        /// </summary>
        /// <param name="newOwner">The new Owner.</param>
        /// <param name="oldOwner">The old Owner.</param>
        internal override void OnOwnerChanged(GaugePanel newOwner, GaugePanel oldOwner)
        {
            base.OnOwnerChanged(newOwner, oldOwner);

            Orientation orientation = RadLinearGauge.GetOrientation(newOwner);
            RadLinearGauge.SetOrientation(this, orientation);

            this.isHorizontal = orientation == Orientation.Horizontal;
            this.minValue = newOwner.OwnerGauge.MinValue;
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
            this.isHorizontal = RadLinearGauge.GetOrientation(this) == Orientation.Horizontal;
            this.minValue = this.Owner.OwnerGauge.MinValue;

            base.UpdateOverride(availableSize);
        }

        /// <summary>
        /// This method is called whenever the segments
        /// need to recreate their visual representation.
        /// </summary>
        /// <param name="availableSize">The available size which the visual parts can occupy.</param>
        internal override void ResetSegments(Size availableSize)
        {
            this.SegmentInfos.Clear();

            this.extent = this.isHorizontal ? availableSize.Width : availableSize.Height;

            double logicalRange = this.MinMaxValueDifference;
            double physicalStart = RadGauge.MapLogicalToPhysicalValue(this.StartValue - this.minValue, this.extent, logicalRange);

            double indicatorExtent = RadGauge.MapLogicalToPhysicalValue(this.ActualValue - this.minValue, this.extent, logicalRange) - physicalStart;
            double startPosition = this.isHorizontal ? physicalStart : (this.extent - physicalStart);

            double halfAvailableWidth = availableSize.Width / 2;
            double halfAvailableHeight = availableSize.Height / 2;
            double offset = RadLinearGauge.GetIndicatorOffset(this);

            foreach (BarIndicatorSegment segment in this.Segments)
            {
                if (segment.Path == null)
                {
                    Debug.Assert(false, "Missing segment path.");
                    continue;
                }
                double lengthRatio = segment.Length / this.TotalSegmentLength;
                double lineLength = indicatorExtent * lengthRatio;

                SegmentInfo info = new SegmentInfo();
                info.Start = startPosition;
                info.End = startPosition + (this.isHorizontal ? lineLength : -lineLength);
                this.SegmentInfos.Add(info);

                LineSegment line = new LineSegment();
                line.Point = this.isHorizontal ? new Point(info.End, halfAvailableHeight + offset) : new Point(halfAvailableWidth + offset, info.End);

                info.PathSegment = line;

                PathFigure figure = new PathFigure();
                figure.StartPoint = this.isHorizontal ? new Point(info.Start, halfAvailableHeight + offset) : new Point(halfAvailableWidth + offset, info.Start);
                figure.Segments.Add(line);

                PathGeometry geom = new PathGeometry();
                geom.Figures.Add(figure);

                Path path = segment.Path;
                path.Stroke = segment.Stroke;
                path.StrokeThickness = segment.Thickness;
                path.Data = geom;

                startPosition = info.End;
            }
        }

        /// <summary>
        /// This method is called so that a segmented indicator can synchronize
        /// its visual state with its current value.
        /// </summary>
        /// <param name="value">The value to synchronize with.</param>
        internal override void SyncWithValue(double value)
        {
            double currentPosition = RadGauge.MapLogicalToPhysicalValue(value - this.minValue, this.extent, this.MinMaxValueDifference);
            if (!this.isHorizontal)
            {
                currentPosition = this.extent - currentPosition;
            }

            foreach (SegmentInfo info in this.SegmentInfos)
            {
                LineSegment line = (LineSegment)info.PathSegment;
                Point currentPoint = line.Point;
                double start = info.Start;
                double end = info.End;

                if (!this.isHorizontal)
                {
                    double tmp = start;
                    start = end;
                    end = tmp;
                }

                if (start <= currentPosition && end >= currentPosition)
                {
                    line.Point = this.isHorizontal ? new Point(currentPosition, currentPoint.Y) : new Point(currentPoint.X, currentPosition);
                }
                else if (end <= currentPosition)
                {
                    line.Point = this.isHorizontal ? new Point(info.End, currentPoint.Y) : new Point(currentPoint.X, info.Start);
                }
                else if (start >= currentPosition)
                {
                    line.Point = this.isHorizontal ? new Point(info.Start, currentPoint.Y) : new Point(currentPoint.X, info.End);
                }
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new SegmentedLinearGaugeIndicatorAutomationPeer(this);
        }
    }
}
