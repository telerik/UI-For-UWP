using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents the financial Candlestick symbol.
    /// </summary>
    public class OhlcStick : OhlcShape
    {
        internal override void UpdateElementAppearance()
        {
            this.UpdateOhlcElementStroke();
        }

        internal override void UpdateGeometry()
        {
            if (this.dataPoint == null)
            {
                (this.Data as PathGeometry).Figures.Clear();
                return;
            }

            PathGeometry pathGeometry = new PathGeometry();

            var numericalPlot = this.dataPoint.numericalPlot;

            // NOTE: Rendering the shape within (1,1) box and then stretching it generally works,
            // however, the "stretch" causes unsolvable layout rounding issues.
            // Therefore, we are now rendering the element within the actual physical dimensions.
            var boxHeight = this.dataPoint.layoutSlot.Height;
            var boxWidth = this.dataPoint.layoutSlot.Width;

            double leftTickValue = numericalPlot.PhysicalOpen;
            double rightTickValue = numericalPlot.PhysicalClose;
            double halfBoxWidth = (int)(0.5 * boxWidth);

            double singlePixelRounding = (this.StrokeThickness % 2) == 1 ? 0.5 : 0;

            PathFigure lineFigure = new PathFigure();
            lineFigure.IsClosed = false;
            lineFigure.StartPoint = new Point(halfBoxWidth + singlePixelRounding, -singlePixelRounding);
            var lineSegment = new LineSegment() { Point = new Point(halfBoxWidth + singlePixelRounding, boxHeight - singlePixelRounding) };
            lineFigure.Segments.Add(lineSegment);
            pathGeometry.Figures.Add(lineFigure);

            lineFigure = new PathFigure();
            lineFigure.IsClosed = false;
            lineFigure.StartPoint = new Point(singlePixelRounding, leftTickValue - singlePixelRounding);
            lineSegment = new LineSegment() { Point = new Point(halfBoxWidth + singlePixelRounding, leftTickValue - singlePixelRounding) };
            lineFigure.Segments.Add(lineSegment);
            pathGeometry.Figures.Add(lineFigure);

            lineFigure = new PathFigure();
            lineFigure.IsClosed = false;
            lineFigure.StartPoint = new Point(halfBoxWidth + singlePixelRounding, rightTickValue - singlePixelRounding);
            lineSegment = new LineSegment() { Point = new Point(boxWidth - singlePixelRounding, rightTickValue - singlePixelRounding) };
            lineFigure.Segments.Add(lineSegment);
            pathGeometry.Figures.Add(lineFigure);

            this.Data = pathGeometry;
        }
    }
}
