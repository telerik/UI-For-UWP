using System;
using System.Collections.Specialized;
using Telerik.Core.Data;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Data
{
    public partial class RadDataBoundListBox
    {
        /// <summary>
        /// Identifies the <see cref="CheckModeDeactivatedOnBackButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CheckModeDeactivatedOnBackButtonProperty =
            DependencyProperty.Register(nameof(CheckModeDeactivatedOnBackButton), typeof(bool), typeof(RadDataBoundListBox), new PropertyMetadata(true, OnCheckModeDeactivatedOnBackButtonChanged));

        /// <summary>
        /// Identifies the <see cref="IsCheckModeEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCheckModeEnabledProperty =
            DependencyProperty.Register(nameof(IsCheckModeEnabled), typeof(bool), typeof(RadDataBoundListBox), new PropertyMetadata(false, OnIsCheckModeEnabledChanged));

        /// <summary>
        /// Identifies the <see cref="IsCheckModeActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCheckModeActiveProperty =
            DependencyProperty.Register(nameof(IsCheckModeActive), typeof(bool), typeof(RadDataBoundListBox), new PropertyMetadata(false, OnIsCheckModeActiveChanged));

        /// <summary>
        /// Identifies the <see cref="ItemCheckedPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemCheckedPathProperty =
            DependencyProperty.Register(nameof(ItemCheckedPath), typeof(string), typeof(RadDataBoundListBox), new PropertyMetadata(null, OnItemCheckedPathChanged));

        /// <summary>
        /// Identifies the <see cref="CheckBoxesIndicatorStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CheckBoxesIndicatorStyleProperty =
            DependencyProperty.Register(nameof(CheckBoxesIndicatorStyle), typeof(Style), typeof(RadDataBoundListBox), new PropertyMetadata(null));

        internal const int ShowCheckBoxesThreshold = 35;
        internal const int CheckBoxesTranslationAmount = 80;
        internal const int CheckBoxesIndicatorLength = 24;
        internal const int CheckBoxesIndicatorLengthMargin = 24;
        internal bool isCheckModeActive = false;
        internal bool isCheckModeEnabled = false;
        internal bool isInternalCheckModeSync = false;
        internal string itemCheckedPathCache = null;

        internal ContentPresenter emptyContentPresenter;

        internal Rectangle checkBoxesPressIndicator;
        internal TranslateTransform indicatorTranslate;

        private const int CheckBoxesIndicatorAnimationDuration = 120;
        private const int CheckBoxesIndicatorAnimationDelay = 50;
        private const int CheckBoxesTranslationAnimationDuration = 150;

        private DoubleAnimation checkBoxesIndicatorAnimation;
        private Storyboard checkBoxesIndicatorStoryboard;
        private CheckedItemsCollection<object> checkedItems;
        private bool checkModeDeactivatedOnBackButton = true;
        private bool indicatorVisible = false;
        private bool isInternalCheckModeChange = false;

        /// <summary>
        /// Fires when the check mode of the <see cref="RadDataBoundListBox"/> control has changed.
        /// </summary>
        public event EventHandler<IsCheckModeActiveChangedEventArgs> IsCheckModeActiveChanged;

        /// <summary>
        /// Fires when the check mode of the <see cref="RadDataBoundListBox"/> control is about to change.
        /// </summary>
        public event EventHandler<IsCheckModeActiveChangingEventArgs> IsCheckModeActiveChanging;

        /// <summary>
        /// Fires when an item is about to be checked or unchecked.
        /// </summary>
        public event EventHandler<ItemCheckedStateChangingEventArgs> ItemCheckedStateChanging;

        /// <summary>
        /// Fires when an item has been checked or unchecked.
        /// </summary>
        public event EventHandler<ItemCheckedStateChangedEventArgs> ItemCheckedStateChanged;

        /// <summary>
        /// Gets or sets an instance of the <see cref="Style"/> class
        /// that represents the style applied to the indicator
        /// shown when the user activates the check mode by tapping on the
        /// left edge of a visual container.
        /// </summary>
        public Style CheckBoxesIndicatorStyle
        {
            get
            {
                return this.GetValue(CheckBoxesIndicatorStyleProperty) as Style;
            }
            set
            {
                this.SetValue(CheckBoxesIndicatorStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets a collection containing the currently checked data items. You can
        /// use this collection to check/uncheck items by adding/removing them.
        /// </summary>
        public CheckedItemsCollection<object> CheckedItems
        {
            get
            {
                return this.checkedItems;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether checkboxes
        /// can be displayed on the right side of the visual items by tapping at the left edge of a visual item.
        /// This will allow the end user to check/uncheck items. Checked items will be available
        /// in the CheckedItems collection.
        /// </summary>
        public bool IsCheckModeEnabled
        {
            get
            {
                return this.isCheckModeEnabled;
            }
            set
            {
                this.SetValue(IsCheckModeEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the end user can hide the checkboxes by
        /// pressing the back button of the phone.
        /// </summary>
        public bool CheckModeDeactivatedOnBackButton
        {
            get
            {
                return this.checkModeDeactivatedOnBackButton;
            }
            set
            {
                this.SetValue(CheckModeDeactivatedOnBackButtonProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether checkboxes will be shown
        /// next to the visual items allowing for multiple selection of items.
        /// </summary>
        public bool IsCheckModeActive
        {
            get
            {
                return this.isCheckModeActive;
            }
            set
            {
                this.SetValue(IsCheckModeActiveProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a path that describes a property on each data item
        /// from the original data source used to populate the <see cref="RadDataBoundListBox"/> control which 
        /// defines the checked state of the item.
        /// </summary>
        public string ItemCheckedPath
        {
            get
            {
                return this.itemCheckedPathCache;
            }

            set
            {
                this.SetValue(ItemCheckedPathProperty, value);
            }
        }

        internal override RadListSource CreateListSource()
        {
            RadListSource listSource = base.CreateListSource();
            listSource.ItemFactory = this.listSourceFactory = new DataBoundListBoxListSourceItemFactory(this);
            listSource.ItemPropertyChanged += this.OnListSource_ItemPropertyChanged;
            return listSource;
        }

        /// <summary>
        /// Called when the checked state of a <see cref="DataSourceItem"/> changes.
        /// Allows for performing internal operations associated with checked state
        /// changes of the given item.
        /// </summary>
        /// <param name="item">The <see cref="DataSourceItem"/> which checked state has been changed.</param>
        /// <param name="operation">Determines the type of CheckedItemsCollection item operation.</param>
        internal virtual void OnItemCheckedStateUpdatedCore(DataSourceItem item, CheckedItemsCollectionOperation operation)
        {
        }

        /// <summary>
        /// Fires the <see cref="RadDataBoundListBox.ItemCheckedStateChanging"/> event.
        /// </summary>
        /// <param name="item">The item which checked state is about to be changed.</param>
        /// <param name="isChecked">True if the item will be checked, otherwise false.</param>
        internal virtual bool OnItemCheckedStateChanging(object item, bool isChecked)
        {
            bool canceled = false;

            if (this.ItemCheckedStateChanging != null)
            {
                ItemCheckedStateChangingEventArgs args = new ItemCheckedStateChangingEventArgs(item, isChecked);
                this.ItemCheckedStateChanging(this, args);
                canceled = args.Cancel;
            }

            return canceled;
        }

        /// <summary>
        /// Fires the <see cref="RadDataBoundListBox.ItemCheckedStateChanged"/> event.
        /// </summary>
        /// <param name="item">The item which checked state has changed.</param>
        /// <param name="isChecked">True if the item has been checked, otherwise false.</param>
        internal virtual void OnItemCheckedStateChanged(object item, bool isChecked)
        {
            if (this.ItemCheckedStateChanged != null)
            {
                ItemCheckedStateChangedEventArgs args = new ItemCheckedStateChangedEventArgs(item, isChecked);
                this.ItemCheckedStateChanged(this, args);
            }
        }

        /// <summary>
        /// Handles check toggle request from a child visual item.
        /// </summary>
        internal virtual void HandleItemCheckStateChange(RadDataBoundListBoxItem item)
        {
            if (this.checkedItems.IsReadOnly)
            {
                return;
            }

            if (!item.IsChecked)
            {
                this.AddCheckedItem(item.associatedDataItem as DataSourceItem);
            }
            else
            {
                this.RemoveCheckedItem(item.associatedDataItem as DataSourceItem);
            }
        }

        internal void ShowCheckboxesPressIndicator(RadDataBoundListBoxItem targetItem)
        {
            if (this.indicatorVisible)
            {
                return;
            }

            this.indicatorVisible = true;
            Rect coordinates = targetItem.GetCheckBoxesIndicatorRectangleForItem(this.checkBoxesPressIndicator.Margin);
            this.checkBoxesPressIndicator.Height = coordinates.Height;
            this.checkBoxesPressIndicator.Width = coordinates.Width;
            this.indicatorTranslate.Y = coordinates.Y;
            this.indicatorTranslate.X = coordinates.X;
            this.checkBoxesIndicatorAnimation.To = 1.0;
            this.checkBoxesIndicatorStoryboard.Begin();
        }

        internal void ToggleIndicatorOffset()
        {
            if (!this.IsOperational())
            {
                return;
            }

            RadVirtualizingDataControlItem[] containers = this.RealizedContainers;

            foreach (RadDataBoundListBoxItem container in containers)
            {
                container.SynchItemCheckBoxState();
            }
        }

        internal void ToggleCheckBoxesVisibility()
        {
            this.HideCheckBoxesPressIndicator();

            RadVirtualizingDataControlItem[] containers = this.RealizedContainers;
            foreach (RadDataBoundListBoxItem item in containers)
            {
                item.SynchItemCheckBoxState();
            }

            if (this.isCheckModeActive)
            {
                this.HookRootVisualBackKeyPress();
            }
            else
            {
                this.UnhookRootVisualBackKeyPress();
            }
        }

        internal void HideCheckBoxesPressIndicator()
        {
            if (!this.indicatorVisible)
            {
                return;
            }

            this.indicatorVisible = false;
            this.checkBoxesIndicatorAnimation.To = 0.0;
            this.checkBoxesIndicatorStoryboard.Begin();
        }

        internal virtual bool IsCheckModeArea(RadDataBoundListBoxItem item, UIElement container, Point hitPoint)
        {
            if (!ElementTreeHelper.IsElementRendered(item) || !ElementTreeHelper.IsElementRendered(container as FrameworkElement))
            {
                return false;
            }

            Point relativeOffset = container.TransformToVisual(item).TransformPoint(new Point(0, 0));
            hitPoint = new Point(relativeOffset.X + hitPoint.X, relativeOffset.Y + hitPoint.Y);
            return item.IsPointInCheckModeAreaForItem(hitPoint);
        }

        internal virtual bool IsItemCheckable(RadDataBoundListBoxItem item)
        {
            if (item.associatedDataItem == RadDataBoundListBox.ListHeaderIndicator ||
                item.associatedDataItem == RadDataBoundListBox.ListFooterIndicator ||
                item.associatedDataItem == RadDataBoundListBox.IncrementalLoadingIndicator)
            {
                return false;
            }

            return true;
        }

        internal virtual void ResetContainersCheckableState()
        {
            RadVirtualizingDataControlItem[] containers = this.RealizedContainers;

            foreach (RadDataBoundListBoxItem container in containers)
            {
                container.isItemCheckable = this.IsItemCheckable(container);
            }
        }

        internal void AddCheckedItem(DataSourceItem item)
        {
            this.checkedItems.Add(item);
        }

        internal void RemoveCheckedItem(DataSourceItem item)
        {
            this.checkedItems.Remove(item);
        }

        private static void OnIsCheckModeActiveChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;
            typedSender.OnIsCheckModeActiveChanged(args);
        }

        private static void OnCheckModeDeactivatedOnBackButtonChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;
            typedSender.OnCheckModeDeactivatedOnBackButtonChanged(args);
        }

        private static void OnIsCheckModeEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;
            typedSender.OnIsCheckModeEnabledChanged(args);
        }

        private static void OnItemCheckedPathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadDataBoundListBox typedSender = sender as RadDataBoundListBox;
            typedSender.OnItemCheckedPathChanged(args);
        }

        private void PrepareCheckboxesSupport()
        {
            this.checkBoxesIndicatorAnimation = new DoubleAnimation();
            this.checkBoxesIndicatorAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(CheckBoxesIndicatorAnimationDuration));
            this.checkBoxesIndicatorAnimation.EnableDependentAnimation = true;
            this.checkBoxesIndicatorStoryboard = new Storyboard();
            this.checkBoxesIndicatorStoryboard.BeginTime = TimeSpan.FromMilliseconds(CheckBoxesIndicatorAnimationDelay);
            this.checkBoxesIndicatorStoryboard.Children.Add(this.checkBoxesIndicatorAnimation);
        }

        private void OnCheckModeDeactivatedOnBackButtonChanged(DependencyPropertyChangedEventArgs args)
        {
            this.checkModeDeactivatedOnBackButton = (bool)args.NewValue;

            if (this.isCheckModeActive)
            {
                if (this.checkModeDeactivatedOnBackButton)
                {
                    this.HookRootVisualBackKeyPress();
                }
                else
                {
                    this.UnhookRootVisualBackKeyPress();
                }
            }
        }

        private void OnIsCheckModeEnabledChanged(DependencyPropertyChangedEventArgs args)
        {
            this.isCheckModeEnabled = (bool)args.NewValue;

            this.ToggleIndicatorOffset();
        }

        private void OnIsCheckModeActiveChanged(DependencyPropertyChangedEventArgs args)
        {
            bool newValue = (bool)args.NewValue;
            this.isCheckModeActive = newValue;

            if (this.isInternalCheckModeChange)
            {
                return;
            }

            bool canceled = this.FireCheckModeChanging(null);

            if (canceled)
            {
                this.isInternalCheckModeChange = true;
                this.SetValue(IsCheckModeActiveProperty, (bool)args.OldValue);
                this.isInternalCheckModeChange = false;
                return;
            }

            //// If there are realized items we play an animation
            //// which shows the checkboxes, otherwise we simply set the X translate parameter
            //// of the TranslateTransform object to the needed amount so that when items are 
            //// realized later their checkbox is immediately shown.
            this.ToggleCheckBoxesVisibility();

            this.FireCheckModeChanged(null);
        }

        private bool FireCheckModeChanging(object targetItem)
        {
            if (this.IsCheckModeActiveChanging != null)
            {
                IsCheckModeActiveChangingEventArgs args = new IsCheckModeActiveChangingEventArgs(!this.isCheckModeActive, targetItem);
                this.IsCheckModeActiveChanging(this, args);
                return args.Cancel;
            }

            return false;
        }

        private void FireCheckModeChanged(object targetItem)
        {
            if (this.IsCheckModeActiveChanged != null)
            {
                IsCheckModeActiveChangedEventArgs args = new IsCheckModeActiveChangedEventArgs(this.isCheckModeActive, targetItem);
                this.IsCheckModeActiveChanged(this, args);
            }
        }

        private void SynchCheckedItemsOnItemsChanged(NotifyCollectionChangedEventArgs args)
        {
            //// We only need to synch when an item is removed or replaced. In case of adding a new
            //// item we would need to synch if the ItemCheckedPath is defined but since the
            //// ListSoruce's view model factory does this, we omit this check here.
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    DataSourceItem newItem = args.OldItems[0] as DataSourceItem;
                    if (newItem.isChecked)
                    {
                        this.checkedItems.UncheckItemSilently(newItem, false, false);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        if (!string.IsNullOrEmpty(this.itemCheckedPathCache))
                        {
                            newItem = args.NewItems[0] as DataSourceItem;

                            if (newItem != null)
                            {
                                this.listSourceFactory.InitializeItemCheckedState(newItem);
                            }
                        }
                    }
                    break;
            }
        }

        private void OnListSource_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (this.isInternalCheckModeSync)
            {
                return;
            }

            if (e.PropertyName.Equals(this.itemCheckedPathCache))
            {
                DataSourceItem item = this.ListSource.FindItem(e.Item) as DataSourceItem;
                this.listSourceFactory.InitializeItemCheckedState(item);
            }
        }

        private void OnItemCheckedPathChanged(DependencyPropertyChangedEventArgs args)
        {
            this.checkedItems.ClearSilently(false);

            string newValue = args.NewValue as string;
            this.itemCheckedPathCache = newValue;
            this.listSourceFactory.itemCheckedPropInfo = null;
            this.listSourceFactory.sourceItemCheckedPathWritable = null;

            if (string.IsNullOrEmpty(this.itemCheckedPathCache))
            {
                return;
            }

            DataSourceItem item = this.ListSource.GetFirstItem() as DataSourceItem;

            while (item != null)
            {
                this.listSourceFactory.InitializeItemCheckedState(item);

                item = item.Next as DataSourceItem;
            }

            RadVirtualizingDataControlItem[] containers = this.RealizedContainers;

            foreach (RadDataBoundListBoxItem container in containers)
            {
                container.SynchCheckBoxEnabledState();
            }
        }
    }
}
