using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// Base class for the gauge ranges. Defines a value range (min and max value), tick and label steps and templates for the ticks and labels.
    /// The class is abstract because the layout of the ticks and labels are defined in concrete types, for example the radial and linear ranges.
    /// </summary>
    public abstract class GaugePanel : Panel
    {
        private Canvas indicatorContainer = new Canvas();

        private List<ContentPresenter> ticks = new List<ContentPresenter>();
        private List<ContentPresenter> labels = new List<ContentPresenter>();

        private bool isMeasurePassed = false;
        private bool isUpdateScheduled;
        private Size measureSize;
        private Size arrangeSize;

        private RadGauge ownerGauge;

        /// <summary>
        /// Initializes a new instance of the <see cref="GaugePanel"/> class.
        /// </summary>
        protected GaugePanel()
        {
            this.Children.Add(this.indicatorContainer);
        }

        internal RadGauge OwnerGauge
        {
            get
            {
                return this.ownerGauge;
            }
            set
            {
                this.ownerGauge = value;

                this.UpdateIndicatorContainerZIndex(this.ownerGauge.IndicatorsZIndex);
            }
        }

        internal Size LastMeasureSize
        {
            get
            {
                return this.measureSize;
            }
        }

        /// <summary>
        /// Gets the count of the currently created ticks.
        /// </summary>
        /// <returns>Returns the count of the currently created ticks.</returns>
        internal int TickCount
        {
            get
            {
                return this.ticks.Count;
            }
        }

        internal bool IsMeasurePassed
        {
            get
            {
                return this.isMeasurePassed;
            }
        }

        /// <summary>
        /// Gets the count of the currently created labels.
        /// </summary>
        /// <returns>Returns the count of the currently created labels.</returns>
        internal int LabelCount
        {
            get
            {
                return this.labels.Count;
            }
        }

        internal UIElementCollection Indicators
        {
            get
            {
                return this.indicatorContainer.Children;
            }
        }

        internal void ScheduleUpdate()
        {
            if (this.isUpdateScheduled || !this.isMeasurePassed)
            {
                return;
            }

            if (DesignMode.DesignModeEnabled)
            {
                this.ResetTicksAndLabels();
                this.InvalidateMeasure();
            }
            else
            {
                this.isUpdateScheduled = true;

                var warningSuppression = this.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () =>
                    {
                        this.ResetTicksAndLabels();
                        this.InvalidateMeasure();

                        this.isUpdateScheduled = false;
                    });
            }
        }

        internal void UpdateOnMinMaxValueChange()
        {
            this.ScheduleUpdate();
        }

        internal void ResetAndArrangeTicks()
        {
            if (!this.isMeasurePassed)
            {
                return;
            }

            this.ResetTicks();
            this.ArrangeTicks(this.arrangeSize);
        }

        internal void ResetAndArrangeLabels()
        {
            if (!this.isMeasurePassed)
            {
                return;
            }

            this.ResetLabels();
            this.ArrangeLabels(this.arrangeSize);
        }

        internal void UpdateTickTemplates(DataTemplate newTemplate, TickType tickType)
        {
            if (!this.isMeasurePassed)
            {
                return;
            }

            DataTemplate minorTemplate = this.ownerGauge.TickTemplate;
            List<ContentPresenter> presenters = tickType == TickType.Label ? this.labels : this.ticks;

            foreach (ContentPresenter tick in presenters)
            {
                TickType currentTickType = (TickType)tick.GetValue(RadGauge.TickTypeProperty);

                if (currentTickType == tickType)
                {
                    if (newTemplate != null)
                    {
                        tick.ContentTemplate = newTemplate;
                    }
                    else
                    {
                        tick.ContentTemplate = minorTemplate;
                    }
                }
            }
        }

        internal void UpdateIndicatorContainerZIndex(int zIndex)
        {
            Canvas.SetZIndex(this.indicatorContainer, zIndex);
        }

        /// <summary>
        /// Defines the arrange logic for the ticks.
        /// </summary>
        /// <param name="finalSize">The size in which the ticks can be arranged.</param>
        internal abstract void ArrangeTicksOverride(Size finalSize);

        /// <summary>
        /// Defines the arrange logic for the labels.
        /// </summary>
        /// <param name="finalSize">The size in which the labels can be arranged.</param>
        internal abstract void ArrangeLabelsOverride(Size finalSize);

        /// <summary>
        /// Gets a tick at the specified index.
        /// </summary>
        /// <param name="index">The index at which the desired tick is.</param>
        /// <returns>A GaugeTick.</returns>
        internal ContentPresenter GetTick(int index)
        {
            return this.ticks[index];
        }

        /// <summary>
        /// Gets a label at the specified index.
        /// </summary>
        /// <param name="index">The index at which the desired label is.</param>
        /// <returns>A GaugeTick which represents a label.</returns>
        internal ContentPresenter GetLabel(int index)
        {
            return this.labels[index];
        }

        /// <summary>
        /// A virtual method that is called for each indicator in the arrange layout pass.
        /// </summary>
        /// <param name="indicator">The indicator to arrange.</param>
        /// <param name="finalSize">The size in which to arrange the indicator.</param>
        /// <remarks>
        /// The Rect in which the indicator will be arranged should be retrieved from
        /// the indicator itself via the GetArrangeRect() method.
        /// </remarks>
        internal virtual void ArrangeIndicator(GaugeIndicator indicator, Size finalSize)
        {
            Rect rect = indicator.GetArrangeRect(finalSize);
            indicator.Arrange(rect);
        }

        /// <summary>
        /// Called in the measure layout pass to determine the desired size.
        /// </summary>
        /// <param name="availableSize">The available size that was given by the layout system.</param>
        /// <returns>Returns the desired size of the panel.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.ownerGauge == null)
            {
                Debug.Assert(false, "Must have an owner gauge reference at this point.");
                return base.MeasureOverride(availableSize);
            }

            Size desired = RadGauge.NormalizeSize(availableSize);

            if (this.arrangeSize.Width > 0 || this.arrangeSize.Height > 0)
            {
                this.measureSize = this.arrangeSize;
            }
            else
            {
                this.measureSize = desired;
            }

            if (this.ticks.Count == 0)
            {
                this.CreateTicks();
            }

            if (this.labels.Count == 0)
            {
                this.CreateLabels();
            }

            foreach (GaugeIndicator indicator in this.indicatorContainer.Children)
            {
                indicator.Owner = this;
                if (indicator.LastMeasureSize == this.measureSize || indicator.DesiredSize == this.measureSize)
                {
                    indicator.InvalidateMeasure();
                }
                indicator.Measure(this.measureSize);
            }
            this.indicatorContainer.Measure(this.measureSize);

            foreach (ContentPresenter tick in this.ticks)
            {
                tick.Measure(this.measureSize);
            }

            foreach (ContentPresenter label in this.labels)
            {
                label.Measure(this.measureSize);
            }

            this.isMeasurePassed = true;

            return desired;
        }

        /// <summary>
        /// Called in the arrange pass of the layout system.
        /// </summary>
        /// <param name="finalSize">The final size that was given by the layout system.</param>
        /// <returns>The final size of the panel.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.ownerGauge == null)
            {
                Debug.Assert(false, "Must have an owner gauge reference at this point.");
                return base.ArrangeOverride(finalSize);
            }

            if (this.isUpdateScheduled)
            {
                return finalSize;
            }

            if (finalSize.Width > 0 && finalSize.Height > 0 && finalSize != this.measureSize)
            {
                // new Measure pass, with the proper final size
                this.arrangeSize = finalSize;
                this.InvalidateMeasure();

                return finalSize;
            }

            this.arrangeSize = new Size(0, 0);

            this.ArrangeTicks(finalSize);
            this.ArrangeLabels(finalSize);

            this.indicatorContainer.Arrange(new Rect(new Point(), this.indicatorContainer.DesiredSize));

            foreach (GaugeIndicator indicator in this.Indicators)
            {
                this.ArrangeIndicator(indicator, finalSize);
            }

            return finalSize;
        }

        private static void InitializeTick(ContentPresenter tick, double value, DataTemplate dataTemplate)
        {
            tick.ContentTemplate = dataTemplate;
            tick.SetValue(RadGauge.TickValueProperty, value);

            // The default value formatter displayes 0d as 0.0000000, that is why we are calling ToString() explicitly
            tick.Content = value.ToString(CultureInfo.CurrentUICulture.NumberFormat);
        }

        private void ArrangeTicks(Size finalSize)
        {
            if (this.ownerGauge.TickStep > 0)
            {
                this.ArrangeTicksOverride(finalSize);
            }
        }

        private void ArrangeLabels(Size finalSize)
        {
            if (this.ownerGauge.LabelStep > 0)
            {
                this.ArrangeLabelsOverride(finalSize);
            }
        }

        private void UpdateIndicators()
        {
            foreach (GaugeIndicator indicator in this.Indicators)
            {
                indicator.Owner = this;
                indicator.Update(indicator.DesiredSize);
            }
        }

        private void ResetTicksAndLabels()
        {
            this.ResetTicks();
            this.ResetLabels();

            if (this.isMeasurePassed)
            {
                this.InvalidateArrange();
            }
        }

        private void CreateTicks()
        {
            double tickValueStep = this.ownerGauge.TickStep;
            if (tickValueStep <= 0)
            {
                return;
            }

            double min = this.ownerGauge.MinValue;
            double max = this.ownerGauge.MaxValue;

            double minMaxDiff = max - min;

            tickValueStep *= Math.Sign(minMaxDiff);

            DataTemplate tickTemplate = this.ownerGauge.TickTemplate;
            DataTemplate middleTemplate = this.ownerGauge.MiddleTickTemplate;
            DataTemplate majorTemplate = this.ownerGauge.MajorTickTemplate;

            int middleTickStep = this.ownerGauge.MiddleTickStep;
            int majorTickStep = this.ownerGauge.MajorTickStep;
            int j = 0;
            double tickValue = 0;

            double absMinMaxDiff = Math.Abs(minMaxDiff);
            int tickCount = (int)(absMinMaxDiff / Math.Abs(tickValueStep));

            for (int k = 0; k <= tickCount; k++, tickValue += tickValueStep, j++)
            {
                this.InitializeTick(tickValue, j, middleTickStep, majorTickStep, tickTemplate, middleTemplate, majorTemplate);
            }
        }

        private void InitializeTick(double i, double j, double middleTickStep, double majorTickStep, DataTemplate tickTemplate, DataTemplate middleTemplate, DataTemplate majorTemplate)
        {
            DataTemplate template = tickTemplate;
            TickType type = TickType.Minor;

            if (middleTickStep != -1 && j % middleTickStep == 0)
            {
                template = middleTemplate;
                type = TickType.Middle;
            }

            if (majorTickStep != -1 && j % majorTickStep == 0)
            {
                template = majorTemplate;
                type = TickType.Major;
            }

            ContentPresenter tick = new ContentPresenter();
            tick.SetValue(RadGauge.TickTypeProperty, type);

            InitializeTick(tick, i, template);
            this.ticks.Add(tick);
            this.Children.Insert(0, tick);
        }

        private void CreateLabels()
        {
            double labelValueStep = this.ownerGauge.LabelStep;
            if (labelValueStep <= 0)
            {
                return;
            }

            double min = this.ownerGauge.MinValue;
            double max = this.ownerGauge.MaxValue;

            double minMaxDiff = max - min;
            labelValueStep *= Math.Sign(minMaxDiff);
            double absMinMaxDiff = Math.Abs(minMaxDiff);

            int labelCount = (int)(absMinMaxDiff / Math.Abs(labelValueStep));

            double i = min;

            for (double j = 0; j <= labelCount; j++, i += labelValueStep)
            {
                this.InitializeLabel(i);
            }
        }

        private void InitializeLabel(double i)
        {
            ContentPresenter label = new ContentPresenter();
            label.SetValue(RadGauge.TickTypeProperty, TickType.Label);

            InitializeTick(label, i, this.ownerGauge.LabelTemplate);
            this.labels.Add(label);
            this.Children.Insert(0, label);
        }

        private void ResetLabels()
        {
            this.ClearLabels();

            if (!this.isMeasurePassed)
            {
                return;
            }

            if (this.ownerGauge.LabelStep > 0)
            {
                this.CreateLabels();
            }
        }

        private void ResetTicks()
        {
            this.ClearTicks();

            if (!this.isMeasurePassed)
            {
                return;
            }

            if (this.ownerGauge.TickStep > 0)
            {
                this.CreateTicks();
            }
        }

        private void ClearTicks()
        {
            foreach (ContentPresenter tick in this.ticks)
            {
                this.Children.Remove(tick);
            }
            this.ticks.Clear();
        }

        private void ClearLabels()
        {
            foreach (ContentPresenter label in this.labels)
            {
                this.Children.Remove(label);
            }
            this.labels.Clear();
        }
    }
}
