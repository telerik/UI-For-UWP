using System;
using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class ChartSeriesLabelUpdateContext
    {
        public ChartSeriesLabelDefinition Definition;
        public AxisPlotDirection PlotDirection;
        public bool IsPlotInverse;
        public int DefinitionIndex;
        public int PointVirtualIndex;
        public DataPoint Point;
    }
}