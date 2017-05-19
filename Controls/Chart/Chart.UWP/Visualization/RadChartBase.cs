using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Telerik.Charting;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Base class for all different charts. Different chart controls are categorized mainly by the coordinate system used to plot their points.
    /// </summary>
    [TemplatePart(Name = "PART_LayoutRoot", Type = typeof(ControlTemplate))]
    [TemplatePart(Name = "PART_RenderSurface", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_PlotAreaDecoration", Type = typeof(Border))]
    [TemplatePart(Name = "PART_LabelLayer", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_AdornerLayer", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_EmptyContentPresenter", Type = typeof(ContentPresenter))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The control is quite complex.")]
    public abstract partial class RadChartBase : PresenterBase, IChartView, ILegendInfoProvider, INoDesiredSizeControl
    {
        /// <summary>
        /// Identifies the <see cref="SeriesProvider"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SeriesProviderProperty =
            DependencyProperty.Register(nameof(SeriesProvider), typeof(ChartSeriesProvider), typeof(RadChartBase), new PropertyMetadata(null, OnSeriesProviderChanged));

        /// <summary>
        /// Identifies the <see cref="Palette"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaletteProperty =
            DependencyProperty.Register(nameof(Palette), typeof(ChartPalette), typeof(RadChartBase), new PropertyMetadata(null, OnPaletteChanged));

        /// <summary>
        /// Identifies the <see cref="PaletteName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaletteNameProperty =
            DependencyProperty.Register(nameof(PaletteName), typeof(PredefinedPaletteName), typeof(RadChartBase), new PropertyMetadata(PredefinedPaletteName.None, OnPredefinedPaletteNameChanged));

        /// <summary>
        /// Identifies the <see cref="SelectionPaletteName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionPaletteNameProperty =
            DependencyProperty.Register(nameof(SelectionPaletteName), typeof(PredefinedPaletteName), typeof(RadChartBase), new PropertyMetadata(PredefinedPaletteName.None, OnPredefinedSelectionPaletteNameChanged));

        /// <summary>
        /// Identifies the <see cref="SelectionPalette"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionPaletteProperty =
            DependencyProperty.Register(nameof(SelectionPalette), typeof(ChartPalette), typeof(RadChartBase), new PropertyMetadata(null, OnSelectionPaletteChanged));

        /// <summary>
        /// Identifies the <see cref="PlotAreaStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlotAreaStyleProperty =
            DependencyProperty.Register(nameof(PlotAreaStyle), typeof(Style), typeof(RadChartBase), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="EmptyContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EmptyContentProperty =
            DependencyProperty.Register(nameof(EmptyContent), typeof(object), typeof(RadChartBase), new PropertyMetadata(null, OnEmptyContentChanged));

        /// <summary>
        /// Identifies the <see cref="EmptyContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EmptyContentTemplateProperty =
            DependencyProperty.Register(nameof(EmptyContentTemplate), typeof(DataTemplate), typeof(RadChartBase), new PropertyMetadata(null));

        internal const string NoSeriesKey = "NoSeries";
        internal const string NoDataKey = "NoData";

        internal const int BackgroundZIndex = -1;
        internal const int SeriesZIndex = 100;
        internal const int IndicatorZIndex = 200;
        internal const int AnnotationZIndex = 250;
        internal const int TrackBallInfoControlZIndex = 350;

        internal ChartAreaModel chartArea;
        internal Size availableSize;
        internal StackedSeriesContext StackedSeriesContext;
        internal bool invalidateScheduled;
        internal Canvas adornerLayer;
        internal Canvas labelLayer;
        internal Border layoutRoot;
        internal Border plotAreaBackground;
        internal ContentPresenter emptyContentPresenter;
        internal ChartPalette paletteCache;
        internal ChartPalette selectionPaletteCache;

        private const string LayoutRootPartName = "PART_LayoutRoot";
        private const string AdornerLayerPartName = "PART_AdornerLayer";
        private const string LabelLayerPartName = "PART_LabelLayer";
        private const string PlotAreaDecorationPartName = "PART_PlotAreaDecoration";
        private const string EmptyContentPresenterPartName = "PART_EmptyContentPresenter";

        private List<ChartElementPresenter> unattachedPresenters;
        private bool arrangePassed;
        private bool clipToBounds;

        // Special flag to postpone UpdateUI pass if a Layout Pass in the framework is pending. See OnPresenterAdded for more information
        private bool skipInvalidated;

        private bool userEmptyContent;
        private RadRect previousPlotAreaClip = RadRect.Empty;
        private bool rendered;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadChartBase"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "These virtual calls do not rely on uninitialized base state.")]
        protected RadChartBase()
        {
            this.clipToBounds = true;

            this.chartArea = this.CreateChartAreaModel();
            this.unattachedPresenters = new List<ChartElementPresenter>();
            this.StackedSeriesContext = new StackedSeriesContext();

            // hook events
            this.SizeChanged += this.OnSizeChanged;

            this.InitManipulation();
        }

        /// <summary>
        /// Gets or sets an object that may be used to create chart series dynamically, depending on the underlying data.
        /// </summary>
        public ChartSeriesProvider SeriesProvider
        {
            get
            {
                return this.GetValue(SeriesProviderProperty) as ChartSeriesProvider;
            }
            set
            {
                this.SetValue(SeriesProviderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the chart content will be clipped to the control's bounds.
        /// </summary>
        public bool ClipToBounds
        {
            get
            {
                return this.clipToBounds;
            }
            set
            {
                if (this.clipToBounds == value)
                {
                    return;
                }

                this.clipToBounds = value;

                if (this.IsLoaded)
                {
                    this.UpdateClip();
                }
            }
        }

        /// <summary>
        /// Gets or sets the content to be displayed when the chart is either not properly initialized or missing data.
        /// </summary>
        public object EmptyContent
        {
            get
            {
                return this.GetValue(EmptyContentProperty);
            }
            set
            {
                this.SetValue(EmptyContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> that defines the visual tree of the <see cref="ContentPresenter"/> instance that visualized the <see cref="P:EmptyContent"/> property.
        /// </summary>
        public DataTemplate EmptyContentTemplate
        {
            get
            {
                return this.GetValue(EmptyContentTemplateProperty) as DataTemplate;
            }
            set
            {
                this.SetValue(EmptyContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> instance that describes the visual appearance of the plot area. The style should target the <see cref="Border"/> type.
        /// </summary>
        public Style PlotAreaStyle
        {
            get
            {
                return this.GetValue(PlotAreaStyleProperty) as Style;
            }
            set
            {
                this.SetValue(PlotAreaStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ChartPalette"/> instance that defines the appearance of the chart.
        /// </summary>
        public ChartPalette Palette
        {
            get
            {
                return this.paletteCache;
            }
            set
            {
                this.SetValue(PaletteProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="PaletteName"/> value used to apply a predefined palette to the chart.
        /// </summary>
        public PredefinedPaletteName PaletteName
        {
            get
            {
                return (PredefinedPaletteName)this.GetValue(PaletteNameProperty);
            }
            set
            {
                this.SetValue(PaletteNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ChartPalette"/> instance that defines the appearance of the chart for selected series and/or data points.
        /// </summary>
        public ChartPalette SelectionPalette
        {
            get
            {
                return this.selectionPaletteCache;
            }
            set
            {
                this.SetValue(SelectionPaletteProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="PaletteName"/> value used to apply a predefined palette to the chart.
        /// </summary>
        public PredefinedPaletteName SelectionPaletteName
        {
            get
            {
                return (PredefinedPaletteName)this.GetValue(SelectionPaletteNameProperty);
            }
            set
            {
                this.SetValue(SelectionPaletteNameProperty, value);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        LegendItemCollection ILegendInfoProvider.LegendInfos
        {
            get
            {
                return this.LegendInfosInternal;
            }
        }

        /// <summary>
        /// Gets the actual width of the chart.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        double IView.ViewportWidth
        {
            get
            {
                return this.availableSize.Width;
            }
        }

        /// <summary>
        /// Gets the actual height of the chart.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        double IView.ViewportHeight
        {
            get
            {
                return this.availableSize.Height;
            }
        }

        internal abstract LegendItemCollection LegendInfosInternal
        {
            get;
        }

        /// <summary>
        /// Gets all available series within the concrete chart instance.
        /// </summary>
        internal abstract IList SeriesInternal
        {
            get;
        }

        internal virtual RadRect PlotAreaDecorationSlot
        {
            get
            {
                return this.chartArea.plotArea.layoutSlot;
            }
        }

        /// <summary>
        /// Invalidates the current visual representation of the chart and schedules a new update that will run asynchronously.
        /// </summary>
        public void InvalidateUI()
        {
            this.chartArea.Invalidate(ChartAreaInvalidateFlags.All);
        }

        void INoDesiredSizeControl.InvalidateUI()
        {
            if (!this.arrangePassed || !this.IsTemplateApplied)
            {
                return;
            }

            this.availableSize = new Size(0, 0);
            this.lastLayoutContext = ChartLayoutContext.Invalid;

            // reset the layout context of each presenter
            foreach (UIElement child in this.renderSurface.Children)
            {
                PresenterBase presenter = child as PresenterBase;
                if (presenter != null)
                {
                    presenter.lastLayoutContext = ChartLayoutContext.Invalid;
                }
            }

            this.InvalidateArrange();

            if (this.invalidateScheduled)
            {
                this.skipInvalidated = true;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Intermediate code, will change once Localization is implemented.")]
        internal string GetLocalizedString(string key)
        {
            return ChartLocalizationManager.Instance.GetString(key);
        }

        internal Brush GetPaletteBrush(int index, PaletteVisualPart part, string seriesFamily, bool selected)
        {
            Brush paletteBrush = null;
            if (selected && this.selectionPaletteCache != null)
            {
                paletteBrush = this.selectionPaletteCache.GetBrush(index, part, seriesFamily);
            }

            if (paletteBrush == null && this.paletteCache != null)
            {
                paletteBrush = this.paletteCache.GetBrush(index, part, seriesFamily);
            }

            return paletteBrush;
        }

        internal Canvas AddLabelLayer(ChartSeries series)
        {
            if (this.labelLayer == null)
            {
                throw new InvalidOperationException("Missing labelLayer template part.");
            }

            Canvas newLabelLayer = new Canvas();
            newLabelLayer.SetBinding(UIElement.VisibilityProperty, new Windows.UI.Xaml.Data.Binding() { Path = new PropertyPath("Visibility"), Source = series });
            newLabelLayer.Tag = series;
            this.labelLayer.Children.Add(newLabelLayer);

            return newLabelLayer;
        }

        internal void RemoveLabelLayer(ChartSeries series)
        {
            for (int i = 0; i < this.labelLayer.Children.Count; i++)
            {
                FrameworkElement child = this.labelLayer.Children[i] as FrameworkElement;
                if (child.Tag == series)
                {
                    this.labelLayer.Children.RemoveAt(i);
                    break;
                }
            }
        }

        internal virtual void OnZoomChanged()
        {
            this.chartArea.OnZoomChanged();
        }

        internal virtual void OnPlotOriginChanged()
        {
            this.chartArea.OnPlotOriginChanged();
        }

        internal void ApplyPlotAreaClip(FrameworkElement visual, bool clip)
        {
            if (clip)
            {
                // clip to the actual plot area bounds
                RectangleGeometry clipGeometry = new RectangleGeometry();
                RadRect clipArea = this.PlotAreaClip;
                clipGeometry.Rect = new Rect(clipArea.X, clipArea.Y, clipArea.Width, clipArea.Height);
                visual.Clip = clipGeometry;
            }
            else
            {
                visual.Clip = null;
            }
        }

        /// <summary>
        /// Updates of all of the chart elements presented by this instance.
        /// </summary>
        internal override void UpdateUICore(ChartLayoutContext context)
        {
            base.UpdateUICore(context);

            // update the UI of each presenter
            foreach (UIElement child in this.renderSurface.Children)
            {
                PresenterBase presenter = child as PresenterBase;
                if (presenter != null)
                {
                    presenter.UpdateUI(context);
                }
            }

            // position the plot area decoraction
            RadRect plotAreaSlot = this.PlotAreaDecorationSlot;
            Canvas.SetLeft(this.plotAreaBackground, plotAreaSlot.X);
            Canvas.SetTop(this.plotAreaBackground, plotAreaSlot.Y);
            this.plotAreaBackground.Width = plotAreaSlot.Width;
            this.plotAreaBackground.Height = plotAreaSlot.Height;

            this.adornerLayer.IsHitTestVisible = !context.IsEmpty;
            this.labelLayer.Opacity = context.IsEmpty ? 0 : 1;

            this.StackedSeriesContext.Clear();
        }

        internal override void InvalidateCore()
        {
            base.InvalidateCore();

            if (this.invalidateScheduled || !this.arrangePassed || this.IsUnloaded)
            {
                return;
            }

            if (DesignMode.DesignModeEnabled)
            {
                this.availableSize = new Size(0, 0);
                this.InvalidateArrange();
            }
            else
            {
                CompositionTarget.Rendering -= this.CompositionTarget_Rendering;
                CompositionTarget.Rendering += this.CompositionTarget_Rendering;
                this.invalidateScheduled = this.InvokeAsync(this.OnInvalidated);
            }
        }

        internal override void OnPaletteInvalidated()
        {
            base.OnPaletteInvalidated();

            this.ProcessPaletteChanged(false);
        }

        internal virtual void OnPresenterAdded(ChartElementPresenter presenter)
        {
            if (this.renderSurface == null)
            {
                this.unattachedPresenters.Add(presenter);
                return;
            }

            this.renderSurface.Children.Add(presenter);
            presenter.Attach(this);

            this.InvalidateOnPresenterChanged();
        }

        internal virtual void OnPresenterRemoved(ChartElementPresenter presenter)
        {
            if (this.renderSurface == null)
            {
                this.unattachedPresenters.Remove(presenter);
                return;
            }

            this.renderSurface.Children.Remove(presenter);
            presenter.Detach();

            this.InvalidateOnPresenterChanged();
        }

        internal virtual void ProcessPaletteChanged(bool force)
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            // update the UI of each presenter
            foreach (UIElement child in this.renderSurface.Children)
            {
                PresenterBase presenter = child as PresenterBase;
                if (presenter != null)
                {
                    presenter.UpdatePalette(force);
                }
            }
        }

        internal string GenerateEmptyContent()
        {
            StringBuilder builder = new StringBuilder();

            // check whether the chart area is successfully loaded
            foreach (string notLoadedReason in this.chartArea.GetNotLoadedReasons())
            {
                builder.Append(this.GetLocalizedString(notLoadedReason));
                builder.Append(Environment.NewLine);
            }

            if (builder.Length == 0)
            {
                // the chart area is loaded, check for series count and data point count
                if (this.chartArea.plotArea.series.Count == 0)
                {
                    builder.Append(this.GetLocalizedString(NoSeriesKey));
                    builder.Append(Environment.NewLine);
                }
                else if (this.chartArea.plotArea.dataPointCount == 0)
                {
                    builder.Append(this.GetLocalizedString(NoDataKey));
                }
            }

            return builder.ToString();
        }

        internal void OnSeriesProviderStateChanged()
        {
            this.UpdateDynamicSeries();
        }

        /// <summary>
        /// This method supports the testing infrastructure and is not intended to be used directly from code (besides the call in the OnInvalidated() method).
        /// </summary>
        internal void InvalidateLayout()
        {
            if (!this.invalidateScheduled && !RadControl.IsInTestMode)
            {
                Debug.Assert(false, "OnInvalidated received without the flag being raised.");
                return;
            }

            if (this.skipInvalidated)
            {
                // this flag will be set to true when a new presenter is added and the chart is already loaded
                // see OnPresenterAdded for more information
                this.skipInvalidated = false;
                this.invalidateScheduled = false;
                return;
            }

            if (!this.IsLoaded || !this.IsTemplateApplied)
            {
                this.invalidateScheduled = false;
                return;
            }

            if (!this.arrangePassed)
            {
                this.InvalidateMeasure();
            }
            else
            {
                this.CallUpdateUI();
            }

            this.invalidateScheduled = false;
        }

        /// <summary>
        /// Creates the model of the plot area.
        /// </summary>
        internal abstract ChartAreaModel CreateChartAreaModel();

        /// <summary>
        /// Initializes the required template parts.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.adornerLayer = this.GetTemplatePartField<Canvas>(AdornerLayerPartName);
            applied = applied && this.adornerLayer != null;

            this.labelLayer = this.GetTemplatePartField<Canvas>(LabelLayerPartName);
            applied = applied && this.labelLayer != null;

            this.layoutRoot = this.GetTemplatePartField<Border>(LayoutRootPartName);
            applied = applied && this.labelLayer != null;

            this.plotAreaBackground = this.GetTemplatePartField<Border>(PlotAreaDecorationPartName);
            applied = applied && this.plotAreaBackground != null;

            this.emptyContentPresenter = this.GetTemplatePartField<ContentPresenter>(EmptyContentPresenterPartName);
            applied = applied && this.emptyContentPresenter != null;

            if (applied)
            {
                foreach (ChartElementPresenter presenter in this.unattachedPresenters)
                {
                    this.renderSurface.Children.Add(presenter);
                    presenter.Attach(this);
                }
                this.unattachedPresenters.Clear();
            }

            return applied;
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            foreach (UIElement child in this.renderSurface.Children)
            {
                ChartElementPresenter presenter = child as ChartElementPresenter;
                if (presenter != null)
                {
                    presenter.Detach();
                }
                this.unattachedPresenters.Add(presenter);
            }

            this.renderSurface.Children.Clear();

            this.OnUnloadedPartial();
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            if (this.SeriesProvider != null)
            {
                this.UpdateDynamicSeries();
            }

            // Ensure behaviors are loaded after the template has been applied.
            this.OnLoadedPartial();

            CompositionTarget.Rendering -= this.CompositionTarget_Rendering;
            CompositionTarget.Rendering += this.CompositionTarget_Rendering;
        }

        /// <summary>
        /// Loads any assigned behaviors.
        /// </summary>
        protected override void LoadCore()
        {
            base.LoadCore();

            this.OnLoadedPartial();

            this.InvalidateCore();
        }

        /// <summary>
        /// Removes behavior subscriptions.
        /// </summary>
        protected override void UnloadCore()
        {
            base.UnloadCore();

            this.availableSize = new Size(0, 0);
            this.OnUnloadedPartial();
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);

            Size oldSize = this.availableSize;
            this.availableSize = this.NormalizeAvailableSize(finalSize);

            if (oldSize == this.availableSize)
            {
                // TODO: Check carefully whether this is true
                return finalSize;
            }

            this.CallUpdateUI();

            this.arrangePassed = true;

            this.UpdatePanZoomOnArrange();

            return finalSize;
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadChartBaseAutomationPeer(this);
        }

        /// <summary>
        /// Prepares the plot area model so that it may be visualized.
        /// </summary>
        protected virtual void UpdateChartArea()
        {
            // prepare plot area
            if (!this.chartArea.IsTreeLoaded)
            {
                this.chartArea.LoadElementTree(this);
            }
            this.chartArea.Arrange();
        }

        private static void OnPaletteChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            RadChartBase chart = target as RadChartBase;
            chart.paletteCache = args.NewValue as ChartPalette;

            if (chart.paletteCache == null)
            {
                UpdatePalette(chart, chart.PaletteName);
            }

            chart.ProcessPaletteChanged(true);
        }

        private static void OnPredefinedPaletteNameChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            RadChartBase chart = target as RadChartBase;
            RadChartBase.UpdatePalette(target as RadChartBase, (PredefinedPaletteName)args.NewValue);
        }

        private static void UpdatePalette(RadChartBase chart, PredefinedPaletteName paletteName)
        {
            if (chart.GetValue(RadChartBase.PaletteProperty) == null)
            {
                chart.paletteCache = ChartPalettes.FromPredefinedName(paletteName);
                chart.ProcessPaletteChanged(true);
            }
        }

        private static void OnPredefinedSelectionPaletteNameChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            RadChartBase chart = target as RadChartBase;
            RadChartBase.UpdateSelectionPalette(target as RadChartBase, (PredefinedPaletteName)args.NewValue);
        }

        private static void UpdateSelectionPalette(RadChartBase chart, PredefinedPaletteName paletteName)
        {
            if (chart.GetValue(RadChartBase.SelectionPaletteProperty) == null)
            {
                chart.selectionPaletteCache = ChartPalettes.FromPredefinedName(paletteName);
                chart.ProcessPaletteChanged(true);
            }
        }

        private static void OnSelectionPaletteChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            RadChartBase chart = target as RadChartBase;
            chart.selectionPaletteCache = args.NewValue as ChartPalette;
            chart.ProcessPaletteChanged(true);
        }

        private static void OnEmptyContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadChartBase chart = d as RadChartBase;
            if (chart.IsInternalPropertyChange)
            {
                return;
            }

            chart.userEmptyContent = e.NewValue != null;
        }

        private static void OnSeriesProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadChartBase chart = d as RadChartBase;

            ChartSeriesProvider oldProvider = e.OldValue as ChartSeriesProvider;
            if (oldProvider != null)
            {
                oldProvider.RemoveListener(chart);
            }

            ChartSeriesProvider newProvider = e.NewValue as ChartSeriesProvider;
            if (newProvider != null)
            {
                newProvider.AddListener(chart);
            }

            chart.UpdateDynamicSeries();
        }

        private void UpdateDynamicSeries()
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            // clean-up existing dynamic series
            for (int i = this.SeriesInternal.Count - 1; i >= 0; i--)
            {
                ChartSeries series = this.SeriesInternal[i] as ChartSeries;
                if (ChartSeriesProvider.GetIsDynamicSeries(series))
                {
                    this.SeriesInternal.RemoveAt(i);
                }
            }

            var seriesProvider = this.SeriesProvider;
            if (seriesProvider == null)
            {
                return;
            }

            // add all the series provided by the current series provider
            foreach (var series in seriesProvider.CreateSeries())
            {
                this.SeriesInternal.Add(series);
            }
        }

        private bool UpdateEmptyContent()
        {
            bool empty = !this.chartArea.IsTreeLoaded || this.chartArea.plotArea.dataPointCount == 0 || this.chartArea.plotArea.series.Count == 0;
            if (!empty)
            {
                this.emptyContentPresenter.Visibility = Visibility.Collapsed;
                return false;
            }

            this.emptyContentPresenter.Visibility = Visibility.Visible;
            if (!this.userEmptyContent)
            {
                object currentContent = this.EmptyContent;
                object newContent = this.GenerateEmptyContent();
                if (!object.Equals(currentContent, newContent))
                {
                    this.ChangePropertyInternally(EmptyContentProperty, newContent);
                }
            }

            return true;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateClip();

            this.previousPlotAreaClip = this.PlotAreaClip;
        }

        private void UpdateClip()
        {
            if (this.clipToBounds)
            {
                // manually apply clipping as we are using canvas which does not clip automatically
                RectangleGeometry clip = new RectangleGeometry();
                clip.Rect = new Rect(0, 0, (int)(this.ActualWidth + .5), (int)(this.ActualHeight + .5));
                this.Clip = clip;
            }
            else
            {
                this.Clip = null;
            }
        }

        private Size NormalizeAvailableSize(Size currentAvailableSize)
        {
            double width = currentAvailableSize.Width;
            if (double.IsInfinity(width))
            {
                width = this.MinWidth;
            }

            double height = currentAvailableSize.Height;
            if (double.IsInfinity(height))
            {
                height = this.MinHeight;
            }

            width = Math.Max(this.MinWidth, width);
            height = Math.Max(this.MinHeight, height);

            return new Size(width, height);
        }

        private void OnInvalidated()
        {
            if (!this.rendered)
            {
                return;
            }

            this.rendered = false;
            this.InvalidateLayout();
        }

        private void CallUpdateUI()
        {
            if (!this.IsTemplateApplied)
            {
                // the template may not be applied if the control is edited in Blend
                return;
            }

            this.UpdateChartArea();
            ChartLayoutContext context = new ChartLayoutContext(this.availableSize, this.zoomCache, this.PlotOrigin, this.PlotAreaClip) { IsEmpty = this.UpdateEmptyContent() };
            this.UpdateUI(context);
        }

        private void InvalidateOnPresenterChanged()
        {
            if (!this.arrangePassed)
            {
                return;
            }

            if (!this.invalidateScheduled)
            {
                this.InvalidateCore();
            }
            else
            {
                // A special case where the underlying charting engine has already invalidated the view while the Measure pass for the newly added control is still pending
                // The result is that we will have UpdateUI pass without having the template for the newly inserted presenter(s) applied
                // We have two options here:
                //  1. Call UpdateLayout synchronously to ensure template is applied BEFORE the OnInvalidated is received. This will bring additional overhead
                //  2. Reset (skip) the next invalidate call to come - this is the currently implemented one
                this.skipInvalidated = true;

                // reset the available size so that the Arrange pass is not skipped
                this.availableSize = new Size(0, 0);

                this.InvalidateArrange();
            }
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            this.rendered = true;
            CompositionTarget.Rendering -= this.CompositionTarget_Rendering;
            this.invalidateScheduled = this.InvokeAsync(this.OnInvalidated);
        }
        
        partial void OnUnloadedPartial();
        partial void OnLoadedPartial();
        partial void InitManipulation();
        partial void UpdatePanZoomOnArrange();
    }
}
