using System;
using System.Collections;
using System.Diagnostics;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives.LoopingList
{
    /// <summary>
    /// Implements a light-weight implementation of a Selector control which is completely virtualized in both UI and Data terms.
    /// </summary>
    public partial class RadLoopingList : RadControl
    {
        /// <summary>
        /// Defines the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(RadLoopingList), new PropertyMetadata(Orientation.Vertical, OnOrientationChanged));

        /// <summary>
        /// Defines the <see cref="IsLoopingEnabled"/> property.
        /// </summary>
        public static readonly DependencyProperty IsLoopingEnabledProperty =
            DependencyProperty.Register(nameof(IsLoopingEnabled), typeof(bool), typeof(RadLoopingList), new PropertyMetadata(true, OnIsLoopingEnabledChanged));

        /// <summary>
        /// Defines <see cref="SnapOffsetCorrection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SnapOffsetCorrectionProperty =
            DependencyProperty.Register(nameof(SnapOffsetCorrection), typeof(double), typeof(RadLoopingList), new PropertyMetadata(0d, OnSnapOffsetCorrectionChanged));

        /// <summary>
        /// Defines the <see cref="CenteredItemSnapPosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenteredItemSnapPositionProperty =
            DependencyProperty.Register(nameof(CenteredItemSnapPosition), typeof(LoopingListItemSnapPosition), typeof(RadLoopingList), new PropertyMetadata(LoopingListItemSnapPosition.Middle, OnCenteredItemSnapPositionChanged));

        /// <summary>
        /// Defines the <see cref="IsCentered"/> property.
        /// </summary>
        public static readonly DependencyProperty IsCenteredProperty =
            DependencyProperty.Register(nameof(IsCentered), typeof(bool), typeof(RadLoopingList), new PropertyMetadata(false, OnIsCenteredChanged));

        /// <summary>
        /// Defines the <see cref="IsExpanded"/> property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(RadLoopingList), new PropertyMetadata(true, OnIsExpandedChanged));

        /// <summary>
        /// Defines the <see cref="SelectedIndex"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(RadLoopingList), new PropertyMetadata(-1, OnSelectedIndexChanged));

        /// <summary>
        /// Defines the <see cref="ItemHeight"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register(nameof(ItemHeight), typeof(double), typeof(RadLoopingList), new PropertyMetadata(138d, OnItemHeightChanged));

        /// <summary>
        /// Defines the <see cref="ItemWidth"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register(nameof(ItemWidth), typeof(double), typeof(RadLoopingList), new PropertyMetadata(138d, OnItemWidthChanged));

        /// <summary>
        /// Defines the <see cref="ItemSpacing"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemSpacingProperty =
            DependencyProperty.Register(nameof(ItemSpacing), typeof(double), typeof(RadLoopingList), new PropertyMetadata(6d, OnItemSpacingChanged));

        /// <summary>
        /// Defines the <see cref="ItemTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(RadLoopingList), new PropertyMetadata(null, OnItemTemplateChanged));

        /// <summary>
        /// Identifies the SpecialDayTemplateSelector dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register(nameof(ItemTemplateSelector), typeof(DataTemplateSelector), typeof(RadLoopingList), new PropertyMetadata(null, OnItemTemplateSelectorChanged));

        /// <summary>
        /// Defines the <see cref="ItemStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemStyleProperty =
            DependencyProperty.Register(nameof(ItemStyle), typeof(Style), typeof(RadLoopingList), new PropertyMetadata(null, OnItemStyleChanged));

        /// <summary>
        /// Defines the <see cref="ItemsSource"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IList), typeof(RadLoopingList), new PropertyMetadata(null, OnItemsSourceChanged));

        internal LoopingPanel itemsPanel;
        private LoopingListSelectionChangeReason selectionChangeReason = LoopingListSelectionChangeReason.User;
        private int selectedIndex;
        private int selectedVisualIndex;
        private bool isExpanded;
        private bool isInitialized;
        private bool allowInertia = true;
        private bool skipTapped; // a flag needed to skip a Tap event straight after a GotFocus event.
        private DataTemplate itemTemplateCache;
        private Style itemStyleCache;
        private DataTemplateSelector itemTemplateSelectorCache;
        private Orientation orientationCache = Orientation.Vertical;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadLoopingList"/> class.
        /// </summary>
        public RadLoopingList()
        {
            this.DefaultStyleKey = typeof(RadLoopingList);

            this.SizeChanged += this.OnSizeChanged;
            this.KeyDown += this.OnKeyDown;
            
            this.isExpanded = true;
            this.selectedVisualIndex = -1;
            this.selectedIndex = -1;

            this.ManipulationMode = ManipulationModes.TranslateInertia | ManipulationModes.TranslateY | ManipulationModes.TranslateX;
        }

        /// <summary>
        /// Notifies for a change in the currently selected index.
        /// </summary>
        public event EventHandler SelectedIndexChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="IsExpanded"/> property has changed.
        /// </summary>
        public event EventHandler IsExpandedChanged;

        /// <summary>
        /// Gets or sets a double value that is applied as an offset correction
        /// after the selected item has been snapped according to the <see cref="RadLoopingList.CenteredItemSnapPosition"/>.
        /// </summary>
        public double SnapOffsetCorrection
        {
            get
            {
                return (double)this.GetValue(SnapOffsetCorrectionProperty);
            }
            set
            {
                this.SetValue(SnapOffsetCorrectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the orientation of looping.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return this.orientationCache;
            }
            set
            {
                this.SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the position of the centered item relatively to the viewport's starting edge
        /// if the <see cref="IsCentered"/> property is set to true.
        /// </summary>
        public LoopingListItemSnapPosition CenteredItemSnapPosition
        {
            get
            {
                return (LoopingListItemSnapPosition)this.GetValue(CenteredItemSnapPositionProperty);
            }
            set
            {
                this.SetValue(CenteredItemSnapPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the list will vertically center the currently selected item.
        /// </summary>
        public bool IsCentered
        {
            get
            {
                return (bool)this.GetValue(IsCenteredProperty);
            }
            set
            {
                this.SetValue(IsCenteredProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether looping is enabled. That is to allow infinite scrolling of the current logical wheel.
        /// </summary>
        public bool IsLoopingEnabled
        {
            get
            {
                return (bool)this.GetValue(IsLoopingEnabledProperty);
            }
            set
            {
                this.SetValue(IsLoopingEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the list is currently expanded. That is to put all the items except the currently selected one in the "Collapsed" visual state.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }
            set
            {
                if (this.isExpanded != value)
                {
                    this.SetValue(IsExpandedProperty, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the index that is currently selected.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return this.selectedIndex;
            }
            set
            {
                this.SetValue(SelectedIndexProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of a single item within the hosted <see cref="RadLoopingList"/> panel.
        /// </summary>
        public double ItemWidth
        {
            get
            {
                return (double)this.GetValue(ItemWidthProperty);
            }
            set
            {
                this.SetValue(ItemWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of a single item within the hosted <see cref="RadLoopingList"/> panel.
        /// </summary>
        public double ItemHeight
        {
            get
            {
                return (double)this.GetValue(ItemHeightProperty);
            }
            set
            {
                this.SetValue(ItemHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the spacing among visual items within the hosted <see cref="LoopingPanel"/> instance.
        /// </summary>
        public double ItemSpacing
        {
            get
            {
                return (double)this.GetValue(ItemSpacingProperty);
            }
            set
            {
                this.SetValue(ItemSpacingProperty, value);
            }
        }

        /// <summary>
        /// Gets the first logical index that is realized within the hosted <see cref="LoopingPanel"/>.
        /// </summary>
        public int FirstRealizedIndex
        {
            get
            {
                if (this.itemsPanel != null)
                {
                    return this.itemsPanel.TopLogicalIndex;
                }

                return -1;
            }
        }

        /// <summary>
        /// Gets or sets the DataTemplate to be applied to each visual item present within the selector.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(ItemTemplateProperty);
            }
            set
            {
                this.SetValue(ItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a template selector that used to provide a specific visualization for
        /// specific items.
        /// </summary>
        public DataTemplateSelector ItemTemplateSelector
        {
            get
            {
                return this.itemTemplateSelectorCache;
            }

            set
            {
                this.SetValue(RadLoopingList.ItemTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets the currently accumulated vertical offset.
        /// </summary>
        public double VerticalOffset
        {
            get
            {
                if (this.itemsPanel == null)
                {
                    return 0;
                }

                return this.itemsPanel.VerticalOffset;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the list is loaded and has its template applied.
        /// </summary>
        public bool IsInitialized
        {
            get
            {
                return this.isInitialized;
            }
        }

        /// <summary>
        /// Gets or sets the Style to be applied to each visual item present within the selector.
        /// </summary>
        public Style ItemStyle
        {
            get
            {
                return (Style)this.GetValue(ItemStyleProperty);
            }
            set
            {
                this.SetValue(ItemStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ItemsSource used as a DataContext of the looping list items.
        /// </summary>
        public IList ItemsSource
        {
            get
            {
                return (IList)this.GetValue(ItemsSourceProperty);
            }
            set
            {
                this.SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets the current scrolling state of the inner <see cref="LoopingPanel"/>.
        /// </summary>
        internal LoopingPanelScrollState ScrollState
        {
            get
            {
                if (this.itemsPanel == null)
                {
                    return LoopingPanelScrollState.NotScrolling;
                }

                return this.itemsPanel.ScrollState;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Tap event will toggle the IsExpanded property of the list.
        /// </summary>
        internal bool AutoExpandCollapse
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the reason for a change in the SelectedIndex property.
        /// </summary>
        internal LoopingListSelectionChangeReason SelectionChangeReason
        {
            get
            {
                return this.selectionChangeReason;
            }
        }

        /// <summary>
        /// Gets the <see cref="LoopingPanel"/> instance that hosts the visual items.
        /// </summary>
        internal LoopingPanel ItemsPanel
        {
            get
            {
                return this.itemsPanel;
            }
        }

        internal int SelectedVisualIndex
        {
            get
            {
                return this.selectedVisualIndex;
            }
        }

        /// <summary>
        /// Applies the desired vertical offset by using an animation, described by the specified duration and easing function.
        /// </summary>
        /// <param name="offset">The desired scroll offset.</param>
        /// <param name="duration">The duration of the animation.</param>
        /// <param name="easing">The easing function.</param>
        public void AnimateVerticalOffset(Duration duration, EasingFunctionBase easing, double offset)
        {
            if (this.itemsPanel == null)
            {
                return;
            }

            this.itemsPanel.SetVerticalOffset(offset, duration, easing);
        }

        /// <summary>
        /// Animates the vertical offset to the specified value, starting from the current one.
        /// </summary>
        /// <param name="offset">The final value of the animation.</param>
        public void AnimateVerticalOffset(double offset)
        {
            if (this.itemsPanel == null)
            {
                return;
            }

            this.itemsPanel.AnimateVerticalOffset(offset);
        }

        internal bool IsIndexInView(int logicalIndex)
        {
            return this.itemsPanel.IsIndexInView(logicalIndex);
        }

        /// <summary>
        /// Applies the specified index as currently selected.
        /// </summary>
        /// <param name="newSelectedLogicalIndex">The desired selected logical index.</param>
        /// <param name="newSelectedVisualIndex">The desired selected visual index.</param>
        /// <param name="reason">The reason of the change.</param>
        internal virtual void UpdateSelection(int newSelectedLogicalIndex, int newSelectedVisualIndex, LoopingListSelectionChangeReason reason)
        {
            if (this.selectedVisualIndex == newSelectedVisualIndex && this.selectedIndex == newSelectedLogicalIndex)
            {
                return;
            }

            if (!this.CanChangeSelectedIndex(newSelectedLogicalIndex))
            {
                this.selectionChangeReason = LoopingListSelectionChangeReason.PrivateNoNotify;
                this.SetValue(SelectedIndexProperty, this.selectedIndex);
            }
            else
            {
                if (reason == LoopingListSelectionChangeReason.User)
                {
                    this.selectionChangeReason = LoopingListSelectionChangeReason.Private;
                }
                else
                {
                    this.selectionChangeReason = reason;
                }

                // do the actual selection change
                this.selectedIndex = newSelectedLogicalIndex;
                this.selectedVisualIndex = newSelectedVisualIndex;
                this.SetValue(SelectedIndexProperty, newSelectedLogicalIndex);

                // examine the selection change reason
                switch (reason)
                {
                    case LoopingListSelectionChangeReason.User:
                    case LoopingListSelectionChangeReason.Private:
                    case LoopingListSelectionChangeReason.PrivateNoNotify:
                        this.SelectCurrentItem();
                        break;
                }

                this.UpdateItemsIsSelectedState();

                if (this.ItemsPanel != null)
                {
                    var loopingListItem = this.ItemsPanel.ItemFromLogicalIndex(this.selectedIndex) as LoopingListItem;
                    if (loopingListItem != null)
                    {
                        loopingListItem.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementAddedToSelection,
                                                             AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection,
                                                             AutomationEvents.SelectionItemPatternOnElementSelected,
                                                             AutomationEvents.SelectionPatternOnInvalidated,
                                                             AutomationEvents.AutomationFocusChanged);

                    }
                }
            }

            if (reason != LoopingListSelectionChangeReason.PrivateNoNotify)
            {
                this.RaiseSelectedIndexChanged();
            }

            this.selectionChangeReason = LoopingListSelectionChangeReason.User;
        }

        /// <summary>
        /// Selects the specified visual item.
        /// </summary>
        /// <param name="item">The item to be selected.</param>
        /// <param name="reason">The reason that caused the select request.</param>
        internal virtual bool SelectItem(LoopingListItem item, LoopingListSelectionChangeReason reason)
        {
            if (!this.CanChangeSelectedIndex(item.LogicalIndex))
            {
                return false;
            }

            if (!item.IsSelected)
            {
                // update the selected index
                this.UpdateSelection(item.LogicalIndex, item.VisualIndex, reason);
            }
            else
            {
                this.UpdateItemsIsSelectedState();
            }

            return true;
        }

        /// <summary>
        /// Creates a <see cref="LoopingListItem"/> instance to be used by the hosted <see cref="LoopingPanel"/>.
        /// </summary>
        internal LoopingListItem CreateVisualItem()
        {
            LoopingListItem item = this.CreateVisualItemInstance();

            item.Width = this.ItemWidth;
            item.Height = this.ItemHeight;

            double spacing = this.ItemSpacing;
            item.Margin = this.orientationCache == Orientation.Vertical ? new Thickness(0, spacing, 0, spacing) : new Thickness(spacing, 0, spacing, 0);

            if (this.itemStyleCache != null)
            {
                item.Style = this.itemStyleCache;
            }

            return item;
        }

        /// <summary>
        /// Associates the data at the specified logical index with the provided visual item instance.
        /// </summary>
        /// <param name="item">The visual item which data is to be updated.</param>
        /// <param name="logicalIndex">The logical index that describes the data.</param>
        /// <param name="visualIndex">The visual index that describes the data.</param>
        /// <param name="force">True to re-evaluate the data event if the visual item is already associated with the specified logical index.</param>
        /// <param name="animate">True to update the visual state using transitions.</param>
        internal void UpdateVisualItem(LoopingListItem item, int logicalIndex, int visualIndex, bool force, bool animate)
        {
            if ((!force && item.LogicalIndex == logicalIndex && item.VisualIndex == visualIndex) || item.IsEmpty)
            {
                return;
            }

            item.LogicalIndex = logicalIndex;
            item.VisualIndex = visualIndex;

            this.UpdateItemVisualState(item, new VisualStateUpdateParams(animate, true, true));
            this.SetDataItem(item);
        }

        /// <summary>
        /// Updates the visual state of a single item.
        /// </summary>
        /// <param name="item">The <see cref="LoopingListItem"/> instance which state is to be updated.</param>
        /// <param name="updateParams">The structure that encapsulates different update parameters such as Animate, EvaluateEnabled, etc.</param>
        internal virtual void UpdateItemVisualState(LoopingListItem item, VisualStateUpdateParams updateParams)
        {
            if (item.IsEmpty)
            {
                return;
            }

            int visualIndex = item.VisualIndex;
            int logicalIndex = item.LogicalIndex;

            item.BeginVisualStateUpdate();

            if (updateParams.EvaluateEnabled)
            {
                item.IsEnabled = this.IsItemEnabled(logicalIndex);
            }

            if (updateParams.EvaluateSelected)
            {
                item.IsSelected = this.IsItemSelected(logicalIndex, visualIndex);
            }

            item.EndVisualStateUpdate(true, updateParams.Animate);
        }

        /// <summary>
        /// Handles a click from a child <see cref="LoopingListItem"/>.
        /// </summary>
        internal virtual void OnVisualItemTap(LoopingListItem item)
        {
            this.SelectTappedItem(item);
        }

        /// <summary>
        /// Handles a Double-tap event from a child <see cref="LoopingListItem"/>.
        /// </summary>
        internal virtual void OnVisualItemDoubleTap(LoopingListItem item)
        {
            this.SelectTappedItem(item);
        }

        /// <summary>
        /// Updates the logical count of the items panel.
        /// </summary>
        internal bool UpdateLogicalCount(bool updateWheel)
        {
            if (this.itemsPanel == null)
            {
                return false;
            }

            int newLogicalCount = this.GetLogicalCount();
            if (newLogicalCount == this.itemsPanel.LogicalCount)
            {
                return false;
            }

            this.itemsPanel.LogicalCount = newLogicalCount;
            if (updateWheel)
            {
                this.itemsPanel.StopOffsetAnimation(false);
                this.itemsPanel.UpdateWheel(0, true);
            }

            return true;
        }

        /// <summary>
        /// Called after template is applied and the control is loaded on the SL visual tree.
        /// </summary>
        internal virtual void OnInitialized()
        {
            this.isInitialized = true;

            this.UpdateItemsVisualState();
        }

        /// <summary>
        /// Initializes the data.
        /// </summary>
        internal virtual void InitData()
        {
        }

        /// <summary>
        /// Retrieves the logical count of the current data source (if any).
        /// </summary>
        internal virtual int GetLogicalCount()
        {
            if (this.ItemsSource != null)
            {
                return this.ItemsSource.Count;
            }

            return this.itemsPanel.LogicalCount;
        }

        /// <summary>
        /// Raises the <see cref="SelectedIndexChanged"/> event.
        /// </summary>
        internal virtual void RaiseSelectedIndexChanged()
        {
            EventHandler eh = this.SelectedIndexChanged;
            if (eh != null)
            {
                eh(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Forces complete data re-evaluation of the currently visualized items.
        /// </summary>
        internal virtual void UpdateData(bool animate)
        {
            if (this.itemsPanel != null)
            {
                this.itemsPanel.UpdateIndexes(true, animate);
            }
        }

        /// <summary>
        /// Allows to minimize the overhead of creating new instances whenever a logical index changes by formatting an existing data item instance.
        /// </summary>
        internal virtual void UpdateDataItem(LoopingListDataItem dataItem, LoopingListItem visualItem)
        {
            if (this.ItemsSource != null)
            {
                dataItem.Item = this.ItemsSource[visualItem.LogicalIndex];
            }

            this.UpdateItemTemplate(dataItem, visualItem);
        }

        internal virtual LoopingListDataItem CreateDataItem(int logicalIndex)
        {
            Debug.WriteLine("logical index: {0}", logicalIndex);
            if (this.ItemsSource[logicalIndex] != null)
            {
                return new LoopingListDataItem() { Item = this.ItemsSource[logicalIndex] };
            }

            return new LoopingListDataItem();
        }

        /// <summary>
        /// Raises the <see cref="E:IsExpandedChanged"/> event and allows inheritors to provide additional logic upon IsExpanded property changed.
        /// </summary>
        internal virtual void OnIsExpandedChanged()
        {
            EventHandler eh = this.IsExpandedChanged;
            if (eh != null)
            {
                eh(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Determines whether the data item that resides at the specified logical index should be visualized with the Selected state.
        /// </summary>
        /// <param name="logicalIndex">The logical index that defines the data to check for.</param>
        /// <param name="visualIndex">The visual index that defines the data to check for.</param>
        internal virtual bool IsItemSelected(int logicalIndex, int visualIndex)
        {
            return visualIndex == this.selectedVisualIndex && logicalIndex == this.selectedIndex;
        }

        internal int GetVisualIndex(int logicalIndex)
        {
            if (!this.IsTemplateApplied || this.itemsPanel.VisualCount == 0)
            {
                return -1;
            }

            if (logicalIndex >= this.itemsPanel.LogicalCount || logicalIndex < 0)
            {
                throw new ArgumentOutOfRangeException("Not valid index.", "logicalIndex");
            }

            var count = this.itemsPanel.VisualCount;
            var visualIndex = 0;
            bool found = false;

            foreach (LoopingListItem item in this.itemsPanel.Children)
            {
                if (!item.IsEmpty && !item.isHidden && item.LogicalIndex == logicalIndex)
                {
                    visualIndex = item.VisualIndex;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                var topItem = this.itemsPanel.GetFirstVisibleItem();
                var topLogicalIndex = topItem.LogicalIndex;
                var topVisualIndex = topItem.VisualIndex;
                var currentLogicalIndex = topLogicalIndex;

                if (this.IsLoopingEnabled)
                {
                    var difference = this.itemsPanel.FindClosestIndexDistance(topLogicalIndex, logicalIndex, this.itemsPanel.LogicalCount);
                    visualIndex = (topVisualIndex + difference) % count;
                }
                else
                {
                    var difference = logicalIndex - topLogicalIndex;
                    visualIndex = (topVisualIndex + difference) % count;
                }
            }

            if (visualIndex < 0)
            {
                visualIndex += count;
            }

            Debug.Assert(visualIndex >= 0, "Visual index should not be negative.");

            return visualIndex;
        }

        /// <summary>
        /// Determines whether the logical item, representing the specified logical index should be enabled or not.
        /// </summary>
        /// <param name="logicalIndex">The logical index that defines the data to check for.</param>
        internal virtual bool IsItemEnabled(int logicalIndex)
        {
            return true;
        }

        /// <summary>
        /// Determines whether the currently selected index can be changed.
        /// Allows inheritors to optionally prevent the change under certain conditions.
        /// </summary>
        internal virtual bool CanChangeSelectedIndex(int newIndex)
        {
            return this.IsItemEnabled(newIndex);
        }

        /// <summary>
        /// Brings the currently selected index into view and selects the visual item that represents it.
        /// </summary>
        internal virtual void SelectCurrentItem()
        {
            if (this.itemsPanel != null)
            {
                this.itemsPanel.BringIntoView(this.selectedIndex, this.selectedVisualIndex);

             //   Debug.Assert(this.IsIndexInView(this.selectedIndex), "Index should be in view.");

                var item = this.itemsPanel.ItemFromVisualIndex(this.selectedVisualIndex);
                if (item != null)
                {
                    this.SelectItem(item, LoopingListSelectionChangeReason.User);
                }
            }
        }

        /// <summary>
        /// Forces each visual item to update its state.
        /// </summary>
        internal void UpdateItemsVisualState(VisualStateUpdateParams updateParams)
        {
            if (!this.isInitialized)
            {
                return;
            }

            if (this.itemsPanel != null)
            {
                foreach (LoopingListItem item in this.itemsPanel.Children)
                {
                    this.UpdateItemVisualState(item, updateParams);
                }
            }
        }

        /// <summary>
        /// Centers the item at the currently selected index.
        /// </summary>
        internal void CenterCurrentItem(bool animate)
        {
            if (this.itemsPanel != null)
            {
                LoopingListItem item = this.itemsPanel.ItemFromVisualIndex(this.selectedVisualIndex);
                if (item != null)
                {
                    this.itemsPanel.CenterItem(item, false, animate);
                }
                else
                {
                    this.itemsPanel.BringIntoView(this.selectedIndex, this.selectedVisualIndex);
                }
            }
        }

        /// <summary>
        /// Creates the core <see cref="LoopingListItem"/> instance.
        /// </summary>
        internal virtual LoopingListItem CreateVisualItemInstance()
        {
            return new LoopingListItem();
        }

        internal void UpdateSelectedItemFocusedState()
        {
            var item = this.ItemsPanel.ItemFromLogicalIndex(this.SelectedIndex);
            if (item != null)
            {
                // update the visual state to show the FocusCue in case the owning picker is focused
                item.UpdateVisualState(false);
            }
        }

        internal void StopInertia()
        {
            if (this.itemsPanel == null || this.itemsPanel.ScrollState != LoopingPanelScrollState.Scrolling)
            {
                return;
            }

            this.allowInertia = false;
            this.itemsPanel.EndScroll();
        }

        internal void ResetItemsPanel()
        {
            if (this.itemsPanel != null)
            {
                this.itemsPanel.Reset();
            }
        }

        internal void UpdateSelectedVisualIndex()
        {
            if (this.selectedIndex >= 0)
            {
                this.selectedVisualIndex = this.GetVisualIndex(this.selectedIndex);
                this.UpdateItemsIsSelectedState();
            }
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Loaded"/> event. Allows inheritors to provide their specific logic.
        /// </summary>
        protected override void LoadCore()
        {
            base.LoadCore();

            if (this.IsTemplateApplied)
            {
                this.OnInitialized();
            }
        }

        /// <summary>
        /// Retrieves the control's template parts.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.itemsPanel = this.GetTemplatePartField<LoopingPanel>("PART_Panel");
            applied = applied && this.itemsPanel != null;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate"/> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.itemsPanel.Owner = this;
            this.SetupPanel();

            if (this.IsLoaded)
            {
                this.OnInitialized();
            }
        }

        /// <summary>
        /// Called before the ManipulationStarted event occurs.
        /// </summary>
        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            base.OnManipulationStarted(e);

            if (e == null)
            {
                return;
            }

            this.allowInertia = true;
            if (!this.isExpanded)
            {
                this.IsExpanded = true;
            }
        }

        /// <summary>
        /// Called before the ManipulationDelta event occurs.
        /// </summary>
        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            base.OnManipulationDelta(e);

            if (e == null)
            {
                return;
            }

            if (e.IsInertial && !this.allowInertia)
            {
                return;
            }

            this.itemsPanel.Scroll(this.Orientation == Orientation.Vertical ? e.Delta.Translation.Y : e.Delta.Translation.X);
        }

        /// <summary>
        /// Called before the ManipulationCompleted event occurs.
        /// </summary>
        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            base.OnManipulationCompleted(e);

            if (e == null)
            {
                return;
            }

            this.allowInertia = true;
            this.itemsPanel.EndScroll();
        }

        /// <summary>
        /// Called before the Tapped event occurs.
        /// </summary>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            if (e == null)
            {
                return;
            }

            if (this.skipTapped)
            {
                this.skipTapped = false;
                return;
            }

            Point hitPoint = e.GetPosition(this);
            LoopingListItem tappedItem = this.itemsPanel.ItemFromOffset(this.Orientation == Orientation.Vertical ? hitPoint.Y : hitPoint.X);
            if (tappedItem != null)
            {
                this.OnVisualItemTap(tappedItem);
            }
        }

        /// <summary>
        /// Called before the GotFocus event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            
            this.IsExpanded = true;
        }

        /// <summary>
        /// Called before the LostFocus event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            
            this.IsExpanded = false;
            this.skipTapped = false;
        }

        /// <summary>
        /// Called before the PointerPressed event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (e == null)
            {
                return;
            }

            if (this.IsTabStop)
            {
                this.CapturePointer(e.Pointer);

                if (this.FocusState == FocusState.Unfocused)
                {
                    this.Focus(FocusState.Programmatic);
                    this.skipTapped = true;
                }
            }
        }

        /// <summary>
        /// Called before the PointerReleased event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);

            this.ReleasePointerCaptures();

            if (e == null)
            {
                return;
            }

            if (this.IsTabStop && this.FocusState != FocusState.Unfocused)
            {
                e.Handled = true;
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new Automation.Peers.RadLoopingListAutomationPeer(this);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadLoopingList list = d as RadLoopingList;
            list.ResetItemsPanel();
            list.UpdateLogicalCount(true);
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadLoopingList list = d as RadLoopingList;
            if (list.IsInternalPropertyChange)
            {
                return;
            }

            var selectedIndex = (int)e.NewValue;
            var selectedVisualIndex = list.GetVisualIndex(selectedIndex);

            if (!list.IsTemplateApplied)
            {
                list.selectedIndex = selectedIndex;
                list.selectedVisualIndex = selectedVisualIndex;
            }
            else if (list.selectionChangeReason == LoopingListSelectionChangeReason.User)
            {
                list.UpdateSelection(selectedIndex, selectedVisualIndex, LoopingListSelectionChangeReason.User);
            }
        }

        private static void OnSnapOffsetCorrectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            RadLoopingList list = d as RadLoopingList;
            if (list.itemsPanel != null)
            {
                list.itemsPanel.SnapOffsetCorrection = (double)args.NewValue;
            }
        }

        private static void OnCenteredItemSnapPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadLoopingList list = d as RadLoopingList;
            if (list.itemsPanel != null)
            {
                list.itemsPanel.CenteredItemSnapPosition = (LoopingListItemSnapPosition)e.NewValue;
            }
        }

        private static void OnIsCenteredChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadLoopingList list = d as RadLoopingList;
            if (list.itemsPanel != null)
            {
                list.itemsPanel.IsCentered = (bool)e.NewValue;
            }
        }

        private static void OnIsLoopingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadLoopingList list = d as RadLoopingList;
            if (list.itemsPanel != null)
            {
                list.itemsPanel.IsLoopingEnabled = (bool)e.NewValue;
            }
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadLoopingList list = d as RadLoopingList;
            list.orientationCache = (Orientation)e.NewValue;
            list.OnOrientationChanged();
        }

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadLoopingList list = d as RadLoopingList;
            list.isExpanded = (bool)e.NewValue;

            list.UpdateItemsVisualState();

            RadLoopingListAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(list) as RadLoopingListAutomationPeer;
            if (peer != null)
            {
                peer.RaiseExpandCollapseAutomationEvent(!((bool)e.NewValue), (bool)e.NewValue);
            }

            list.OnIsExpandedChanged();
        }

        private static void OnItemHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadLoopingList list = d as RadLoopingList;
            if (list.itemsPanel != null)
            {
                list.ResetItemsPanel();
            }
        }

        private static void OnItemWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadLoopingList list = d as RadLoopingList;
            if (list.itemsPanel != null)
            {
                list.ResetItemsPanel();
            }
        }

        private static void OnItemSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadLoopingList list = d as RadLoopingList;
            if (list.itemsPanel != null)
            {
                list.ResetItemsPanel();
            }
        }

        private static void OnItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadLoopingList list = d as RadLoopingList;
            list.itemTemplateCache = (DataTemplate)e.NewValue;
        }

        private static void OnItemStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadLoopingList list = d as RadLoopingList;
            list.itemStyleCache = (Style)e.NewValue;
        }

        private static void OnItemTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadLoopingList list = d as RadLoopingList;
            list.itemTemplateSelectorCache = (DataTemplateSelector)e.NewValue;
        }

        private int GetLogicalIndex(int visualIndex)
        {
            var topLogicalIndex = this.itemsPanel.GetFirstRealizedIndex(this.itemsPanel.LogicalCount);
            var index = this.itemsPanel.VisualIndexChain.IndexOf(visualIndex);

            return (topLogicalIndex + index) % this.itemsPanel.LogicalCount;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RectangleGeometry clip = new RectangleGeometry();
            clip.Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);
            this.Clip = clip;
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = this.HandleKeyboardInput(e.Key);
            }
        }

        private bool HandleKeyboardInput(VirtualKey key)
        {
            if (this.SelectedIndex > -1)
            {
                switch (key)
                {
                    case VirtualKey.Up:
                        this.SelectPrev(this.SelectedIndex - 1);
                        return true;
                    case VirtualKey.Down:
                        this.SelectNext(this.SelectedIndex + 1);
                        return true;
                    case VirtualKey.Home:
                        if (this.SelectedIndex != 0)
                        {
                            this.SelectedIndex = 0;
                        }
                        return true;
                    case VirtualKey.End:
                        var lastIndex = this.itemsPanel.LogicalCount - 1;
                        if (this.SelectedIndex != lastIndex)
                        {
                            this.SelectedIndex = lastIndex;
                        }
                        return true;
                }
            }

            return false;
        }

        internal void SelectNext(int indexToSelect)
        {
            if (this.IsIndexSelectable(indexToSelect))
            {
                this.SelectedIndex = indexToSelect >= this.itemsPanel.LogicalCount ? 0 : indexToSelect;
            }
        }

        internal void SelectPrev(int indexToSelect)
        {
            if (this.IsIndexSelectable(indexToSelect))
            {
                this.SelectedIndex = indexToSelect < 0 ? this.itemsPanel.LogicalCount - 1 : indexToSelect;
            }
        }

        private bool IsIndexSelectable(int index)
        {
            if (this.IsLoopingEnabled || (index < this.itemsPanel.LogicalCount && index > -1))
            {
                return true;
            }

            return false;
        }

        private void SetupPanel()
        {
            this.itemsPanel.IsCentered = this.IsCentered;
            this.itemsPanel.IsLoopingEnabled = this.IsLoopingEnabled;
            this.itemsPanel.Owner = this;
            this.itemsPanel.CenteredItemSnapPosition = this.CenteredItemSnapPosition;
            this.itemsPanel.SnapOffsetCorrection = this.SnapOffsetCorrection;

            this.UpdateLogicalCount(false);
        }

        private void SetDataItem(LoopingListItem visualItem)
        {
            LoopingListDataItem dataItem = visualItem.Content as LoopingListDataItem;
            if (dataItem == null)
            {
                dataItem = this.CreateDataItem(visualItem.LogicalIndex);
                visualItem.DataContext = dataItem;
                visualItem.Content = dataItem;
            }

            this.UpdateDataItem(dataItem, visualItem);
        }

        private void UpdateItemsIsSelectedState()
        {
            if (this.itemsPanel == null)
            {
                return;
            }

            foreach (LoopingListItem item in this.itemsPanel.Children)
            {
                if (item.IsEmpty)
                {
                    continue;
                }

                item.IsSelected = this.IsItemSelected(item.LogicalIndex, item.VisualIndex);
            }
        }

        private void UpdateItemTemplate(LoopingListDataItem dataItem, LoopingListItem visualItem)
        {
            DataTemplate template = null;
            DataTemplateSelector selector = this.itemTemplateSelectorCache;
            if (selector != null)
            {
                template = selector.SelectTemplate(dataItem, visualItem);
            }

            if (template == null)
            {
                template = this.itemTemplateCache;
            }

            visualItem.ContentTemplate = template;
        }

        private void SelectTappedItem(LoopingListItem item)
        {
            if (!this.isExpanded)
            {
                this.IsExpanded = true;
                return;
            }

            if (item.isHidden || item.IsEmpty)
            {
                return;
            }

            if (!item.IsSelected)
            {
                if (this.isExpanded)
                {
                    this.SelectItem(item, LoopingListSelectionChangeReason.VisualItemClick);
                }
            }
            else if (this.AutoExpandCollapse)
            {
                // Toggle expand/collapse if the tap is over the selected item
                this.IsExpanded = !this.isExpanded;
            }

            if (item.IsSelected && this.itemsPanel != null && this.itemsPanel.IsCentered)
            {
                this.itemsPanel.CenterItem(item, false, true);
            }
        }

        private void UpdateItemsVisualState()
        {
            if (this.itemsPanel != null)
            {
                this.StopInertia();

                VisualStateUpdateParams updateParams = new VisualStateUpdateParams(true);
                this.UpdateItemsVisualState(updateParams);
            }
        }

        private void OnOrientationChanged()
        {
            if (this.itemsPanel != null)
            {
                this.itemsPanel.UpdateOrientation(this.orientationCache);
            }
        }
    }
}
