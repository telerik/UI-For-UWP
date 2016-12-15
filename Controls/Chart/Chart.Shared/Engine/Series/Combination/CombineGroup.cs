using System;
using System.Collections.Generic;

namespace Telerik.Charting
{
    /// <summary>
    /// Stores one or more data points of combined chart series within a group.
    /// </summary>
    internal class CombineGroup
    {
        public List<CombineStack> Stacks = new List<CombineStack>();
        public object Key;

        public static void ProcessPoint(DataPoint point)
        {
            ChartSeriesModel series = point.parent as ChartSeriesModel;
        }

        public CombineStack GetStack(ISupportCombineMode series)
        {
            if (series.CombineMode == ChartSeriesCombineMode.Cluster)
            {
                return this.CreateNewStack(series);
            }

            object stackKey = series.StackGroupKey;
            foreach (CombineStack stack in this.Stacks)
            {
                if (object.Equals(stack.Key, stackKey))
                {
                    return stack;
                }
            }

            return this.CreateNewStack(series);
        }

        private CombineStack CreateNewStack(ISupportCombineMode series)
        {
            CombineStack newStack = new CombineStack();
            newStack.Key = series.StackGroupKey;
            this.Stacks.Add(newStack);

            return newStack;
        }
    }
}
