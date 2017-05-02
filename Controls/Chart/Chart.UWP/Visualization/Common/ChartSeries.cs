using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using Telerik.Charting;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Base class for all series of data points, plotted on a <see cref="RadChartBase"/> instance.
    /// </summary>
    public abstract partial class ChartSeries : ChartElementPresenter, IChartSeries
    {
        /// <summary>
        /// Identifies the <see cref="ShowLabels"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowLabelsProperty =
            DependencyProperty.Register(nameof(ShowLabels), typeof(bool), typeof(ChartSeries), new PropertyMetadata(false, OnShowLabelsChanged));

        /// <summary>
        /// Identifies the <see cref="ItemsSource"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(ChartSeries), new PropertyMetadata(null, OnItemsSourceChanged));

        /// <summary>
        /// Identifies the <see cref="ClipToPlotArea"/> property.
        /// </summary>
        public static readonly DependencyProperty ClipToPlotAreaProperty =
            DependencyProperty.Register(nameof(ClipToPlotArea), typeof(bool), typeof(ChartSeries), new PropertyMetadata(true, OnClipToPlotAreaChanged));

        /// <summary>
        /// Identifies the <see cref="IsSelectedBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedBindingProperty =
            DependencyProperty.Register(nameof(IsSelectedBinding), typeof(DataPointBinding), typeof(ChartSeries), new PropertyMetadata(null, OnIsSelectedBindingChanged));

        /// <summary>
        /// Identifies the <see cref="PaletteIndex"/> property.
        /// </summary>
        public static readonly DependencyProperty PaletteIndexProperty =
            DependencyProperty.Register(nameof(PaletteIndex), typeof(int), typeof(ChartSeries), new PropertyMetadata(-1, OnPaletteIndexChanged));

        internal ChartSeriesDataSource dataSource;
        internal TranslateTransform plotOriginTransform;
        internal Canvas labelLayer;

        private static readonly DependencyProperty VisibilityWatcherProperty =
    DependencyProperty.Register("VisibilityWatcher", typeof(Visibility), typeof(ChartSeries), new PropertyMetadata(Visibility.Visible, OnVisibilityWatcherChanged));

        private RadRect lastPlotAreaRect;
        private bool showLabelsCache;
        private int paletteIndexCache = -1;
        private ObservableCollection<ChartSeriesLabelDefinition> labelDefinitions;
        private List<KeyValuePair<DataPoint, List<FrameworkElement>>> labels; //// more than one label may be associated with a data point, that is why we have list of lists
       
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSeries"/> class.
        /// </summary>
        protected ChartSeries()
        {
            this.DefaultStyleKey = typeof(ChartSeries);
            BindingOperations.SetBinding(this, ChartSeries.VisibilityWatcherProperty, new Binding() { Path = new PropertyPath("Visibility"), Source = this });

            this.labels = new List<KeyValuePair<DataPoint, List<FrameworkElement>>>();
            this.labelDefinitions = new ObservableCollection<ChartSeriesLabelDefinition>();
            this.labelDefinitions.CollectionChanged += this.OnLabelDefinitionsChanged;

            this.dataSource = this.CreateDataSourceInstance();
            this.dataSource.Owner = this;

            this.plotOriginTransform = new TranslateTransform();
        }

        /// <summary>
        /// Occurs when a data binding operation has been successfully completed.
        /// </summary>
        public event EventHandler DataBindingComplete;

        /// <summary>
        /// Gets or sets the preferred palette index for this series. 
        /// By default the palette index is equal to the index of this series within the owning chart's Series collection.
        /// Set this value to -1 to reset it to its default value and have the CollectionIndex-based logic applied.
        /// </summary>
        public int PaletteIndex
        {
            get
            {
                return this.paletteIndexCache;
            }
            set
            {
                this.SetValue(PaletteIndexProperty, value);
            }
        }

        /// <summary>
        /// Gets the collection that stores all the definitions that describe the appearance of each label per data point.
        /// When <see cref="ShowLabels"/> is true and no custom definition is present within the collection, a default one is used.
        /// </summary>
        public ObservableCollection<ChartSeriesLabelDefinition> LabelDefinitions
        {
            get
            {
                return this.labelDefinitions;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataPointBinding "/> instance that provides mechanism for a ViewModel to define the <see cref="P:DataPoint.IsSelected"/> property.
        /// </summary>
        public DataPointBinding IsSelectedBinding
        {
            get
            {
                return this.GetValue(IsSelectedBindingProperty) as DataPointBinding;
            }
            set
            {
                this.SetValue(IsSelectedBindingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the series will display a label associated with each data point.
        /// </summary>
        public bool ShowLabels
        {
            get
            {
                return this.showLabelsCache;
            }
            set
            {
                this.SetValue(ShowLabelsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the human-readable name of the series.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance will be clipped to the bounds of the plot area.
        /// </summary>
        public bool ClipToPlotArea
        {
            get
            {
                return (bool)this.GetValue(ClipToPlotAreaProperty);
            }
            set
            {
                this.SetValue(ClipToPlotAreaProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the source items to generate data points from.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get
            {
                var value = this.GetValue(ItemsSourceProperty);
                if (value == null)
                {
                    return null;
                }

                var enumerable = value as IEnumerable;
                if (enumerable != null)
                {
                    return enumerable;
                }

                return value as IEnumerable<object>;
            }
            set
            {
                this.SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets the actual palette index used to retrieve the actual palette brush used for this series.
        /// </summary>
        public int ActualPaletteIndex
        {
            get
            {
                if (this.paletteIndexCache >= 0)
                {
                    return this.paletteIndexCache;
                }

                return this.Model.CollectionIndex;
            }
        }

        internal abstract ChartSeriesModel Model { get; }

        /// <summary>
        /// Gets the human-readable family of this instance. For example Area is the family for Area, SplineArea and PolarArea series.
        /// </summary>
        internal abstract string Family { get; }

        internal override Element Element
        {
            get
            {
                return this.Model;
            }
        }

        internal override int DefaultZIndex
        {
            get
            {
                return RadChartBase.SeriesZIndex + this.Model.Index;
            }
        }

        internal RadSize PlotAreaSize
        {
            get
            {
                if (this.chart == null)
                {
                    return RadSize.Empty;
                }

                ChartPlotAreaModel plotArea = this.chart.chartArea.plotArea;
                return new RadSize(plotArea.layoutSlot.Width, plotArea.layoutSlot.Height);
            }
        }

        /// <summary>
        /// Gets the labels. Exposed for testing purposes.
        /// </summary>
        internal List<KeyValuePair<DataPoint, List<FrameworkElement>>> Labels
        {
            get
            {
                return this.labels;
            }
        }

        /// <summary>
        /// A callback, raised by a data point visualized by this instance. Intended for internal use.
        /// </summary>
        void IChartSeries.OnDataPointIsSelectedChanged(DataPoint point)
        {
            this.OnDataPointSelectionChanged(point);
        }

        /// <summary>
        /// Gets all the <see cref="FrameworkElement"/> instances that represent labels, associated with the specified <see cref="DataPoint"/> instance.
        /// </summary>
        public IEnumerable<FrameworkElement> GetDataPointLabels(DataPoint point)
        {
            if (point.parent != this.Model)
            {
                throw new ArgumentException("Specified data point does not belong to this series.");
            }

            foreach (KeyValuePair<DataPoint, List<FrameworkElement>> pair in this.labels)
            {
                if (pair.Key != point)
                {
                    continue;
                }

                foreach (FrameworkElement label in pair.Value)
                {
                    yield return label;
                }
            }
        }

        internal static bool IsDefaultLabelVisual(FrameworkElement visual)
        {
            return visual is TextBlock;
        }

        internal virtual void OnDataPointSelectionChanged(DataPoint point)
        {
            this.InvalidatePalette();
        }

        internal virtual int GetPaletteIndexForPoint(DataPoint point)
        {
            return this.ActualPaletteIndex;
        }

        internal virtual void SetDynamicLegendTitle(string titlePath, string extractedValue)
        {
        }

        /// <summary>
        /// Called by the aggregated <see cref="ChartSeriesDataSource"/> instance upon a change of a property in a bound business object.
        /// </summary>
        internal void OnBoundItemPropertyChanged()
        {
            if (!this.showLabelsCache || this.IsInvalidated)
            {
                return;
            }

            // check whether we have bound labels
            foreach (ChartSeriesLabelDefinition definition in this.labelDefinitions)
            {
                if (definition.Binding != null)
                {
                    // we have bound labels, invalidate the visual scene
                    // TODO: Possible performance optimization - add logic for separate label invalidation mechanism
                    this.InvalidateCore();
                    break;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="DataBindingComplete"/> event.
        /// </summary>
        internal virtual void OnDataBindingComplete()
        {
            EventHandler eh = this.DataBindingComplete;
            if (eh != null)
            {
                eh(this, EventArgs.Empty);
            }
        }

        internal virtual void UpdateClip(ChartLayoutContext context)
        {
            this.plotOriginTransform.X = context.PlotOrigin.X;
            this.plotOriginTransform.Y = context.PlotOrigin.Y;

            if (this.renderSurface != null)
            {
                this.chart.ApplyPlotAreaClip(this.renderSurface, this.ClipToPlotArea);
            }

            if (this.labelLayer != null)
            {
                this.chart.ApplyPlotAreaClip(this.labelLayer, this.ClipToPlotArea);
            }
        }

        /// <summary>
        /// Updates of all of the chart elements presented by this instance.
        /// </summary>
        internal override void UpdateUICore(ChartLayoutContext context)
        {
            base.UpdateUICore(context);

            // hide the series if for some reason the chart is not setup properly
            this.Opacity = context.IsEmpty ? 0 : 1;

            if (this.showLabelsCache && this.labelLayer == null)
            {
                this.labelLayer = this.chart.AddLabelLayer(this);
                this.labelLayer.RenderTransform = this.plotOriginTransform;
            }

            this.UpdateLabels(context);

            this.UpdateClip(context);
            this.lastPlotAreaRect = this.chart.chartArea.plotArea.layoutSlot;
        }

        internal override void OnPlotOriginChanged(ChartLayoutContext context)
        {
            base.OnPlotOriginChanged(context);

            if (this.chart.chartArea.plotArea.layoutSlot != this.lastPlotAreaRect)
            {
                this.UpdateUICore(context);
            }
            else
            {
                this.UpdateClip(context);
            }
        }

        /// <summary>
        /// Creates the concrete data source for this instance.
        /// </summary>
        internal abstract ChartSeriesDataSource CreateDataSourceInstance();

        internal virtual FrameworkElement CreateDefaultLabelVisual(ChartSeriesLabelUpdateContext context)
        {
            if (context.Definition.Strategy != null)
            {
                if ((context.Definition.Strategy.Options & LabelStrategyOptions.DefaultVisual) == LabelStrategyOptions.DefaultVisual)
                {
                    return context.Definition.Strategy.CreateDefaultVisual(context.Point, context.DefinitionIndex);
                }
            }

            TextBlock block = new TextBlock();
            block.Style = context.Definition.DefaultVisualStyle;

            return block;
        }

        internal virtual RadSize MeasureLabel(FrameworkElement visual, ChartSeriesLabelUpdateContext context)
        {
            if (context.Definition.Strategy != null)
            {
                if ((context.Definition.Strategy.Options & LabelStrategyOptions.Measure) == LabelStrategyOptions.Measure)
                {
                    return context.Definition.Strategy.GetLabelDesiredSize(context.Point, visual, context.DefinitionIndex);
                }
            }

            return PresenterBase.MeasureVisual(visual);
        }

        internal virtual void ArrangeLabel(FrameworkElement visual, ChartSeriesLabelUpdateContext context)
        {
            RadSize size = this.MeasureLabel(visual, context);
            this.ArrangeUIElement(visual, GetLabelSlot(size, context));
        }

        internal virtual DataTemplate GetLabelTemplate(ChartSeriesLabelUpdateContext context)
        {
            DataTemplate template = null;
            if (context.Definition.Template != null)
            {
                template = context.Definition.Template;
            }
            else if (context.Definition.TemplateSelector != null)
            {
                template = context.Definition.TemplateSelector.SelectTemplate(context.Point, this);
            }

            return template;
        }

        internal virtual object GetLabelContent(ChartSeriesLabelUpdateContext context)
        {
            // we have a custom strategy that will provide a label content
            if (context.Definition.Strategy != null)
            {
                if ((context.Definition.Strategy.Options & LabelStrategyOptions.Content) == LabelStrategyOptions.Content)
                {
                    return context.Definition.Strategy.GetLabelContent(context.Point, context.DefinitionIndex);
                }
            }

            object label = null;

            // we have a label binding - it will work if we are data-bound (run-time only)
            if (context.Definition.Binding != null && !DesignMode.DesignModeEnabled)
            {
                if (!this.Model.isDataBound)
                {
                    throw new ArgumentException("ChartSeriesLabelDefinition.Binding is valid when owning series is data-bound.");
                }
                object dataItem = context.Point.dataItem;
                if (dataItem == null)
                {
                    Debug.Assert(false, string.Format("Failed to retrieve data item for data point at index {0}", context.Point.CollectionIndex));
                    return null;
                }

                label = context.Definition.Binding.GetValue(dataItem);
            }
            else
            {
                label = context.Point.Label;
            }

            if (label != null && !string.IsNullOrEmpty(context.Definition.Format))
            {
                return string.Format(CultureInfo.CurrentUICulture, context.Definition.Format, label);
            }

            return label;
        }

        internal virtual void SetLabelContent(FrameworkElement visual, ChartSeriesLabelUpdateContext context)
        {
            if (context.Definition.Strategy != null)
            {
                if ((context.Definition.Strategy.Options & LabelStrategyOptions.DefaultVisual) == LabelStrategyOptions.DefaultVisual)
                {
                    context.Definition.Strategy.SetLabelContent(context.Point, visual, context.DefinitionIndex);
                }
            }
        }

        internal virtual double GetDistanceToPoint(DataPoint dataPoint, Point tapLocation, ChartPointDistanceCalculationMode pointDistanceMode)
        {
            var dataPointLocation = dataPoint.Center();

            if (pointDistanceMode == ChartPointDistanceCalculationMode.TwoDimensional)
            {
                ////TODO: Math.Sqrt could lead to potential performance issues with lost of points/series.
                return RadMath.GetPointDistance(dataPointLocation.X, tapLocation.X, dataPointLocation.Y, tapLocation.Y);
            }
            else
            {
                AxisPlotDirection plotDirection = this.Model.GetTypedValue<AxisPlotDirection>(AxisModel.PlotDirectionPropertyKey, AxisPlotDirection.Vertical);

                if (plotDirection == AxisPlotDirection.Vertical)
                {
                    return Math.Abs(tapLocation.X - dataPointLocation.X);
                }
                else
                {
                    return Math.Abs(tapLocation.Y - dataPointLocation.Y);
                }
            }
        }

        /// <summary>
        /// Applies the pan transformation to the render surface template part and initializes any data bindings specified.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            if (applied)
            {
                this.renderSurface.RenderTransform = this.plotOriginTransform;
            }

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate"/> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            IEnumerable itemsSource = this.ItemsSource;
            if (itemsSource != null)
            {
                this.dataSource.ItemsSource = itemsSource;
            }

            if (DesignMode.DesignModeEnabled)
            {
                this.PrepareDesignTimeData();
            }
        }

        /// <summary>
        /// Occurs when the presenter has been successfully attached to its owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            this.chart.chartArea.Series.Add(this.Model);
        }

        /// <summary>
        /// Occurs when the presenter has been successfully detached from its owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected override void OnDetached(RadChartBase oldChart)
        {
            base.OnDetached(oldChart);

            if (oldChart != null)
            {
                if (oldChart.chartArea != null && oldChart.chartArea.Series != null)
                {
                    oldChart.chartArea.Series.Remove(this.Model);
                    if (this.labelLayer != null)
                    {
                        oldChart.RemoveLabelLayer(this);
                        this.labelLayer = null;
                    }
                }
                //// TODO: Consider throwing exception if needed.
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ChartSeriesAutomationPeer(this);
        }

        private static void OnVisibilityWatcherChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.chart != null)
            {
                series.chart.chartArea.InvalidateCore(ChartAreaInvalidateFlags.All);
            }

            series.InvalidateCore();
        }

        private static void OnShowLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            series.showLabelsCache = (bool)e.NewValue;
            series.InvalidateCore();
        }

        private static void OnClipToPlotAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            series.InvalidateCore();
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.IsTemplateApplied)
            {
                series.dataSource.ItemsSource = e.NewValue as IEnumerable;
            }
        }

        private static void OnIsSelectedBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            series.dataSource.IsSelectedBinding = e.NewValue as DataPointBinding;
        }

        private static void OnPaletteIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            series.InvalidatePalette();
            series.paletteIndexCache = (int)e.NewValue;
        }

        private static ChartSeriesLabelDefinition CreateDefaultLabelDefinition(AxisPlotDirection plotDirection)
        {
            if (plotDirection == AxisPlotDirection.Horizontal)
            {
                return new ChartSeriesLabelDefinition()
                {
                    Margin = new Thickness(10, 0, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }

            return new ChartSeriesLabelDefinition()
            {
                Margin = new Thickness(0, 0, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top
            };
        }

        private static RadRect GetLabelSlot(RadSize labelSize, ChartSeriesLabelUpdateContext context)
        {
            double x = double.NaN;
            double y = double.NaN;

            Thickness margin = context.Definition.Margin;
            RadRect pointSlot = context.Point.layoutSlot;

            switch (context.Definition.HorizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    x = pointSlot.X + ((pointSlot.Width - labelSize.Width) / 2) + margin.Left - margin.Right;
                    break;
                case HorizontalAlignment.Stretch:
                    x = pointSlot.X + ((pointSlot.Width - labelSize.Width) / 2);
                    break;
                case HorizontalAlignment.Left:
                    // positive point with regular axis is equivalent to negative point with inverse axis
                    if (context.PlotDirection == AxisPlotDirection.Horizontal && !(context.Point.isPositive ^ context.IsPlotInverse))
                    {
                        // swap Left with Right due to negative values
                        x = pointSlot.Right - margin.Left + margin.Right;
                    }
                    else
                    {
                        x = pointSlot.X - labelSize.Width - margin.Right + margin.Left;
                    }
                    break;
                case HorizontalAlignment.Right:
                    // positive point with regular axis is equivalent to negative point with inverse axis
                    if (context.PlotDirection == AxisPlotDirection.Horizontal && !(context.Point.isPositive ^ context.IsPlotInverse))
                    {
                        // swap Left with Right due to negative values
                        x = pointSlot.X - labelSize.Width - margin.Left + margin.Right;
                    }
                    else
                    {
                        x = pointSlot.Right + margin.Left - margin.Right;
                    }
                    break;
            }

            switch (context.Definition.VerticalAlignment)
            {
                case VerticalAlignment.Center:
                    y = pointSlot.Y + ((pointSlot.Height - labelSize.Height) / 2) + margin.Top - margin.Bottom;
                    break;
                case VerticalAlignment.Stretch:
                    y = pointSlot.Y + ((pointSlot.Height - labelSize.Height) / 2);
                    break;
                case VerticalAlignment.Bottom:
                    // positive point with regular axis is equivalent to negative point with inverse axis
                    if (context.PlotDirection == AxisPlotDirection.Vertical && !(context.Point.isPositive ^ context.IsPlotInverse))
                    {
                        // swap Bottom with Top due to negative values
                        y = pointSlot.Y - labelSize.Height - margin.Top + margin.Bottom;
                    }
                    else
                    {
                        y = pointSlot.Bottom + margin.Top - margin.Bottom;
                    }
                    break;
                case VerticalAlignment.Top:
                    // positive point with regular axis is equivalent to negative point with inverse axis
                    if (context.PlotDirection == AxisPlotDirection.Vertical && !(context.Point.isPositive ^ context.IsPlotInverse))
                    {
                        // swap Top with Bottom due to negative values
                        y = pointSlot.Bottom + margin.Bottom - margin.Top;
                    }
                    else
                    {
                        y = pointSlot.Y - labelSize.Height - margin.Bottom + margin.Top;
                    }
                    break;
            }

            return new RadRect(x, y, labelSize.Width, labelSize.Height);
        }

        private void BeginUpdate()
        {
            this.Model.canModifyDataPoints = true;
        }

        private void EndUpdate()
        {
            this.Model.canModifyDataPoints = false;
            this.Model.isDataBound = this.ItemsSource != null;
        }

        private void OnLabelDefinitionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.InvalidateCore();
        }

        private FrameworkElement CreateLabelVisual(ChartSeriesLabelUpdateContext context)
        {
            FrameworkElement visual;
            DataTemplate template = this.GetLabelTemplate(context);

            if (template != null)
            {
                visual = new ContentPresenter();
            }
            else
            {
                visual = this.CreateDefaultLabelVisual(context);
                if (visual == null)
                {
                    throw new ArgumentNullException("Label default visual cannot be null.");
                }
            }

            this.labelLayer.Children.Add(visual);

            return visual;
        }

        private FrameworkElement GetLabelVisual(ChartSeriesLabelUpdateContext context)
        {
            FrameworkElement element;
            List<FrameworkElement> pointLabels;

            if (context.PointVirtualIndex < this.labels.Count)
            {
                pointLabels = this.labels[context.PointVirtualIndex].Value;
                if (pointLabels == null)
                {
                    Debug.Assert(false, "Must have list of label visuals created at this point");
                    return null;
                }

                if (pointLabels.Count > context.DefinitionIndex)
                {
                    element = pointLabels[context.DefinitionIndex];
                    element.Visibility = Visibility.Visible;
                }
                else
                {
                    element = this.CreateLabelVisual(context);
                    pointLabels.Add(element);
                }

                if (IsDefaultLabelVisual(element))
                {
                    element.Style = context.Definition.DefaultVisualStyle;
                }
            }
            else
            {
                element = this.CreateLabelVisual(context);
                pointLabels = new List<FrameworkElement>();
                pointLabels.Add(element);
                this.labels.Add(new KeyValuePair<DataPoint, List<FrameworkElement>>(context.Point, pointLabels));
            }

            DataTemplate labelTemplate = this.GetLabelTemplate(context);

            // NOTE: When Template or TemplateSelector is defined, the DataContext of the item labels' DataTemplate 
            // should be the respective DataPoint and not just calculated value.
            if (labelTemplate != null)
            {
                // Assumes ContentPresenter has been created to handle this case.
                // NOTE: this will override any ChartSeriesLabelDefinition.Binding logic now (if both Binding and Template/TemplateSelector are set)
                ContentPresenter presenter = element as ContentPresenter;
                presenter.Content = context.Point;
                presenter.ContentTemplate = labelTemplate;
            }
            else
            {
                TextBlock textblock = element as TextBlock;
                if (textblock != null)
                {
                    // Assumes TextBlock has been created to handle the default case.
                    object label = this.GetLabelContent(context);
                    textblock.Text = label == null ? string.Empty : label.ToString();
                }
                else
                {
                    // Assumes if user-defined label strategy is involved in visual creation (even if the strategy creates ContentPresenters),
                    // the strategy should be responsible for setting its content (via ChartSeriesLabelStrategy.SetLabelContent(...) method override)
                    this.SetLabelContent(element, context);
                }
            }

            return element;
        }

        private void UpdateLabels(ChartLayoutContext context)
        {
            int index = 0;
            ChartSeriesModel series = this.Model;
            ChartSeriesLabelUpdateContext labelContext = new ChartSeriesLabelUpdateContext();
            labelContext.PlotDirection = this.Model.GetTypedValue<AxisPlotDirection>(AxisModel.PlotDirectionPropertyKey, AxisPlotDirection.Vertical);
            labelContext.IsPlotInverse = this.Model.GetIsPlotInverse(labelContext.PlotDirection);

            if (this.showLabelsCache)
            {
                foreach (DataPoint point in series.DataPointsInternal)
                {
                    // point is empty or is laid-out outside the clip area, skip it from visualization.
                    if (point.isEmpty || !context.ClipRect.IntersectsWith(point.layoutSlot))
                    {
                        continue;
                    }

                    labelContext.Point = point;
                    labelContext.PointVirtualIndex = index;
                    this.ProcessDataPointLabels(labelContext);
                    index++;
                }
            }

            while (index < this.labels.Count)
            {
                List<FrameworkElement> pointLabels = this.labels[index].Value;
                foreach (FrameworkElement label in pointLabels)
                {
                    label.Visibility = Visibility.Collapsed;
                }
                index++;
            }
        }

        private void ProcessDataPointLabels(ChartSeriesLabelUpdateContext context)
        {
            // we have user-defined definition(s)
            if (this.labelDefinitions.Count > 0)
            {
                int index = 0;
                foreach (ChartSeriesLabelDefinition definition in this.labelDefinitions)
                {
                    context.Definition = definition;
                    context.DefinitionIndex = index;
                    this.ProcessLabelDefinition(context);
                    index++;
                }
            }
            else
            {
                context.Definition = CreateDefaultLabelDefinition(context.PlotDirection);
                this.ProcessLabelDefinition(context);
            }
        }

        private void ProcessLabelDefinition(ChartSeriesLabelUpdateContext context)
        {
            FrameworkElement visual = this.GetLabelVisual(context);
            if (visual == null)
            {
                Debug.Assert(false, "No label visual created.");
                return;
            }

            if (context.Definition.Strategy != null)
            {
                if ((context.Definition.Strategy.Options & LabelStrategyOptions.Arrange) == LabelStrategyOptions.Arrange)
                {
                    RadRect labelSlot = context.Definition.Strategy.GetLabelLayoutSlot(context.Point, visual, context.DefinitionIndex);
                    this.ArrangeUIElement(visual, labelSlot);
                    return;
                }
            }

            this.ArrangeLabel(visual, context);
        }

        partial void PrepareDesignTimeData();
    }
}