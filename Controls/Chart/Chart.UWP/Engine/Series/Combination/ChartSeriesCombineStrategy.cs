using System;
using System.Collections.Generic;

namespace Telerik.Charting
{
    /// <summary>
    /// Handles combination of chart series that are <see cref="ISupportCombineMode"/> instances and have their <see cref="ISupportCombineMode.CombineMode"/> member specified.
    /// </summary>
    internal class ChartSeriesCombineStrategy
    {
        public List<CombinedSeries> CombinedSeries;
        public List<ChartSeriesModel> NonCombinedSeries;
        public bool HasCombination;
        public ReferenceDictionary<AxisModel, double> MaximumStackSums;
        public ReferenceDictionary<AxisModel, double> MinimumStackSums;
        public AxisModel StackAxis;
        public List<AxisModel> StackValueAxes;
        public bool isUpdated;

        private ValueAxesExtractor valueAxesExtractor;

        public ChartSeriesCombineStrategy()
        {
            this.CombinedSeries = new List<CombinedSeries>();
            this.NonCombinedSeries = new List<ChartSeriesModel>();
            this.StackValueAxes = new List<AxisModel>();
            this.MaximumStackSums = new ReferenceDictionary<AxisModel, double>();
            this.MinimumStackSums = new ReferenceDictionary<AxisModel, double>();
        }

        internal delegate AxisModel ValueAxesExtractor(IPlotAreaElementModelWithAxes seriesModel);

        /// <summary>
        /// Gets or sets the combined bar series count. Exposed for testing purposes.
        /// </summary>
        internal int CombinedBarSeriesCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the combined stroked series count. Exposed for testing purposes.
        /// </summary>
        internal int CombinedStrokedSeriesCount
        {
            get;
            set;
        }

        public void Update(IList<ChartSeriesModel> series, AxisModel stackAxis)
        {
            if (this.isUpdated)
            {
                return;
            }

            this.StackAxis = stackAxis;

            if (stackAxis.Type == AxisType.First)
            {
                this.valueAxesExtractor = sm => sm.SecondAxis;
            }
            else
            {
                this.valueAxesExtractor = sm => sm.FirstAxis;
            }

            foreach (ChartSeriesModel model in series)
            {
                if (!model.presenter.IsVisible)
                {
                    continue;
                }

                AxisModel stackValueAxis = this.valueAxesExtractor(model as IPlotAreaElementModelWithAxes);
                if (!this.StackValueAxes.Contains(stackValueAxis))
                {
                    this.StackValueAxes.Add(stackValueAxis);
                }

                ISupportCombineMode combinableSeries = model as ISupportCombineMode;
                if (combinableSeries == null || combinableSeries.CombineMode == ChartSeriesCombineMode.None)
                {
                    this.NonCombinedSeries.Add(model);
                    continue;
                }

                CombinedSeries combinedSeries = this.GetCombinedSeries(combinableSeries);
                combinedSeries.Series.Add(model);

                this.HasCombination = true;
            }

            if (this.HasCombination)
            {
                this.BuildGroups();
            }

            this.isUpdated = true;
        }

        public void ApplyLayoutRounding(ChartAreaModel chartArea)
        {
            // combined series
            foreach (CombinedSeries series in this.CombinedSeries)
            {
                CombinedSeriesRoundLayoutStrategy strategy = ChartSeriesCombineStrategy.GetRoundLayoutStrategy(series);
                if (strategy != null)
                {
                    strategy.ApplyLayoutRounding(chartArea, series);
                }
            }

            // non-combined series
            foreach (ChartSeriesModel nonCombinedSeries in this.NonCombinedSeries)
            {
                nonCombinedSeries.ApplyLayoutRounding();
            }
        }

        public void Plot()
        {
            int count = this.CombinedSeries.Count;
            foreach (CombinedSeries series in this.CombinedSeries)
            {
                CombinedSeriesPlotStrategy strategy = ChartSeriesCombineStrategy.GetPlotStrategy(series);
                if (strategy != null)
                {
                    strategy.Plot(series, count);
                }
            }
        }

        public void Reset()
        {
            this.CombinedSeries.Clear();
            this.NonCombinedSeries.Clear();
            this.HasCombination = false;
            this.MaximumStackSums.Clear();
            this.MinimumStackSums.Clear();
            this.StackValueAxes.Clear();
            this.isUpdated = false;
        }

        private static CombinedSeriesPlotStrategy GetPlotStrategy(CombinedSeries series)
        {
            if (series.Series.Count > 0)
            {
                return series.Series[0].GetCombinedPlotStrategy();
            }

            return null;
        }

