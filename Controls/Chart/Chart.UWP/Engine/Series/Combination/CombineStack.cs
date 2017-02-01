using System;
using System.Collections.Generic;

namespace Telerik.Charting
{
    internal class CombineStack
    {
        public List<DataPoint> Points = new List<DataPoint>();
        public object Key;
        public double PositiveSum;
        public double NegativeSum;
    }
}
