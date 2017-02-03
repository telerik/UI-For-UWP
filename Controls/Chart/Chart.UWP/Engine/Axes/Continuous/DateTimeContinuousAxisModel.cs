using System;
using System.Collections.Generic;
using System.Diagnostics;
using Telerik.Core;

namespace Telerik.Charting
{
    /// <summary>
    /// Defines a DateTime axis that uses the actual timeline to plot series points.
    /// </summary>
    internal class DateTimeContinuousAxisModel : AxisModel, IContinuousAxisModel, ISupportGapLength
    {
        internal static readonly int MajorStepPropertyKey = PropertyKeys.Register(typeof(DateTimeContinuousAxisModel), "MajorStep", ChartAreaInvalidateFlags.All);
        internal static readonly int MajorStepUnitPropertyKey = PropertyKeys.Register(typeof(DateTimeContinuousAxisModel), "MajorStepUnit", ChartAreaInvalidateFlags.All);
        internal static readonly int GapLengthPropertyKey = PropertyKeys.Register(typeof(DateTimeContinuousAxisModel), "GapLength", ChartAreaInvalidateFlags.All);
        internal static readonly int PlotModePropertyKey = PropertyKeys.Register(typeof(DateTimeContinuousAxisModel), "PlotMode", ChartAreaInvalidateFlags.All);
        internal static readonly int MinimumPropertyKey = PropertyKeys.Register(typeof(DateTimeContinuousAxisModel), "Minimum", ChartAreaInvalidateFlags.All);
        internal static readonly int MaximumPropertyKey = PropertyKeys.Register(typeof(DateTimeContinuousAxisModel), "Maximum", ChartAreaInvalidateFlags.All);
        internal static readonly int MaximumTicksPropertyKey = PropertyKeys.Register(typeof(DateTimeContinuousAxisModel), "MaximumTicks", ChartAreaInvalidateFlags.All);
        internal static readonly int PlotStretchPropertyKey = PropertyKeys.Register(typeof(DateTimeContinuousAxisModel), "PlotStretch", ChartAreaInvalidateFlags.All);

        internal ValueRange<DateTime> actualRange;

        internal List<DateTimePoint> values;

        private decimal majorStep;
        private int monthStep;
        private int yearStep;
        private int maxTickCount;
        private TimeSpan tickInterval;
        private TimeSpan minDelta;
        private decimal tickZoomFactor;
        private ValueRange<decimal> visibleTicks;

        private decimal gapLength;
        private AxisPlotMode plotMode, actualPlotMode;
        private PlotInfo plotInfo;

        public DateTimeContinuousAxisModel()
        {
            this.actualRange = ValueRange<DateTime>.Empty;

            this.visibleTicks = new ValueRange<decimal>(-1, -1);
            this.tickZoomFactor = 1m;
            this.gapLength = 0.3m;
            this.plotMode = AxisPlotMode.BetweenTicks;

            this.values = new List<DateTimePoint>(16);

            this.maxTickCount = 31;
        }

        /// <summary>
        /// Gets or sets the <see cref="DateTimePlotStretchMode"/> value that determines the length of each <see cref="DataPoint"/> plotted by the axis.
        /// Defaults to DateTimePlotStretchMode.TickSlot.
        /// </summary>
        public DateTimePlotStretchMode PlotStretch
        {
            get
            {
                return this.GetTypedValue<DateTimePlotStretchMode>(PlotStretchPropertyKey, DateTimePlotStretchMode.TickSlot);
            }
            set
            {
                this.SetValue(PlotStretchPropertyKey, value);
            }
        }

