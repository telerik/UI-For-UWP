using System;
using Telerik.Charting;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal partial class PieSegment
    {
        internal const double ArcSegmentMaxAngle = 360;

        internal DataPoint point;
        internal bool isDefaultStyle;
        internal PathFigure figure;
        private Path path;
        private Path highlightPath;

        public PieSegment()
        {
            this.highlightPath = new Path();
        }

        /// <summary>
        /// Gets the shape that represents this segment.
        /// </summary>
        public virtual Path Path
        {
            get
            {
                if (this.path == null)
                {
                    this.figure = new PathFigure();
                    this.figure.IsClosed = true;
                    this.figure.IsFilled = true;

                    PathGeometry geometry = new PathGeometry();
                    geometry.Figures.Add(this.figure);

                    this.path = new Path();
                    this.path.Data = geometry;
                }

                return this.path;
            }
        }

        /// <summary>
        /// Gets the shape that represents the highlight shape over the segment.
        /// </summary>
        public Path HighlightPath
        {
            get
            {
                return this.highlightPath;
            }
        }

        internal static Point GetCenterPointWithOffset(PieDataPoint dataPoint, PieUpdateContext context)
        {
            double offset = dataPoint.OffsetFromCenter;
            if (offset == 0)
            {
                return context.Center;
            }

            double pixelOffset = (int)(context.Radius * offset);
            double originalRadius = context.Radius;
            context.Radius = pixelOffset;

            Point centerWithOffset = context.CalculateArcPoint(context.StartAngle + (dataPoint.sweepAngle / 2));
            context.Radius = originalRadius;

            return centerWithOffset;
        }

        internal void Update(PieDataPoint dataPoint, PieUpdateContext context)
        {
            this.UpdateCore(dataPoint, context);

            this.point = dataPoint;

            // tag is used for hit-testing purposes
            this.Path.Tag = dataPoint;
        }

        internal void BuildArc(PieDataPoint dataPoint, PieUpdateContext updateContext)
        {
            if (this.figure == null)
            {
                return;
            }

            Point originalCenter = updateContext.Center;
            updateContext.Center = GetCenterPointWithOffset(dataPoint, updateContext);
            this.figure.StartPoint = updateContext.Center;

            this.UpdateHighlightPath(dataPoint, updateContext);

            // first line
            LineSegment line1 = new LineSegment();
            line1.Point = updateContext.CalculateArcPoint(updateContext.StartAngle);
            this.figure.Segments.Add(line1);

            // arc
            ArcSegment arc = new ArcSegment();
            arc.IsLargeArc = dataPoint.SweepAngle > 180 || dataPoint.SweepAngle < -180;
            arc.Size = updateContext.ArcSize;
            arc.SweepDirection = dataPoint.SweepAngle > 0 ? updateContext.SweepDirection : updateContext.SweepDirection ^ SweepDirection.Clockwise;

            // advance the starting angle
            if (updateContext.SweepDirection == SweepDirection.Clockwise)
            {
                updateContext.StartAngle += dataPoint.SweepAngle;
            }
            else
            {
                updateContext.StartAngle -= dataPoint.SweepAngle;
            }

            arc.Point = updateContext.CalculateArcPoint(updateContext.StartAngle);
            this.figure.Segments.Add(arc);

            // second line
            LineSegment line2 = new LineSegment();
            line2.Point = updateContext.Center;
            this.figure.Segments.Add(line2);

            updateContext.Center = originalCenter;
        }

        protected virtual void UpdateCore(PieDataPoint dataPoint, PieUpdateContext context)
        {
            if (this.figure != null)
            {
                this.figure.Segments.Clear();

                if (dataPoint.SweepAngle >= ArcSegmentMaxAngle)
                {
                    this.AddEllipse(context);
                }
                else
                {
                    this.BuildArc(dataPoint, context);
                }

                dataPoint.Radius = context.Radius;
                dataPoint.CenterPoint = context.Center;
            }
        }

        private void UpdateHighlightPath(PieDataPoint dataPoint, PieUpdateContext updateContext)
        {
            DoughnutSegmentData segment = new DoughnutSegmentData()
            {
                Center = new RadPoint(updateContext.Center.X, updateContext.Center.Y),
                Radius1 = updateContext.Radius,
                Radius2 = updateContext.Radius * this.ParentSeries.HighlightInnerRadiusFactor,
                StartAngle = updateContext.StartAngle,
                SweepAngle = dataPoint.SweepAngle,
                SweepDirection = updateContext.SweepDirection
            };

            this.highlightPath.Data = DoughnutSegmentRenderer.Render(dataPoint, segment);
        }

        private void AddEllipse(PieUpdateContext context)
        {
            this.figure.StartPoint = context.CalculateArcPoint(0);

            ArcSegment arc = new ArcSegment();
            arc.IsLargeArc = true;
            arc.Size = context.ArcSize;
            arc.SweepDirection = SweepDirection.Clockwise;
            arc.Point = context.CalculateArcPoint(180);

            this.figure.Segments.Add(arc);

            arc = new ArcSegment();
            arc.IsLargeArc = true;
            arc.Size = context.ArcSize;
            arc.SweepDirection = SweepDirection.Clockwise;
            arc.Point = context.CalculateArcPoint(360);

            // update the highlight path
            EllipseGeometry outerEllipse = new EllipseGeometry();
            outerEllipse.RadiusX = context.Radius;
            outerEllipse.RadiusY = context.Radius;
            outerEllipse.Center = context.Center;

            EllipseGeometry innerEllipse = new EllipseGeometry();
            innerEllipse.RadiusX = context.Radius * this.ParentSeries.HighlightInnerRadiusFactor;
            innerEllipse.RadiusY = context.Radius * this.ParentSeries.HighlightInnerRadiusFactor;
            innerEllipse.Center = context.Center;

            GeometryGroup data = new GeometryGroup();
            data.Children.Add(outerEllipse);
            data.Children.Add(innerEllipse);

            this.highlightPath.Data = data;

            this.figure.Segments.Add(arc);
        }
    }
}