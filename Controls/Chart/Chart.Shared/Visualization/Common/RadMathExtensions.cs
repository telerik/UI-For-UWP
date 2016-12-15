using System;
using Telerik.Charting;
using Telerik.Core;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal static class RadMathExtensions
    {
        public static Point Location(this DataPoint dataPoint)
        {
            return new Point(dataPoint.layoutSlot.X, dataPoint.layoutSlot.Y);
        }

        public static Point Center(this DataPoint dataPoint)
        {
            return new Point(
                dataPoint.layoutSlot.X + (int)(dataPoint.layoutSlot.Width / 2),
                dataPoint.layoutSlot.Y + (int)(dataPoint.layoutSlot.Height / 2));
        }

        public static double CenterX(this DataPoint dataPoint)
        {
            return dataPoint.layoutSlot.X + (int)(dataPoint.layoutSlot.Width / 2);
        }

        public static double CenterY(this DataPoint dataPoint)
        {
            return dataPoint.layoutSlot.Y + (int)(dataPoint.layoutSlot.Height / 2);
        }

        public static Point ToPoint(this RadPoint point)
        {
            return new Point(point.X, point.Y);
        }

        public static Rect ToRect(this RadRect rect)
        {
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
