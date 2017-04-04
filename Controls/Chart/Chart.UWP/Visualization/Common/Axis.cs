using System;
using System.Collections.Generic;
using Telerik.Charting;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents an axis within a <see cref="RadCartesianChart"/> instance.
    /// </summary>
    public abstract class Axis : ChartElementPresenter
    {
        /// <summary>
        /// Identifies the <see cref="LabelRotationAngle"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelRotationAngleProperty =
            DependencyProperty.Register(nameof(LabelRotationAngle), typeof(double), typeof(Axis), new PropertyMetadata(300d, OnLabelRotationAngleChanged));

        /// <summary>
        /// Identifies the <see cref="MajorTickTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty MajorTickTemplateProperty =
            DependencyProperty.Register(nameof(MajorTickTemplate), typeof(DataTemplate), typeof(Axis), new PropertyMetadata(null, OnMajorTickTemplateChanged));

        /// <summary>
        /// Identifies the <see cref="MajorTickStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty MajorTickStyleProperty =
            DependencyProperty.Register(nameof(MajorTickStyle), typeof(Style), typeof(Axis), new PropertyMetadata(null, OnMajorTickStyleChanged));

        /// <summary>
        /// Identifies the <see cref="TitleTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty TitleTemplateProperty =
            DependencyProperty.Register(nameof(TitleTemplate), typeof(DataTemplate), typeof(Axis), new PropertyMetadata(null, OnTitleTemplateChanged));

        /// <summary>
        /// Identifies the <see cref="ShowLabels"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowLabelsProperty =
            DependencyProperty.Register(nameof(ShowLabels), typeof(bool), typeof(Axis), new PropertyMetadata(true, OnShowLabelsPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="LabelInterval"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelIntervalProperty =
            DependencyProperty.Register(nameof(LabelInterval), typeof(int), typeof(Axis), new PropertyMetadata(1, OnLabelIntervalPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="LabelFitMode"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelFitModeProperty =
            DependencyProperty.Register(nameof(LabelFitMode), typeof(AxisLabelFitMode), typeof(Axis), new PropertyMetadata(AxisLabelFitMode.None, OnLabelFitModePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="LabelTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelTemplateProperty =
            DependencyProperty.Register(nameof(LabelTemplate), typeof(DataTemplate), typeof(Axis), new PropertyMetadata(null, OnLabelTemplateChanged));

        /// <summary>
        /// Identifies the <see cref="LabelTemplateSelector"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelTemplateSelectorProperty =
            DependencyProperty.Register(nameof(LabelTemplateSelector), typeof(DataTemplateSelector), typeof(Axis), new PropertyMetadata(null, OnLabelTemplateSelectorChanged));

        /// <summary>
        /// Identifies the <see cref="LabelStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelStyleProperty =
            DependencyProperty.Register(nameof(LabelStyle), typeof(Style), typeof(Axis), new PropertyMetadata(null, OnLabelStyleChanged));

        /// <summary>
        /// Identifies the <see cref="LineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty LineStyleProperty =
            DependencyProperty.Register(nameof(LineStyle), typeof(Style), typeof(Axis), new PropertyMetadata(null, OnLineStyleChanged));

        /// <summary>
        /// Identifies the <see cref="Title"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(object), typeof(Axis), new PropertyMetadata(null, OnTitleChanged));

        /// <summary>
        /// Identifies the <see cref="LabelFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFormatProperty =
            DependencyProperty.Register(nameof(LabelFormat), typeof(string), typeof(Axis), new PropertyMetadata(string.Empty, OnLabelFormatChanged));

        /// <summary>
        /// Identifies the <see cref="LabelFormatter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFormatterProperty =
            DependencyProperty.Register(nameof(LabelFormatter), typeof(IContentFormatter), typeof(Axis), new PropertyMetadata(null, OnLabelFormatterChanged));

        internal const int DefaultMajorTickLength = 5;

        internal AxisType type;
        internal AxisModel model;

        private const int MaxLabelSizeCache = 128;

        private static readonly DependencyProperty VisibilityWatcherProperty =
            DependencyProperty.Register("VisibilityWatcher", typeof(Visibility), typeof(Axis), new PropertyMetadata(Visibility.Visible, OnVisibilityWatcherChanged));

        private List<FrameworkElement> tickVisuals = new List<FrameworkElement>();
        private List<FrameworkElement> labelVisuals = new List<FrameworkElement>();
        private DataTemplate majorTickTemplateCache;
        private DataTemplate labelTemplateCache;
        private DataTemplateSelector labelTemplateSelectorCache;
        private Style labelStyleCache;
        private Style majorTickStyleCache;
        private ContentPresenter titlePresenter = new ContentPresenter();
        private double labelRotationAngleCache = 300;
        private double normalizedRotationAngle = 300;

        private Dictionary<object, LabelSizeInfo> labelSizeCache = new Dictionary<object, LabelSizeInfo>(MaxLabelSizeCache);

        /// <summary>
        /// Initializes a new instance of the <see cref="Axis"/> class.
        /// </summary>
        protected Axis()
        {
            this.model = this.CreateModel();
            this.titlePresenter.Content = null;
            this.SetBinding(Axis.VisibilityWatcherProperty, new Binding() { Path = new PropertyPath("Visibility"), Source = this });
        }

        /// <summary>
        /// Gets or sets the <see cref="IContentFormatter"/> instance used to format each axis label.
        /// </summary>
        public IContentFormatter LabelFormatter
        {
            get
            {
                return this.model.ContentFormatter;
            }
            set
            {
                this.SetValue(LabelFormatterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the rotation angle of the labels when LabelFitMode equals Rotate.
        /// </summary>
        public double LabelRotationAngle
        {
            get
            {
                return this.labelRotationAngleCache;
            }
            set
            {
                this.SetValue(LabelRotationAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> object that defines the appearance of the <see cref="Shape"/> shape used to display axis' line.
        /// For a CartesianAxis the shape will be a <see cref="Line"/> instance while for a RadialAxis the shape will be an <see cref="Ellipse"/> shape.
        /// </summary>
        public Style LineStyle
        {
            get
            {
                return (Style)this.GetValue(LineStyleProperty);
            }
            set
            {
                this.SetValue(LineStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the strategy that defines the last axis label visibility.
        /// </summary>
        public AxisLastLabelVisibility LastLabelVisibility
        {
            get
            {
                return this.model.LastLabelVisibility;
            }
            set
            {
                this.model.LastLabelVisibility = value;
            }
        }

        /// <summary>
        /// Gets or sets the format used to format all the labels present on the axis.
        /// </summary>
        public string LabelFormat
        {
            get
            {
                return this.model.labelFormat;
            }
            set
            {
                this.SetValue(LabelFormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets index-based offset of the first tick to be displayed.
        /// </summary>
        public int MajorTickOffset
        {
            get
            {
                return this.model.MajorTickOffset;
            }
            set
            {
                this.model.MajorTickOffset = value;
            }
        }

        /// <summary>
        /// Gets or sets index-based offset of the first tick to be displayed.
        /// </summary>
        public int LabelOffset
        {
            get
            {
                return this.model.LabelOffset;
            }
            set
            {
                this.model.LabelOffset = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether labels will be displayed on this axis.
        /// </summary>
        public bool ShowLabels
        {
            get
            {
                return this.model.ShowLabels;
            }
            set
            {
                this.SetValue(ShowLabelsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that determines how the axis labels will be laid out when they are overlapping each other.
        /// </summary>
        public AxisLabelFitMode LabelFitMode
        {
            get
            {
                return this.model.LabelFitMode;
            }
            set
            {
                this.SetValue(LabelFitModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the title of the associated logical axis model.
        /// </summary>
        public object Title
        {
            get
            {
                return this.GetValue(TitleProperty);
            }
            set
            {
                this.SetValue(TitleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to visualize axis labels.
        /// </summary>
        public DataTemplate LabelTemplate
        {
            get
            {
                return this.labelTemplateCache;
            }
            set
            {
                this.SetValue(LabelTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplateSelector"/> used to provide conditional <see cref="DataTemplate"/> look-up when axis labels are visualized.
        /// </summary>
        public DataTemplateSelector LabelTemplateSelector
        {
            get
            {
                return this.labelTemplateSelectorCache;
            }
            set
            {
                this.SetValue(LabelTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> to be applied to the default <see cref="TextBlock"/> instance created when <see cref="LabelTemplate"/> property is not specified.
        /// Creating a <see cref="TextBlock"/> instance per label instead of a <see cref="ContentPresenter"/> gives a huge performance boost.
        /// </summary>
        public Style LabelStyle
        {
            get
            {
                return this.labelStyleCache;
            }
            set
            {
                this.SetValue(LabelStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> instance used to visualize the axis's title.
        /// </summary>
        public DataTemplate TitleTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(TitleTemplateProperty);
            }
            set
            {
                this.SetValue(TitleTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> instance used to visualize major ticks on the axis.
        /// If this value is not specified ticks will be presented by a <see cref="Rectangle"/> shape.
        /// </summary>
        public DataTemplate MajorTickTemplate
        {
            get
            {
                return this.majorTickTemplateCache;
            }
            set
            {
                this.SetValue(MajorTickTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style for each <see cref="Rectangle"/> instance created to represent a major tick when no <see cref="MajorTickTemplate"/> is specified.
        /// Creating a <see cref="Rectangle"/> instance per tick instead of a <see cref="ContentPresenter"/> gives a huge performance boost.
        /// </summary>
        public Style MajorTickStyle
        {
            get
            {
                return this.majorTickStyleCache;
            }
            set
            {
                this.SetValue(MajorTickStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the thickness of a single tick present on the axis. Useful when custom tick templates are specified.
        /// </summary>
        public double TickThickness
        {
            get
            {
                return this.model.TickThickness;
            }
            set
            {
                this.model.TickThickness = value;
            }
        }

        /// <summary>
        /// Gets or sets the step at which labels are positioned.
        /// </summary>
        public int LabelInterval
        {
            get
            {
                return this.model.LabelInterval;
            }
            set
            {
                this.SetValue(LabelIntervalProperty, value);
            }
        }

        internal override Element Element
        {
            get
            {
                return this.model;
            }
        }

        internal override int DefaultZIndex
        {
            get
            {
                return RadCartesianChart.AxisZIndex;
            }
        }

        /// <summary>
        /// Gets the visual that represents the stroke of the axis.
        /// </summary>
        internal abstract Shape StrokeVisual { get; }

        internal abstract AxisModel CreateModel();

        /// <summary>
        /// Exposed for testing purposes.
        /// </summary>
        internal void SetTestModel(AxisModel testModel)
        {
            this.model = testModel;
        }

        internal virtual void TransformTick(AxisTickModel tick, FrameworkElement visual)
        {
        }

        internal virtual void TransformLabel(AxisLabelModel label, FrameworkElement visual)
        {
            if (this.model.type == AxisType.First)
            {
                if (this.model.LabelFitMode == AxisLabelFitMode.Rotate)
                {
                    visual.RenderTransform = new RotateTransform() { Angle = this.labelRotationAngleCache };
                    visual.RenderTransformOrigin = new Point(0.5, 0.5);
                }
                else
                {
                    visual.RenderTransform = null;
                }
            }
        }

        internal virtual RadRect GetLayoutSlot(Node node, ChartLayoutContext context)
        {
            RadRect layoutSlot = node.layoutSlot;
            if (this.type == AxisType.First)
            {
                layoutSlot.X += context.PlotOrigin.X;
            }
            else
            {
                layoutSlot.Y += context.PlotOrigin.Y;
            }

            return layoutSlot;
        }

        /// <summary>
        /// Updates of all of the chart elements presented by this instance.
        /// </summary>
        internal override void UpdateUICore(ChartLayoutContext context)
        {
            base.UpdateUICore(context);

            if (this.renderSurface != null)
            {
                // create and arrange ticks, labels and axis line
                this.ArrangeVisuals(context);

                this.UpdateTitle();
            }
        }

        internal override void OnPlotOriginChanged(ChartLayoutContext context)
        {
            base.OnPlotOriginChanged(context);

            this.ArrangeVisuals(context);
        }

        internal virtual void TransformTitle(FrameworkElement title)
        {
        }

        internal FrameworkElement GetTickVisual(AxisTickModel tick, int index)
        {
            FrameworkElement visual;

            if (index >= this.tickVisuals.Count)
            {
                visual = this.CreateTickVisual(tick);
                this.tickVisuals.Add(visual);
            }
            else
            {
                visual = this.tickVisuals[index];
                visual.Visibility = Visibility.Visible;
            }

            return visual;
        }

        internal FrameworkElement GetLabelVisual(AxisLabelModel label, int index)
        {
            FrameworkElement visual;
            if (index >= this.labelVisuals.Count)
            {
                visual = this.CreateLabelVisual(label);
                this.labelVisuals.Add(visual);
            }
            else
            {
                visual = this.labelVisuals[index];
                visual.Visibility = Visibility.Visible;
            }

            Axis.SetLabelContent(visual, label);
            this.TransformLabel(label, visual);

            return visual;
        }

        /// <summary>
        /// Core entry point for calculating the size of a node's content.
        /// </summary>
        protected internal override RadSize MeasureNodeOverride(Node node, object content)
        {
            // TODO: Further optimization: check whether content is actually measured already
            var labelModel = node as AxisLabelModel;
            if (labelModel != null)
            {
                return this.MeasureLabel(labelModel, content);
            }

            if (node == this.model.title)
            {
                return this.MeasureTitle();
            }

            return base.MeasureNodeOverride(node, content);
        }

        /// <summary>
        /// Gets the <see cref="DataTemplate"/> instance used to visualize the a tick with the specified <see cref="TickType"/>.
        /// </summary>
        protected virtual DataTemplate GetTickTemplate(TickType tickType)
        {
            return this.majorTickTemplateCache;
        }

        /// <summary>
        /// Occurs when the presenter has been successfully attached to its owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            (this.chart.chartArea as ChartAreaModelWithAxes).SetAxis(this.model, this.type);
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            if (this.renderSurface != null)
            {
                this.renderSurface.Children.Remove(this.titlePresenter);
            }
        }

        /// <summary>
        /// Adds the <see cref="ContentPresenter"/> instances used to visualize the title of the axis.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            if (applied)
            {
                this.renderSurface.Children.Add(this.titlePresenter);
            }

            return applied;
        }

        // TODO: Check whether we should raise exception if the oldChart is invalid.

        /// <summary>
        /// Occurs when the presenter has been successfully detached from its owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected override void OnDetached(RadChartBase oldChart)
        {
            this.model.labels.Clear();
            this.model.ticks.Clear();

            base.OnDetached(oldChart);

            if (oldChart != null)
            {
                var oldChartArea = oldChart.chartArea as ChartAreaModelWithAxes;

                if (oldChartArea != null)
                {
                    oldChartArea.RemoveAxis(this.model);
                }
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new AxisAutomationPeer(this);
        }

        private static void OnLabelRotationAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis presenter = d as Axis;
            presenter.labelRotationAngleCache = (double)e.NewValue;
            presenter.normalizedRotationAngle = presenter.labelRotationAngleCache % 360;
            if (presenter.normalizedRotationAngle < 0)
            {
                presenter.normalizedRotationAngle += 360;
            }

            presenter.labelSizeCache.Clear();
            presenter.model.NormalizedLabelRotationAngle = presenter.normalizedRotationAngle;
        }

        private static void OnMajorTickStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis presenter = d as Axis;
            presenter.majorTickStyleCache = e.NewValue as Style;

            // no label template, so we have the default text blocks created, apply the new style to them
            if (presenter.majorTickTemplateCache == null)
            {
                foreach (FrameworkElement tickVisual in presenter.tickVisuals)
                {
                    tickVisual.Style = presenter.majorTickStyleCache;
                }
            }
        }

        private static void OnMajorTickTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis presenter = d as Axis;
            presenter.majorTickTemplateCache = e.NewValue as DataTemplate;
            presenter.ClearPresenters(presenter.tickVisuals);
        }

        private static void OnTitleTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis presenter = d as Axis;
            presenter.titlePresenter.ContentTemplate = e.NewValue as DataTemplate;
        }

        private static void OnShowLabelsPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            Axis presenter = target as Axis;
            presenter.model.ShowLabels = (bool)args.NewValue;
        }

        private static void OnLabelIntervalPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            Axis presenter = target as Axis;
            presenter.model.LabelInterval = (int)args.NewValue;
        }

        private static void OnLabelFitModePropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            Axis presenter = target as Axis;

            presenter.model.LabelFitMode = (AxisLabelFitMode)args.NewValue;
            presenter.labelSizeCache.Clear();
        }

        private static void OnLabelTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis presenter = d as Axis;
            presenter.labelTemplateCache = e.NewValue as DataTemplate;

            // reset all cached label sizes
            presenter.labelSizeCache.Clear();

            presenter.ClearPresenters(presenter.labelVisuals);
        }

        private static void OnLabelTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis presenter = d as Axis;
            presenter.labelTemplateSelectorCache = e.NewValue as DataTemplateSelector;

            // reset all cached label sizes
            presenter.labelSizeCache.Clear();

            presenter.ClearPresenters(presenter.labelVisuals);
        }

        private static void OnLabelStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis presenter = d as Axis;
            presenter.labelStyleCache = e.NewValue as Style;

            // reset all cached label sizes
            if (presenter.labelSizeCache != null)
            {
                presenter.labelSizeCache.Clear();
            }

            // no label template, so we have the default text blocks created, apply the new style to them
            if (presenter.labelTemplateCache == null)
            {
                foreach (FrameworkElement labelVisual in presenter.labelVisuals)
                {
                    labelVisual.Style = presenter.labelStyleCache;
                }
            }
        }

        private static void OnLineStyleChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            Axis presenter = target as Axis;
            presenter.UpdateStrokeVisual(e.NewValue as Style);
        }

        private static void OnTitleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Axis presenter = sender as Axis;

            presenter.titlePresenter.Content = e.NewValue;
            presenter.model.Title.Content = e.NewValue;
        }

        private static void OnLabelFormatChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Axis presenter = sender as Axis;
            presenter.model.LabelFormat = (string)e.NewValue;
        }

        private static void OnLabelFormatterChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Axis presenter = sender as Axis;
            presenter.model.ContentFormatter = (IContentFormatter)e.NewValue;
        }

        private static void SetLabelContent(FrameworkElement presenter, AxisLabelModel label)
        {
            var contentPresetner = presenter as ContentPresenter;
            var textBlock = presenter as TextBlock;

            if (contentPresetner != null)
            {
                contentPresetner.Content = label.Content;
            }
            else if (textBlock != null)
            {
                textBlock.Text = label.Content == null ? string.Empty : label.Content.ToString();
            }
            else
            {
                // TODO: consider throwing exception
            }
        }

        private static LabelSizeInfo GetLabelSize(FrameworkElement visual)
        {
            RadSize visualSize = GetVisualDesiredSize(visual);
            LabelSizeInfo sizeInfo = new LabelSizeInfo() { UntransformedSize = visualSize };

            GeneralTransform transform = visual.RenderTransform;
            if (transform != null)
            {
                Rect labelRect = new Rect(0, 0, visualSize.Width, visualSize.Height);
                labelRect = transform.TransformBounds(labelRect);
                sizeInfo.TransformOffset = new RadPoint(labelRect.X, labelRect.Y);
                sizeInfo.Size = new RadSize(labelRect.Width, labelRect.Height);
            }
            else
            {
                sizeInfo.Size = visualSize;
            }

            return sizeInfo;
        }

        private static void UpdateLabelSizeInfo(AxisLabelModel label, LabelSizeInfo sizeInfo)
        {
            label.transformOffset = sizeInfo.TransformOffset;
            label.untransformedDesiredSize = sizeInfo.UntransformedSize;
        }

        private static void OnVisibilityWatcherChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Axis axis = sender as Axis;
            if (axis.chart != null)
            {
                axis.chart.InvalidateUI();
            }
        }

        private void UpdateStrokeVisual(Style style)
        {
            if (this.StrokeVisual != null)
            {
                this.StrokeVisual.ClearValue(Shape.StrokeThicknessProperty);
                this.StrokeVisual.Style = style;
                this.model.LineThickness = this.StrokeVisual.StrokeThickness;

                if (this.IsTemplateApplied)
                {
                    this.chart.InvalidateUI();
                }
            }
        }

        private void ArrangeVisuals(ChartLayoutContext context)
        {
            this.UpdateTicks(context);
            this.UpdateLabels(context);
        }

        private void UpdateTitle()
        {
            if (this.titlePresenter.Content == null)
            {
                this.titlePresenter.Visibility = Visibility.Collapsed;
                return;
            }

            this.titlePresenter.Visibility = Visibility.Visible;

            this.TransformTitle(this.titlePresenter);
            this.ArrangeUIElement(this.titlePresenter, this.model.title.layoutSlot, false);
        }

        private RadSize MeasureTitle()
        {
            this.titlePresenter.Visibility = Visibility.Visible;

            // remove previous transform so that it will not be considered in measurement logic
            this.titlePresenter.ClearValue(UIElement.RenderTransformProperty);
            this.titlePresenter.ClearValue(FrameworkElement.WidthProperty);
            this.titlePresenter.ClearValue(FrameworkElement.HeightProperty);

            this.titlePresenter.Measure(PresenterBase.InfinitySize);

            Size desiredSize = this.titlePresenter.DesiredSize;
            return new RadSize(desiredSize.Width, desiredSize.Height);
        }

        private void UpdateTicks(ChartLayoutContext context)
        {
            int visibleTicks = 0;

            foreach (AxisTickModel tick in this.model.ticks)
            {
                if (!tick.isVisible)
                {
                    continue;
                }

                // the GetVisual method will update the UIElement's Visibility property
                FrameworkElement visual = this.GetTickVisual(tick, visibleTicks);
                this.TransformTick(tick, visual);
                this.ArrangeUIElement(visual, this.GetLayoutSlot(tick, context));

                visibleTicks++;
            }

            // remove unnecessary ticks
            while (visibleTicks < this.tickVisuals.Count)
            {
                this.tickVisuals[visibleTicks].Visibility = Visibility.Collapsed;
                visibleTicks++;
            }
        }

        private void UpdateLabels(ChartLayoutContext context)
        {
            double sineOrCosine = 0;
            if (this.model.labelFitMode == AxisLabelFitMode.Rotate)
            {
                double radiansByModulo = (this.normalizedRotationAngle % 90) * RadMath.DegToRadFactor;

                if (this.normalizedRotationAngle < 90 ||
                    (this.normalizedRotationAngle >= 180 && this.normalizedRotationAngle < 270))
                {
                    sineOrCosine = Math.Sin(radiansByModulo);
                }
                else
                {
                    sineOrCosine = Math.Cos(radiansByModulo);
                }
            }

            int visibleLabels = 0;
            foreach (AxisLabelModel label in this.model.labels)
            {
                if (!label.isVisible)
                {
                    continue;
                }

                // the GetVisual method will update the UIelement's Visibility property
                FrameworkElement visual = this.GetLabelVisual(label, visibleLabels);
                RadRect layoutSlot = this.GetLayoutSlot(label, context);

                if (this.model.labelFitMode == AxisLabelFitMode.Rotate)
                {
                    var margin = visual.Margin;
                    var actualWidth = label.untransformedDesiredSize.Width - (margin.Left + margin.Right);
                    layoutSlot.Y += (int)(sineOrCosine * actualWidth / 2);
                }

                this.ArrangeUIElement(visual, layoutSlot, false);
                visibleLabels++;
            }

            // remove unnecessary ticks
            while (visibleLabels < this.labelVisuals.Count)
            {
                this.labelVisuals[visibleLabels].Visibility = Visibility.Collapsed;
                visibleLabels++;
            }
        }

        private RadSize MeasureLabel(AxisLabelModel label, object content)
        {
            LabelSizeInfo sizeInfo;
            if (this.labelSizeCache.TryGetValue(content, out sizeInfo))
            {
                Axis.UpdateLabelSizeInfo(label, sizeInfo);
                return sizeInfo.Size;
            }

            FrameworkElement visual = this.GetLabelVisual(label, label.CollectionIndex);
            visual.ClearValue(FrameworkElement.WidthProperty);
            visual.ClearValue(FrameworkElement.HeightProperty);
            visual.Measure(PresenterBase.InfinitySize);

            sizeInfo = GetLabelSize(visual);
            Axis.UpdateLabelSizeInfo(label, sizeInfo);

            // cache the measured size since this is one of the most expensive visual operation
            if (this.labelSizeCache.Count > MaxLabelSizeCache)
            {
                // do not exceed max cache count
                this.labelSizeCache.Clear();
            }
            this.labelSizeCache[content] = sizeInfo;

            return sizeInfo.Size;
        }

        private FrameworkElement CreateLabelVisual(AxisLabelModel label)
        {
            DataTemplate template = null;
            if (this.labelTemplateCache != null)
            {
                template = this.labelTemplateCache;
            }
            else if (this.labelTemplateSelectorCache != null)
            {
                template = this.labelTemplateSelectorCache.SelectTemplate(label.Content, this);
            }

            FrameworkElement visual;
            if (template == null)
            {
                // creating a TextBlock directly gives huge performance boost - about 10 frames per second!!!
                visual = this.CreateLabelTextBlock();
            }
            else
            {
                visual = this.CreateContentPresenter(label.Content, template);
            }

            return visual;
        }

        private FrameworkElement CreateTickVisual(AxisTickModel tick)
        {
            FrameworkElement visual;
            DataTemplate template = this.GetTickTemplate(tick.Type);
            if (template == null)
            {
                visual = this.CreateTickRectangle();
            }
            else
            {
                visual = this.CreateContentPresenter(tick, template);
            }

            return visual;
        }

        private TextBlock CreateLabelTextBlock()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Style = this.labelStyleCache;
            this.renderSurface.Children.Add(textBlock);

            return textBlock;
        }

        private Rectangle CreateTickRectangle()
        {
            Rectangle visual = new Rectangle();
            visual.Style = this.majorTickStyleCache;
            this.renderSurface.Children.Add(visual);

            return visual;
        }

        private struct LabelSizeInfo
        {
            public RadSize Size;
            public RadSize UntransformedSize;
            public RadPoint TransformOffset;
        }
    }
}