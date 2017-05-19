using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Telerik.Core;
using Telerik.Core.Data;
using Telerik.Data.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Data.ListView;
using Telerik.UI.Xaml.Controls.Data.ListView.Commands;
using Telerik.UI.Xaml.Controls.Data.ListView.Model;
using Telerik.UI.Xaml.Controls.Data.ListView.Model.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Data.ListView.Primitives;
using Telerik.UI.Xaml.Controls.Data.ListView.View.Controls;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a custom <see cref="Control"/> implementation that may be used to visualize data in a list with different layout strategies.
    /// </summary>
    public partial class RadListView : RadControl, IListView
    {
        /// <summary>
        /// Identifies the <see cref="RealizedItemsBufferScale"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty RealizedItemsBufferScaleProperty =
            DependencyProperty.Register(nameof(RealizedItemsBufferScale), typeof(double), typeof(RadListView), new PropertyMetadata(1, OnRealizedItemsBufferScaleChanged));

        /// <summary>
        /// Identifies the <see cref="IncrementalLoadingMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IncrementalLoadingModeProperty =
            DependencyProperty.Register(nameof(IncrementalLoadingMode), typeof(BatchLoadingMode), typeof(RadListView), new PropertyMetadata(BatchLoadingMode.Auto, OnIncrementalLoadingModeChanged));

        /// <summary>
        /// Identifies the <see cref="IsBusyIndicatorEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsBusyIndicatorEnabledProperty =
            DependencyProperty.Register(nameof(IsBusyIndicatorEnabled), typeof(bool), typeof(RadListView), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(RadListView), new PropertyMetadata(Orientation.Vertical, OnOrientationChanged));

        /// <summary>
        /// Identifies the <see cref="GroupHeaderTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupHeaderTemplateProperty =
            DependencyProperty.Register(nameof(GroupHeaderTemplate), typeof(DataTemplate), typeof(RadListView), new PropertyMetadata(null, OnGroupHeaderTemplateChanged));

        /// <summary>
        /// Identifies the <see cref="GroupHeaderTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupHeaderTemplateSelectorProperty =
            DependencyProperty.Register(nameof(GroupHeaderTemplateSelector), typeof(DataTemplateSelector), typeof(RadListView), new PropertyMetadata(null, OnGroupHeaderTemplateSelectorChanged));

        /// <summary>
        /// Identifies the <see cref="GroupHeaderStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupHeaderStyleProperty =
            DependencyProperty.Register(nameof(GroupHeaderStyle), typeof(Style), typeof(RadListView), new PropertyMetadata(null, OnGroupHeaderStyleChanged));

        /// <summary>
        /// Identifies the <see cref="GroupHeaderStyleSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupHeaderStyleSelectorProperty =
            DependencyProperty.Register(nameof(GroupHeaderStyleSelector), typeof(StyleSelector), typeof(RadListView), new PropertyMetadata(null, OnGroupHeaderStyleSelectorChanged));

        /// <summary>
        /// Identifies the <see cref="GroupHeaderStyleSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupHeaderDisplayModeProperty =
            DependencyProperty.Register(nameof(GroupHeaderDisplayMode), typeof(ListViewGroupHeaderDisplayMode), typeof(RadListView), new PropertyMetadata(ListViewGroupHeaderDisplayMode.Frozen, OnGroupHeaderDisplayModeChanged));

        /// <summary>
        /// Identifies the <see cref="ItemStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemStyleProperty =
            DependencyProperty.Register(nameof(ItemStyle), typeof(Style), typeof(RadListView), new PropertyMetadata(null, OnItemStyleChanged));

        /// <summary>
        /// Identifies the <see cref="ItemStyleSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemStyleSelectorProperty =
            DependencyProperty.Register(nameof(ItemStyleSelector), typeof(StyleSelector), typeof(RadListView), new PropertyMetadata(null, OnItemStyleSelectorChanged));

        /// <summary>
        /// Identifies the <see cref="ItemTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register(nameof(ItemTemplateSelector), typeof(DataTemplateSelector), typeof(RadListView), new PropertyMetadata(null, OnItemTemplateSelectorChanged));

        /// <summary>
        /// Identifies the <see cref="ItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(RadListView), new PropertyMetadata(null, OnItemTemplateChanged));

        /// <summary>
        /// Identifies the <see cref="EmptyContentDisplayMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EmptyContentDisplayModeProperty =
            DependencyProperty.Register(nameof(EmptyContentDisplayMode), typeof(EmptyContentDisplayMode), typeof(RadListView), new PropertyMetadata(EmptyContentDisplayMode.Always, OnEmptyContentDisplayModeChanged));

        /// <summary>
        /// Identifies the <see cref="EmptyContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EmptyContentProperty =
            DependencyProperty.Register(nameof(EmptyContent), typeof(object), typeof(RadListView), new PropertyMetadata("No data", OnEmptyContentChanged));

        /// <summary>
        /// Identifies the <see cref="ListHeader"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ListHeaderProperty =
            DependencyProperty.Register(nameof(ListHeader), typeof(object), typeof(RadListView), new PropertyMetadata(null, OnListHeaderChanged));

        /// <summary>
        /// Identifies the <see cref="ListFooter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ListFooterProperty =
            DependencyProperty.Register(nameof(ListFooter), typeof(object), typeof(RadListView), new PropertyMetadata(null, OnListFooterChanged));

        /// <summary>
        /// Identifies the <see cref="ItemsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(RadListView), new PropertyMetadata(null, OnItemsSourceChanged));

        /// <summary>
        /// Identifies the <see cref="LayoutDefinition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LayoutDefinitionProperty =
            DependencyProperty.Register(nameof(LayoutDefinition), typeof(LayoutDefinitionBase), typeof(RadListView), new PropertyMetadata(null, OnLayoutDefinitionChanged));

        /// <summary>
        /// Identifies the <see cref="IncrementalLoadingBufferItemsCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IncrementalLoadingBufferItemsCountProperty =
            DependencyProperty.Register(nameof(IncrementalLoadingBufferItemsCount), typeof(int), typeof(RadListView), new PropertyMetadata(10, IncrementalLoadingBufferItemsCountChanged));

        internal RadListViewItem swipedItem;
        internal bool isActionContentDisplayed;
        internal CurrencyListViewLayer currencyLayerCache;
        internal CheckBoxListViewLayer checkBoxLayerCache;
        internal HeaderFooterListViewLayer headerFooterLayerCache;
        internal EmptyContentListViewLayer emptyContentLayerCahce;
        internal ScrollableAdornerListViewLayer scrollableAdornerLayerCache;
        internal OverlayAdornerListViewLayer overlayAdornerLayerCache;
        internal ListViewVisualStateService visualStateService;
        internal CommandService commandService;
        internal ContentControl swipeActionContentControl;
        internal RadSize panelAvailableSize;
        internal ListViewPanel contentPanel;
        internal Panel childrenPanel;
        internal Panel animatingChildrenPanel;
        internal UpdateServiceBase<UpdateFlags> updateService;
        internal ListViewAnimationService animationSurvice;
        internal ListViewLoadDataControl loadMoreDataControl;
        internal Panel frozenGroupHeadersHost;

        private const string StaggeredLayoutGroupingExceptionMessage = "Staggered Layout does not support grouping.";
        private PullToRefreshIndicator pullToRefreshIndicator;
        private Panel rootScrollPanel;

        private bool itemsMeasured;

        private DataTemplate groupHeaderTemplateCache;
        private DataTemplateSelector groupHeaderTemplateSelectorCache;
        private Style groupHeaderStyleCache;
        private StyleSelector groupHeaderStyleSelectorCache;
        private DataTemplate itemTemplateCache;
        private DataTemplateSelector itemTemplateSelectorCache;
        private Style itemStyleCache;
        private StyleSelector itemStyleSelectorCache;
        private bool isActionOnSwipeEnabledCache;
        private Panel scrollableAdornerHostPanel;
        private Panel adornerHostPanel;
        private ListViewRootPanel listViewRootPanel;
        private bool renderingHooked;
        private RadPoint lastScrollOffset;
        private ListViewItemUIContainerGenerator containerGenerator;
        private ScrollViewer scrollViewer;
        private ListViewModel model;
        private Size lastAvailableSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadListView"/> class.
        /// </summary>
        public RadListView()
        {
            this.DefaultStyleKey = typeof(RadListView);

            this.updateService = new ListViewUpdateService(this, RadListView.ShouldExecuteOperationsSyncroniously);
            this.visualStateService = new ListViewVisualStateService(this);
            this.commandService = new CommandService(this);
            this.selectionService = new ListViewSelectionService(this);
            this.currencyService = new ListViewCurrencyService(this);
            this.itemCheckBoxService = new ListViewItemCheckBoxService(this);
            this.animationSurvice = new ListViewAnimationService(this);
            this.DragBehavior = new ListViewDragBehavior(this);

            this.containerGenerator = new ListViewItemUIContainerGenerator(this);

            this.model = new ListViewModel(this, RadListView.ShouldExecuteOperationsSyncroniously);

            this.model.layoutController.strategy.EnableFrozenDecorators = true;
            this.InitializeReorder();
        }

        /// <summary>
        /// Gets or sets the relative to viewport size buffer scale that will be used to realize items outside viewport. Default value is 1.
        /// </summary>
        public double RealizedItemsBufferScale
        {
            get { return (double)GetValue(RealizedItemsBufferScaleProperty); }
            set { this.SetValue(RealizedItemsBufferScaleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content that will be displayed when there are no items in the list view.
        /// </summary>
        public object EmptyContent
        {
            get
            {
                return (object)this.GetValue(EmptyContentProperty);
            }
            set
            {
                this.SetValue(EmptyContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies when the <see cref="RadListView.EmptyContent"/> will be displayed.
        /// </summary>
        public EmptyContentDisplayMode EmptyContentDisplayMode
        {
            get
            {
                return (EmptyContentDisplayMode)this.GetValue(EmptyContentDisplayModeProperty);
            }
            set
            {
                this.SetValue(EmptyContentDisplayModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="DataTemplate"/> that represents the template applied to each visual item in the control.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get
            {
                return this.itemTemplateCache;
            }

            set
            {
                this.SetValue(ItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Style"/> that is applied to each visual item in the control.
        /// </summary>
        public Style ItemStyle
        {
            get
            {
                return this.itemStyleCache;
            }

            set
            {
                this.SetValue(ItemStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="StyleSelector"/> that provides logic for customizing the appearance for each visual item in the control.
        /// </summary>
        public StyleSelector ItemStyleSelector
        {
            get
            {
                return this.itemStyleSelectorCache;
            }

            set
            {
                this.SetValue(ItemStyleSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplateSelector"/> that provides logic for customizing the <see cref="DataTemplate"/> for each visual item in the control.
        /// </summary>
        public DataTemplateSelector ItemTemplateSelector
        {
            get
            {
                return this.itemTemplateSelectorCache;
            }

            set
            {
                this.SetValue(ItemTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the control.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(OrientationProperty);
            }
            set
            {
                this.SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content of the header element.
        /// </summary>
        public object ListHeader
        {
            get
            {
                return (object)this.GetValue(ListHeaderProperty);
            }
            set
            {
                this.SetValue(ListHeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content of the footer element.
        /// </summary>
        public object ListFooter
        {
            get
            {
                return (object)this.GetValue(ListFooterProperty);
            }
            set
            {
                this.SetValue(ListFooterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="DataTemplate"/> that represents the template applied to the group headers.
        /// </summary>
        public DataTemplate GroupHeaderTemplate
        {
            get
            {
                return this.groupHeaderTemplateCache;
            }

            set
            {
                this.SetValue(GroupHeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplateSelector"/> that provides logic for customizing the <see cref="DataTemplate"/> for each group header.
        /// </summary>
        public DataTemplateSelector GroupHeaderTemplateSelector
        {
            get
            {
                return this.groupHeaderTemplateSelectorCache;
            }

            set
            {
                this.SetValue(GroupHeaderTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Style"/> that is applied to the group headers.
        /// </summary>
        public Style GroupHeaderStyle
        {
            get
            {
                return this.groupHeaderStyleCache;
            }

            set
            {
                this.SetValue(GroupHeaderStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="StyleSelector"/> that provides logic for customizing the appearance for each group header.
        /// </summary>
        public StyleSelector GroupHeaderStyleSelector
        {
            get
            {
                return this.groupHeaderStyleSelectorCache;
            }

            set
            {
                this.SetValue(GroupHeaderStyleSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ListViewGroupHeaderDisplayMode"/> instance.
        /// </summary>
        public ListViewGroupHeaderDisplayMode GroupHeaderDisplayMode
        {
            get { return (ListViewGroupHeaderDisplayMode)GetValue(GroupHeaderDisplayModeProperty); }
            set { this.SetValue(GroupHeaderDisplayModeProperty, value); }
        }

        /// <summary>
        /// Gets the <see cref="CommandService"/> instance that manages the commanding behavior of this instance.
        /// </summary>
        public CommandService CommandService
        {
            get
            {
                return this.commandService;
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies the incremental loading mode.
        /// </summary>
        public BatchLoadingMode IncrementalLoadingMode
        {
            get
            {
                return (BatchLoadingMode)this.GetValue(IncrementalLoadingModeProperty);
            }
            set
            {
                this.SetValue(IncrementalLoadingModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the incremental loading buffer items count.
        /// </summary>
        /// <value>
        /// The incremental loading buffer items count.
        /// </value>
        public int IncrementalLoadingBufferItemsCount
        {
            get { return (int)GetValue(IncrementalLoadingBufferItemsCountProperty); }
            set { this.SetValue(IncrementalLoadingBufferItemsCountProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a busy indicator will appear when the data from the source is being loaded. 
        /// </summary>
        public bool IsBusyIndicatorEnabled
        {
            get
            {
                return (bool)this.GetValue(IsBusyIndicatorEnabledProperty);
            }
            set
            {
                this.SetValue(IsBusyIndicatorEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the source used to generate the content of the control. 
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
        /// Gets or sets the the layout definition that specifies how the elements will be rendered.
        /// </summary>
        public LayoutDefinitionBase LayoutDefinition
        {
            get
            {
                return (LayoutDefinitionBase)this.GetValue(LayoutDefinitionProperty);
            }
            set
            {
                this.SetValue(LayoutDefinitionProperty, value);
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="SortDescriptorBase"/> objects that defines the current sorting within this instance.
        /// Multiple sort descriptors define a sorting operation by multiple keys.
        /// </summary>
        public SortDescriptorCollection SortDescriptors
        {
            get
            {
                return this.model.SortDescriptors;
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="GroupDescriptorBase"/> objects that defines the current grouping within this instance.
        /// Multiple group descriptors define multiple group levels.
        /// </summary>
        public GroupDescriptorCollection GroupDescriptors
        {
            get
            {
                return this.model.GroupDescriptors;
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="FilterDescriptorBase"/> objects that defines the current filtering within this instance.
        /// </summary>
        public FilterDescriptorCollection FilterDescriptors
        {
            get
            {
                return this.model.FilterDescriptors;
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="AggregateDescriptorBase"/> objects that defines the current aggregate functions to be applied when the data view is computed.
        /// </summary>
        public AggregateDescriptorCollection AggregateDescriptors
        {
            get
            {
                return this.model.AggregateDescriptors;
            }
        }

        /// <summary>
        /// Gets the collection with all the custom commands registered with the <see cref="CommandService"/>. Custom commands have higher priority than the built-in (default) ones.
        /// </summary>
        public CommandCollection<RadListView> Commands
        {
            get
            {
                return this.commandService.UserCommands;
            }
        }

        /// <summary>
        /// Gets the scroll offset of the control.
        /// </summary>
        public double ScrollOffset
        {
            get
            {
                return (this.LayoutDefinition != null && this.Orientation == Orientation.Horizontal) ? this.lastScrollOffset.X : this.lastScrollOffset.Y;
            }
        }

        UpdateServiceBase<UpdateFlags> IListView.UpdateService
        {
            get
            {
                return this.updateService;
            }
        }

        IUIContainerGenerator<GeneratedItemModel, ItemGenerationContext> IListView.ContainerGenerator
        {
            get
            {
                return this.containerGenerator;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        double IView.ViewportWidth
        {
            get
            {
                return this.lastAvailableSize.Width;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        double IView.ViewportHeight
        {
            get
            {
                return this.lastAvailableSize.Height;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        bool IElementPresenter.IsVisible
        {
            get
            {
                return this.Visibility == Visibility.Visible;
            }
        }

        internal ListViewDragBehavior DragBehavior
        {
            get
            {
                return (ListViewDragBehavior)this.GetValue(DragBehaviorProperty);
            }
            set
            {
                this.SetValue(RadListView.DragBehaviorProperty, value);
            }
        }

        internal Panel AdornerHost
        {
            get
            {
                return this.adornerHostPanel;
            }
        }

        internal Panel ScrollableAdornerHost
        {
            get
            {
                return this.scrollableAdornerHostPanel;
            }
        }

        internal ScrollViewer ScrollViewer
        {
            get
            {
                return this.scrollViewer;
            }
        }

        internal ListViewModel Model
        {
            get
            {
                return this.model;
            }
        }

        private static bool ShouldExecuteOperationsSyncroniously
        {
            get
            {
                return RadControl.IsInTestMode || DesignMode.DesignModeEnabled;
            }
        }

        private bool ShouldDisplayEmptyContent
        {
            get
            {
                if (this.model.ShouldDisplayIncrementalLoadingIndicator)
                {
                    return false;
                }

                if (this.EmptyContentDisplayMode == EmptyContentDisplayMode.None)
                {
                    return false;
                }

                if (this.ItemsSource == null && (this.EmptyContentDisplayMode & EmptyContentDisplayMode.DataSourceNull) == EmptyContentDisplayMode.DataSourceNull)
                {
                    return true;
                }

                if (this.ItemsSource != null && (this.EmptyContentDisplayMode & EmptyContentDisplayMode.DataSourceEmpty) == EmptyContentDisplayMode.DataSourceEmpty)
                {
                    var source = this.ItemsSource as IEnumerable;

                    if (source != null)
                    {
                        foreach (var item in source)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Attempts to bring the specified data item into view asynchronously.
        /// </summary>
        /// <param name="item">The data item to scroll to.</param>
        public void ScrollItemIntoView(object item)
        {
            this.ScrollItemIntoView(item, null);
        }

        /// <summary>
        /// Attempts to bring the specified data item into view asynchronously.
        /// </summary>
        /// <param name="item">The data item to scroll to.</param>
        /// <param name="scrollCompletedAction">Arbitrary action that may be executed after the asynchronous update is executed.</param>
        public void ScrollItemIntoView(object item, Action scrollCompletedAction)
        {
            if (!this.IsTemplateApplied || !this.itemsMeasured)
            {
                this.updateService.RegisterUpdate(new DelegateUpdate<UpdateFlags>(() => this.ScrollItemIntoView(item, scrollCompletedAction)));
                return;
            }

            var action = (Action)(() =>
            {
                var info = this.model.FindItemInfo(item);

                if (info != null)
                {
                    var scrollOperation = new ScrollIntoViewOperation<ItemInfo?>(info, this.ScrollOffset) { CompletedAction = scrollCompletedAction };
                    this.Model.ScrollIndexIntoViewCore(scrollOperation);
                }
            });

            this.updateService.RegisterUpdate(new DelegateUpdate<UpdateFlags>(action));
        }
        
        /// <summary>
        /// Invalidates the measure of the <see cref="RadListView"/> content panel.
        /// </summary>
        public void RebuildUI()
        {
            this.ResetActionContent();
            this.contentPanel.InvalidateMeasure();
        }

        /// <inheritdoc/>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void ChangeDataLoadingStatus(BatchLoadingStatus status)
        {
            if (this.model.CurrentDataProvider.DataView.BatchDataProvider != null)
            {
                this.model.CurrentDataProvider.DataView.BatchDataProvider.OnStatusChanged(status);
            }
        }

        void IListView.OnDataStatusChanged(Telerik.Data.Core.DataProviderStatus status)
        {
            this.visualStateService.UpdateDataLoadingStatus(status);
        }

        RadSize IOrientedParentView.Measure(IGeneratedContainer container, RadSize availableSize)
        {
            if (container is HeaderGeneratedModel)
            {
                return this.headerFooterLayerCache.MeasureHeader(availableSize);
            }

            if (container is FooterGeneratedModel)
            {
                return this.headerFooterLayerCache.MeasureFooter(availableSize);
            }

            if (container is EmptyContentGeneratedModel)
            {
                return this.emptyContentLayerCahce.MeasureEmptyContent(availableSize, this.ShouldDisplayEmptyContent);
            }

            UIElement element = container.Container as UIElement;

            if (element != null && element is ListViewGroupHeader)
            {
                element.Measure(availableSize.ToSize());
            }
            else
            {
                var itemVisual = element as RadListViewItem;
                if (itemVisual != null && !this.IsCheckModeActive)
                {
                    itemVisual.IsSelected = this.selectionService.IsSelected(container.ItemInfo.Item);
                    itemVisual.IsHandleEnabled = this.IsItemReorderEnabled && this.ReorderMode == ListViewReorderMode.Handle;
                }

                element.Measure(availableSize.ToSize());
            }

            return element.DesiredSize.ToRadSize();
        }

        void IOrientedParentView.Arrange(IGeneratedContainer container)
        {
            if (container is HeaderGeneratedModel)
            {
                this.headerFooterLayerCache.ArrangeHeader(container.LayoutSlot);
                return;
            }

            if (container is FooterGeneratedModel)
            {
                this.headerFooterLayerCache.ArrangeFooter(container.LayoutSlot);
                return;
            }

            if (container is EmptyContentGeneratedModel)
            {
                this.emptyContentLayerCahce.ArrangeEmptyContent(container.LayoutSlot);
                return;
            }

            ListViewGroupHeader groupHeader = container.Container as ListViewGroupHeader;
            RadListViewItem listItem = container.Container as RadListViewItem;
            UIElement element = container.Container as UIElement;

            if (groupHeader != null)
            {
                groupHeader.arrangeRect = container.LayoutSlot.ToRect();
            }

            if (listItem != null)
            {
                listItem.arrangeRect = container.LayoutSlot.ToRect();
            }

            element.Arrange(container.LayoutSlot.ToRect());

            if (listItem != null && listItem.isDraggedForAction)
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    Canvas.SetLeft(element, container.LayoutSlot.X);
                }
                else
                {
                    Canvas.SetTop(element, container.LayoutSlot.Y);
                }
            }
            else
            {
                Canvas.SetLeft(element, container.LayoutSlot.X);
                Canvas.SetTop(element, container.LayoutSlot.Y);
            }

            if (groupHeader != null)
            {
                groupHeader.OwnerArranging = false;
            }
        }

        void IListView.SetScrollPosition(RadPoint point, bool updateUI, bool updateScrollViewer)
        {
            this.SetHorizontalOffset(point.X, updateUI, updateScrollViewer);
            this.SetVerticalOffset(point.Y, updateUI, updateScrollViewer);
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void IElementPresenter.RefreshNode(object node)
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        RadSize IElementPresenter.MeasureContent(object owner, object content)
        {
            return RadSize.Empty;
        }

        void IListView.ScrollToTop()
        {
            if (this.ScrollViewer != null)
            {
                if (this.Orientation == Orientation.Vertical)
                {
                    this.ScrollViewer.ChangeView(null, 0, null);
                }
                else
                {
                    this.ScrollViewer.ChangeView(0, null, null);
                }
            }
        }

        internal void InvalidatePanelMeasure(RadSize radSize)
        {
            if (this.contentPanel != null)
            {
                this.model.pendingMeasure = true;
                this.contentPanel.InvalidateMeasure();
            }
        }

        internal void InvalidatePanelArrange()
        {
            if (this.contentPanel != null)
            {
                this.contentPanel.InvalidateArrange();
            }
        }

        internal RadSize OnContentPanelMeasure(RadSize newAvailableSize)
        {
            newAvailableSize = ListViewModel.DoubleArithmetics.Ceiling(newAvailableSize);
            this.lastAvailableSize = newAvailableSize.ToSize();

            double w = newAvailableSize.Width;
            double h = newAvailableSize.Height;
            if (this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal && double.IsInfinity(newAvailableSize.Height))
            {
                h = this.MinHeight;
            }
            if (this.Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical && double.IsInfinity(newAvailableSize.Width))
            {
                w = this.MinWidth;
            }

            newAvailableSize = new RadSize(w, h);

            var size = this.Model.MeasureContent(newAvailableSize);

            double newContentHeight = 0;
            double newContentWidth = 0;

            if (this.Orientation == Orientation.Vertical)
            {
                newContentHeight = double.IsInfinity(newAvailableSize.Height) ? size.Height : Math.Max(size.Height, newAvailableSize.Height);
                newContentWidth = size.Width;
            }
            else
            {
                newContentHeight = size.Height;
                newContentWidth = double.IsInfinity(newAvailableSize.Width) ? size.Width : Math.Max(size.Width, newAvailableSize.Width);
            }

            this.childrenPanel.Height = newContentHeight;
            this.childrenPanel.Width = newContentWidth;

            this.animatingChildrenPanel.Height = newContentHeight;
            this.animatingChildrenPanel.Width = newContentWidth;

            if (this.ScrollableAdornerHost != null)
            {
                this.ScrollableAdornerHost.Height = newContentHeight;
                this.ScrollableAdornerHost.Width = newContentWidth;
            }

            if (size.Height > 0 || size.Width > 0)
            {
                this.itemsMeasured = true;
            }

            return size;
        }

        internal RadSize OnContentPanelArrange(RadSize finalSize)
        {
            var adjustedfinalSize = ListViewModel.DoubleArithmetics.Ceiling(finalSize);
            this.Model.ArrangeContent(adjustedfinalSize);

            return finalSize;
        }

        internal void ResetActionContent()
        {
            if (this.swipeActionContentControl != null)
            {
                this.swipeActionContentControl.Visibility = Visibility.Collapsed;
            }

            this.isActionContentDisplayed = false;
        }

        internal void OnGroupIsExpandedChanged()
        {
        }

        internal void OnGroupHeaderTap(ListViewGroupHeader header)
        {
            var context = header.DataContext as GroupHeaderContext;
            context.IsExpanded = header.IsExpanded;
            header.IsExpanded = context.IsExpanded;
        }

        /// <summary>
        /// Return the element used to display the given item.
        /// </summary>
        protected internal virtual RadListViewItem GetContainerForItem()
        {
            return new RadListViewItem();
        }

        /// <summary>
        /// Return the element used to display the group header.
        /// </summary>
        protected internal virtual ListViewGroupHeader GetContainerForGroupHeader()
        {
            return new ListViewGroupHeader();
        }
        
        /// <summary>
        /// Prepare the <see cref="ListViewLoadDataControl"/>.
        /// </summary>
        protected internal virtual void PrepareLoadDataControl(ListViewLoadDataControl control)
        {
        }

        /// <summary>
        /// Invalidate the <see cref="ListViewLoadDataControl"/>.
        /// </summary>
        protected internal void InvalidateLoadDataControl()
        {
            this.PrepareLoadDataControl(this.loadMoreDataControl);
        }

        /// <summary>
        /// Prepare the element to act as the ItemUI for the corresponding item.
        /// </summary>
        protected internal virtual void PrepareContainerForItem(RadListViewItem item, object context)
        {
            item.DataContext = context;
            item.Content = context;

            var style = this.ItemStyle;
            if (style == null)
            {
                if (this.ItemStyleSelector != null)
                {
                    style = this.ItemStyleSelector.SelectStyle(context, item);
                }
            }

            if (style != null)
            {
                item.Style = style;
            }
            else
            {
                item.ClearValue(RadListViewItem.StyleProperty);
            }

            var dataTemplate = this.ItemTemplate;
            if (dataTemplate == null)
            {
                if (this.ItemTemplateSelector != null)
                {
                    dataTemplate = this.ItemTemplateSelector.SelectTemplate(context, item);
                }
            }

            if (dataTemplate != null)
            {
                item.ContentTemplate = dataTemplate;
            }
            else
            {
                item.ClearValue(RadListViewItem.ContentTemplateProperty);
            }
        }

        /// <summary>
        /// Return the element used to display the group header.
        /// </summary>
        protected internal virtual ListViewGroupHeader GetContainerForGroupHeader(bool isFrozen)
        {
            var header = this.GetContainerForGroupHeader();

            header.IsFrozen = isFrozen;

            return header;
        }

        /// <summary>
        ///  Undoes the effects of the PrepareContainerForItem method.
        /// </summary>
        protected internal virtual void ClearContainerForItem(RadListViewItem item)
        {
            item.ClearValue(FrameworkElement.DataContextProperty);
        }

        /// <summary>
        ///  Undoes the effects of the PrepareContainerForGroupHeader method.
        /// </summary>
        protected internal virtual void ClearContainerForGroupHeader(ListViewGroupHeader item)
        {
            item.ClearValue(FrameworkElement.DataContextProperty);
        }
        
        /// <summary>
        /// Prepare the element to act as the ItemUI for the corresponding group header.
        /// </summary>
        protected internal virtual void PrepareContainerForGroupHeader(ListViewGroupHeader groupHeader, GroupHeaderContext context)
        {
            groupHeader.DataContext = context;

            var style = this.GroupHeaderStyle;
            if (style == null)
            {
                if (this.GroupHeaderStyleSelector != null)
                {
                    style = this.GroupHeaderStyleSelector.SelectStyle(context, groupHeader);
                }
            }

            if (style != null)
            {
                groupHeader.Style = style;
            }
            else
            {
                groupHeader.ClearValue(ListViewGroupHeader.StyleProperty);
            }

            var dataTemplate = this.GroupHeaderTemplate;
            if (dataTemplate == null)
            {
                if (this.GroupHeaderTemplateSelector != null)
                {
                    dataTemplate = this.GroupHeaderTemplateSelector.SelectTemplate(context, groupHeader);
                }
            }

            if (dataTemplate != null)
            {
                groupHeader.ContentTemplate = dataTemplate;
            }
            else
            {
                groupHeader.ClearValue(ListViewGroupHeader.ContentTemplateProperty);
            }
        }

        /// <summary>
        /// Recycles all containers and rebuilds the UI.
        /// </summary>
        protected virtual void InvalidateUI()
        {
            this.model.RecycleAllContainers();
            this.RebuildUI();
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadListViewAutomationPeer(this);
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            if (this.contentPanel != null)
            {
                this.contentPanel.Owner = null;
            }

            if (this.scrollViewer != null)
            {
                this.scrollViewer.ViewChanged -= this.ScrollViewer_ViewChanged;
                this.scrollViewer.SizeChanged -= this.OnScrollViewer_SizeChanged;
            }

            base.UnapplyTemplateCore();
        }

        /// <inheritdoc/>
        protected override void UnloadCore()
        {
            this.model.StopAllAnimations();

            base.UnloadCore();
        }

        /// <inheritdoc/>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.contentPanel.Owner = this;
            this.listViewRootPanel.Owner = this;

            this.scrollViewer.ViewChanged += this.ScrollViewer_ViewChanged;
            this.pullToRefreshIndicator.SizeChanged += this.PullToRefreshIndicator_SizeChanged;
            this.scrollViewer.SizeChanged += this.OnScrollViewer_SizeChanged;

            this.UpdateLayersOnTemplateApplied();

            this.updateService.Start();

            this.UpdateFrozenGroupHostVisibility();

            if (this.IncrementalLoadingMode == BatchLoadingMode.Auto && this.GroupDescriptors.Count == 0)
            {
                this.visualStateService.RegisterDataLoadingListener(this.scrollableAdornerLayerCache.Listener);
            }
            else
            {
                this.visualStateService.UnregisterDataLoadingListener(this.scrollableAdornerLayerCache.Listener);
            }

            if (this.LayoutDefinition is StaggeredLayoutDefinition && this.GroupDescriptors.Count > 0)
            {
                throw new NotSupportedException(StaggeredLayoutGroupingExceptionMessage);
            }

            this.GroupDescriptors.CollectionChanged += (s, e) =>
            {
                if (this.GroupDescriptors.Count > 0)
                {
                    this.visualStateService.UnregisterDataLoadingListener(this.scrollableAdornerLayerCache.Listener);

                    if (this.LayoutDefinition is StaggeredLayoutDefinition)
                    {
                        throw new NotSupportedException(StaggeredLayoutGroupingExceptionMessage);
                    }
                }
            };

            this.InvalidateMeasure();

            this.headerFooterLayerCache.UpdateHeader(this.ListHeader != null);

            this.headerFooterLayerCache.UpdateFooter(this.ListFooter != null);

            this.emptyContentLayerCahce.UpdateEmptyContent();

            this.InitializePullToRefresh();

            this.swipeActionContentControl.Visibility = Visibility.Collapsed;

            if (this.IsCheckModeActive)
            {
                this.OnIsCheckModeActiveChanged(true);
            }
        }

        /// <inheritdoc/>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.listViewRootPanel = this.GetTemplatePartField<ListViewRootPanel>("PART_ListViewRootPanel");
            applied = applied && this.listViewRootPanel != null;

            this.contentPanel = this.GetTemplatePartField<ListViewPanel>("PART_ScrollableContent");
            applied = applied && this.contentPanel != null;

            this.childrenPanel = this.GetTemplatePartField<Panel>("PART_ChildrenPanel");
            applied = applied && this.childrenPanel != null;

            this.animatingChildrenPanel = this.GetTemplatePartField<Panel>("PART_AnimatingChildrenPanel");
            applied = applied && this.animatingChildrenPanel != null;

            this.scrollViewer = this.GetTemplatePartField<ScrollViewer>("PART_ScrollViewer");
            applied = applied && this.scrollViewer != null;

            this.scrollableAdornerHostPanel = this.GetTemplatePartField<Panel>("PART_ScrollableAdornerHost");
            applied = applied && this.scrollableAdornerHostPanel != null;

            this.adornerHostPanel = this.GetTemplatePartField<Panel>("PART_AdornerHost");
            applied = applied && this.adornerHostPanel != null;

            this.pullToRefreshIndicator = this.GetTemplatePartField<PullToRefreshIndicator>("PART_PullToRefreshIndicator");
            applied = applied && this.pullToRefreshIndicator != null;

            this.rootScrollPanel = this.GetTemplatePartField<Panel>("PART_RootScrollablePanel");
            applied = applied && this.rootScrollPanel != null;

            this.swipeActionContentControl = this.GetTemplateChild("PART_SwipeActionContent") as ContentControl;
            applied = applied && this.swipeActionContentControl != null;

            this.frozenGroupHeadersHost = this.GetTemplateChild("PART_FrozenGroupHeadersHost") as Panel;
            applied = applied && this.frozenGroupHeadersHost != null;

            return applied;
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            this.panelAvailableSize = availableSize.ToRadSize();

            return base.MeasureOverride(availableSize);
        }

        private static void OnItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;
            listView.itemTemplateCache = e.NewValue as DataTemplate;
            listView.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
        }

        private static void OnItemStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;
            listView.itemStyleCache = e.NewValue as Style;
            listView.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
        }

        private static void OnItemStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;
            listView.itemStyleSelectorCache = e.NewValue as StyleSelector;
            listView.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
        }

        private static void OnItemTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;
            listView.itemTemplateSelectorCache = e.NewValue as DataTemplateSelector;
            listView.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
        }

        private static void OnGroupHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;
            listView.groupHeaderTemplateCache = e.NewValue as DataTemplate;
            listView.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
        }

        private static void OnGroupHeaderTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;
            listView.groupHeaderTemplateSelectorCache = e.NewValue as DataTemplateSelector;
            listView.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
        }

        private static void OnGroupHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;
            listView.groupHeaderStyleCache = e.NewValue as Style;
            listView.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
        }

        private static void OnGroupHeaderStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;
            listView.groupHeaderStyleSelectorCache = e.NewValue as StyleSelector;
            listView.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
        }

        private static void OnRealizedItemsBufferScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var list = d as RadListView;
            var scale = (double)e.NewValue;

            if (scale < 0)
            {
                throw new ArgumentException("The realized items buffer scale must positive number");
            }

            list.Model.BufferScale = scale;

            list.InvalidatePanelMeasure(list.panelAvailableSize);
        }

        private static void OnGroupHeaderDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;
            if (listView != null)
            {
                var enableFrozenDecorators = (ListViewGroupHeaderDisplayMode)e.NewValue == ListViewGroupHeaderDisplayMode.Frozen;
                listView.model.layoutController.strategy.EnableFrozenDecorators = enableFrozenDecorators;
                listView.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);

                listView.UpdateFrozenGroupHostVisibility();
            }
        }

        private static void OnListHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listview = d as RadListView;

            if (listview.IsTemplateApplied)
            {
                listview.headerFooterLayerCache.UpdateHeader(e.NewValue != null);
            }
        }

        private static void OnListFooterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listview = d as RadListView;

            if (listview.IsTemplateApplied)
            {
                listview.headerFooterLayerCache.UpdateFooter(e.NewValue != null);
            }
        }

        private static void OnIncrementalLoadingModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;
            if (listView != null)
            {
                var newMode = (BatchLoadingMode)e.NewValue;
                listView.model.DataLoadingMode = newMode;

                if (listView.IsTemplateApplied)
                {
                    if (newMode == BatchLoadingMode.Auto && listView.GroupDescriptors.Count == 0)
                    {
                        listView.visualStateService.RegisterDataLoadingListener(listView.scrollableAdornerLayerCache.Listener);
                    }
                    else
                    {
                        listView.visualStateService.UnregisterDataLoadingListener(listView.scrollableAdornerLayerCache.Listener);
                    }

                    listView.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
                }
            }
        }

        private static void IncrementalLoadingBufferItemsCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;
            listView.Model.DataLoadingBufferSize = (int)e.NewValue;
        }

        private static void OnLayoutDefinitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // TODO: refresh UI.
            var listView = d as RadListView;

            if (e.NewValue is StaggeredLayoutDefinition && listView.GroupDescriptors.Count > 0)
            {
                throw new NotSupportedException(StaggeredLayoutGroupingExceptionMessage);
            }

            listView.Model.OnLayoutDefinitionChanged(e.OldValue as LayoutDefinitionBase, e.NewValue as LayoutDefinitionBase);

            listView.model.layoutController.strategy.EnableFrozenDecorators = listView.GroupHeaderDisplayMode == ListViewGroupHeaderDisplayMode.Frozen;
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadListView;

            control.GroupDescriptors.TryRemoveCollectionViewGroup();

            var collView = e.NewValue as ICollectionView;
            if (collView != null && collView.CollectionGroups != null)
            {
                control.GroupDescriptors.Insert(0, new CollectionViewGroupDescriptor());
            }

            control.model.OnItemsSourceChanged(e.NewValue);
            control.currencyService.OnItemsSourceChanged(e.NewValue);
            control.selectionService.ClearSelection();
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;
            var orientation = (Orientation)e.NewValue;

            listView.model.OnOrientationChanged(orientation);

            listView.CleanupSwipedItem();

            if (listView.gestureRecognizer != null)
            {
                listView.gestureRecognizer.Dispose();
            }

            listView.InitializePullToRefresh();

            if (listView.gestureRecognizer != null)
            {
                listView.gestureRecognizer.SwipeTheshold = listView.refreshThreshold;
            }

            if (listView.IsTemplateApplied)
            {
                listView.pullToRefreshIndicator.SetOrientation(listView.Orientation);
            }

            listView.PositionPullToRefreshIndicator();
        }

        private static void OnEmptyContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadListView;
            if (control.IsTemplateApplied)
            {
                control.emptyContentLayerCahce.UpdateEmptyContent();
                control.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
            }
        }

        private static void OnEmptyContentDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadListView;
            if (control.IsTemplateApplied)
            {
                control.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
            }
        }

        private void AddLayer(ListViewLayer layer, Panel parent)
        {
            layer.DetachUI(layer.VisualElement as Panel);
            layer.Owner = this;
            layer.AttachUI(parent);
        }

        private void UpdateLayersOnTemplateApplied()
        {
            if (this.overlayAdornerLayerCache == null)
            {
                this.overlayAdornerLayerCache = new OverlayAdornerListViewLayer();
                this.visualStateService.RegisterDataLoadingListener(this.overlayAdornerLayerCache);
            }

            this.AddLayer(this.overlayAdornerLayerCache, this.adornerHostPanel);

            if (this.headerFooterLayerCache == null)
            {
                this.headerFooterLayerCache = new HeaderFooterListViewLayer();
            }

            if (this.Orientation == Orientation.Vertical)
            {
                this.AddLayer(this.headerFooterLayerCache, this.childrenPanel);
            }
            else
            {
                this.AddLayer(this.headerFooterLayerCache, this.listViewRootPanel);
            }

            if (this.emptyContentLayerCahce == null)
            {
                this.emptyContentLayerCahce = new EmptyContentListViewLayer();
            }

            this.AddLayer(this.emptyContentLayerCahce, this.childrenPanel);

            if (this.scrollableAdornerLayerCache == null)
            {
                this.scrollableAdornerLayerCache = new ScrollableAdornerListViewLayer();
            }

            this.AddLayer(this.scrollableAdornerLayerCache, this.scrollableAdornerHostPanel);

            if (this.currencyLayerCache == null)
            {
                this.currencyLayerCache = new CurrencyListViewLayer();
            }

            this.AddLayer(this.currencyLayerCache, this.childrenPanel);

            if (this.checkBoxLayerCache == null)
            {
                this.checkBoxLayerCache = new CheckBoxListViewLayer();
            }

            this.AddLayer(this.checkBoxLayerCache, this.animatingChildrenPanel);
        }

        private void SetOffset(ScrollViewer scrollViewer, double scrollDirectionOffset, double? crossDirectionOffset = null)
        {
            if (this.Orientation == Orientation.Horizontal)
            {
                scrollViewer.ChangeView(scrollDirectionOffset, crossDirectionOffset, null);
            }
            else
            {
                scrollViewer.ChangeView(crossDirectionOffset, scrollDirectionOffset, null);
            }
        }

        private double GetOffset(ScrollViewer scrollViewer)
        {
            return this.Orientation == Orientation.Horizontal ? scrollViewer.HorizontalOffset : scrollViewer.VerticalOffset;
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
            this.SetHorizontalOffset(this.scrollViewer.HorizontalOffset, true, false);
            this.SetVerticalOffset(this.scrollViewer.VerticalOffset, true, false);
        }

        private void SetVerticalOffset(double physicalOffset, bool updateUI, bool updateScrollViewer)
        {
            var adjustedOffset = ListViewModel.DoubleArithmetics.Ceiling(physicalOffset);
            bool needUpdate = !LayoutDoubleUtil.AreClose(this.lastScrollOffset.Y, physicalOffset) || physicalOffset < 0;
            this.lastScrollOffset.Y = adjustedOffset;

            if (needUpdate)
            {
                if (updateUI && this.contentPanel != null)
                {
                    this.contentPanel.InvalidateMeasure();
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
            this.lastScrollOffset.X = adjustedOffset;

            if (needUpdate)
            {
                // TODO: The updateUI flag should be removed and all the needed updates should be processed by the UpdateService
                if (updateUI)
                {
                    if (this.contentPanel != null)
                    {
                        this.contentPanel.InvalidateMeasure();
                    }
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

        private void OnScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.IsTemplateApplied)
            {
                this.UpdatePullToRefreshSize();
                this.RebuildUI();
            }
        }

        private void UpdateFrozenGroupHostVisibility()
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            // hide/show the frozen group headers panel
            this.frozenGroupHeadersHost.Visibility = this.GroupHeaderDisplayMode == ListViewGroupHeaderDisplayMode.Frozen ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}