        private static CombinedSeriesRoundLayoutStrategy GetRoundLayoutStrategy(CombinedSeries series)
        {
            if (series.Series.Count > 0)
            {
                return series.Series[0].GetCombinedRoundLayoutStrategy();
            }

            return null;
        }

        private CombinedSeries GetCombinedSeries(ISupportCombineMode combinableSeries)
        {
            Type type = combinableSeries.GetType();
            ChartSeriesCombineMode combineMode = combinableSeries.CombineMode;
            AxisModel stackValueAxis = this.valueAxesExtractor(combinableSeries as IPlotAreaElementModelWithAxes);

            foreach (CombinedSeries series in this.CombinedSeries)
            {
                if (series.SeriesType == type && series.CombineMode == combineMode && (combineMode == ChartSeriesCombineMode.Cluster ||
                    ((combineMode == ChartSeriesCombineMode.Stack || combineMode == ChartSeriesCombineMode.Stack100) && series.StackValueAxis == stackValueAxis)))
                {
                    return series;
                }
            }

            CombinedSeries newSeries = new CombinedSeries();
            newSeries.SeriesType = type;
            newSeries.CombineMode = combineMode;
            newSeries.CombineIndex = this.CombinedSeries.Count;
            newSeries.StackAxis = this.StackAxis;
            newSeries.StackValueAxis = stackValueAxis;
            this.CombinedSeries.Add(newSeries);

            return newSeries;
        }

        private void BuildGroups()
        {
            // default group logic
            foreach (CombinedSeries combinedSeries in this.CombinedSeries)
            {
                this.ProcessSeries(combinedSeries);
            }
        }

        private void ProcessSeries(CombinedSeries combinedSeries)
        {
            Dictionary<object, CombineGroup> groupsByKey = new Dictionary<object, CombineGroup>(8);
            Dictionary<CombineStack, double> positiveValuesSumByStack = new Dictionary<CombineStack, double>();
            Dictionary<CombineStack, double> negativeValuesSumByStack = new Dictionary<CombineStack, double>();
            CombineStack stack;
            double min;
            double max;
            bool positive;

            foreach (ChartSeriesModel series in combinedSeries.Series)
            {
                if (!series.presenter.IsVisible)
                {
                    continue;
                }

                AxisModel stackValueAxis = this.valueAxesExtractor(series as IPlotAreaElementModelWithAxes);
                if (!this.MinimumStackSums.ContainsKey(stackValueAxis))
                {
                    this.MinimumStackSums.Set(stackValueAxis, double.PositiveInfinity);
                    this.MaximumStackSums.Set(stackValueAxis, double.NegativeInfinity);
                }

                this.MinimumStackSums.TryGetValue(stackValueAxis, out min);
                this.MaximumStackSums.TryGetValue(stackValueAxis, out max);

                foreach (DataPoint point in series.DataPointsInternal)
                {
                    object key = this.StackAxis.GetCombineGroupKey(point);
                    if (key == null)
                    {
                        continue;
                    }

                    CombineGroup group;
                    if (!groupsByKey.TryGetValue(key, out group))
                    {
                        group = new CombineGroup();
                        group.Key = key;
                        groupsByKey[key] = group;
                        combinedSeries.Groups.Add(group);
                    }

                    stack = group.GetStack(series as ISupportCombineMode);
                    stack.Points.Add(point);

                    if (!positiveValuesSumByStack.ContainsKey(stack))
                    {
                        positiveValuesSumByStack[stack] = 0d;
                        negativeValuesSumByStack[stack] = 0d;
                    }

                    double positivesSum = positiveValuesSumByStack[stack];
                    double negativesSum = negativeValuesSumByStack[stack];

                    double stackSumValue;
                    if (!stackValueAxis.TryGetStackSumValue(point, out stackSumValue, out positive, ref positivesSum, ref negativesSum))
                    {
                        continue;
                    }

                    positiveValuesSumByStack[stack] = positivesSum;
                    negativeValuesSumByStack[stack] = negativesSum;

                    if (positive)
                    {
                        stack.PositiveSum = stackSumValue;
                    }
                    else
                    {
                        stack.NegativeSum = stackSumValue;
                    }

                    min = Math.Min(min, stack.NegativeSum);
                    max = Math.Max(max, stack.PositiveSum);
                }

                this.MinimumStackSums.Set(stackValueAxis, min);
                this.MaximumStackSums.Set(stackValueAxis, max);
            }
        }
    }
}
