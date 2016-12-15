using System;
using Telerik.Core;

namespace Telerik.Charting
{
    internal static class NodeExtensions
    {
        public static ChartAreaModel GetChartArea(this Node node)
        {
            return node.root as ChartAreaModel;
        }

        public static T GetChartArea<T>(this Node node) where T : ChartAreaModel
        {
            return node.root as T;
        }
    }
}
