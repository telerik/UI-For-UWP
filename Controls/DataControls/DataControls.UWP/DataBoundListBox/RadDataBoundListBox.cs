using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using Telerik.Core.Data;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Data.DataBoundListBox;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a ListBox control that provides currency and selection logic.
    /// </summary>
    [TemplatePart(Name = "PART_ItemsPanel", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_EmptyContentPresenter", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_CheckBoxesPressIndicator", Type = typeof(Rectangle))]
    [StyleTypedPropertyAttribute(Property = "ItemContainerStyle", StyleTargetType = typeof(RadDataBoundListBoxItem))]
    [StyleTypedPropertyAttribute(Property = "PullToRefreshIndicatorStyle", StyleTargetType = typeof(PullToRefreshIndicatorControl))]
    [StyleTypedPropertyAttribute(Property = "ItemReorderControlStyle", StyleTargetType = typeof(ItemReorderControl))]
    [StyleTypedPropertyAttribute(Property = "CheckBoxStyle", StyleTargetType = typeof(ItemCheckBox))]
    public partial class RadDataBoundListBox : RadVirtualizingDataControl
    {
        /// <summary>
        /// Identifies the <see cref="CheckBoxStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CheckBoxStyleProperty =
            DependencyProperty.Register(nameof(CheckBoxStyle), typeof(Style), typeof(RadDataBoundListBox), new PropertyMetadata(null, OnCheckBoxStyleChanged));

        /// <summary>
        /// Identifies the <see cref="EmptyContentDisplayMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EmptyContentDisplayModeProperty =
            DependencyProperty.Register(nameof(EmptyContentDisplayMode), typeof(EmptyContentDisplayMode), typeof(RadDataBoundListBox), new PropertyMetadata(EmptyContentDisplayMode.Always, OnEmptyContentDisplayModeChanged));

        /// <summary>
        /// Identifies the <see cref="ListHeaderTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ListHeaderTemplateProperty =
            DependencyProperty.Register(nameof(ListHeaderTemplate), typeof(DataTemplate), typeof(RadDataBoundListBox), new PropertyMetadata(null, OnListHeaderTemplateChanged));

        /// <summary>
        /// Identifies the <see cref="ListHeaderContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ListHeaderContentProperty =
            DependencyProperty.Register(nameof(ListHeaderContent), typeof(object), typeof(RadDataBoundListBox), new PropertyMetadata(null, OnListHeaderContentChanged));

        /// <summary>
        /// Identifies the <see cref="ListFooterTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ListFooterTemplateProperty =
            DependencyProperty.Register(nameof(ListFooterTemplate), typeof(DataTemplate), typeof(RadDataBoundListBox), new PropertyMetadata(null, OnListFooterTemplateChanged));

        /// <summary>
        /// Identifies the <see cref="ListFooterContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ListFooterContentProperty =
            DependencyProperty.Register(nameof(ListFooterContent), typeof(object), typeof(RadDataBoundListBox), new PropertyMetadata(null, OnListFooterContentChanged));

        /// <summary>
        /// Identifies the <see cref="IncrementalLoadingItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IncrementalLoadingItemTemplateProperty =
            DependencyProperty.Register(nameof(IncrementalLoadingItemTemplate), typeof(DataTemplate), typeof(RadDataBoundListBox), new PropertyMetadata(null, OnIncrementalLoadingItemTemplateChanged));

        /// <summary>
        /// Identifies the <see cref="IncrementalLoadingItemContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IncrementalLoadingItemContentProperty =
            DependencyProperty.Register(nameof(IncrementalLoadingItemContent), typeof(object), typeof(RadDataBoundListBox), new PropertyMetadata(null, OnIncrementalLoadingItemContentChanged));

        /// <summary>
        /// Identifies the ItemLoadingTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemLoadingTemplateProperty =
            DependencyProperty.Register(nameof(ItemLoadingTemplate), typeof(DataTemplate), typeof(RadDataBoundListBox), new PropertyMetadata(null, OnItemLoadingTemplateChanged));

        /// <summary>
        /// Identifies the <see cref="ItemLoadingContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemLoadingContentProperty =
            DependencyProperty.Register(nameof(ItemLoadingContent), typeof(object), typeof(RadDataBoundListBox), new PropertyMetadata(null, OnItemLoadingContentChanged));

        /// <summary>
        /// Identifies the <see cref="EmptyContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EmptyContentTemplateProperty =
            DependencyProperty.Register(nameof(EmptyContentTemplate), typeof(DataTemplate), typeof(RadDataBoundListBox), new PropertyMetadata(null, OnEmptyContentTemplateChanged));

        /// <summary>
        /// Identifies the <see cref="EmptyContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EmptyContentProperty =
            DependencyProperty.Register(nameof(EmptyContent), typeof(object), typeof(RadDataBoundListBox), new PropertyMetadata(null, OnEmptyContentChanged));

        /// <summary>
        /// Defines the <see cref="IsSynchronizedWithCurrentItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty =
            DependencyProperty.Register(nameof(IsSynchronizedWithCurrentItem), typeof(bool?), typeof(RadDataBoundListBox), new PropertyMetadata(null, OnIsSynchronizedWithCurrentItemChanged));

        /// <summary>
        /// Defines the <see cref="SelectedItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(RadDataBoundListBox), new PropertyMetadata(null, OnSelectedItemChanged));

        /// <summary>
        /// Identifies the <see cref="IncrementalLoadingMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IncrementalLoadingModeProperty =
            DependencyProperty.Register(nameof(IncrementalLoadingMode), typeof(BatchLoadingMode), typeof(RadVirtualizingDataControl), new PropertyMetadata(BatchLoadingMode.Auto, OnIncrementalLoadingModeChanged));

        /// <summary>
        /// Identifies the <see cref="ListHeaderDisplayMode"/> property.
        /// </summary>
        public static readonly DependencyProperty ListHeaderDisplayModeProperty =
            DependencyProperty.Register(nameof(ListHeaderDisplayMode), typeof(HeaderFooterDisplayMode), typeof(RadDataBoundListBox), new PropertyMetadata(HeaderFooterDisplayMode.AlwaysVisible, OnListHeaderDisplayModeChanged));

        /// <summary>
        /// Identifies the <see cref="ListFooterDisplayMode"/> property.
        /// </summary>
        public static readonly DependencyProperty ListFooterDisplayModeProperty =
            DependencyProperty.Register(nameof(ListFooterDisplayMode), typeof(HeaderFooterDisplayMode), typeof(RadDataBoundListBox), new PropertyMetadata(HeaderFooterDisplayMode.AlwaysVisible, OnListFooterDisplayModeChanged));

        /// <summary>
        /// Defines the <see cref="SelectedValue"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(nameof(SelectedValue), typeof(object), typeof(RadDataBoundListBox), new PropertyMetadata(null));

        /// <summary>
        /// Defines the <see cref="SelectedValuePath"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register(nameof(SelectedValuePath), typeof(string), typeof(RadDataBoundListBox), new PropertyMetadata(null, OnSelectedValuePathChanged));

        internal static readonly IncrementalLoadingIndicatorItem IncrementalLoadingIndicator = new IncrementalLoadingIndicatorItem(null, new object());
        internal static readonly ListHeaderIndicatorItem ListHeaderIndicator = new ListHeaderIndicatorItem(null, new object());
        internal static readonly ListFooterIndicatorItem ListFooterIndicator = new ListFooterIndicatorItem(null, new object());

        internal bool hasHeader = false;
        internal bool hasFooter = false;
        internal bool isPullToRefreshEnabledCache = false;
        internal BatchLoadingMode incrementalLoadingModeCache = BatchLoadingMode.Auto;
        internal DataBoundListBoxListSourceItemFactory listSourceFactory;
        internal Style checkBoxStyleCache;

        private const int DataRequestEventThreshold = 1000;

        private RadVirtualizingDataControlItem headerContainerCache;
        private RadVirtualizingDataControlItem footerContainerCache;
        private RadVirtualizingDataControlItem dataRequestContainerCache;
        private PropertyInfo selectedValuePropInfo;
        private string selectedValuePathCache;
        private DataTemplate incrementalLoadingItemTemplateCache = null;
        private object incrementalLoadingItemContentCache = null;
        private object listHeaderContentCache = null;
        private object listFooterContentCache = null;
        private object itemLoadingContentCache = null;
        private DataTemplate listHeaderTemplateCache = null;
        private DataTemplate listFooterTemplateCache = null;
        private DataTemplate itemLoadingTemplateCache = null;
        private bool isInternalSelectionChange = false;
        private object selectedItemCache;
        private IEnumerable itemsSourceCache = null;
        private bool isCompositionTargetRenderListening;
        private double maxScrollingOffset = 100;

        private BatchLoadingStatus currentLoadingStatus = BatchLoadingStatus.ItemsLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadDataBoundListBox"/> class.
        /// </summary>
        public RadDataBoundListBox()
        {
            this.DefaultStyleKey = typeof(RadDataBoundListBox);
            this.ListSource.CurrencyMode = CurrencyManagementMode.LocalAndExternal;
            this.checkedItems = new CheckedItemsCollection<object>(this);
            this.PrepareCheckboxesSupport();
            this.SizeChanged += this.RadDataBoundListBox_SizeChanged;

            // TODO:CONTEXTMENU
            //// RadContextMenu.SetFocusedElementType(this, typeof(RadDataBoundListBoxItem));
        }

        /// <summary>
        /// Fires when the user pulls down the scrollable list to refresh the data. This event will fire
        /// when the <see cref="IsPullToRefreshEnabled"/> property is set to true.
        /// </summary>
        public event EventHandler<EventArgs> RefreshRequested;

        /// <summary>
        /// Occurs when an item within the control has been tapped.
        /// </summary>
        public event EventHandler<ListBoxItemTapEventArgs> ItemTap;

        /// <summary>
        /// Occurs when the <see cref="SelectedItem"/> property is changed.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Occurs before the <see cref="SelectedItem"/> property is about to change.
        /// </summary>
        public event EventHandler<SelectionChangingEventArgs> SelectionChanging;

        /// <summary>
        /// Gets or sets an instance of the <see cref="Windows.UI.Xaml.Style"/> class that
        /// defines the visual appearance of the pull-to-refresh indicator element.
        /// </summary>
        public Style PullToRefreshIndicatorStyle
        {
            get
            {
                return this.GetValue(PullToRefreshIndicatorStyleProperty) as Style;
            }
            set
            {
                this.SetValue(PullToRefreshIndicatorStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value from the <see cref="HeaderFooterDisplayMode"/> enum
        /// that defines the way the footer is displayed depending on the state of the items source.
        /// </summary>
        public HeaderFooterDisplayMode ListFooterDisplayMode
        {
            get
            {
                return (HeaderFooterDisplayMode)this.GetValue(ListFooterDisplayModeProperty);
            }
            set
            {
                this.SetValue(ListFooterDisplayModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value from the <see cref="ListHeaderDisplayMode"/> enum
        /// that defines the way the header is displayed depending on the state of the items source.
        /// </summary>
        public HeaderFooterDisplayMode ListHeaderDisplayMode
        {
            get
            {
                return (HeaderFooterDisplayMode)this.GetValue(ListHeaderDisplayModeProperty);
            }
            set
            {
                this.SetValue(ListHeaderDisplayModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// the end user will be able to load more items in the listbox
        /// by pulling down the scrollable content when the top end edge is reached.
        /// </summary>
        public bool IsPullToRefreshEnabled
        {
            get
            {
                return this.isPullToRefreshEnabledCache;
            }
            set
            {
                this.SetValue(IsPullToRefreshEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="Style"/> class
        /// that represents the style defining the visual appearance
        /// of the checkboxes shown next to each visual container
        /// when the <see cref="IsCheckModeActive"/> property is set to true.
        /// </summary>
        public Style CheckBoxStyle
        {
            get
            {
                return this.checkBoxStyleCache;
            }
            set
            {
                this.SetValue(CheckBoxStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the mode for displaying the content displayed when there is no
        /// data to display.
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
        /// Gets or sets an instance of the <see cref="DataTemplate"/> class that represents
        /// the template used to visualize the content of the list's header.
        /// </summary>
        public DataTemplate ListHeaderTemplate
        {
            get
            {
                return this.GetValue(ListHeaderTemplateProperty) as DataTemplate;
            }
            set
            {
                this.SetValue(ListHeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content of the list's header.
        /// </summary>
        public object ListHeaderContent
        {
            get
            {
                return this.GetValue(ListHeaderContentProperty);
            }
            set
            {
                this.SetValue(ListHeaderContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="DataTemplate"/> class that represents
        /// the template used to visualize the content of the list's footer.
        /// </summary>
        public DataTemplate ListFooterTemplate
        {
            get
            {
                return this.GetValue(ListFooterTemplateProperty) as DataTemplate;
            }
            set
            {
                this.SetValue(ListFooterTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content of the list's footer.
        /// </summary>
        public object ListFooterContent
        {
            get
            {
                return this.GetValue(ListFooterContentProperty);
            }
            set
            {
                this.SetValue(ListFooterContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content displayed in the visual container
        /// that is used to request more data when the control is in 
        /// <see cref="Telerik.Core.Data.BatchLoadingMode.Explicit"/> data virtualization mode.
        /// </summary>
        public DataTemplate IncrementalLoadingItemTemplate
        {
            get
            {
                return this.GetValue(IncrementalLoadingItemTemplateProperty) as DataTemplate;
            }
            set
            {
                this.SetValue(IncrementalLoadingItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content displayed in the visual container
        /// that is used to request more data when the control is in 
        /// <see cref="Telerik.Core.Data.BatchLoadingMode.Explicit"/> data virtualization mode.
        /// </summary>
        public object IncrementalLoadingItemContent
        {
            get
            {
                return this.GetValue(IncrementalLoadingItemContentProperty);
            }
            set
            {
                this.SetValue(IncrementalLoadingItemContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an object used to define the content of an item in loading state. This content
        /// is also applied to the visual container that is shown at the end of the list when the
        /// control is in <see cref="Telerik.Core.Data.BatchLoadingMode.Auto"/> data virtualization mode.
        /// </summary>
        public object ItemLoadingContent
        {
            get
            {
                return this.GetValue(ItemLoadingContentProperty);
            }
            set
            {
                this.SetValue(ItemLoadingContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="DataTemplate"/> class that represents
        /// the template used to define the visual appearance of an item in loading state. This template
        /// is also applied to the visual container that is shown at the end of the list when the
        /// control is in <see cref="Telerik.Core.Data.BatchLoadingMode.Auto"/> data virtualization mode.
        /// </summary>
        public DataTemplate ItemLoadingTemplate
        {
            get
            {
                return this.GetValue(ItemLoadingTemplateProperty) as DataTemplate;
            }
            set
            {
                this.SetValue(ItemLoadingTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content that will be shown when the control is not bound to a data collection or when the collection is empty.
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
        /// Gets or sets an instance of the <see cref="DataTemplate"/> class that defines how the content
        /// defined in the <see cref="EmptyContent"/> property will be shown.
        /// </summary>
        public DataTemplate EmptyContentTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(EmptyContentTemplateProperty);
            }
            set
            {
                this.SetValue(EmptyContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value defining whether the <see cref="SelectedItem"/> property
        /// is synchronized with the current item in the provided <see cref="DataControlBase.ItemsSource"/> in case
        /// the <see cref="DataControlBase.ItemsSource"/> supports currency.
        /// </summary>
        /// <value>The is synchronized with current item.</value>
        public bool? IsSynchronizedWithCurrentItem
        {
            get
            {
                return this.GetValue(IsSynchronizedWithCurrentItemProperty) as bool?;
            }
            set
            {
                this.SetValue(IsSynchronizedWithCurrentItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the currently selected item.
        /// </summary>
        public object SelectedItem
        {
            get
            {
                return this.GetValue(SelectedItemProperty);
            }
            set
            {
                this.SetValue(SelectedItemProperty, value);
            }
        }

        /// <summary>
        /// Gets the value of the SelectedItem, obtained by using SelectedValuePath. 
        /// </summary>
        /// <value>The selected value.</value>
        public object SelectedValue
        {
            get
            {
                return this.GetValue(SelectedValueProperty);
            }
        }

        /// <summary>
        /// Gets or sets the path that is used to get the SelectedValue from the SelectedItem. 
        /// </summary>
        /// <value>The selected value path.</value>
        public string SelectedValuePath
        {
            get
            {
                return this.selectedValuePathCache;
            }
            set
            {
                this.selectedValuePathCache = value;
                this.SetValue(SelectedValuePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value from the <see cref="IncrementalLoadingMode"/> enum which
        /// defines whether and how data items will be virtualized.
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
        /// Gets or sets the scroll offset threshold that triggers refresh request.
        /// </summary>
        public double PullToRefreshScrollOffsetThreshold
        {
            get
            {
                return this.maxScrollingOffset;
            }
            set
            {
                if (this.maxScrollingOffset != value)
                {
                    this.maxScrollingOffset = value;
                }
            }
        }

        internal override RadVirtualizingDataControlItem LastRealizedDataItem
        {
            get
            {
                if (this.lastItemCache == null)
                {
                    return null;
                }

                if (this.lastItemCache.associatedDataItem == RadDataBoundListBox.IncrementalLoadingIndicator && this.lastItemCache.previous != null)
                {
                    if (this.lastItemCache.previous.associatedDataItem == RadDataBoundListBox.ListFooterIndicator)
                    {
                        return this.lastItemCache.previous.previous;
                    }

                    return this.lastItemCache.previous;
                }

                return this.lastItemCache;
            }
        }

        internal override RadVirtualizingDataControlItem FirstRealizedDataItem
        {
            get
            {
                if (this.firstItemCache == null)
                {
                    return null;
                }

                if (this.firstItemCache.associatedDataItem == RadDataBoundListBox.ListHeaderIndicator)
                {
                    return this.firstItemCache.next;
                }

                return this.firstItemCache;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this control is properly templated.
        /// </summary>
        /// <value>
        /// <c>True</c> if this instance is properly templated; otherwise, <c>false</c>.
        /// </value>
        protected internal override bool IsProperlyTemplated
        {
            get
            {
                return base.IsProperlyTemplated && this.pullToRefreshIndicator != null && this.itemReorderPopup != null;
            }
        }

        /// <summary>
        /// Gets an <see cref="IDataSourceItem"/> implementation which holds the 
        /// given data item from the original source used to populate this
        /// <see cref="RadDataBoundListBox"/> instance.
        /// </summary>
        public IDataSourceItem GetDataSourceItemForDataItem(object dataItem)
        {
            return this.ListSource.FindItem(dataItem);
        }

        /// <summary>
        /// Brings the given data item into the viewport.
        /// </summary>
        /// <param name="item">The item to show.</param>
        public override void BringIntoView(object item)
        {
            base.BringIntoView(item);
            this.CheckFireDataRequested();
        }
        internal override int GetItemCount()
        {
            int count = base.GetItemCount();
            double originalCount = count;

            if (this.hasHeader)
            {
                if (this.ListHeaderDisplayMode == HeaderFooterDisplayMode.AlwaysVisible ||
                    (this.ListHeaderDisplayMode == HeaderFooterDisplayMode.WithDataItems && originalCount > 0))
                {
                    count++;
                }
            }

            if (this.hasFooter)
            {
                if (this.ListFooterDisplayMode == HeaderFooterDisplayMode.AlwaysVisible ||
                    (this.ListFooterDisplayMode == HeaderFooterDisplayMode.WithDataItems && originalCount > 0))
                {
                    count++;
                }
            }

            if (this.IncrementalLoadingMode == BatchLoadingMode.Explicit || this.ItemsSource is ISupportIncrementalLoading)
            {
                count++;
            }

            return count;
        }

        /// <summary>
        /// Gets the realized containers change location.
        /// </summary>
        /// <param name="changeIndex">Index of the change.</param>
        /// <param name="firstItemIndex">First index of the item.</param>
        internal override int GetItemRealizedIndexFromListSourceIndex(int changeIndex, int firstItemIndex)
        {
            int location = changeIndex - firstItemIndex;

            if (this.headerContainerCache != null)
            {
                location++;
            }

            return location;
        }

        internal override int GetLastItemCacheIndex()
        {
            if (this.lastItemCache == null)
            {
                return base.GetLastItemCacheIndex();
            }

            if (this.lastItemCache.associatedDataItem == ListFooterIndicator &&
                this.lastItemCache.previous != null)
            {
                if (this.lastItemCache.previous.associatedDataItem == IncrementalLoadingIndicator)
                {
                    if (this.lastItemCache.previous.previous != null)
                    {
                        return this.lastItemCache.previous.previous.associatedDataItem.Index + 2;
                    }
                    return 0;
                }

                return this.lastItemCache.previous.associatedDataItem.Index + 1;
            }

            if (this.lastItemCache.associatedDataItem == IncrementalLoadingIndicator)
            {
                if (this.lastItemCache.previous != null)
                {
                    return this.lastItemCache.previous.associatedDataItem.Index + 1;
                }
                return 0;
            }

            return base.GetLastItemCacheIndex();
        }

        internal override int GetFirstItemCacheIndex()
        {
            if (this.firstItemCache == null)
            {
                return base.GetFirstItemCacheIndex();
            }

            if (this.firstItemCache.associatedDataItem == ListHeaderIndicator &&
                this.firstItemCache.next != null)
            {
                return this.firstItemCache.next.associatedDataItem.Index;
            }

            return base.GetFirstItemCacheIndex();
        }

        internal override bool IsLastItemLastInListSource()
        {
            if (this.footerContainerCache != null)
            {
                return true;
            }

            if (this.dataRequestContainerCache != null)
            {
                return !this.hasFooter;
            }

            if (this.lastItemCache.associatedDataItem == ListHeaderIndicator)
            {
                return true;
            }

            return base.IsLastItemLastInListSource();
        }

        internal override bool IsFirstItemFirstInListSource()
        {
            if (this.firstItemCache.associatedDataItem == ListHeaderIndicator ||
                this.firstItemCache.associatedDataItem == ListFooterIndicator)
            {
                return true;
            }

            return base.IsFirstItemFirstInListSource();
        }

        internal override IDataSourceItem GetFirstItem()
        {
            if (this.hasHeader)
            {
                if ((this.ListHeaderDisplayMode == HeaderFooterDisplayMode.WithDataItems &&
                     this.listSource.Count > 0) ||
                    this.ListHeaderDisplayMode == HeaderFooterDisplayMode.AlwaysVisible)
                {
                    return ListHeaderIndicator;
                }
            }

            IDataSourceItem firstDataItem = base.GetFirstItem();

            if (firstDataItem == null && this.hasFooter)
            {
                if (this.ListFooterDisplayMode == HeaderFooterDisplayMode.AlwaysVisible)
                {
                    return ListFooterIndicator;
                }
            }

            if (firstDataItem == null && this.IncrementalLoadingMode == BatchLoadingMode.Explicit)
            {
                return IncrementalLoadingIndicator;
            }

            return firstDataItem;
        }

        internal override IDataSourceItem GetItemBefore(IDataSourceItem item)
        {
            if (item.Previous != null)
            {
                return item.Previous;
            }

            if (this.hasFooter)
            {
                if (item == ListFooterIndicator)
                {
                    IDataSourceItem lastDataItem = this.ListSource.GetLastItem();

                    if (lastDataItem != null)
                    {
                        if (this.ItemsSource is ISupportIncrementalLoading)
                        {
                            return IncrementalLoadingIndicator;
                        }

                        return lastDataItem;
                    }
                }
            }

            if (item == IncrementalLoadingIndicator)
            {
                return this.ListSource.GetLastItem();
            }

            if (this.hasHeader)
            {
                IDataSourceItem headerIndicator = this.CheckGetHeaderIndicator(item);

                if (headerIndicator != null)
                {
                    return headerIndicator;
                }
            }

            return null;
        }

        internal override IDataSourceItem GetItemAfter(IDataSourceItem item)
        {
            if (item.Next != null)
            {
                return item.Next;
            }

            if (item == ListHeaderIndicator)
            {
                IDataSourceItem firstDataItem = this.ListSource.GetFirstItem();

                if (firstDataItem != null)
                {
                    return firstDataItem;
                }
            }

            IDataSourceItem result = this.CheckGetIncrementalLoadingIndicator(item);

            if (result == null && this.hasFooter)
            {
                result = this.CheckGetFooterIndicator(item);
            }

            return result;
        }

        internal override void OnScrollOffsetChanged(bool balanceImmediately)
        {
            base.OnScrollOffsetChanged(balanceImmediately);

            if (this.lastItemCache == null)
            {
                return;
            }

            this.CheckFireDataRequested();
        }

        internal override bool CanPlayAnimationForItem(RadVirtualizingDataControlItem item, bool adding)
        {
            if (item == null)
            {
                return false;
            }

            if (item.associatedDataItem == RadDataBoundListBox.ListHeaderIndicator ||
                item.associatedDataItem == RadDataBoundListBox.ListFooterIndicator ||
                item.associatedDataItem == RadDataBoundListBox.IncrementalLoadingIndicator)
            {
                return false;
            }

            return base.CanPlayAnimationForItem(item, adding);
        }

        /// <summary>
        /// Called when a manipulation operation has been started on an item.
        /// </summary>
        protected internal virtual void OnItemManipulationStarted(RadDataBoundListBoxItem item, UIElement container, Point hitPoint)
        {
            if (!item.isItemCheckable)
            {
                return;
            }

            if (this.isCheckModeEnabled && this.IsCheckModeArea(item, container, hitPoint) && !this.isCheckModeActive)
            {
                this.ShowCheckboxesPressIndicator(item);
            }
        }
        
        /// <summary>
        /// Handles a click from a child visual item.
        /// </summary>
        protected async internal virtual void OnItemTap(RadDataBoundListBoxItem item, UIElement container, UIElement originalSource, Point hitPoint)
        {
            if (item.checkBoxVisible && item.isItemCheckable)
            {
                this.HandleItemCheckStateChange(item);
                item.UpdateCheckedState();
                return;
            }

            if (item.associatedDataItem == IncrementalLoadingIndicator)
            {
                if (this.incrementalLoadingModeCache == BatchLoadingMode.Explicit)
                {
                    this.OnDataRequested();
                }

                return;
            }

            if (this.hasHeader && item.associatedDataItem == ListHeaderIndicator)
            {
                return;
            }

            if (this.hasFooter && item.associatedDataItem == ListFooterIndicator)
            {
                return;
            }

            if (this.isCheckModeEnabled)
            {
                bool checkModeRequest = this.IsCheckModeArea(item, container, hitPoint);
                if (checkModeRequest)
                {
                    await this.Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal,
                        () =>
                        {
                            bool cancelled = this.FireCheckModeChanging(item.associatedDataItem.Value);
                            if (!cancelled)
                            {
                                this.isInternalCheckModeChange = true;
                                this.IsCheckModeActive = !this.isCheckModeActive;
                                this.isInternalCheckModeChange = false;
                                this.ToggleCheckBoxesVisibility();
                                this.FireCheckModeChanged(item.associatedDataItem.Value);
                            }
                            else
                            {
                                this.HideCheckBoxesPressIndicator();
                            }
                        });

                    return;
                }
                else
                {
                    this.HideCheckBoxesPressIndicator();
                }
            }

            this.PerformSelection(this.SelectedItem, item.associatedDataItem.Value, true);

            EventHandler<ListBoxItemTapEventArgs> eh = this.ItemTap;
            if (eh != null)
            {
                eh(this, new ListBoxItemTapEventArgs(item, container, originalSource, hitPoint));
            }

            AutomationPeer itemPeer = FrameworkElementAutomationPeer.FromElement(item);
            if (itemPeer != null)
            {
                itemPeer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
            }
        }

        /// <summary>
        /// Called when SelectedValuePath property value is changed.
        /// </summary>
        protected internal virtual void OnSelectedValuePathChanged()
        {
            this.UpdateSelectedValue();
        }

        /// <summary>
        /// Updates the selected state of the visual container that holds the given item.
        /// </summary>
        /// <param name="itemToUpdate">The item which container should be updated.</param>
        /// <param name="isSelected">True if the container should be selected, otherwise false.</param>
        protected internal virtual void SetItemContainerSelectedState(object itemToUpdate, bool isSelected)
        {
            RadDataBoundListBoxItem visualItem = this.GetContainerForItem(itemToUpdate) as RadDataBoundListBoxItem;

            if (visualItem != null)
            {
                visualItem.IsSelected = isSelected;
            }
        }

        /// <summary>
        /// Called when the current item of the ListSource has changed.
        /// </summary>
        protected internal override void OnListSourceCurrentItemChanged()
        {
            base.OnListSourceCurrentItemChanged();

            if (this.isInternalSelectionChange)
            {
                return;
            }

            bool? isSynchronized = this.IsSynchronizedWithCurrentItem;

            if (isSynchronized == null || isSynchronized.Value)
            {
                if (this.ListSource.CurrentItem != null)
                {
                    this.SelectedItem = this.ListSource.CurrentItem.Value;
                }
                else
                {
                    this.SelectedItem = null;
                }
            }
        }

        /// <summary>
        /// Called when the control has requested more data.
        /// </summary>
        protected virtual void OnDataRequested()
        {
            if (this.listSource != null && this.listSource.BatchDataProvider != null)
            {
                this.listSource.BatchDataProvider.StatusChanged -= this.BatchDataProvider_StatusChanged;
                this.listSource.BatchDataProvider.StatusChanged += this.BatchDataProvider_StatusChanged;
                this.listSource.RequestData(this.GetLastItemCacheIndex() + 1);
            }
        }

        /// <summary>
        /// Updates the IsSelected property of the specified visual item.
        /// </summary>
        /// <param name="visualItem">The visual item.</param>
        /// <param name="select">True to select the item, false otherwise.</param>
        protected virtual void SelectItem(RadDataBoundListBoxItem visualItem, bool select)
        {
            visualItem.IsSelected = select;
        }

        /// <summary>
        /// Raises the <see cref="SelectionChanging"/> event.
        /// </summary>
        protected virtual void OnSelectionChanging(SelectionChangingEventArgs args)
        {
            EventHandler<SelectionChangingEventArgs> eh = this.SelectionChanging;
            if (eh != null)
            {
                eh(this, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="SelectionChanged"/> event.
        /// </summary>
        protected virtual void OnSelectionChanged(SelectionChangedEventArgs args)
        {
            SelectionChangedEventHandler eh = this.SelectionChanged;
            if (eh != null)
            {
                eh(this, args);
            }
        }

        /// <summary>
        /// Called when the <see cref="RadDataBoundListBox.IsSynchronizedWithCurrentItem"/> property value has changed.
        /// </summary>
        protected virtual void OnIsSynchronizedWithCurrentItemChanged(DependencyPropertyChangedEventArgs args)
        {
            bool? newValue = (bool?)args.NewValue;
            if (newValue.HasValue && !newValue.Value)
            {
                this.ListSource.CurrencyMode = CurrencyManagementMode.None;
            }
            else
            {
                this.ListSource.CurrencyMode = CurrencyManagementMode.LocalAndExternal;
            }
        }

        /// <summary>
        /// Gets an <see cref="IDataSourceItem"/> implementation which holds the 
        /// given data item from the original source used to populate this
        /// <see cref="RadDataBoundListBox"/> instance.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.UpdateVisualState(true);
            this.pullToRefreshIndicator = this.GetTemplateChild("PART_PullToRefreshIndicator") as PullToRefreshIndicatorControl;
            this.checkBoxesPressIndicator = this.GetTemplateChild("PART_CheckBoxesPressIndicator") as Rectangle;
            this.emptyContentPresenter = this.GetTemplateChild("PART_EmptyContentPresenter") as ContentPresenter;

            this.itemReorderControl = this.GetTemplateChild("PART_ItemReorderControl") as ItemReorderControl;
            this.itemReorderPopup = this.GetTemplateChild("PART_ItemReorderPopup") as Popup;

            if (this.itemReorderControl != null && this.itemReorderPopup != null)
            {
                this.itemReorderPopup.Opened += this.OnItemReorderPopup_OpenedChanged;
                this.itemReorderPopup.Closed += this.OnItemReorderPopup_OpenedChanged;
                this.itemReorderControl.owner = this;
            }

            if (this.ReadLocalValue(EmptyContentProperty) == DependencyProperty.UnsetValue)
            {
                if (this.EmptyContent == null)
                {
                    this.EmptyContent = DataControlsLocalizationManager.Instance.GetString("DataBoundListBoxEmptyContentString");
                }
            }

            if (this.checkBoxesPressIndicator != null)
            {
                this.indicatorTranslate = new TranslateTransform();
                this.checkBoxesPressIndicator.RenderTransform = this.indicatorTranslate;
                Storyboard.SetTarget(this.checkBoxesIndicatorAnimation, this.checkBoxesPressIndicator);
                Storyboard.SetTargetProperty(this.checkBoxesIndicatorAnimation, "(Rectangle.Opacity)");
            }
        }

        /// <inheritdoc/>
        protected override void OnTemplateApplied()
        {
            CompositionTarget.Rendering += this.CompositionTarget_Rendering;
            this.isCompositionTargetRenderListening = true;
            this.manipulationContainer.ViewChanging += this.manipulationContainer_ViewChanging;
            this.manipulationContainer.ViewChanged += this.manipulationContainer_ViewChanged;
            this.scrollableContent.PointerMoved += this.ScrollViewerPresenter_PointerMoved;
            this.scrollableContent.PointerPressed += this.ScrollableContent_PointerPressed;
            this.scrollableContent.PointerReleased += this.ScrollableContent_PointerReleased;
        }

        /// <summary>
        /// Occurs when a System.Windows.FrameworkElement has been constructed and added to the object tree.
        /// </summary>
        protected override void LoadCore()
        {
            base.LoadCore();

            this.CheckFireDataRequested();

            if (this.isCheckModeActive)
            {
                this.HookRootVisualBackKeyPress();
            }

            if (this.isPullToRefreshEnabledCache && this.listSource.Count == 0 && this.ShowPullToRefreshWhenNoData)
            {
                this.ShowPullToRefreshIndicator();
            }
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of Windows Phone layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size result = base.ArrangeOverride(finalSize);

            // This code fixes an issue reported in ticket 734637.
            // The problem happens when the control unloads and the end of a removed
            // animation triggers the positioning of the PTR when its actual size is empty.
            // This causes wrong positioning. We need to therefore repositiong it correctly here
            // upon intiial layout before the Loaded event comes again.
            if (this.isPullToRefreshEnabledCache && !this.IsLoaded)
            {
                if (this.IsProperlyTemplated && (this.realizedItems.Count > 0 || this.ShowPullToRefreshWhenNoData))
                {
                    this.ShowPullToRefreshIndicator();
                }
            }

            if (this.emptyContentPresenter != null)
            {
                this.emptyContentPresenter.Width = finalSize.Width;
                this.emptyContentPresenter.Height = finalSize.Height;
            }

            return result;
        }

        /// <summary>
        /// Occurs when this object is no longer connected to the main object tree.
        /// </summary>
        protected override void UnloadCore()
        {
            base.UnloadCore();

            if (this.refreshRequested)
            {
                this.StopPullToRefreshLoading(false);
            }

            this.UnhookRootVisualBackKeyPress();
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected override string ComposeVisualStateName()
        {
            EmptyContentDisplayMode mode = this.EmptyContentDisplayMode;

            if (this.IncrementalLoadingMode != BatchLoadingMode.Explicit)
            {
                if (mode == EmptyContentDisplayMode.DataSourceEmpty && this.ListSource.SourceCollection != null && this.ListSource.Count == 0 && this.scheduledRemoveAnimations.Count == 0 && this.currentLoadingStatus != BatchLoadingStatus.ItemsRequested)
                {
                    return "NoData";
                }

                if (mode == EmptyContentDisplayMode.Always && this.GetItemCount() == 0 && this.scheduledRemoveAnimations.Count == 0 && this.currentLoadingStatus != BatchLoadingStatus.ItemsRequested)
                {
                    return "NoData";
                }

                if (mode == EmptyContentDisplayMode.DataSourceNull && this.itemsSourceCache == null && this.scheduledRemoveAnimations.Count == 0 && this.currentLoadingStatus != BatchLoadingStatus.ItemsRequested)
                {
                    return "NoData";
                }
            }

            return "Normal";
        }

        /// <summary>
        /// Occurs when the <see cref="DataControlBase.ItemsSource" /> property has changed.
        /// </summary>
        protected override void OnItemsSourceChanged(IEnumerable oldSource)
        {
            var newSource = this.ItemsSource as ISupportIncrementalLoading;
            if (newSource != null)
            {
                this.ListSource.Suspend();
            }

            this.checkedItems.ClearSilently(false);
            this.ListSource.SourceCollection = this.itemsSourceCache = this.ItemsSource;
            this.ListSource.ItemPropertyChanged += this.ListSource_ItemPropertyChanged;

            if (newSource != null)
            {
                this.ListSource.Resume(true);
                return;
            }

            if (this.ItemsSource == null)
            {
                this.ClearReycledItems();
            }

            if (this.itemsSourceCache != null)
            {
                this.CheckFireDataRequested();
            }
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The element used to display the specified item.</param>
        /// <param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(RadVirtualizingDataControlItem element, IDataSourceItem item)
        {
            if (item == IncrementalLoadingIndicator)
            {
                this.PrepareIncrementalLoadingIndicatorContainer(element, item);
                return;
            }

            if (item == ListHeaderIndicator)
            {
                this.PrepareListHeaderIndicatorContainer(element, item);
                return;
            }

            if (item == ListFooterIndicator)
            {
                this.PrepareListFooterIndicatorContainer(element, item);
                return;
            }

            base.PrepareContainerForItemOverride(element, item);

            if (item.Value == RadListSource.UnsetObject)
            {
                element.Content = this.itemLoadingContentCache;
                element.ContentTemplate = this.itemLoadingTemplateCache;
            }

            RadDataBoundListBoxItem visualItem = element as RadDataBoundListBoxItem;
            if (item.Value != null && object.Equals(item.Value, this.selectedItemCache))
            {
                this.SelectItem(visualItem, true);
            }
        }

        /// <summary>
        /// Called when an animation used to animate a visual container
        /// out of the viewport has ended. Fires the <see cref="RadVirtualizingDataControl.ItemAnimationEnded" /> event.
        /// </summary>
        /// <param name="args">An instance of the <see cref="ItemAnimationEndedEventArgs" /> class
        /// that holds information about the event.</param>
        protected override void OnItemAnimationEnded(ItemAnimationEndedEventArgs args)
        {
            if (args.RemainingAnimationsCount == 0 && args.Animation == this.ItemRemovedAnimation)
            {
                this.UpdateVisualState(true);
            }

            base.OnItemAnimationEnded(args);
        }

        /// <summary>
        /// Called when the value of the <see cref="P:System.Windows.Controls.ItemsControl.Items"/>
        /// property changes.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs"/>
        /// that contains the event data.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.IsTemplateApplied && this.IsPullToRefreshEnabled)
            {
                this.StopPullToRefreshLoading(false, true);
                if (this.listSource.Count > 0 && this.realizedItems.Count > 0)
                {
                    if (!(this.scheduledRemoveAnimations.Count != 0 && e.OldStartingIndex == 0))
                    {
                        this.PositionPullToRefreshIndicator();
                    }
                }
            }

            base.OnItemsChanged(e);

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.PerformSelection(this.SelectedItem, null, true);
                this.CheckFireDataRequested();
            }
            else
            {
                this.SynchCheckedItemsOnItemsChanged(e);
            }

            this.UpdateVisualState(true);
        }

        /// <summary>
        /// Undoes the effects of the <see cref="M:System.Windows.Controls.ItemsControl.PrepareContainerForItemOverride(System.Windows.DependencyObject,System.Object)"/> method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item.</param>
        protected override void ClearContainerForItemOverride(RadVirtualizingDataControlItem element, IDataSourceItem item)
        {
            base.ClearContainerForItemOverride(element, item);

            if (item == ListHeaderIndicator)
            {
                this.headerContainerCache.Content = null;
                this.headerContainerCache = null;
            }
            else if (item == ListFooterIndicator)
            {
                this.footerContainerCache.Content = null;
                this.footerContainerCache = null;
            }
            else if (item == IncrementalLoadingIndicator)
            {
                this.dataRequestContainerCache.Content = null;
                this.dataRequestContainerCache = null;
            }

            RadDataBoundListBoxItem typedElement = element as RadDataBoundListBoxItem;
            this.SelectItem(typedElement, false);
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override RadVirtualizingDataControlItem GetContainerForItemOverride()
        {
            return new RadDataBoundListBoxItem();
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadDataBoundListBoxAutomationPeer(this);
        }

        private static void OnSelectedValuePathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;

            typedSender.OnSelectedValuePathChanged();
        }

        private static void OnIsSynchronizedWithCurrentItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;
            typedSender.OnIsSynchronizedWithCurrentItemChanged(args);
        }

        private static void OnItemLoadingTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;
            typedSender.itemLoadingTemplateCache = args.NewValue as DataTemplate;
        }

        private static void OnItemLoadingContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;
            typedSender.itemLoadingContentCache = args.NewValue;
        }

        private static void OnListHeaderTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;
            typedSender.OnListHeaderTemplateChanged(args);
        }

        private static void OnListHeaderContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;
            typedSender.OnListHeaderContentChanged(args);
        }

        private static void OnListFooterTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;
            typedSender.OnListFooterTemplateChanged(args);
        }

        private static void OnListFooterContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;
            typedSender.OnListFooterContentChanged(args);
        }

        private static void OnIncrementalLoadingItemTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;
            typedSender.OnGetMoreDataItemTemplateChanged(args);
        }

        private static void OnIncrementalLoadingItemContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;
            typedSender.OnGetMoreDataItemContentChanged(args);
        }

        private static void OnIncrementalLoadingModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;
            typedSender.OnIncrementalLoadingModeChanged(args);
        }

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;

            if (typedSender.isInternalSelectionChange)
            {
                return;
            }

            typedSender.PerformSelection(e.OldValue, e.NewValue, false);
        }

        private static void OnCheckBoxStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;
            typedSender.OnCheckBoxStyleChanged(args);
        }

        private static void OnPullToRefreshContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
        }

        private static void OnPullToRefreshContentTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
        }

        private static void OnBusyContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnBusyContentTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnEmptyContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;
            typedSender.OnEmptyContentChanged(e);
        }

        private static void OnEmptyContentTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs a)
        {
        }

        private static void OnEmptyContentDisplayModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs a)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;

            typedSender.UpdateVisualState(true);
        }

        private static void OnListFooterDisplayModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs a)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;
            typedSender.OnListFooterDisplayModeChanged(a);
        }

        private static void OnListHeaderDisplayModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs a)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;
            typedSender.OnListHeaderDisplayModeChanged(a);
        }

        private void manipulationContainer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            this.OnScrollOffsetChanged(true);
        }

        private void manipulationContainer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            if ((e.NextView.VerticalOffset > 0 && this.virtualizationStrategy.LayoutOrientation == Orientation.Vertical ||
                 e.NextView.HorizontalOffset > 0 && this.virtualizationStrategy.LayoutOrientation == Orientation.Horizontal) &&
                this.isCompositionTargetRenderListening)
            {
                CompositionTarget.Rendering -= this.CompositionTarget_Rendering;
                this.isCompositionTargetRenderListening = false;
            }
            else if ((e.NextView.VerticalOffset == 0 && this.virtualizationStrategy.LayoutOrientation == Orientation.Vertical ||
                      e.NextView.HorizontalOffset == 0 && this.virtualizationStrategy.LayoutOrientation == Orientation.Horizontal) && !this.isCompositionTargetRenderListening)
            {
                CompositionTarget.Rendering += this.CompositionTarget_Rendering;
                this.isCompositionTargetRenderListening = true;
            }
        }

        private void UpdateItemReorderPopupOffset(double height, double width)
        {
            if (height < this.itemReorderControl.Height)
            {
                this.itemReorderPopup.VerticalOffset = 0;
            }
            else
            {
                this.itemReorderPopup.VerticalOffset = height - this.itemReorderControl.Height;
            }
            if (width < this.itemReorderControl.Width)
            {
                this.itemReorderPopup.HorizontalOffset = 0;
            }
            else
            {
                this.itemReorderPopup.HorizontalOffset = width - this.itemReorderControl.Width;
            }
        }

        private void BatchDataProvider_StatusChanged(object sender, BatchLoadingEventArgs e)
        {
            if (this.lastItemCache != null && this.lastItemCache.AssociatedDataItem is IncrementalLoadingIndicatorItem)
            {
                this.currentLoadingStatus = e.Status;

                switch (e.Status)
                {
                    case BatchLoadingStatus.ItemsRequested:
                        this.lastItemCache.Content = this.itemLoadingContentCache;
                        this.lastItemCache.ContentTemplate = this.itemLoadingTemplateCache;
                        break;
                    case BatchLoadingStatus.ItemsLoaded:
                        if (this.IncrementalLoadingMode == BatchLoadingMode.Explicit)
                        {
                            this.lastItemCache.Content = this.incrementalLoadingItemContentCache;
                            this.lastItemCache.ContentTemplate = this.incrementalLoadingItemTemplateCache;
                        }
                        else
                        {
                            this.lastItemCache.Content = null;
                            this.lastItemCache.ContentTemplate = null;
                        }

                        this.listSource.BatchDataProvider.StatusChanged -= this.BatchDataProvider_StatusChanged;

                        if (this.IncrementalLoadingMode == BatchLoadingMode.Auto)
                        {
                            this.CheckFireDataRequested();
                        }

                        break;
                    case BatchLoadingStatus.ItemsLoadFailed:
                        if (this.IncrementalLoadingMode == BatchLoadingMode.Explicit)
                        {
                            this.lastItemCache.Content = this.incrementalLoadingItemContentCache;
                            this.lastItemCache.ContentTemplate = this.incrementalLoadingItemTemplateCache;
                        }
                        else
                        {
                            this.lastItemCache.Content = null;
                            this.lastItemCache.ContentTemplate = null;
                        }

                        this.listSource.BatchDataProvider.StatusChanged -= this.BatchDataProvider_StatusChanged;
                        break;
                }
            }
        }

        private void ListSource_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            this.ResetOnTemplateChange(false);
        }

        private void RadDataBoundListBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateItemReorderPopupOffset(e.NewSize.Height, e.NewSize.Width);
        }

        private void OnListFooterDisplayModeChanged(DependencyPropertyChangedEventArgs args)
        {
            this.RecycleAllItems();
            this.BalanceVisualSpace();
        }

        private void OnListHeaderDisplayModeChanged(DependencyPropertyChangedEventArgs args)
        {
            this.RecycleAllItems();
            this.BalanceVisualSpace();
        }

        private void OnEmptyContentChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is UIElement)
            {
                this.EmptyContentTemplate = null;
            }
        }

        private void OnCheckBoxStyleChanged(DependencyPropertyChangedEventArgs a)
        {
            this.checkBoxStyleCache = a.NewValue as Style;

            RadVirtualizingDataControlItem[] realizedContainers = this.RealizedContainers;

            foreach (RadDataBoundListBoxItem container in realizedContainers)
            {
                if (this.checkBoxStyleCache != null)
                {
                    container.checkBox.Style = this.checkBoxStyleCache;
                }
                else
                {
                    container.checkBox.ClearValue(CheckBox.StyleProperty);
                }
            }
        }

        private void OnGetMoreDataItemTemplateChanged(DependencyPropertyChangedEventArgs args)
        {
            this.incrementalLoadingItemTemplateCache = args.NewValue as DataTemplate;

            if (this.dataRequestContainerCache != null)
            {
                this.dataRequestContainerCache.ContentTemplate = this.incrementalLoadingItemTemplateCache;
            }
        }

        private void OnGetMoreDataItemContentChanged(DependencyPropertyChangedEventArgs args)
        {
            this.incrementalLoadingItemContentCache = args.NewValue;

            if (this.dataRequestContainerCache != null)
            {
                this.dataRequestContainerCache.Content = this.incrementalLoadingItemContentCache;
            }
        }

        private void CheckFireDataRequested()
        {
            if (this.itemsSourceCache == null)
            {
                return;
            }

            if (this.incrementalLoadingModeCache == BatchLoadingMode.Auto)
            {
                if (this.dataRequestContainerCache != null)
                {
                    double itemRelativeOffset = this.virtualizationStrategy.GetItemRelativeOffset(this.dataRequestContainerCache);

                    var availableLength = this.virtualizationStrategy.Orientation == Orientation.Horizontal ? this.availableWidth : this.availableHeight;

                    if (itemRelativeOffset - this.virtualizationStrategy.ScrollOffset - availableLength < DataRequestEventThreshold)
                    {
                        this.OnDataRequested();
                    }
                }
                else if (this.ListSource.Count == 0)
                {
                    this.OnDataRequested();
                }
            }
        }

        private void OnListHeaderContentChanged(DependencyPropertyChangedEventArgs args)
        {
            this.listHeaderContentCache = args.NewValue;
            this.hasHeader = this.listHeaderTemplateCache != null || this.listHeaderContentCache != null;

            if (!this.hasHeader && this.headerContainerCache != null)
            {
                this.RecycleHeader();
            }

            if (this.headerContainerCache != null)
            {
                this.headerContainerCache.Content = this.listHeaderContentCache;
            }
            else
            {
                this.BalanceVisualSpace();
            }

            this.UpdateVisualState(true);
        }

        private void OnListHeaderTemplateChanged(DependencyPropertyChangedEventArgs args)
        {
            this.listHeaderTemplateCache = args.NewValue as DataTemplate;
            this.hasHeader = this.listHeaderTemplateCache != null || this.listHeaderContentCache != null;

            if (!this.hasHeader && this.headerContainerCache != null)
            {
                this.RecycleHeader();
            }

            if (this.headerContainerCache != null)
            {
                this.headerContainerCache.ContentTemplate = this.listHeaderTemplateCache;
            }
            else
            {
                this.BalanceVisualSpace();
            }

            this.UpdateVisualState(true);
        }

        private void OnListFooterTemplateChanged(DependencyPropertyChangedEventArgs args)
        {
            this.listFooterTemplateCache = args.NewValue as DataTemplate;
            this.hasFooter = this.listFooterTemplateCache != null || this.listFooterContentCache != null;

            if (!this.hasFooter)
            {
                this.RecycleFooter();
            }

            if (this.footerContainerCache != null)
            {
                this.footerContainerCache.ContentTemplate = this.listFooterTemplateCache;
            }
            else
            {
                this.BalanceVisualSpace();
            }

            this.UpdateVisualState(true);
        }

        private void OnListFooterContentChanged(DependencyPropertyChangedEventArgs args)
        {
            this.listFooterContentCache = args.NewValue;
            this.hasFooter = this.listFooterTemplateCache != null || this.listFooterContentCache != null;

            if (!this.hasFooter && this.footerContainerCache != null)
            {
                this.RecycleFooter();
            }

            if (this.footerContainerCache != null)
            {
                this.footerContainerCache.Content = this.listFooterContentCache;
            }
            else
            {
                this.BalanceVisualSpace();
            }

            this.UpdateVisualState(true);
        }

        private IDataSourceItem CheckGetIncrementalLoadingIndicator(IDataSourceItem previousItem)
        {
            if (!(this.ItemsSource is ISupportIncrementalLoading))
            {
                return null;
            }
            else
            {
                if (previousItem == ListHeaderIndicator ||
                    previousItem == IncrementalLoadingIndicator ||
                    previousItem == ListFooterIndicator)
                {
                    return null;
                }

                return IncrementalLoadingIndicator;
            }
        }

        private IDataSourceItem CheckGetFooterIndicator(IDataSourceItem previousItem)
        {
            if (previousItem == ListFooterIndicator)
            {
                return null;
            }

            if (this.ListFooterDisplayMode == HeaderFooterDisplayMode.WithDataItems &&
                this.listSource.Count == 0)
            {
                return null;
            }

            return ListFooterIndicator;
        }

        private IDataSourceItem CheckGetHeaderIndicator(IDataSourceItem previousItem)
        {
            if (previousItem == ListHeaderIndicator)
            {
                return null;
            }

            if (this.ListHeaderDisplayMode == HeaderFooterDisplayMode.WithDataItems &&
                this.listSource.Count == 0)
            {
                return null;
            }

            return ListHeaderIndicator;
        }

        private void RecycleIncrementalLoadingContainer()
        {
            if (this.dataRequestContainerCache != null)
            {
                this.ClearContainerForItemOverride(this.dataRequestContainerCache, this.dataRequestContainerCache.associatedDataItem);
            }
        }

        private void RecycleHeader()
        {
            if (this.headerContainerCache != null)
            {
                this.ClearContainerForItemOverride(this.headerContainerCache, this.headerContainerCache.associatedDataItem);
            }
        }

        private void RecycleFooter()
        {
            if (this.footerContainerCache != null)
            {
                this.ClearContainerForItemOverride(this.footerContainerCache, this.footerContainerCache.associatedDataItem);
            }
        }

        private void RecycleTailItems()
        {
            this.RecycleHeader();
            this.RecycleFooter();
            this.RecycleIncrementalLoadingContainer();
        }

        private void OnIncrementalLoadingModeChanged(DependencyPropertyChangedEventArgs args)
        {
            this.incrementalLoadingModeCache = (BatchLoadingMode)args.NewValue;
            this.RecycleTailItems();
            this.BalanceVisualSpace();
            this.CheckFireDataRequested();
            this.UpdateVisualState(true);
        }

        private async void PrepareIncrementalLoadingIndicatorContainer(RadVirtualizingDataControlItem element, IDataSourceItem item)
        {
            if (this.incrementalLoadingModeCache == BatchLoadingMode.Explicit)
            {
                element.Content = this.incrementalLoadingItemContentCache;
                element.ContentTemplate = this.incrementalLoadingItemTemplateCache;
            }
            else if (this.incrementalLoadingModeCache == BatchLoadingMode.Auto)
            {
                element.Content = this.itemLoadingContentCache;
                element.ContentTemplate = this.itemLoadingTemplateCache;
            }

            this.dataRequestContainerCache = element;
            element.BindToDataItem(item);
            this.PrepareStyle(element, item);
            await this.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    this.CheckFireDataRequested();
                });
        }

        private void PrepareListFooterIndicatorContainer(RadVirtualizingDataControlItem element, IDataSourceItem item)
        {
            this.footerContainerCache = element;
            element.Content = this.listFooterContentCache;
            element.ContentTemplate = this.listFooterTemplateCache;
            element.BindToDataItem(item);
            this.PrepareStyle(element, item);
        }

        private void PrepareListHeaderIndicatorContainer(RadVirtualizingDataControlItem element, IDataSourceItem item)
        {
            this.headerContainerCache = element;
            element.Content = this.listHeaderContentCache;
            element.ContentTemplate = this.listHeaderTemplateCache;
            element.BindToDataItem(item);
            this.PrepareStyle(element, item);
        }

        private void PerformSelection(object oldItem, object newItem, bool isInternal)
        {
            if (oldItem == newItem)
            {
                return;
            }

            bool cancel;

            if (newItem == RadListSource.UnsetObject ||
                (newItem != null && !isInternal && (this.ListSource.FindItem(newItem) == null)))
            {
                cancel = true;
            }
            else
            {
                cancel = this.FireSelectionChanging(oldItem, newItem);
            }

            if (cancel && isInternal)
            {
                return;
            }

            if (!cancel)
            {
                this.isInternalSelectionChange = true;
                this.SelectedItem = newItem;
                this.SetItemContainerSelectedState(oldItem, false);
                this.SetItemContainerSelectedState(newItem, true);
                this.UpdateSelectedValue();
                this.SynchronizeCurrentItemWithListSource(oldItem, newItem);
                this.FireSelectionChanged(oldItem, newItem);
                this.isInternalSelectionChange = false;
            }
            else
            {
                this.isInternalSelectionChange = true;
                this.SetItemContainerSelectedState(newItem, false);
                this.SetItemContainerSelectedState(oldItem, true);
                this.SynchronizeCurrentItemWithListSource(newItem, oldItem);
                this.SelectedItem = oldItem;
                this.isInternalSelectionChange = false;
            }
        }

        private void SynchronizeCurrentItemWithListSource(object oldItem, object newItem)
        {
            bool? isSynchronized = this.IsSynchronizedWithCurrentItem;

            if (isSynchronized == null || isSynchronized.Value)
            {
                this.ListSource.SetCurrentItem(this.ListSource.FindItem(newItem));
            }
        }

        private bool FireSelectionChanging(object oldItem, object newItem)
        {
            SelectionChangingEventArgs changingArgs = new SelectionChangingEventArgs(
                this.GetSelectionArguments(oldItem),
                this.GetSelectionArguments(newItem));
            this.OnSelectionChanging(changingArgs);
            return changingArgs.Cancel;
        }

        private void FireSelectionChanged(object oldItem, object newItem)
        {
            SelectionChangedEventArgs changedArgs = new SelectionChangedEventArgs(
                this.GetSelectionArguments(oldItem),
                this.GetSelectionArguments(newItem));
            this.OnSelectionChanged(changedArgs);
        }

        private void UpdateSelectedValue()
        {
            this.selectedItemCache = this.SelectedItem;

            if (this.selectedItemCache == null)
            {
                this.SetValue(SelectedValueProperty, null);
                return;
            }

            if (string.IsNullOrEmpty(this.selectedValuePathCache))
            {
                this.SetValue(SelectedValueProperty, this.selectedItemCache);
                return;
            }

            this.SetValue(SelectedValueProperty, this.ReflectPropertyValueAndStoreInfoIfNeeded(ref this.selectedValuePropInfo, this.selectedItemCache, this.selectedValuePathCache));
        }

        private IList<object> GetSelectionArguments(object value)
        {
            if (value == null)
            {
                return new object[] { };
            }
            else
            {
                return new object[] { value };
            }
        }

        partial void HookRootVisualBackKeyPress();

        partial void UnhookRootVisualBackKeyPress();
    }
}
