using System;

namespace Telerik.Charting
{
    internal interface IPlotAreaElementModelWithAxes
    {
        AxisModel FirstAxis { get; }
        AxisModel SecondAxis { get; }

        void AttachAxis(AxisModel axis, AxisType type);
        void DetachAxis(AxisModel axis);
    }
}
