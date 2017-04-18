using System;
using System.Collections.Generic;
using System.Globalization;
using Telerik.Core;

namespace Telerik.Charting
{
    internal abstract class AxisModel : Element
    {
        internal static readonly int PlotOriginPropertyKey = PropertyKeys.Register(typeof(AxisModel), "PlotOrigin");
        internal static readonly int PlotDirectionPropertyKey = PropertyKeys.Register(typeof(AxisModel), "PlotDirection");
        internal static readonly int MajorTickLengthPropertyKey = PropertyKeys.Register(typeof(AxisModel), "MajorTickLength", ChartAreaInvalidateFlags.All);
        internal static readonly int MajorTickOffsetPropertyKey = PropertyKeys.Register(typeof(AxisModel), "MajorTickOffset", ChartAreaInvalidateFlags.All);
        internal static readonly int TickThicknessPropertyKey = PropertyKeys.Register(typeof(AxisModel), "TickThickness", ChartAreaInvalidateFlags.InvalidateAxes);
        internal static readonly int LineThicknessPropertyKey = PropertyKeys.Register(typeof(AxisModel), "LineThickness", ChartAreaInvalidateFlags.InvalidateAxes);
        internal static readonly int ShowLabelsPropertyKey = PropertyKeys.Register(typeof(AxisModel), "ShowLabels", ChartAreaInvalidateFlags.All);
        internal static readonly int LabelIntervalPropertyKey = PropertyKeys.Register(typeof(AxisModel), "LabelInterval", ChartAreaInvalidateFlags.All);
        internal static readonly int LabelOffsetPropertyKey = PropertyKeys.Register(typeof(AxisModel), "LabelOffset", ChartAreaInvalidateFlags.All);
        internal static readonly int LabelFitModePropertyKey = PropertyKeys.Register(typeof(AxisModel), "LabelFitMode", ChartAreaInvalidateFlags.All);
        internal static readonly int LabelFormatPropertyKey = PropertyKeys.Register(typeof(AxisModel), "LabelFormat", ChartAreaInvalidateFlags.All);
        internal static readonly int ContentFormatterPropertyKey = PropertyKeys.Register(typeof(AxisModel), "ContentFormatter", ChartAreaInvalidateFlags.All);
        internal static readonly int LastLabelVisibilityPropertyKey = PropertyKeys.Register(typeof(AxisModel), "LastLabelVisibility", ChartAreaInvalidateFlags.All);
        internal static readonly int HorizontalLocationPropertyKey = PropertyKeys.Register(typeof(AxisModel), "HorizontalLocation", ChartAreaInvalidateFlags.All);
        internal static readonly int VerticalLocationPropertyKey = PropertyKeys.Register(typeof(AxisModel), "VerticalLocation", ChartAreaInvalidateFlags.All);
        internal static readonly int NormalizedLabelRotationAnglePropertyKey = PropertyKeys.Register(typeof(AxisModel), "NormalizedLabelRotationAngle", ChartAreaInvalidateFlags.All);
        internal static readonly int IsInversePropertyKey = PropertyKeys.Register(typeof(AxisModel), "IsInverse", ChartAreaInvalidateFlags.All);

        internal ElementCollection<AxisTickModel> ticks;
        internal ElementCollection<AxisLabelModel> labels;
        internal AxisType type;
        internal RadRect desiredArrangeRect;
        internal RadSize desiredSize;
        internal RadThickness desiredMargin;
        internal bool isPlotValid;
        internal bool isUpdated;
        internal bool isMeasureValid;
        internal int majorTickCount;
        internal AxisTitleModel title;
        internal ValueRange<decimal> visibleRange;
        internal string labelFormat;
        internal AxisModelLayoutStrategy layoutStrategy;
        internal AxisLabelFitMode labelFitMode;
        internal bool isPrimary;
        private RadSize lastMeasureSize;

        public AxisModel()
        {
            this.TrackPropertyChanged = true;
            this.ticks = new ElementCollection<AxisTickModel>(this);
            this.labels = new ElementCollection<AxisLabelModel>(this);
            this.title = new AxisTitleModel();
            this.title.TrackPropertyChanged = true; // track changes to invalidate the chart area when content is changed

            this.children.Add(this.title);
            this.labelFormat = string.Empty;

            this.Type = AxisType.First;
        }

