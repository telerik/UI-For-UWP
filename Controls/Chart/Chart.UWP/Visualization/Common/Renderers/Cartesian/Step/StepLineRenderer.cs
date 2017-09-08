using System;
using System.Collections.Generic;
using Telerik.Charting;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class StepLineRenderer : LineRenderer
    {
        private static ReferenceDictionary<string, Delegate> verticalPlotValueExtractors;
        private static ReferenceDictionary<string, Delegate> horizontalPlotValueExtractors;

        public static ReferenceDictionary<string, Delegate> VerticalPlotValueExtractors
        {
            get
            {
                if (verticalPlotValueExtractors == null)
                {
                    verticalPlotValueExtractors = new ReferenceDictionary<string, Delegate>();

                    verticalPlotValueExtractors.Set(StepSeriesHelper.RiserPointGetter, (Func<Point, Point, Point>)((currentPoint, nextPoint) => new Point(nextPoint.X, currentPoint.Y)));
                    verticalPlotValueExtractors.Set(StepSeriesHelper.FirstRiserPointGetter, (Func<Point, Point, double, Point>)((currentPoint, nextPoint, halfSlotLength) => new Point(currentPoint.X + halfSlotLength, currentPoint.Y)));
                    verticalPlotValueExtractors.Set(StepSeriesHelper.SecondRiserPointGetter, (Func<Point, Point, double, Point>)((currentPoint, nextPoint, halfSlotLength) => new Point(currentPoint.X + halfSlotLength, nextPoint.Y)));
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

                    horizontalPlotValueExtractors.Set(StepSeriesHelper.RiserPointGetter, (Func<Point, Point, Point>)((currentPoint, nextPoint) => new Point(currentPoint.X, nextPoint.Y)));
                    horizontalPlotValueExtractors.Set(StepSeriesHelper.FirstRiserPointGetter, (Func<Point, Point, double, Point>)((currentPoint, nextPoint, halfSlotLength) => new Point(currentPoint.X, currentPoint.Y - halfSlotLength)));
                    horizontalPlotValueExtractors.Set(StepSeriesHelper.SecondRiserPointGetter, (Func<Point, Point, double, Point>)((currentPoint, nextPoint, halfSlotLength) => new Point(nextPoint.X, currentPoint.Y - halfSlotLength)));
                }

                return horizontalPlotValueExtractors;
            }
        }

        protected internal override IEnumerable<Point> GetPoints(DataPointSegment segment)
        {
            AxisPlotDirection plotDirection = this.model.GetTypedValue<AxisPlotDirection>(AxisModel.PlotDirectionPropertyKey, AxisPlotDirection.Vertical);
            ReferenceDictionary<string, Delegate> valueExtractor = plotDirection == AxisPlotDirection.Vertical ? VerticalPlotValueExtractors : HorizontalPlotValueExtractors;

            foreach (Point point in StepSeriesHelper.GetPoints(segment, this.model as StepSeriesModel, this.renderPoints, valueExtractor))
            {
                yield return point;
            }
        }
    }
}