        public double GapLength
        {
            get
            {
                return (double)this.gapLength;
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

        /// <summary>
        /// Gets or sets the plot mode used to position points along the axis.
        /// </summary>
        public AxisPlotMode PlotMode
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
        /// Gets or sets custom major step of the axis. The TimeSpan between each tick is calculated by using this value and the MajorStepUnit.
        /// Specify double.PositiveInfinity or double.NegativeInfinity to clear the custom value and to generate the step automatically.
        /// </summary>
        public double MajorStep
        {
            get
            {
                return this.GetTypedValue<double>(MajorStepPropertyKey, 0d);
            }
            set
            {
                if (double.IsInfinity(value))
                {
                    this.ClearValue(MajorStepPropertyKey);
                }
                else
                {
                    this.SetValue(MajorStepPropertyKey, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the unit that defines the custom major step of the axis.
        /// If no explicit step is defined, the axis will automatically calculate one, depending on the smallest difference between any two dates.
        /// </summary>
        public TimeInterval MajorStepUnit
        {
            get
            {
                return this.GetTypedValue<TimeInterval>(MajorStepUnitPropertyKey, TimeInterval.Year);
            }
            set
            {
                this.SetValue(MajorStepUnitPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the custom minimum of the axis.
        /// Specify DateTime.MinValue to clear the property value so that the minimum is auto-generated.
        /// </summary>
        public DateTime Minimum
        {
            get
            {
                return this.GetTypedValue<DateTime>(MinimumPropertyKey, DateTime.MinValue);
            }
            set
            {
                if (value == DateTime.MinValue)
                {
                    this.ClearValue(MinimumPropertyKey);
                }
                else
                {
                    this.SetValue(MinimumPropertyKey, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the custom maximum of the axis.
        /// Specify DateTime.MaxValue to clear the property value so that the maximum is auto-generated.
        /// </summary>
        public DateTime Maximum
        {
            get
            {
                return this.GetTypedValue<DateTime>(MaximumPropertyKey, DateTime.MaxValue);
            }
            set
            {
                if (value == DateTime.MaxValue)
                {
                    this.ClearValue(MaximumPropertyKey);
                }
                else
                {
                    this.SetValue(MaximumPropertyKey, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum ticks that might be displayed on the axis. There are corner cases when ticks may become really huge number. Defaults to 31.
        /// </summary>
        public int MaximumTicks
        {
            get
            {
                return this.maxTickCount;
            }
            set
            {
                this.SetValue(MaximumTicksPropertyKey, value);
            }
        }

        internal override bool SupportsCombinedPlot
        {
            get
            {
                return true;
            }
        }

        internal override AxisPlotMode ActualPlotMode
        {
            get
            {
                return this.actualPlotMode;
            }
        }

        /// <summary>
        /// Gets the month step. Exposed for testing purposes.
        /// </summary>
        internal int MonthStep
        {
            get
            {
                return this.monthStep;
            }
        }

        /// <summary>
        /// Gets the year step. Exposed for testing purposes.
        /// </summary>
        internal int YearStep
        {
            get
            {
                return this.yearStep;
            }
        }

        /// <summary>
        /// Gets the DateTime points. Exposed for testing purposes.
        /// </summary>
        internal List<DateTimePoint> DateTimePoints
        {
            get
            {
                return this.values;
            }
        }

        /// <summary>
        /// Gets the actual range. Exposed for testing purposes.
        /// </summary>
        internal ValueRange<DateTime> ActualRange
        {
            get
            {
                return this.actualRange;
            }
        }

        /// <summary>
        /// Gets the plot information. Exposed for testing purposes.
        /// </summary>
        internal PlotInfo PlotInformation
        {
            get
            {
                return this.plotInfo;
            }
        }

        /// <summary>
        /// Gets the tick interval. Exposed for testing purposes.
        /// </summary>
        internal TimeSpan TickInterval
        {
            get
            {
                return this.tickInterval;
            }
        }

        /// <summary>
        /// Gets the min delta. Exposed for testing purposes.
        /// </summary>
        internal TimeSpan MinDelta
        {
            get
            {
                return this.minDelta;
            }
        }

        private bool CanPlot
        {
            get
            {
                return this.values.Count > 0 && this.minDelta.Ticks > 0;
            }
        }

        internal override void ResetState()
        {
            base.ResetState();

            this.visibleTicks = new ValueRange<decimal>(-1, -1);
        }

        internal override void Reset()
        {
            base.Reset();

            this.values.Clear();
        }

        internal override void UpdateCore(AxisUpdateContext context)
        {
            this.BuildValues(context);
            if (this.values.Count == 0)
            {
                return;
            }

            this.UpdateActualPlotMode(context.Series);
            this.UpdateActualRange();
            this.FindMinDelta();

            if (!this.CanPlot)
            {
                return;
            }

            this.UpdateUnits();
            this.UpdatePlotInfo();

            this.BuildTimeSlots();
        }

        internal virtual void UpdateActualPlotMode(IList<ChartSeriesModel> seriesModels)
        {
            if (this.IsLocalValue(DateTimeContinuousAxisModel.PlotModePropertyKey))
            {
                this.actualPlotMode = this.plotMode;
            }
            else
            {
                this.actualPlotMode = ChartSeriesModel.SelectPlotMode(seriesModels);
            }
        }

        internal override void PlotCore(AxisUpdateContext context)
        {
            if (!this.CanPlot)
            {
                return;
            }

            decimal delta = this.plotInfo.Max - this.plotInfo.Min;
            if (delta == 0)
            {
                Debug.Assert(false, "Invalid plot pass.");
                return;
            }

            decimal pointPosition, timeSlotPosition, timeSlotLength;
            decimal pointSlotLength;
            decimal extend = this.plotInfo.Extend / 2;

            bool uniform = this.PlotStretch == DateTimePlotStretchMode.Uniform;

            foreach (DateTimePoint value in this.values)
            {
                if (value.Slot == null)
                {
                    // We want to run the tests in both Debug and Release modes.
                    this.ThrowNoTimeSlotException();
                    continue;
                }

                decimal pointTicks = value.Date.Ticks;

                pointPosition = (pointTicks - this.plotInfo.Min + extend) / delta;
                timeSlotLength = value.Slot.Ticks / delta;
                timeSlotPosition = pointPosition - (timeSlotLength / 2);

                if (uniform)
                {
                    int pointCount = (value.Point.parent as ChartSeriesModel).DataPointsInternal.Count;
                    pointSlotLength = (1m - this.gapLength) / pointCount;
                }
                else
                {
                    pointSlotLength = (1m - this.gapLength) * timeSlotLength;
                }

                CategoricalAxisPlotInfo currentPlotInfo = CategoricalAxisPlotInfo.Create(this, (double)timeSlotPosition);
                currentPlotInfo.CategoryKey = value.Date;
                currentPlotInfo.Position = this.IsInverse ? 1 - (double)pointPosition : (double)pointPosition;
                currentPlotInfo.Length = (double)pointSlotLength;

                value.Point.SetValueFromAxis(this, currentPlotInfo);
            }
        }

        internal override void OnZoomChanged()
        {
            base.OnZoomChanged();

            if (!this.CanPlot)
            {
                return;
            }

            decimal oldZoom = this.tickZoomFactor;
            this.UpdateUnits();

            if (oldZoom != this.tickZoomFactor)
            {
                // reset the visible ticks
                // TODO: Possible optimization - may subtract the difference, depending on the new zoom factor
                this.visibleTicks = new ValueRange<decimal>(-1, -1);

                if (this.actualPlotMode == AxisPlotMode.BetweenTicks)
                {
                    this.UpdatePlotInfo();
                    this.isPlotValid = false;
                }
            }
        }

        internal override IEnumerable<AxisTickModel> GenerateTicks(ValueRange<decimal> currentVisibleRange)
        {
            if (!this.CanPlot || !(this.plotInfo.Min < this.plotInfo.Max))
            {
                yield break;
            }

            this.UpdateVisibleTicks(currentVisibleRange);
            decimal plotDelta = this.plotInfo.Max - this.plotInfo.Min;

            decimal startTicks = Math.Max(0, (this.visibleTicks.minimum - this.plotInfo.Min) / plotDelta);
            decimal endTicks = Math.Min(1, (this.visibleTicks.maximum - this.plotInfo.Min) / plotDelta);
            decimal currentTicks = startTicks;
            decimal paddedCurrentTicks = currentTicks;

            int virtualIndex = (int)(startTicks * this.values.Count);

            if (this.actualPlotMode == AxisPlotMode.OnTicksPadded)
            {
                decimal nextTicks = this.GetNextTicks(this.plotInfo.Min, this.tickZoomFactor);
                paddedCurrentTicks += (nextTicks - this.plotInfo.Min) / plotDelta / 2;
            }

            while (paddedCurrentTicks < endTicks || RadMath.AreClose((double)paddedCurrentTicks, (double)endTicks))
            {
                MajorTickModel tick = new MajorTickModel();
                tick.virtualIndex = virtualIndex;
                tick.normalizedValue = this.IsInverse ? 1 - paddedCurrentTicks : paddedCurrentTicks;
                tick.value = this.plotInfo.Min + (currentTicks * plotDelta);

                decimal nextTicks = this.GetNextTicks(tick.value, this.tickZoomFactor);
                decimal step = (nextTicks - tick.value) / plotDelta;
                currentTicks += step;
                paddedCurrentTicks += step;
                virtualIndex++;

                yield return tick;
            }
        }

        internal override object GetLabelContent(AxisTickModel tick)
        {
            return new DateTime((long)tick.value);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Method is internal")]
        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            // update local values first and then call base to raise the PropertyChanged event (if needed)
            if (e.Key == GapLengthPropertyKey)
            {
                this.gapLength = (decimal)((double)e.NewValue);
            }
            else if (e.Key == PlotModePropertyKey)
            {
                this.plotMode = (AxisPlotMode)e.NewValue;
            }
            else if (e.Key == MaximumTicksPropertyKey)
            {
                this.maxTickCount = (int)e.NewValue;
            }

            base.OnPropertyChanged(e);
        }

        internal override AxisPlotInfo CreatePlotInfo(object value)
        {
            DateTime date;
            if (!DateTimeHelper.TryGetDateTime(value, out date) || this.plotInfo == null)
            {
                return base.CreatePlotInfo(value);
            }

            decimal delta = this.plotInfo.Max - this.plotInfo.Min;
            decimal extend = this.plotInfo.Extend / 2;
            decimal pointTicks = date.Ticks;
            decimal pointPosition = (pointTicks - this.plotInfo.Min + extend) / delta;
      
            CategoricalAxisPlotInfo info = CategoricalAxisPlotInfo.Create(this, (double)pointPosition);
            info.CategoryKey = date;
            info.Position = this.IsInverse ? (double)(1 - pointPosition) : (double)pointPosition;

            return info;
        }

        internal override object ConvertPhysicalUnitsToData(double coordinate, RadRect axisVirtualSize)
        {
            if (!this.isUpdated)
            {
                return null;
            }

            double relativePosition = this.CalculateRelativePosition(coordinate, axisVirtualSize);
     
            decimal delta = this.plotInfo.Max - this.plotInfo.Min;
            decimal pointTicks = ((decimal)relativePosition * delta) + this.plotInfo.Min - this.plotInfo.Extend / 2;

            return new DateTime((long)(pointTicks + 0.5M));
        }

        internal override object GetCombineGroupKey(DataPoint point)
        {
            object value = point.GetValueForAxis(this);
            if (value != null)
            {
                return value;
            }

            return base.GetCombineGroupKey(point);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Private method with conditional debug symbol"), Conditional("DEBUG")]
        private void ThrowNoTimeSlotException()
        {
            throw new ArgumentException("Must have a time slot at this point.");
        }

        private void UpdateVisibleTicks(ValueRange<decimal> visibleRange)
        {
            ValueRange<decimal> oldRange = this.visibleTicks;

            decimal delta = this.plotInfo.Max - this.plotInfo.Min;
            decimal visibleTicksStart = this.plotInfo.Min + (visibleRange.minimum * delta);
            decimal visibleTicksEnd = this.plotInfo.Min + (visibleRange.maximum * delta);

            // check whether this is the first time we are initializing the ticks
            if (this.visibleTicks.minimum == -1)
            {
                this.visibleTicks.minimum = this.plotInfo.Min;
            }
            if (this.visibleTicks.maximum == -1)
            {
                this.visibleTicks.maximum = this.plotInfo.Max;
            }

            // find minimum
            while (this.visibleTicks.minimum > visibleTicksStart)
            {
                this.visibleTicks.minimum = this.GetPreviousTicks(this.visibleTicks.minimum, this.tickZoomFactor);
            }
            while (this.visibleTicks.minimum < visibleTicksStart)
            {
                this.visibleTicks.minimum = this.GetNextTicks(this.visibleTicks.minimum, this.tickZoomFactor);
            }

            // find maximum
            while (this.visibleTicks.maximum < visibleTicksEnd)
            {
                this.visibleTicks.maximum = this.GetNextTicks(this.visibleTicks.maximum, this.tickZoomFactor);
            }
            while (this.visibleTicks.maximum > visibleTicksEnd)
            {
                this.visibleTicks.maximum = this.GetPreviousTicks(this.visibleTicks.maximum, this.tickZoomFactor);
            }

            // add one additional tick at start and one at end
            this.visibleTicks.minimum = this.GetPreviousTicks(this.visibleTicks.minimum, this.tickZoomFactor);
            this.visibleTicks.maximum = this.GetNextTicks(this.visibleTicks.maximum, this.tickZoomFactor);
        }

        private decimal CalculateTickZoomFactor()
        {
            decimal zoomFactor = (decimal)this.layoutStrategy.GetZoom();

            // minimum delta is the maximum available zoom
            if (this.tickInterval.Ticks / zoomFactor < this.minDelta.Ticks)
            {
                zoomFactor = this.tickInterval.Ticks / this.minDelta.Ticks;
            }
            else
            {
                zoomFactor -= zoomFactor % 2;
            }

            return Math.Max(1, zoomFactor);
        }

        private void FindMinDelta()
        {
            // find the smallest difference between any two dates - this will give us the major and minor unit components
            // since values are sorted, all we need to do is loop the list once and compare two adjacent values
            this.minDelta = TimeSpan.Zero;
            int count = this.values.Count;
            DateTimePoint prevPoint = null;

            for (int i = 0; i < count; i++)
            {
                DateTimePoint point = this.values[i];
                if (prevPoint != null && prevPoint.Date != point.Date)
                {
                    TimeSpan diff = point.Date - prevPoint.Date;
                    if (diff < this.minDelta || this.minDelta == TimeSpan.Zero)
                    {
                        this.minDelta = diff;
                    }
                }

                prevPoint = point;
            }

            // min delta will not be initialized if only one point is present in the chart
            if (this.minDelta == TimeSpan.Zero)
            {
                this.minDelta = new TimeSpan(this.values[0].Date.Ticks);
            }
        }

        private void UpdateUnits()
        {
            this.monthStep = -1;
            this.yearStep = -1;

            object userStep = this.GetValue(MajorStepPropertyKey);
            TimeSpan range = new TimeSpan(this.actualRange.maximum.Ticks - this.actualRange.minimum.Ticks);
            if (userStep != null)
            {
                this.tickInterval = this.GetUserStep((double)userStep);

                if (this.tickInterval > range)
                {
                    this.tickInterval = range;
                }
            }
            else
            {
                this.tickInterval = this.minDelta;

                if (this.GetValue(MaximumTicksPropertyKey) == null)
                {
                    if ((this.tickInterval.TotalDays >= 28 && this.tickInterval.TotalDays <= 31) ||
                        (this.tickInterval.TotalDays >= 59 && this.tickInterval.TotalDays <= 62) ||
                        (this.tickInterval.TotalDays >= 89 && this.tickInterval.TotalDays <= 92) ||
                        (this.tickInterval.TotalDays >= 120 && this.tickInterval.TotalDays <= 122) ||
                        (this.tickInterval.TotalDays >= 181 && this.tickInterval.TotalDays <= 184))
                    {
                        // tickInterval represents something like 1, 2, 3, 4, or 6 months. 
                        this.monthStep = 1;
                    }
                    else if ((this.tickInterval.TotalDays >= 365 && this.tickInterval.TotalDays <= 366) ||
                             (this.tickInterval.TotalDays >= 730 && this.tickInterval.TotalDays <= 731))
                    {
                        // tickInterval represents something like 1 or 2 years.
                        this.yearStep = 1;
                    }
                }

                double zoomFactor = this.layoutStrategy.GetZoom();
                long tickCount = (long)(range.Ticks / this.tickInterval.Ticks / zoomFactor);
                if (tickCount > this.maxTickCount - 1)
                {
                    this.tickInterval = TimeSpan.FromTicks(range.Ticks / (this.maxTickCount - 1));
                }
            }

            this.majorStep = this.tickInterval.Ticks;

            // we can have zero as a step when adding points with same DateTime.
            if (this.majorStep == 0)
            {
                // use one-month step as default
                this.monthStep = 1;
            }

            this.tickZoomFactor = this.CalculateTickZoomFactor();
        }

        private TimeSpan GetUserStep(double step)
        {
            switch (this.MajorStepUnit)
            {
                case TimeInterval.Day:
                    return TimeSpan.FromDays(step);
                case TimeInterval.Hour:
                    return TimeSpan.FromHours(step);
                case TimeInterval.Millisecond:
                    return TimeSpan.FromMilliseconds(step);
                case TimeInterval.Minute:
                    return TimeSpan.FromMinutes(step);
                case TimeInterval.Month:
                    this.monthStep = (int)step;
                    return TimeSpan.FromDays(365 / 12 * step);
                case TimeInterval.Quarter:
                    this.monthStep = (int)step * 3;
                    return TimeSpan.FromDays(365 / 12 * step);
                case TimeInterval.Second:
                    return TimeSpan.FromSeconds(step);
                case TimeInterval.Week:
                    return TimeSpan.FromDays(7 * step);
                default: // year
                    this.yearStep = (int)step;
                    return TimeSpan.FromDays(365 * step);
            }
        }

        private void UpdateActualRange()
        {
            this.actualRange = this.GetAutoRange();

            object userMin = this.GetValue(MinimumPropertyKey);
            if (userMin != null)
            {
                this.actualRange.minimum = (DateTime)userMin;
            }

            object userMax = this.GetValue(MaximumPropertyKey);
            if (userMax != null)
            {
                this.actualRange.maximum = (DateTime)userMax;
            }
        }

        // TODO: Check whether this parameter is needed.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "context")]
        private ValueRange<DateTime> GetAutoRange()
        {
            ValueRange<DateTime> autoRange = new ValueRange<DateTime>();
            autoRange.minimum = this.values[0].Date;
            autoRange.maximum = this.values[this.values.Count - 1].Date;

            return autoRange;
        }

        private void UpdatePlotInfo()
        {
            this.plotInfo = new PlotInfo();

            this.plotInfo.Min = this.actualRange.minimum.Ticks;
            this.plotInfo.Max = this.actualRange.maximum.Ticks;

            if (this.actualPlotMode == AxisPlotMode.BetweenTicks || this.actualPlotMode == AxisPlotMode.OnTicksPadded)
            {
                // add one additional tick at the end
                decimal nextTicks = this.GetNextTicks(this.plotInfo.Max, this.tickZoomFactor);
                this.plotInfo.Extend = nextTicks - this.plotInfo.Max;
                this.plotInfo.Max += this.plotInfo.Extend;
            }
            else if (this.plotInfo.Min == this.plotInfo.Max)
            {
                this.plotInfo.Max = this.GetNextTicks(this.plotInfo.Min, this.tickZoomFactor);
            }
        }

        private void BuildTimeSlots()
        {
            // TODO: We should build time slots for all the points, not just the ones within the plot range
            ValueRange<DateTime> autoRange = this.GetAutoRange();
            decimal startTicks = autoRange.minimum.Ticks;
            decimal endTicks = autoRange.maximum.Ticks;

            if (startTicks == endTicks)
            {
                this.BuildSingleTimeSlot();
                return;
            }

            int pointCount = this.values.Count;
            int pointIndex = 0;
            decimal currentTicks = startTicks;
            decimal nextTicks;

            while (currentTicks <= endTicks)
            {
                nextTicks = this.GetNextTicks(currentTicks, 1);

                if (pointIndex < pointCount)
                {
                    DateTimePoint point = this.values[pointIndex];

                    // value falls within the slot
                    if (point.Date.Ticks < nextTicks)
                    {
                        TimeSlot slot = new TimeSlot() { StartTicks = currentTicks, Ticks = nextTicks - currentTicks };
                        point.Slot = slot;

                        // move to next point index
                        pointIndex++;
                        while (pointIndex < pointCount)
                        {
                            DateTimePoint nextPoint = this.values[pointIndex];
                            if (nextPoint.Date.Ticks >= nextTicks)
                            {
                                break;
                            }

                            nextPoint.Slot = slot;
                            pointIndex++;
                            slot.PointCount++;
                        }
                    }
                }

                currentTicks = nextTicks;
            }
        }

        private void BuildSingleTimeSlot()
        {
            decimal startTicks = this.actualRange.minimum.Ticks;
            decimal ticks = this.GetNextTicks(startTicks, 1) - startTicks;

            TimeSlot slot = new TimeSlot() { StartTicks = startTicks, Ticks = ticks };
            foreach (DateTimePoint point in this.values)
            {
                point.Slot = slot;
            }
        }

        private void BuildValues(AxisUpdateContext context)
        {
            this.values.Clear();

            if (context.Series == null)
            {
                return;
            }

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
                    object value = point.GetValueForAxis(this);
                    DateTime date;
                    if (!DateTimeHelper.TryGetDateTime(value, out date))
                    {
                        continue;
                    }

                    DateTimePoint datePoint = new DateTimePoint() { Point = point, Date = date };
                    this.values.Add(datePoint);
                }
            }

            // sort all the values chronologically
            this.values.Sort();         
        }

        private decimal GetNextTicks(decimal currentTicks, decimal zoomFactor)
        {
            if (this.monthStep != -1)
            {
                DateTime date = new DateTime((long)currentTicks);
                int months = Math.Max(1, (int)(this.monthStep / zoomFactor));
                date = date.AddMonths(months);

                return date.Ticks;
            }
            else if (this.yearStep != -1)
            {
                DateTime date = new DateTime((long)currentTicks);
                int years = Math.Max(1, (int)(this.yearStep / zoomFactor));
                date = date.AddYears(years);

                return date.Ticks;
            }

            return currentTicks + (this.majorStep / zoomFactor);
        }

        private decimal GetPreviousTicks(decimal currentTicks, decimal zoomFactor)
        {
            if (this.monthStep != -1)
            {
                DateTime date = new DateTime((long)currentTicks);
                int months = Math.Max(1, (int)(this.monthStep / zoomFactor));
                date = date.AddMonths(-months);

                return date.Ticks;
            }
            else if (this.yearStep != -1)
            {
                DateTime date = new DateTime((long)currentTicks);
                int years = Math.Max(1, (int)(this.yearStep / zoomFactor));
                date = date.AddYears(-years);

                return date.Ticks;
            }

            return currentTicks - (this.majorStep / zoomFactor);
        }

        internal class DateTimePoint : IComparable<DateTimePoint>
        {
            public DateTime Date;
            public DataPoint Point;
            public TimeSlot Slot;

            public int CompareTo(DateTimePoint other)
            {
                if (other == null)
                {
                    return 1;
                }

                return this.Date.CompareTo(other.Date);
            }
        }

        internal class TimeSlot
        {
            public decimal StartTicks;
            public decimal Ticks;
            public int PointCount = 1;
        }

        internal class PlotInfo
        {
            public decimal Min;
            public decimal Max;
            public decimal Extend;
        }
    }
}