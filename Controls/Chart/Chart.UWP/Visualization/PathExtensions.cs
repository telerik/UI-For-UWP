using System;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    public static class PathExtensions
    {
        public static Point GetTopLeft(this Path path)
        {
            if (path.Data.Bounds != new Rect(0, 0, 0, 0))
                return new Point(path.Data.Bounds.X, path.Data.Bounds.Y);

            double x = 0;
            double y = 0;
            foreach (var figure in (path.Data as PathGeometry).Figures)
            {
                x = figure.StartPoint.X;
                y = figure.StartPoint.Y;
                foreach (var segment in figure.Segments)
                {
                    foreach (var segmentPoint in (segment as PolyLineSegment).Points)
                    {
                        x = Math.Min(x, segmentPoint.X);
                        y = Math.Min(y, segmentPoint.Y);
                    }
                }
            }

            return new Point(x, y);
        }
    }
}
