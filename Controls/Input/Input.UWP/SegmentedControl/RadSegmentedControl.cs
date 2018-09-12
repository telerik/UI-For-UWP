using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Input;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Input
{
    [ContentProperty(Name = "Items")]
    [TemplatePart(Name = AnimationLayerTemplatePartName, Type = typeof(Canvas))]
    [TemplatePart(Name = ItemsPanelTemplatePartName, Type = typeof(SegmentedItemsControl))]
    public partial class RadSegmentedControl : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(RadSegmentedControl), new PropertyMetadata(new CornerRadius(0d)));

        /// <summary>
        /// Identifies the <see cref="DisplayMemberPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(nameof(DisplayMemberPath), typeof(string), typeof(RadSegmentedControl), new PropertyMetadata(null, OnDisplayMemberPathChanged));

        /// <summary>
        /// Identifies the <see cref="ItemCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemCommandProperty =
            DependencyProperty.Register(nameof(ItemCommand), typeof(ICommand), typeof(RadSegmentedControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ItemContainerStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register(nameof(ItemContainerStyle), typeof(Style), typeof(RadSegmentedControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ItemContainerStyleSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleSelectorProperty =
            DependencyProperty.Register(nameof(ItemContainerStyleSelector), typeof(StyleSelector), typeof(RadSegmentedControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ItemsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(RadSegmentedControl), new PropertyMetadata(null, OnItemsSourceChanged));

        /// <summary>
        /// Identifies the <see cref="ItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(RadSegmentedControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ItemTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register(nameof(ItemTemplateSelector), typeof(DataTemplateSelector), typeof(RadSegmentedControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SegmentWidthMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentWidthModeProperty =
            DependencyProperty.Register(nameof(SegmentWidthMode), typeof(SegmentWidthMode), typeof(RadSegmentedControl), new PropertyMetadata(SegmentWidthMode.Equal));

        /// <summary>
        /// Identifies the <see cref="SeparatorWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SeparatorWidthProperty =
            DependencyProperty.Register(nameof(SeparatorWidth), typeof(double), typeof(RadSegmentedControl), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the <see cref="SeparatorBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SeparatorBrushProperty =
            DependencyProperty.Register(nameof(SeparatorBrush), typeof(Brush), typeof(RadSegmentedControl), new PropertyMetadata(null));

        private const string AnimationLayerTemplatePartName = "PART_AnimationLayer";
        private const string ItemsPanelTemplatePartName = "PART_ItemsPanel";
        private readonly IList<object> itemsCache;
        private IList<int> disabledItemsCache;

        private bool isInternalChange;
        private SegmentedItemsControl itemsControl;
        private Canvas animationLayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadSegmentedControl"/> class.
        /// </summary>
        public RadSegmentedControl()
        {
            this.DefaultStyleKey = typeof(RadSegmentedControl);
            this.itemsCache = new List<object>();
            this.disabledItemsCache = new List<int>();
        }

        /// <summary>
        /// Occurs when the layout slot or the visual state of a child <see cref="Segment"/> has changed.
        /// </summary>
        public event EventHandler<SegmentAnimationContextEventArgs> SegmentAnimationContextChanged;

        /// <summary>
        /// Gets or sets the corner radius of the control outer border.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { this.SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets a path to a value on the source object to serve as the visual representation
        /// of the object.
        /// </summary>
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { this.SetValue(DisplayMemberPathProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command that is executed when an auto-generated segment is pressed.
        /// </summary>
        public ICommand ItemCommand
        {
            get { return (ICommand)GetValue(ItemCommandProperty); }
            set { this.SetValue(ItemCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Style that is applied to the container element generated for each item.
        /// </summary>
        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { this.SetValue(ItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets custom style-selection logic for a style that can be applied to each
        /// generated container element.
        /// </summary>
        public StyleSelector ItemContainerStyleSelector
        {
            get { return (StyleSelector)GetValue(ItemContainerStyleSelectorProperty); }
            set { this.SetValue(ItemContainerStyleSelectorProperty, value); }
        }

        /// <summary>
        /// Gets the collection used to generate the content of the <see cref="RadSegmentedControl"/>.
        /// </summary>
        public IList<object> Items
        {
            get
            {
                return this.IsTemplateApplied ? this.itemsControl.Items : this.itemsCache;
            }
        }

        /// <summary>
        /// Gets or sets a collection used to generate the content of the <see cref="RadSegmentedControl"/>.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display each item.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { this.SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom logic for choosing a template used to display each item.
        /// </summary>
        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { this.SetValue(ItemTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width mode of the segments.
        /// </summary>
        public SegmentWidthMode SegmentWidthMode
        {
            get { return (SegmentWidthMode)GetValue(SegmentWidthModeProperty); }
            set { this.SetValue(SegmentWidthModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the separators between the segments.
        /// </summary>
        public double SeparatorWidth
        {
            get { return (double)GetValue(SeparatorWidthProperty); }
            set { this.SetValue(SeparatorWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the brush that defines the background of the separators between the segments.
        /// </summary>
        public Brush SeparatorBrush
        {
            get { return (Brush)GetValue(SeparatorBrushProperty); }
            set { this.SetValue(SeparatorBrushProperty, value); }
        }

        /// <summary>
        /// Gets a base layer where the end-user can add and animate custom elements utilizing the <see cref="SegmentAnimationContextChanged"/> event.
        /// </summary>
        public Canvas AnimationLayer
        {
            get
            {
                return this.animationLayer;
            }
        }

        /// <summary>
        /// Gets the <see cref="ItemsControl"/>. Exposed for testing purposes.
        /// </summary>
        internal ItemsControl ItemsControl
        {
            get
            {
                return this.itemsControl;
            }
        }

        /// <summary>
        /// Set the Enable state of the Segment.
        /// </summary>
        public void SetSegmentEnabled(int index, bool isEnabled)
        {
            if (this.IsTemplateApplied)
            {
                this.UpdateSegmentIsEnabled(index, isEnabled);
            }
            else
            {
                bool containsItem = this.disabledItemsCache.Contains(index);

                if (!containsItem && !isEnabled)
                {
                    this.disabledItemsCache.Add(index);
                }
                else if (containsItem && isEnabled)
                {
                    this.disabledItemsCache.Remove(index);
                }
            }
        }

        /// <summary>
        /// Returns <see cref="Boolean"/> value that indicates if the Segment is enabled.
        /// </summary>
        public bool IsSegmentEnabled(int index)
        {
            return this.IsTemplateApplied ? this.GetContainerForIndex(index).IsEnabled : !this.disabledItemsCache.Contains(index);
        }
        
        internal void OnSegmentAnimationContextChanged(Segment segment)
        {
            var handler = this.SegmentAnimationContextChanged;
            if (handler != null)
            {
                handler(this, new SegmentAnimationContextEventArgs(segment));
            }
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadSegmentedControlAutomationPeer(this);
        }

        /// <inheritdoc/>
        protected override void OnIsEnabledChanged(bool newValue, bool oldValue)
        {
            base.OnIsEnabledChanged(newValue, oldValue);
            if (this.IsTemplateApplied)
            {
                foreach (var item in this.itemsControl.ItemsPanelRoot.Children)
                {
                    var segment = item as Segment;
                    if (segment != null)
                    {
                        segment.IsParentEnabled = newValue;
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override bool ApplyTemplateCore()
        {
            var isApplied = base.ApplyTemplateCore();

            this.itemsControl = this.GetTemplatePartField<SegmentedItemsControl>(ItemsPanelTemplatePartName);
            isApplied &= this.itemsControl != null;

            this.animationLayer = this.GetTemplatePartField<Canvas>(AnimationLayerTemplatePartName);
            isApplied &= this.animationLayer != null;

            return isApplied;
        }

        /// <inheritdoc/>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.itemsControl.Owner = this;

            foreach (var item in this.itemsCache)
            {
                this.itemsControl.Items.Add(item);
            }

            this.itemsCache.Clear();

            this.itemsControl.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = this, Path = new PropertyPath("ItemsSource") });

            var temp = this.Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal, 
                () =>
                {
                    foreach (var disabledItem in this.disabledItemsCache)
                    {
                        this.UpdateSegmentIsEnabled(disabledItem, false);
                    }

                    this.disabledItemsCache.Clear();
                });
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            this.itemsControl.Owner = null;
            this.itemsControl.UnapplyTemplate();

            var observableCollection = this.ItemsSource as INotifyCollectionChanged;
            if (observableCollection != null)
            {
                observableCollection.CollectionChanged -= this.ItemsSourceCollectionChanged;
            }

            base.UnapplyTemplateCore();
        }

        private static void OnDisplayMemberPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadSegmentedControl;
            if (control.IsTemplateApplied)
            {
                control.itemsControl.PrepareContainers();

                var container = control.itemsControl.ContainerFromItem(control.SelectedItem) as Segment;
                if (container != null)
                {
                    container.IsSelected = true;
                }
            }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadSegmentedControl;

            control.ClearSelection();

            var oldObservableCollection = e.OldValue as INotifyCollectionChanged;
            if (oldObservableCollection != null)
            {
                oldObservableCollection.CollectionChanged -= control.ItemsSourceCollectionChanged;
            }

            var observableCollection = e.NewValue as INotifyCollectionChanged;
            if (observableCollection != null)
            {
                observableCollection.CollectionChanged += control.ItemsSourceCollectionChanged;
            }
        }

        private void UpdateSegmentIsEnabled(int index, bool isEnabled)
        {
            var item = this.GetContainerForIndex(index);
            item.IsEnabled = isEnabled;
            item.UpdateVisualState(true);
        }

        private Segment GetContainerForIndex(int index)
        {
            return this.GetContainerForValue(this.GetItemByIndex(index));
        }

        private Segment GetContainerForValue(object value)
        {
            if (this.IsTemplateApplied)
            {
                foreach (var sourceItem in this.itemsControl.Items)
                {
                    object propertyValue;
                    if (sourceItem.TryGetPropertyValue(this.SelectedValuePath, out propertyValue))
                    {
                        if ((value == null && propertyValue == null) || (value != null && value.Equals(propertyValue)))
                        {
                            return this.itemsControl.ContainerFromItem(sourceItem) as Segment;
                        }
                    }
                }

                return null;
            }
            else
            {
                throw new InvalidOperationException("Template should be applied first.");
            }
        }

        private int GetIndexByItem(object item)
        {
            var source = this.IsTemplateApplied ? this.itemsControl.Items : this.ItemsSource ?? this.Items;

            // TODO: create extension method to get the index
            var i = 0;
            foreach (var sourceItem in source)
            {
                if (item != null && item.Equals(sourceItem))
                {
                    return i;
                }
                i++;
            }

            return -1;
        }

        private object GetItemByIndex(int index)
        {
            if (this.ItemsSource != null)
            {
                var i = 0;
                foreach (var item in this.ItemsSource)
                {
                    if (i == index)
                    {
                        return item;
                    }
                    i++;
                }
            }
            else if (this.Items.Count > index)
            {
                return this.Items[index];
            }

            return null;
        }

        private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.IsTemplateApplied)
            {
                (sender as INotifyCollectionChanged).CollectionChanged -= this.ItemsSourceCollectionChanged;
            }
            else
            {
                if (e.OldItems.Count > 0 && e.OldItems[0].Equals(this.selectedItemCache))
                {
                    this.ClearSelection();
                }
            }
        }

        private bool TryGetItemByValue(object value, out object item)
        {
            var source = this.IsTemplateApplied ? this.itemsControl.Items : this.ItemsSource ?? this.Items;

            foreach (var sourceItem in source)
            {
                object property = null;
                if (sourceItem.TryGetPropertyValue(this.SelectedValuePath, out property))
                {
                    if ((value == null && property == null) || (value != null && value.Equals(property)))
                    {
                        item = sourceItem;
                        return true;
                    }
                }
            }

            item = null;
            return false;
        }
    }
}
