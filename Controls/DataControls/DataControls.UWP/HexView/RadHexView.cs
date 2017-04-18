using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Telerik.Data.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Data.HexView;
using Telerik.UI.Xaml.Controls.Primitives;
using Telerik.UI.Xaml.Controls.Primitives.HubTile;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// A data control that arranges items in a honeycomb pattern.
    /// </summary>
    public class RadHexView : RadControl, IHexViewListener
    {
        /// <summary>
        /// Identifies the <see cref="ImageSourcePath"/> property.
        /// </summary>
        public static readonly DependencyProperty ImageSourcePathProperty =
            DependencyProperty.Register(nameof(ImageSourcePath), typeof(string), typeof(RadHexView), new PropertyMetadata(string.Empty, OnImageSourcePathChanged));

        /// <summary>
        /// Identifies the <see cref="TitlePath"/> property.
        /// </summary>
        public static readonly DependencyProperty TitlePathProperty =
            DependencyProperty.Register(nameof(TitlePath), typeof(string), typeof(RadHexView), new PropertyMetadata(string.Empty, OnTitlePathChanged));

        /// <summary>
        /// Identifies the <see cref="BackContentPath"/> property.
        /// </summary>
        public static readonly DependencyProperty BackContentPathProperty =
            DependencyProperty.Register(nameof(BackContentPath), typeof(string), typeof(RadHexView), new PropertyMetadata(string.Empty, OnBackContentPathChanged));

        /// <summary>
        /// Identifies the <see cref="LayoutDefinition"/> property.
        /// </summary>
        public static readonly DependencyProperty LayoutDefinitionProperty =
            DependencyProperty.Register(nameof(LayoutDefinition), typeof(HexLayoutDefinitionBase), typeof(RadHexView), new PropertyMetadata(null, OnLayoutDefinitionChanged));

        /// <summary>
        /// Identifies the <see cref="ItemStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemStyleProperty =
            DependencyProperty.Register(nameof(ItemStyle), typeof(Style), typeof(RadHexView), new PropertyMetadata(null, OnItemStyleChanged));

        /// <summary>
        /// Identifies the <see cref="ItemStyleSelector"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemStyleSelectorProperty =
            DependencyProperty.Register(nameof(ItemStyleSelector), typeof(StyleSelector), typeof(RadHexView), new PropertyMetadata(null, OnItemStyleSelectorChanged));

        /// <summary>
        /// Identifies the <see cref="ItemsSource"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(RadHexView), new PropertyMetadata(null, OnItemsSourceChanged));

        /// <summary>
        /// Identifies the <see cref="MinUpdateInterval"/> property.
        /// </summary>
        public static readonly DependencyProperty MinUpdateIntervalProperty =
            DependencyProperty.Register(nameof(MinUpdateInterval), typeof(TimeSpan), typeof(RadHexView), new PropertyMetadata(TimeSpan.Zero, OnMinUpdateIntervalChanged));

        /// <summary>
        /// Identifies the <see cref="MaxUpdateInterval"/> property.
        /// </summary>
        public static readonly DependencyProperty MaxUpdateIntervalProperty =
            DependencyProperty.Register(nameof(MaxUpdateInterval), typeof(TimeSpan), typeof(RadHexView), new PropertyMetadata(TimeSpan.Zero, OnMaxUpdateIntervalChanged));

        /// <summary>
        /// Identifies the <see cref="UpdateIntervalStep"/> property.
        /// </summary>
        public static readonly DependencyProperty UpdateIntervalStepProperty =
            DependencyProperty.Register(nameof(UpdateIntervalStep), typeof(double), typeof(RadHexView), new PropertyMetadata(0d, OnUpdateIntervalStepChanged));

        private static Random random = new Random();

        private readonly IValueConverter imageConverter;

        private ScrollViewer scrollViewer;
        private HexPanel childrenPanel;
        private HexItemModelGenerator itemGenerator;
        private HexViewItemUIContainerGenerator containerGenerator;
        private HexLayoutStrategyBase strategy;
        private IDataSourceView sourceView;
        private bool renderingHooked;
        private Point lastScrollOffset;
        private Size panelAvailableSize;
        private int minUpdateInterval;
        private int maxUpdateInterval;
        private int updateIntervalStep;
        private StyleSelector itemStyleSelectorChache;
        private Style itemStyleChache;
        private bool contentMeasureRequested;
        private HexLayoutDefinitionBase layoutDefinitionCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadHexView"/> class.
        /// </summary>
        public RadHexView()
        {
            this.DefaultStyleKey = typeof(RadHexView);
            this.containerGenerator = new HexViewItemUIContainerGenerator(this);
            this.itemGenerator = new HexItemModelGenerator(this.containerGenerator);
            this.imageConverter = new ImageConverter(this);
        }

        /// <summary>
        /// Occurs when an item within the control has been tapped.
        /// </summary>
        public event EventHandler<HexViewItemTapEventArgs> ItemTap;

        /// <summary>
        /// Gets or sets the style of the items. The <see cref="Style"/> should target the <see cref="RadHexHubTile"/> type.
        /// </summary>
        public Style ItemStyle
        {
            get
            {
                return this.itemStyleChache;
            }
            set
            {
                this.SetValue(ItemStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a selector that is used when applying the style to an item.
        /// </summary>
        public StyleSelector ItemStyleSelector
        {
            get
            {
                return this.itemStyleSelectorChache;
            }
            set
            {
                this.SetValue(ItemStyleSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the layout definition that specifies how the items will be arranged.
        /// </summary>
        public HexLayoutDefinitionBase LayoutDefinition
        {
            get
            {
                return this.layoutDefinitionCache;
            }
            set
            {
                this.SetValue(LayoutDefinitionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        public object ItemsSource
        {
            get
            {
                return (object)this.GetValue(ItemsSourceProperty);
            }
            set
            {
                this.SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the property name of the data model that will be used
        /// to set the <see cref="RadHexHubTile.ImageSource"/> property of the items.
        /// </summary>
        public string ImageSourcePath
        {
            get
            {
                return (string)this.GetValue(ImageSourcePathProperty);
            }
            set
            {
                this.SetValue(ImageSourcePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the property name of the data model that will be used
        /// to set the <see cref="HubTileBase.Title"/> property of the items.
        /// </summary>
        public string TitlePath
        {
            get
            {
                return (string)this.GetValue(TitlePathProperty);
            }
            set
            {
                this.SetValue(TitlePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the property name of the data model that will be used
        /// to set the <see cref="HubTileBase.BackContent"/> property of the items.
        /// </summary>
        public string BackContentPath
        {
            get
            {
                return (string)this.GetValue(BackContentPathProperty);
            }
            set
            {
                this.SetValue(BackContentPathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum update interval of the tiles in the <see cref="RadHexView"/>.
        /// The update interval of the items is set to a random value between the <see cref="MinUpdateInterval"/>
        /// and <see cref="MaxUpdateInterval"/>, considering the <see cref="UpdateIntervalStep"/>.
        /// </summary>
        public TimeSpan MinUpdateInterval
        {
            get
            {
                return (TimeSpan)this.GetValue(MinUpdateIntervalProperty);
            }
            set
            {
                this.SetValue(MinUpdateIntervalProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum update interval of the tiles in the <see cref="RadHexView"/>.
        /// The update interval of the items is set to a random value between the <see cref="MinUpdateInterval"/>
        /// and <see cref="MaxUpdateInterval"/>, considering the <see cref="UpdateIntervalStep"/>.
        /// </summary>
        public TimeSpan MaxUpdateInterval
        {
            get
            {
                return (TimeSpan)this.GetValue(MaxUpdateIntervalProperty);
            }
            set
            {
                this.SetValue(MaxUpdateIntervalProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the step of the update interval values applied to the tiles in the <see cref="RadHexView"/>.
        /// The update interval of the items is set to a random value between the <see cref="MinUpdateInterval"/>
        /// and <see cref="MaxUpdateInterval"/>, considering the <see cref="UpdateIntervalStep"/>.
        /// </summary>
        public double UpdateIntervalStep
        {
            get
            {
                return (double)this.GetValue(UpdateIntervalStepProperty);
            }
            set
            {
                this.SetValue(UpdateIntervalStepProperty, value);
            }
        }

        /// <summary>
        /// Gets the current scroll offset.
        /// </summary>
        public double ScrollOffset
        {
            get
            {
                return this.strategy.IsVertical ? this.scrollViewer.VerticalOffset : this.scrollViewer.HorizontalOffset;
            }
        }

        internal IDataSourceView SourceView
        {
            get
            {
                return this.sourceView;
            }
        }

        internal Panel ChildrenPanel
        {
            get
            {
                return this.childrenPanel;
            }
        }

        void IHexViewListener.RaiseItemTap(RadHexHubTile item)
        {
            var handler = this.ItemTap;
            if (handler != null)
            {
                handler(this, new HexViewItemTapEventArgs(item));
            }
        }

        internal void UpdateBindings(RadHexHubTile item)
        {
            UpdateBinding(item, RadHexHubTile.ImageSourceProperty, this.ImageSourcePath, this.imageConverter);
            UpdateBinding(item, RadHexHubTile.TitleProperty, this.TitlePath);
            UpdateBinding(item, RadHexHubTile.BackContentProperty, this.BackContentPath);
        }

        internal void OnChildrenPanelArrange(Size finalSize)
        {
            this.strategy.ArrangeContent();
        }

        internal Size OnChildrenPanelMeasure(Size availableSize)
        {
            return this.strategy.MeasureContent(this.panelAvailableSize, this.ScrollOffset);
        }

        internal TimeSpan GetRandomUpdateInterval()
        {
            return TimeSpan.FromMilliseconds(this.minUpdateInterval + random.Next((this.maxUpdateInterval - this.minUpdateInterval) / this.updateIntervalStep) * this.updateIntervalStep);
        }

        /// <inheritdoc/>
        protected override bool ApplyTemplateCore()
        {
            var isApplied = base.ApplyTemplateCore();

            this.scrollViewer = this.GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
            this.childrenPanel = this.GetTemplateChild("PART_ChildrenPanel") as HexPanel;

            return isApplied && (this.scrollViewer != null) && (this.childrenPanel != null);
        }

        /// <inheritdoc/>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();
            this.childrenPanel.Owner = this;
            this.scrollViewer.ViewChanged += this.ScrollViewer_ViewChanged;
            this.scrollViewer.SizeChanged += this.ScrollViewer_SizeChanged;
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            this.childrenPanel.Owner = null;
            this.scrollViewer.ViewChanged -= this.ScrollViewer_ViewChanged;
            this.scrollViewer.SizeChanged -= this.ScrollViewer_SizeChanged;
            base.UnapplyTemplateCore();
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadHexViewAutomationPeer(this);
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            this.panelAvailableSize = new Size(Math.Round(availableSize.Width), Math.Round(availableSize.Height));
            return base.MeasureOverride(this.panelAvailableSize);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as RadHexView;
            view.UpdateSource(e.NewValue);
            if (view.strategy != null)
            {
                view.strategy.RecycleAllItems();
            }
            view.UpdateUI(true);
        }

        private static void OnLayoutDefinitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as RadHexView;

            if (view.layoutDefinitionCache != null)
            {
                view.layoutDefinitionCache.PropertyChanged -= view.LayoutDefinitionPropertyChanged;
            }

            view.layoutDefinitionCache = e.NewValue as HexLayoutDefinitionBase;

            if (view.layoutDefinitionCache == null)
            {
                throw new Exception();
            }

            view.layoutDefinitionCache.PropertyChanged += view.LayoutDefinitionPropertyChanged;

            view.UpdateStrategy();
        }

        private static void OnMinUpdateIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as RadHexView;
            var newInterval = (int)((TimeSpan)e.NewValue).TotalMilliseconds;
            if (view.minUpdateInterval != newInterval)
            {
                view.minUpdateInterval = newInterval;
                view.UpdateUI(false);
            }
        }

        private static void OnMaxUpdateIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as RadHexView;
            var newInterval = (int)((TimeSpan)e.NewValue).TotalMilliseconds;
            if (view.maxUpdateInterval != newInterval)
            {
                view.maxUpdateInterval = newInterval;
                view.UpdateUI(false);
            }
        }

        private static void OnUpdateIntervalStepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as RadHexView;
            view.updateIntervalStep = (int)((double)e.NewValue) * 1000;
        }

        private static void OnItemStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as RadHexView;
            var newStyle = (Style)e.NewValue;
            if (view.itemStyleChache != newStyle)
            {
                view.itemStyleChache = newStyle;
                view.UpdateUI(true);
            }
        }

        private static void OnItemStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as RadHexView;
            var newStyleSelector = (StyleSelector)e.NewValue;
            if (view.itemStyleSelectorChache != newStyleSelector)
            {
                view.itemStyleSelectorChache = newStyleSelector;
                view.UpdateUI(true);
            }
        }

        private static void OnImageSourcePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as RadHexView;
            view.UpdateUI(false);
        }

        private static void OnTitlePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as RadHexView;
            view.UpdateUI(false);
        }

        private static void OnBackContentPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as RadHexView;
            view.UpdateUI(false);
        }

        private static void UpdateBinding(RadHexHubTile item, DependencyProperty property, string path, IValueConverter converter = null)
        {
            var bindingExpression = item.GetBindingExpression(property);

            if (!string.IsNullOrEmpty(path))
            {
                if (bindingExpression == null || bindingExpression.ParentBinding.Path.Path != path)
                {
                    item.SetBinding(property, new Binding { Path = new PropertyPath(path), Converter = converter });
                }
            }
            else if (bindingExpression != null)
            {
                item.ClearValue(property);
            }
        }

        private void LayoutDefinitionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.UpdateStrategy();
        }

        private void UpdateStrategy()
        {
            if (this.strategy != null)
            {
                this.strategy.RecycleAllItems();
            }

            this.strategy = this.layoutDefinitionCache.CreateStrategy(this.itemGenerator, this);
            this.UpdateUI(true);
        }

        private void UpdateUI(bool resetScrollOffset)
        {
            if (this.IsTemplateApplied)
            {
                if (this.ScrollOffset == 0 || !resetScrollOffset)
                {
                    this.childrenPanel.measureRequested = true;
                    this.childrenPanel.InvalidateMeasure();
                }
                else
                {
                    this.ScrollToTop();
                    this.contentMeasureRequested = true;
                }
            }
        }

        private void UpdateSource(object newSource)
        {
            if (this.sourceView != null)
            {
                this.sourceView.CollectionChanging -= this.SourceCollectionChanging;
                this.sourceView.CollectionChanged -= this.SourceCollectionChanged;
            }

            this.sourceView = DataSourceViewFacotry.CreateDataSourceView(newSource ?? Enumerable.Empty<object>());
            this.sourceView.CollectionChanging += this.SourceCollectionChanging;
            this.sourceView.CollectionChanged += this.SourceCollectionChanged;
        }

        private void SourceCollectionChanging(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.sourceView.ProcessPendingCollectionChange();
        }

        private void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.childrenPanel.measureRequested = true;
            this.childrenPanel.InvalidateMeasure();
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (e.IsIntermediate)
            {
                this.HookRendering();
            }
            else
            {
                this.UnhookRendering();
                this.UpdateScrollOffsetOnRendering();
            }
        }

        private void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateUI(false);
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            this.UpdateScrollOffsetOnRendering();
        }

        private void HookRendering()
        {
            if (this.renderingHooked)
            {
                return;
            }

            CompositionTarget.Rendering += this.CompositionTarget_Rendering;
            this.renderingHooked = true;
        }

        private void UnhookRendering()
        {
            if (!this.renderingHooked)
            {
                return;
            }

            CompositionTarget.Rendering -= this.CompositionTarget_Rendering;
            this.renderingHooked = false;
        }

        private void UpdateScrollOffsetOnRendering()
        {
            this.SetHorizontalOffset(this.scrollViewer.HorizontalOffset, this.contentMeasureRequested, false);
            this.SetVerticalOffset(this.scrollViewer.VerticalOffset, this.contentMeasureRequested, false);
        }

        private void SetVerticalOffset(double physicalOffset, bool updateUI, bool updateScrollViewer)
        {
            var adjustedOffset = ListViewModel.DoubleArithmetics.Ceiling(physicalOffset);
            bool needUpdate = !LayoutDoubleUtil.AreClose(this.lastScrollOffset.Y, physicalOffset) || physicalOffset < 0;
            var previousOffset = this.lastScrollOffset.Y;
            this.lastScrollOffset.Y = adjustedOffset;

            if (needUpdate)
            {
                if (this.childrenPanel != null && (updateUI || this.strategy.RequiresUpdate(this.panelAvailableSize, previousOffset, adjustedOffset)))
                {
                    this.contentMeasureRequested = false;
                    this.childrenPanel.measureRequested = true;
                    this.childrenPanel.InvalidateMeasure();
                }

                if (updateScrollViewer)
                {
                    this.scrollViewer.ChangeView(null, adjustedOffset, null, true);

                    // Ensure that scrollviewer has updated its position.
                    if (updateUI)
                    {
                        this.scrollViewer.UpdateLayout();
                    }
                }
            }
        }

        private void SetHorizontalOffset(double physicalOffset, bool updateUI, bool updateScrollViewer)
        {
            var adjustedOffset = ListViewModel.DoubleArithmetics.Ceiling(physicalOffset);
            bool needUpdate = !LayoutDoubleUtil.AreClose(this.lastScrollOffset.X, physicalOffset) || physicalOffset < 0;
            var previousOffset = this.lastScrollOffset.X;
            this.lastScrollOffset.X = adjustedOffset;

            if (needUpdate)
            {
                if (this.childrenPanel != null && (updateUI || this.strategy.RequiresUpdate(this.panelAvailableSize, previousOffset, adjustedOffset)))
                {
                    this.contentMeasureRequested = false;
                    this.childrenPanel.measureRequested = true;
                    this.childrenPanel.InvalidateMeasure();
                }

                if (updateScrollViewer)
                {
                    this.scrollViewer.ChangeView(adjustedOffset, null, null, true);

                    // Ensure that scrollviewer has updated its position.
                    if (updateUI)
                    {
                        this.scrollViewer.UpdateLayout();
                    }
                }
            }
        }

        private void ScrollToTop()
        {
            if (this.scrollViewer != null)
            {
                if (this.strategy.IsVertical)
                {
                    this.scrollViewer.ChangeView(null, 0, null, true);
                }
                else
                {
                    this.scrollViewer.ChangeView(0, null, null, true);
                }
            }
        }
    }
}
