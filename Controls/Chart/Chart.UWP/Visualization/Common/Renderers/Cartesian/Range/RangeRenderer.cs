using System;
using System.Collections.Generic;
using Telerik.Charting;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class RangeRenderer : AreaRendererBase
    {
        internal const string TopPointGetter = "TopPointGetter";
        internal const string BottomPointGetter = "BottomPointGetter";

        internal RangeSeriesStrokeMode strokeMode;

        private static ReferenceDictionary<string, Delegate> verticalRangePlotValueExtractors;
        private static ReferenceDictionary<string, Delegate> horizontalRangePlotValueExtractors;

        public RangeRenderer()
        {
            this.strokeMode = RangeSeriesStrokeMode.LowAndHighPoints;
        }

        public static ReferenceDictionary<string, Delegate> VerticalRangePlotValueExtractors
        {
            get
            {
                if (verticalRangePlotValueExtractors == null)
                {
                    verticalRangePlotValueExtractors = new ReferenceDictionary<string, Delegate>();

                    verticalRangePlotValueExtractors.Set(TopPointGetter, (Func<DataPoint, Point>)((currentPoint) => new Point(currentPoint.LayoutSlot.X + currentPoint.LayoutSlot.Width / 2, currentPoint.LayoutSlot.Y)));
                    verticalRangePlotValueExtractors.Set(BottomPointGetter, (Func<DataPoint, Point>)((currentPoint) => new Point(currentPoint.LayoutSlot.X + currentPoint.LayoutSlot.Width / 2, currentPoint.LayoutSlot.Bottom)));
                }

                return verticalRangePlotValueExtractors;
            }
        }

        public static ReferenceDictionary<string, Delegate> HorizontalRangePlotValueExtractors
        {
            get
            {
                if (horizontalRangePlotValueExtractors == null)
                {
                    horizontalRangePlotValueExtractors = new ReferenceDictionary<string, Delegate>();

                    horizontalRangePlotValueExtractors.Set(TopPointGetter, (Func<DataPoint, Point>)((currentPoint) => new Point(currentPoint.LayoutSlot.Right, currentPoint.LayoutSlot.Y + currentPoint.LayoutSlot.Height / 2)));
                    horizontalRangePlotValueExtractors.Set(BottomPointGetter, (Func<DataPoint, Point>)((currentPoint) => new Point(currentPoint.LayoutSlot.X, currentPoint.LayoutSlot.Y + currentPoint.LayoutSlot.Height / 2)));
                }

                return horizontalRangePlotValueExtractors;
            }
        } 

        protected override bool ShouldAddTopPointsToStroke
        {
            get { return (this.strokeMode & RangeSeriesStrokeMode.HighPoints) == RangeSeriesStrokeMode.HighPoints; }
        }

        protected override bool ShouldAddBottomPointsToStroke
        {
            get { return (this.strokeMode & RangeSeriesStrokeMode.LowPoints) == RangeSeriesStrokeMode.LowPoints; }
        }

        protected override IEnumerable<Windows.Foundation.Point> GetTopPoints(DataPointSegment segment)
        {
            AxisPlotDirection plotDirection = this.model.GetTypedValue<AxisPlotDirection>(AxisModel.PlotDirectionPropertyKey, AxisPlotDirection.Vertical);
            ReferenceDictionary<string, Delegate> valueExtractor = plotDirection == AxisPlotDirection.Vertical ? VerticalRangePlotValueExtractors : HorizontalRangePlotValueExtractors;
            Func<DataPoint, Point> topPointGetter = (Func<DataPoint, Point>)valueExtractor[TopPointGetter];

            int pointIndex = segment.StartIndex;
            while (pointIndex <= segment.EndIndex)
            {
                var currentPoint = this.renderPoints[pointIndex];
                yield return topPointGetter(currentPoint);

                pointIndex++;
            }
        }

        protected override IList<Point> GetBottomPoints(AreaRenderContext context)
        {
            AxisPlotDirection plotDirection = this.model.GetTypedValue<AxisPlotDirection>(AxisModel.PlotDirectionPropertyKey, AxisPlotDirection.Vertical);
            ReferenceDictionary<string, Delegate> valueExtractor = plotDirection == AxisPlotDirection.Vertical ? VerticalRangePlotValueExtractors : HorizontalRangePlotValueExtractors;
            Func<DataPoint, Point> bottomPointGetter = (Func<DataPoint, Point>)valueExtractor[BottomPointGetter];

            DataPointSegment currentSegment = context.CurrentSegment;
            List<Point> points = new List<Point>();
            int pointIndex = currentSegment.StartIndex;
            while (pointIndex <= currentSegment.EndIndex)
            {
                var currentPoint = this.renderPoints[pointIndex];
                points.Add(bottomPointGetter(currentPoint));
                pointIndex++;
            }

            return points;
        }
    }
}
