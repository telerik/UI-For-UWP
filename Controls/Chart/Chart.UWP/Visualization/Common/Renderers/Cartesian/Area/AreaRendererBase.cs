using System;
using System.Collections.Generic;
using Telerik.Charting;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal abstract class AreaRendererBase : LineRenderer
    { 
        internal const string PointGetter = "PointGetter";
        internal const string PointBoundsGetter = "PointBoundsGetter";
        internal const string DataPointBoundsGetter = "DataPointBoundsGetter";
        internal const string ShouldSkipPreviousPoints = "ShouldSkipPreviousPoints";
        internal const string SegmentEndNotReached = "SegmentEndNotReached";
        internal const string ShouldRemoveDuplicate = "ShouldRemoveDuplicate";
        internal const string ShouldRemoveDuplicate2 = "ShouldRemoveDuplicate2";

        internal List<Point> topSurfacePoints;
        internal Path areaShape;
        internal PathGeometry areaShapeGeometry;

        private static ReferenceDictionary<string, Delegate> verticalPlotValueExtractors;
        private static ReferenceDictionary<string, Delegate> horizontalPlotValueExtractors;

        public AreaRendererBase()
        {
            this.areaShape = new Path();
            this.areaShapeGeometry = new PathGeometry();
            this.areaShape.Data = this.areaShapeGeometry;
            this.topSurfacePoints = new List<Point>();
            this.strokeShape.StrokeEndLineCap = PenLineCap.Triangle;
            this.strokeShape.StrokeStartLineCap = PenLineCap.Triangle;
        }

        public static ReferenceDictionary<string, Delegate> VerticalPlotValueExtractors
        {
            get
            {
                if (verticalPlotValueExtractors == null)
                {
                    verticalPlotValueExtractors = new ReferenceDictionary<string, Delegate>();

                    verticalPlotValueExtractors.Set(PointGetter, (Func<DataPoint, double, Point>)((dataPoint, plotLine) => new Point(dataPoint.CenterX(), plotLine)));
                    verticalPlotValueExtractors.Set(PointBoundsGetter, (Func<Point, int>)(point => (int)(point.X - .5)));
                    verticalPlotValueExtractors.Set(DataPointBoundsGetter, (Func<DataPoint, bool, int>)((dataPoint, isPlotInverse) => (int)(dataPoint.CenterX() + (isPlotInverse ? 0.5 : -0.5))));
                    verticalPlotValueExtractors.Set(ShouldSkipPreviousPoints, (Func<int, int, bool, bool>)((value1, value2, isPlotInverse) => (value1 < value2) ^ isPlotInverse));
                    verticalPlotValueExtractors.Set(SegmentEndNotReached, (Func<int, int, bool, bool>)((value1, value2, isPlotInverse) => (value1 < value2) ^ isPlotInverse));
                    verticalPlotValueExtractors.Set(ShouldRemoveDuplicate, (Func<Point, Point, bool>)((point1, point2) => point1.X == point2.X));
                    verticalPlotValueExtractors.Set(ShouldRemoveDuplicate2, (Func<Point, Point, Point, bool>)((point1, point2, point3) => point1.X == point2.X && point2.X == point3.X));
                }

                return verticalPlotValueExtractors;
            }
        }

        public static ReferenceDictionary<string, Delegate> HorizontalPlotValueExtractors
        {
            get
            {
                if (horizontalPlotValueExtractors == null)
                {
                    horizontalPlotValueExtractors = new ReferenceDictionary<string, Delegate>();

                    horizontalPlotValueExtractors.Set(PointGetter, (Func<DataPoint, double, Point>)((dataPoint, plotLine) => new Point(plotLine, dataPoint.CenterY())));
                    horizontalPlotValueExtractors.Set(PointBoundsGetter, (Func<Point, int>)(point => (int)(point.Y + .5)));
                    horizontalPlotValueExtractors.Set(DataPointBoundsGetter, (Func<DataPoint, bool, int>)((dataPoint, isPlotInverse) => (int)(dataPoint.CenterY() + (isPlotInverse ? -.5 : .5))));
                    horizontalPlotValueExtractors.Set(ShouldSkipPreviousPoints, (Func<int, int, bool, bool>)((value1, value2, isPlotInverse) => (value1 > value2) ^ isPlotInverse));
                    horizontalPlotValueExtractors.Set(SegmentEndNotReached, (Func<int, int, bool, bool>)((value1, value2, isPlotInverse) => (value1 > value2) ^ isPlotInverse));
                    horizontalPlotValueExtractors.Set(ShouldRemoveDuplicate, (Func<Point, Point, bool>)((point1, point2) => point1.Y == point2.Y));
                    horizontalPlotValueExtractors.Set(ShouldRemoveDuplicate2, (Func<Point, Point, Point, bool>)((point1, point2, point3) => point1.Y == point2.Y && point2.Y == point3.Y));
                }

                return horizontalPlotValueExtractors;
            }
        }

        protected virtual bool ShouldAddBottomPointsToStroke
        {
            get
            {
                return false;
            }
        }

        protected virtual bool ShouldAddTopPointsToStroke
        {
            get
            {
                return false;
            }
        }

        protected virtual bool ShouldAddRightPointsToStroke
        {
            get
            {
                return false;
            }
        }

        protected virtual bool ShouldAddLeftPointsToStroke
        {
            get
            {
                return false;
            }
        }

        protected override PaletteVisualPart StrokePart
        {
            get
            {
                // the area has a fill, which is the primary part
                return PaletteVisualPart.Stroke;
            }
        }

        public override void ApplyPalette()
        {
            base.ApplyPalette();

            IFilledSeries filledSeries = this.model.presenter as IFilledSeries;
            if (filledSeries == null || filledSeries.IsFillSetLocally)
            {
                return;
            }

            Brush paletteFill = this.GetPaletteBrush(PaletteVisualPart.Fill);
            if (paletteFill != null)
            {
                this.areaShape.Fill = paletteFill;
            }
            else
            {
                this.areaShape.Fill = filledSeries.Fill;
            }
        }

        protected override void Reset()
        {
            base.Reset();

            this.topSurfacePoints.Clear();
            this.areaShapeGeometry.Figures.Clear();
        }

        protected override void RenderCore()
        {
            // we need at least two points to calculate the line
            if (this.renderPoints.Count < 2)
            {
                return;
            }

            AreaRenderContext context = new AreaRenderContext(this);

            foreach (DataPointSegment dataSegment in ChartSeriesRenderer.GetDataSegments(this.renderPoints))
            {
                context.CurrentSegment = dataSegment;
                context.AreaFigure = new PathFigure();

                this.AddTopPoints(context);
                this.AddBottomPoints(context);

                context.AreaFigure.IsClosed = true;
                this.areaShapeGeometry.Figures.Add(context.AreaFigure);

                context.PreviousSegmentEndIndex = dataSegment.EndIndex;
            }

            // Fill in top points
            this.FillEmptyPointsToTopSurface(context, this.renderPoints.Count - 1);

            if (!this.renderPoints[this.renderPoints.Count - 1].isEmpty)
            {
                this.AddRightStrokeLine(context);
            }

            if (!this.renderPoints[0].isEmpty)
            {
                this.AddLeftStrokeLine(context);
            }
        }

        protected abstract IEnumerable<Point> GetTopPoints(DataPointSegment segment);

        protected abstract IList<Point> GetBottomPoints(AreaRenderContext context);     

        private void AddTopPoints(AreaRenderContext context)
        {
            PathFigure strokeFigure = null;
            PolyLineSegment strokeLineSegment = null;
            PolyLineSegment areaSegment = null;
            bool addTopPointsToStroke = this.ShouldAddTopPointsToStroke;

            this.FillEmptyPointsToTopSurface(context, context.CurrentSegment.StartIndex);

            foreach (Point point in this.GetTopPoints(context.CurrentSegment))
            {
                this.topSurfacePoints.Add(point);
                context.LastTopPoint = point;

                // pass for first point
                if (areaSegment == null)
                {
                    areaSegment = new PolyLineSegment();
                    context.AreaFigure.StartPoint = point;

                    if (!context.IsFirstTopPointSet)
                    {
                        context.FirstTopPoint = point;
                        context.IsFirstTopPointSet = true;
                    }

                    if (addTopPointsToStroke)
                    {
                        strokeFigure = new PathFigure();
                        strokeFigure.IsFilled = false;
                        strokeFigure.StartPoint = point;
                        strokeLineSegment = new PolyLineSegment();
                    }

                    continue;
                }

                areaSegment.Points.Add(point);

                if (addTopPointsToStroke)
                {
                    strokeLineSegment.Points.Add(point);
                }
            }

            context.AreaFigure.Segments.Add(areaSegment);

            if (addTopPointsToStroke)
            {
                strokeFigure.Segments.Add(strokeLineSegment);
                this.shapeGeometry.Figures.Add(strokeFigure);
            }
        }

        private void AddBottomPoints(AreaRenderContext context)
        {
            IList<Point> bottomPoints = this.GetBottomPoints(context);
            if (!context.IsFirstBottomPointSet)
            {
                context.FirstBottomPoint = bottomPoints[0];
                context.IsFirstBottomPointSet = true;
            }

            context.LastBottomPoint = bottomPoints[bottomPoints.Count - 1];

            // Add the bottom points in reverse order
            PathFigure strokeFigure = null;
            PolyLineSegment strokeLineSegment = null;
            bool addBottomPointsToStroke = this.ShouldAddBottomPointsToStroke;
            if (addBottomPointsToStroke)
            {
                strokeFigure = new PathFigure();
                strokeFigure.IsFilled = false;
                strokeFigure.StartPoint = bottomPoints[bottomPoints.Count - 1];
                strokeLineSegment = new PolyLineSegment();
            }

            PolyLineSegment areaSegment = new PolyLineSegment();
            for (int i = bottomPoints.Count - 1; i >= 0; i--)
            {
                Point point = bottomPoints[i];
                areaSegment.Points.Add(point);
                if (addBottomPointsToStroke && i < bottomPoints.Count - 1)
                {
                    // Skip last point since we already added it
                    strokeLineSegment.Points.Add(point);
                }
            }

            context.AreaFigure.Segments.Add(areaSegment);

            if (addBottomPointsToStroke)
            {
                strokeFigure.Segments.Add(strokeLineSegment);
                this.shapeGeometry.Figures.Add(strokeFigure);
            }
        }

        private void FillEmptyPointsToTopSurface(AreaRenderContext context, int lastDataPointIndex)
        {
            ReferenceDictionary<string, Delegate> valueExtractor;
            if (context.PlotDirection == AxisPlotDirection.Vertical)
            {
                valueExtractor = VerticalPlotValueExtractors;
            }
            else
            {
                valueExtractor = HorizontalPlotValueExtractors;
            }

            Func<DataPoint, bool, int> dataPointBoundsGetter = (Func<DataPoint, bool, int>)valueExtractor[DataPointBoundsGetter];

            int lastPointBounds = dataPointBoundsGetter(this.renderPoints[lastDataPointIndex], context.IsPlotInverse);
            int previousSegmentEnd = dataPointBoundsGetter(this.renderPoints[context.PreviousSegmentEndIndex], context.IsPlotInverse);

            if (context.PreviousSegmentEndIndex == lastDataPointIndex || previousSegmentEnd == lastPointBounds)
            {
                return;
            }

            if (!context.IsStacked)
            {
                Func<DataPoint, double, Point> pointGetter = (Func<DataPoint, double, Point>)valueExtractor[PointGetter];

                for (int i = context.PreviousSegmentEndIndex; i <= lastDataPointIndex; i++)
                {
                    this.topSurfacePoints.Add(pointGetter(this.renderPoints[i], context.PlotLine));
                }
            }
            else
            {
                Func<Point, int> pointBoundsGetter = (Func<Point, int>)valueExtractor[PointBoundsGetter];
                Func<int, int, bool, bool> shouldSkipPreviousPoints = (Func<int, int, bool, bool>)valueExtractor[ShouldSkipPreviousPoints];
                Func<int, int, bool, bool> segmentEndNotReached = (Func<int, int, bool, bool>)valueExtractor[SegmentEndNotReached];
                Func<Point, Point, Point, bool> shouldRemoveDuplicate = (Func<Point, Point, Point, bool>)valueExtractor[ShouldRemoveDuplicate2];

                int lastTopSurfacePointsCount = this.topSurfacePoints.Count;
                IList<Point> stackedPoints = context.PreviousStackedPoints;
                Point currentPoint;
                int currentPointBounds;

                do
                {
                    currentPoint = stackedPoints[context.PreviousStackedPointsCurrentIndex];
                    currentPointBounds = pointBoundsGetter(currentPoint);

                    context.PreviousStackedPointsCurrentIndex++;

                    if (shouldSkipPreviousPoints(currentPointBounds, previousSegmentEnd, context.IsPlotInverse))
                    {
                        continue;
                    }

                    this.topSurfacePoints.Add(currentPoint);
                }
                while (segmentEndNotReached(currentPointBounds, lastPointBounds, context.IsPlotInverse) && context.PreviousStackedPointsCurrentIndex < stackedPoints.Count);

                context.PreviousStackedPointsCurrentIndex--;

                if (lastTopSurfacePointsCount > 0 &&
                    shouldRemoveDuplicate(this.topSurfacePoints[lastTopSurfacePointsCount - 1], this.topSurfacePoints[lastTopSurfacePointsCount], this.topSurfacePoints[lastTopSurfacePointsCount + 1]))
                {
                    this.topSurfacePoints.RemoveAt(lastTopSurfacePointsCount);
                }
            }
        }

        private void AddLeftStrokeLine(AreaRenderContext context)
        {
            if (this.ShouldAddLeftPointsToStroke)
            {
                PathFigure strokeFigure = new PathFigure();
                strokeFigure.IsFilled = false;
                strokeFigure.StartPoint = context.FirstTopPoint;
                PolyLineSegment strokeLineSegment = new PolyLineSegment();
                strokeLineSegment.Points.Add(context.FirstBottomPoint);
                strokeFigure.Segments.Add(strokeLineSegment);
                this.shapeGeometry.Figures.Add(strokeFigure);
            }
        }

        private void AddRightStrokeLine(AreaRenderContext context)
        {
            if (this.ShouldAddRightPointsToStroke)
            {
                PathFigure strokeFigure = new PathFigure();
                strokeFigure.IsFilled = false;
                strokeFigure.StartPoint = context.LastTopPoint;
                PolyLineSegment strokeLineSegment = new PolyLineSegment();
                strokeLineSegment.Points.Add(context.LastBottomPoint);
                strokeFigure.Segments.Add(strokeLineSegment);
                this.shapeGeometry.Figures.Add(strokeFigure);
            }
        }
    }
}