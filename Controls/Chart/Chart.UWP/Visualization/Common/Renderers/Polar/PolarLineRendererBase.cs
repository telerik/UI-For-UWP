using System.Collections.Generic;
using Telerik.Charting;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal abstract class PolarLineRendererBase : LineRenderer
    {
        internal bool autoSortPoints = true;
        internal bool isClosed = true;

        internal abstract IComparer<DataPoint> Comparer { get; }

        public override void ApplyPalette()
        {
            base.ApplyPalette();

            IFilledSeries filledSeries = this.model.presenter as IFilledSeries;
            if (filledSeries == null || filledSeries.IsFillSetLocally)
            {
                return;
            }

            Brush paletteBrush = this.GetPaletteBrush(PaletteVisualPart.Fill);
            if (paletteBrush != null)
            {
                this.strokeShape.Fill = paletteBrush;
            }
            else
            {
                this.strokeShape.Fill = filledSeries.Fill;
            }
        }

        protected override void RenderCore()
        {
            base.RenderCore();

            // we need at least two points to calculate the line
            if (this.isClosed && this.renderPoints.Count >= 2)
            {
                this.ConnectFirstLastDataPoints();
            }
        }

        protected virtual void ConnectFirstLastDataPoints()
        {
            DataPoint firstPoint = this.renderPoints[0];
            DataPoint lastPoint = this.renderPoints[this.renderPoints.Count - 1];

            if (firstPoint.isEmpty || lastPoint.isEmpty)
            {
                return;
            }

            PathFigure figure = new PathFigure();
            figure.StartPoint = lastPoint.Center();
            PolyLineSegment lineSegment = new PolyLineSegment();
            lineSegment.Points.Add(firstPoint.Center());

            figure.Segments.Add(lineSegment);
            this.shapeGeometry.Figures.Add(figure);
        }

        protected override IList<DataPoint> GetRenderPoints()
        {
            IList<DataPoint> points;
            if (this.autoSortPoints)
            {
                List<DataPoint> sortedPoints = new List<DataPoint>(this.model.DataPointsInternal.Count);
                foreach (DataPoint point in this.model.DataPointsInternal)
                {
                    sortedPoints.Add(point);
                }

                sortedPoints.Sort(this.Comparer);
                points = sortedPoints;
            }
            else
            {
                points = this.model.DataPointsInternal;
            }

            return points;
        }
    }
}
