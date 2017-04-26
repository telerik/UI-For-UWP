using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Telerik.Core;
using Telerik.Core.Data;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a base class for all data controls that support UI virtualization.
    /// </summary>
    public partial class RadVirtualizingDataControl : DataControlBase, IItemsContainer, INotifyPropertyChanged
    {
        /// <summary>
        /// Identifies the <see cref="VirtualizationStrategyDefinition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VirtualizationStrategyDefinitionProperty =
            DependencyProperty.Register(nameof(VirtualizationStrategyDefinition), typeof(VirtualizationStrategyDefinition), typeof(RadVirtualizingDataControl), new PropertyMetadata(new StackVirtualizationStrategyDefinition() { Orientation = Orientation.Vertical }, OnVirtualizationStrategyDefinitionChanged));

        /// <summary>
        /// Identifies the DisplayMemberPath dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(nameof(DisplayMemberPath), typeof(string), typeof(RadVirtualizingDataControl), new PropertyMetadata(null, OnDisplayMemberPathChanged));

        internal double availableWidth;
        internal bool layoutUpdated;
        internal double availableHeight;
        internal double balanceCounter = 0;
        internal ScrollUpdateService scrollUpdateService;
        internal bool scrollScheduled;

        private const double DefaultItemLength = 200;

        private const string NotSupportedExceptionText = "Cannot support both DisplayMemberPath and ItemTemplate or ItemTemplateSelector.";

        private static readonly double Exponent = Math.Log(PhysicsConstants.MotionParameters.Friction);

        private DispatcherTimer renderingTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(5) };
        private string displayMemberPathCache = null;
        private PropertyInfo displayMemberPropInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadVirtualizingDataControl" /> class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "These virtual calls do not rely on uninitialized base state.")]
        public RadVirtualizingDataControl()
        {
            this.DefaultStyleKey = typeof(RadVirtualizingDataControl);
            this.recycledItems = new Queue<RadVirtualizingDataControlItem>();
            this.realizedItems = new List<RadVirtualizingDataControlItem>();
            this.opacityAnimation = new DoubleAnimation();
            this.opacityAnimation.EnableDependentAnimation = true;
            this.scrollUpdateService = new ScrollUpdateService();
            this.SizeChanged += this.OnSizeChanged;

            this.enableAsyncBalance = false;

            this.InitItemAnimations();
            this.InitVirtualizationStrategy(this.VirtualizationStrategyDefinition);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fires when the virtualization state of an item is changed.
        /// </summary>
        public event EventHandler<ItemStateChangedEventArgs> ItemStateChanged;

        /// <summary>
        /// Occurs when the items in the data source have changed.
        /// </summary>
        internal event EventHandler<NotifyCollectionChangedEventArgs> ItemsChanged;

        /// <summary>
        /// Gets or sets a struct that represents a metadata description of a <see cref="VirtualizationStrategy"/> implementation.
        /// </summary>
        public VirtualizationStrategyDefinition VirtualizationStrategyDefinition
        {
            get
            {
                return (VirtualizationStrategyDefinition)this.GetValue(VirtualizationStrategyDefinitionProperty);
            }
            set
            {
                this.SetValue(VirtualizationStrategyDefinitionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether asynchronous balance may be used when needed -
        /// for example when the control is initially loading its items or upon a <see cref="M:BringIntoView"/> call.
        /// </summary>
        public bool IsAsyncBalanceEnabled
        {
            get
            {
                return this.enableAsyncBalance;
            }
            set
            {
                this.enableAsyncBalance = value;
                if (!this.enableAsyncBalance)
                {
                    this.useAsyncBalance = false;
                }
            }
        }

        /// <summary>
        /// Gets the count of the currently recycled visual items.
        /// </summary>
        public int RecycledItemsCount
        {
            get
            {
                return this.recycledItems.Count;
            }
        }

        /// <summary>
        /// Gets the count of the visual items that are currently realized.
        /// </summary>
        public int RealizedItemsCount
        {
            get
            {
                return this.realizedItems.Count;
            }
        }

        /// <summary>
        /// Gets an array containing the currently realized data items, i.e. the
        /// data items that a currently bound to a visual item.
        /// </summary>
        public object[] RealizedItems
        {
            get
            {
                object[] result = new object[this.realizedItems.Count];

                int currentIndex = 0;
                foreach (RadVirtualizingDataControlItem item in this.realizedItems)
                {
                    result[currentIndex++] = item.associatedDataItem.Value;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the top viewport item currently realized in <see cref="RadDataBoundListBox"/>.
        /// </summary>
        public object TopVisibleItem
        {
            get
            {
                IDataSourceItem topVisibleItem = this.GetTopVisibleItem();

                if (topVisibleItem != null)
                {
                    return topVisibleItem.Value;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the name or path of the property that is displayed for each data item.
        /// </summary>
        /// <value>The name or path of the property that is displayed for each the data item in the control. The default is an empty string ("").</value>
        public string DisplayMemberPath
        {
            get
            {
                return this.GetValue(DisplayMemberPathProperty) as string;
            }
            set
            {
                this.SetValue(DisplayMemberPathProperty, value);
            }
        }

        /// <summary>
        /// Gets the currently visible items in the Viewport.
        /// </summary>
        public FrameworkElement[] ViewportItems
        {
            get
            {
                if (this.realizedItems.Count == 0)
                {
                    return new RadVirtualizingDataControlItem[0];
                }

                RadVirtualizingDataControlItem topMostVisible = this.virtualizationStrategy.GetTopVisibleContainer();
                if (topMostVisible == null)
                {
                    return new RadVirtualizingDataControlItem[0];
                }

                List<RadVirtualizingDataControlItem> foundItems = new List<RadVirtualizingDataControlItem>();

                foundItems.Add(topMostVisible);
                RadVirtualizingDataControlItem currentItem = topMostVisible.next;

                while (currentItem != null)
                {
                    if (this.virtualizationStrategy.GetItemRelativeOffset(currentItem) > this.availableHeight)
                    {
                        break;
                    }

                    foundItems.Add(currentItem);
                    currentItem = currentItem.next;
                }

                RadVirtualizingDataControlItem[] result = null;
                result = new RadVirtualizingDataControlItem[foundItems.Count];
                foundItems.CopyTo(result);
                return result;
            }
        }

        /// <summary>
        /// Gets the IDataSourceItem instance that is the first visible item on top of the list.
        /// </summary>
        public IDataSourceItem TopVisibleListSourceItem
        {
            get
            {
                return this.GetTopVisibleItem();
            }
        }

        /// <summary>
        /// Gets all realized containers, i.e. all containers that are currently visible and bound to a data item.
        /// </summary>
        internal RadVirtualizingDataControlItem[] RealizedContainers
        {
            get
            {
                RadVirtualizingDataControlItem[] items = new RadVirtualizingDataControlItem[this.realizedItems.Count];
                this.realizedItems.CopyTo(items, 0);
                return items;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this control is properly templated.
        /// </summary>
        /// <value>
        /// <c>True</c> if this instance is properly templated; otherwise, <c>false</c>.
        /// </value>
        protected internal virtual bool IsProperlyTemplated
        {
            get
            {
                return this.itemsPanel != null && this.manipulationContainer != null && this.scrollableContent != null;
            }
        }

        /// <summary>
        /// Brings the given data item into the viewport.
        /// </summary>
        /// <param name="item">The item to show.</param>
        public virtual void BringIntoView(object item)
        {
            this.BringIntoView(item, null);
        }

        /// <summary>
        /// Brings the given data item into the viewport and calls the <paramref name="scrollCompletedAction"/> method when the operation is completed.
        /// </summary>
        /// <param name="item">The item to show.</param>
        /// <param name="scrollCompletedAction">Action to be executed when the operation is completed.</param>
        public virtual void BringIntoView(object item, Action scrollCompletedAction)
        {
            if (!this.IsProperlyTemplated)
            {
                this.initialVirtualizationItem = this.listSource.FindItem(item);
                return;
            }

            var bringIntoViewOperation = new BringIntoViewOperation(item) { CompletedAction = scrollCompletedAction, LastAverageItemLength = this.virtualizationStrategy.averageItemLength };
            this.BringIntoViewCore(bringIntoViewOperation);
        }

        internal virtual void BringIntoView(BringIntoViewOperation bringIntoViewOperation)
        {
            // Check whether the item to bring into view is already realized
            RadVirtualizingDataControlItem foundItem = null;
            foreach (RadVirtualizingDataControlItem uiItem in this.realizedItems)
            {
                if (object.Equals(uiItem.associatedDataItem.Value, bringIntoViewOperation.RequestedItem))
                {
                    foundItem = uiItem;
                    break;
                }
            }

            // If the item is not realized, recycle all items and start realizing from the pivot item (the one we should bring into view).
            if (foundItem == null)
            {
                this.initialVirtualizationItem = this.listSource.FindItem(bringIntoViewOperation.RequestedItem);
                if (this.initialVirtualizationItem == null)
                {
                    throw new ArgumentException("Specified item does not exist in the Items Source.");
                }

                this.RecycleAllItems();
                this.ApplyScrollOffsetForItem(this.initialVirtualizationItem, bringIntoViewOperation.LastAverageItemLength);
                this.BeginAsyncBalance();
            }
            else
            {
                // If the item is realized, calculate the smallest amount that the item can be offset so that it is fully visible in the viewport.
                double offset = this.GetRealizedItemBringIntoViewOffset(foundItem);
                this.virtualizationStrategy.ScrollToOffset(
                    offset,
                    () =>
                    {
                        // Perform a balance to ensure that all buffers are filled.
                        this.SubscribeRendering();
                        this.BalanceVisualSpace();
                    });
            }
        }

        internal void BringIntoViewCore(BringIntoViewOperation bringIntoViewOperation)
        {
            if (bringIntoViewOperation == null)
            {
                return;
            }

            if (bringIntoViewOperation.ScrollAttempts < BringIntoViewOperation.MaxScrollAttempts)
            {
                if (this.IsItemInViewport(bringIntoViewOperation.RequestedItem, false))
                {
                    if (bringIntoViewOperation.CompletedAction != null)
                    {
                        bringIntoViewOperation.CompletedAction.Invoke();
                    }

                    this.UnsubscribeRendering();
                }
                else
                {
                    this.BringIntoView(bringIntoViewOperation);
                    bringIntoViewOperation.ScrollAttempts++;
                    this.scrollUpdateService.RegisterUpdate(new DelegateUpdate(() => this.BringIntoViewCore(bringIntoViewOperation)));
                }
            }
            else
            {
                this.UnsubscribeRendering();
            }
        }

        internal void ApplyScrollOffsetForItem(IDataSourceItem item, double lastAverageLength)
        {
            if (lastAverageLength <= 0)
            {
                lastAverageLength = this.virtualizationStrategy.averageItemLength;

                if (lastAverageLength == 0)
                {
                    lastAverageLength = DefaultItemLength;
                }
            }

            double offset = this.virtualizationStrategy.CalculateItemOffset(item, lastAverageLength);
            this.scrollScheduled = true;

            this.virtualizationStrategy.ScrollToOffset(
                offset,
                () =>
                {
                    // this.virtualizationStrategy.SetElementCanvasOffset(this.itemsPanel, this.virtualizationStrategy.ScrollOffset);
                    //// Perform a balance to ensure that all buffers are filled.
                    this.SubscribeRendering();
                    this.BalanceVisualSpace();
                    scrollScheduled = false;
                });
        }

        internal override RadListSource CreateListSource()
        {
            RadListSource listSource = base.CreateListSource();
            this.SetupListSource(listSource);
            return listSource;
        }

        internal virtual double GetRealizedItemBringIntoViewOffset(RadVirtualizingDataControlItem item)
        {
            double itemOffset = this.virtualizationStrategy.GetItemRelativeOffset(item);
            double itemLowerBound = itemOffset + this.virtualizationStrategy.GetItemLength(item);
            double correctionOffset = 0;
            if (itemLowerBound > this.virtualizationStrategy.ViewportLength)
            {
                correctionOffset = itemLowerBound - this.virtualizationStrategy.ViewportLength;
            }
            else if (itemOffset < 0)
            {
                correctionOffset = itemOffset;
            }

            return correctionOffset;
        }

        internal virtual object ReflectPropertyValueAndStoreInfoIfNeeded(ref PropertyInfo cachedPropInfo, object source, string propertyPath)
        {
            Type objectType = source.GetType();

            if (cachedPropInfo != null &&
                cachedPropInfo.Name == propertyPath &&
                cachedPropInfo.DeclaringType == objectType)
            {
                return cachedPropInfo.GetValue(source, new object[] { });
            }

            if (!string.IsNullOrEmpty(propertyPath))
            {
                cachedPropInfo = objectType.GetRuntimeProperty(propertyPath);
                if (cachedPropInfo != null)
                {
                    return cachedPropInfo.GetValue(source, new object[] { });
                }
            }

            return null;
        }

        internal virtual int GetDataItemCount()
        {
            return this.listSource.Count;
        }

        internal virtual int GetItemCount()
        {
            return this.listSource.Count;
        }

        internal void ResetOnTemplateChange(bool bringItemIntoView = true)
        {
            if (this.realizedItems.Count == 0)
            {
                return;
            }

            DataSourceItem topMostItem = this.GetTopVisibleItem() as DataSourceItem;
            foreach (RadVirtualizingDataControlItem item in this.RealizedContainers)
            {
                this.PrepareContainerForItemOverride(item, item.associatedDataItem);
            }

            if (bringItemIntoView)
            {
                this.BringIntoView(topMostItem.Value);
            }
        }

        internal void OnContainerSizeChanged(RadVirtualizingDataControlItem container, Size newSize, Size oldSize)
        {
            if (this.GetItemCount() == 0)
            {
                return;
            }

            this.virtualizationStrategy.OnContainerSizeChanged(container, newSize, oldSize);
        }

        internal virtual void SetupListSource(RadListSource listSource)
        {
            listSource.CurrentItemChanging += this.OnListSource_CurrentItemChanging;
            listSource.CurrentItemChanged += this.OnListSource_CurrentItemChanged;
        }

        /// <summary>
        /// Changes the current ListSource instance with an external one. This is used in the PaginationListControl to save an additional ListSource instance.
        /// </summary>
        internal virtual void ChangeListSource(RadListSource externalSource)
        {
            this.listSource = externalSource;
            this.OnCollectionReset();
            this.UpdateVisualState(false);
        }

        internal virtual void OnVirtualizationStrategyDefinitionChanged(DependencyPropertyChangedEventArgs args)
        {
            VirtualizationStrategyDefinition definition = (VirtualizationStrategyDefinition)args.NewValue;
            this.InitVirtualizationStrategy(definition);
        }

        /// <summary>
        /// Determines whether the <see cref="RadVirtualizingDataControl"/> is in operational state.
        /// By default this method checks whether the control is in loaded state, whether the items panel is initialized
        /// and whether there are items in the list source.
        /// </summary>
        internal virtual bool IsOperational()
        {
            return this.IsLoaded && this.virtualizationStrategy != null && this.itemsPanel != null && this.GetItemCount() > 0;
        }

        internal virtual void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.virtualizationStrategy.OnViewportSizeChanged(e.NewSize, e.PreviousSize);
        }

        /// <summary>
        /// Called when the current item of the ListSource is about to be changed.
        /// </summary>
        protected internal virtual void OnListSourceCurrentItemChanging()
        {
        }

        /// <summary>
        /// Called when the current item of the ListSource has changed.
        /// </summary>
        protected internal virtual void OnListSourceCurrentItemChanged()
        {
        }

        /// <summary>
        /// Called when the <see cref="RadVirtualizingDataControl.DisplayMemberPath"/> has changed.
        /// </summary>
        protected internal virtual void OnDisplayMemberPathChanged()
        {
            if (this.itemTemplateSelectorCache != null || this.itemTemplateCache != null)
            {
                return;
            }

            this.displayMemberPropInfo = null;

            foreach (RadVirtualizingDataControlItem item in this.realizedItems)
            {
                object dataItem = item.associatedDataItem.Value;
                if (!string.IsNullOrEmpty(this.displayMemberPathCache))
                {
                    item.Content = this.ReflectPropertyValueAndStoreInfoIfNeeded(ref this.displayMemberPropInfo, dataItem, this.displayMemberPathCache);
                }
                else
                {
                    item.Content = dataItem.ToString();
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            this.manipulationContainer = this.GetTemplateChild("PART_ManipulationContainer") as ScrollViewer;
            this.itemsPanel = this.GetTemplateChild("PART_ItemsPanel") as Panel;
            this.scrollableContent = this.GetTemplateChild("PART_ScrollableContent") as Canvas;
            this.manipulationContainer.ApplyTemplate();
            base.OnApplyTemplate();
            this.LayoutUpdated += this.OnVirtualizingDataControl_LayoutUpdated;
            this.virtualizationStrategy.UpdateScrollBarsVisibility();
            this.virtualizationStrategy.OnOwnerApplyTemplate();
        }

        /// <summary>
        /// Occurs when this object is no longer connected to the main object tree.
        /// </summary>
        protected override void UnloadCore()
        {
            base.UnloadCore();
            this.UnsubscribeRendering();
            this.UpdateStrategyDefinitionPropertyChangedHandling(false);

            this.StopAllRemovedAnimations();
            this.StopAllAddedAnimations();

            if (!this.itemAddedBatchAnimationScheduled && this.itemAddedAnimationCache != null)
            {
                this.itemAddedBatchAnimationScheduled = false;
                this.itemAddedAnimationCache.Ended -= this.OnItemAddedAnimation_Ended;
            }

            if (!this.itemRemovedBatchAnimationScheduled && this.itemRemovedAnimationCache != null)
            {
                this.itemRemovedBatchAnimationScheduled = false;
                this.itemRemovedAnimationCache.Ended -= this.OnItemRemovedAnimation_Ended;
            }
        }

        /// <summary>
        /// Occurs when a System.Windows.FrameworkElement has been constructed and added to the object tree.
        /// </summary>
        protected override void OnLoaded()
        {
            base.OnLoaded();

            if (this.itemAddedAnimationCache != null)
            {
                this.itemAddedAnimationCache.Ended -= this.OnItemAddedAnimation_Ended;
                this.itemAddedAnimationCache.Ended += this.OnItemAddedAnimation_Ended;
            }

            if (this.itemRemovedAnimationCache != null)
            {
                this.itemRemovedAnimationCache.Ended -= this.OnItemRemovedAnimation_Ended;
                this.itemRemovedAnimationCache.Ended += this.OnItemRemovedAnimation_Ended;
            }

            this.UpdateStrategyDefinitionPropertyChangedHandling(true);
            this.virtualizationStrategyDefinition.SynchStrategyProperties(this.virtualizationStrategy);

            if (this.scrollContentPresenter == null)
            {
                this.scrollContentPresenter = ElementTreeHelper.FindVisualDescendant<ScrollContentPresenter>(this.manipulationContainer);
            }

            this.BalanceVisualSpace();
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size measuredSize = base.MeasureOverride(availableSize);

            if (this.virtualizationStrategy != null)
            {
                if (this.IsLoaded)
                {
                    this.ManageViewport();
                }

                measuredSize = this.virtualizationStrategy.Measure(availableSize);
            }

            return measuredSize;
        }

        /// <summary>
        /// Occurs when the <see cref="Telerik.UI.Xaml.Controls.Data.TemplateProviderControl.ItemTemplate" /> property has changed.
        /// </summary>
        protected override void OnItemTemplateChanged(DataTemplate oldTemplate)
        {
            base.OnItemTemplateChanged(oldTemplate);
            this.itemTemplateCache = this.ItemTemplate;
            this.ValidateDisplayMemberPath();
            this.ResetOnTemplateChange();
        }

        /// <summary>
        /// Occurs when the <see cref="Telerik.UI.Xaml.Controls.Data.TemplateProviderControl.ItemTemplateSelector" /> property has changed.
        /// </summary>
        protected override void OnItemTemplateSelectorChanged(DataTemplateSelector oldSelector)
        {
            base.OnItemTemplateSelectorChanged(oldSelector);
            this.itemTemplateSelectorCache = this.ItemTemplateSelector;
            this.ValidateDisplayMemberPath();
            this.ResetOnTemplateChange();
        }

        /// <summary>
        /// Occurs when the <see cref="Telerik.UI.Xaml.Controls.Data.TemplateProviderControl.ItemContainerStyle" /> property has changed.
        /// </summary>
        protected override void OnItemContainerStyleChanged(Style oldStyle)
        {
            base.OnItemContainerStyleChanged(oldStyle);
            this.itemContainerStyleCache = this.ItemContainerStyle;
            this.ResetOnTemplateChange();
        }

        /// <summary>
        /// Occurs when the <see cref="Telerik.UI.Xaml.Controls.Data.TemplateProviderControl.ItemContainerStyleSelector" /> property has changed.
        /// </summary>
        protected override void OnItemContainerStyleSelectorChanged(StyleSelector oldSelector)
        {
            base.OnItemContainerStyleSelectorChanged(oldSelector);
            this.ResetOnTemplateChange();
        }

        private static void OnDisplayMemberPathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadVirtualizingDataControl typedSender = sender as RadVirtualizingDataControl;
            typedSender.displayMemberPathCache = (string)args.NewValue;

            typedSender.ValidateDisplayMemberPath();

            typedSender.OnDisplayMemberPathChanged();
        }

        private static void OnVirtualizationStrategyDefinitionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadVirtualizingDataControl typedSender = sender as RadVirtualizingDataControl;
            typedSender.OnVirtualizationStrategyDefinitionChanged(args);
        }

        partial void InitItemAnimations();

        private void InitVirtualizationStrategy(VirtualizationStrategyDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(VirtualizationStrategyDefinition));
            }

            this.RecycleAllItems();
            this.ClearReycledItems();

            if (this.virtualizationStrategyDefinition != null)
            {
                this.UpdateStrategyDefinitionPropertyChangedHandling(false);
                this.virtualizationStrategy.ResetOwner();
            }

            this.virtualizationStrategyDefinition = definition;
            this.virtualizationStrategy = this.virtualizationStrategyDefinition.CreateStrategy();
            this.virtualizationStrategy.InitializeOwner(this);

            if (this.IsLoaded)
            {
                this.UpdateStrategyDefinitionPropertyChangedHandling(true);
            }

            this.ResetScrollViewer();
            this.InvalidateMeasure();
            this.InvalidateArrange();
            this.ScheduleAsyncBalance();
            this.BeginAsyncBalance();
        }

        private void ValidateDisplayMemberPath()
        {
            if (!string.IsNullOrEmpty(this.displayMemberPathCache) &&
                (this.itemTemplateCache != null || this.itemTemplateSelectorCache != null))
            {
                throw new NotSupportedException(NotSupportedExceptionText);
            }
        }

        private void OnVirtualizingDataControl_LayoutUpdated(object sender, object e)
        {
            this.LayoutUpdated -= this.OnVirtualizingDataControl_LayoutUpdated;
            this.BeginAsyncBalance();
            this.BalanceVisualSpace();

            this.layoutUpdated = true;
        }

        private void UpdateStrategyDefinitionPropertyChangedHandling(bool subscribe)
        {
            this.virtualizationStrategyDefinition.PropertyChanged -= this.OnVirtualizationStrategyDefinitionPropertyChanged;
            if (subscribe)
            {
                this.virtualizationStrategyDefinition.PropertyChanged += this.OnVirtualizationStrategyDefinitionPropertyChanged;
            }
        }

        private void OnVirtualizationStrategyDefinitionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                this.virtualizationStrategyDefinition.SynchStrategyProperties(this.virtualizationStrategy);
            }
        }
    }
}