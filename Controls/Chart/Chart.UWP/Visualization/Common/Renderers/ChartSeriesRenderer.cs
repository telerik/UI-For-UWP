using System.Collections.Generic;
using Telerik.Charting;
using Telerik.Core;
using Windows.UI.Composition;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal abstract class ChartSeriesRenderer
    {
        internal ChartSeriesModel model;
        internal IList<DataPoint> renderPoints;

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

        protected virtual bool ShouldPlotPoint(RadRect point)
        {
            var plotBounds = this.model.layoutSlot;

            var isWithinXPlot = point.X <= plotBounds.X + plotBounds.Width && point.X + point.Width >= plotBounds.X;
            var isWithinYPlot = point.Y <= plotBounds.Y + plotBounds.Height && point.Y + point.Height >= plotBounds.Y;

            if (isWithinXPlot && isWithinYPlot)
            {
                return true;
            }

            var plotDirection = this.model.GetTypedValue<AxisPlotDirection>(AxisModel.PlotDirectionPropertyKey, AxisPlotDirection.Vertical);
            
            // empty values
            return (isWithinXPlot && double.IsNaN(point.Y) && plotDirection == AxisPlotDirection.Vertical) ||
                   (isWithinYPlot && double.IsNaN(point.X) && plotDirection == AxisPlotDirection.Horizontal);
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

        private static bool LineIntersectsRect(RadPoint firstPoint, RadPoint secondPoint, RadRect rect)
        {
            return LineIntersectsLine(firstPoint, secondPoint, new RadPoint(rect.X, rect.Y), new RadPoint(rect.X + rect.Width, rect.Y)) ||
                LineIntersectsLine(firstPoint, secondPoint, new RadPoint(rect.X + rect.Width, rect.Y), new RadPoint(rect.X + rect.Width, rect.Y + rect.Height)) ||
                LineIntersectsLine(firstPoint, secondPoint, new RadPoint(rect.X + rect.Width, rect.Y + rect.Height), new RadPoint(rect.X, rect.Y + rect.Height)) ||
                LineIntersectsLine(firstPoint, secondPoint, new RadPoint(rect.X, rect.Y + rect.Height), new RadPoint(rect.X, rect.Y)) ||
                (rect.Contains(firstPoint.X, firstPoint.Y) && rect.Contains(secondPoint.X, secondPoint.Y));
        }

        private static bool LineIntersectsLine(RadPoint firstLineFirstPoint, RadPoint firstLineSecondPoint, RadPoint secondLineFirstPoint, RadPoint secondLineSecondPoint)
        {
            var d = (firstLineSecondPoint.X - firstLineFirstPoint.X) * (secondLineSecondPoint.Y - secondLineFirstPoint.Y) - (firstLineSecondPoint.Y - firstLineFirstPoint.Y) * (secondLineSecondPoint.X - secondLineFirstPoint.X);
            if (d == 0)
            {
                return false;
            }

            var q = (firstLineFirstPoint.Y - secondLineFirstPoint.Y) * (secondLineSecondPoint.X - secondLineFirstPoint.X) - (firstLineFirstPoint.X - secondLineFirstPoint.X) * (secondLineSecondPoint.Y - secondLineFirstPoint.Y);
            var r = q / d;

            q = (firstLineFirstPoint.Y - secondLineFirstPoint.Y) * (firstLineSecondPoint.X - firstLineFirstPoint.X) - (firstLineFirstPoint.X - secondLineFirstPoint.X) * (firstLineSecondPoint.Y - firstLineFirstPoint.Y);
            var s = q / d;
            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            return true;
        }

        private List<DataPoint> GetRenderPoints(IList<DataPoint> points)
        {
            var renderPoints = new List<DataPoint>();
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                var pointBounds = RadRect.Round(point.layoutSlot);
                if (this.ShouldPlotPoint(pointBounds))
                {
                    renderPoints.Add(point);
                    continue;
                }

                RadRect nextPointBounds = RadRect.Empty;
                if (i + 1 < points.Count)
                {
                    nextPointBounds = RadRect.Round(points[i + 1].layoutSlot);
                }
                else if (i - 1 > -1)
                {
                    nextPointBounds = RadRect.Round(points[i - 1].layoutSlot);
                }

                if (LineIntersectsRect(pointBounds.Location, nextPointBounds.Location, this.model.layoutSlot))
                {
                    renderPoints.Add(points[i]);
                }
            }

            return renderPoints;
        }
    }
}