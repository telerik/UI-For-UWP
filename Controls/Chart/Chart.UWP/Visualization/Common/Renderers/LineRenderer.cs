using System.Collections.Generic;
using System.Linq;
using Telerik.Charting;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class LineRenderer : ChartSeriesRenderer
    {
        internal Path strokeShape;
        internal PathGeometry shapeGeometry;

        public LineRenderer()
        {
            this.strokeShape = new Path();
            this.strokeShape.StrokeThickness = 2;
            this.shapeGeometry = new PathGeometry();
            this.strokeShape.Data = this.shapeGeometry;
        }

        protected virtual PaletteVisualPart StrokePart
        {
            get
            {
                // The default line will have the Fill part as it is the major component for a Line
                return PaletteVisualPart.Fill;
            }
        }

        public override void ApplyPalette()
        {
            base.ApplyPalette();

            IStrokedSeries strokedSeries = this.model.presenter as IStrokedSeries;
            if (strokedSeries == null || strokedSeries.IsStrokeSetLocally)
            {
                return;
            }

            Brush paletteStroke = this.GetPaletteBrush(this.StrokePart);
            if (paletteStroke != null)
            {
                this.strokeShape.Stroke = paletteStroke;
            }
            else
            {
                this.strokeShape.Stroke = strokedSeries.Stroke;
            }
        }

        public override void ApplyContainerVisualPalette(ContainerVisual containerVisual, ContainerVisualsFactory factory)
        {
            base.ApplyContainerVisualPalette(containerVisual, factory);

            IStrokedSeries strokedSeries = this.model.presenter as IStrokedSeries;
            if (strokedSeries == null || strokedSeries.IsStrokeSetLocally)
            {
                return;
            }

            Brush paletteStroke = this.GetPaletteBrush(this.StrokePart);

            for (int i = 0; i < containerVisual.Children.Count; i++)
            {
                var childVisual = containerVisual.Children.ElementAt(i) as SpriteVisual;
                if (childVisual != null)
                {
                    if (paletteStroke != null)
                    {
                        factory.SetCompositionColorBrush(childVisual, paletteStroke, true);
                    }
                    else
                    {
                        factory.SetCompositionColorBrush(childVisual, paletteStroke, true);
                    }
                }
            }
        }

        protected override void Reset()
        {
            this.shapeGeometry.Figures.Clear();
        }

        protected override void RenderCore()
        {
            // we need at least two points to calculate the line
            if (this.renderPoints.Count < 2)
            {
                return;
            }

            PathFigure figure = null;
            PolyLineSegment lineSegment = null;

            foreach (DataPointSegment dataSegment in ChartSeriesRenderer.GetDataSegments(this.renderPoints))
            {
                foreach (Point point in this.GetPoints(dataSegment))
                {
                    if (lineSegment == null)
                    {
                        figure = new PathFigure();
                        figure.StartPoint = point;

                        lineSegment = new PolyLineSegment();

                        continue;
                    }

                    lineSegment.Points.Add(point);
                }

                // NOTE: data segment is defined if it contains at least two points so we are sure that the PolyLineSegment is meaningful (if it exists)
                if (lineSegment != null)
                {
                    figure.Segments.Add(lineSegment);
                    this.shapeGeometry.Figures.Add(figure);

                    figure = null;
                    lineSegment = null;
                }
            }
        }

        protected internal virtual IEnumerable<Point> GetPoints(DataPointSegment segment)
        {
            int pointIndex = segment.StartIndex;
            while (pointIndex <= segment.EndIndex)
            {
                yield return this.renderPoints[pointIndex].Center();

                pointIndex++;
            }
        }
    }
}
