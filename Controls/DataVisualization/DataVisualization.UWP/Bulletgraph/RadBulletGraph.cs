using System.Collections.ObjectModel;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// A control that represents the bullet graph data visualization.
    /// </summary>
    [TemplatePart(Name = "PART_Scale", Type = typeof(RadLinearGauge))]
    [TemplatePart(Name = "PART_QualitativeBar", Type = typeof(SegmentedLinearGaugeIndicator))]
    [TemplatePart(Name = "PART_FeaturedMeasure", Type = typeof(LinearBarGaugeIndicator))]
    [TemplatePart(Name = "PART_ProjectedMeasure", Type = typeof(LinearBarGaugeIndicator))]
    [TemplatePart(Name = "PART_AlternativeFeaturedMeasure", Type = typeof(MarkerGaugeIndicator))]
    [TemplatePart(Name = "PART_ComparativeMeasure", Type = typeof(MarkerGaugeIndicator))]
    [ContentProperty(Name = "QualitativeRanges")]
    public class RadBulletGraph : RadControl
    {
        /// <summary>
        /// Identifies the StartValue dependency property.
        /// </summary>
        public static readonly DependencyProperty StartValueProperty =
            DependencyProperty.Register(nameof(StartValue), typeof(double), typeof(RadBulletGraph), new PropertyMetadata(0.0, OnStartValuePropertyChanged));

        /// <summary>
        /// Identifies the EndValue dependency property.
        /// </summary>
        public static readonly DependencyProperty EndValueProperty =
            DependencyProperty.Register(nameof(EndValue), typeof(double), typeof(RadBulletGraph), new PropertyMetadata(100.0, OnEndValuePropertyChanged));

        /// <summary>
        /// Identifies the TickStep dependency property.
        /// </summary>
        public static readonly DependencyProperty TickStepProperty =
            DependencyProperty.Register(nameof(TickStep), typeof(double), typeof(RadBulletGraph), new PropertyMetadata(5.0));

        /// <summary>
        /// Identifies the LabelStep dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelStepProperty =
            DependencyProperty.Register(nameof(LabelStep), typeof(double), typeof(RadBulletGraph), new PropertyMetadata(25.0));

        /// <summary>
        /// Identifies the LabelOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelOffsetProperty =
            DependencyProperty.Register(nameof(LabelOffset), typeof(double), typeof(RadBulletGraph), new PropertyMetadata(20.0));

        /// <summary>
        /// Identifies the TickTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty TickTemplateProperty =
            DependencyProperty.Register(nameof(TickTemplate), typeof(DataTemplate), typeof(RadBulletGraph), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the LabelTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTemplateProperty =
            DependencyProperty.Register(nameof(LabelTemplate), typeof(DataTemplate), typeof(RadBulletGraph), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(RadBulletGraph), new PropertyMetadata(Orientation.Horizontal));

        /// <summary>
        /// Identifies the FeaturedMeasure dependency property.
        /// </summary>
        public static readonly DependencyProperty FeaturedMeasureProperty =
            DependencyProperty.Register(nameof(FeaturedMeasure), typeof(double), typeof(RadBulletGraph), new PropertyMetadata(90.0, OnFeaturedMeasurePropertyChanged));

        /// <summary>
        /// Identifies the FeaturedMeasureStartValue dependency property.
        /// </summary>
        public static readonly DependencyProperty FeaturedMeasureStartValueProperty =
            DependencyProperty.Register(nameof(FeaturedMeasureStartValue), typeof(double), typeof(RadBulletGraph), new PropertyMetadata(0.0, OnFeaturedMeasureStartValuePropertyChanged));

        /// <summary>
        /// Identifies the FeaturedMeasureBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty FeaturedMeasureBrushProperty =
            DependencyProperty.Register(nameof(FeaturedMeasureBrush), typeof(Brush), typeof(RadBulletGraph), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the FeaturedMeasureThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty FeaturedMeasureThicknessProperty =
            DependencyProperty.Register(nameof(FeaturedMeasureThickness), typeof(double), typeof(RadBulletGraph), new PropertyMetadata(8.0));

        /// <summary>
        /// Identifies the FeaturedMeasureAlternativeTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty FeaturedMeasureAlternativeTemplateProperty =
            DependencyProperty.Register(nameof(FeaturedMeasureAlternativeTemplate), typeof(DataTemplate), typeof(RadBulletGraph), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the ProjectedMeasure dependency property.
        /// </summary>
        public static readonly DependencyProperty ProjectedMeasureProperty =
            DependencyProperty.Register(nameof(ProjectedMeasure), typeof(double), typeof(RadBulletGraph), new PropertyMetadata(0.0, OnProjectedMeasurePropertyChanged));

        /// <summary>
        /// Identifies the ProjectedMeasureBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty ProjectedMeasureBrushProperty =
            DependencyProperty.Register(nameof(ProjectedMeasureBrush), typeof(Brush), typeof(RadBulletGraph), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the ComparativeMeasure dependency property.
        /// </summary>
        public static readonly DependencyProperty ComparativeMeasureProperty =
            DependencyProperty.Register(nameof(ComparativeMeasure), typeof(double), typeof(RadBulletGraph), new PropertyMetadata(80.0, OnComparativeMeasurePropertyChanged));

        /// <summary>
        /// Identifies the ComparativeMeasureTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ComparativeMeasureTemplateProperty =
            DependencyProperty.Register(nameof(ComparativeMeasureTemplate), typeof(DataTemplate), typeof(RadBulletGraph), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the IsAnimated dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAnimatedProperty =
            DependencyProperty.Register(nameof(IsAnimated), typeof(bool), typeof(RadBulletGraph), new PropertyMetadata(true));

        internal const int FeaturedMeasuresZIndex = 3;
        internal const int ComparativeMeasuresZIndex = 2;

        internal RadLinearGauge scalePart;

        private const string QualitativeBarPartName = "PART_QualitativeBar";
        private const string ComparativeMeasurePartName = "PART_ComparativeMeasure";
        private const string ScalePartName = "PART_Scale";

        private SegmentedLinearGaugeIndicator qualitativeBarPart;
        private MarkerGaugeIndicator comparativeMeasurePart;
        private bool isValueCoerceScheduled;

        /// <summary>
        /// Initializes a new instance of the RadBulletGraph class.
        /// </summary>
        public RadBulletGraph()
        {
            this.DefaultStyleKey = typeof(RadBulletGraph);

            this.QualitativeRanges = new ObservableCollection<BarIndicatorSegment>();
            this.AdditionalComparativeMeasures = new AdditionalMeasuresCollection<BulletGraphComparativeMeasure>(this);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the featured measure will be animated when its value changes.
        /// </summary>
        public bool IsAnimated
        {
            get
            {
                return (bool)this.GetValue(RadBulletGraph.IsAnimatedProperty);
            }

            set
            {
                this.SetValue(RadBulletGraph.IsAnimatedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the start value of the bullet graph scale.
        /// </summary>
        public double StartValue
        {
            get
            {
                return (double)this.GetValue(RadBulletGraph.StartValueProperty);
            }

            set
            {
                this.SetValue(RadBulletGraph.StartValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the end value of the bullet graph scale.
        /// </summary>
        public double EndValue
        {
            get
            {
                return (double)this.GetValue(RadBulletGraph.EndValueProperty);
            }

            set
            {
                this.SetValue(RadBulletGraph.EndValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tick step of the bullet graph scale.
        /// </summary>
        public double TickStep
        {
            get
            {
                return (double)this.GetValue(RadBulletGraph.TickStepProperty);
            }

            set
            {
                this.SetValue(RadBulletGraph.TickStepProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label step of the bullet graph scale.
        /// </summary>
        public double LabelStep
        {
            get
            {
                return (double)this.GetValue(RadBulletGraph.LabelStepProperty);
            }

            set
            {
                this.SetValue(RadBulletGraph.LabelStepProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label template of the bullet graph scale.
        /// </summary>
        public DataTemplate LabelTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(RadBulletGraph.LabelTemplateProperty);
            }

            set
            {
                this.SetValue(RadBulletGraph.LabelTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tick template of the bullet graph scale.
        /// </summary>
        public DataTemplate TickTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(RadBulletGraph.TickTemplateProperty);
            }

            set
            {
                this.SetValue(RadBulletGraph.TickTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label offset of the bullet graph scale. This offset moves the labels
        /// relative to the ticks and the measures.
        /// </summary>
        public double LabelOffset
        {
            get
            {
                return (double)this.GetValue(RadBulletGraph.LabelOffsetProperty);
            }

            set
            {
                this.SetValue(RadBulletGraph.LabelOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the bullet graph.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(RadBulletGraph.OrientationProperty);
            }

            set
            {
                this.SetValue(RadBulletGraph.OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets the collection that contains the qualitative ranges of the bullet graph.
        /// New ranges can be added and old ones can be removed. There must be two ranges at minimum and
        /// no more than 5. Otherwise an exception is thrown.
        /// </summary>
        public ObservableCollection<BarIndicatorSegment> QualitativeRanges
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the value of the featured measure.
        /// </summary>
        public double FeaturedMeasure
        {
            get
            {
                return (double)this.GetValue(RadBulletGraph.FeaturedMeasureProperty);
            }

            set
            {
                this.SetValue(RadBulletGraph.FeaturedMeasureProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the start value of the featured measure.
        /// </summary>
        public double FeaturedMeasureStartValue
        {
            get
            {
                return (double)this.GetValue(RadBulletGraph.FeaturedMeasureStartValueProperty);
            }

            set
            {
                this.SetValue(RadBulletGraph.FeaturedMeasureStartValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the featured measure.
        /// </summary>
        public Brush FeaturedMeasureBrush
        {
            get
            {
                return (Brush)this.GetValue(RadBulletGraph.FeaturedMeasureBrushProperty);
            }

            set
            {
                this.SetValue(RadBulletGraph.FeaturedMeasureBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the thickness of the featured measure.
        /// </summary>
        public double FeaturedMeasureThickness
        {
            get
            {
                return (double)this.GetValue(RadBulletGraph.FeaturedMeasureThicknessProperty);
            }

            set
            {
                this.SetValue(RadBulletGraph.FeaturedMeasureThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the template that determines the look of the alternative featured measure.
        /// The alternative featured measure is visible when the StartValue of RadBulletGraph is non-zero.
        /// </summary>
        public DataTemplate FeaturedMeasureAlternativeTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(RadBulletGraph.FeaturedMeasureAlternativeTemplateProperty);
            }

            set
            {
                this.SetValue(RadBulletGraph.FeaturedMeasureAlternativeTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the projected measure.
        /// </summary>
        public double ProjectedMeasure
        {
            get
            {
                return (double)this.GetValue(RadBulletGraph.ProjectedMeasureProperty);
            }

            set
            {
                this.SetValue(RadBulletGraph.ProjectedMeasureProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the projected measure.
        /// </summary>
        public Brush ProjectedMeasureBrush
        {
            get
            {
                return (Brush)this.GetValue(RadBulletGraph.ProjectedMeasureBrushProperty);
            }

            set
            {
                this.SetValue(RadBulletGraph.ProjectedMeasureBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the comparative measure.
        /// </summary>
        public double ComparativeMeasure
        {
            get
            {
                return (double)this.GetValue(RadBulletGraph.ComparativeMeasureProperty);
            }

            set
            {
                this.SetValue(RadBulletGraph.ComparativeMeasureProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a template that determines the look of the default comparative measure.
        /// If not template is specified for the additional comparative measures this template will be used instead.
        /// </summary>
        public DataTemplate ComparativeMeasureTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(RadBulletGraph.ComparativeMeasureTemplateProperty);
            }

            set
            {
                this.SetValue(RadBulletGraph.ComparativeMeasureTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets a collection that contains any additional comparative measure.
        /// All operations on this collection are supported.
        /// </summary>
        public AdditionalMeasuresCollection<BulletGraphComparativeMeasure> AdditionalComparativeMeasures
        {
            get;
            private set;
        }

        internal void InsertMeasure(int index, BulletGraphMeasureBase measure)
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            this.scalePart.Indicators.Insert(index, measure.Visual);
            this.SyncComparativeMeasureTemplate(measure as BulletGraphComparativeMeasure);
            this.scalePart.InvalidateMeasure();
        }

        internal void RemoveMeasure(BulletGraphMeasureBase measure)
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            this.scalePart.Indicators.Remove(measure.Visual);
            this.scalePart.InvalidateMeasure();
        }

        internal void SetMeasure(BulletGraphMeasureBase newMeasure, BulletGraphMeasureBase oldMeasure)
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            int oldIndex = this.scalePart.Indicators.IndexOf(oldMeasure.Visual);
            this.scalePart.Indicators[oldIndex] = newMeasure.Visual;

            this.SyncComparativeMeasureTemplate(newMeasure as BulletGraphComparativeMeasure);
            this.scalePart.InvalidateMeasure();
        }

        /// <summary>
        /// Retrieves the template parts of the control.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            base.ApplyTemplateCore();

            this.qualitativeBarPart = this.GetTemplatePartField<SegmentedLinearGaugeIndicator>(QualitativeBarPartName);
            bool applied = this.qualitativeBarPart != null;

            if (applied)
            {
                foreach (BarIndicatorSegment segment in this.QualitativeRanges)
                {
                    this.qualitativeBarPart.Segments.Add(segment);
                }
                this.QualitativeRanges.Clear();
                this.QualitativeRanges = this.qualitativeBarPart.Segments;
            }

            this.comparativeMeasurePart = this.GetTemplatePartField<MarkerGaugeIndicator>(ComparativeMeasurePartName);
            applied = applied && this.comparativeMeasurePart != null;

            this.scalePart = this.GetTemplatePartField<RadLinearGauge>(ScalePartName);
            applied = applied && this.scalePart != null;

            if (applied)
            {
                foreach (BulletGraphComparativeMeasure measure in this.AdditionalComparativeMeasures)
                {
                    this.scalePart.Indicators.Add(measure.Visual);
                    this.SyncComparativeMeasureTemplate(measure);
                }
            }

            return applied;
        }

        /// <summary>
        /// Ensures that all logical values fall within the specified range.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.EnsureAllValuesInRange();
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadBulletGraphAutomationPeer(this);
        }

        private static void OnStartValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadBulletGraph bulletGraph = sender as RadBulletGraph;
            bulletGraph.EnsureAllValuesInRange();

            RadBulletGraphAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(bulletGraph) as RadBulletGraphAutomationPeer;
            if (peer != null)
            {
                peer.RaiseMinimumPropertyChangedEvent((double)args.OldValue, (double)args.NewValue);
            }
        }

        private static void OnEndValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadBulletGraph bulletGraph = sender as RadBulletGraph;
            bulletGraph.EnsureAllValuesInRange();

            RadBulletGraphAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(bulletGraph) as RadBulletGraphAutomationPeer;
            if (peer != null)
            {
                peer.RaiseMaximumPropertyChangedEvent((double)args.OldValue, (double)args.NewValue);
            }
        }

        private static void OnFeaturedMeasurePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadBulletGraph bulletGraph = sender as RadBulletGraph;
            bulletGraph.EnsureAllValuesInRange();

            RadBulletGraphAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(bulletGraph) as RadBulletGraphAutomationPeer;
            if (peer != null)
            {
                peer.RaiseValuePropertyChangedEvent((double)args.OldValue, (double)args.NewValue);
            }
        }

        private static void OnFeaturedMeasureStartValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadBulletGraph bulletGraph = sender as RadBulletGraph;
            bulletGraph.EnsureAllValuesInRange();
        }

        private static void OnComparativeMeasurePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadBulletGraph bulletGraph = sender as RadBulletGraph;
            bulletGraph.EnsureAllValuesInRange();
        }

        private static void OnProjectedMeasurePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadBulletGraph bulletGraph = sender as RadBulletGraph;
            bulletGraph.EnsureAllValuesInRange();
        }

        private void SyncComparativeMeasureTemplate(BulletGraphComparativeMeasure measure)
        {
            if (measure.Template == null)
            {
                measure.Template = this.comparativeMeasurePart.ContentTemplate;
            }
        }

        private void EnsureAllValuesInRange()
        {
            if (this.isValueCoerceScheduled || this.IsInternalPropertyChange || !this.IsTemplateApplied)
            {
                return;
            }

            this.isValueCoerceScheduled = true;

            var warningSuppression = this.InvokeAsync(() =>
                {
                    var startValue = this.StartValue;
                    var endValue = this.EndValue;

                    if (endValue < startValue)
                    {
                        this.ChangePropertyInternally(EndValueProperty, startValue);
                        endValue = this.EndValue;
                    }

                    this.SetValuesInRange(FeaturedMeasureStartValueProperty, startValue, endValue);
                    this.SetValuesInRange(ComparativeMeasureProperty, startValue, endValue);
                    this.SetValuesInRange(ProjectedMeasureProperty, this.FeaturedMeasureStartValue, endValue);
                    this.SetValuesInRange(FeaturedMeasureProperty, this.FeaturedMeasureStartValue, endValue);
                    this.SetValuesInRange(FeaturedMeasureStartValueProperty, startValue, this.FeaturedMeasure);

                    this.isValueCoerceScheduled = false;
                });
        }

        private void SetValuesInRange(DependencyProperty valueProperty, double minRange, double maxRange)
        {
            double currentVal = (double)this.GetValue(valueProperty);

            if (currentVal < minRange)
            {
                this.ChangePropertyInternally(valueProperty, minRange);
            }
            else if (currentVal > maxRange)
            {
                this.ChangePropertyInternally(valueProperty, maxRange);
            }
        }
    }
}
