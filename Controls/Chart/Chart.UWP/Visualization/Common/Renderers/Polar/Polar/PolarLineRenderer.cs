using System;
using System.Collections;
using System.Collections.Generic;
using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class PolarLineRenderer : PolarLineRendererBase
    {
        private IComparer<DataPoint> comparer;

        internal override IComparer<DataPoint> Comparer
        {
            get
            {
                if (this.comparer == null)
                {
                    this.comparer = new AngleComparer();
                }
                return this.comparer;
            }
        }

        private class AngleComparer : IComparer<DataPoint>
        {
            public int Compare(DataPoint x, DataPoint y)
            {
                PolarDataPoint point1 = x as PolarDataPoint;
                PolarDataPoint point2 = y as PolarDataPoint;

                return point1.Angle.CompareTo(point2.Angle);
            }
        }
    }
}
