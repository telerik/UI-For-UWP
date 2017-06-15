using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Telerik.Charting;
using Telerik.Core;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// This class represents the MovingAverageConvergenceDivergence financial indicator. 
    /// </summary>
    public class MacdhIndicator : BarIndicatorBase
    {
        /// <summary>
        /// Identifies the <see cref="PointTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty PointTemplateProperty =
            DependencyProperty.Register(nameof(PointTemplate), typeof(DataTemplate), typeof(MacdhIndicator), new PropertyMetadata(null, OnPointTemplateChanged));

        /// <summary>
        /// Identifies the <see cref="LongPeriod"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LongPeriodProperty =
            DependencyProperty.Register(nameof(LongPeriod), typeof(int), typeof(MacdhIndicator), new PropertyMetadata(0, OnLongPeriodChanged));

        /// <summary>
        /// Identifies the <see cref="ShortPeriod"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShortPeriodProperty =
            DependencyProperty.Register(nameof(ShortPeriod), typeof(int), typeof(MacdhIndicator), new PropertyMetadata(0, OnShortPeriodChanged));

        /// <summary>
        /// Identifies the <see cref="SignalPeriod"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SignalPeriodProperty =
            DependencyProperty.Register(nameof(SignalPeriod), typeof(int), typeof(MacdhIndicator), new PropertyMetadata(0, OnSignalPeriodChanged));

        /// <summary>
        /// Identifies the <see cref="PointTemplateSelector"/> property.
        /// </summary>
        public static readonly DependencyProperty PointTemplateSelectorProperty =
            DependencyProperty.Register(nameof(PointTemplateSelector), typeof(DataTemplateSelector), typeof(MacdhIndicator), new PropertyMetadata(null, OnPointTemplateSelectorChanged));

        internal DataTemplate pointTemplateCache;
        internal DataTemplateSelector pointTemplateSelectorCache;

        private ObservableCollection<DataTemplate> pointTemplates;
        private Dictionary<DataTemplate, RadSize> pointSizeCache = new Dictionary<DataTemplate, RadSize>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MacdhIndicator"/> class.
        /// </summary>
        public MacdhIndicator()
        {
            this.DefaultStyleKey = typeof(MacdhIndicator);

            this.pointTemplates = new ObservableCollection<DataTemplate>();
            this.pointTemplates.CollectionChanged += this.OnPointTemplatesChanged;

            this.model = new BarSeriesModel();
        }

        /// <summary>
        /// Gets or sets the indicator long period.
        /// </summary>
        /// <value>The long period.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "long")]
        public int LongPeriod
        {
            get
            {
                return (int)this.GetValue(LongPeriodProperty);
            }
            set
            {
                this.SetValue(LongPeriodProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the indicator short period.
        /// </summary>
        /// <value>The short period.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "short")]
        public int ShortPeriod
        {
            get
            {
                return (int)this.GetValue(ShortPeriodProperty);
            }
            set
            {
                this.SetValue(ShortPeriodProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the indicator signal period.
        /// </summary>
        /// <value>The signal period.</value>
        public int SignalPeriod
        {
            get
            {
                return (int)this.GetValue(SignalPeriodProperty);
            }
            set
            {
                this.SetValue(SignalPeriodProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> property used to visualize each <see cref="MacdhIndicator"/> presented.
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

        internal override ChartSeriesModel Model
        {
            get
            {
                return this.model;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        internal bool SupportsDefaultVisuals
        {
            get
            {
                return true;
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
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        public override string ToString()
        {
            return "Moving Average Convergence Divergence Histogram (" + this.LongPeriod + ", " + this.ShortPeriod + ", " + this.SignalPeriod + ")";
        }

        internal static FrameworkElement CreateDefaultDataPointVisual()
        {
            return new Border();
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new MacdhIndicatorDataSource();
        }

        /// <summary>
        /// Updates of all of the chart elements presented by this instance.
        /// </summary>
        internal override void UpdateUICore(ChartLayoutContext context)
        {
            base.UpdateUICore(context);

            this.UpdatePresenters(context);
        }

        internal override void OnPlotOriginChanged(ChartLayoutContext context)
        {
            this.UpdateUICore(context);
        }

        internal virtual void SetDefaultVisualContent(FrameworkElement visual, DataPoint point)
        {
        }

        private static void OnLongPeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MacdhIndicator presenter = d as MacdhIndicator;
            (presenter.dataSource as MacdhIndicatorDataSource).LongPeriod = (int)e.NewValue;
        }

        private static void OnShortPeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MacdhIndicator presenter = d as MacdhIndicator;
            (presenter.dataSource as MacdhIndicatorDataSource).ShortPeriod = (int)e.NewValue;
        }

        private static void OnSignalPeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MacdhIndicator presenter = d as MacdhIndicator;
            (presenter.dataSource as MacdhIndicatorDataSource).SignalPeriod = (int)e.NewValue;
        }

        private static void OnPointTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MacdhIndicator presenter = d as MacdhIndicator;
            presenter.pointTemplateCache = (DataTemplate)e.NewValue;
            presenter.InvalidatePointTemplates();
        }

        private static void OnPointTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MacdhIndicator presenter = d as MacdhIndicator;
            presenter.pointTemplateSelectorCache = (DataTemplateSelector)e.NewValue;
            presenter.InvalidatePointTemplates();
        }

        private void InvalidatePointTemplates()
        {
            this.pointSizeCache.Clear();
            this.InvalidateCore();
        }

        private void UpdatePresenters(ChartLayoutContext context)
        {
            int index = 0;
            ChartSeriesModel series = this.Model;

            if (this.HasPointTemplate || this.SupportsDefaultVisuals)
            {
                foreach (DataPoint point in series.DataPointsInternal)
                {
                    // point is laid-out outside the clip area, skip it from visualization
                    if (!context.ClipRect.IntersectsWith(point.layoutSlot))
                    {
                        continue;
                    }

                    if (this.drawWithComposition)
                    {
                        var containerVisual = this.GetContainerVisual(index);
                        this.chart.ContainerVisualsFactory.PrepareBarIndicatorVisual(containerVisual, point);
                        index++;
                    }
                    else
                    {
                        FrameworkElement element = this.GetDataPointVisual(point, index);
                        if (element != null)
                        {
                            this.ArrangeUIElement(element, point.layoutSlot);
                            index++;
                        }
                    }
                }
            }

            if (this.drawWithComposition)
            {
                while (index < this.realizedVisualDataPoints.Count)
                {
                    this.realizedVisualDataPoints[index].IsVisible = false;
                    index++;
                }
            }
            else
            {
                while (index < this.realizedDataPoints.Count)
                {
                    this.realizedDataPoints[index].Visibility = Visibility.Collapsed;
                    index++;
                }
            }
        }
        

        private FrameworkElement GetDataPointVisual(DataPoint point, int virtualIndex)
        {
            FrameworkElement visual;

            if (virtualIndex < this.realizedDataPoints.Count)
            {
                visual = this.realizedDataPoints[virtualIndex];
                visual.Visibility = Visibility.Visible;
                ContentPresenter presenter = visual as ContentPresenter;
                if (presenter != null)
                {
                    presenter.Content = point;
                    presenter.ContentTemplate = this.GetDataTemplate(point);
                }
                else if (BarIndicatorBase.IsDefaultVisual(visual))
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

        private ContainerVisual GetContainerVisual(int index)
        {
            ContainerVisual visual;

            if (index < this.realizedDataPoints.Count)
            {
                visual = this.realizedVisualDataPoints[index];
                if (!visual.IsVisible)
                {
                    visual.IsVisible = true;
                }
            }
            else
            {
                visual = this.chart.ContainerVisualsFactory.CreateContainerVisual(this.Compositor, this.GetType());
                this.realizedVisualDataPoints.Add(visual);
                this.ContainerVisualRoot.Children.InsertAtBottom(visual);
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
                visual = CreateDefaultDataPointVisual();
                if (visual != null)
                {
                    visual.Style = this.defaultVisualStyleCache;
                    this.renderSurface.Children.Add(visual);
                    this.SetDefaultVisualContent(visual, point);
                }
            }

            if (visual != null)
            {
                this.realizedDataPoints.Add(visual);
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

        private void OnPointTemplatesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.InvalidatePointTemplates();
        }
    }
}
