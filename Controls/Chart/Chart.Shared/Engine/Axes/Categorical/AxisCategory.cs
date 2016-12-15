using System;
using System.Collections.Generic;

namespace Telerik.Charting
{
    internal class AxisCategory
    {
        // the key used to determine distinct categories
        public object Key;

        // the source object, containing the key. 
        // this for example is used by the DateTime categorical axis where key is the Day while the key source is the associated DateTime.
        public object KeySource;
        public List<DataPoint> Points = new List<DataPoint>();
    }
}
