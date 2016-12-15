using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Charting;
using Telerik.Core;
using Windows.Foundation;

namespace Telerik.Charting
{
    /// <summary>
    /// Represents a single-valued data point plotted by a doughnut series.
    /// </summary>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.RadPieChart"/>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.DoughnutSeries"/>
    public class DoughnutDataPoint : PieDataPoint
    {
        internal double InnerRadius { get; set; }

        internal override bool ContainsRect(Rect touchRect)
        {
            var xLength = Math.Abs(this.CenterPoint.X - touchRect.X);
            var yLength = Math.Abs(this.CenterPoint.Y - touchRect.Y);

            var pointRadius = Math.Sqrt(xLength * xLength + yLength * yLength);

            if (pointRadius < this.InnerRadius || pointRadius > this.Radius)
            {
                return false;
            }
            return base.ContainsRect(touchRect);
        }

        internal override double GetPolarDistance(Point point)
        {
            var coordinates = RadMath.ToPolarCoordinates(new RadPoint(point.X, point.Y), new RadPoint(this.CenterPoint.X, this.CenterPoint.Y));

            var normalizedStartAngle = this.startAngle % 360;

            if (this.CenterPoint.X < point.X && this.CenterPoint.Y < point.Y)
            {
                // IV quadrant
                if (this.startAngle + this.sweepAngle > 360)
                {
                    normalizedStartAngle = this.startAngle - 360;
                }
            }

            var pointRadius = coordinates.Item1;

            // Outside of bounds.
            if (coordinates.Item2 <= normalizedStartAngle || coordinates.Item2 >= normalizedStartAngle + this.sweepAngle)
            {
                pointRadius = double.PositiveInfinity;
            }

            return Math.Max(0, pointRadius - (this.Radius - this.InnerRadius) / 2 + this.InnerRadius);
        }
    }
}
