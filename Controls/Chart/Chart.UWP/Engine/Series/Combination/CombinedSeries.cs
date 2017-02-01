using System;
using System.Collections.Generic;

namespace Telerik.Charting
{
    internal class CombinedSeries
    {
        public Type SeriesType;
        public ChartSeriesCombineMode CombineMode;
        public List<ChartSeriesModel> Series = new List<ChartSeriesModel>();
        public List<CombineGroup> Groups = new List<CombineGroup>();
        public int CombineIndex;
        public AxisModel StackAxis;
        public AxisModel StackValueAxis;
    }
}
