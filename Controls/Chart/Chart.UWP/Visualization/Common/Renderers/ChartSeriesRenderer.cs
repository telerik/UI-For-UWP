using System.Collections.Generic;
using Telerik.Charting;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal abstract class ChartSeriesRenderer
    {
        internal ChartSeriesModel model;
        internal IList<DataPoint> renderPoints;
        private const double PlotAreaVicinity = 5.0;

        public void Render(bool isDrawnWithComposition = false)
        {
            this.Reset();

            this.renderPoints = this.GetRenderPoints();
            if (this.renderPoints.Count == 0)
            {
                return;
            }

            if (!isDrawnWithComposition)
            {
                this.RenderCore();
            }
        }

        public virtual void ApplyPalette()
        {
        }

        public virtual void ApplyContainerVisualPalette(ContainerVisual containerVisual, ContainerVisualsFactory factory)
        {
        }

        protected internal static IEnumerable<DataPointSegment> GetDataSegments(IList<DataPoint> dataPoints)
        {
            DataPointSegment dataSegment = null;

            int pointIndex = 0;
            foreach (DataPoint point in dataPoints)
            {
                if (point.isEmpty || double.IsNaN(point.CenterX()) || double.IsNaN(point.CenterY()))
                {
                    if (dataSegment != null)
                    {
                        dataSegment.EndIndex = pointIndex - 1;

                        // segment is defined for at least two points
                        if (dataSegment.StartIndex != dataSegment.EndIndex)
                        {
                            yield return dataSegment;
                        }

                        dataSegment = null;
                    }
                }
                else
                {
                    if (dataSegment == null)
                    {
                        dataSegment = new DataPointSegment();
                        dataSegment.StartIndex = pointIndex;
                    }
                }

                pointIndex++;
            }

            if (dataSegment != null)
            {
                dataSegment.EndIndex = dataPoints.Count - 1;

                // segment is defined for at least two points
                if (dataSegment.StartIndex != dataSegment.EndIndex)
                {
                    yield return dataSegment;
                }
            }
        }
        protected virtual IList<DataPoint> GetRenderPoints()
        {
            if (this.model.renderablePoints.Count > 0)
            {
                return this.GetRenderPoints(this.model.renderablePoints);
            }

            return this.GetRenderPoints(this.model.DataPointsInternal);
        }

        protected Brush GetPaletteBrush(PaletteVisualPart part)
        {
            ChartSeries series = this.model.presenter as ChartSeries;
            if (series == null)
            {
                return null;
            }

            return series.chart.GetPaletteBrush(series.ActualPaletteIndex, part, series.Family, series.IsSelected);
        }

        protected abstract void Reset();

        protected abstract void RenderCore();

        private List<DataPoint> GetRenderPoints(IList<DataPoint> dataPoints)
        {
            var clipRect = this.model.layoutSlot;
            var plotDirection = this.model.GetTypedValue<AxisPlotDirection>(AxisModel.PlotDirectionPropertyKey, AxisPlotDirection.Vertical);
            bool isVertical = plotDirection == AxisPlotDirection.Vertical;
            double start = isVertical ? clipRect.X - PlotAreaVicinity : clipRect.Y - PlotAreaVicinity;
            double end = isVertical ? clipRect.Right + PlotAreaVicinity : clipRect.Bottom + PlotAreaVicinity;

            bool addedPreviousPoint = false;
            Point? previousPoint = null;
            DataPoint previousDataPoint = null;
            bool isPreviousPointInView = false;

            var renderPoints = new List<DataPoint>();
            for (int i = 0; i < dataPoints.Count; i++)
            {
                var dataPoint = dataPoints[i];
                var point = dataPoint.Center();
                if (dataPoints[i].isEmpty || double.IsNaN(point.X) || double.IsNaN(point.Y))
                {
                    // Point is empty
                    addedPreviousPoint = false;
                    previousPoint = null;
                    previousDataPoint = null;
                    isPreviousPointInView = false;
                    continue;
                }

                double position = isVertical ? point.X : point.Y;
                if (start <= position && position <= end)
                {
                    // Point is inside the viewport
                    if (!addedPreviousPoint && previousPoint.HasValue)
                    {
                        renderPoints.Add(previousDataPoint);
                    }

                    renderPoints.Add(dataPoint);
                    addedPreviousPoint = true;
                    isPreviousPointInView = true;
                }
                else
                {
                    if (isPreviousPointInView)
                    {
                        // Point is not inside the viewport, but the previous point is
                        renderPoints.Add(dataPoint);
                        addedPreviousPoint = true;
                    }
                    else
                    {
                        // Both the current point and the previous point are not in the viewport
                        bool lineIntersectsClipRect = false;
                        if (previousPoint.HasValue)
                        {
                            double prevPosition = isVertical ? previousPoint.Value.X : previousPoint.Value.Y;
                            position = isVertical ? point.X : point.Y;
                            lineIntersectsClipRect = (prevPosition <= start && end <= position) || (position <= start && end <= prevPosition);
                        }

                        if (!addedPreviousPoint && lineIntersectsClipRect)
                        {
                            renderPoints.Add(previousDataPoint);
                        }

                        if (lineIntersectsClipRect)
                        {
                            renderPoints.Add(dataPoint);
                            addedPreviousPoint = true;
                        }
                        else
                        {
                            addedPreviousPoint = false;
                        }
                    }

                    isPreviousPointInView = false;
                }

                previousPoint = point;
                previousDataPoint = dataPoint;
            }

            return renderPoints;
        }
    }
}