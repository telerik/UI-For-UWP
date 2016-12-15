using System;
using System.Collections.Generic;
using Telerik.Charting;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class AreaRenderer : AreaRendererBase
    {
        internal AreaSeriesStrokeMode strokeMode;

        public AreaRenderer()
        {
            this.strokeMode = AreaSeriesStrokeMode.Points;
        }

        protected override bool ShouldAddBottomPointsToStroke
        {
            get
            {
                return (this.strokeMode & AreaSeriesStrokeMode.PlotLine) == AreaSeriesStrokeMode.PlotLine;
            }
        }

        protected override bool ShouldAddTopPointsToStroke
        {
            get
            {
                return (this.strokeMode & AreaSeriesStrokeMode.Points) == AreaSeriesStrokeMode.Points;
            }
        }

        protected override bool ShouldAddRightPointsToStroke
        {
            get
            {
                return (this.strokeMode & AreaSeriesStrokeMode.RightLine) == AreaSeriesStrokeMode.RightLine;
            }
        }

        protected override bool ShouldAddLeftPointsToStroke
        {
            get
            {
                return (this.strokeMode & AreaSeriesStrokeMode.LeftLine) == AreaSeriesStrokeMode.LeftLine;
            }
        }

        protected override IEnumerable<Point> GetTopPoints(DataPointSegment segment)
        {
            int pointIndex = segment.StartIndex;
            while (pointIndex <= segment.EndIndex)
            {
                yield return this.renderPoints[pointIndex].Center();

                pointIndex++;
            }
        }
        
        protected override IList<Windows.Foundation.Point> GetBottomPoints(AreaRenderContext context)
        {
            List<Point> points = new List<Point>();
            DataPointSegment currentSegment = context.CurrentSegment;

            ReferenceDictionary<string, Delegate> valueExtractor;
            if (context.PlotDirection == AxisPlotDirection.Vertical)
            {
                valueExtractor = AreaRendererBase.VerticalPlotValueExtractors;
            }
            else
            {
                valueExtractor = AreaRendererBase.HorizontalPlotValueExtractors;
            }

            if (!context.IsStacked)
            {
                Func<DataPoint, double, Point> pointGetter = (Func<DataPoint, double, Point>)valueExtractor[PointGetter];

                points.Add(pointGetter(this.renderPoints[currentSegment.StartIndex], context.PlotLine));
                points.Add(pointGetter(this.renderPoints[currentSegment.EndIndex], context.PlotLine));
            }
            else
            {
                Func<Point, int> pointBoundsGetter = (Func<Point, int>)valueExtractor[PointBoundsGetter];
                Func<DataPoint, bool, int> dataPointBoundsGetter = (Func<DataPoint, bool, int>)valueExtractor[DataPointBoundsGetter];
                Func<int, int, bool, bool> shouldSkipPreviousPoints = (Func<int, int, bool, bool>)valueExtractor[ShouldSkipPreviousPoints];
                Func<int, int, bool, bool> segmentEndNotReached = (Func<int, int, bool, bool>)valueExtractor[SegmentEndNotReached];
                Func<Point, Point, bool> shouldRemoveDuplicate = (Func<Point, Point, bool>)valueExtractor[ShouldRemoveDuplicate];

                int segmentStart = dataPointBoundsGetter(this.renderPoints[currentSegment.StartIndex], context.IsPlotInverse);
                int segmentEnd = dataPointBoundsGetter(this.renderPoints[currentSegment.EndIndex], context.IsPlotInverse);
                IList<Point> stackedPoints = context.PreviousStackedPoints;
                Point currentPoint;
                int currentPointBounds;

                do
                {
                    currentPoint = stackedPoints[context.PreviousStackedPointsCurrentIndex];
                    currentPointBounds = pointBoundsGetter(currentPoint);

                    context.PreviousStackedPointsCurrentIndex++;
                    if (shouldSkipPreviousPoints(currentPointBounds, segmentStart, context.IsPlotInverse))
                    {
                        continue;
                    }

                    points.Add(currentPoint);
                }
                while (segmentEndNotReached(currentPointBounds, segmentEnd, context.IsPlotInverse) && context.PreviousStackedPointsCurrentIndex < stackedPoints.Count);

                context.PreviousStackedPointsCurrentIndex--;

                if (shouldRemoveDuplicate(points[0], points[1]))
                {
                    points.RemoveAt(0);
                }
            }

            return points;
        }
    }
}