using System.Collections.Generic;
using System.Linq;
using Telerik.Charting;
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
                return this.model.renderablePoints.Where(this.ShouldPlotPoint).ToList();
            }

            return this.model.DataPointsInternal.Where(this.ShouldPlotPoint).ToList();
        }

        protected virtual bool ShouldPlotPoint(DataPoint point)
        {
            var pointBounds = point.layoutSlot;
            var plotBounds = this.model.layoutSlot;

            var isWithinXPlot = pointBounds.X <= plotBounds.X + plotBounds.Width && pointBounds.X + pointBounds.Width >= plotBounds.X;
            var isWithinYPlot = pointBounds.Y <= plotBounds.Y + plotBounds.Height && pointBounds.Y + pointBounds.Height >= plotBounds.Y;

            if (isWithinXPlot && isWithinYPlot)
            {
                return true;
            }

            var plotDirection = this.model.GetTypedValue<AxisPlotDirection>(AxisModel.PlotDirectionPropertyKey, AxisPlotDirection.Vertical);

            // empty values
            return (isWithinXPlot && double.IsNaN(point.layoutSlot.Y) && plotDirection == AxisPlotDirection.Vertical) ||
                   (isWithinYPlot && double.IsNaN(point.layoutSlot.X) && plotDirection == AxisPlotDirection.Horizontal);
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
    }
}