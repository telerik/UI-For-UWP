using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Telerik.Charting;
using Telerik.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Base class for all <see cref="ChartSeries"/> that may visualize their data points through Data templates.
    /// </summary>
    public abstract partial class PointTemplateSeries : ChartSeries
    {
        /// <summary>
        /// Identifies the <see cref="PointTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty PointTemplateProperty =
            DependencyProperty.Register(nameof(PointTemplate), typeof(DataTemplate), typeof(PointTemplateSeries), new PropertyMetadata(null, OnPointTemplateChanged));

        /// <summary>
        /// Identifies the <see cref="PointTemplateSelector"/> property.
        /// </summary>
        public static readonly DependencyProperty PointTemplateSelectorProperty =
            DependencyProperty.Register(nameof(PointTemplateSelector), typeof(DataTemplateSelector), typeof(PointTemplateSeries), new PropertyMetadata(null, OnPointTemplateSelectorChanged));

        /// <summary>
        /// Identifies the <see cref="DefaultVisualStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty DefaultVisualStyleProperty =
            DependencyProperty.Register(nameof(DefaultVisualStyle), typeof(Style), typeof(PointTemplateSeries), new PropertyMetadata(null, OnDefaultVisualStyleChanged));

        internal DataTemplate pointTemplateCache;
        internal DataTemplateSelector pointTemplateSelectorCache;
        internal List<FrameworkElement> realizedDataPointPresenters;
        internal Style defaultVisualStyleCache;

        private RadSize defaultVisualSize = RadSize.Invalid;
        private ContentPresenter measurementPresenter;
        private ObservableCollection<DataTemplate> pointTemplates;
        private Dictionary<DataTemplate, RadSize> pointSizeCache = new Dictionary<DataTemplate, RadSize>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PointTemplateSeries"/> class.
        /// </summary>
        protected PointTemplateSeries()
        {
            this.realizedDataPointPresenters = new List<FrameworkElement>();
            this.pointTemplates = new ObservableCollection<DataTemplate>();
            this.pointTemplates.CollectionChanged += this.OnPointTemplatesChanged;

            this.measurementPresenter = new ContentPresenter();
            this.measurementPresenter.Opacity = 0;
            this.measurementPresenter.IsHitTestVisible = false;
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> instance that will define the appearance of series' default visuals (if any).
        /// For example a BarSeries will create <see cref="Border"/> instances as its default visuals.
        /// Point templates (if specified) however have higher precedence compared to the default visuals.
        /// </summary>
        public Style DefaultVisualStyle
        {
            get
            {
                return this.defaultVisualStyleCache;
            }
            set
            {
                this.SetValue(DefaultVisualStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> property used to visualize each <see cref="PointTemplateSeries"/> presented.
        /// </summary>
        public DataTemplate PointTemplate
        {
            get
            {
                return this.pointTemplateCache;
            }
            set
            {
                this.SetValue(PointTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplateSelector"/> property used to provide conditional <see cref="DataTemplate"/> look-up when visualizing each data point presented.
        /// </summary>
        public DataTemplateSelector PointTemplateSelector
        {
            get
            {
                return this.pointTemplateSelectorCache;
            }
            set
            {
                this.SetValue(PointTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets the collection that stores index-based templates for each data point.
        /// </summary>
        public ObservableCollection<DataTemplate> PointTemplates
        {
            get
            {
                return this.pointTemplates;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the series can have default visuals, created without an explicit DataTemplate. Such series currently are BarSeries.
        /// </summary>
        internal virtual bool SupportsDefaultVisuals
        {
            get
            {
                return false;
            }
        }

        internal virtual RadSize DefaultVisualSize
        {
            get
            {
                return RadSize.Empty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a valid DataTemplate instance to present each DataPoint exists.
        /// </summary>
        private bool HasPointTemplate
        {
            get
            {
                if (this.pointTemplateCache != null || this.pointTemplateSelectorCache != null)
                {
                    return true;
                }

                return this.pointTemplates.Count > 0;
            }
        }

        /// <summary>
        /// Gets the <see cref="UIElement"/> instance used to visualize the corresponding data point.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "A right level of abstraction is required.")]
        public UIElement GetDataPointVisual(DataPoint point)
        {
            if (!this.IsLoaded || !this.IsTemplateApplied)
            {
                return null;
            }

            if (point == null)
            {
                throw new NullReferenceException("Specified data point is NULL.");
            }
            else if (point.parent != this.Model)
            {
                throw new ArgumentException("Specified data point does not belong to this series.");
            }

            foreach (FrameworkElement element in this.realizedDataPointPresenters)
            {
                if (element.Tag == point)
                {
                    return element;
                }
            }

            return null;
        }

        internal static RadSize GetSizeFromStyle(Style style)
        {
            double width = 0;
            double height = 0;
            double value;

            foreach (Setter setter in style.Setters)
            {
                if (setter.Property == FrameworkElement.WidthProperty ||
                    setter.Property == FrameworkElement.MinWidthProperty)
                {
                    if (NumericConverter.TryConvertToDouble(setter.Value, out value))
                    {
                        width = value;
                    }
                }
                else if (setter.Property == FrameworkElement.HeightProperty ||
                    setter.Property == FrameworkElement.MinHeightProperty)
                {
                    if (NumericConverter.TryConvertToDouble(setter.Value, out value))
                    {
                        height = value;
                    }
                }
            }

            return new RadSize(width, height);
        }

        internal override void InvalidateCore()
        {
            this.defaultVisualSize = RadSize.Invalid;

            base.InvalidateCore();
        }

        /// <summary>
        /// Updates of all of the chart elements presented by this instance.
        /// </summary>
        internal override void UpdateUICore(ChartLayoutContext context)
        {
            if (this.renderSurface == null)
            {
                return;
            }

            base.UpdateUICore(context);

            this.UpdatePresenters(context);
        }

        internal override void OnUIUpdated()
        {
            base.OnUIUpdated();

            this.UpdateLegendItems();
        }

        internal override void OnPlotOriginChanged(ChartLayoutContext context)
        {
            // TODO: Possible optimizations
            this.UpdateUICore(context);
        }

        internal override void ApplyPaletteCore()
        {
            foreach (FrameworkElement visual in this.realizedDataPointPresenters)
            {
                if (!this.IsDefaultVisual(visual))
                {
                    continue;
                }

                DataPoint point = visual.Tag as DataPoint;
                this.ApplyPaletteToDefaultVisual(visual, point);
            }
        }

        internal virtual void SetDefaultVisualContent(FrameworkElement visual, DataPoint point)
        {
        }

        internal virtual void ApplyPaletteToDefaultVisual(FrameworkElement visual, DataPoint point)
        {
        }

        internal virtual FrameworkElement CreateDefaultDataPointVisual(DataPoint point)
        {
            return null;
        }

        internal virtual bool IsDefaultVisual(FrameworkElement visual)
        {
            return false;
        }

        internal virtual void SetDefaultVisualStyle(FrameworkElement visual)
        {
            visual.Style = this.defaultVisualStyleCache;
        }

        /// <summary>
        /// Core entry point for calculating the size of a node's content.
        /// </summary>
        protected internal override RadSize MeasureNodeOverride(Node node, object content)
        {
            var dataPoint = node as DataPoint;
            if (dataPoint != null)
            {
                return this.MeasureDataPoint(dataPoint);
            }
            return base.MeasureNodeOverride(node, content);
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            if (this.renderSurface != null)
            {
                this.renderSurface.Children.Remove(this.measurementPresenter);
            }
        }

        /// <summary>
        /// Adds the <see cref="ContentPresenter"/> instance used to retrieve the desired size of each chart model.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            if (applied)
            {
                this.renderSurface.Children.Add(this.measurementPresenter);
            }

            return applied;
        }

        /// <summary>
        /// Occurs when the presenter has been successfully attached to its owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (this.IsVisibleInLegend)
            {
                foreach (var item in this.LegendItems)
                {
                    this.Chart.LegendInfosInternal.Add(item);
                }
            }
        }

        /// <summary>
        /// Occurs when the presenter has been successfully detached from its owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected override void OnDetached(RadChartBase oldChart)
        {
            base.OnDetached(oldChart);

            if (oldChart != null)
            {
                foreach (var item in this.LegendItems)
                {
                    oldChart.LegendInfosInternal.Remove(item);
                }
            }
        }

        private static void OnDefaultVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PointTemplateSeries presenter = d as PointTemplateSeries;
            presenter.defaultVisualStyleCache = e.NewValue as Style;
            presenter.UpdateDefaultVisualsStyle();
        }

        private static void OnPointTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PointTemplateSeries presenter = d as PointTemplateSeries;
            presenter.pointTemplateCache = (DataTemplate)e.NewValue;
            presenter.InvalidatePointTemplates();
        }

        private static void OnPointTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PointTemplateSeries presenter = d as PointTemplateSeries;
            presenter.pointTemplateSelectorCache = (DataTemplateSelector)e.NewValue;
            presenter.InvalidatePointTemplates();
        }

        private void OnPointTemplatesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.InvalidatePointTemplates();
        }

        private void InvalidatePointTemplates()
        {
            this.pointSizeCache.Clear();

            // Toggling point templates on/off should force the data points to re-measure themselves.
            foreach (DataPoint dataPoint in this.Model.DataPointsInternal)
            {
                dataPoint.desiredSize = RadSize.Invalid;
            }

            foreach (FrameworkElement realizedDataPoint in this.realizedDataPointPresenters)
            {
                this.renderSurface.Children.Remove(realizedDataPoint);
            }
            this.realizedDataPointPresenters.Clear();

            this.InvalidateCore();
        }

        private void UpdatePresenters(ChartLayoutContext context)
        {
            int index = 0;
            ChartSeriesModel series = this.Model;

            if (this.chart.chartArea.IsTreeLoaded && (this.HasPointTemplate || this.SupportsDefaultVisuals))
            {
                foreach (DataPoint point in series.DataPointsInternal)
                {
                    // point is empty or is laid-out outside the clip area, skip it from visualization.
                    if (point.isEmpty || !context.ClipRect.IntersectsWith(point.layoutSlot))
                    {
                        continue;
                    }

                    FrameworkElement element = this.GetDataPointVisual(point, index);
                    if (element != null)
                    {
                        this.ArrangeUIElement(element, point.layoutSlot);
                        index++;
                    }
                }
            }

            while (index < this.realizedDataPointPresenters.Count)
            {
                var presenter = this.realizedDataPointPresenters[index];
                if (presenter.Visibility == Visibility.Visible)
                    presenter.Visibility = Visibility.Collapsed;
                index++;
            }
        }

        private FrameworkElement GetDataPointVisual(DataPoint point, int virtualIndex)
        {
            FrameworkElement visual;

            if (virtualIndex < this.realizedDataPointPresenters.Count)
            {
                visual = this.realizedDataPointPresenters[virtualIndex];
                visual.Visibility = Visibility.Visible;
                ContentPresenter presenter = visual as ContentPresenter;
                if (presenter != null)
                {
                    presenter.Content = point;
                    presenter.ContentTemplate = this.GetDataTemplate(point);
                }
                else if (this.IsDefaultVisual(visual))
                {
                    this.SetDefaultVisualContent(visual, point);
                }
            }
            else
            {
                visual = this.CreateDataPointVisual(point);
            }

            if (visual != null)
            {
                visual.Tag = point;
            }

            return visual;
        }

        private FrameworkElement CreateDataPointVisual(DataPoint point)
        {
            FrameworkElement visual;
            DataTemplate template = this.GetDataTemplate(point);

            if (template != null)
            {
                visual = this.CreateContentPresenter(point, template);
            }
            else
            {
                visual = this.CreateDefaultDataPointVisual(point);
                if (visual != null)
                {
                    visual.Style = this.defaultVisualStyleCache;
                    this.renderSurface.Children.Add(visual);
                    this.SetDefaultVisualContent(visual, point);
                }
            }

            if (visual != null)
            {
                this.realizedDataPointPresenters.Add(visual);
            }

            return visual;
        }

        private DataTemplate GetDataTemplate(DataPoint dataPoint)
        {
            DataTemplate template = null;
            if (this.pointTemplateCache != null)
            {
                template = this.pointTemplateCache;
            }
            else if (this.pointTemplates.Count > 0)
            {
                template = this.pointTemplates[dataPoint.CollectionIndex % this.pointTemplates.Count];
            }
            else if (this.pointTemplateSelectorCache != null)
            {
                template = this.pointTemplateSelectorCache.SelectTemplate(dataPoint, this);
            }

            return template;
        }

        private RadSize MeasureDataPoint(DataPoint point)
        {
            if (point.desiredSize != RadSize.Invalid)
            {
                return point.desiredSize;
            }

            DataTemplate template = this.GetDataTemplate(point);
            if (template == null)
            {
                if (this.SupportsDefaultVisuals)
                {
                    return this.GetDefaultVisualSize();
                }

                return RadSize.Empty;
            }

            RadSize cachedSize;
            if (this.pointSizeCache.TryGetValue(template, out cachedSize))
            {
                return cachedSize;
            }

            this.measurementPresenter.Content = point;
            this.measurementPresenter.ContentTemplate = template;

            this.measurementPresenter.ClearValue(FrameworkElement.WidthProperty);
            this.measurementPresenter.ClearValue(FrameworkElement.HeightProperty);

            this.measurementPresenter.Measure(RadChartBase.InfinitySize);

            RadSize size = new RadSize((int)(this.measurementPresenter.DesiredSize.Width + .5), (int)(this.measurementPresenter.DesiredSize.Height + .5));

            // cache the size
            this.pointSizeCache.Add(template, size);

            return size;
        }

        private void UpdateDefaultVisualsStyle()
        {
            foreach (FrameworkElement visual in this.realizedDataPointPresenters)
            {
                if (this.IsDefaultVisual(visual))
                {
                    this.SetDefaultVisualStyle(visual);
                }
            }

            this.UpdatePalette(true);
        }

        private RadSize GetDefaultVisualSize()
        {
            if (this.defaultVisualSize == RadSize.Invalid)
            {
                if (this.defaultVisualStyleCache != null)
                {
                    this.defaultVisualSize = GetSizeFromStyle(this.defaultVisualStyleCache);
                }
            }

            if (this.defaultVisualSize == RadSize.Empty)
            {
                this.defaultVisualSize = this.DefaultVisualSize;
            }

            return this.defaultVisualSize;
        }
    }
}
