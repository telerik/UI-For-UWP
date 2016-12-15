using System;
using System.Collections.Generic;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class PolarChartGridModel : ChartGridModel
    {
        internal List<RadCircle> radialLines;
        internal List<RadPolarVector> polarLines;

        public PolarChartGridModel()
        {
            this.radialLines = new List<RadCircle>(4);
            this.polarLines = new List<RadPolarVector>(16);
        }

        internal override RadRect ArrangeOverride(RadRect rect)
        {
            this.radialLines.Clear();
            this.polarLines.Clear();

            RadPoint center = RadPoint.Round(rect.Center);
            double radius = Math.Max(0, rect.Width / 2);
            PolarChartAreaModel polarArea = this.GetChartArea<PolarChartAreaModel>();
            PolarAxisModel polarAxis = polarArea.PolarAxis;
            foreach (AxisTickModel tick in polarAxis.MajorTicks)
            {
                double tickRadius = (int)(((double)tick.normalizedValue * radius) + 0.5);
                this.radialLines.Add(new RadCircle() { Center = center, Radius = tickRadius });
            }

            AxisModel angleAxis = polarArea.AngleAxis;
            foreach (AxisTickModel tick in angleAxis.MajorTicks)
            {
                // do not add a line for the last tick
                if (RadMath.IsOne(angleAxis.IsInverse ? 1 - tick.normalizedValue : tick.normalizedValue))
                {
                    continue;
                }

                double angle = polarArea.NormalizeAngle((double)tick.value);
                this.polarLines.Add(new RadPolarVector() { Center = center, Point = tick.layoutSlot.Location, Angle = angle });
            }

            return rect;
        }
    }
}