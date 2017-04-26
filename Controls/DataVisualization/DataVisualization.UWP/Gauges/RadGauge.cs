using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// Base class for <see cref="RadLinearGauge"/> and <see cref="RadRadialGauge"/>.
    /// </summary>
    [ContentProperty(Name = "Indicators")]
    public abstract class RadGauge : RadControl
    {
        /// <summary>
        /// Identifies the MinValue property.
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(nameof(MinValue), typeof(double), typeof(RadGauge), new PropertyMetadata(0d, OnMinValuePropertyChanged));

        /// <summary>
        /// Identifies the MinValue property.
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(RadGauge), new PropertyMetadata(100d, OnMaxValuePropertyChanged));

        /// <summary>
        /// Identifies the TickTemplate property.
        /// </summary>
        public static readonly DependencyProperty TickTemplateProperty =
            DependencyProperty.Register(nameof(TickTemplate), typeof(DataTemplate), typeof(RadGauge), new PropertyMetadata(null, OnTickTemplatePropertyChanged));

        /// <summary>
        /// Identifies the MiddleTickTemplate property.
        /// </summary>
        public static readonly DependencyProperty MiddleTickTemplateProperty =
            DependencyProperty.Register(nameof(MiddleTickTemplate), typeof(DataTemplate), typeof(RadGauge), new PropertyMetadata(null, OnMiddleTickTemplatePropertyChanged));

        /// <summary>
        /// Identifies the MajorTickTemplate property.
        /// </summary>
        public static readonly DependencyProperty MajorTickTemplateProperty =
            DependencyProperty.Register(nameof(MajorTickTemplate), typeof(DataTemplate), typeof(RadGauge), new PropertyMetadata(null, OnMajorTickTemplatePropertyChanged));

        /// <summary>
        /// Identifies the LabelTemplate property.
        /// </summary>
        public static readonly DependencyProperty LabelTemplateProperty =
            DependencyProperty.Register(nameof(LabelTemplate), typeof(DataTemplate), typeof(RadGauge), new PropertyMetadata(null, OnLabelTemplatePropertyChanged));

        /// <summary>
        /// Identifies the TickStep property.
        /// </summary>
        public static readonly DependencyProperty TickStepProperty =
            DependencyProperty.Register(nameof(TickStep), typeof(double), typeof(RadGauge), new PropertyMetadata(10d, OnTickStepPropertyChanged));

        /// <summary>
        /// Identifies the LabelStep property.
        /// </summary>
        public static readonly DependencyProperty LabelStepProperty =
            DependencyProperty.Register(nameof(LabelStep), typeof(double), typeof(RadGauge), new PropertyMetadata(10d, OnLabelStepPropertyChanged));

        /// <summary>
        /// Identifies the MiddleTickStep property.
        /// </summary>
        public static readonly DependencyProperty MiddleTickStepProperty =
            DependencyProperty.Register(nameof(MiddleTickStep), typeof(int), typeof(RadGauge), new PropertyMetadata(-1, OnMiddleTickStepPropertyChanged));

        /// <summary>
        /// Identifies the MajorTickStep property.
        /// </summary>
        public static readonly DependencyProperty MajorTickStepProperty =
            DependencyProperty.Register(nameof(MajorTickStep), typeof(int), typeof(RadGauge), new PropertyMetadata(-1, OnMajorTickStepPropertyChanged));

        /// <summary>
        /// Identifies the IndicatorsZIndex property.
        /// </summary>
        public static readonly DependencyProperty IndicatorsZIndexProperty =
            DependencyProperty.Register(nameof(IndicatorsZIndex), typeof(int), typeof(RadGauge), new PropertyMetadata(-1, OnIndicatorsZIndexPropertyChanged));

        internal static readonly DependencyProperty TickTypeProperty =
            DependencyProperty.RegisterAttached("TickType", typeof(TickType), typeof(RadGauge), null);

        internal static readonly DependencyProperty TickValueProperty =
            DependencyProperty.RegisterAttached("TickValue", typeof(double), typeof(RadGauge), new PropertyMetadata(0d));

        private const string PanelPartName = "PART_Panel";

        private GaugeIndicatorCollection indicators;
        private GaugePanel panel;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadGauge"/> class.
        /// </summary>
        protected RadGauge()
        {
            this.indicators = new GaugeIndicatorCollection();
            this.indicators.CollectionChanged += this.OnIndicatorsCollectionChanged;
        }

        /// <summary>
        /// Gets a collection that holds the indicators in this range.
        /// </summary>
        public GaugeIndicatorCollection Indicators
        {
            get
            {
                return this.indicators;
            }
        }

        /// <summary>
        /// Gets or sets the minimum value that this range can represent.
        /// </summary>
        /// <remarks>
        /// The indicators can not point to a value that is less than MinValue.
        /// </remarks>
        public double MinValue
        {
            get
            {
                return (double)this.GetValue(MinValueProperty);
            }

            set
            {
                this.SetValue(MinValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum value that this range represent.
        /// </summary>
        /// <remarks>
        /// The indicators can not point to a value that is larger than MaxValue.
        /// </remarks>
        public double MaxValue
        {
            get
            {
                return (double)this.GetValue(MaxValueProperty);
            }

            set
            {
                this.SetValue(MaxValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tick step. 
        /// </summary>
        /// <remarks>
        /// The tick step is used to determine how
        /// the range ticks are spread over the value range.
        /// In other words, it determines how many ticks will be created.
        /// </remarks>
        public double TickStep
        {
            get
            {
                return (double)this.GetValue(TickStepProperty);
            }

            set
            {
                this.SetValue(TickStepProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label step. 
        /// </summary>
        /// <remarks>
        /// This step used to determine how
        /// the range labels will be spread over the value range.
        /// </remarks>
        public double LabelStep
        {
            get
            {
                return (double)this.GetValue(LabelStepProperty);
            }

            set
            {
                this.SetValue(LabelStepProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a step that is used to determine which ticks
        /// will be middle ticks. 
        /// </summary>
        /// <remarks>
        /// This step is used on top of the number of
        /// created ticks, not the value range. Middle ticks will use the
        /// middle tick template.
        /// </remarks>
        public int MiddleTickStep
        {
            get
            {
                return (int)this.GetValue(MiddleTickStepProperty);
            }

            set
            {
                this.SetValue(MiddleTickStepProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a step that is used to determine which ticks
        /// will be major ticks. 
        /// </summary>
        /// <remarks>
        /// This step is used on top of the number of
        /// created ticks, not the value range. Major ticks will use the major
        /// tick template.
        /// </remarks>
        public int MajorTickStep
        {
            get
            {
                return (int)this.GetValue(MajorTickStepProperty);
            }

            set
            {
                this.SetValue(MajorTickStepProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the template for the minor ticks.
        /// </summary>
        public DataTemplate TickTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(TickTemplateProperty);
            }

            set
            {
                this.SetValue(TickTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the template for the middle ticks.
        /// </summary>
        /// <remarks>
        /// If this is null, the minor template will be used instead.
        /// </remarks>
        public DataTemplate MiddleTickTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(MiddleTickTemplateProperty);
            }

            set
            {
                this.SetValue(MiddleTickTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the template for the major ticks.
        /// </summary>
        /// <remarks>
        /// If this is null, the minor template will be used instead.
        /// </remarks>
        public DataTemplate MajorTickTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(MajorTickTemplateProperty);
            }

            set
            {
                this.SetValue(MajorTickTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the template that will represent the labels.
        /// </summary>
        public DataTemplate LabelTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(LabelTemplateProperty);
            }

            set
            {
                this.SetValue(LabelTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the z-index for the gauge indicators that belong to this instance.
        /// </summary>
        /// <value>
        /// The default value is -1 (i.e. indicators are rendered below ticks and labels).
        /// </value>
        public int IndicatorsZIndex
        {
            get
            {
                return (int)this.GetValue(IndicatorsZIndexProperty);
            }
            set
            {
                this.SetValue(IndicatorsZIndexProperty, value);
            }
        }

        /// <summary>
        /// Gets the type of this range. It can be Radial or Linear.
        /// </summary>
        internal abstract GaugeType GaugeType
        {
            get;
        }

        internal GaugePanel Panel
        {
            get
            {
                return this.panel;
            }
        }

        /// <summary>
        /// Converts a logical value from a value range to some kind of physical value in another range.
        /// </summary>
        /// <param name="logicalValue">The value to be converted.</param>
        /// <param name="physicalValueMinMaxDifference">The difference of the min and max values of the expected metric's range.</param>
        /// <param name="logicalValueMinMaxDifference">The difference between the min and max values.</param>
        /// <returns>Returns a physical value that corresponds to the logical value in the physical value's respective range.</returns>
        /// <remarks>
        /// The other physical value can be an angle in the radial range and a distance in the linear range.
        /// For example a value in the min/max value range can be converted to an angle in min/max angle range
        /// or to a distance in the min/max distance range.
        /// </remarks>
        internal static double MapLogicalToPhysicalValue(double logicalValue, double physicalValueMinMaxDifference, double logicalValueMinMaxDifference)
        {
            if (logicalValueMinMaxDifference == 0)
            {
                return 0;
            }

            if (logicalValueMinMaxDifference < 0 || physicalValueMinMaxDifference < 0)
            {
                return 0;
            }

            return logicalValue * physicalValueMinMaxDifference / logicalValueMinMaxDifference;
        }

        internal static Size NormalizeSize(Size size)
        {
            double width = 0, height = 0;
            if (!double.IsInfinity(size.Width))
            {
                width = size.Width;
            }

            if (!double.IsInfinity(size.Height))
            {
                height = size.Height;
            }

            if (height == 0)
            {
                height = width;
            }
            else if (width == 0)
            {
                width = height;
            }

            return new Size(width, height);
        }

        internal static void ValidateValue(double val)
        {
            if (double.IsInfinity(val) || double.IsNaN(val))
            {
                throw new ArgumentException("Infinity or Nan double value.");
            }
        }

        /// <summary>
        /// Resolves the control's template parts.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.panel = this.GetTemplatePartField<GaugePanel>(PanelPartName);
            applied = applied && this.panel != null;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate"/> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.panel.OwnerGauge = this;

            foreach (GaugeIndicator indicator in this.indicators)
            {
                this.panel.Indicators.Add(indicator);
            }
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            this.panel.Indicators.Clear();
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadGaugeAutomationPeer(this);
        }

        private static void OnMinValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            double newVal = (double)args.NewValue;
            ValidateValue(newVal);

            RadGauge gauge = sender as RadGauge;
            if (gauge.panel != null)
            {
                gauge.panel.UpdateOnMinMaxValueChange();
            }
            
            if (AutomationPeer.ListenerExists(AutomationEvents.PropertyChanged))
            {
                var peer = FrameworkElementAutomationPeer.FromElement(gauge) as RadGaugeAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseMinimumPropertyChangedEvent((double)args.OldValue, (double)args.NewValue);
                }
            }
        }

        private static void OnMaxValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            double newVal = (double)args.NewValue;
            ValidateValue(newVal);

            RadGauge gauge = sender as RadGauge;
            if (gauge.panel != null)
            {
                gauge.panel.UpdateOnMinMaxValueChange();
            }

            if (AutomationPeer.ListenerExists(AutomationEvents.PropertyChanged))
            {
                var peer = FrameworkElementAutomationPeer.FromElement(gauge) as RadGaugeAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseMinimumPropertyChangedEvent((double)args.OldValue, (double)args.NewValue);
                }
            }
        }

        private static void OnTickStepPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadGauge gauge = sender as RadGauge;
            if (gauge.panel != null)
            {
                gauge.panel.ResetAndArrangeTicks();
            }
        }

        private static void OnLabelStepPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadGauge gauge = sender as RadGauge;
            if (gauge.panel != null)
            {
                gauge.panel.ResetAndArrangeLabels();
            }
        }

        private static void OnMiddleTickStepPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadGauge gauge = sender as RadGauge;
            if (gauge.panel != null)
            {
                gauge.panel.ResetAndArrangeTicks();
            }
        }

        private static void OnMajorTickStepPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadGauge gauge = sender as RadGauge;
            if (gauge.panel != null)
            {
                gauge.panel.ResetAndArrangeTicks();
            }
        }

        private static void OnTickTemplatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadGauge gauge = sender as RadGauge;
            if (gauge.panel != null)
            {
                gauge.panel.UpdateTickTemplates(args.NewValue as DataTemplate, TickType.Minor);
            }
        }

        private static void OnMiddleTickTemplatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadGauge gauge = sender as RadGauge;
            if (gauge.panel != null)
            {
                gauge.panel.UpdateTickTemplates(args.NewValue as DataTemplate, TickType.Middle);
            }
        }

        private static void OnMajorTickTemplatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadGauge gauge = sender as RadGauge;
            if (gauge.panel != null)
            {
                gauge.panel.UpdateTickTemplates(args.NewValue as DataTemplate, TickType.Major);
            }
        }

        private static void OnLabelTemplatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadGauge gauge = sender as RadGauge;
            if (gauge.panel != null)
            {
                gauge.panel.UpdateTickTemplates(args.NewValue as DataTemplate, TickType.Label);
            }
        }

        private static void OnIndicatorsZIndexPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadGauge gauge = sender as RadGauge;

            if (gauge.panel != null)
            {
                gauge.panel.UpdateIndicatorContainerZIndex((int)args.NewValue);
            }
        }

        private void OnIndicatorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (GaugeIndicator indicator in e.NewItems)
                    {
                        this.panel.Indicators.Add(indicator);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (GaugeIndicator indicator in e.OldItems)
                    {
                        this.panel.Indicators.Remove(indicator);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (GaugeIndicator indicator in e.OldItems)
                    {
                        this.panel.Indicators.Remove(indicator);
                    }
                    foreach (GaugeIndicator indicator in e.NewItems)
                    {
                        this.panel.Indicators.Add(indicator);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.panel.Indicators.Clear();
                    foreach (GaugeIndicator indicator in this.indicators)
                    {
                        this.panel.Indicators.Add(indicator);
                    }
                    break;
            }
        }
    }
}
