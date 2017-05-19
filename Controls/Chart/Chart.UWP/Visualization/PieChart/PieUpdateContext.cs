using System;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class PieUpdateContext
    {
        public Size ArcSize;
        public Point Center;
        public double Radius;
        public double Diameter;
        
        public SweepDirection SweepDirection;

        public double StartAngle { get; set; }

        public Point CalculateArcPoint(double angle)
        {
            double angleInRad = angle * RadMath.DegToRadFactor;

            double x = this.Center.X + (Math.Cos(angleInRad) * this.Radius);
            double y = this.Center.Y + (Math.Sin(angleInRad) * this.Radius);

            // NOTE: Rounding this causes several issues with arc rendering of a combination of very large & very small slices so we do not do it.
            // Issue #1: All pie slices are not rendereded due to wrong rounded arc coordinates.
            // Issue #2: Under specific chart height/width the arc is rendereded misplaced (not centered).
            return new Point(x, y);
        }
    }
}