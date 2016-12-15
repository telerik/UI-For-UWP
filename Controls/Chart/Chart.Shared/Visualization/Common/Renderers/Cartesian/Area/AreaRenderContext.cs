using System.Collections.Generic;
using Telerik.Charting;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class AreaRenderContext
    {
        public double StrokeThickness;
        public double StrokeThicknessOffset;
        public RadRect PlotArea;
        public double PlotLine;
        public AxisPlotDirection PlotDirection;
        public bool IsPlotInverse;

        public bool IsStacked;
        public IList<Point> PreviousStackedPoints;

        public PathFigure AreaFigure;
        public Point FirstBottomPoint, LastBottomPoint, FirstTopPoint, LastTopPoint;
        public bool IsFirstBottomPointSet, IsFirstTopPointSet;

        public DataPointSegment CurrentSegment;
        public int PreviousSegmentEndIndex;
        public int PreviousStackedPointsCurrentIndex;

        public AreaRenderContext(AreaRendererBase renderer)
        {
            ChartSeries series = renderer.model.presenter as ChartSeries;

            this.StrokeThickness = renderer.strokeShape.Stroke == null ? 0 : renderer.strokeShape.StrokeThickness;
            this.StrokeThicknessOffset = (int)(this.StrokeThickness / 2);

            this.PreviousStackedPoints = series.chart.StackedSeriesContext.PreviousStackedArea;
            this.IsStacked = this.PreviousStackedPoints != null && this.PreviousStackedPoints.Count > 0;

            this.PlotArea = renderer.model.GetChartArea().PlotArea.layoutSlot;
            this.PlotArea.Width *= series.chart.zoomCache.Width;
            this.PlotArea.Height *= series.chart.zoomCache.Height;

            // calculate the plot line - consider plot origin
            double plotOrigin = renderer.model.GetTypedValue<double>(AxisModel.PlotOriginPropertyKey, 0d);

            this.PlotDirection = renderer.model.GetTypedValue<AxisPlotDirection>(AxisModel.PlotDirectionPropertyKey, AxisPlotDirection.Vertical);

            if (this.PlotDirection == AxisPlotDirection.Vertical)
            {
                this.PlotLine = this.PlotArea.Bottom - (int)((plotOrigin * this.PlotArea.Height) + 0.5);
            }
            else
            {
                this.PlotLine = this.PlotArea.X + (int)((plotOrigin * this.PlotArea.Width) + 0.5);
            }

            // if PlotDirection is Verticel, check the IsInverse property for the Horizontal axis and vice versa
            this.IsPlotInverse = renderer.model.GetIsPlotInverse(this.PlotDirection ^ AxisPlotDirection.Horizontal);
        }
    }
}