using System;
using System.Collections.ObjectModel;
using Telerik.Core;
using Telerik.Core.Data;
using Telerik.Data.Core;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Telerik.UI.Xaml.Controls.Grid.Model;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Grid.View;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents a custom <see cref="Control"/> implementation that may be used to visualize data in tabular format - that is by rows and columns.
    /// </summary>
    [TemplatePart(Name = "PART_TableRoot", Type = typeof(DataGridRootPanel))]
    [TemplatePart(Name = "PART_ColumnHeadersHost", Type = typeof(DataGridColumnHeaderPanel))]
    [TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_DecorationsHost", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_CellsHost", Type = typeof(DataGridCellsPanel))]
    [TemplatePart(Name = "PART_HeadersHost", Type = typeof(FrozenGroupsPanel))]
    [TemplatePart(Name = "PART_ServicePanel", Type = typeof(DataGridServicePanel))]
    [TemplatePart(Name = "PART_AdornerHost", Type = typeof(Canvas))]
    public partial class RadDataGrid : RadControl, IGridView
    {
        /// <summary>
        /// Identifies the <see cref="RealizedItemsVerticalBufferScale"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty RealizedItemsBufferScaleProperty =
            DependencyProperty.Register(nameof(RealizedItemsVerticalBufferScale), typeof(double), typeof(RadDataGrid), new PropertyMetadata(1d, OnRealizedItemsBufferScaleChanged));

        /// <summary>
        /// Identifies the <see cref="IsBusyIndicatorEnabled"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsBusyIndicatorEnabledProperty =
            DependencyProperty.Register(nameof(IsBusyIndicatorEnabled), typeof(bool), typeof(RadDataGrid), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IndentWidth"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty IndentWidthProperty =
            DependencyProperty.Register(nameof(IndentWidth), typeof(double), typeof(RadDataGrid), new PropertyMetadata(24d, OnIndentWidthChanged));

        /// <summary>
        /// Identifies the RowHeight dependency property. 
        /// </summary>
        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register(nameof(RowHeight), typeof(double), typeof(RadDataGrid), new PropertyMetadata(double.NaN, OnRowHeightChanged));

        /// <summary>
        /// Identifies the <see cref="GroupHeaderTemplate"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty GroupHeaderTemplateProperty =
            DependencyProperty.Register(nameof(GroupHeaderTemplate), typeof(DataTemplate), typeof(RadDataGrid), new PropertyMetadata(null, OnGroupHeaderTemplateChanged));

        /// <summary>
        /// Identifies the <see cref="GroupHeaderTemplateSelector"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty GroupHeaderTemplateSelectorProperty =
            DependencyProperty.Register(nameof(GroupHeaderTemplateSelector), typeof(DataTemplateSelector), typeof(RadDataGrid), new PropertyMetadata(null, OnGroupHeaderTemplateSelectorChanged));

        /// <summary>
        /// Identifies the <see cref="GroupHeaderStyle"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty GroupHeaderStyleProperty =
            DependencyProperty.Register(nameof(GroupHeaderStyle), typeof(Style), typeof(RadDataGrid), new PropertyMetadata(null, OnGroupHeaderStyleChanged));

        /// <summary>
        /// Identifies the <see cref="GroupHeaderStyleSelector"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty GroupHeaderStyleSelectorProperty =
            DependencyProperty.Register(nameof(GroupHeaderStyleSelector), typeof(StyleSelector), typeof(RadDataGrid), new PropertyMetadata(null, OnGroupHeaderStyleSelectorChanged));

        /// <summary>
        /// Identifies the <see cref="GroupHeaderDisplayMode"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty GroupHeaderDisplayModeProperty =
            DependencyProperty.Register(nameof(GroupHeaderDisplayMode), typeof(DataGridGroupHeaderDisplayMode), typeof(RadDataGrid), new PropertyMetadata(DataGridGroupHeaderDisplayMode.Frozen, OnGroupHeaderDisplayModeChanged));

        /// <summary>
        /// Identifies the <see cref="UserSortMode"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty UserSortModeProperty =
            DependencyProperty.Register(nameof(UserSortMode), typeof(DataGridUserSortMode), typeof(RadDataGrid), new PropertyMetadata(DataGridUserSortMode.Auto));

        /// <summary>
        /// Identifies the <see cref="UserGroupMode"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty UserGroupModeProperty =
            DependencyProperty.Register(nameof(UserGroupMode), typeof(DataGridUserGroupMode), typeof(RadDataGrid), new PropertyMetadata(DataGridUserGroupMode.Auto, OnUserGroupModeChanged));

        /// <summary>
        /// Identifies the <see cref="ColumnResizeHandleDisplayMode"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ColumnResizeHandleDisplayModeProperty =
            DependencyProperty.Register(nameof(ColumnResizeHandleDisplayMode), typeof(DataGridColumnResizeHandleDisplayMode), typeof(RadDataGrid), new PropertyMetadata(DataGridColumnResizeHandleDisplayMode.None, OnColumnResizeModeChanged));

        /// <summary>
        /// Identifies the <see cref="UserColumnReorderMode"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty UserColumnReorderModeProperty =
            DependencyProperty.Register(nameof(UserColumnReorderMode), typeof(DataGridUserColumnReorderMode), typeof(RadDataGrid), new PropertyMetadata(DataGridUserColumnReorderMode.Interactive));

        /// <summary>
        /// Identifies the <see cref="UserGroupMode"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty UserFilterModeProperty =
            DependencyProperty.Register(nameof(UserFilterMode), typeof(DataGridUserFilterMode), typeof(RadDataGrid), new PropertyMetadata(DataGridUserFilterMode.Auto, OnUserFilterModeChanged));

        /// <summary>
        /// Identifies the <see cref="DragBehavior"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty DragBehaviorProperty =
            DependencyProperty.Register(nameof(DragBehavior), typeof(DataGridDragBehavior), typeof(RadDataGrid), new PropertyMetadata(null, OnDragBehaviorChanged));

        /// <summary>
        /// Identifies the <see cref="GroupPanelPosition"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty GroupPanelPositionProperty =
            DependencyProperty.Register(nameof(GroupPanelPosition), typeof(GroupPanelPosition), typeof(RadDataGrid), new PropertyMetadata(GroupPanelPosition.Left, OnGroupPanelPositionChanged));

        /// <summary>
        /// Identifies the <see cref="ListenForNestedPropertyChange"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ListenForNestedPropertyChangeProperty =
            DependencyProperty.Register(nameof(ListenForNestedPropertyChange), typeof(bool), typeof(RadDataGrid), new PropertyMetadata(false, OnListenForNestedPropertyChangePropertyChanged));

        private DataGridColumnHeaderPanel columnHeadersPanel;
        private DataGridCellsPanel cellsPanel;
        private DataGridRootPanel rootPanel;
        private Panel frozenGroupHeadersHost;
        private Panel scrollableAdornerHostPanel;
        private Panel adornerHostPanel;
        private DataGridServicePanel servicePanel;
        private Panel frozenColumnsHost;
        private Panel frozenColumnHeadersHost;
        private DataGridContentFlyout contentFlyout;
        private DataGridColumnReorderServicePanel columnReorderServicePanel;

        private GridEditRowLayer editRowLayer;
        private GridEditRowLayer frozenEditRowLayer;

        private DataTemplate groupHeaderTemplateCache;
        private DataTemplateSelector groupHeaderTemplateSelectorCache;
        private Style groupHeaderStyleCache;
        private StyleSelector groupHeaderStyleSelectorCache;
        private double indentWidthCache = 24;
        private DataGridGroupHeaderDisplayMode groupHeaderDisplayModeCache = DataGridGroupHeaderDisplayMode.Frozen;

        private Size lastAvailableSize;

        private KeyEventHandler keyDownHandler;

        private GridCellModel hoveredCell;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadDataGrid" /> class.
        /// </summary>
        public RadDataGrid()
        {
            this.DefaultStyleKey = typeof(RadDataGrid);

            this.lineDecorationsPresenter = new LineDecorationPresenter();
            this.frozenLineDecorationsPresenter = new LineDecorationPresenter();
            this.selectionDecorationsPresenter = new SelectionDecorationPresenter();
            this.frozenSelectionDecorationsPresenter = new SelectionDecorationPresenter();

            this.contentLayers = new ObservableCollection<ContentLayer>();
            this.contentLayers.CollectionChanged += this.OnContentLayersCollectionChanged;

            this.rowItemGenerator = new XamlGridRowGenerator(this);
            this.columnItemGenerator = new XamlGridHeaderCellGenerator(this);
            this.cellItemGenerator = new XamlGridCellGenerator(this);
            this.editRowItemGenerator = new XamlGridEditRowGenerator(this);
            this.cellEditorItemGenerator = new XamlGridEditCellGenerator(this);

            this.model = new GridModel(this, this, RadDataGrid.ShouldExecuteOperationsSyncroniously);
            this.model.RowPool.EnableFrozenDecorators = true;
            this.model.RowHeight = double.NaN;

            this.visualStateService = new VisualStateService(this);
            this.selectionService = new SelectionService(this);
            this.hitTestService = new HitTestService(this);
            this.commandService = new CommandService(this);
            this.updateService = new UpdateService(this, RadDataGrid.ShouldExecuteOperationsSyncroniously);
            this.editService = new EditingService(this);
            this.CurrencyService = new DataGridCurrencyService(this);
            this.rowDetailsService = new RowDetailsService(this);

            this.DragBehavior = new DataGridDragBehavior(this);

            this.editRowLayer = new GridEditRowLayer();
            this.frozenEditRowLayer = new GridEditRowLayer();

            this.keyDownHandler = new KeyEventHandler(this.OnScrollViewerKeyDown);
        }

        /// <summary>
        /// Occurs when the associated data (<see cref="ItemsSource"/>) has been successfully bound to the control or any data operation like Group, Sort or Filter is applied.
        /// This event is useful if an additional action is required once the data is ready to be visualized.
        /// </summary>
        public event EventHandler<DataBindingCompleteEventArgs> DataBindingComplete;

        /// <summary>
        /// Occurs when the currently selected items change.
        /// </summary>
        public event EventHandler<DataGridSelectionChangedEventArgs> SelectionChanged
        {
            add
            {
                this.selectionService.SelectionChanged += value;
            }
            remove
            {
                this.selectionService.SelectionChanged -= value;
            }
        }

        /// <summary>
        /// Gets or sets the relative to viewport size buffer scale that will be used to realize items outside viewport. Default value is 1.
        /// </summary>
        public double RealizedItemsVerticalBufferScale
        {
            get { return (double)GetValue(RealizedItemsBufferScaleProperty); }
            set { this.SetValue(RealizedItemsBufferScaleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the built-in BusyIndicator control is enabled. 
        /// If true, the Grid will display an indeterminate progress indicator while it is processing some background requests like filtering, sorting or grouping.
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
        /// Gets or sets the <see cref="DataGridUserFilterMode"/> value that defines whether the Filter Glyph is present in the header of the currently available filterable columns.
        /// The default value is <c>DataGridUserFilterMode.Auto</c>
        /// </summary>
        /// <example>
        /// XAML
        /// <code>
        ///  &lt;telerikGrid:RadDataGrid x:Name="dataGrid" UserFilterMode="Disabled"/&gt;
        /// </code>
        /// C#
        /// <code>
        /// this.dataGrid.UserFilterMode = DataGridUserFilterMode.Disabled;
        /// </code>
        /// </example>
        public DataGridUserFilterMode UserFilterMode
        {
            get
            {
                return (DataGridUserFilterMode)this.GetValue(UserFilterModeProperty);
            }
            set
            {
                this.SetValue(UserFilterModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataGridUserColumnReorderMode"/> value that defines how User Input 
        /// (dragging column header over another headers) affects the current column position in the grid.
        /// The default value is <c>DataGridUserColumnReorderMode.Interactive</c>
        /// </summary>
        /// <example>
        /// XAML
        /// <code>
        /// &lt;telerikGrid:RadDataGrid x:Name="dataGrid" UserColumnReorderMode="None"/&gt;
        /// </code>
        /// C#
        /// <code>
        /// this.dataGrid.UserColumnReorderMode = DataGridUserColumnReorderMode.None;
        /// </code>
        /// </example>
        public DataGridUserColumnReorderMode UserColumnReorderMode
        {
            get
            {
                return (DataGridUserColumnReorderMode)this.GetValue(UserColumnReorderModeProperty);
            }
            set
            {
                this.SetValue(UserColumnReorderModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataGridColumnResizeHandleDisplayMode"/> value that defines how User Input 
        /// (dragging column resize thumb) affects the current column size in the grid.
        /// The default value is <c>DataGridColumnResizeHandleDisplayMode.OnHold</c>
        /// </summary>
        /// <example>
        /// XAML
        /// <code>
        /// &lt;telerikGrid:RadDataGrid x:Name="dataGrid" ColumnResizeHandleDisplayMode="None"/&gt;
        /// </code>
        /// C#
        /// <code>
        /// this.dataGrid.ColumnResizeHandleDisplayMode = DataGridColumnResizeHandleDisplayMode.None;
        /// </code>
        /// </example>
        public DataGridColumnResizeHandleDisplayMode ColumnResizeHandleDisplayMode
        {
            get
            {
                return (DataGridColumnResizeHandleDisplayMode)this.GetValue(ColumnResizeHandleDisplayModeProperty);
            }
            set
            {
                this.SetValue(ColumnResizeHandleDisplayModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataGridUserSortMode"/> value that defines how User Input (column header tap) affects the current Sort state of the grid.
        /// The default value is <c>DataGridUserSortMode.Auto</c>
        /// </summary>
        /// <example>
        /// XAML
        /// <code>
        /// &lt;telerikGrid:RadDataGrid x:Name="dataGrid" UserSortMode="None"/&gt;
        /// </code>
        /// C#
        /// <code>
        /// this.dataGrid.UserSortMode = DataGridUserSortMode.None;
        /// </code>
        /// </example>
        public DataGridUserSortMode UserSortMode
        {
            get
            {
                return (DataGridUserSortMode)this.GetValue(UserSortModeProperty);
            }
            set
            {
                this.SetValue(UserSortModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataGridUserGroupMode"/> value that determines whether the User is allowed to change the current Grouping state through the User Interface.
        /// The default value is <c>DataGridUserGroupMode.Auto</c>
        /// </summary>
        /// <example>
        /// XAML
        /// <code>
        /// &lt;telerikGrid:RadDataGrid x:Name="dataGrid" UserGroupMode="Disabled"/&gt;
        /// </code>
        /// C#
        /// <code>this.dataGrid.UserGroupMode = DataGridUserGroupMode.Disabled</code>
        /// </example>
        public DataGridUserGroupMode UserGroupMode
        {
            get
            {
                return (DataGridUserGroupMode)this.GetValue(UserGroupModeProperty);
            }
            set
            {
                this.SetValue(UserGroupModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that is used to offset different levels of nested groups.
        /// The default value is 24.
        /// </summary>
        public double IndentWidth
        {
            get
            {
                return this.indentWidthCache;
            }
            set
            {
                this.SetValue(IndentWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataGridGroupHeaderDisplayMode"/> value that determines how group headers are displayed.
        /// The default value is <c>DataGridGroupHeaderDisplayMode.Frozen</c> 
        /// </summary>
        /// <example>
        /// XAML
        /// <code>&lt;telerikGrid:RadDataGrid x:Name="dataGrid" GroupHeaderDisplayMode="Scrollable"/&gt;</code>
        /// C#
        /// <code>this.dataGrid.GroupHeaderDisplayMode = DataGridGroupHeaderDisplayMode.Scrollable;</code>
        /// </example>
        public DataGridGroupHeaderDisplayMode GroupHeaderDisplayMode
        {
            get
            {
                return this.groupHeaderDisplayModeCache;
            }
            set
            {
                this.SetValue(GroupHeaderDisplayModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> instance that defines the appearance of each group header within the grid.
        /// The data context of each group header will be a <see cref="GroupHeaderContext"/> instance.
        /// </summary>
        /// <example>
        /// XAML
        /// <code>
        ///   &lt;telerikGrid:RadDataGrid.GroupHeaderTemplate&gt;
        ///        &lt;DataTemplate&gt;
        ///            &lt;TextBlock Text="{Binding Level}" FontSize="26"/&gt;
        ///        &lt;/DataTemplate&gt;
        ///     &lt;/telerikGrid:RadDataGrid.GroupHeaderTemplate&gt;
        /// </code>
        /// </example>
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
        /// Gets or sets the <see cref="DataTemplateSelector"/> instance that may be used to select group header template on a per header instance basis.
        /// The "Item" parameter of the <see cref="M:DataTemplateSelector.SelectTemplate"/> method will be a <see cref="GroupHeaderContext"/> instance.
        /// </summary>
        /// <example>
        /// C#
        /// <code>
        /// public class CustomDataTemplateSelector : DataTemplateSelector
        /// {
        ///    public DataTemplate ExpandedTemplate { get; set; }
        ///    public DataTemplate CollapsedTemplate { get; set; }
        ///    protected override DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        ///    {
        ///        if ((item as GroupHeaderContext).IsExpanded == true) { return this.ExpandedTemplate; } else { return this.CollapsedTemplate; }
        ///    }
        /// }
        /// </code>
        /// </example>
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
        /// Gets or sets the <see cref="Style"/> instance that defines the appearance of all the group headers within the grid.
        /// The style instance should target the <see cref="DataGridGroupHeader"/> type.
        /// </summary>
        /// <example>
        /// XAML
        /// <code>                 &lt;Style TargetType="GridPrimitives:DataGridGroupHeader"&gt;
        ///            &lt;Setter Property="Foreground" Value="Red"/&gt;
        ///       &lt;/Style&gt;</code>
        /// </example>
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
        /// Gets or sets the <see cref="StyleSelector"/> instance that may be used to select different styles on a per group header basis.
        /// The style instance returned should target the <see cref="DataGridGroupHeader"/> type.
        /// </summary>
        /// <example>
        /// C#
        /// <code>
        ///     public class CustomStyleSelector : StyleSelector
        /// {
        /// public Style ExpandedStyle {get;set;}
        /// public Style CollapsedStyle {get;set;}
        /// protected override Style SelectStyleCore(object item, DependencyObject container)
        /// {
        ///    if ((item as GroupHeaderContext).IsExpanded == true) { return this.ExpandedStyle;} else{ return this.CollapsedStyle;}
        /// }
        /// }
        /// </code>
        /// </example>
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
        /// Gets the collection with all the custom commands registered with the <see cref="CommandService"/>. Custom commands have higher priority than the built-in (default) ones.
        /// </summary>
        public CommandCollection<RadDataGrid> Commands
        {
            get
            {
                return this.commandService.UserCommands;
            }
        }

        /// <summary>
        /// Gets or sets the standard height of rows in the control. The default value is <see cref="double.NaN"/> (auto).
        /// </summary>
        public double RowHeight
        {
            get
            {
                return this.model.RowHeight;
            }
            set
            {
                this.SetValue(RowHeightProperty, value);
            }
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
        /// Gets or sets the behavior controlling drag operations inside the DataGrid.
        /// </summary>
        public DataGridDragBehavior DragBehavior
        {
            get
            {
                return (DataGridDragBehavior)this.GetValue(DragBehaviorProperty);
            }
            set
            {
                this.SetValue(DragBehaviorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the DataGrid should be updated if INotifyPropertyChanged item from its source is changed.
        /// </summary>
        public bool ListenForNestedPropertyChange
        {
            get
            {
                return (bool)this.GetValue(ListenForNestedPropertyChangeProperty);
            }
            set
            {
                this.SetValue(ListenForNestedPropertyChangeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the position of the Group Panel related to the DataGrid.
        /// </summary>
        public GroupPanelPosition GroupPanelPosition
        {
            get { return (GroupPanelPosition)GetValue(GroupPanelPositionProperty); }
            set { this.SetValue(GroupPanelPositionProperty, value); }
        }

        internal bool IsServicePanelVisible
        {
            get
            {
                // TODO: Add the NewRow notation when implemented
                return this.UserGroupMode != DataGridUserGroupMode.Disabled;
            }
        }

        internal RadSize CellsHostAvaialbleSize
        {
            get
            {
                return this.rootPanel.CellsHostAvailableSize.ToRadSize();
            }
        }
        
        internal DataGridColumn LastSelectedColumn
        {
            get;
            private set;
        }

        internal bool RootPanelMeasure { get; set; }

        internal ScrollViewer ScrollViewer
        {
            get
            {
                return this.scrollViewer;
            }
        }

        internal DataGridColumnHeaderPanel ColumnHeadersHost
        {
            get
            {
                return this.columnHeadersPanel;
            }
        }

        internal Panel FrozenColumnHeadersHost
        {
            get
            {
                // TODO: Consider making it DataGridColumnHeaderPanel
                return this.frozenColumnHeadersHost;
            }
        }

        internal Panel DecorationsHost
        {
            get
            {
                return this.decorationsHost;
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

        internal Panel FrozenGroupHeadersHost
        {
            get
            {
                return this.frozenGroupHeadersHost;
            }
        }

        internal DataGridCellsPanel CellsPanel
        {
            get
            {
                return this.cellsPanel;
            }
        }

        internal DataGridServicePanel ServicePanel
        {
            get
            {
                return this.servicePanel;
            }
        }

        internal DataGridColumnReorderServicePanel ColumnReorderServicePanel
        {
            get
            {
                return this.columnReorderServicePanel;
            }
        }

        internal GridEditRowLayer EditRowLayer
        {
            get
            {
                return this.editRowLayer;
            }
        }

        internal GridEditRowLayer FrozenEditRowLayer
        {
            get
            {
                return this.frozenEditRowLayer;
            }
        }

        internal Panel FrozenContentHost
        {
            get
            {
                return this.frozenColumnsHost;
            }
        }

        internal DataGridContentFlyout ContentFlyout
        {
            get
            {
                return this.contentFlyout;
            }
        }

        private static bool ShouldExecuteOperationsSyncroniously
        {
            get
            {
                return RadControl.IsInTestMode || DesignMode.DesignModeEnabled;
            }
        }

        internal RadSize OnCellsPanelMeasure(RadSize newAvailableSize)
        {
            newAvailableSize = GridModel.DoubleArithmetics.Ceiling(newAvailableSize);
            this.lastAvailableSize = newAvailableSize.ToSize();
            var size = this.model.MeasureCells(newAvailableSize);

            return size;
        }

        internal RadSize OnCellsPanelArrange(RadSize finalSize)
        {
            var adjustedfinalSize = GridModel.DoubleArithmetics.Ceiling(finalSize);
            var resultSize = this.model.ArrangeCells(adjustedfinalSize);

            return resultSize;
        }

        internal RadSize OnHeaderRowMeasure(RadSize newAvailableSize)
        {
            newAvailableSize = GridModel.DoubleArithmetics.Ceiling(newAvailableSize);
            return this.model.MeasureHeaderRow(newAvailableSize);
        }

        internal RadSize OnHeaderRowArrange(RadSize finalSize)
        {
            finalSize = GridModel.DoubleArithmetics.Ceiling(finalSize);
            return this.model.ArrangeHeaderRow(finalSize);
        }

        internal void OnDataBindingComplete(DataBindingCompleteEventArgs e)
        {
            bool scrollToCurrentItem = e.ChangeFlags == DataChangeFlags.PropertyChanged ? false : true;
            this.CurrencyService.OnDataBindingComplete(scrollToCurrentItem);

            var eh = this.DataBindingComplete;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.scrollViewer = this.GetTemplatePartField<ScrollViewer>("PART_ScrollViewer");
            applied = applied && this.scrollViewer != null;

            this.columnHeadersPanel = this.GetTemplatePartField<DataGridColumnHeaderPanel>("PART_ColumnHeadersHost");
            applied = applied && this.columnHeadersPanel != null;

            this.cellsPanel = this.GetTemplatePartField<DataGridCellsPanel>("PART_CellsHost");
            applied = applied && this.cellsPanel != null;

            this.rootPanel = this.GetTemplatePartField<DataGridRootPanel>("PART_TableRoot");
            applied = applied && this.rootPanel != null;

            this.servicePanel = this.GetTemplatePartField<DataGridServicePanel>("PART_ServicePanel");
            applied = applied && this.servicePanel != null;

            this.decorationsHost = this.GetTemplatePartField<Panel>("PART_DecorationsHost");
            applied = applied && this.decorationsHost != null;

            this.FrozenDecorationsHost = this.GetTemplatePartField<Panel>("PART_FrozenDecorationsHost");
            applied = applied && this.FrozenDecorationsHost != null;

            this.GroupHeadersHost = this.GetTemplatePartField<Panel>("PART_GroupHeadersHost");
            applied = applied && this.GroupHeadersHost != null;

            this.frozenGroupHeadersHost = this.GetTemplatePartField<Panel>("PART_HeadersHost");
            applied = applied && this.frozenGroupHeadersHost != null;

            this.adornerHostPanel = this.GetTemplatePartField<Panel>("PART_AdornerHost");
            applied = applied && this.adornerHostPanel != null;

            this.scrollableAdornerHostPanel = this.GetTemplatePartField<Panel>("PART_ScrollableAdornerHost");
            applied = applied && this.scrollableAdornerHostPanel != null;

            this.frozenColumnsHost = this.GetTemplatePartField<Panel>("PART_FrozenColumnsHost");
            applied = applied && this.frozenColumnsHost != null;

            this.frozenColumnHeadersHost = this.GetTemplatePartField<Panel>("PART_FrozenColumnHeadersHost");
            applied = applied && this.frozenColumnHeadersHost != null;

            this.contentFlyout = this.GetTemplatePartField<DataGridContentFlyout>("PART_GridFlyout");
            applied = applied && this.contentFlyout != null;

            this.columnReorderServicePanel = this.GetTemplatePartField<DataGridColumnReorderServicePanel>("PART_ColumnReorderServicePanel");
            applied = applied && this.columnReorderServicePanel != null;

            return applied;
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            this.UnsubscribeFromFrozenHostEvents();

            base.UnapplyTemplateCore();

            this.updateService.Stop();

            this.scrollViewer.ViewChanged -= this.OnScrollViewerViewChanged;
            this.scrollViewer.KeyDown -= this.keyDownHandler;

            this.UpdateLayersOnTemplateUnapplied();

            this.columnHeadersPanel.Owner = null;
            this.cellsPanel.Owner = null;
            this.rootPanel.Owner = null;
            this.servicePanel.Owner = null;
            this.editRowLayer.Owner = null;
            this.frozenEditRowLayer.Owner = null;
            this.contentFlyout.Owner = null;
            this.columnReorderServicePanel.Owner = null;

#if WINDOWS_UWP
            this.isListeningCurrentViewBackRequested = false;
            if (!DesignMode.DesignModeEnabled)
            {
                SystemNavigationManager.GetForCurrentView().BackRequested -= this.RadDataGrid_BackRequested;
            }
#endif
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.scrollViewer.ViewChanged += this.OnScrollViewerViewChanged;
            this.scrollViewer.AddHandler(ScrollViewer.KeyDownEvent, this.keyDownHandler, true);

            this.columnHeadersPanel.Owner = this;
            this.cellsPanel.Owner = this;
            this.rootPanel.Owner = this;
            this.servicePanel.Owner = this;

            this.UpdateLayersOnTemplateApplied();
            this.UpdateFrozenGroupHostVisibility();
            this.UpdateServicePanelVisibility();

            this.updateService.Start();

            if (this.IncrementalLoadingMode == BatchLoadingMode.Auto && this.GroupDescriptors.Count == 0)
            {
                this.visualStateService.RegisterDataLoadingListener(this.scrolalbleAdornerLayerCache.Listener);
            }
            else
            {
                this.visualStateService.UnregisterDataLoadingListener(this.scrolalbleAdornerLayerCache.Listener);
            }

            this.GroupDescriptors.CollectionChanged += (s, args) =>
            {
                if (this.GroupDescriptors.Count > 0)
                {
                    this.visualStateService.UnregisterDataLoadingListener(this.scrolalbleAdornerLayerCache.Listener);
                }
            };

            this.SubscribeToFrozenHostEvents();

            this.contentFlyout.Owner = this;
            this.columnReorderServicePanel.Owner = this;
            this.cellFlyoutShowTimeOutAnimationBoard = new Storyboard();
            this.cellFlyoutShowTimeOutAnimation = new DoubleAnimation();
            this.cellFlyoutShowTimeOutAnimation.Duration = TimeSpan.FromSeconds(1);
            this.cellFlyoutShowTimeOutAnimationBoard.Children.Add(this.cellFlyoutShowTimeOutAnimation);
            Storyboard.SetTarget(this.cellFlyoutShowTimeOutAnimation, this.ContentFlyout);
            Storyboard.SetTargetProperty(this.cellFlyoutShowTimeOutAnimation, "Opacity");

            this.UpdateColumnChooserButtonVisibility(this.CanUserChooseColumns);

#if WINDOWS_UWP
            if (this.HideFlyoutOnBackButtonPressed)
            {
                if (!this.isListeningCurrentViewBackRequested)
                {
                    this.isListeningCurrentViewBackRequested = true;
                    if (!DesignMode.DesignModeEnabled)
                    {
                        SystemNavigationManager.GetForCurrentView().BackRequested -= this.RadDataGrid_BackRequested;
                    }
                }
            }
            else
            {
                this.isListeningCurrentViewBackRequested = false;
                if (!DesignMode.DesignModeEnabled)
                {
                    SystemNavigationManager.GetForCurrentView().BackRequested -= this.RadDataGrid_BackRequested;
                }
            }
#endif

            this.rowDetailsService.Init();
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadDataGridAutomationPeer(this);
        }
        
        private static void OnDragBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldBehavior = e.OldValue as DataGridDragBehavior;
            var newBehavior = e.NewValue as DataGridDragBehavior;

            if (oldBehavior != null)
            {
                oldBehavior.Owner = null;
            }

            if (newBehavior != null)
            {
                newBehavior.Owner = d as RadDataGrid;
            }
        }

        private static void OnRealizedItemsBufferScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            var scale = (double)e.NewValue;
            
            if (scale < 0)
            {
                throw new ArgumentException("The realized items buffer scale must positive number");
            }

            grid.Model.VerticalBufferScale = scale;

            grid.InvalidatePanelsMeasure();
        }

        private static void OnGroupPanelPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;

            if (grid != null)
            {
                grid.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
            }
        }

        private static void OnListenForNestedPropertyChangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadDataGrid grid = d as RadDataGrid;
            grid.model.ListenForNestedPropertyChange = (bool)e.NewValue;
        }

        private static void OnColumnResizeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;

            if (grid != null)
            {
                grid.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
            }
        }

        private static void OnRowHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadDataGrid table = d as RadDataGrid;
            if (table != null)
            {
                table.model.RowHeight = (double)e.NewValue;
                table.updateService.RegisterUpdate(UpdateFlags.None);
            }
        }

        private static void OnGroupHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            grid.groupHeaderTemplateCache = e.NewValue as DataTemplate;
            grid.updateService.RegisterUpdate(UpdateFlags.AffectsContent);
        }

        private static void OnGroupHeaderTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            grid.groupHeaderTemplateSelectorCache = e.NewValue as DataTemplateSelector;
            grid.updateService.RegisterUpdate(UpdateFlags.AffectsContent);
        }

        private static void OnGroupHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            grid.groupHeaderStyleCache = e.NewValue as Style;
            grid.updateService.RegisterUpdate(UpdateFlags.AffectsContent);
        }

        private static void OnGroupHeaderStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            grid.groupHeaderStyleSelectorCache = e.NewValue as StyleSelector;
            grid.updateService.RegisterUpdate(UpdateFlags.AffectsContent);
        }

        private static void OnIndentWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            grid.indentWidthCache = (double)e.NewValue;
            grid.updateService.RegisterUpdate(UpdateFlags.AffectsContent);
        }

        private static void OnGroupHeaderDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;

            grid.groupHeaderDisplayModeCache = (DataGridGroupHeaderDisplayMode)e.NewValue;
            grid.model.RowPool.EnableFrozenDecorators = grid.groupHeaderDisplayModeCache == DataGridGroupHeaderDisplayMode.Frozen;
            grid.UpdateFrozenGroupHostVisibility();

            grid.updateService.RegisterUpdate(UpdateFlags.AffectsContent);
        }

        private static void OnUserGroupModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadDataGrid grid = d as RadDataGrid;
            if (grid.IsTemplateApplied)
            {
                grid.UpdateServicePanelVisibility();
                grid.InvalidateMeasure();
            }
        }

        private static void OnUserFilterModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            grid.updateService.RegisterUpdate(UpdateFlags.AffectsContent);
        }

        private void ResetSelectedHeader()
        {
            if (this.LastSelectedColumn != null)
            {
                this.UpdateSelectedHeader(this.LastSelectedColumn.HeaderControl, false);
                this.LastSelectedColumn = null;
            }
        }

        private void UpdateServicePanelVisibility()
        {
            if (this.IsTemplateApplied)
            {
                this.servicePanel.Visibility = this.IsServicePanelVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void UpdateFrozenGroupHostVisibility()
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            // hide/show the frozen group headers panel
            this.frozenGroupHeadersHost.Visibility = this.groupHeaderDisplayModeCache == DataGridGroupHeaderDisplayMode.Frozen ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}