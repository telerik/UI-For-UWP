using System;
using System.Globalization;
using Telerik.Core;
using Windows.Foundation;

namespace Telerik.Charting
{
    /// <summary>
    /// Represents a single-valued data point drawn as a pie slice when plotted by a pie chart.
    /// </summary>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.RadPieChart"/>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.PieSeries"/>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.DoughnutSeries"/>
    public class PieDataPoint : SingleValueDataPoint
    {
        internal static readonly int OffsetFromCenterPropertyKey = PropertyKeys.Register(typeof(PieDataPoint), "OffsetFromCenter", ChartAreaInvalidateFlags.All);

        internal double startAngle;
        internal double sweepAngle;
        internal double normalizedValue;

        /// <summary>
        /// Gets the value which indicates what percentage of all data point values sum this point's value is.
        /// </summary>
        public double Percent
        {
            get
            {
                return this.normalizedValue * 100;
            }
        }

        /// <summary>
        /// Gets or sets the offset of the point from the center of the pie.
        /// </summary>
        public double OffsetFromCenter
        {
            get
            {
                return this.GetTypedValue<double>(OffsetFromCenterPropertyKey, 0d);
            }
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                this.SetValue(OffsetFromCenterPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets the starting angle of this point.
        /// </summary>
        public double StartAngle
        {
            get
            {
                return this.startAngle;
            }
        }

        /// <summary>
        /// Gets the sweep angle of this point.
        /// </summary>
        public double SweepAngle
        {
            get
            {
                return this.sweepAngle;
            }
        }

        internal double Radius { get; set; }

        internal Point CenterPoint { get; set; }

        internal override object GetDefaultLabel()
        {
            string format;
            PieSeriesModel model = this.parent as PieSeriesModel;
            if (model != null)
            {
                format = model.labelFormat;
            }
            else
            {
                format = PieSeriesModel.DefaultLabelFormat;
            }
            return this.normalizedValue.ToString(format, CultureInfo.CurrentUICulture);
        }

        internal override RadRect GetPosition()
        {
            // TODO: Consider better interpretation of segments in cartesian coordinates.
            var centerR = this.Radius / 2;
            var centerAng = this.startAngle % 360 + this.SweepAngle / 2;

            var point = RadMath.ToCartesianCoordinates(centerR, centerAng);
            return new RadRect(this.CenterPoint.X + point.X, this.CenterPoint.Y + point.Y, 1, 1);
        }

        internal override bool ContainsPosition(double x, double y)
        {
            return this.ContainsRect(new Rect(x, y, 1, 1));
        }

        internal virtual double GetPolarDistance(Point point)
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

            return Math.Max(0, pointRadius - this.Radius / 2);
        }

        internal virtual bool ContainsRect(Rect touchRect)
        {
            // Switch to polar coordinate system. Intersect only for top left edge of the rect for simplicity.
            var xLength = Math.Abs(this.CenterPoint.X - touchRect.X);
            var yLength = Math.Abs(this.CenterPoint.Y - touchRect.Y);

            var pointRadius = Math.Sqrt(xLength * xLength + yLength * yLength);

            if (pointRadius > this.Radius)
            {
                return false;
            }

            var pointAngle = Math.Asin(yLength / pointRadius) * 180 / Math.PI;
            var normalizedStartAngle = this.startAngle % 360;

            // Determine quadrant and adjust the point angle accordingly
            if (this.CenterPoint.X < touchRect.X && this.CenterPoint.Y > touchRect.Y)
            {
                // I quadrant
                pointAngle = 360 - pointAngle;
            }
            else if (this.CenterPoint.X > touchRect.X && this.CenterPoint.Y > touchRect.Y)
            {
                // II quadrant
                pointAngle += 180;
            }
            else if (this.CenterPoint.X > touchRect.X && this.CenterPoint.Y < touchRect.Y)
            {
                // III quadrant
                pointAngle = 180 - pointAngle;
            }
            else if (this.CenterPoint.X < touchRect.X && this.CenterPoint.Y < touchRect.Y)
            {
                // IV quadrant
                if (this.startAngle + this.sweepAngle > 360)
                {
                    normalizedStartAngle = this.startAngle - 360;
                }
            }

            return pointAngle > normalizedStartAngle && pointAngle < normalizedStartAngle + this.sweepAngle;
        }
    }
}
