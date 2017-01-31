using System;
using System.Collections.Generic;
using System.Globalization;
using Telerik.Core;

namespace Telerik.Charting
{
    internal abstract class NumericalAxisModel : AxisModel, IContinuousAxisModel
    {
        internal static readonly int MinimumPropertyKey = PropertyKeys.Register(typeof(NumericalAxisModel), "Minimum", ChartAreaInvalidateFlags.All);
        internal static readonly int MaximumPropertyKey = PropertyKeys.Register(typeof(NumericalAxisModel), "Maximum", ChartAreaInvalidateFlags.All);
        internal static readonly int MajorStepPropertyKey = PropertyKeys.Register(typeof(NumericalAxisModel), "MajorStep", ChartAreaInvalidateFlags.All);
        internal static readonly int RangeExtendDirectionPropertyKey = PropertyKeys.Register(typeof(NumericalAxisModel), "RangeExtendDirection", ChartAreaInvalidateFlags.All);
        internal static readonly int DesiredTickCountPropertyKey = PropertyKeys.Register(typeof(NumericalAxisModel), "DesiredTickCount", ChartAreaInvalidateFlags.All);

        internal ValueRange<double> actualRange;
        internal ValueRange<double> pointMinMax;
        internal double majorStep;
        internal double normalizedOrigin;
        internal NumericalAxisRangeExtendDirection extendDirection;
        internal int userTickCount;

        // TODO: axis origin for logarithmic axis
        private const double DefaultOrigin = 0d;
        private const double DefaultMajorStep = 0d;
        private const double DefaultMinimum = double.NegativeInfinity;
        private const double DefaultMaximum = double.PositiveInfinity;
        private const string DefaultNumericalLabelFormat = "{0,0:G7}";

        private byte percentDecimalOffset;
        private bool isStacked100;

        protected NumericalAxisModel()
        {
            this.extendDirection = NumericalAxisRangeExtendDirection.Both;
        }

        private delegate double StackValueProcessor(CombineStack combineStack, double value);

