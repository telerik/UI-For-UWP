using System;
using System.Collections.Generic;
using System.Globalization;
using Telerik.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Primitives.Scale
{
    internal class NumericalAxisModel : RootElement
    {
        internal static readonly int MinimumPropertyKey = PropertyKeys.Register(typeof(NumericalAxisModel), "Minimum");
        internal static readonly int MaximumPropertyKey = PropertyKeys.Register(typeof(NumericalAxisModel), "Maximum");
        internal static readonly int TickFrequencyPropertyKey = PropertyKeys.Register(typeof(NumericalAxisModel), "TickFrequency");
        internal static readonly int OrientationPropertyKey = PropertyKeys.Register(typeof(NumericalAxisModel), "Orientation");
        internal static readonly int TickPlacementPropertyKey = PropertyKeys.Register(typeof(NumericalAxisModel), "TickPlacement");
        internal static readonly int LabelFormatPropertyKey = PropertyKeys.Register(typeof(NumericalAxisModel), "LabelFormat");
        internal static readonly int LineThicknessPropertyKey = PropertyKeys.Register(typeof(NumericalAxisModel), "LineThickness");
        internal static readonly int LabelPlacementPropertyKey = PropertyKeys.Register(typeof(NumericalAxisModel), "LabelPlacement");

        internal static readonly int TickThicknessPropertyKey = PropertyKeys.Register(typeof(NumericalAxisModel), "TickThickness");
        internal static readonly int TickLengthPropertyKey = PropertyKeys.Register(typeof(NumericalAxisModel), "TickLength");

        internal RadSize tickSize;

        private RadLine line;
        private ElementCollection<AxisTickModel> ticks;
        private ElementCollection<AxisLabelModel> labels;
        private RadSize desiredSize;

        private double minimumCache = ScalePrimitive.DefaultMinimum;
        private double maximumCache = ScalePrimitive.DefaultMaximum;
        private double tickFrequencyCache = ScalePrimitive.DefaultTickFrequency;
        private double lineThicknessCache = 1;
        private double tickThicknessCache = 1;
        private double tickLengthCache = 5;
        private ScaleElementPlacement tickPlacementCache = ScalePrimitive.DefaultTickPlacement;
        private ScaleElementPlacement labelPlacementCache = ScalePrimitive.DefaultTickPlacement;
        private Orientation orientationCache = ScalePrimitive.DefaultOrientation;
        private string labelFormatCache;

        private Thickness axisLineOffset;
        private double maxLabelHeight;
        private double maxLabelWidth;
        private ScaleLayoutMode layoutMode = ScaleLayoutMode.ShortenAxisLine;
        private RadSize lastMeasureSize;
        private RadSize labelSize;

        private AxisModelLayoutStrategy layoutStrategy;

        public NumericalAxisModel()
        {
            this.TrackPropertyChanged = true;

            this.ticks = new ElementCollection<AxisTickModel>(this);
            this.labels = new ElementCollection<AxisLabelModel>(this);

            this.UpdateLayoutStrategy();
        }

        public double Minimum
        {
            get
            {
                return this.minimumCache;
            }
            set
            {
                this.SetValue(MinimumPropertyKey, value);
            }
        }

        public double Maximum
        {
            get
            {
                return this.maximumCache;
            }
            set
            {
                this.SetValue(MaximumPropertyKey, value);
            }
        }

        public double TickFrequency
        {
            get
            {
                return this.tickFrequencyCache;
            }
            set
            {
                this.SetValue(TickFrequencyPropertyKey, value);
            }
        }

        public string LabelFormat
        {
            get
            {
                return this.labelFormatCache;
            }
            set
            {
                this.SetValue(LabelFormatPropertyKey, value);
            }
        }

        public double LineThickness
        {
            get
            {
                return this.lineThicknessCache;
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

        public double TickThickness
        {
            get
            {
                return this.tickThicknessCache;
            }
            set
            {
                this.SetValue(TickThicknessPropertyKey, value);
            }
        }

        public double TickLength
        {
            get
            {
                return this.tickLengthCache;
            }
            set
            {
                this.SetValue(TickLengthPropertyKey, value);
            }
        }

        public ScaleElementPlacement TickPlacement
        {
            get
            {
                return this.tickPlacementCache;
            }
            set
            {
                this.SetValue(TickPlacementPropertyKey, value);
            }
        }

        public ScaleElementPlacement LabelPlacement
        {
            get
            {
                return this.labelPlacementCache;
            }
            set
            {
                this.SetValue(LabelPlacementPropertyKey, value);
            }
        }

        public Orientation Orientation
        {
            get
            {
                return this.orientationCache;
            }
            set
            {
                this.SetValue(OrientationPropertyKey, value);
            }
        }

        public Thickness AxisLineOffset
        {
            get
            {
                return this.axisLineOffset;
            }
        }

        internal RadLine Line
        {
            get
            {
                return this.line;
            }
        }

        internal ElementCollection<AxisTickModel> Ticks
        {
            get
            {
                return this.ticks;
            }
        }

        internal ElementCollection<AxisLabelModel> Labels
        {
            get
            {
                return this.labels;
            }
        }

        internal RadSize DesiredSize
        {
            get
            {
                return this.desiredSize;
            }
        }

        internal double LineThicknessCache
        {
            get
            {
                return this.lineThicknessCache;
            }
        }

        internal ScaleElementPlacement TickPlacementCache
        {
            get
            {
                return this.tickPlacementCache;
            }
        }

        internal ScaleElementPlacement LabelPlacementCache
        {
            get
            {
                return this.labelPlacementCache;
            }
        }

        internal double MaxLabelHeight
        {
            get
            {
                return this.maxLabelHeight;
            }
        }

        internal double MaxLabelWidth
        {
            get
            {
                return this.maxLabelWidth;
            }
        }

        internal RadSize LabelSize
        {
            get
            {
                return this.labelSize;
            }
        }

        internal ScaleLayoutMode LayoutMode
        {
            get
            {
                return this.layoutMode;
            }
        }

        internal override void PreviewMessage(Message msg)
        {
            base.PreviewMessage(msg);

            this.View.RefreshNode(null);
        }

        internal void Measure(RadSize availableSize)
        {
            this.lastMeasureSize = availableSize;
            this.MeasureCore();

            this.UpdateDesiredSize(availableSize);

            this.UpdateAxisLineOffset();
        }

        internal override RadRect ArrangeOverride(RadRect rect)
        {
            RadRect arrangedRect = base.ArrangeOverride(rect);

            this.line = this.layoutStrategy.ArrangeLine(arrangedRect);
            this.layoutStrategy.ArrangeTicks();
            this.layoutStrategy.ArrangeLabels(arrangedRect);

            return arrangedRect;
        }

        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e == null)
            {
                return;
            }

            if (e.Key == MinimumPropertyKey)
            {
                this.minimumCache = (double)e.NewValue;
                this.UpdateTickFrequency();
                this.ResetTicks();
                this.ResetLabels();
            }
            else if (e.Key == MaximumPropertyKey)
            {
                this.maximumCache = (double)e.NewValue;
                this.UpdateTickFrequency();
                this.ResetTicks();
                this.ResetLabels();
            }
            else if (e.Key == TickFrequencyPropertyKey)
            {
                this.tickFrequencyCache = (double)e.NewValue;
                this.UpdateTickFrequency();
                this.ResetTicks();
            }
            else if (e.Key == LabelFormatPropertyKey)
            {
                this.labelFormatCache = (string)e.NewValue;
                this.ResetLabels();
            }
            else if (e.Key == LineThicknessPropertyKey)
            {
                this.lineThicknessCache = (double)e.NewValue;
            }
            else if (e.Key == TickPlacementPropertyKey)
            {
                this.tickPlacementCache = (ScaleElementPlacement)e.NewValue;
                this.ResetTicks();
            }
            else if (e.Key == TickThicknessPropertyKey)
            {
                this.tickThicknessCache = (double)e.NewValue;
            }
            else if (e.Key == TickLengthPropertyKey)
            {
                this.tickLengthCache = (double)e.NewValue;
            }
            else if (e.Key == LabelPlacementPropertyKey)
            {
                this.labelPlacementCache = (ScaleElementPlacement)e.NewValue;
                this.ResetLabels();
            }
            else if (e.Key == OrientationPropertyKey)
            {
                this.orientationCache = (Orientation)e.NewValue;
                this.UpdateLayoutStrategy();
            }
        }

        internal override ModifyChildrenResult CanAddChild(Node child)
        {
            if (child is AxisLabelModel || child is AxisTickModel)
            {
                return ModifyChildrenResult.Accept;
            }

            return ModifyChildrenResult.Refuse;
        }

        internal void ResetLabels()
        {
            this.labels.Clear();
        }

        private void UpdateDesiredSize(RadSize availableSize)
        {
            this.desiredSize = this.layoutStrategy.UpdateDesiredSize(availableSize);
        }

        private void MeasureCore()
        {
            this.BuildTicksAndLabels();

            this.tickSize = RadSize.Empty;

            this.maxLabelWidth = 0;
            this.maxLabelHeight = 0;

            // measure ticks to calculate tick size  
            foreach (var tick in this.ticks)
            {
                // Go through all the ticks and call the Measure routine on the presenter, which will generate VisualElement for each tick.
                // This is important to be be done in the Measure cycle to ensure properly build Visual tree.
                this.tickSize = this.presenter.MeasureContent(tick, null);
            }

            this.labelSize = RadSize.Empty;

            foreach (AxisLabelModel label in this.labels)
            {
                // We may enter one or more additional measure passes until all axes are best fit,
                // so do not re-measure already measured labels.
                if (label.desiredSize == RadSize.Empty)
                {
                    label.desiredSize = this.presenter.MeasureContent(label, label.Content);
                }

                this.maxLabelWidth = Math.Max(this.maxLabelWidth, label.desiredSize.Width);
                this.maxLabelHeight = Math.Max(this.maxLabelHeight, label.desiredSize.Height);

                this.labelSize.Width += label.desiredSize.Width;
                this.labelSize.Height += label.desiredSize.Height;
            }
        }

        private void UpdateAxisLineOffset()
        {
            //// This method updates the axisLineOffset. 
            //// In horizontal orientation the left axis line offset is the offset in pixels which
            //// determine where exaclty is the Minimum value. This offset depends on the tick thickness 
            //// and first label width (if the labels are visible and the layout mode is ShortenAxisLine).
            //// To calculate this offset, we need to arrange the line first. The RadRect which we use for this
            //// can be any RadRect with valid values, even (0, 0, 0, 0).

            RadRect rect = new RadRect();
            RadLine arrangedLine = this.layoutStrategy.ArrangeLine(rect);

            double halfTickThickness = this.tickPlacementCache == ScaleElementPlacement.None ? 0 : (this.TickThickness / 2);
            if (this.orientationCache == Orientation.Horizontal)
            {
                this.axisLineOffset = new Thickness((int)(arrangedLine.X1 + halfTickThickness), 0, (int)(rect.Right - arrangedLine.X2 + halfTickThickness), 0);
            }
            else
            {
                this.axisLineOffset = new Thickness(0, (int)(arrangedLine.Y1 + halfTickThickness), 0, (int)(rect.Bottom - arrangedLine.Y2 + halfTickThickness));
            }
        }

        private void UpdateTickFrequency()
        {
            double localValue = (double)this.GetTypedValue<double>(TickFrequencyPropertyKey, double.NaN);
            if (double.IsNaN(localValue))
            {
                double range = this.Maximum - this.Minimum;
                double step = range / 10;
                if (step > 0)
                {
                    this.tickFrequencyCache = step;
                }
            }
        }

        private void ResetTicks()
        {
            this.ticks.Clear();
        }

        private IEnumerable<AxisTickModel> GenerateTicks()
        {
            if (this.tickPlacementCache == ScaleElementPlacement.None)
            {
                yield break;
            }

            decimal min = Math.Round((decimal)this.minimumCache, 4);
            decimal max = Math.Round((decimal)this.maximumCache, 4);
            decimal step = (decimal)this.tickFrequencyCache;

            decimal delta = max - min;
            if (delta > 0 && step > 0)
            {
                for (int i = 0;; i++)
                {
                    decimal value = min + i * Math.Round(step, 4);
                    if (max < value)
                    {
                        break;
                    }

                    AxisTickModel tick = new AxisTickModel();
                    tick.normalizedValue = (value - min) / delta;
                    yield return tick;
                }
            }
        }

        private IEnumerable<AxisLabelModel> GenerateLabels()
        {
            double range = this.maximumCache - this.minimumCache;
            if (range > 0 && this.LabelPlacement != ScaleElementPlacement.None)
            {
                yield return this.BuildAxisLabelModel(this.minimumCache);
                yield return this.BuildAxisLabelModel(this.maximumCache);
            }
        }

        private AxisLabelModel BuildAxisLabelModel(object content)
        {
            AxisLabelModel label = new AxisLabelModel();
            if (!string.IsNullOrEmpty(this.labelFormatCache))
            {
                content = string.Format(CultureInfo.CurrentUICulture, this.labelFormatCache, content);
            }
            label.Content = content;
            return label;
        }

        private void BuildTicksAndLabels()
        {
            if (this.ticks.Count == 0)
            {
                foreach (var item in this.GenerateTicks())
                {
                    this.ticks.Add(item);
                }
            }

            if (this.labels.Count == 0)
            {
                foreach (var item in this.GenerateLabels())
                {
                    this.labels.Add(item);
                }
            }
        }

        private void UpdateLayoutStrategy()
        {
            if (this.orientationCache == Orientation.Horizontal)
            {
                this.layoutStrategy = new HorizontalAxisLayoutStrategy(this);
            }
            else
            {
                this.layoutStrategy = new VerticalAxisLayoutStrategy(this);
            }
        }
    }
}
