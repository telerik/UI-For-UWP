using System;
using Telerik.Charting;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal static class DoughnutSegmentRenderer
    {
        private const double ArcSegmentMaxAngle = 359.99;

        public static Geometry Render(PieDataPoint dataPoint, DoughnutSegmentData context)
        {
            if (dataPoint.SweepAngle >= ArcSegmentMaxAngle)
            {
                return RenderEllipse(context);
            }
            else
            {
                return RenderArc(context);
            }
        }

        public static Geometry RenderArc(DoughnutSegmentData context)
        {
            PathFigure figure = new PathFigure();
            figure.IsClosed = true;
            figure.IsFilled = true;

            RadPoint startPoint = RadMath.GetArcPoint(context.StartAngle, context.Center, context.Radius1);
            figure.StartPoint = startPoint.ToPoint();

            ArcSegment firstArc = new ArcSegment();
            firstArc.Size = new Size(context.Radius1, context.Radius1);
            firstArc.IsLargeArc = context.SweepAngle > 180 || context.SweepAngle < -180;

            var angle = context.StartAngle;
            if (context.SweepDirection == SweepDirection.Clockwise)
            {
                angle += context.SweepAngle;
            }
            else
            {
                angle -= context.SweepAngle;
            }

            firstArc.SweepDirection = context.SweepAngle > 0 ? context.SweepDirection : context.SweepDirection ^ SweepDirection.Clockwise;
            
            firstArc.Point = RadMath.GetArcPoint(angle, context.Center, context.Radius1).ToPoint();
            figure.Segments.Add(firstArc);

            LineSegment firstLine = new LineSegment();
            firstLine.Point = RadMath.GetArcPoint(angle, context.Center, context.Radius2).ToPoint();
            figure.Segments.Add(firstLine);

            ArcSegment secondArc = new ArcSegment();
            secondArc.Size = new Size(context.Radius2, context.Radius2);
            secondArc.IsLargeArc = context.SweepAngle > 180 || context.SweepAngle < -180;
            secondArc.SweepDirection = context.SweepAngle > 0 ? context.SweepDirection ^ SweepDirection.Clockwise : context.SweepDirection;
            secondArc.Point = RadMath.GetArcPoint(context.StartAngle, context.Center, context.Radius2).ToPoint();
            figure.Segments.Add(secondArc);

            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            return geometry;
        }

        public static Geometry RenderEllipse(DoughnutSegmentData context)
        {
            var centerPoint = new Point(context.Center.X, context.Center.Y);

            EllipseGeometry outerEllipse = new EllipseGeometry();
            outerEllipse.RadiusX = context.Radius1;
            outerEllipse.RadiusY = context.Radius1;
            outerEllipse.Center = centerPoint;

            EllipseGeometry innerEllipse = new EllipseGeometry();

            double innerRadius = context.Radius2;
            innerEllipse.RadiusX = innerRadius;
            innerEllipse.RadiusY = innerRadius;

            innerEllipse.Center = centerPoint;

            GeometryGroup data = new GeometryGroup();
            data.Children.Add(outerEllipse);
            data.Children.Add(innerEllipse);

            return data;
        }
    }
}