        /// <summary>
        /// Gets or sets the number of the ticks available on the axis. If a value less than 2 is set, the property is reset to its default value.
        /// </summary>
        public int DesiredTickCount
        {
            get
            {
                return this.userTickCount == 0 ? this.DefaultTickCount : this.userTickCount;
            }
            set
            {
                if (value <= 1)
                {
                    this.ClearValue(DesiredTickCountPropertyKey);
                }
                else
                {
                    this.SetValue(DesiredTickCountPropertyKey, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies how the auto-range of this axis will be extended so that each data point is visualized in the best possible way.
        /// </summary>
        public NumericalAxisRangeExtendDirection RangeExtendDirection
        {
            get
            {
                return this.extendDirection;
            }
            set
            {
                this.SetValue(RangeExtendDirectionPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the user-defined major step of the axis.
        /// </summary>
        public double MajorStep
        {
            get
            {
                return this.GetTypedValue<double>(MajorStepPropertyKey, this.majorStep);
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Major step may not be less than 0");
                }

                if (value != DefaultMajorStep)
                {
                    this.SetValue(MajorStepPropertyKey, value);
                }
                else
                {
                    this.ClearValue(MajorStepPropertyKey);
                }
            }
        }

        /// <summary>
        /// Gets or sets the user-defined minimum of the axis.
        /// </summary>
        public double Minimum
        {
            get
            {
                return this.GetTypedValue<double>(MinimumPropertyKey, DefaultMinimum);
            }
            set
            {
                if (value != DefaultMinimum)
                {
                    this.SetValue(MinimumPropertyKey, value);
                }
                else
                {
                    this.ClearValue(MinimumPropertyKey);
                }
            }
        }

        /// <summary>
        /// Gets or sets the user-defined maximum of the axis.
        /// </summary>
        public double Maximum
        {
            get
            {
                return this.GetTypedValue<double>(MaximumPropertyKey, DefaultMaximum);
            }
            set
            {
                if (value != DefaultMaximum)
                {
                    this.SetValue(MaximumPropertyKey, value);
                }
                else
                {
                    this.ClearValue(MaximumPropertyKey);
                }
            }
        }

        /// <summary>
        /// Gets the actual range (minimum and maximum values) used by the axis.
        /// </summary>
        public ValueRange<double> ActualRange
        {
            get
            {
                return this.actualRange;
            }
        }

        internal virtual int DefaultTickCount
        {
            get
            {
                return 8;
            }
        }

        internal static double NormalizeStep(double initialStep)
        {
            double magnitute = Math.Floor(Math.Log10(initialStep));
            double magnitutePower = Math.Pow(10d, magnitute);

            // Calculate most significant digit of the new step size
            double magnitudeDigit = (int)((initialStep / magnitutePower) + .5);

            if (magnitudeDigit > 5)
            {
                magnitudeDigit = 10;
            }
            else if (magnitudeDigit > 2)
            {
                magnitudeDigit = 5;
            }
            else if (magnitudeDigit > 1)
            {
                magnitudeDigit = 2;
            }

            return magnitudeDigit * magnitutePower;
        }

        internal override void OnZoomChanged()
        {
            base.OnZoomChanged();

            if (this.isStacked100)
            {
                double zoom = this.layoutStrategy.GetZoom();
                this.percentDecimalOffset = 0;
                double step = NormalizeStep(this.majorStep / zoom) * 100;

                while (step > 0 && step < 1)
                {
                    this.percentDecimalOffset++;
                    step *= 10;
                }
            }
        }

        /// <summary>
        /// Gets the stack sum value for each DataPoint in a stack group used by a CombineStrategy.
        /// The result is the transformed value of the stack sum of the DataPoint values.
        /// </summary>
        /// <param name="point">The data point.</param>
        /// <param name="transformedStackSumValue">The transformed value of the stack sum of the DataPoint values.</param>
        /// <param name="positive">Determines whether the point value is positive relative to the plot origin.</param>
        /// <param name="positiveValuesSum">The present sum of positive DataPoint values in the stack. Updated if the DataPoint value is positive.</param>
        /// <param name="negativeValuesSum">The present sum of negative DataPoint values in the stack. Updated if the DataPoint value is negative.</param>
        /// <returns>Value that indicates whether the <paramref name="transformedStackSumValue"/> is calculated successfully.</returns>
        internal override bool TryGetStackSumValue(DataPoint point, out double transformedStackSumValue, out bool positive, ref double positiveValuesSum, ref double negativeValuesSum)
        {
            double doubleValue = DefaultOrigin;
            if (!point.isEmpty)
            {
                var objectValue = point.GetValueForAxis(this);
                if (!(objectValue is Range))
                {
                    doubleValue = (double)objectValue;
                }         
            }
            
            double transformedValue = this.TransformValue(doubleValue);
            if (double.IsNaN(transformedValue))
            {
                // Invalid state
                transformedStackSumValue = double.NaN;
                positive = false;

                return false;
            }

            positive = transformedValue >= DefaultOrigin;
            if (positive)
            {
                positiveValuesSum += doubleValue;
                transformedStackSumValue = this.TransformValue(positiveValuesSum);
            }
            else
            {
                negativeValuesSum += doubleValue;
                transformedStackSumValue = this.TransformValue(negativeValuesSum);
            }

            return true;
        }

        internal override AxisPlotInfo CreatePlotInfo(object value)
        {
            double doubleValue = 0d;
            if (value is double)
            {
                doubleValue = (double)value;
                if (double.IsNaN(doubleValue) || double.IsInfinity(doubleValue))
                {
                    return null;
                }
            }
            else if (value == null || !double.TryParse(value.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out doubleValue))
            {
                return base.CreatePlotInfo(value);
            }

            object transformedValue = this.TransformValue(doubleValue);
            if (double.IsNaN((double)transformedValue))
            {
                return null;
            }

            double delta = this.actualRange.maximum - this.actualRange.minimum;

            return this.CreateAxisPlotInfo(delta, (double)transformedValue);
        }

        internal override void ResetState()
        {
            base.ResetState();

            this.actualRange = new ValueRange<double>(0, 0);
            this.pointMinMax = new ValueRange<double>(0, 0);
        }

        internal override void UpdateCore(AxisUpdateContext context)
        {
            base.UpdateCore(context);

            this.UpdateActualRange(context);
            this.UpdatePlotOrigin(context);

            this.isStacked100 = context.IsStacked100;
        }

        internal override void PlotCore(AxisUpdateContext context)
        {
            if (context.IsStacked)
            {
                this.PlotStacked(context);
            }
            else if (context.IsStacked100)
            {
                this.PlotStacked100(context);
            }
            else if (context.Series != null)
            {
                this.PlotNormal(context.Series);
            }
        }

        internal override string GetLabelFormat()
        {
            string baseFormat = base.GetLabelFormat();

            if (string.IsNullOrEmpty(baseFormat))
            {
                return this.isStacked100 ? string.Format(CultureInfo.InvariantCulture, "{{0,0:p{0}}}", this.percentDecimalOffset) : DefaultNumericalLabelFormat;
            }

            return baseFormat;
        }

        internal virtual double CalculateAutoStep(ValueRange<double> range)
        {
            double step = (range.maximum - range.minimum) / (this.DesiredTickCount - 1);
            return NormalizeStep(step);
        }

        internal virtual object TransformValue(object value)
        {
            if (value is double)
            {
                return this.TransformValue((double)value);
            }
            else if (value is Ohlc)
            {
                return this.TransformValue((Ohlc)value);
            }
            else if (value is Range)
            {
                return this.TransformValue((Range)value);
            }
                
            return value;
        }

        internal virtual double TransformValue(double value)
        {
            return value;
        }

        internal virtual Ohlc TransformValue(Ohlc value)
        {
            return value;
        }

        internal virtual Range TransformValue(Range value)
        {
            return value;
        }

        internal virtual double ReverseTransformValue(double value)
        {
            return value;
        }

        internal virtual void UpdateActualRange(AxisUpdateContext context)
        {
            // TODO: Do not calculate auto-range if both min and max are user-defined
            this.pointMinMax = this.CalculateRange(context);
            this.actualRange = this.pointMinMax;

            object userMin = this.GetValue(MinimumPropertyKey);
            if (userMin != null)
            {
                this.actualRange.minimum = this.TransformValue((double)userMin);
            }
            object userMax = this.GetValue(MaximumPropertyKey);
            if (userMax != null)
            {
                this.actualRange.maximum = this.TransformValue((double)userMax);
            }

            this.actualRange.maximum = Math.Max(this.actualRange.minimum, this.actualRange.maximum);

            RangeCalculator calculator = new RangeCalculator(this, userMin != null, userMax != null);
            if (!context.IsStacked100)
            {
                this.actualRange = calculator.Extend();
            }

            object userStep = this.GetValue(MajorStepPropertyKey);
            if (userStep != null)
            {
                // LogarithmicAxis.ExponentStep now specifies the actual exponent so we should not "transform" the step value here.
                this.majorStep = (double)userStep;
            }
            else
            {
                this.majorStep = this.CalculateAutoStep(this.actualRange);
            }

            this.actualRange = calculator.RoundToMajorStep(this.majorStep);
            if (this.userTickCount > 0 && userStep == null)
            {
                this.RoundToUserTicks();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Method is internal")]
        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            // update local value first and then call base to raise the PropertyChanged event (if needed)
            if (e.Key == RangeExtendDirectionPropertyKey)
            {
                this.extendDirection = (NumericalAxisRangeExtendDirection)e.NewValue;
            }
            else if (e.Key == DesiredTickCountPropertyKey)
            {
                this.userTickCount = e.NewValue == null ? 0 : (int)e.NewValue;
            }

            base.OnPropertyChanged(e);
        }

        internal override object ConvertPhysicalUnitsToData(double coordinate, RadRect axisVirtualSize)
        {
            if (!this.isUpdated)
            {
                return null;
            }

            double relativePosition = this.CalculateRelativePosition(coordinate, axisVirtualSize);
            double delta = this.actualRange.maximum - this.actualRange.minimum;
            double value = (relativePosition * delta) + this.actualRange.minimum;

            return this.ReverseTransformValue(value);
        }

        private static ValueRange<double> AdjustRange(object value, ValueRange<double> range)
        {
            if (value is double)
            { 
                return AdjustRange((double)value, range);                
            }
            else if (value is Ohlc)
            {
                return AdjustRange((Ohlc)value, range);
            }
            else if (value is Range)
            {
                return AdjustRange((Range)value, range);
            }

            return range;
        }

        private static ValueRange<double> AdjustRange(Ohlc value, ValueRange<double> range)
        {
            double high = value.High;
            double low = value.Low;

            if (high > range.maximum)
            {
                range.maximum = high;
            }
            if (low < range.minimum)
            {
                range.minimum = low;
            }

            return range;
        }

        private static ValueRange<double> AdjustRange(Range value, ValueRange<double> range)
        {
            double high = value.High;
            double low = value.Low;

            if (high > range.maximum)
            {
                range.maximum = high;
            }
            if (low < range.minimum)
            {
                range.minimum = low;
            }

            return range;
        }

        private static ValueRange<double> AdjustRange(double value, ValueRange<double> range)
        {
            if (value > range.maximum)
            {
                range.maximum = value;
            }
            if (value < range.minimum)
            {
                range.minimum = value;
            }

            return range;
        }

        private static ValueRange<double> CalculateStacked100Range(AxisUpdateContext context)
        {
            // Note: Stacked100 series cannot be combined with stacked & normal series
            // so we should not loop through any potentially non-combined series here.
            ValueRange<double> stacked100Range = new ValueRange<double>(double.PositiveInfinity, double.NegativeInfinity);

            foreach (CombinedSeries series in context.CombinedSeries)
            {
                foreach (CombineGroup group in series.Groups)
                {
                    foreach (CombineStack stack in group.Stacks)
                    {
                        if (stack.PositiveSum == 0 && stack.NegativeSum == 0)
                        {
                            continue;
                        }

                        if (stack.PositiveSum == 0)
                        {
                            if (stacked100Range.maximum < 0)
                            {
                                stacked100Range.maximum = 0;
                            }

                            if (stacked100Range.minimum > -1)
                            {
                                stacked100Range.minimum = -1;
                            }
                        }
                        else if (stack.NegativeSum == 0)
                        {
                            if (stacked100Range.maximum < 1)
                            {
                                stacked100Range.maximum = 1;
                            }

                            if (stacked100Range.minimum > 0)
                            {
                                stacked100Range.minimum = 0;
                            }
                        }
                        else
                        {
                            double calculatedValue = stack.PositiveSum / (stack.PositiveSum - stack.NegativeSum);
                            if (calculatedValue > stacked100Range.maximum)
                            {
                                stacked100Range.maximum = calculatedValue;
                            }

                            calculatedValue = stack.NegativeSum / (stack.PositiveSum - stack.NegativeSum);
                            if (calculatedValue < stacked100Range.minimum)
                            {
                                stacked100Range.minimum = calculatedValue;
                            }
                        }

                        if (stacked100Range.minimum == -1 && stacked100Range.maximum == 1)
                        {
                            return stacked100Range;
                        }
                    }
                }
            }

            // Stacked100 series may have had no data points added
            if (stacked100Range.minimum == double.PositiveInfinity)
            {
                stacked100Range.minimum = 0d;
            }

            if (stacked100Range.maximum == double.NegativeInfinity)
            {
                stacked100Range.maximum = 0d;
            }

            return stacked100Range;
        }

        private void RoundToUserTicks()
        {
            int fractionalDigits = 0;
            double tempStep = this.majorStep;
            while (tempStep < 1 && tempStep > 0)
            {
                fractionalDigits++;
                tempStep *= 10;
            }

            double newStep = (this.actualRange.maximum - this.actualRange.minimum) / (this.userTickCount - 1);
            double multiplier = Math.Pow(10, fractionalDigits);

            newStep *= multiplier;
            newStep += Math.Ceiling(newStep) - newStep;
            newStep /= multiplier;

            this.majorStep = newStep;
            this.actualRange.maximum = (this.userTickCount - 1) * this.majorStep + this.actualRange.minimum;
        }

        private ValueRange<double> CalculateRange(AxisUpdateContext context)
        {
            ValueRange<double> range;

            if (context.IsStacked)
            {
                range = this.CalculateStackedRange(context);
            }
            else if (context.IsStacked100)
            {
                range = CalculateStacked100Range(context);
            }
            else
            {
                range = this.CalculateNormalRange(context.Series);
            }

            return range;
        }

        private ValueRange<double> CalculateNormalRange(IEnumerable<ChartSeriesModel> series)
        {
            ValueRange<double> range = new ValueRange<double>();
            range.minimum = double.PositiveInfinity;

            // retrieve core range
            if (series != null)
            {
                foreach (ChartSeriesModel model in series)
                {
                    if (!model.presenter.IsVisible)
                    {
                        continue;
                    }

                    foreach (DataPoint point in model.DataPointsInternal)
                    {
                        object value = point.GetValueForAxis(this);
                        object transformedValue = this.TransformValue(value);
                        range = AdjustRange(transformedValue, range);
                    }
                }
            }

            if (range.minimum == double.PositiveInfinity)
            {
                range.minimum = 0;
            }
            else if (range.minimum == range.maximum)
            {
                if (range.minimum != 0)
                {
                    range.minimum = 0;
                }
                else
                {
                    range.maximum = 1;
                }
            }

            return range;
        }

        private ValueRange<double> CalculateStackedRange(AxisUpdateContext context)
        {
            ValueRange<double> stackedRange = new ValueRange<double>();
            stackedRange.minimum = context.MinimumStackSum;
            stackedRange.maximum = context.MaximumStackSum;

            // loop through non-combined series to check min/max value in their points
            ValueRange<double> nonCombinedRange = this.CalculateNormalRange(context.NonCombinedSeries);
            if (stackedRange.minimum > nonCombinedRange.minimum)
            {
                stackedRange.minimum = nonCombinedRange.minimum;
            }
            if (stackedRange.maximum < nonCombinedRange.maximum)
            {
                stackedRange.maximum = nonCombinedRange.maximum;
            }

            return stackedRange;
        }

        private void PlotNormal(IEnumerable<ChartSeriesModel> series)
        {
            double delta = this.actualRange.maximum - this.actualRange.minimum;
            object value, transformedValue;
            NumericalAxisPlotInfoBase plotInfo;

            // update points values
            foreach (ChartSeriesModel model in series)
            {
                if (!model.presenter.IsVisible)
                {
                    continue;
                }

                foreach (DataPoint point in model.DataPointsInternal)
                {
                    if (point.isEmpty)
                    {
                        continue;
                    }

                    value = point.GetValueForAxis(this);
                    transformedValue = this.TransformValue(value);

                    if (transformedValue is double)
                    {
                        double doubleValue = (double)transformedValue;
                      
                        // TODO: Remove insignificant datapoints from drawing stack.
                        if (double.IsNaN(doubleValue))
                        {
                            plotInfo = null;                            
                        }
                        else
                        {
                            plotInfo = this.CreateAxisPlotInfo(delta, doubleValue);
                        }
                    }
                    else if (transformedValue is Ohlc)
                    {
                        Ohlc ohlcValue = (Ohlc)transformedValue;
                        plotInfo = this.CreateAxisOhlcPlotInfo(delta, ohlcValue);
                    }
                    else if (transformedValue is Range)
                    {
                        Range rangeValue = (Range)transformedValue;
                        plotInfo = this.CreateAxisRangePlotInfo(delta, rangeValue);
                    }
                    else
                    {
                        continue;
                    }

                    point.SetValueFromAxis(this, plotInfo);
                }
            }
        }

        private NumericalAxisOhlcPlotInfo CreateAxisOhlcPlotInfo(double delta, Ohlc value)
        {
            NumericalAxisOhlcPlotInfo plotInfo;
            double normalizedHigh, normalizedLow, normalizedOpen, normalizedClose;
            if (delta == 0)
            {
                normalizedHigh = 0;
                normalizedLow = 0;
                normalizedOpen = 0;
                normalizedClose = 0;
            }
            else
            {
                normalizedHigh = (value.High - this.actualRange.minimum) / delta;
                normalizedLow = (value.Low - this.actualRange.minimum) / delta;
                normalizedOpen = (value.Open - this.actualRange.minimum) / delta;
                normalizedClose = (value.Close - this.actualRange.minimum) / delta;
            }

            plotInfo = NumericalAxisOhlcPlotInfo.Create(this, this.normalizedOrigin, normalizedHigh, normalizedLow, normalizedOpen, normalizedClose, this.normalizedOrigin);
            plotInfo.SnapTickIndex = this.GetSnapTickIndex(value.High);
            plotInfo.SnapBaseTickIndex = this.GetSnapTickIndex(value.Low);
            plotInfo.SnapOpenTickIndex = this.GetSnapTickIndex(value.Open);
            plotInfo.SnapCloseTickIndex = this.GetSnapTickIndex(value.Close);

            return plotInfo;
        }

        private NumericalAxisRangePlotInfo CreateAxisRangePlotInfo(double delta, Range rangeValue)
        {
            NumericalAxisRangePlotInfo plotInfo;
            double normalizedHigh, normalizedLow;
            if (delta == 0)
            {
                normalizedHigh = 0;
                normalizedLow = 0;
            }
            else
            {
                normalizedHigh = (rangeValue.High - this.actualRange.minimum) / delta;
                normalizedLow = (rangeValue.Low - this.actualRange.minimum) / delta;
            }

            if (normalizedHigh < normalizedLow)
            {
                normalizedLow = normalizedHigh;
            }

            plotInfo = NumericalAxisRangePlotInfo.Create(this, this.normalizedOrigin, normalizedHigh, normalizedLow, this.normalizedOrigin);
            plotInfo.SnapTickIndex = this.GetSnapTickIndex(rangeValue.High);
            plotInfo.SnapBaseTickIndex = this.GetSnapTickIndex(rangeValue.Low);

            return plotInfo;
        }

        private NumericalAxisPlotInfo CreateAxisPlotInfo(double delta, double doubleValue)
        { 
            NumericalAxisPlotInfo plotInfo;
            double normalizedValue;
            if (delta == 0)
            {
                normalizedValue = 0;
            }
            else
            {
                normalizedValue = (doubleValue - this.actualRange.minimum) / delta;
            }
            
            if (this.IsInverse)
            {
                plotInfo = NumericalAxisPlotInfo.Create(this, 1 - this.normalizedOrigin, 1 - normalizedValue, 1 - this.normalizedOrigin);
            }
            else
            {
                plotInfo = NumericalAxisPlotInfo.Create(this, this.normalizedOrigin, normalizedValue, this.normalizedOrigin);
            }

            plotInfo.SnapTickIndex = this.GetSnapTickIndex(doubleValue);

            return plotInfo;
        }

        private void PlotStacked(AxisUpdateContext context)
        {
            foreach (CombinedSeries series in context.CombinedSeries)
            {
                foreach (CombineGroup group in series.Groups)
                {
                    this.PlotCombineGroup(group, (stack, value) => value);
                }
            }

            this.PlotNormal(context.NonCombinedSeries);
        }

        private void PlotStacked100(AxisUpdateContext context)
        {
            // Note: Stacked100 series cannot be combined with stacked & normal series
            // so we should not plot any non-combined series here.
            foreach (CombinedSeries series in context.CombinedSeries)
            {
                foreach (CombineGroup group in series.Groups)
                {
                    this.PlotCombineGroup(group, (stack, value) => value / (stack.PositiveSum - stack.NegativeSum));
                }
            }
        }

        private void PlotCombineGroup(CombineGroup group, StackValueProcessor stackValueProcessor)
        {
            double plotPositionPositiveStack, plotPositionNegativeStack; 
            double value, plotOriginOffset, normalizedValue;

            double stackSum;
            double valuesSum;
            double delta = this.actualRange.maximum - this.actualRange.minimum;

            foreach (CombineStack stack in group.Stacks)
            {
                double positiveValuesSum = 0d, negativeValuesSum = 0d;
                valuesSum = 0d;
                plotPositionPositiveStack = plotPositionNegativeStack = this.normalizedOrigin;

                foreach (DataPoint point in stack.Points)
                {
                    if (point.isEmpty)
                    {
                        continue;
                    }

                    value = (double)point.GetValueForAxis(this);

                    if (value >= DefaultOrigin)
                    {
                        valuesSum = positiveValuesSum;
                        plotOriginOffset = plotPositionPositiveStack;
                    }
                    else
                    {
                        valuesSum = negativeValuesSum;
                        plotOriginOffset = plotPositionNegativeStack;
                    }

                    valuesSum += double.IsNaN(value) ? 0d : value;

                    // TODO: Remove insignificant datapoints from drawing stack.
                    stackSum = this.TransformValue(stackValueProcessor(stack, valuesSum));

                    if (delta == 0)
                    {
                        normalizedValue = 0;
                    }
                    else
                    {
                        normalizedValue = (stackSum - this.actualRange.minimum) / delta;
                    }

                    NumericalAxisPlotInfo plotInfo;
                    if (this.IsInverse)
                    {
                        plotInfo = NumericalAxisPlotInfo.Create(this, 1 - plotOriginOffset, 1 - normalizedValue, 1 - this.normalizedOrigin);
                    }
                    else
                    {
                        plotInfo = NumericalAxisPlotInfo.Create(this, plotOriginOffset, normalizedValue, this.normalizedOrigin);
                    }

                    plotInfo.SnapTickIndex = this.GetSnapTickIndex(stackSum);
                    point.SetValueFromAxis(this, plotInfo);

                    if (value >= DefaultOrigin)
                    {
                        positiveValuesSum = valuesSum;
                        plotPositionPositiveStack = normalizedValue;
                    }
                    else
                    {
                        negativeValuesSum = valuesSum;
                        plotPositionNegativeStack = normalizedValue;
                    }
                }
            }
        }

        private void UpdatePlotOrigin(AxisUpdateContext context)
        {
            if (DefaultOrigin >= this.actualRange.maximum)
            {
                this.normalizedOrigin = 1;
            }
            else if (DefaultOrigin > this.actualRange.minimum)
            {
                this.normalizedOrigin = (DefaultOrigin - this.actualRange.minimum) / (this.actualRange.maximum - this.actualRange.minimum);
            }
            else
            {
                this.normalizedOrigin = 0;
            }

            // apply the plot origin to the series
            if (context.Series != null)
            {
                double plotOrigin = this.IsInverse ? 1 - this.normalizedOrigin : this.normalizedOrigin;
                foreach (ChartSeriesModel model in context.Series)
                {
                    model.SetValue(AxisModel.PlotOriginPropertyKey, plotOrigin);
                }
            }
        }

        private int GetSnapTickIndex(double value)
        {
            if (value < this.actualRange.minimum || (value % this.majorStep) != 0)
            {
                return -1;
            }

            return (int)((value - this.actualRange.minimum) / this.majorStep);
        }

        private class RangeCalculator
        {
            public double Minimum;
            public double Maximum;

            private const double DeltaPercent = 16.667 / 100;
            private const double ExtendFactor = 0.05;

            private ValueRange<double> range;
            private NumericalAxisRangeExtendDirection extendDirection;
            private bool userMin;
            private bool userMax;

            public RangeCalculator(NumericalAxisModel axis, bool userMin, bool userMax)
            {
                this.range = axis.actualRange;
                this.extendDirection = axis.extendDirection;

                this.userMin = userMin;
                this.userMax = userMax;

                this.Minimum = this.range.minimum;
                this.Maximum = this.range.maximum;
            }

            public ValueRange<double> Extend()
            {
                //// we are using the same logic as within MS Excel to calculate the min and max values of the auto-range
                //// more details at http://support.microsoft.com/kb/214075
                bool extendPositive = (this.extendDirection & NumericalAxisRangeExtendDirection.Positive) != 0;
                bool extendNegative = (this.extendDirection & NumericalAxisRangeExtendDirection.Negative) != 0;

                if (extendNegative && !this.userMin)
                {
                    this.ExtendNegative();
                }

                if (extendPositive && !this.userMax)
                {
                    this.ExtendPositive();
                }

                return new ValueRange<double>(this.Minimum, this.Maximum);
            }

            public ValueRange<double> RoundToMajorStep(double step)
            {
                double mod;
                if (!this.userMax)
                {
                    mod = this.Maximum % step;
                    if (!RadMath.IsZero(mod))
                    {
                        if (mod > 0)
                        {
                            this.Maximum += step - mod;
                        }
                        else if (mod < 0)
                        {
                            this.Maximum += step + mod;
                        }
                    }
                }

                if (!this.userMin)
                {
                    mod = this.Minimum % step;
                    if (!RadMath.IsZero(mod))
                    {
                        if (mod > 0)
                        {
                            this.Minimum -= mod;
                        }
                        else if (mod < 0)
                        {
                            this.Minimum -= step + mod;
                        }
                    }
                }

                return new ValueRange<double>(this.Minimum, this.Maximum);
            }

            private void ExtendPositive()
            {
                double delta = this.range.maximum - this.range.minimum;

                if (this.range.minimum <= 0 && this.range.maximum <= 0)
                {
                    if (delta > DeltaPercent * -this.range.minimum)
                    {
                        this.Maximum = 0;
                    }
                    else
                    {
                        this.Maximum = this.range.maximum - ((this.range.minimum - this.range.maximum) / 2);
                    }
                }
                else
                {
                    this.Maximum = this.range.maximum + (ExtendFactor * delta);
                }
            }

            private void ExtendNegative()
            {
                double delta = this.range.maximum - this.range.minimum;

                if (this.range.minimum >= 0 && this.range.maximum >= 0)
                {
                    if (delta > DeltaPercent * this.range.maximum)
                    {
                        this.Minimum = 0;
                    }
                    else
                    {
                        this.Minimum = this.range.minimum - (delta / 2);
                    }
                }
                else
                {
                    this.Minimum = this.range.minimum + (ExtendFactor * (this.range.minimum - this.range.maximum));
                }
            }
        }
    }
}