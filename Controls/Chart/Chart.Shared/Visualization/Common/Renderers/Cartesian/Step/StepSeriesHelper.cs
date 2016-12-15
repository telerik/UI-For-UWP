using System;
using System.Collections.Generic;
using Telerik.Charting;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal static class StepSeriesHelper
    {
        internal const string RiserPointGetter = "RiserPointGetter";
        internal const string FirstRiserPointGetter = "FirstRiserPointGetter";
        internal const string SecondRiserPointGetter = "SecondRiserPointGetter";

        internal static IEnumerable<Point> GetPoints(DataPointSegment segment, StepSeriesModel seriesModel, IList<DataPoint> renderPoints, ReferenceDictionary<string, Delegate> valueExtractor)
        {
            AxisPlotDirection plotDirection = seriesModel.GetTypedValue<AxisPlotDirection>(AxisModel.PlotDirectionPropertyKey, AxisPlotDirection.Vertical);
            AxisModel axisModel = plotDirection == AxisPlotDirection.Vertical ? seriesModel.firstAxis : seriesModel.secondAxis;

            var risersActualPosition = StepSeriesHelper.GetActualRisersPosition(seriesModel.RisersPosition, axisModel.ActualPlotMode);
            var isRisersPositionEqualToPlotMode = StepSeriesHelper.IsRisersPositionEqualToPlotMode(axisModel.ActualPlotMode, risersActualPosition);

            var view = seriesModel.GetChartArea().view;
            Size chartScale = new Size(view.ZoomWidth, view.ZoomHeight);
            double slotLength = StepSeriesHelper.GetSlotLength(seriesModel, plotDirection, chartScale);
            double halfSlotLength = slotLength / 2;

            Func<Point, Point, Point> riserPointGetter = (Func<Point, Point, Point>)valueExtractor[RiserPointGetter];
            Func<Point, Point, double, Point> firstRiserPointGetter = (Func<Point, Point, double, Point>)valueExtractor[FirstRiserPointGetter];
            Func<Point, Point, double, Point> secondRiserPointGetter = (Func<Point, Point, double, Point>)valueExtractor[SecondRiserPointGetter];

            int pointIndex = segment.StartIndex;
            while (pointIndex <= segment.EndIndex)
            {
                var currentPoint = renderPoints[pointIndex].Center();
                yield return currentPoint;

                if (pointIndex == segment.EndIndex)
                {
                    yield break;
                }

                var nextPoint = renderPoints[pointIndex + 1].Center();
                if (isRisersPositionEqualToPlotMode || axisModel is DateTimeContinuousAxisModel)
                {
                    yield return riserPointGetter(currentPoint, nextPoint);
                }
                else
                {
                    yield return firstRiserPointGetter(currentPoint, nextPoint, halfSlotLength);
                    yield return secondRiserPointGetter(currentPoint, nextPoint, halfSlotLength);
                }

                pointIndex++;
            }
        }

        internal static double GetSlotLength(CategoricalStrokedSeriesModel stepLineSeriesModel, AxisPlotDirection plotDirection, Size chartScale)
        {
            double slotLength;
            int slotsCount;
            AxisModel axisModel = plotDirection == AxisPlotDirection.Vertical ? stepLineSeriesModel.firstAxis : stepLineSeriesModel.secondAxis;
            double axisLength = plotDirection == AxisPlotDirection.Vertical ? axisModel.layoutSlot.Width : axisModel.layoutSlot.Height;
            double zoomScale = plotDirection == AxisPlotDirection.Vertical ? chartScale.Width : chartScale.Height;
            axisLength = axisLength * zoomScale;

            DateTimeContinuousAxisModel dateTimeContinuousAxisModel = axisModel as DateTimeContinuousAxisModel;
            if (dateTimeContinuousAxisModel != null)
            {
                return 0;
            }
            CategoricalAxisModel categoricalAxisModel = (CategoricalAxisModel)axisModel;
            slotsCount = categoricalAxisModel.ActualPlotMode == AxisPlotMode.OnTicks ? categoricalAxisModel.categories.Count - 1 : categoricalAxisModel.categories.Count;
            slotsCount = slotsCount * categoricalAxisModel.GetMajorTickInterval();
            slotsCount = Math.Max(1, slotsCount);
            slotLength = axisLength / slotsCount;

            return slotLength;
        }

        internal static StepSeriesRisersPosition GetActualRisersPosition(StepSeriesRisersPosition stepSeriesRisersPosition, AxisPlotMode axisPlotMode)
        {
            StepSeriesRisersPosition risersActualPosition = stepSeriesRisersPosition;
            if (risersActualPosition == StepSeriesRisersPosition.Default)
            {
                risersActualPosition = axisPlotMode == AxisPlotMode.BetweenTicks ? StepSeriesRisersPosition.OnTicks : StepSeriesRisersPosition.BetweenTicks;
            }

            return risersActualPosition;
        }

        internal static bool IsRisersPositionEqualToPlotMode(AxisPlotMode axisActualPlotMode, StepSeriesRisersPosition risersActualPosition)
        {
            return (axisActualPlotMode == AxisPlotMode.BetweenTicks && risersActualPosition == StepSeriesRisersPosition.BetweenTicks)
                   || ((axisActualPlotMode == AxisPlotMode.OnTicksPadded || axisActualPlotMode == AxisPlotMode.OnTicks) && risersActualPosition == StepSeriesRisersPosition.OnTicks);
        }
    }
}
