using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives.HubTile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// RadCycleHubTile can be bound a data source. Each item from the data source is displayed with a slide animation either randomly or in order as well as
    /// horizontally or vertically.
    /// </summary>
    [TemplatePart(Name = "PART_FlipControl", Type = typeof(FlipControl))]
    [TemplatePart(Name = "PART_LayoutRoot", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_Panel", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_FirstItem", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_SecondItem", Type = typeof(ContentPresenter))]
    public class RadCycleHubTile : HubTileBase, IWeakEventListener
    {
        /// <summary>
        /// Identifies the <see cref="ItemsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(RadCycleHubTile), new PropertyMetadata(null, OnItemsSourceChanged));

        /// <summary>
        /// Identifies the <see cref="ItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(RadCycleHubTile), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CycleRandomly"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CycleRandomlyProperty =
            DependencyProperty.Register(nameof(CycleRandomly), typeof(bool), typeof(RadCycleHubTile), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(RadCycleHubTile), new PropertyMetadata(Orientation.Vertical, OnOrientationChanged));

        /// <summary>
        /// Identifies the <see cref="FlipUpdateIntervalsCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FlipUpdateIntervalsCountProperty =
            DependencyProperty.Register(nameof(FlipUpdateIntervalsCount), typeof(int), typeof(RadCycleHubTile), new PropertyMetadata(5, null));

        private static readonly Random RandomGenerator = new Random();
        private int index = -1;
        private bool skipBackState = true;
        private int counter;
        private UIElement panel;
        private FrameworkElement firstContentContainer;

        private Storyboard globalContentAnimation = new Storyboard();
        private DoubleAnimation moveUpGlobalAnimation;

        private Storyboard localContentAnimation = new Storyboard();
        private DoubleAnimation moveUpLocalAnimation;

        private ContentPresenter firstItem;
        private ContentPresenter secondItem;

        private WeakEventHandler<NotifyCollectionChangedEventArgs> collectionChangedEventHandler;
        private IList sourceAsList = new List<object>();

        /// <summary>
        /// Initializes a new instance of the RadCycleHubTile class.
        /// </summary>
        public RadCycleHubTile()
        {
            this.DefaultStyleKey = typeof(RadCycleHubTile);

            this.moveUpGlobalAnimation = new DoubleAnimation();
            this.moveUpGlobalAnimation.Duration = TimeSpan.FromSeconds(0.4);
            this.moveUpGlobalAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseInOut };
            this.moveUpGlobalAnimation.From = 0;
            Storyboard.SetTargetProperty(this.moveUpGlobalAnimation, "(Canvas.Top)");

            this.moveUpLocalAnimation = new DoubleAnimation();
            this.moveUpLocalAnimation.BeginTime = TimeSpan.FromSeconds(0.3);
            this.moveUpLocalAnimation.Duration = TimeSpan.FromSeconds(8);
            this.moveUpLocalAnimation.From = 0;
            Storyboard.SetTargetProperty(this.moveUpLocalAnimation, "(Canvas.Top)");

            this.localContentAnimation.Children.Add(this.moveUpLocalAnimation);

            this.globalContentAnimation.Children.Add(this.moveUpGlobalAnimation);
            this.globalContentAnimation.Completed += this.OnCurrentAnimationCompleted;
        }

        /// <summary>
        /// Gets or sets the data source of the cycle tile.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)this.GetValue(RadCycleHubTile.ItemsSourceProperty);
            }

            set
            {
                this.SetValue(RadCycleHubTile.ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the item template of the cycle tile.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(RadCycleHubTile.ItemTemplateProperty);
            }

            set
            {
                this.SetValue(RadCycleHubTile.ItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the cycle animation.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(RadCycleHubTile.OrientationProperty);
            }

            set
            {
                this.SetValue(RadCycleHubTile.OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the items in the data source will be cycled randomly or not.
        /// </summary>
        public bool CycleRandomly
        {
            get
            {
                return (bool)this.GetValue(RadCycleHubTile.CycleRandomlyProperty);
            }

            set
            {
                this.SetValue(RadCycleHubTile.CycleRandomlyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies how many update intervals have to pass before the tile flips.
        /// </summary>
        public int FlipUpdateIntervalsCount
        {
            get
            {
                return (int)this.GetValue(FlipUpdateIntervalsCountProperty);
            }

            set
            {
                this.SetValue(FlipUpdateIntervalsCountProperty, value);
            }
        }

        /// <summary>
        /// Gets a value of the secondItem displayed. Exposed for testing purposes.
        /// </summary>
        internal ContentPresenter FirstItem
        {
            get
            {
                return this.firstItem;
            }
        }

        /// <summary>
        /// Gets a value of the firstItem displayed. Exposed for testing purposes.
        /// </summary>
        internal ContentPresenter SecondItem
        {
            get
            {
                return this.secondItem;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a rectangle clip is set on the LayoutRoot.
        /// </summary>
        protected override bool ShouldClip
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Determines whether the update timer used to update the tile's VisualState needs to be started.
        /// </summary>
        protected override bool IsUpdateTimerNeeded
        {
            get
            {
                if (this.ItemsSource != null)
                {
                    return true;
                }

                return base.IsUpdateTimerNeeded;
            }
        }

        void IWeakEventListener.ReceiveEvent(object sender, object args)
        {
            if ((args as NotifyCollectionChangedEventArgs).NewStartingIndex == 0)
            {
                this.index = -1;
                this.RefreshItems(false);
            }
            else
            {
                this.RefreshItems(true);
            }
        }

        internal void UpdateItems()
        {
            if (!this.IsTemplateApplied || !this.IsLoaded)
            {
                return;
            }

            this.firstContentContainer.LayoutUpdated += this.OnFirstContentLayoutUpdated;

            if (this.Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
            {
                Canvas.SetTop(this.panel, 0);
                Canvas.SetTop(this.firstContentContainer, 0);
            }
            else
            {
                Canvas.SetLeft(this.panel, 0);
                Canvas.SetLeft(this.firstContentContainer, 0);
            }

            object tmp = this.secondItem.Content;

            this.secondItem.Content = null;
            this.firstItem.Content = tmp;

            this.secondItem.Content = this.GetItem(this.CycleRandomly);
        }

        internal void RefreshItems(bool refreshOnlySecondItem)
        {
            if (!refreshOnlySecondItem)
            {
                this.firstItem.Content = this.GetItem(this.CycleRandomly);
            }
            this.secondItem.Content = this.GetItem(this.CycleRandomly);
        }

        /// <summary>
        /// A virtual callback that is called periodically when the tile is no frozen. It can be used to
        /// update the tile visual states or other necessary operations.
        /// </summary>
        protected internal override void Update(bool animate, bool updateIsFlipped)
        {
            if (!this.skipBackState)
            {
                if (this.counter == this.FlipUpdateIntervalsCount)
                {
                    base.Update(animate, updateIsFlipped);
                    this.counter = 0;
                    return;
                }
                else if (this.IsFlipped)
                {
                    base.Update(animate, updateIsFlipped);
                    return;
                }
            }

            if (this.firstItem == null)
            {
                return;
            }

            // local moveup animation is running, stop it first
            if (this.localContentAnimation.GetCurrentState() == ClockState.Active)
            {
                this.localContentAnimation.SkipToFill();
            }

            if (this.Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
            {
                this.moveUpGlobalAnimation.To = -this.Height;
            }
            else
            {
                this.moveUpGlobalAnimation.To = -this.Width;
            }

            if (this.sourceAsList.Count > 0)
            {
                this.globalContentAnimation.Begin();
            }

            this.counter++;
        }

        /// <summary>
        /// Retrieves the ControlTemplate parts.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.panel = this.GetTemplatePartField<UIElement>("PART_Panel");
            applied = applied && this.panel != null;

            this.firstContentContainer = this.GetTemplatePartField<FrameworkElement>("PART_FirstContent");
            applied = applied && this.firstContentContainer != null;

            this.firstItem = this.GetTemplatePartField<ContentPresenter>("PART_FirstItem");
            applied = applied && this.firstItem != null;

            this.secondItem = this.GetTemplatePartField<ContentPresenter>("PART_SecondItem");
            applied = applied && this.secondItem != null;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate"/> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            Storyboard.SetTarget(this.moveUpLocalAnimation, this.firstContentContainer);
            Storyboard.SetTarget(this.moveUpGlobalAnimation, this.panel);
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Loaded"/> event. Allows inheritors to provide their specific logic.
        /// </summary>
        protected override void LoadCore()
        {
            base.LoadCore();

            if (!this.IsTemplateApplied)
            {
                return;
            }

            this.RefreshItems(false);
        }

        /// <summary>
        /// This callback is invoked when BackContent is set to a non-null value.
        /// </summary>
        protected override void OnBackStateActivated()
        {
            base.OnBackStateActivated();
            this.skipBackState = false;
        }

        /// <summary>
        /// This callback is invoked when BackContent is set to a null value.
        /// </summary>
        protected override void OnBackStateDeactivated()
        {
            base.OnBackStateDeactivated();
            this.skipBackState = true;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadCycleHubTileAutomationPeer(this);
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadCycleHubTile tile = d as RadCycleHubTile;
            string path;
            Orientation orientation = (Orientation)e.NewValue;
            if (orientation == Orientation.Horizontal)
            {
                path = "(Canvas.Left)";
            }
            else
            {
                path = "(Canvas.Top)";
            }
            tile.globalContentAnimation.Stop();
            tile.localContentAnimation.Stop();
            Storyboard.SetTargetProperty(tile.moveUpGlobalAnimation, path);
            Storyboard.SetTargetProperty(tile.moveUpLocalAnimation, path);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadCycleHubTile tile = d as RadCycleHubTile;

            IList sourceAsList = e.NewValue as IList;
            IEnumerable sourceAsIEnumerable = e.NewValue as IEnumerable;
            INotifyCollectionChanged sourceAsCollectionChanged = e.NewValue as INotifyCollectionChanged;
            if (sourceAsList != null)
            {
                tile.sourceAsList = sourceAsList;
            }
            else
            {
                tile.sourceAsList = new List<object>();

                if (sourceAsIEnumerable != null)
                {
                    foreach (var item in sourceAsIEnumerable)
                    {
                        tile.sourceAsList.Add(item);
                    }
                }
            }
            tile.UpdateTimerState();
            tile.index = -1;

            tile.UnsubscribeFromColectionChanged();
            if (sourceAsCollectionChanged != null)
            {
                tile.collectionChangedEventHandler = new WeakEventHandler<NotifyCollectionChangedEventArgs>(sourceAsCollectionChanged, (IWeakEventListener)tile, KnownEvents.CollectionChanged);
            }
        }

        private void UnsubscribeFromColectionChanged()
        {
            if (this.collectionChangedEventHandler != null)
            {
                this.collectionChangedEventHandler.Unsubscribe();
                this.collectionChangedEventHandler = null;
            }
        }

        private object GetItem(bool random)
        {
            if (this.sourceAsList.Count < 1)
            {
                return null;
            }

            if (random)
            {
                return this.GetRandomItem();
            }

            return this.GetNextItem();
        }

        private object GetRandomItem()
        {
            this.index = RandomGenerator.Next(this.sourceAsList.Count);
            return this.sourceAsList[this.index];
        }

        private object GetNextItem()
        {
            this.index++;
            return this.sourceAsList[this.index % this.sourceAsList.Count];
        }

        private void OnCurrentAnimationCompleted(object sender, object args)
        {
            this.UpdateItems();
        }

        private void OnFirstContentLayoutUpdated(object sender, object args)
        {
            this.firstContentContainer.LayoutUpdated -= this.OnFirstContentLayoutUpdated;

            if (this.sourceAsList.Count < 1)
            {
                return;
            }
            if (this.Orientation == Orientation.Vertical)
            {
                double heightDiff = this.firstContentContainer.DesiredSize.Height - this.Height;
                if (heightDiff < 0)
                {
                    return;
                }

                if (heightDiff > this.Height)
                {
                    heightDiff = this.Height;
                }

                this.moveUpLocalAnimation.To = -heightDiff;
            }
            else
            {
                double widthDiff = this.firstContentContainer.DesiredSize.Width - this.Width;
                if (widthDiff < 0)
                {
                    return;
                }

                if (widthDiff > this.Width)
                {
                    widthDiff = this.Width;
                }

                this.moveUpLocalAnimation.To = -widthDiff;
            }

            if (this.sourceAsList.Count > 0)
            {
                this.localContentAnimation.Begin();
            }
        }
    }
}
