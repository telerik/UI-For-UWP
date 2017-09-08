using System;
using System.Collections.Generic;
using Telerik.Charting;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal static class SplineHelper
    {
        // TODO: Should we expose these as public properties for Spline and SplineArea series?
        private const double DefaultTension = 0.5d;
        private const double DefaultTolerance = 5d;

        public static IEnumerable<Point> GetSplinePoints(IList<DataPoint> dataPoints, DataPointSegment dataSegment, double scaleFactor, bool isClosedShape = false)
        {
            double tolerance = DefaultTolerance;
            if (scaleFactor > 2)
            {
                tolerance *= (int)scaleFactor / 2;
            }

            Point startPoint = FindPreviousNonEmptyPoint(dataPoints, dataSegment, isClosedShape).Center();
            Point endPoint = FindNextNonEmptyPoint(dataPoints, dataSegment, isClosedShape).Center();

            // NOTE: data segment is defined for at least two points so we do not need the check conditions for zero or one count.
            int segmentPointCount = dataSegment.EndIndex - dataSegment.StartIndex + 1;
            if (segmentPointCount == 2)
            {
                Point firstPoint = dataPoints[dataSegment.StartIndex].Center();
                Point secondPoint = dataPoints[dataSegment.EndIndex].Center();

                foreach (Point segmentedPoint in Segment(startPoint, firstPoint, secondPoint, endPoint, tolerance))
                {
                    yield return segmentedPoint;
                }

                yield break;
            }

            IEnumerable<Point> segmentedPoints;

            for (int i = dataSegment.StartIndex; i < dataSegment.EndIndex; i++)
            {
                if (i == dataSegment.StartIndex)
                {
                    segmentedPoints = Segment(
                        startPoint,
                        dataPoints[i].Center(),
                        dataPoints[i + 1].Center(),
                        dataPoints[i + 2].Center(),
                        tolerance);
                }
                else if (i == dataSegment.EndIndex - 1)
                {
                    segmentedPoints = Segment(
                        dataPoints[i - 1].Center(),
                        dataPoints[i].Center(),
                        dataPoints[i + 1].Center(),
                        endPoint,
                        tolerance);
                }
                else
                {
                    segmentedPoints = Segment(
                        dataPoints[i - 1].Center(),
                        dataPoints[i].Center(),
                        dataPoints[i + 1].Center(),
                        dataPoints[i + 2].Center(),
                        tolerance);
                }

                foreach (Point point in segmentedPoints)
                {
                    yield return point;
                }
            }
        }

        public static IEnumerable<Point> GetSplinePointsConnectingAbsoluteFirstLastDataPoints(IList<DataPoint> dataPoints, double scaleFactor)
        {
            DataPoint firstPoint = dataPoints[0];
            DataPoint lastPoint = dataPoints[dataPoints.Count - 1];

            if (firstPoint.isEmpty || double.IsNaN(firstPoint.CenterX()) || double.IsNaN(firstPoint.CenterY()) ||
                lastPoint.isEmpty || double.IsNaN(lastPoint.CenterX()) || double.IsNaN(lastPoint.CenterY()))
            {
                yield break;
            }

            double tolerance = DefaultTolerance;
            if (scaleFactor > 2)
            {
                tolerance *= (int)scaleFactor / 2;
            }

            DataPoint previousSignificantPointBeforeAbsoluteLast = FindPreviousNonEmptyPointBeforeAbsoluteLast(dataPoints);
            DataPoint nextSignificantPointAfterAbsoluteFirst = FindNextNonEmptyPointAfterAbsoluteFirst(dataPoints);

            // return the first point since spline segmentation skips it
            yield return lastPoint.Center();

            foreach (Point point in Segment(previousSignificantPointBeforeAbsoluteLast.Center(), lastPoint.Center(), firstPoint.Center(), nextSignificantPointAfterAbsoluteFirst.Center(), tolerance))
            {
                yield return point;
            }
        }

        private static IEnumerable<Point> Segment(Point pt0, Point pt1, Point pt2, Point pt3, double tolerance)
        {
            double sX1 = DefaultTension * (pt2.X - pt0.X);
            double sY1 = DefaultTension * (pt2.Y - pt0.Y);
            double sX2 = DefaultTension * (pt3.X - pt1.X);
            double sY2 = DefaultTension * (pt3.Y - pt1.Y);

            double ax = sX1 + sX2 + (2 * pt1.X) - (2 * pt2.X);
            double ay = sY1 + sY2 + (2 * pt1.Y) - (2 * pt2.Y);
            double bx = (-2 * sX1) - sX2 - (3 * pt1.X) + (3 * pt2.X);
            double by = (-2 * sY1) - sY2 - (3 * pt1.Y) + (3 * pt2.Y);

            double cx = sX1;
            double cy = sY1;
            double dx = pt1.X;
            double dy = pt1.Y;

            int num = Math.Max(2, (int)((Math.Abs(pt1.X - pt2.X) + Math.Abs(pt1.Y - pt2.Y)) / tolerance));

            for (int i = 1; i < num; i++)
            {
                double t = (double)i / (num - 1);
                yield return new Point(
                    (ax * t * t * t) + (bx * t * t) + (cx * t) + dx,
                    (ay * t * t * t) + (by * t * t) + (cy * t) + dy);
            }
        }

        private static DataPoint FindPreviousNonEmptyPoint(IList<DataPoint> dataPoints, DataPointSegment dataSegment, bool isClosedShape)
        {
            // NOTE: We cannot simply get the last point from the previous segment as "segment" is defined at least for two points
            // and it is possible that there are single non-empty points in-between segments.
            if (dataSegment.StartIndex > 0)
            {
                for (int i = dataSegment.StartIndex - 1; i >= 0; i--)
                {
                    var point = dataPoints[i];
                    if (!point.isEmpty && !double.IsNaN(point.CenterX()) && !double.IsNaN(point.CenterY()))
                    {
                        return point;
                    }
                }
            }

            if (isClosedShape)
            {
                for (int i = dataPoints.Count - 1; i >= 0; i--)
                {
                    var point = dataPoints[i];
                    if (!point.isEmpty && !double.IsNaN(point.CenterX()) && !double.IsNaN(point.CenterY()))
                    {
                        return point;
                    }
                }
            }

            return dataPoints[dataSegment.StartIndex];
        }

        private static DataPoint FindNextNonEmptyPoint(IList<DataPoint> dataPoints, DataPointSegment dataSegment, bool isClosedShape)
        {
            // NOTE: We cannot simply get the first point from the next segment as "segment" is defined at least for two points
            // and it is possible that there are single non-empty points in-between segments.
            if (dataSegment.EndIndex < dataPoints.Count - 1)
            {
                for (int i = dataSegment.EndIndex + 1; i < dataPoints.Count; i++)
                {
                    var point = dataPoints[i];
                    if (!point.isEmpty && !double.IsNaN(point.CenterX()) && !double.IsNaN(point.CenterY()))
                    {
                        return point;
                    }
                }
            }

            if (isClosedShape)
            {
                for (int i = 0; i < dataPoints.Count; i++)
                {
                    var point = dataPoints[i];
                    if (!point.isEmpty && !double.IsNaN(point.CenterX()) && !double.IsNaN(point.CenterY()))
                    {
                        return point;
                    }
                }
            }

            return dataPoints[dataSegment.EndIndex];
        }

        private static DataPoint FindPreviousNonEmptyPointBeforeAbsoluteLast(IList<DataPoint> points)
        {
            DataPoint previousPoint = points[points.Count - 1];

            for (int i = points.Count - 2; i > 0; i--)
            {
                if (!points[i].isEmpty && !double.IsNaN(points[i].CenterX()) && !double.IsNaN(points[i].CenterY()))
                {
                    previousPoint = points[i];
                    break;
                }
            }

            return previousPoint;
        }

        private static DataPoint FindNextNonEmptyPointAfterAbsoluteFirst(IList<DataPoint> points)
        {
            DataPoint nextPoint = points[0];

            for (int i = 1; i < points.Count - 2; i++)
            {
                if (!points[i].isEmpty && !double.IsNaN(points[i].CenterX()) && !double.IsNaN(points[i].CenterY()))
                {
                    nextPoint = points[i];
                    break;
                }
            }

            return nextPoint;
        }
    }
}