        public bool IsInverse
        {
            get
            {
                return this.GetTypedValue<bool>(IsInversePropertyKey, false);
            }
            set
            {
                this.SetValue(IsInversePropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the string that formats the labels of the axis.
        /// </summary>
        public string LabelFormat
        {
            get
            {
                return this.labelFormat;
            }
            set
            {
                this.SetValue(LabelFormatPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IContentFormatter"/> instance that is used to format each label's content.
        /// </summary>
        public IContentFormatter ContentFormatter
        {
            get
            {
                return this.GetTypedValue<IContentFormatter>(ContentFormatterPropertyKey, null);
            }
            set
            {
                this.SetValue(ContentFormatterPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the strategy that defines the last axis label visibility.
        /// </summary>
        public AxisLastLabelVisibility LastLabelVisibility
        {
            get
            {
                return this.GetTypedValue<AxisLastLabelVisibility>(LastLabelVisibilityPropertyKey, AxisLastLabelVisibility.Visible);
            }
            set
            {
                this.SetValue(LastLabelVisibilityPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that determines how the axis labels will be laid out when they are overlapping each other.
        /// </summary>
        public AxisLabelFitMode LabelFitMode
        {
            get
            {
                return this.labelFitMode;
            }
            set
            {
                this.SetValue(LabelFitModePropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the step at which labels are positioned.
        /// </summary>
        public int LabelInterval
        {
            get
            {
                return this.GetTypedValue<int>(LabelIntervalPropertyKey, 1);
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                this.SetValue(LabelIntervalPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets index offset from the first label to be displayed.
        /// </summary>
        public int LabelOffset
        {
            get
            {
                return this.GetTypedValue<int>(LabelOffsetPropertyKey, 0);
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                this.SetValue(LabelOffsetPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets index offset of the first tick to be displayed.
        /// </summary>
        public int MajorTickOffset
        {
            get
            {
                return this.GetTypedValue<int>(MajorTickOffsetPropertyKey, 0);
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MajorTickOffset));
                }

                this.SetValue(MajorTickOffsetPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the normalized rotation angle for the axis labels.
        /// </summary>
        public double NormalizedLabelRotationAngle
        {
            get
            {
                return this.GetTypedValue<double>(NormalizedLabelRotationAnglePropertyKey, 300d);
            }
            set
            {
                if (value < 0 || value > 360)
                {
                    throw new ArgumentOutOfRangeException(nameof(NormalizedLabelRotationAngle));
                }

                this.SetValue(NormalizedLabelRotationAnglePropertyKey, value);
            }
        }

        /// <summary>
        /// Gets the collection with all the major ticks, currently present on the axis.
        /// </summary>
        public IEnumerable<AxisTickModel> MajorTicks
        {
            get
            {
                foreach (AxisTickModel tick in this.ticks)
                {
                    if (tick.Type == TickType.Major)
                    {
                        yield return tick;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all the labels currently present on the axis.
        /// </summary>
        public ElementCollection<AxisLabelModel> Labels
        {
            get
            {
                return this.labels;
            }
        }

        /// <summary>
        /// Gets or sets the thickness of a single tick presented on the axis.
        /// </summary>
        public double TickThickness
        {
            get
            {
                return this.GetTypedValue<double>(TickThicknessPropertyKey, 1);
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Thickness may not be negative value.");
                }

                this.SetValue(TickThicknessPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether labels will be displayed on this axis.
        /// </summary>
        public bool ShowLabels
        {
            get
            {
                return this.GetTypedValue<bool>(ShowLabelsPropertyKey, true);
            }
            set
            {
                this.SetValue(ShowLabelsPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the length of a single tick presented on the axis.
        /// </summary>
        public double MajorTickLength
        {
            get
            {
                return this.GetTypedValue<double>(MajorTickLengthPropertyKey, 5);
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Thickness may not be negative value.");
                }

                this.SetValue(MajorTickLengthPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the thickness of the axis line.
        /// </summary>
        public double LineThickness
        {
            get
            {
                return this.GetTypedValue<double>(LineThicknessPropertyKey, 1);
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Thickness may not be negative value.");
                }

                this.SetValue(LineThicknessPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal location of an axis (rendered vertically) in relation to the plot area.
        /// </summary>
        /// <value>The horizontal location.</value>
        public AxisHorizontalLocation HorizontalLocation
        {
            get
            {
                return this.GetTypedValue<AxisHorizontalLocation>(HorizontalLocationPropertyKey, AxisHorizontalLocation.Left);
            }
            set
            {
                this.SetValue(HorizontalLocationPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical location of an axis (rendered horizontally) in relation to the plot area.
        /// </summary>
        /// <value>The vertical location.</value>
        public AxisVerticalLocation VerticalLocation
        {
            get
            {
                return this.GetTypedValue<AxisVerticalLocation>(VerticalLocationPropertyKey, AxisVerticalLocation.Bottom);
            }
            set
            {
                this.SetValue(VerticalLocationPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets the first tick present on the axis. Valid when the axis is loaded.
        /// </summary>
        public AxisTickModel FirstTick
        {
            get
            {
                if (this.ticks.Count > 0)
                {
                    return this.ticks[0];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the first tick present on the axis. Valid when the axis is loaded.
        /// </summary>
        public AxisTickModel LastTick
        {
            get
            {
                if (this.ticks.Count > 1)
                {
                    return this.ticks[this.ticks.Count - 1];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets the type (X or Y) of this instance.
        /// </summary>
        public AxisType Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
                this.UpdateLayoutStrategy();
            }
        }

        /// <summary>
        /// Gets the <see cref="AxisTitleModel"/> instance that represents the title of this axis.
        /// </summary>
        public AxisTitleModel Title
        {
            get
            {
                return this.title;
            }
        }

        internal virtual bool SupportsCombinedPlot
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the plot mode actually used by this axis.
        /// </summary>
        internal virtual AxisPlotMode ActualPlotMode
        {
            get
            {
                return AxisPlotMode.OnTicks;
            }
        }

        /// <summary>
        /// Gets the collection with all the tick currently present on this axis.
        /// </summary>
        internal ElementCollection<AxisTickModel> Ticks
        {
            get
            {
                return this.ticks;
            }
        }

        /// <summary>
        /// Performs pixel-snapping and corrects floating-point calculations errors.
        /// </summary>
        internal override void ApplyLayoutRounding()
        {
            this.layoutStrategy.ApplyLayoutRounding();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            // update local value first and then call base to raise the PropertyChanged event (if needed)
            if (e.Key == LabelFormatPropertyKey)
            {
                this.labelFormat = (string)e.NewValue;
            }
            else if (e.Key == LabelFitModePropertyKey)
            {
                this.labelFitMode = (AxisLabelFitMode)e.NewValue;
            }

            base.OnPropertyChanged(e);
        }

        internal virtual double CalculateRelativePosition(double coordinate, RadRect axisVirtualSize, double stepOffset = 0)
        {
            double position;

            if (this.type == AxisType.First)
            {
                position = (coordinate - axisVirtualSize.X) / axisVirtualSize.Width + stepOffset;
            }
            else
            {
                position = 1 - (coordinate - axisVirtualSize.Y) / axisVirtualSize.Height - stepOffset;
            }

            if (this.IsInverse)
            {
                position = 1 - position;
            }

            return position;
        }

        internal virtual void Reset()
        {
            this.ResetState();
            this.ticks.Clear();
            this.labels.Clear();
        }

        internal virtual void ResetState()
        {
            this.isPlotValid = false;
            this.isUpdated = false;
            this.isMeasureValid = false;
            this.visibleRange = new ValueRange<decimal>(-1, -1);
        }

        internal virtual void OnZoomChanged()
        {
            this.isMeasureValid = false;
        }

        internal virtual void OnPlotOriginChanged()
        {
            this.isMeasureValid = false;
        }

        internal virtual AxisModelLayoutStrategy CreateLayoutStrategy()
        {
            if (this.type == AxisType.First)
            {
                return new HorizontalAxisLayoutStrategy();
            }

            return new VerticalAxisLayoutStrategy();
        }

        internal void Update(AxisUpdateContext context)
        {
            if (this.isUpdated)
            {
                return;
            }

            this.UpdateCore(context);
            this.isUpdated = true;
        }

        internal void Plot(AxisUpdateContext context)
        {
            if (!this.isPlotValid)
            {
                // actual points plot
                this.PlotCore(context);
                this.isPlotValid = true;
            }
        }

        internal void UpdateTicksVisibility(RadRect clipRect)
        {
            this.layoutStrategy.UpdateTicksVisibility(clipRect);
        }

        /// <summary>
        /// Gets the key used to group series when combination mode like Stack is specified.
        /// </summary>
        internal virtual object GetCombineGroupKey(DataPoint point)
        {
            return point.CollectionIndex + 1;
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
        internal virtual bool TryGetStackSumValue(DataPoint point, out double transformedStackSumValue, out bool positive, ref double positiveValuesSum, ref double negativeValuesSum)
        {
            positive = false;
            transformedStackSumValue = double.NaN;

            return false;
        }

        internal virtual void PlotCore(AxisUpdateContext context)
        {
        }

        internal virtual void UpdateCore(AxisUpdateContext context)
        {
        }

        internal abstract IEnumerable<AxisTickModel> GenerateTicks(ValueRange<decimal> currentVisibleRange);

        internal virtual IEnumerable<AxisLabelModel> GenerateLabels()
        {
            AxisPlotMode plotMode = this.ActualPlotMode;
            int labelIndex = 0;
            int startIndex = this.LabelOffset;
            int labelStep = this.LabelInterval;
            int skipLabelCount = 1;

            IContentFormatter labelFormatter = this.ContentFormatter;
            object owner = this.Presenter;
            string format = this.GetLabelFormat();

            // generate label for each major tick
            foreach (AxisTickModel tick in this.ticks)
            {
                if (labelIndex < startIndex)
                {
                    labelIndex++;
                    continue;
                }

                // skip minor ticks
                if (tick.Type == TickType.Minor)
                {
                    continue;
                }

                if (skipLabelCount > 1)
                {
                    skipLabelCount--;
                    continue;
                }

                // no need to process last tick if we are plotting between ticks
                if (plotMode == AxisPlotMode.BetweenTicks && RadMath.IsOne(this.IsInverse ? 1 - tick.normalizedValue : tick.normalizedValue))
                {
                    break;
                }

                AxisLabelModel label = new AxisLabelModel();
                object content = this.GetLabelContent(tick);
                if (labelFormatter != null)
                {
                    content = labelFormatter.Format(owner, content);
                }
                else if (!string.IsNullOrEmpty(format))
                {
                    content = string.Format(CultureInfo.CurrentUICulture, format, content);
                }
                label.Content = content;
                tick.associatedLabel = label;

                if (plotMode == AxisPlotMode.BetweenTicks)
                {
                    decimal length = tick.NormalizedForwardLength;
                    if (length == 0)
                    {
                        length = tick.NormalizedBackwardLength;
                    }
                    tick.associatedLabel.normalizedPosition = tick.normalizedValue + (length / 2);
                }
                else
                {
                    tick.associatedLabel.normalizedPosition = tick.normalizedValue;
                }               

                yield return label;

                skipLabelCount = labelStep;
            }
        }

        internal virtual string GetLabelFormat()
        {
            return this.labelFormat;
        }

        internal virtual object GetLabelContent(AxisTickModel tick)
        {
            return tick.value;
        }

        internal virtual bool Measure(RadSize availableSize)
        {
            if (this.presenter == null || !this.presenter.IsVisible)
            {
                this.desiredSize = RadSize.Empty;
                this.desiredArrangeRect = RadRect.Empty;
                return false;
            }

            if (this.lastMeasureSize != availableSize)
            {
                this.isMeasureValid = false;
            }

            if (this.isMeasureValid)
            {
                return false;
            }

            this.lastMeasureSize = availableSize;
            this.MeasureCore(availableSize);
            this.isMeasureValid = true;

            return true;
        }

        internal override RadRect ArrangeOverride(RadRect availableRect)
        {
            if (this.presenter == null || !this.presenter.IsVisible)
            {
                this.desiredArrangeRect = RadRect.Empty;
                return availableRect;
            }
            RadSize availableSize = new RadSize(availableRect.Width, availableRect.Height);
            if (availableSize != this.lastMeasureSize)
            {
                this.Measure(availableSize);
            }
            this.layoutStrategy.Arrange(availableRect);
            this.desiredArrangeRect = availableRect;
            return availableRect;
        }

        internal virtual double GetTickLength(AxisTickModel tick)
        {
            return this.MajorTickLength;
        }

        internal override ModifyChildrenResult CanAddChild(Node child)
        {
            if (child == this.title || child is AxisTickModel || child is AxisLabelModel)
            {
                return ModifyChildrenResult.Accept;
            }

            return ModifyChildrenResult.Refuse;
        }

        // TODO: Is there a better way for plotInfos creation for annotations?
        internal virtual AxisPlotInfo CreatePlotInfo(object value)
        {
            return null;
        }

        internal abstract object ConvertPhysicalUnitsToData(double coordinatePlusOffset, RadRect axisVirtualSize);

        protected void UpdateLayoutStrategy()
        {
            this.layoutStrategy = this.CreateLayoutStrategy();
            this.layoutStrategy.owner = this;
        }

        protected virtual bool BuildTicksAndLabels(RadSize availableSize)
        {
            ValueRange<decimal> newVisibleRange = this.layoutStrategy.GetVisibleRange(availableSize);
            if (newVisibleRange == this.visibleRange)
            {
                return false;
            }

            this.visibleRange = newVisibleRange;

            // clear labels first, they are at the end of the children collection (performance improvement)
            this.labels.Clear();
            this.ticks.Clear();

            // prepare ticks
            this.UpdateTicks();

            // update labels
            this.UpdateLabels();

            return true;
        }

        private void MeasureCore(RadSize availableSize)
        {
            this.BuildTicksAndLabels(availableSize);

            foreach (AxisLabelModel label in this.labels)
            {
                // We may enter one or more additional measure passes until all axes are best fit,
                // so do not re-measure already measured labels.
                if (label.desiredSize == RadSize.Empty)
                {
                    label.desiredSize = this.presenter.MeasureContent(label, label.Content);
                }
            }
            if (this.title.desiredSize == RadSize.Empty)
            {
                this.title.desiredSize = this.presenter.MeasureContent(this.title, this.title.Content);
            }
            this.desiredSize = this.layoutStrategy.GetDesiredSize(availableSize);
            this.desiredMargin = this.layoutStrategy.GetDesiredMargin(availableSize);
        }

        private void UpdateTicks()
        {
            this.majorTickCount = 0;
            AxisTickModel previous = null;
            int tickIndex = 0;
            int startIndex = this.MajorTickOffset;

            foreach (AxisTickModel tick in this.GenerateTicks(this.visibleRange))
            {
                // consider tick offset
                if (tickIndex < startIndex)
                {
                    tickIndex++;
                    continue;
                }

                tick.position = TickPosition.Inner;
                this.ticks.Add(tick);
                tick.previous = previous;

                if (previous != null)
                {
                    previous.next = tick;
                }

                previous = tick;
                if (tick.Type == TickType.Major)
                {
                    this.majorTickCount++;
                }
            }

            if (this.ticks.Count > 0)
            {
                this.ticks[0].position = TickPosition.First;
            }
            if (this.ticks.Count > 1)
            {
                this.ticks[this.ticks.Count - 1].position = TickPosition.Last;
            }
        }

        private void UpdateLabels()
        {
            if (!this.ShowLabels)
            {
                return;
            }

            foreach (AxisLabelModel label in this.GenerateLabels())
            {
                this.labels.Add(label);
            }

            if (this.labels.Count > 1 && this.LastLabelVisibility == AxisLastLabelVisibility.Hidden)
            {
                this.labels[this.labels.Count - 1].isVisible = false;
            }
        }
    }
}