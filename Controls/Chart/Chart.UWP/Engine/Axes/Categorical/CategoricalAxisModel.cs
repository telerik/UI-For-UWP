using System;
using System.Collections.Generic;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class CategoricalAxisModel : AxisModel, ISupportGapLength
    {
        internal const double DefaultGapLength = 0.3;

        internal static readonly int GapLengthPropertyKey = PropertyKeys.Register(typeof(CategoricalAxisModel), "GapLength", ChartAreaInvalidateFlags.All);
        internal static readonly int PlotModePropertyKey = PropertyKeys.Register(typeof(CategoricalAxisModel), "PlotMode", ChartAreaInvalidateFlags.All);
        internal static readonly int MajorTickIntervalPropertyKey = PropertyKeys.Register(typeof(CategoricalAxisModel), "MajorTickInterval", ChartAreaInvalidateFlags.All);
        internal static readonly int AutoGroupPropertyKey = PropertyKeys.Register(typeof(CategoricalAxisModel), "AutoGroup", ChartAreaInvalidateFlags.All);

        internal List<AxisCategory> categories;
        internal AxisPlotMode plotMode, actualPlotMode;

        public CategoricalAxisModel()
        {
            this.categories = new List<AxisCategory>();
            this.plotMode = AxisPlotMode.BetweenTicks;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the axis will perform its own grouping 
        /// logic or it will consider each data point as a new group.
        /// </summary>
        public bool AutoGroup
        {
            get
            {
                return this.GetTypedValue<bool>(AutoGroupPropertyKey, true);
            }
            set
            {
                this.SetValue(AutoGroupPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the step at which ticks are positioned.
        /// </summary>
        public int MajorTickInterval
        {
            get
            {
                return this.GetTypedValue<int>(MajorTickIntervalPropertyKey, 1);
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                this.SetValue(MajorTickIntervalPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the length of the gap to be applied for each category.
        /// </summary>
        public double GapLength
        {
            get
            {
                return this.GetTypedValue<double>(GapLengthPropertyKey, DefaultGapLength);
            }
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                this.SetValue(GapLengthPropertyKey, value);
            }
        }

        internal override bool SupportsCombinedPlot
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the plot mode used to position points along the axis.
        /// </summary>
        internal AxisPlotMode PlotMode
        {
            get
            {
                return this.plotMode;
            }
            set
            {
                this.SetValue(PlotModePropertyKey, value);
            }
        }

        /// <summary>
        /// Gets the plot mode actually used by this axis.
        /// </summary>
        internal override AxisPlotMode ActualPlotMode
        {
            get
            {
                return this.actualPlotMode;
            }
        }

        internal override object GetCombineGroupKey(DataPoint point)
        {
            object value = this.GetCategoryValue(point);

            return this.GetCategoryKey(point, value);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Method is internal")]
        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            // update local value first and then call base to raise the PropertyChanged event (if needed)
            if (e.Key == PlotModePropertyKey)
            {
                this.plotMode = (AxisPlotMode)e.NewValue;
            }

            base.OnPropertyChanged(e);
        }

        internal override void Reset()
        {
            base.Reset();

            this.categories.Clear();
        }

        internal override void UpdateCore(AxisUpdateContext context)
        {
            this.UpdateActualPlotMode(context.Series);

            this.categories.Clear();
            this.BuildCategories(context);
        }

        internal override void PlotCore(AxisUpdateContext context)
        {
            int count = this.categories.Count;
            if (count == 0)
            {
                return;
            }

            double step = this.CalculateRelativeStep(count);

            double value = 0;
            double gap = this.GapLength * step;
            double length = step - gap;
            
            double offset = this.actualPlotMode == AxisPlotMode.OnTicks ? 0 : step / 2;
            double position;
           
            for (int i = 0; i < count; i++)
            {
                AxisCategory category = this.categories[i];
                position = this.IsInverse ? 1 - value - offset : value + offset;
                foreach (DataPoint point in category.Points)
                {
                    CategoricalAxisPlotInfo info = CategoricalAxisPlotInfo.Create(this, value);
                    info.CategoryKey = category.KeySource;
                    info.Position = position;
                    info.Length = length;

                    point.SetValueFromAxis(this, info);
                }

                value += step;
            }
        }

        internal override IEnumerable<AxisTickModel> GenerateTicks(ValueRange<decimal> currentVisibleRange)
        {
            // use the decimal type for higher accuracy; see the XML comments on the GetVisibleRange method
            int categoryCount = this.categories.Count;
            if (categoryCount == 0)
            {
                yield break;
            }

            int tickInterval = this.GetMajorTickInterval();
            int emptyTickCount = 0;

            int tickCount = this.actualPlotMode == AxisPlotMode.OnTicks ? categoryCount : categoryCount + 1;
            decimal tickStep = tickCount == 1 ? 1 : 1m / (tickCount - 1);

            currentVisibleRange.minimum -= currentVisibleRange.minimum % tickStep;
            currentVisibleRange.maximum += tickStep - (currentVisibleRange.maximum % tickStep);
            decimal startTick, endTick;
            if (this.IsInverse)
            {
                startTick = Math.Max(0, 1 - currentVisibleRange.maximum);
                endTick = Math.Min(1, 1 - currentVisibleRange.minimum);
            }
            else
            {
                startTick = Math.Max(0, currentVisibleRange.minimum);
                endTick = Math.Min(1, currentVisibleRange.maximum);
            }

            if (this.actualPlotMode == AxisPlotMode.OnTicksPadded)
            {
                startTick = tickStep / 2;
            }

            decimal currentTick = startTick;

            int virtualIndex = (int)(startTick / tickStep);
            while (currentTick < endTick || RadMath.AreClose((double)currentTick, (double)endTick))
            {
                if (emptyTickCount == 0)
                {
                    AxisTickModel tick = new MajorTickModel();
                    tick.normalizedValue = this.IsInverse ? 1 - currentTick : currentTick;
                    tick.value = currentTick;
                    tick.virtualIndex = virtualIndex;

                    emptyTickCount = tickInterval - 1;

                    yield return tick;
                }
                else
                {
                    emptyTickCount--;
                }

                currentTick += tickStep;
                virtualIndex++;
            }
        }

        internal override object GetLabelContent(AxisTickModel tick)
        {
            if (tick.virtualIndex < this.categories.Count)
            {
                return this.categories[tick.virtualIndex].Key;
            }

            return null;
        }

        internal virtual void UpdateActualPlotMode(IList<ChartSeriesModel> seriesModels)
        {
            if (this.IsLocalValue(CategoricalAxisModel.PlotModePropertyKey))
            {
                this.actualPlotMode = this.plotMode;
            }
            else if (seriesModels != null)
            {
                this.actualPlotMode = ChartSeriesModel.SelectPlotMode(seriesModels);
            }
        }

        internal virtual object GetCategoryValue(DataPoint point)
        {
            return point.GetValueForAxis(this);
        }

        internal virtual object GetCategoryKey(DataPoint point, object value)
        {
            if (value != null)
            {
                return value;
            }

            return point.CollectionIndex + 1;
        }

        internal virtual int GetMajorTickInterval()
        {
            // reduce the interval proportionally to the zoom factor
            double scale = this.layoutStrategy.GetZoom();
            return Math.Max(1, this.MajorTickInterval / (int)(scale + .5));
        }

        internal override AxisPlotInfo CreatePlotInfo(object value)
        {
            for (int index = 0; index < this.categories.Count; index++)
            {
                AxisCategory category = this.categories[index];
                if (object.Equals(category.KeySource, value))
                {
                    double step = this.CalculateRelativeStep(this.categories.Count);
                    double gap = this.GapLength * step;
                    double length = step - gap;
                    double valueLength = index * step;
                    double offset = this.actualPlotMode == AxisPlotMode.OnTicks ? 0 : step / 2;
 
                    CategoricalAxisPlotInfo info = CategoricalAxisPlotInfo.Create(this, valueLength);
                    info.CategoryKey = value;
                    info.Position = this.IsInverse ? 1 - valueLength - offset : valueLength + offset;
                    info.Length = length;

                    return info;
                }
            }

            return null;
        }

        internal override object ConvertPhysicalUnitsToData(double coordinate, RadRect axisVirtualSize)
        {
            int categoriesCount = this.categories.Count;
            if (!this.isUpdated || categoriesCount == 0)
            {
                return null;
            }

            double step = this.CalculateRelativeStep(categoriesCount);
            double stepOffset = this.actualPlotMode == AxisPlotMode.OnTicks ? 0.5 * step : 0;
            double relativePosition = this.CalculateRelativePosition(coordinate, axisVirtualSize, stepOffset);

            int categoryIndex = (int)(relativePosition / step);
            if (categoryIndex < 0 || categoryIndex > categoriesCount - 1)
            {
                return null;
            }

            AxisCategory category = this.categories[categoryIndex];

            return category.KeySource;
        }

        protected virtual double CalculateRelativeStep(int count)
        {
            double step;
            if (this.actualPlotMode == AxisPlotMode.BetweenTicks || this.actualPlotMode == AxisPlotMode.OnTicksPadded)
            {
                step = 1d / count;
            }
            else
            {
                step = count == 1 ? 1 : 1d / (count - 1);
            }

            return step;
        }

        private void BuildCategories(AxisUpdateContext context)
        {
            if (context.Series == null)
            {
                return;
            }

            bool autoGroup = this.AutoGroup;

            Dictionary<object, AxisCategory> categoriesByKey = new Dictionary<object, AxisCategory>(8);
            AxisPlotDirection direction = this.type == AxisType.First ? AxisPlotDirection.Vertical : AxisPlotDirection.Horizontal;

            foreach (ChartSeriesModel series in context.Series)
            {
                // tell each series what is the plot direction
                series.SetValue(AxisModel.PlotDirectionPropertyKey, direction);

                if (!series.presenter.IsVisible)
                {
                    continue;
                }

                foreach (DataPoint point in series.DataPointsInternal)
                {
                    object value = this.GetCategoryValue(point);
                    object categoryKey = this.GetCategoryKey(point, value);
                    if (categoryKey == null)
                    {
                        continue;
                    }

                    AxisCategory category;
                    if (autoGroup)
                    {
                        if (!categoriesByKey.TryGetValue(categoryKey, out category))
                        {
                            category = this.CreateCategory(categoryKey, value);
                            categoriesByKey[categoryKey] = category;
                        }
                    }
                    else
                    {
                        category = this.CreateCategory(categoryKey, value);
                    }

                    category.Points.Add(point);
                }
            }
        }

        private AxisCategory CreateCategory(object key, object keySource)
        {
            AxisCategory category = new AxisCategory() { Key = key, KeySource = keySource };
            this.categories.Add(category);

            return category;
        }
    }
}